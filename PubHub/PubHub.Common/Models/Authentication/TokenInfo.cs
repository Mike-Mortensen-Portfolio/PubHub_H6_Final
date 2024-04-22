namespace PubHub.Common.Models.Authentication
{
    public class TokenInfo
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public bool IsValid => !string.IsNullOrWhiteSpace(Token) && !string.IsNullOrWhiteSpace(RefreshToken);
    }
}
