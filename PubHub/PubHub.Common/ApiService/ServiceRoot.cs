using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.Common.ApiService
{
    public abstract class ServiceRoot
    {
        private readonly Func<Task<TokenInfo>> _tokenInfoAsync;
        private readonly Action<TokenInfo> _setTokenInfo;
        private readonly Action _removeTokenInfo;

        protected IHttpClientService Client { get; }

        /// <summary>
        /// Holds a reference to the <see cref="IHttpClientService"/> with a <see cref="HttpClient"/> instance to use in the services.
        /// </summary>
        /// <param name="clientService"></param>
        /// <param name="clientName"></param>
        /// <exception cref="NullReferenceException"></exception>
        internal ServiceRoot(IHttpClientService clientService,
            Func<Task<TokenInfo>> getTokenInfoAsync,
            Action<TokenInfo> setTokenInfoAsync,
            Action removeTokenInfoAsync)
        {
            _tokenInfoAsync = getTokenInfoAsync;
            _setTokenInfo = setTokenInfoAsync;
            _removeTokenInfo = removeTokenInfoAsync;
            Client = clientService;
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        protected JsonSerializerOptions JsonSerializerOptions { get; set; }

        /// <summary>
        /// Set tokens of <see cref="Client"/>.
        /// </summary>
        public async Task SetTokensAsync()
        {
            var tokenInfo = await _tokenInfoAsync.Invoke();
            Client.AddBearer(tokenInfo.Token);
            Client.AddOrReplaceHeader("refreshToken", tokenInfo.RefreshToken);
        }

        /// <summary>
        /// Set access and refresh tokens on the current <see cref="_client"/>.
        /// </summary>
        public async Task TryRefreshTokenAsync()
        {
            var tokenInfo = await _tokenInfoAsync.Invoke();
            var claimsPrincipal = GetClaims(tokenInfo.Token);

            // Check if token is about to expire.
            var expireClaim = claimsPrincipal.FindFirst("exp");
            if (expireClaim == null)
                return;
                
            var expireUnixTime = long.Parse(expireClaim.Value);
            var expireDateTime = DateTimeOffset.FromUnixTimeSeconds(expireUnixTime).UtcDateTime;
            // If the token expires in 5 minutes, then we want to refresh the token and update the existing token and refresh token in our local storage.
            if (expireDateTime > DateTime.UtcNow.AddMinutes(5))
                return;
            
            var response = await Client.PostAsync($"auth/refresh");
            if (!response.IsSuccessStatusCode)
            {
                _removeTokenInfo();

                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponseModel>(content, JsonSerializerOptions);
            _setTokenInfo(new()
            {
                Token = tokenResponse!.Token,
                RefreshToken = tokenResponse!.RefreshToken
            });
            Client.AddBearer(tokenResponse.Token);
            Client.AddOrReplaceHeader("refreshToken", tokenResponse!.RefreshToken);
        }

        /// <summary>
        /// Retrieving claims from the token saved in <see cref="ILocalStorageService"/>.
        /// </summary>
        /// <param name="token">The token which needs to be read and retrieve claims from.</param>
        /// <returns>A new <see cref="ClaimsPrincipal"/> with an identity.</returns>
        /// <exception cref="ArgumentException"></exception>
        public ClaimsPrincipal GetClaims(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Retrieves the claims contained within' the JWT token-
                var claims = jwtToken.Claims;

                var identity = new ClaimsIdentity(claims, "jwt");

                return new ClaimsPrincipal(identity);
            }
            else
            {
                throw new ArgumentException("Invalid token format");
            }
        }
    }
}
