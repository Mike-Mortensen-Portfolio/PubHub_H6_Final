using PubHub.Common.Models.Authentication;

namespace PubHub.Common.ApiService
{
    public class ApiOptions
    {
        /// <summary>
        /// The base address used to configure the <see langword="internal"/> <see cref="HttpClient"/>
        /// </summary>
        public string Address { get; set; } = null!;
        /// <summary>
        /// The name of the <see langword="internal"/> <see cref="HttpClient"/>
        /// </summary>
        public string? HttpClientName { get; set; }
        /// <summary>
        /// Whether services (incl. the <see cref="HttpClient"/>) should be configured for use on mobile.
        /// </summary>
        public bool ConfigureForMobile { get; set; } = false;
        /// <summary>
        /// Unique identifier for the current application. Used to authenticate the application on the PubHub API.
        /// </summary>
        public string AppId { get; set; } = string.Empty;
        /// <summary>
        /// Get the current access and refresh token.
        /// </summary>
        public Func<IServiceProvider, Task<TokenInfo>> GetTokenInfoAsync { get; set; } = null!;
        /// <summary>
        /// Set the current access and refresh token.
        /// </summary>
        public Action<IServiceProvider, TokenInfo> SetTokenInfo { get; set; } = null!;
        /// <summary>
        /// Remove the current access and refresh token.
        /// </summary>
        public Action<IServiceProvider> RemoveTokenInfo { get; set; } = null!;
    }
}
