namespace PubHub.API.Domain.Auth
{
    public class AuthOptions
    {
        public required IEnumerable<string> Issuers { get; init; }
        public required int Lifetime { get; init; }
        public required int RefreshLifetime { get; init; }
        public required IEnumerable<string> Audiences { get; init; }
        public required string KeyId {  get; init; }
    }
}
