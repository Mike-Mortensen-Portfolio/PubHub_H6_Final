using Polly;
using PubHub.Common.Models.Authentication;
using System.Diagnostics;
using System.Net.Mime;

namespace PubHub.Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private const string BEARER_KEY = "Bearer";

        private readonly HttpClient _client;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;
        private readonly Func<Task<TokenInfo>> _tokenInfoAsync;

        public HttpClientService(HttpClient client, ResiliencePipeline<HttpResponseMessage> resiliencePipeline, Func<Task<TokenInfo>> tokenInfoAsync)
        {
            _client = client;
            _client.DefaultRequestHeaders.Add("refreshToken", string.Empty);
            _resiliencePipeline = resiliencePipeline;
            _tokenInfoAsync = tokenInfoAsync;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                var tokenInfo = await _tokenInfoAsync.Invoke();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, tokenInfo.Token);                    
                _client.DefaultRequestHeaders.Remove("refreshToken"); 
                _client.DefaultRequestHeaders.Add("refreshToken", tokenInfo.RefreshToken);

                return await _client.GetAsync(uri, cancellationToken);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string uri, string? content = null)
        {
            CheckNetwork();            

            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                var tokenInfo = await _tokenInfoAsync.Invoke();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, tokenInfo.Token);
                _client.DefaultRequestHeaders.Remove("refreshToken");
                _client.DefaultRequestHeaders.Add("refreshToken", tokenInfo.RefreshToken);

                return await _client.PostAsync(uri, new StringContent(content ?? string.Empty, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string uri, string? content = null)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                var tokenInfo = await _tokenInfoAsync.Invoke();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, tokenInfo.Token);
                _client.DefaultRequestHeaders.Remove("refreshToken");
                _client.DefaultRequestHeaders.Add("refreshToken", tokenInfo.RefreshToken);

                return await _client.PutAsync(uri, new StringContent(content ?? string.Empty, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json), cancellationToken);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            CheckNetwork();

            return await _resiliencePipeline.ExecuteAsync(async cancellationToken =>
            {
                var tokenInfo = await _tokenInfoAsync.Invoke();
                _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, tokenInfo.Token);
                _client.DefaultRequestHeaders.Remove("refreshToken");
                _client.DefaultRequestHeaders.Add("refreshToken", tokenInfo.RefreshToken);

                return await _client.DeleteAsync(uri, cancellationToken);
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
