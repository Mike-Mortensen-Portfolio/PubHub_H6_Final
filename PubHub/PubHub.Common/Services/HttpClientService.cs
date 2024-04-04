using Polly;
using System.Diagnostics;

namespace PubHub.Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private const int RETRY_COUNT = 5;

        private readonly HttpClient _client;
        private readonly Func<HttpResponseMessage, bool> _retryPredicate = res => res.IsSuccessStatusCode;

        public HttpClientService(string apiBaseUrl)
        {
            _client = new HttpClient() { BaseAddress = new Uri(apiBaseUrl) };
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            CheckNetwork();

            return await Policy
                .HandleResult(_retryPredicate)
                .WaitAndRetryAsync(retryCount: RETRY_COUNT, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(0.2 * Math.Pow(2, retryAttempt)), onRetry: (result, time) =>
                {
                    Debug.WriteLine($"{nameof(GetAsync)}: Retrying in {time} ...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine($"{nameof(GetAsync)}: {_client.BaseAddress}{uri}");
                    return await _client.GetAsync(uri);
                });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent? httpContent = null)
        {
            CheckNetwork();

            return await Policy
                .HandleResult(_retryPredicate)
                .WaitAndRetryAsync(retryCount: RETRY_COUNT, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(0.2 * Math.Pow(2, retryAttempt)), onRetry: (result, time) =>
                {
                    Debug.WriteLine($"{nameof(PostAsync)}: Retrying in {time} ...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine($"{nameof(PostAsync)}: {_client.BaseAddress}{uri}");
                    return await _client.PostAsync(uri, httpContent);
                });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string uri, HttpContent? httpContent = null)
        {
            CheckNetwork();

            return await Policy
                .HandleResult(_retryPredicate)
                .WaitAndRetryAsync(retryCount: RETRY_COUNT, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(0.2 * Math.Pow(2, retryAttempt)), onRetry: (result, time) =>
                {
                    Debug.WriteLine($"{nameof(PutAsync)}: Retrying in {time} ...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine($"{nameof(PutAsync)}: {_client.BaseAddress}{uri}");
                    return await _client.PutAsync(uri, httpContent);
                });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            CheckNetwork();

            return await Policy
                .HandleResult(_retryPredicate)
                .WaitAndRetryAsync(retryCount: RETRY_COUNT, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(0.2 * Math.Pow(2, retryAttempt)), onRetry: (result, time) =>
                {
                    Debug.WriteLine($"{nameof(DeleteAsync)}: Retrying in {time} ...");
                })
                .ExecuteAsync(async () =>
                {
                    Debug.WriteLine($"{nameof(DeleteAsync)}: {_client.BaseAddress}{uri}");
                    return await _client.DeleteAsync(uri);
                });
        }

        /// <summary>
        /// Check if the device has an active internet connection and throw an exception if not.
        /// </summary>
        /// <exception cref="Exception">No internet access.</exception>
        /// <remarks>
        /// Checks are only ran on .NET MAUI.
        /// </remarks>
        private static void CheckNetwork()
        {
#if MAUI
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new Exception("No network access;");
            }
#endif
        }
    }
}
