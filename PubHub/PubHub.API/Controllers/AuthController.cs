using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Identity;
using PubHub.Common.Models.Accounts;

namespace PubHub.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthController : Controller
    {
        private readonly PubHubContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly AuthService _authService;

        public AuthController(PubHubContext context, UserManager<Account> userManager, AuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _authService = authService;
        }

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(TokenResponseModel))]
        public async Task<IResult> GetTokenAsync([FromHeader] string email, [FromHeader] string password)
        {
            if (!ModelState.IsValid)
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Login request parameters were malformed.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "State", ModelState }
                    });
            }

            // Validate user email and password.
            bool passwordIsCorrect = false;
            var account = await _userManager.FindByEmailAsync(email);
            if (account != null)
            {
                passwordIsCorrect = await _userManager.CheckPasswordAsync(account, password);
            }
            if (account == null || !passwordIsCorrect)
            {
                return Results.Problem(
                    statusCode: UnauthorizedSpecification.STATUS_CODE,
                    title: UnauthorizedSpecification.TITLE,
                    detail: "The provided credentials were incorrect.");
            }

            // Create token.
            var tokenResult = await _authService.CreateTokenPairAsync(account);
            if (!tokenResult.Success)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and no token could be created for the given account. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", account.Id }
                    });
            }

            return Results.Accepted(value: new TokenResponseModel(tokenResult.Token, tokenResult.RefreshToken));
        }

        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(TokenResponseModel))]
        public async Task<IResult> RefreshTokenAsync([FromHeader] string expiredToken, [FromHeader] string refreshToken)
        {
            // Read expired token and find subject (account GUID).
            JwtSecurityToken jwt = new(expiredToken);
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (sub == null)
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to extract subject from token.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Token" , expiredToken }
                    });
            }

            // Parse subject as GUID.
            if (!Guid.TryParse(sub, out Guid accountId))
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to parse subject as GUID.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Subject" , sub }
                    });
            }

            // Get stored refresh token.
            var storedRefreshToken = await _context.Set<AccountRefreshToken>()
                .FirstOrDefaultAsync(art => art.AccountId == accountId && art.Value == refreshToken);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.Expiration)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No matching refresh token found for account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id" , accountId }
                    });
            }

            // Get account.
            var account = await _context.Set<Account>()
                .FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No account with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id" , accountId }
                    });
            }

            // Create new token.
            var tokenResult = await _authService.CreateTokenPairAsync(account);
            if (!tokenResult.Success)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the token couldn't be updated. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", accountId }
                    });
            }

            return Results.Accepted(value: new TokenResponseModel(tokenResult.Token, tokenResult.RefreshToken));
        }
        
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IResult> RevokeTokenAsync([FromHeader] string token, [FromHeader] string refreshToken)
        {
            // Read expired token and find subject (account GUID).
            JwtSecurityToken jwt = new(token);
            var sub = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (sub == null)
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to extract subject from token.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Token" , token }
                    });
            }

            // Parse subject as GUID.
            if (!Guid.TryParse(sub, out Guid accountId))
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to parse subject as GUID.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Subject" , sub }
                    });
            }

            // Get stored refresh token.
            var storedRefreshToken = await _context.Set<AccountRefreshToken>()
                .FirstOrDefaultAsync(art => art.AccountId == accountId && art.Value == refreshToken);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.Expiration)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No matching refresh token found for account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id" , accountId }
                    });
            }

            // Remove token.
            _context.Set<AccountRefreshToken>().Remove(storedRefreshToken);
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the token couldn't be revoked. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", accountId }
                    });
            }

            return Results.Ok();
        }
    }
}
