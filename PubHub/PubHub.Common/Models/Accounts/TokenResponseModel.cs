namespace PubHub.Common.Models.Accounts
{
    public class TokenResponseModel
    {
        public TokenResponseModel(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        public string Token { get; }
        public string RefreshToken { get; }
    }
}
