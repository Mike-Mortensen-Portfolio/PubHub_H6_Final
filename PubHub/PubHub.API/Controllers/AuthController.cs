using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Users;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly PubHubContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly AuthService _authService;
        private readonly AccessService _accessService;

        public AuthController(ILogger<AuthController> logger, PubHubContext context, UserManager<Account> userManager, AuthService authService, AccessService accessService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _authService = authService;
            _accessService = accessService;
        }

        [HttpPost("user")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IResult> RegisterUserAsync([FromBody] UserCreateModel userCreateModel, [FromHeader] string appId)
        {
            if (!_accessService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            // TODO (SIA): Validate model.

            // Check if user already exists.
            var existingUser = await _context.Users.
                FirstOrDefaultAsync(account => account.NormalizedEmail == userCreateModel.Account.Email.ToUpper());
            if (existingUser is not null)
                return Results.Problem(
                    type: DuplicateProblemSpecification.TYPE,
                    statusCode: DuplicateProblemSpecification.STATUS_CODE,
                    title: DuplicateProblemSpecification.TITLE,
                    detail: "A matching user already exists",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", existingUser.Id}
                    });

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.ToLower() == AccountTypeConstants.USER_ACCOUNT_TYPE)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == INVALID_ENTITY_ID)
            {
                _logger.LogError("Unable to get account type: {TypeName}", AccountTypeConstants.USER_ACCOUNT_TYPE);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to find account type.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "AccountType", AccountTypeConstants.USER_ACCOUNT_TYPE }
                    });
            }

            // Create user account.
            Account account = new()
            {
                Email = userCreateModel.Account.Email,
                AccountTypeId = accountTypeId
            };
            var createIdentityResult = await _userManager.CreateAsync(account, userCreateModel.Account.Password);
            if (!createIdentityResult.Succeeded)
            {
                _logger.LogError("Unable to create account with type ID: {AccountTypeId}", accountTypeId);

                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: $"Unable to create user account.");
            }

            // Create user associated with the account.
            var newUser = new User()
            {
                Name = userCreateModel.Name,
                Surname = userCreateModel.Surname,
                Birthday = userCreateModel.Birthday,
                Account = account
            };
            await _context.Set<User>()
                .AddAsync(newUser);
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when adding user.");

                // Attempt to delete account.
                var deleteIdentityResult = await _userManager.DeleteAsync(account);
                if (!deleteIdentityResult.Succeeded)
                {
                    _logger.LogError("Failed to delete account: {Account}", account.Id);
                }

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the user couldn't be created. Please try again.");
            }

            // Retreive newly added user.
            var userInfo = await _context.GetUserInfoAsync(newUser.Id);
            if (userInfo == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "Unable to retreive information of new user.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id" , newUser.Id }
                    });
            }

            // Create token.
            var tokenResult = await _authService.CreateTokenPairAsync(account, userInfo.Id);
            if (!tokenResult.Success)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and no token could be created for the given user account. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "UserId", userInfo.Id },
                        { "AccountId", account.Id }
                    });
            }

            return Results.Created($"users/{userInfo.Id}", new UserCreatedResponseModel()
            {
                TokenResponseModel = new(tokenResult.Token, tokenResult.RefreshToken),
                UserInfo = userInfo
            });
        }

        [HttpPost("token")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(TokenResponseModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetTokenAsync([FromHeader] string email, [FromHeader] string password, [FromHeader] string appId)
        {
            if (!_accessService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

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

            // Get account type name.
            var accountTypeName = await _context.Set<Account>()
                .Include(a => a.AccountType)
                .Where(a => a.Id == account.Id)
                .Select(a => a.AccountType.Name)
                .FirstOrDefaultAsync();
            if (string.IsNullOrEmpty(accountTypeName))
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Unable to resolve account type name.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", account.Id }
                    });
            }

            // Create token.
            var tokenResult = await _authService.CreateTokenPairAsync(account, accountTypeName);
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
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(TokenResponseModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> RefreshTokenAsync([FromHeader] string expiredToken, [FromHeader] string refreshToken, [FromHeader] string appId)
        {
            if (!_accessService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            // Read expired token.
            JwtSecurityToken jwt = new(expiredToken);

            // Find subject (account holder GUID).
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

            // Find account type ID.
            var accountType = jwt.Claims.FirstOrDefault(c => c.Type == "accountType")?.Value;
            if (accountType == null)
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to extract account type from token.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Token" , expiredToken }
                    });
            }

            // Parse IDs as GUIDs.
            if (!Guid.TryParse(sub, out Guid subjectId) ||
                !Guid.TryParse(accountType, out Guid accountTypeId))
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to parse GUID claims.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "sub" , sub },
                        { "accountType", accountType }
                    });
            }

            // Get account type name.
            var accountTypeName = (await _context.Set<AccountType>().FirstOrDefaultAsync(at => at.Id == accountTypeId))?.Name;
            if (string.IsNullOrEmpty(accountTypeName))
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Unable to resolve account type.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", accountTypeId }
                    });
            }

            // Get account ID.
            Guid? accountId = null;
            var accountTypeNameLower = accountTypeName.ToLowerInvariant();
            if (AccountTypeConstants.USER_ACCOUNT_TYPE == accountTypeName.ToLowerInvariant())
            {
                accountId = (await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId;
            }
            else if (AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE == accountTypeName.ToLowerInvariant())
            {
                accountId = (await _context.Set<Publisher>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId;
            }
            else if (AccountTypeConstants.OPERATOR_ACCOUNT_TYPE == accountTypeName.ToLowerInvariant())
            {
                accountId = (await _context.Set<Operator>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId;
            }
            if (accountId == null)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "No account for subject with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "AccountType", accountTypeName },
                        { "Id", subjectId }
                    });
            }

            // Get stored refresh token matching passed refresh token.
            var storedRefreshToken = await _context.Set<AccountRefreshToken>()
                .Include(art => art.Account)
                .FirstOrDefaultAsync(art => art.AccountId == accountId && art.Value == refreshToken);
            if (storedRefreshToken == null || DateTime.UtcNow > storedRefreshToken.Expiration)
            {
                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: "No matching refresh token found for account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id" , accountId }
                    });
            }

            // Create new token.
            var tokenResult = await _authService.CreateTokenPairAsync(storedRefreshToken.Account, subjectId);
            if (!tokenResult.Success)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the token couldn't be updated. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", subjectId }
                    });
            }

            return Results.Accepted(value: new TokenResponseModel(tokenResult.Token, tokenResult.RefreshToken));
        }
        
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> RevokeTokenAsync([FromHeader] string token, [FromHeader] string refreshToken, [FromHeader] string appId)
        {
            if (!_accessService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            // Read expired token.
            JwtSecurityToken jwt = new(token);

            // Find subject (account holder GUID).
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

            // Find account type ID.
            var accountType = jwt.Claims.FirstOrDefault(c => c.Type == "accountType")?.Value;
            if (accountType == null)
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to extract account type from token.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Token" , token }
                    });
            }

            // Parse IDs as GUIDs.
            if (!Guid.TryParse(sub, out Guid subjectId) ||
                !Guid.TryParse(accountType, out Guid accountTypeId))
            {
                return Results.Problem(
                    statusCode: BadRequestSpecification.STATUS_CODE,
                    title: BadRequestSpecification.TITLE,
                    detail: "Unable to parse GUID claims.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "sub" , sub },
                        { "accountType", accountType }
                    });
            }

            // Get account type name.
            var accountTypeName = (await _context.Set<AccountType>().FirstOrDefaultAsync(at => at.Id == accountTypeId))?.Name;
            if (accountTypeName == null)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Unable to resolve account type.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", accountTypeId }
                    });
            }

            // Get account ID.
            var accountId = accountTypeName.ToLowerInvariant() switch
            {
                AccountTypeConstants.USER_ACCOUNT_TYPE => (await _context.Set<User>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId,
                AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE => (await _context.Set<Publisher>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId,
                AccountTypeConstants.OPERATOR_ACCOUNT_TYPE => (await _context.Set<Operator>().FirstOrDefaultAsync(u => u.Id == subjectId))?.AccountId,
                _ => null
            };
            if (accountId == null)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "No account for subject with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "AccountType", accountTypeName },
                        { "Id", subjectId }
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
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when removing refresh token.");

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
