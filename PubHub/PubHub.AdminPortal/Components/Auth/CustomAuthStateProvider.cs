using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace PubHub.AdminPortal.Components.Auth
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorage.GetItemAsync<string>("token");

                if (string.IsNullOrWhiteSpace(token))
                    return await Task.FromResult(new AuthenticationState(_anonymous));

                var claimsPrincipal = GetClaims(token);

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
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to retrieve the AuthenticationState, ", ex.Message);
                return await Task.FromResult(new AuthenticationState(_anonymous)); 
            }          
            
        }

        public void UpdateAuhenticationState(string jwtToken)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                claimsPrincipal = GetClaims(jwtToken);
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
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
