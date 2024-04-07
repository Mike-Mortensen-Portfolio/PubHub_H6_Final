using Polly;

namespace PubHub.Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

        public HttpClientService(HttpClient client, ResiliencePipeline<HttpResponseMessage> resiliencePipeline)
        {
            _client = client;
            _resiliencePipeline = resiliencePipeline;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                return await _client.GetAsync(uri, token);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string uri, HttpContent? httpContent = null)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                return await _client.PostAsync(uri, httpContent, token);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string uri, HttpContent? httpContent = null)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                return await _client.PutAsync(uri, httpContent, token);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                return await _client.DeleteAsync(uri, token);
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
