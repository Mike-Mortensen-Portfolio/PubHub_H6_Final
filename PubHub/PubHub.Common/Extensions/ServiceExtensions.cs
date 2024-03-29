using Microsoft.Extensions.DependencyInjection;
using PubHub.Common.ApiService;
using PubHub.Common.Services;
using System.Net.Http;

namespace PubHub.Common.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddPubHubServices(this IServiceCollection services, ApiOptions apiOptions)
        {
            if (apiOptions == null) 
                throw new ArgumentNullException(nameof(services), "Options cannot be empty.");

            return services.AddApiService(apiOptions);
        }

        private static IServiceCollection AddApiService(this IServiceCollection services, ApiOptions apiOptions)
        {
            if (apiOptions == null)
                throw new ArgumentNullException(nameof(services), "Options cannot be empty.");

            return services.AddTransient(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                return new PubHubApiService(factory, apiOptions.HttpClientName ?? "PubHubApi");
            }).AddHttpClient(apiOptions.HttpClientName ?? "PubHubApi", options => { options.BaseAddress = new Uri(apiOptions.Address); }).Services;
        }
    }
}
