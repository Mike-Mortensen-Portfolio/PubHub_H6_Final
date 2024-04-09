using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IntelliJ.Lang.Annotations;
using PubHub.Common;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;

namespace PubHub.BookMobile.Auth
{
    /// <summary>
    /// Represents the authentication state of any idenitty
    /// </summary>
    internal static class User
    {
        private static ClaimsPrincipal? _identity;

        public static Guid? Id => ExtractGuid(TokenClaimConstants.ID);
        public static string? Email => ExtractClaim(TokenClaimConstants.EMAIL);
        public static Guid? AccountTypeId => ExtractGuid(TokenClaimConstants.ACCOUNT_TYPE);
        public static bool IsAuthenticated => _identity?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// Sets the identity of the currently active user
        /// </summary>
        /// <param name="tokens"></param>
        /// <exception cref="Exception"></exception>
        internal static void Set(TokenResponseModel tokens)
        {
            _identity = GetIdentity(tokens.Token) ?? throw new Exception($"Token validation failed: {tokens.Token}");

            Preferences.Set(PreferenceConstants.TOKEN_KEY, tokens.Token);
            Preferences.Set(PreferenceConstants.REFRESH_TOKEN_KEY, tokens.RefreshToken);
        }

        private static ClaimsPrincipal? GetIdentity(string token)
        {

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return null;

            var jwtToken = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Removes the identity of the currently active user
        /// </summary>
        internal static void Unset()
        {
            _identity = null;

            Preferences.Clear();
        }

        private static bool HasChachedUser()
        {
            return Preferences.ContainsKey(PreferenceConstants.TOKEN_KEY) && Preferences.ContainsKey(PreferenceConstants.REFRESH_TOKEN_KEY);
        }

        internal static bool TryGetCachedToken(out TokenInfo? tokens)
        {
            tokens = null;
            if (!HasChachedUser())
                return false;

            tokens = GetChachedToken();

            return true;
        }

        internal static TokenInfo GetChachedToken()
        {
            return new TokenInfo
            {
                RefreshToken = Preferences.Get(PreferenceConstants.REFRESH_TOKEN_KEY, null) ?? throw new Exception($"No {PreferenceConstants.REFRESH_TOKEN_KEY} found!"),
                Token = Preferences.Get(PreferenceConstants.TOKEN_KEY, null) ?? throw new Exception($"No {PreferenceConstants.TOKEN_KEY} found!")
            };
        }

        private static Guid? ExtractGuid(string claimType)
        {
            var claim = ExtractClaim(claimType);

            if (Guid.TryParse(claim, out Guid result))
                return null;

            return result;
        }

        private static string? ExtractClaim(string claimType)
        {
            var claimValue = _identity?.FindFirst(c => c.Type == claimType)?.Value;

            return claimValue;
        }
    }
}
