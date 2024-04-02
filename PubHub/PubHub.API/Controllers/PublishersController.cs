using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;
using PubHub.Common.Models.Publishers;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class PublishersController(PubHubContext context) : Controller
    {
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons
        private readonly PubHubContext _context = context;

        /// <summary>
        /// Get a list of all publishers.
        /// </summary>
        /// <param name="publisherCreateModel">Publisher information.</param>
        /// <response code="200">Success. A new publisher account was created.</response>
        /// <response code="400">Invalid model data or format.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PublisherInfoModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IResult> AddPublisherAsync([FromBody] PublisherCreateModel publisherCreateModel)
        {
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
                        {"Id", existingPublisher.Id}
                    });

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.ToLower() == AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE)
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == Guid.Empty)
            {
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
            // TODO (SIA): Add more account related data to fully set up an account.
            var addedPublisher = new Publisher()
            {
                Name = publisherCreateModel.Name,
                Account = new()
                {
                    Email = publisherCreateModel.Account.Email,
                    AccountTypeId = accountTypeId
                }
            };
            await _context.Set<Publisher>()
                .AddAsync(addedPublisher);
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: $"Unable to save changes to the database.");
            }

            // Retrieve newly added publisher.
            var publisherInfo = await GetPublisherInfoAsync(addedPublisher.Id);

            return Results.Created($"publishers/{publisherInfo!.Id}", publisherInfo);
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
        public async Task<IResult> GetPublisherAsync(Guid id)
        {
            var publisherModel = await GetPublisherInfoAsync(id);
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
        public async Task<IResult> GetPublishersAsync([FromQuery] PublisherQuery query)
        {
            var publishers = await _context.Set<Publisher>()
                 .Include(u => u.Account)
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
        public async Task<IResult> GetBooksAsync(Guid id)
        {
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
        public async Task<IResult> UpdatePublisherAsync(Guid id, [FromBody] PublisherUpdateModel publisherUpdateModel)
        {
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
            if (!(await _context.SaveChangesAsync() > 0))
            {
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
            var publisherInfo = await GetPublisherInfoAsync(id);

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
        public async Task<IResult> DeletePublisherAsync(Guid id)
        {
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
                // Unable to delete; report back.
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

        private async Task<PublisherInfoModel?> GetPublisherInfoAsync(Guid id) =>
            await _context.Set<Publisher>()
                .Include(p => p.Account)
                .Select(p => new PublisherInfoModel()
                {
                    Id = p.Id,
                    Email = p.Account!.Email ?? string.Empty,
                    Name = p.Name
                })
                .FirstOrDefaultAsync(p => p.Id == id);

    }
}
