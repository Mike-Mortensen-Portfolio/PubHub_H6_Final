using Polly;
using Polly.RateLimiting;
using PubHub.Common.Models.Authentication;
using System.Net.Mime;
using System.Text;

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
            _client.Timeout = TimeSpan.Parse("24.20:31:23.6470000"); // Polly will take care of timeout.
            _resiliencePipeline = resiliencePipeline;
            _tokenInfoAsync = tokenInfoAsync;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            CheckNetwork();

            try
            {
                var context = StartRequest();
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
                    await SetTokensAsync();

                    return await _client.GetAsync(uri, context.CancellationToken);
                }, context);
                EndRequest(context);

                return response;
            }
            catch (RateLimiterRejectedException)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.TooManyRequests
                };
            }
        }

        /// <inheritdoc/>
        public async Task<Stream> GetStreamAsync(string uri)
        {
            CheckNetwork();

            await SetTokensAsync();

            return await _client.GetStreamAsync(uri);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string uri, string? content = null)
        {
            CheckNetwork();

            try
            {
                var context = StartRequest(content);
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
                    await SetTokensAsync();

                    return await _client.PostAsync(uri, new StringContent(content ?? string.Empty, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json), context.CancellationToken);
                }, context);
                EndRequest(context);

                return response;
            }
            catch (RateLimiterRejectedException)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.TooManyRequests
                };
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string uri, string? content = null)
        {
            CheckNetwork();

            try
            {
                var context = StartRequest(content);
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
                    await SetTokensAsync();

                    return await _client.PutAsync(uri, new StringContent(content ?? string.Empty, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json), context.CancellationToken);
                }, context);
                EndRequest(context);

                return response;
            }
            catch (RateLimiterRejectedException)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.TooManyRequests
                };
            }
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            CheckNetwork();

            try
            {
                var context = StartRequest();
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
                    await SetTokensAsync();

                    return await _client.DeleteAsync(uri, context.CancellationToken);
                }, context);
                EndRequest(context);

                return response;
            }
            catch (RateLimiterRejectedException)
            {
                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.TooManyRequests
                };
            }
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

        /// <summary>
        /// Call before executing Polly pipeline.
        /// </summary>
        /// <param name="content">Content of HTTP request.</param>
        /// <returns>Polly <see cref="ResilienceContext"/> from <see cref="ResilienceContextPool"/>.</returns>
        private static ResilienceContext StartRequest(string? content = null)
        {
            var context = ResilienceContextPool.Shared.Get();
            if (!string.IsNullOrWhiteSpace(content))
            {
                var megabytes = (decimal)Encoding.UTF8.GetByteCount(content) / 1048576;
                context.Properties.Set(new(ResilienceConstants.CONTENT_MEGABYTES_RESILIENCE_KEY), megabytes);
            }

            return context;
        }

        /// <summary>
        /// Call after executing Polly pipeline.
        /// </summary>
        /// <param name="context">Polly <see cref="ResilienceContext"/> to put back in the <see cref="ResilienceContextPool"/>. Recommended, but not required.</param>
        private static void EndRequest(ResilienceContext context)
        {
            ResilienceContextPool.Shared.Return(context);
        }

        /// <summary>
        /// Set access and refresh tokens on the current <see cref="_client"/>.
        /// </summary>
        private async Task SetTokensAsync()
        {
            var tokenInfo = await _tokenInfoAsync.Invoke();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, tokenInfo.Token);
            _client.DefaultRequestHeaders.Remove("refreshToken");
            _client.DefaultRequestHeaders.Add("refreshToken", tokenInfo.RefreshToken);
        }
    }
}
