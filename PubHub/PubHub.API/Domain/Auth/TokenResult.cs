namespace PubHub.API.Domain.Auth
{
    public class TokenResult
    {
        public required string Token { get; init; }
        public required string RefreshToken { get; init; }
        public required bool Success { get; init; }
    }
}
