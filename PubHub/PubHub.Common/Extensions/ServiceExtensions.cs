using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using PubHub.Common.ApiService;
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

            // Add HttpClient for specified platform.
            var uri = new Uri(apiOptions.Address);
            if (apiOptions.ConfigureForMobile)
            {
                services.AddSingleton<IHttpClientService>(sp => new HttpClientService(
                    new HttpClient() { BaseAddress = uri },
                    services.BuildServiceProvider().GetRequiredKeyedService<ResiliencePipeline>(POLLY_PIPELINE)));
            }
            else
            {
                var clientName = apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME;
                services.AddScoped<IHttpClientService>(sp => new HttpClientService(
                    services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(clientName),
                    services.BuildServiceProvider().GetRequiredKeyedService<ResiliencePipeline>(POLLY_PIPELINE)));
                services.AddHttpClient(clientName, options => { options.BaseAddress = uri; });
            }

            return services
                .AddScoped<IUserService, UserService>()
                .AddScoped<IBookService, BookService>()
                .AddScoped<IPublisherService, PublisherService>()
                .AddScoped<IAuthorService, AuthorService>()
                .AddScoped<IGenreService, GenreService>()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IContentTypeService, ContentTypeService>()
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
            services.AddResiliencePipeline(POLLY_PIPELINE, builder =>
            {
                builder.AddRetry(new RetryStrategyOptions()
                {
                    ShouldHandle = static args =>
                    {
                        if (args.Outcome.Result is HttpResponseMessage responseMessage &&
                            responseMessage.IsSuccessStatusCode)
                        {
                            // Request succeeded.
                            return new ValueTask<bool>(false);
                        }

                        // Request failed.
                        return new ValueTask<bool>(true);
                    },
                    MaxRetryAttempts = 5,
                    DelayGenerator = static args => new ValueTask<TimeSpan?>(TimeSpan.FromSeconds(0.2 * Math.Pow(2, args.AttemptNumber))),
                    OnRetry = static args =>
                    {
                        Debug.WriteLine($"Polly: Retrying in {args.RetryDelay} ...");

                        return default;
                    }
                });
            });

            return services;
        }
    }    
}
