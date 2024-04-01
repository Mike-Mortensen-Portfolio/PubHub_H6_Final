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

            return services
                .AddScopedApiService<IUserService, UserService>(apiOptions)
                .AddScopedApiService<IBookService, BookService>(apiOptions)
                .AddScopedApiService<IPublisherService, PublisherService>(apiOptions)
                .AddHttpClient(apiOptions.HttpClientName ?? "PubHubApi", options => { options.BaseAddress = new Uri(apiOptions.Address); }).Services;
        }

        private static IServiceCollection AddScopedApiService<TService, TConcrete>(this IServiceCollection services, ApiOptions apiOptions) where TConcrete : ServiceRoot, TService where TService : class
        {
            return services.AddScoped<TService, TConcrete>(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                var instance = Activator.CreateInstance(typeof(TConcrete), [factory, apiOptions.HttpClientName ?? "PubHubApi"]) as TConcrete;
                return instance!;
            });
        }
    }    
}
