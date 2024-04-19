using System.Net.Mime;
using System.Text;
using Polly;
using Polly.RateLimiting;

namespace PubHub.Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private const string BEARER_KEY = "Bearer";

        private readonly HttpClient _client;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

        public HttpClientService(HttpClient client, ResiliencePipeline<HttpResponseMessage> resiliencePipeline)
        {
            _client = client;
            _client.Timeout = TimeSpan.Parse("24.20:31:23.6470000"); // Polly will take care of timeout.
            _resiliencePipeline = resiliencePipeline;
        }

        protected Dictionary<string, string> Headers { get; set; } = [];

        public void AddBearer(string bearer)
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(BEARER_KEY, bearer);
        }

        public void AddOrReplaceHeader(string key, string value)
        {
            if (!Headers.TryAdd(key, value))
                Headers[key] = value;
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> GetAsync(string uri)
        {
            CheckNetwork();
            SetHeaders();

            try
            {
                var context = StartRequest();
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
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

            return await _client.GetStreamAsync(uri);
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PostAsync(string uri, string? content = null)
        {
            CheckNetwork();
            SetHeaders();

            try
            {
                var context = StartRequest(content);
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
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
            SetHeaders();

            try
            {
                var context = StartRequest(content);
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
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
            SetHeaders();

            try
            {
                var context = StartRequest();
                var response = await _resiliencePipeline.ExecuteAsync(async context =>
                {
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
        /// Set headers from <see cref="Headers"/> on the current <see cref="_client"/>.
        /// </summary>
        private void SetHeaders()
        {
            foreach (var header in Headers)
            {
                if (_client.DefaultRequestHeaders.Contains(header.Key))
                    _client.DefaultRequestHeaders.Remove(header.Key);
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }
}
