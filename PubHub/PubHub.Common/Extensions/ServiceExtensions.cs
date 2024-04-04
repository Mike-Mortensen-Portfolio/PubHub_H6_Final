using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PubHub.Common.ApiService;
using PubHub.Common.Services;

namespace PubHub.Common.Extensions
{
    public static class ServiceExtensions
    {
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

            services
                .AddScopedApiService<IUserService, UserService>(apiOptions)
                .AddScopedApiService<IBookService, BookService>(apiOptions)
                .AddScopedApiService<IPublisherService, PublisherService>(apiOptions)
                .AddScopedApiService<IAuthorService, AuthorService>(apiOptions)
                .AddScopedApiService<IGenreService, GenreService>(apiOptions)
                .AddScopedApiService<IAuthenticationService, AuthenticationService>(apiOptions)
                .AddScopedApiService<IContentTypeService, ContentTypeService>(apiOptions);

            // Add HttpClient for specified platform.
            var uri = new Uri(apiOptions.Address);
            if (apiOptions.ConfigureForMobile)
            {
                services.AddSingleton<IHttpClientService>(sp => new HttpClientService(new HttpClient() { BaseAddress = uri }));
            }
            else
            {
                services.AddHttpClient(apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME, options => { options.BaseAddress = uri; });
                services.AddScoped<IHttpClientService>(sp => new HttpClientService(services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient()));
            }

            return services;
        }

        private static IServiceCollection AddScopedApiService<TService, TConcrete>(this IServiceCollection services, ApiOptions apiOptions) where TConcrete : ServiceRoot, TService where TService : class
        {
            return services.AddScoped<TService, TConcrete>(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var instance = Activator.CreateInstance(typeof(TConcrete), [factory, apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME]) as TConcrete;

                return instance!;
            });
        }
    }    
}
