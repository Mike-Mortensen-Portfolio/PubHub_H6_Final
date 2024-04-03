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
        private readonly ILogger<AuthService> _logger;
        private readonly PubHubContext _context;

        public AuthService(ILogger<AuthService> logger, IOptions<AuthOptions> authOptions, PubHubContext context)
        {
            _authOptions = authOptions.Value;
            _logger = logger;
            _context = context;
        }

        public async Task<TokenResult> CreateTokenPairAsync(Account account)
        {
            var token = CreateToken(account);
            var refreshToken = await CreateRefreshTokenAsync(account);

            return new()
            {
                Token = token,
                RefreshToken = refreshToken,
                Success = !string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken)
            };
        }

        /// <summary>
        /// Create a JWT token for a given <see cref="Account"/>.
        /// </summary>
        /// <param name="account"><see cref="Account"/> to use as subject.</param>
        /// <returns>JWT token; otherwise <see cref="string.Empty"/>.</returns>
        private string CreateToken(Account account)
        {
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key)), SecurityAlgorithms.HmacSha256);
            var claims = BuildClaimsAsync(account);
            var jwt = GetJwtToken(signingCredentials, claims);

            string token = string.Empty;
            try
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to create token.");
            }

            return token;
        }

        /// <summary>
        /// Create a new refresh token for a given <see cref="Account"/> and store it in the database.
        /// </summary>
        /// <param name="account"><see cref="Account"/> to associate refresh token with.</param>
        /// <returns>Refresh token; otherwise <see cref="string.Empty"/>.</returns>
        private async Task<string> CreateRefreshTokenAsync(Account account)
        {
            // Generate value.
            var refreshToken = Guid.NewGuid().ToString();

            // Store refresh token.
            await _context.Set<AccountRefreshToken>()
                .AddAsync(new()
                {
                    AccountId = account.Id,
                    Value = refreshToken,
                    Expiration = DateTime.Now.AddDays(_authOptions.RefreshLifetime)
                });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to store refresh token for account: {AccountId}", account.Id);

                return string.Empty;
            }

            return refreshToken;
        }

        /// <summary>
        /// Build claims based on properties of a given <see cref="Account"/>. Claims include claims used for identification as well as audiences to which access is given.
        /// </summary>
        /// <param name="account"><see cref="Account"/> to create claims for.</param>
        /// <returns>List of claims for the account.</returns>
        private List<Claim> BuildClaimsAsync(Account account)
        {
            List<Claim> claims =
            [
                new("sub", account.Id.ToString()),
                new(ClaimTypes.Email, account.Email)
            ];

            foreach (var audience in _authOptions.Audiences)
            {
                claims.Add(new Claim("aud", audience));
            }

            return claims;
        }

        /// <summary>
        /// Create a JWT token with claims, expiration date, etc.
        /// </summary>
        /// <param name="signingCredentials">Credentials used to sign the token.</param>
        /// <param name="claims">Claims to include in the token.</param>
        /// <returns>JWT token object; ready to write with <see cref="JwtSecurityTokenHandler"/>.</returns>
        private JwtSecurityToken GetJwtToken(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: _authOptions.Issuers.FirstOrDefault(),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_authOptions.Lifetime),
                signingCredentials: signingCredentials);
            
            return jwt;
        }
    }
}
