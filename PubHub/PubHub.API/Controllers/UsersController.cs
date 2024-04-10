﻿using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models.Authors;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;
using PubHub.Common.Models.Users;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public sealed class UsersController(ILogger<UsersController> logger, PubHubContext context, WhitelistService whitelistService) : Controller
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly PubHubContext _context = context;
        private readonly WhitelistService _whitelistService = whitelistService;

        /// <summary>
        /// Get all general information about a specific user.
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <response code="200">Success. User information was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetUserAsync(Guid id, [FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            var userInfo = await _context.GetUserInfoAsync(id);
            if (userInfo == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No user with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            return Results.Ok(userInfo);
        }

        /// <summary>
        /// Get all books for a specific user.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <response code="200">Success. All books of the user was retreived.</response>
        /// <response code="404">The user wasn't found.</response>
        [HttpGet("{id}/books")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBooksAsync(Guid id, [FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            // Check if user exists.
            if (!await _context.Set<User>().AnyAsync(u => u.Id == id))
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No user with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
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
                .Include(ub => ub.Book)
                    .ThenInclude(b => b!.BookAuthors)
                        .ThenInclude(ba => ba.Author)
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
                    Summary = ub.Book!.Summary,
                    Genres = ub.Book.BookGenres.Select(bookGenres => new GenreInfoModel
                    {
                        Id = bookGenres.GenreId,
                        Name = bookGenres.Genre!.Name
                    }).ToList(),
                    Authors = ub.Book.BookAuthors.Select(bookAuthors => new AuthorInfoModel
                    {
                        Id = bookAuthors.AuthorId,
                        Name = bookAuthors.Author!.Name
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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> UpdateUserAsync(Guid id, [FromBody] UserUpdateModel userUpdateModel, [FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

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
                    detail: "No user with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
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
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when updating user: {UserId}", user.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the user couldn't be updated. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "User", updatedUser }
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeleteUserAsync(Guid id, [FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

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
                    detail: "No user with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Delete account.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.DeletedDate, DateTime.UtcNow));
            if (updatedRows == 0)
            {
                Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to delete user account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
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
        [HttpDelete("{id}/suspend-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IResult> SuspendUserAsync(Guid id, [FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.ToLower() == AccountTypeConstants.SUSPENDED_ACCOUNT_TYPE)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();

            if (accountTypeId == INVALID_ENTITY_ID)
            {
                _logger.LogError("Unable to get account type: {TypeName}", AccountTypeConstants.SUSPENDED_ACCOUNT_TYPE);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Something went wrong and the user couldn't be suspended. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Get account ID.
            var accountId = await _context.Set<User>()
                .Where(u => u.Id == id)
                .Select(u => u.AccountId)
                .SingleOrDefaultAsync();
            if (accountId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No user with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Set account type.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.AccountTypeId, accountTypeId));
            if (updatedRows == 0)
            {
                _logger.LogError("Unable to set account type: {TypeId}", accountTypeId);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to suspend user account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id}
                    });
            }

            return Results.Ok();
        }
    }
}
