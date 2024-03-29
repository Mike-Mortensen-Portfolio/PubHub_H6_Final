﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public sealed class UsersController(PubHubContext context) : Controller
    {
        private readonly PubHubContext _context = context;

        /// <summary>
        /// Add a new user account to PubHub.
        /// </summary>
        /// <param name="userCreateModel">User information.</param>
        /// <response code="200">Success. A new user account was created.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IResult> AddUserAsync([FromBody] UserCreateModel userCreateModel)
        {
            // TODO (SIA): Validate model.

            // Get account type Id.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(AccountTypeConstants.USER_ACCOUNT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: $"Unable to find account type: '{AccountTypeConstants.USER_ACCOUNT_TYPE}'",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", accountTypeId }
                    });
            }

            // Create user account.
            // TODO (SIA): Add more account related data to fully set up an account.
            var newUser = new User()
            {
                Name = userCreateModel.Name,
                Surname = userCreateModel.Surname,
                Birthday = userCreateModel.Birthday,
                Account = new()
                {
                    Email = userCreateModel.Account.Email,
                    AccountTypeId = accountTypeId
                }
            };

            await _context.Set<User>()
                .AddAsync(newUser);

            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                        statusCode: InternalServerErrorSpecification.STATUS_CODE,
                        title: InternalServerErrorSpecification.TITLE,
                        detail: "Something went wrong and the user couldn't be created. Please try again.",
                        extensions: new Dictionary<string, object?>
                        {
                            {"User", newUser}
                        });
            }

            newUser = await _context.Set<User>()
                .Include(user => user.Account)
                .FirstOrDefaultAsync(user => user.Account!.NormalizedEmail == newUser.Account.NormalizedEmail);

            return Results.Created($"users/{newUser!.Id}", newUser);
        }

        /// <summary>
        /// Get all general information about a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <response code="200">Success. User information was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public IResult GetUser(int id)
        {
            var userModel = _context.Set<User>()
                .Include(u => u.Account)
                .ThenInclude(a => a!.AccountType)
                .Select(u => new UserInfoModel()
                {
                    Id = u.Id,
                    Email = u.Account!.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    Birthday = u.Birthday,
                    AccountType = u.Account!.AccountType!.Name
                })
                .FirstOrDefault(u => u.Id == id);
            if (userModel == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: $"No user with Id: {id}",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            return Results.Ok(userModel);
        }

        /// <summary>
        /// Get all books for a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <response code="200">Success. All books of the user was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}/books")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBooksAsync(int id)
        {
            // Check if user exists.
            if (!await _context.Set<User>().AnyAsync(u => u.Id == id))
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: $"No user with Id: {id}",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            // Retreive all books.
            var bookModels = await _context.Set<UserBook>()
                .Include(ub => ub.Book)
                    .ThenInclude(b => b!.ContentType)
                .Include(ub => ub.Book)
                    .ThenInclude(b => b!.Publisher)
                .Include(ub => ub.Book)
                    .ThenInclude(b => b!.BookGenres)
                        .ThenInclude(bg => bg.Genre)
                .Where(ub => ub.UserId == id)
                .Select(ub => new BookInfoModel()
                {
                    ContentType = new ContentTypeInfoModel
                    {
                        Id = ub.Book!.ContentTypeId,
                        Name = ub.Book.ContentType!.Name
                    },
                    CoverImage = ub.Book.CoverImage,
                    Id = ub.Book.Id,
                    Length = ub.Book.Length,
                    PublicationDate = ub.Book.PublicationDate,
                    Publisher = new BookPublisherModel
                    {
                        Id = ub.Book.PublisherId,
                        Name = ub.Book.Publisher!.Name
                    },
                    Title = ub.Book!.Title,
                    Genres = ub.Book.BookGenres.Select(bookGenres => new GenreInfoModel
                    {
                        Id = bookGenres.GenreId,
                        Name = bookGenres.Genre!.Name
                    }).ToList()
                })
                .ToListAsync();

            return Results.Ok(bookModels);
        }

        /// <summary>
        /// Update information of a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <param name="userUpdateModel">Information to update the existing user with.</param>
        /// <response code="200">Success. The user was updated.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> UpdateUserAsync(int id, [FromBody] UserUpdateModel userUpdateModel)
        {
            // TODO (SIA): Validate model.

            // Get current entry.
            var user = _context.Set<User>()
                .Include(u => u.Account)
                .FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: $"No user with Id: {id}",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            // Update entry with new data.
            user.Account!.Email = userUpdateModel.Account.Email;
            user.Name = userUpdateModel.Name;
            user.Surname = userUpdateModel.Surname;
            user.Birthday = userUpdateModel.Birthday;

            var updatedUser = new UserInfoModel
            {
                Id = user.Id,
                AccountType = user.Account.AccountType.Name,
                Birthday = user.Birthday,
                Email = user.Account.Email,
                Name = user.Name,
                Surname = user.Surname
            };

            _context.Set<User>().Update(user);
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                        statusCode: InternalServerErrorSpecification.STATUS_CODE,
                        title: InternalServerErrorSpecification.TITLE,
                        detail: "Something went wrong and the user couldn't be created. Please try again.",
                        extensions: new Dictionary<string, object?>
                        {
                            {"User", updatedUser}
                        });
            }

            return Results.Ok(updatedUser);
        }

        /// <summary>
        /// Terminate the account of a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <response code="200">Success. The user was deleted.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeleteUserAsync(int id)
        {
            // Get account Id.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: $"No user with Id: {id}",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            // Delete account.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
            if (updatedRows == 0)
            {
                Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to delete user account",
                    extensions: new Dictionary<string, object?>
                    {
                        { "UserId", id}
                    });
            }

            return Results.Ok();
        }

        /// <summary>
        /// Suspend the account of a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <response code="200">Success. The user was suspended.</response>
        /// <response code="404">The user wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete("{id}/suspend-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IResult> SuspendUserAsync(int id)
        {
            // Get account type Id.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(AccountTypeConstants.SUSPENDED_ACCOUNT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (accountTypeId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Something went wrong and the user couldn't be suspended. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            // Get account Id.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: $"No user with Id: {id}",
                    extensions: new Dictionary<string, object?>
                    {
                        {"Id", id }
                    });
            }

            // Set account type.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.AccountTypeId, accountTypeId));
            if (updatedRows == 0)
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to suspend user account",
                    extensions: new Dictionary<string, object?>
                    {
                        { "UserId", id}
                    });
            }

            return Results.Ok();
        }
    }
}
