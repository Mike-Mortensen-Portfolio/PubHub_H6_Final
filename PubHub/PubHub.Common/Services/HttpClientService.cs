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

        public HttpClient Client => _client;

        /// <inheritdoc/>
        public async Task<HttpResponseMessage?> GetAsync(string uri)
        {
            try
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
                        Debug.WriteLine($"{nameof(GetAsync)}: {Client.BaseAddress}{uri}");
                        return await Client.GetAsync(uri);
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(GetAsync)} failed: {ex.Message}");
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage?> PostAsync(string uri, HttpContent? httpContent = null)
        {
            try
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
                        Debug.WriteLine($"{nameof(PostAsync)}: {Client.BaseAddress}{uri}");
                        return await Client.PostAsync(uri, httpContent);
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(PostAsync)} failed: {ex.Message}");
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage?> PutAsync(string uri, HttpContent? httpContent = null)
        {
            try
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
                        Debug.WriteLine($"{nameof(PutAsync)}: {Client.BaseAddress}{uri}");
                        return await Client.PutAsync(uri, httpContent);
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(PutAsync)} failed: {ex.Message}");
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage?> DeleteAsync(string uri)
        {
            try
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
                        Debug.WriteLine($"{nameof(DeleteAsync)}: {Client.BaseAddress}{uri}");
                        return await Client.DeleteAsync(uri);
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(DeleteAsync)} failed: {ex.Message}");
            }

            return null;
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
