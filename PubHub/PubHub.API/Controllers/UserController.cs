using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
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
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUser(int id)
        {
            var userModel = _context.PubHubUsers
                .Include(u => u.Account)
                .ThenInclude(a => a!.AccountType)
                .Select(u => u.MapUserModelProperties())
                .SingleOrDefault(u => u.Id == id);
            if (userModel == null)
            {
                return NotFound($"No user with ID: {id}");
            }

            return Ok(userModel);
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
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooksAsync(int id)
        {
            // Check if user exists.
            if (!await _context.PubHubUsers.AnyAsync(u => u.Id == id))
            {
                return NotFound($"No user with ID: {id}");
            }

            // Retreive all books.
            var books = await _context.UserBooks
                .Where(b => b.UserId == id)
                .Select(b => b.Book.MapBookModelProperties())
                .ToListAsync();
            
            return Ok(books);
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
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUserAsync(int id, [FromBody] UserUpdateModel userUpdateModel)
        {
            // TODO: Validate model.

            // Get current entry.
            var user = _context.PubHubUsers
                .Include(u => u.Account)
                .SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound($"No user with ID: {id}");
            }

            // Sanity check properties of entry.
            if (user.Account == null)
            {
                throw new NullReferenceException("User account property is null.");
            }

            // Update entry with new data.
            user.Account.Email = userUpdateModel.Email;
            user.Name = userUpdateModel.Name;
            user.Surname = userUpdateModel.Surname;
            user.Birthday = userUpdateModel.Birthday;

            _context.PubHubUsers.Update(user);
            await _context.SaveChangesAsync();

            return Ok();
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
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            // Get account ID.
            var accountId = await _context.PubHubUsers
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == null)
            {
                return NotFound($"No user with ID: {id}");
            }

            // Delete account.
            int updatedRows = await _context.Users
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
            if (updatedRows > 0)
            {
                return Ok();
            }

            return NotFound($"Unable to delete account with ID: {id}");
        }
    }
}
