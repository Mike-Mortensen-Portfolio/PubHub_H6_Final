using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Identity;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishersController(PubHubContext context) : Controller
    {
        private readonly PubHubContext _context = context;

        /// <summary>
        /// Add a new publisher account to PubHub.
        /// </summary>
        /// <param name="publisherCreateModel">Publisher information.</param>
        /// <response code="200">Success. A new publisher account was created.</response>
        /// <response code="400">Invalid model data or format.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> AddPublisherAsync([FromBody] PublisherCreateModel publisherCreateModel)
        {
            // TODO (SIA): Validate model.

            // Get account type ID.
            var accountTypeId = await _context.Set<AccountType>()
                .Where(a => a.Name.Equals(AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                .Select(a => a.Id)
                .FirstOrDefaultAsync();
            if (accountTypeId == 0)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to find account type: '{AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE}'");
            }

            // Create publisher account.
            // TODO (SIA): Add more account related data to fully set up an account.
            await _context.Set<Publisher>()
                .AddAsync(new Publisher()
                {
                    Name = publisherCreateModel.Name,
                    Account = new()
                    {
                        Email = publisherCreateModel.Account.Email,
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
        /// Get all general information about a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. Publisher information was retreived.</response>
        /// <response code="404">The publisher wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> GetPublisherAsync(int id)
        {
            var publisherModel = await _context.Set<Publisher>()
                .Include(p => p.Account)
                .Select(p => new PublisherInfoModel()
                {
                    Id = p.Id,
                    Email = p.Account!.Email,
                    Name = p.Name
                })
                .FirstOrDefaultAsync(p => p.Id == id);
            if (publisherModel == null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No publisher with ID: {id}");
            }

            return Results.Ok(publisherModel);
        }

        /// <summary>
        /// Get a list of all publishers.
        /// </summary>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> GetPublishersAsync([FromQuery] PublisherQuery query)
        {
            var publishers = await _context.Set<Publisher>()
                .Filter(query)
                .ToArrayAsync();

            return Results.Ok(publishers);
        }

        /// <summary>
        /// Get all books for a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. All books of the publisher was retreived.</response>
        /// <response code="404">The publisher wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpGet("{id}/books")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> GetBooksAsync(int id)
        {
            // Check if publisher exists.
            if (!await _context.Set<Publisher>().AnyAsync(u => u.Id == id))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No publisher with ID: {id}");
            }

            // Retreive all books.
            var bookModels = await _context.Set<Book>()
                .Include(b => b.Publisher)
                .Include(b => b!.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude (ba => ba.Author)
                .Include(b => b.ContentType)
                .Where(b => b.Publisher!.Id == id)
                .Select(book => new BookInfoModel()
                {
                    ContentType = new ContentTypeInfoModel
                    {
                        Id = book.ContentTypeId,
                        Name = book.ContentType!.Name
                    },
                    CoverImage = book.CoverImage,
                    Id = book.Id,
                    Length = book.Length,
                    PublicationDate = book.PublicationDate,
                    Publisher = new BookPublisherModel
                    {
                        Id = book.PublisherId,
                        Name = book.Publisher!.Name
                    },
                    Title = book.Title,
                    Genres = book.BookGenres.Select(bookGenres => new GenreInfoModel
                    {
                        Id = bookGenres.GenreId,
                        Name = bookGenres.Genre!.Name
                    }).ToList(),
                    Authors = book.BookAuthors.Select(bookAuthors => new AuthorInfoModel
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
        /// <response code="500">Unexpected error.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> UpdatePublisherAsync(int id, [FromBody] PublisherUpdateModel publisherUpdateModel)
        {
            // TODO (SIA): Validate model.

            // Get current entry.
            var publisher = await _context.Set<Publisher>()
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.Id == id);
            if (publisher == null)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No publisher with ID: {id}");
            }

            // Update entry with new data.
            if (publisherUpdateModel.Account != null &&
                publisher.Account != null)
            {
                publisher.Account.Email = publisherUpdateModel.Account.Email;
            }
            publisher.Name = publisherUpdateModel.Name;

            _context.Set<Publisher>().Update(publisher);
            if (!(await _context.SaveChangesAsync() > 0))
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to save changes to the database.");
            }

            return Results.Ok();
        }

        /// <summary>
        /// Terminate the account of a specific publisher.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        /// <response code="200">Success. The publisher was deleted.</response>
        /// <response code="404">The publisher wasn't found.</response>
        /// <response code="500">Unexpected error.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IResult> DeletePublisherAsync(int id)
        {
            const int INVALID_ID = 0;
            const int NO_ACCOUNT = -1;

            // Get account ID.
            var accountId = await _context.Set<Publisher>()
                .Where(p => p.Id == id)
                .Select(p => p.AccountId ?? NO_ACCOUNT)
                .FirstOrDefaultAsync();
            if (accountId == INVALID_ID)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"No publisher with ID: {id}");
            }
            if (accountId == NO_ACCOUNT)
            {
                return Results.Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    detail: $"Publisher (ID: {id}) doesn't have an account.");
            }

            // Delete account.
            int updatedRows = await _context.Set<Account>()
                .Where(u => u.Id == accountId)
                .ExecuteUpdateAsync(u => u.SetProperty(u => u.IsDeleted, true));
            if (updatedRows < 1)
            {
                // Unable to delete; report back.
                Dictionary<string, object?> extensions = new()
                {
                    ["PublisherId"] = id,
                    ["AccountId"] = accountId
                };

                return Results.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    detail: $"Unable to delete account (ID: {accountId}) of publisher (ID: {id}).",
                    extensions: extensions);
            }

            return Results.Ok();
        }
    }
}
