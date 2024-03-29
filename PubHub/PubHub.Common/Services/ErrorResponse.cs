namespace PubHub.Common.Services
{
    /// <summary>
    /// Represents a <see href="https://datatracker.ietf.org/doc/html/rfc9457">RFC9457</see> compliant error response 
    /// </summary>
    public class ErrorResponse
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int Status { get; set; }
        public required string Detail { get; set; }
        public string? Instance { get; set; }
        public IDictionary<string, object?> Extensions { get; set; } = new Dictionary<string, object?>(StringComparer.Ordinal);
    }
}
