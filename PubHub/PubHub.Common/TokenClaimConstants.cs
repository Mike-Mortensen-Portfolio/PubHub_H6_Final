using System.Security.Claims;

namespace PubHub.Common
{
    public static class TokenClaimConstants
    {
        public const string ID = "sub";
        public const string EMAIL = ClaimTypes.Email;
        public const string ACCOUNT_TYPE = "accountType";
        public const string AUDIENCE = "aud";        
    }
}
