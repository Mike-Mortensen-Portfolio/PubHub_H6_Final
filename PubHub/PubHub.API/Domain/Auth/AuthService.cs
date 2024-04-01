using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PubHub.API.Domain.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PubHub.API.Domain.Auth
{
    public class AuthService
    {
        private readonly AuthOptions _authOptions;

        public AuthService(IOptions<AuthOptions> authOptions)
        {
            _authOptions = authOptions.Value;
        }

        public void CreateToken(Account account)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.KeyId)), SecurityAlgorithms.HmacSha256);
            var claims = GetClaimsAsync(account);
            //var token = GenerateTokenOptions(signingCredentials, claims);

            //var test = new JwtSecurityTokenHandler().WriteToken(token);
            //var token = new TokenReader(test);
        }

        private JwtSecurityToken GetJwtToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: _authOptions.Issuers.FirstOrDefault(),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: signingCredentials);
            
            return jwt;
        }

        private List<Claim> GetClaimsAsync(Account account)
        {
            List<Claim> claims =
            [
                new(ClaimTypes.Email, account.Email)
            ];

            foreach (var audience in _authOptions.Audiences)
            {
                claims.Add(new Claim("aud", audience));
            }

            return claims;
        }
    }
}
