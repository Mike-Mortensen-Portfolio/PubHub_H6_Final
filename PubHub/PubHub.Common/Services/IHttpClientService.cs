namespace PubHub.Common.Services
{
    public interface IHttpClientService
    {
        /// <summary>
        /// Add bearer authenticatoin to the wrapped <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="bearer">BEARER key.</param>
        public void AddBearer(string bearer);

        /// <summary>
        /// Add a header to the wrapped <see cref="HttpClient"/>. If they header already exists, the header value will be replaced.
        /// </summary>
        /// <param name="key">Header key.</param>
        /// <param name="value">Header value.</param>
        public void AddOrReplaceHeader(string key, string value);

        /// <summary>
        /// Send a GET message using the base address of the wrapped <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="uri">URI (must NOT start with '/').</param>
        /// <returns>HTTP response.</returns>
        public Task<HttpResponseMessage> GetAsync(string uri);

        /// <param name="uri">URI (must NOT start with '/').</param>
        public Task<Stream> GetStreamAsync(string uri);

        /// <summary>
        /// Send a POST message using the base address of the wrapped <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="uri">URI (must NOT start with '/').</param>
        /// <param name="content">Content of the POST message.</param>
        /// <returns>HTTP response.</returns>
        public Task<HttpResponseMessage> PostAsync(string uri, string? content = null);

        /// <summary>
        /// Send a PUT message using the base address of the wrapped <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="uri">URI (must NOT start with '/').</param>
        /// <param name="content">Content of the PUT message.</param>
        /// <returns>HTTP response.</returns>
        public Task<HttpResponseMessage> PutAsync(string uri, string? content = null);

        /// <summary>
        /// Send a DELETE message using the base address of the wrapped <see cref="HttpClient"/>.
        /// </summary>
        /// <param name="uri">URI (must NOT start with '/').</param>
        /// <returns>HTTP response.</returns>
        public Task<HttpResponseMessage> DeleteAsync(string uri);
    }
}
