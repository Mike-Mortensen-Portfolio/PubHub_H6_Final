using System.Diagnostics;
using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Simmy;
using Polly.Timeout;
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

            services.AddScoped<Func<Task<TokenInfo>>>((sp) => () => apiOptions.GetTokenInfoAsync.Invoke(sp));
            services.AddScoped<Action<TokenInfo>>((sp) => (tokenInfo) => apiOptions.SetTokenInfo.Invoke(sp, tokenInfo));
            services.AddScoped<Action>((sp) => () => apiOptions.RemoveTokenInfo.Invoke(sp));

            // Add HttpClient for specified platform.
            var uri = new Uri(apiOptions.Address);
            if (apiOptions.ConfigureForMobile)
            {
                HttpClient httpClient = new() { BaseAddress = uri };
                httpClient.DefaultRequestHeaders.Add(ApiConstants.APP_ID, apiOptions.AppId);
                services.AddSingleton<IHttpClientService>(sp => new HttpClientService(
                    httpClient,
                    sp.GetRequiredKeyedService<ResiliencePipeline<HttpResponseMessage>>(POLLY_PIPELINE)
                    ));
            }
            else
            {
                var clientName = apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME;
                services.AddScoped<IHttpClientService>(sp => new HttpClientService(
                    sp.GetRequiredService<IHttpClientFactory>().CreateClient(clientName),
                    sp.GetRequiredKeyedService<ResiliencePipeline<HttpResponseMessage>>(POLLY_PIPELINE)
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
                .AddScoped<IEpubReaderService, EpubReaderService>()
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
                builder
                    .AddRetry(new()
                    {
                        ShouldHandle = static args =>
                        {
                            var response = args.Outcome.Result;
                            if (response == null ||
                                response.StatusCode == HttpStatusCode.InternalServerError ||
                                response.StatusCode == HttpStatusCode.NotFound)
                            {
                                // Request failed.
                                return new ValueTask<bool>(true);
                            }

                            // Request succeeded.
                            return new ValueTask<bool>(false);
                        },
                        MaxRetryAttempts = 5,
                        DelayGenerator = static args =>
                        {
                            var delay = args.AttemptNumber switch
                            {
                                0 => TimeSpan.Zero,
                                1 => TimeSpan.FromSeconds(0.2),
                                _ => TimeSpan.FromSeconds(0.2 * Math.Pow(2, args.AttemptNumber))
                            };

                            return new ValueTask<TimeSpan?>(delay);
                        },
                        OnRetry = static args =>
                        {
                            Debug.WriteLine($"Polly: Retrying in {args.RetryDelay} ...");

                            return default;
                        }
                    })
                    .AddTimeout(new TimeoutStrategyOptions()
                    {
                        TimeoutGenerator = static args =>
                        {
                            var contentMegabytes = args.Context.Properties.GetValue(new(ResilienceConstants.CONTENT_MEGABYTES_RESILIENCE_KEY), (decimal)0);
                            double timeoutSeconds = 30;
                            if (contentMegabytes >= 2)
                            {
                                // Content is more than 2 MB; use 5 min. timeout.
                                timeoutSeconds = 300;
                            }

                            return ValueTask.FromResult(TimeSpan.FromSeconds(timeoutSeconds));
                        }
                    })
                    .AddRateLimiter(new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        AutoReplenishment = true,
                        SegmentsPerWindow = 1,
                        QueueLimit = 5,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));

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
