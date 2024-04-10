using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Services;

namespace PubHub.AdminPortal.Components.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IAuthenticationService _authenticationService;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage, IAuthenticationService authenticationService)
        {
            _localStorage = localStorage;
            _authenticationService = authenticationService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("token");
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

                if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                TokenInfo tokenInfo = new() { Token = token, RefreshToken = refreshToken };                

                var claimsPrincipal = GetClaims(token);

                if (claimsPrincipal.FindFirst("exp") != null)
                {
                    var expireClaim = long.Parse(claimsPrincipal.FindFirst("exp")?.Value ?? "0");

                    var utcExpire = DateTimeOffset.FromUnixTimeSeconds(expireClaim);
                    var refreshdate = utcExpire.DateTime.Subtract(TimeSpan.FromMinutes(12));
                    // If the token has expires in 5 minutes, then we want to refresh the token and update the existing token and refresh token in our local storage.
                    if (refreshdate < DateTime.UtcNow)
                    {
                        var tokenResponse = await _authenticationService.RefreshesTokenAsync();
                        if (tokenResponse != null && tokenResponse.Instance != null)
                        {
                            await _localStorage.SetItemAsync<string>("token", tokenResponse.Instance.Token);
                            await _localStorage.SetItemAsync<string>("refreshToken", tokenResponse.Instance.RefreshToken);
                            UpdateAuhenticationState(tokenResponse.Instance.Token);
                        }
                        else
                        {
                            await _localStorage.RemoveItemAsync("token");
                            await _localStorage.RemoveItemAsync("refreshToken");
                            return await Task.FromResult(new AuthenticationState(_anonymous));
                        }
                    }
                }

                if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
                {
                    return new AuthenticationState(claimsPrincipal);
                }
                else
                {
                    // Return an anonymous authentication state if the user is not authenticated
                    return new AuthenticationState(_anonymous);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to retrieve the AuthenticationState, ", ex.Message);
                return await Task.FromResult(new AuthenticationState(_anonymous)); 
            }          
            
        }

        /// <summary>
        /// Used to update the AuthenticationState.
        /// </summary>
        /// <param name="jwtToken"></param>
        public void UpdateAuhenticationState(string jwtToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                claimsPrincipal = GetClaims(jwtToken);
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
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
