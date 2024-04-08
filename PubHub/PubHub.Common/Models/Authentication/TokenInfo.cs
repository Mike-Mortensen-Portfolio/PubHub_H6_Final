namespace PubHub.Common.Models.Authentication
{
    public class TokenInfo
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}
