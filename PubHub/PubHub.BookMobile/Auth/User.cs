using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using IntelliJ.Lang.Annotations;
using PubHub.BookMobile.ErrorSpecifications;
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
        private const string JWT_IDENTIFIER = "jwt";
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
        internal static async Task Set(TokenResponseModel tokens)
        {
            _identity = await GetIdentity(tokens.Token) ?? throw new Exception($"Token validation failed: {tokens.Token}");

            await SecureStorage.Default.SetAsync(StorageConstants.TOKEN_KEY, tokens.Token);
            await SecureStorage.Default.SetAsync(StorageConstants.REFRESH_TOKEN_KEY, tokens.RefreshToken);
        }

        private static async Task<ClaimsPrincipal?> GetIdentity(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
            {
                await Shell.Current.CurrentPage.DisplayAlert(InvalidTokenError.TITLE, InvalidTokenError.ERROR_MESSAGE, InvalidTokenError.BUTTON_TEXT);
                return null;
            }

            var jwtToken = handler.ReadJwtToken(token);

            var identity = new ClaimsIdentity(jwtToken.Claims, JWT_IDENTIFIER);
            return new ClaimsPrincipal(identity);
        }

        /// <summary>
        /// Removes the identity of the currently active user
        /// </summary>
        internal static void Unset()
        {
            _identity = null;

            SecureStorage.Default.RemoveAll();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>A <see cref="Tuple{T1, T2}"/> that contains a <see langword="bool"/> success state and the <see cref="TokenInfo"/> pair if the process was successful; otherwise, if not, <see langword="null"/></returns>
        internal static async Task<Tuple<bool, TokenInfo?>> TryGetCachedToken()
        {
            TokenInfo? tokens = null;
            try
            {
                tokens = await GetChachedToken();
            }
            catch (Exception) { /*Save guard to avoid a crash. It's up to the consumer of this method to ensure the program acts accordingly when failing to retrieve tokens*/ }

            return new Tuple<bool, TokenInfo?>(tokens is not null, tokens);
        }

        /// <summary>
        /// Retrieves the <see cref="TokenInfo"/> pair from <see cref="SecureStorage"/> or throws and <see cref="Exception"/> if the transaction wasn't successful
        /// </summary>
        /// <returns>A new instance of type <see cref="TokenInfo"/> pair</returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<TokenInfo> GetChachedToken()
        {
            return new TokenInfo
            {
                RefreshToken = await SecureStorage.Default.GetAsync(StorageConstants.REFRESH_TOKEN_KEY) ?? throw new Exception($"No {StorageConstants.REFRESH_TOKEN_KEY} found!"),
                Token = await SecureStorage.Default.GetAsync(StorageConstants.TOKEN_KEY) ?? throw new Exception($"No {StorageConstants.TOKEN_KEY} found!")
            };
        }

        /// <summary>
        /// Retrieves a claim an tries to parse it as a <see cref="Guid"/>
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns>The parsed <see cref="Guid"/> or <see langword="null"/> if the operation wasn't successful</returns>
        private static Guid? ExtractGuid(string claimType)
        {
            var claim = ExtractClaim(claimType);

            if (!Guid.TryParse(claim, out Guid result))
                return null;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimType"></param>
        /// <returns>The claim value or <see langword="null"/> if the claim couldn't be extracted</returns>
        private static string? ExtractClaim(string claimType)
        {
            var claimValue = _identity?.FindFirst(c => c.Type == claimType)?.Value;

            return claimValue;
        }
    }
}
