using System;
using System.Net.Mime;
using System.Text;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.RateLimiting;

namespace PubHub.Common.Services
{
    public class HttpClientService : IHttpClientService
    {
        private const string BEARER_KEY = "Bearer";
        private readonly ILogger<HttpClientService> _logger;
        private readonly HttpClient _client;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;
        private readonly PollyInfoService _pollyInfoService;

        public HttpClientService(ILogger<HttpClientService> logger, HttpClient client, ResiliencePipeline<HttpResponseMessage> resiliencePipeline, PollyInfoService pollyInfoService)
        {
            _logger = logger;
            _client = client;
            _client.Timeout = TimeSpan.Parse("24.20:31:23.6470000"); // Polly will take care of timeout.
            _resiliencePipeline = resiliencePipeline;
            _pollyInfoService = pollyInfoService;
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
            _pollyInfoService.SetActionType(PollyInfoService.ActionType.GET);

            return await DoRequestAsync(async context =>
            {
                return await _client.GetAsync(uri, context.CancellationToken);
            });
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
            _pollyInfoService.SetActionType(PollyInfoService.ActionType.POST);

            return await DoRequestAsync(async context =>
            {
                return await _client.PostAsync(uri, new StringContent(content ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json), context.CancellationToken);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> PutAsync(string uri, string? content = null)
        {
            _pollyInfoService.SetActionType(PollyInfoService.ActionType.PUT);

            return await DoRequestAsync(async context =>
            {
                return await _client.PutAsync(uri, new StringContent(content ?? string.Empty, Encoding.UTF8, MediaTypeNames.Application.Json), context.CancellationToken);
            });
        }

        /// <inheritdoc/>
        public async Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            _pollyInfoService.SetActionType(PollyInfoService.ActionType.DELETE);

            return await DoRequestAsync(async context =>
            {
                return await _client.DeleteAsync(uri, context.CancellationToken);
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

        private async Task<HttpResponseMessage> DoRequestAsync(Func<ResilienceContext, ValueTask<HttpResponseMessage>> func)
        {
            const string UNFULFILLED_REQUESTS_MESSAGE = "Unfulfilled requests. Please reload to try again.";
            const string RATE_LIMIT_MESSAGE = "Internal rate limiter was triggered.";

            CheckNetwork();

            var context = StartRequest();

            try
            {
                SetHeaders();
                var response = await _resiliencePipeline.ExecuteAsync(func, context);

                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    _pollyInfoService.ErrorText = UNFULFILLED_REQUESTS_MESSAGE;

                return response;
            }
            catch (RateLimiterRejectedException)
            {
                _logger.LogWarning("Polly rate limiter was triggerd.");
                _pollyInfoService.ErrorText = RATE_LIMIT_MESSAGE;

                return new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.TooManyRequests
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception was thrown from Polly pipeline.");
                _pollyInfoService.ErrorText = UNFULFILLED_REQUESTS_MESSAGE;

                throw;
            }
            finally
            {
                EndRequest(context);
            }
        }

        /// <summary>
        /// Call before executing Polly pipeline.
        /// </summary>
        /// <param name="content">Content of HTTP request.</param>
        /// <returns>Polly <see cref="ResilienceContext"/> from <see cref="ResilienceContextPool"/>.</returns>
        private ResilienceContext StartRequest(string? content = null)
        {
            var context = ResilienceContextPool.Shared.Get();
            if (!string.IsNullOrWhiteSpace(content))
            {
                var megabytes = (decimal)Encoding.UTF8.GetByteCount(content) / 1048576;
                context.Properties.Set(new(ResilienceConstants.CONTENT_MEGABYTES_RESILIENCE_KEY), megabytes);
            }
            context.Properties.Set(new(ResilienceConstants.INFO_SERVICE_KEY), (PollyInfoService?)_pollyInfoService);

            return context;
        }

        /// <summary>
        /// Call after executing Polly pipeline.
        /// </summary>
        /// <param name="context">Polly <see cref="ResilienceContext"/> to put back in the <see cref="ResilienceContextPool"/>. Recommended, but not required.</param>
        private void EndRequest(ResilienceContext context)
        {
            _pollyInfoService.Stop();
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
