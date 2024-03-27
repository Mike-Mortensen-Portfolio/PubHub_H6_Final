using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common.Models;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(PubHubContext context) : Controller
    {
        private readonly PubHubContext _context = context;

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
                .Select(u => new UserModel()
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
                .Select(b => new BookModel()
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
        /// <param name="userUpdateModel">Information to update the existing record with.</param>
        /// <response code="200">Success. The user was updated.</response>
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
            user.Account.Email = userUpdateModel.Email;
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
            if (accountId == default)
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
            const string accountTypeName = "suspended";

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(accountTypeName, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == default)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to find account type: '{accountTypeName}'");
            }

            // Get account ID.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == default)
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
