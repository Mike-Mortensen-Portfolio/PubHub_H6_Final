using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(PubHubContext context) : Controller
    {
        private readonly PubHubContext _context = context;

        /// <summary>
        /// Add a new user account to PubHub.
        /// </summary>
        /// <param name="userCreateModel">User information.</param>
        /// <response code="200">Success. A new user account was created.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPost("", Name = "AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> AddUserAsync([FromBody] UserCreateModel userCreateModel)
        {
            // TODO (SIA): Validate model.

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(Constants.USER_ACCOUNT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to find account type: '{Constants.USER_ACCOUNT_TYPE}'");
            }

            // Create user account.
            // TODO (SIA): Add more account related data to fully set up an account.
            await _context.Set<User>()
                .AddAsync(new User()
                {
                    Name = userCreateModel.Name,
                    Surname = userCreateModel.Surname,
                    Birthday = userCreateModel.Birthday,
                    Account = new()
                    {
                        Email = userCreateModel.Account.Email,
                        AccountTypeId = accountTypeId
                    }
                });
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to save changes to the database.");
            }

            return Results.Ok();
        }

        /// <summary>
        /// Get all general information about a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <response code="200">Success. User information was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IResult GetUser(int id)
        {
            var userModel = _context.Set<User>()
                .Include(u => u.Account)
                .ThenInclude(a => a!.AccountType)
                .Select(u => new UserInfoModel()
                {
                    Id = u.Id,
                    Email = u.Account.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    Birthday = u.Birthday,
                    AccountType = u.Account!.AccountType!.Name
                })
                .SingleOrDefault(u => u.Id == id);
            if (userModel == null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No user with ID: {id}");
            }

            return Results.Ok(userModel);
        }

        /// <summary>
        /// Get all books for a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <response code="200">Success. All books of the user was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}/books", Name = "GetBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> GetBooksAsync(int id)
        {
            // Check if user exists.
            if (!await _context.Set<User>().AnyAsync(u => u.Id == id))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No user with ID: {id}");
            }

            // Retreive all books.
            var bookModels = await _context.Set<UserBook>()
                .Where(b => b.UserId == id)
                .Select(b => new BookInfoModel()
                {
                    Title = b.Book.Title,
                    PublicationDate = b.Book.PublicationDate
                })
                .ToListAsync();
            
            return Results.Ok(bookModels);
        }

        /// <summary>
        /// Update information of a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <param name="userUpdateModel">Information to update the existing user with.</param>
        /// <response code="200">Success. The user was updated.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPut("{id}", Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> UpdateUserAsync(int id, [FromBody] UserUpdateModel userUpdateModel)
        {
            // TODO (SIA): Validate model.

            // Get current entry.
            var user = _context.Set<User>()
                .Include(u => u.Account)
                .SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No user with ID: {id}");
            }

            // Update entry with new data.
            user.Account.Email = userUpdateModel.Account.Email;
            user.Name = userUpdateModel.Name;
            user.Surname = userUpdateModel.Surname;
            user.Birthday = userUpdateModel.Birthday;

            _context.Set<User>().Update(user);
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to save changes to the database.");
            }

            return Results.Ok();
        }

        /// <summary>
        /// Terminate the account of a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <response code="200">Success. The user was deleted.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete(Name = "DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> DeleteUserAsync(int id)
        {
            // Get account ID.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No user with ID: {id}");
            }

            // Delete account.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
            if (updatedRows > 0)
            {
                return Results.Ok();
            }

            // Unable to delete; report back.
            Dictionary<string, object?> extensions = new()
            {
                ["UserId"] = id,
                ["AccountId"] = accountId
            };

            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: $"Unable to delete account (ID: {accountId}) of user (ID: {id}).",
                extensions: extensions);
        }

        /// <summary>
        /// Suspend the account of a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <response code="200">Success. The user was suspended.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete(Name = "SuspendUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> SuspendUserAsync(int id)
        {
            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(Constants.SUSPENDED_ACCOUNT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to find account type: '{Constants.SUSPENDED_ACCOUNT_TYPE}'");
            }

            // Get account ID.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No user with ID: {id}");
            }

            // Set account type.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.AccountTypeId, accountTypeId));
            if (updatedRows > 0)
            {
                return Results.Ok();
            }

            // Unable to suspend; report back.
            Dictionary<string, object?> extensions = new()
            {
                ["UserId"] = id,
                ["AccountId"] = accountId,
                ["AccountTypeId"] = accountTypeId
            };

            return Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                detail: $"Unable to suspend account (ID: {accountId}) of user (ID: {id}).",
                extensions: extensions);
        }
    }
}
