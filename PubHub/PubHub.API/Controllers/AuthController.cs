using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common.Models.Accounts;

namespace PubHub.API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthController : Controller
    {
        private readonly PubHubContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly SignInManager<Account> _signInManager;
        private readonly AuthService _authService;

        public AuthController(PubHubContext context, UserManager<Account> userManager, SignInManager<Account> signInManager, AuthService authService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        [HttpPost("token")]
        public async Task<IResult> GetTokenAsync(AccountLoginModel loginInfo)
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

            //var user = await _context.Set<User>().Include(u => u.Account).FirstOrDefaultAsync();
            //_authService.CreateToken(user.Account);

            //// Validate user email.
            //var account = await _userManager.FindByEmailAsync(loginInfo.Email);
            //if (account == null)
            //{
            //    return Results.Problem(
            //        statusCode: UnauthorizedSpecification.STATUS_CODE,
            //        title: UnauthorizedSpecification.TITLE,
            //        detail: "Email doesn't exist.",
            //        extensions: new Dictionary<string, object?>
            //        {
            //            { "Email", loginInfo.Email }
            //        });
            //}

            //// Validate user password.
            //if (!(await _userManager.CheckPasswordAsync(account, loginInfo.Password)))
            //{
            //    return Results.Problem(
            //        statusCode: UnauthorizedSpecification.STATUS_CODE,
            //        title: UnauthorizedSpecification.TITLE,
            //        detail: "Provided password is incorrect.");
            //}

            // Create token.

            return Results.Accepted();
        }

        //[HttpPost("refresh")]
        //RefreshToken()

        //[HttpPost("revoke")]
        //RevokeToken()
    }
}
