using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace PubHub.AdminPortal.Components.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpClientFactory _httpClientFactory;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory)
        {
            _localStorage = localStorage;
            _httpClientFactory = httpClientFactory;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            try
            {
                var token = await _localStorage.GetItemAsync<string>("token");

                if (string.IsNullOrWhiteSpace(token))
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                claimsPrincipal = GetClaims(token);
                if (claimsPrincipal == null)
                    return await Task.FromResult(new AuthenticationState(claimsPrincipal));

                if (claimsPrincipal.Identity != null && claimsPrincipal.Identity.IsAuthenticated)
                {
                    return new AuthenticationState(claimsPrincipal);
                }
                else
                {
                    // Return an anonymous authentication state if the user is not authenticated
                    return new AuthenticationState(new ClaimsPrincipal());
                }
            }
            catch { return await Task.FromResult(new AuthenticationState(_anonymous)); }
            

            
        }

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

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
