﻿using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Simmy;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.Common.Extensions
{
    public static class ServiceExtensions
    {
        private const string POLLY_PIPELINE = "polly-pipeline";

        public static IServiceCollection AddPubHubServices(this IServiceCollection services, Action<ApiOptions> apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(services), "Options cannot be empty.");

            var options = new ApiOptions();
            apiOptions(options);

            return services.AddApiService(options);
        }

        private static IServiceCollection AddApiService(this IServiceCollection services, ApiOptions apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(services), "Options cannot be empty.");

            services.AddScoped<Func<Task<TokenInfo>>>((sp) => () => apiOptions.TokenInfoAsync.Invoke(sp));

            // Add HttpClient for specified platform.
            var uri = new Uri(apiOptions.Address);
            if (apiOptions.ConfigureForMobile)
            {
                HttpClient httpClient = new() { BaseAddress = uri };
                httpClient.DefaultRequestHeaders.Add(ApiConstants.APP_ID, apiOptions.AppId);
                services.AddSingleton<IHttpClientService>(sp => new HttpClientService(
                    httpClient,
                    sp.GetRequiredKeyedService<ResiliencePipeline<HttpResponseMessage>>(POLLY_PIPELINE),
                    sp.GetRequiredService<Func<Task<TokenInfo>>>()
                    ));
            }
            else
            {
                var clientName = apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME;
                services.AddScoped<IHttpClientService>(sp => new HttpClientService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName),
                    sp.GetRequiredKeyedService<ResiliencePipeline<HttpResponseMessage>>(POLLY_PIPELINE),
                    sp.GetRequiredService<Func<Task<TokenInfo>>>()
                    ));
                services.AddHttpClient(clientName, options =>
                {
                    options.BaseAddress = uri;
                    options.DefaultRequestHeaders.Add(ApiConstants.APP_ID, apiOptions.AppId);
                });
            }

            return services
                .AddScoped<IUserService, UserService>()
                .AddScoped<IBookService, BookService>()
                .AddScoped<IPublisherService, PublisherService>()
                .AddScoped<IAuthorService, AuthorService>()
                .AddScoped<IGenreService, GenreService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IContentTypeService, ContentTypeService>()
                .AddSingleton<IChaosService>(new ChaosService())
                .AddPolly();
        }

        //private static IServiceCollection AddScopedApiService<TService, TConcrete>(this IServiceCollection services, ApiOptions apiOptions) where TConcrete : ServiceRoot, TService where TService : class
        //{
        //    return services.AddScoped<TService, TConcrete>(provider =>
        //    {
        //        var factory = provider.GetRequiredService<IHttpClientFactory>();
        //        var instance = Activator.CreateInstance(typeof(TConcrete), [factory, apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME]) as TConcrete;

        //        return instance!;
        //    });
        //}

        private static IServiceCollection AddPolly(this IServiceCollection services)
        {
            services.AddResiliencePipeline<string, HttpResponseMessage>(POLLY_PIPELINE, (builder, context) =>
            {
                // Configure Polly.
                builder.AddRetry(new()
                {
                    ShouldHandle = static args =>
                    {
                        var result = args.Outcome.Result;
                        if (result == null ||
                            result.StatusCode == HttpStatusCode.InternalServerError ||
                            result.StatusCode == HttpStatusCode.NotFound)
                        {
                            // Request failed.
                            return new ValueTask<bool>(true);
                        }

                        // Request succeeded.
                        return new ValueTask<bool>(false);
                    },
                    MaxRetryAttempts = 5,
                    DelayGenerator = static args => new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(0.2 * Math.Pow(2, args.AttemptNumber))),
                    OnRetry = static args =>
                    {
                        Debug.WriteLine($"Polly: Retrying in {args.RetryDelay} ...");

                        return default;
                    }
                });

                // Configure Simmy.
                var chaosService = context.ServiceProvider.GetRequiredService<IChaosService>();
                builder
                    .AddChaosLatency(new()
                    {
                        EnabledGenerator = args => chaosService.IsLatencyEnabledAsync(args.Context),
                        InjectionRateGenerator = args => chaosService.GetLatencyInjectionRateAsync(args.Context),
                        LatencyGenerator = args => chaosService.GetLatencyAsync(args.Context)
                    })
                    .AddChaosFault(new()
                    {
                        EnabledGenerator = args => chaosService.IsFaultEnabledAsync(args.Context),
                        InjectionRateGenerator = args => chaosService.GetFaultInjectionRateAsync(args.Context),
                        FaultGenerator = args => chaosService.GetFaultAsync(args.Context)
                    })
                    .AddChaosOutcome(new()
                    {
                        EnabledGenerator = args => chaosService.IsOutcomeEnabledAsync(args.Context),
                        InjectionRateGenerator = args => chaosService.GetOutcomeInjectionRateAsync(args.Context),
                        OutcomeGenerator = args => chaosService.GetOutcomeAsync(args.Context)
                    });
            });

            return services;
        }
    }    
}
