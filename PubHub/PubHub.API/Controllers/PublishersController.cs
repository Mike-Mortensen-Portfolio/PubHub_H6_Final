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
using PubHub.Common.Models.Authors;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;
using PubHub.Common.Models.Publishers;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class PublishersController : Controller
    {
        private readonly ILogger<PublishersController> _logger;
        private readonly PubHubContext _context;
        private readonly UserManager<Account> _userManager;
        private readonly AccessService _accessService;

        public PublishersController(ILogger<PublishersController> logger, PubHubContext context, UserManager<Account> userManager, AccessService accessService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _accessService = accessService;
        }

        /// <summary>
        /// Get a list of all publishers.
        /// </summary>
        /// <param name="publisherCreateModel">Publisher information.</param>
        /// <response code="200">Success. A new publisher account was created.</response>
        /// <response code="400">Invalid model data or format.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PublisherInfoModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IResult> AddPublisherAsync([FromBody] PublisherCreateModel publisherCreateModel, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            // TODO (SIA): Validate model.

            // Check if a publisher like this already exists.
            var existingPublisher = await _context.Users.
                FirstOrDefaultAsync(account => account.NormalizedEmail == publisherCreateModel.Account.Email.ToUpperInvariant());
            if (existingPublisher is not null)
                return Results.Problem(
                    type: DuplicateProblemSpecification.TYPE,
                    statusCode: DuplicateProblemSpecification.STATUS_CODE,
                    title: DuplicateProblemSpecification.TITLE,
                    detail: "A matching publisher already exists",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", existingPublisher.Id }
                    });

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.ToLower() == AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == Guid.Empty)
            {
                _logger.LogError("Unable to get account type: {TypeName}", AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to find account type.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "AccountType", AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE }
                    });
            }

            // Create publisher account.
            Account account = new()
            {
                Email = publisherCreateModel.Account.Email,
                AccountTypeId = accountTypeId
            };
            var createIdentityResult = await _userManager.CreateAsync(account, publisherCreateModel.Account.Password);
            if (!createIdentityResult.Succeeded)
            {
                _logger.LogError("Unable to create account with type ID: {AccountTypeId}", accountTypeId);

                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: $"Unable to create user account.");
            }

            // Create publisher associated with the account.
            var addedPublisher = new Publisher()
            {
                Name = publisherCreateModel.Name,
                Account = account
            };
            await _context.Set<Publisher>()
                .AddAsync(addedPublisher);
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when adding publisher.");

                // Attempt to delete account.
                var deleteIdentityResult = await _userManager.DeleteAsync(account);
                if (!deleteIdentityResult.Succeeded)
                {
                    _logger.LogError("Failed to delete account: {Account}", account.Id);
                }

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to save changes to the database.");
            }

            // Respond with newly added publisher.
            PublisherInfoModel publisherInfo = new()
            {
                Id = addedPublisher.Id,
                Email = addedPublisher.Account.Email,
                Name = addedPublisher.Name
            };

            return Results.Created($"publishers/{publisherInfo.Id}", publisherInfo);
        }

        /// <summary>
        /// Get all general information about a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. Publisher information was retreived.</response>
        /// <response code="404">The publisher wasn't found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublisherInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetPublisherAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowPublisher(id)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var publisherModel = await _context.GetPublisherInfoAsync(id);
            if (publisherModel == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            return Results.Ok(publisherModel);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublisherInfoModel[]))]
        public async Task<IResult> GetPublishersAsync([FromQuery] PublisherQuery query, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var publishers = await _context.Set<Publisher>()
                .Include(p => p.Account)
                .Filter(query)
                .Select(p => new PublisherInfoModel()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Account!.Email
                })
                .ToArrayAsync();

            return Results.Ok(publishers);
        }

        /// <summary>
        /// Get all books for a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. All books of the publisher was retreived.</response>
        /// <response code="404">The publisher wasn't found.</response>
        [HttpGet("{id}/books")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<BookInfoModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBooksAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowUser()
                .AllowPublisher(id)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            // Check if publisher exists.
            if (!await _context.Set<Publisher>().AnyAsync(u => u.Id == id))
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Retreive all books.
            var bookModels = await _context.Set<Book>()
                .Include(b => b.Publisher)
                .Include(b => b.ContentType)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Where(b => b.Publisher!.Id == id)
                .Select(b => new BookInfoModel()
                {
                    ContentType = new ContentTypeInfoModel
                    {
                        Id = b.ContentTypeId,
                        Name = b.ContentType!.Name
                    },
                    CoverImage = b.CoverImage,
                    Id = b.Id,
                    Length = b.Length,
                    PublicationDate = b.PublicationDate,
                    Publisher = new BookPublisherModel
                    {
                        Id = b.Publisher!.Id,
                        Name = b.Publisher.Name
                    },
                    Title = b.Title,
                    Summary = b.Summary,
                    Genres = b.BookGenres.Select(bookGenres => new GenreInfoModel
                    {
                        Id = bookGenres.GenreId,
                        Name = bookGenres.Genre!.Name
                    }).ToList(),
                    Authors = b.BookAuthors.Select(bookAuthors => new AuthorInfoModel
                    {
                        Id = bookAuthors.AuthorId,
                        Name = bookAuthors.Author!.Name
                    }).ToList()
                })
                .ToListAsync();

            return Results.Ok(bookModels);
        }

        /// <summary>
        /// Get specific book information on the authenticated publisher.
        /// </summary>
        /// <param name="id">Id of user.</param>
        /// <param name="bookId">Id of the book.</param>
        /// <returns></returns>
        [HttpGet("{id}/books/{bookId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookContentModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBookContentAsync(Guid id, Guid bookId, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .AllowPublisher(id)
                .AllowPublisherOnlyIfOwns(bookId, _context)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var query = _context.Set<Book>()
                .Include(book => book.ContentType)
                .Include(book => book.Publisher)
                .Include(book => book.BookGenres)
                    .ThenInclude(bookGenres => bookGenres.Genre)
                .Include(book => book.BookAuthors)
                    .ThenInclude(bookAuthors => bookAuthors.Author);

            var entityBook = await query.Where(book => book.Id == id)
                .FirstOrDefaultAsync();

            if (entityBook is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "We couldn't locate a book with the given ID.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            var bookContent = new BookContentModel()
            {
                BookContent = entityBook.BookContent
            };

            return Results.Ok(bookContent);
        }

        /// <summary>
        /// Update information of a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <param name="publisherUpdateModel">Information to update the existing publisher with.</param>
        /// <response code="200">Success. The publisher was updated.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="404">The publisher wasn't found.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PublisherInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> UpdatePublisherAsync(Guid id, [FromBody] PublisherUpdateModel publisherUpdateModel, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowPublisher(id)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            // TODO (SIA): Validate model.

            // Get current entry.
            var publisher = await _context.Set<Publisher>()
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (publisher == null)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                       { "Id", id }
                    });
            }

            // Update entry with new data.
            if (publisherUpdateModel.Account != null &&
                publisher.Account != null)
            {
                publisher.Account.Email = publisherUpdateModel.Account.Email;
            }
            publisher.Name = publisherUpdateModel.Name;

            var updatedPublisher = new PublisherInfoModel()
            {
                Id = publisher.Id,
                Email = publisher.Account!.Email,
                Name = publisherUpdateModel.Name
            };

            _context.Set<Publisher>().Update(publisher);
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when updating publisher: {PublisherId}", publisher.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the publisher couldn't be updated. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Publisher", updatedPublisher }
                    });
            }

            // Retreive newly updated publisher.
            var publisherInfo = await _context.GetPublisherInfoAsync(id);

            return Results.Ok(publisherInfo);
        }

        /// <summary>
        /// Terminate the account of a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. The publisher was deleted.</response>
        /// <response code="404">The publisher wasn't found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeletePublisherAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowPublisher(id)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            // Check if publisher exists.
            if (!await _context.Set<Publisher>().AnyAsync(p => p.Id == id))
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Get account ID.
            var accountId = await _context.Set<Publisher>()
                .Where(p => p.Id == id)
                .Select(p => p.AccountId)
                .FirstOrDefaultAsync();
            if (accountId == INVALID_ENTITY_ID)
            {
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "Publisher with the given ID doesn't have an account.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            // Delete account.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.DeletedDate, DateTime.UtcNow));
            if (updatedRows < 1)
            {
                _logger.LogError("Couldn't save changes to the database when deleting account (ID: {AccountId}) of publisher (ID: {PublisherId}).", accountId, id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the publisher couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        ["PublisherId"] = id,
                        ["AccountId"] = accountId
                    });
            }

            return Results.Ok();
        }
    }
}
