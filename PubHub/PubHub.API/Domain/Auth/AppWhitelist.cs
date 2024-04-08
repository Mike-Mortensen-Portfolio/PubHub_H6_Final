namespace PubHub.API.Domain.Auth
{
    public class AppWhitelist
    {
        public required string AppId { get; init; }
        public required IDictionary<string, IEnumerable<string>> ControllerEndpoints { get; init; }
    }
}
