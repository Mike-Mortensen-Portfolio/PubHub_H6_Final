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
    }
}
