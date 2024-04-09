using Microsoft.Extensions.DependencyInjection;
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

            // Add HttpClient for specified platform.
            var uri = new Uri(apiOptions.Address);
            if (apiOptions.ConfigureForMobile)
            {
                HttpClient httpClient = new() { BaseAddress = uri };
                httpClient.DefaultRequestHeaders.Add(ApiConstants.APP_ID, apiOptions.AppId);
                services.AddSingleton<IHttpClientService>(sp => new HttpClientService(httpClient));
            }
            else
            {
                var clientName = apiOptions.HttpClientName ?? ApiConstants.HTTPCLIENT_NAME;
                services.AddScoped<IHttpClientService>(sp => new HttpClientService(services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient(clientName)));
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
                .AddScoped<IContentTypeService, ContentTypeService>();
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
    }    
}
