using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Services;
using PubHub.Common;
using PubHub.Common.Models.Authors;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [EnableRateLimiting("concurrency")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public sealed class BooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly PubHubContext _context;
        private readonly AccessService _accessService;
        private readonly TypeLookupService _accessTypeLookupService;

        public BooksController(ILogger<BooksController> logger, PubHubContext context, AccessService accessService, TypeLookupService accessTypeLookupService)
        {
            _logger = logger;
            _context = context;
            _accessService = accessService;
            _accessTypeLookupService = accessTypeLookupService;
        }

        /// <summary>
        /// Get all books that matches the criteria <paramref name="queryOptions"/>
        /// </summary>
        /// <param name="BookQuery">Filtering options</param>
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<BookInfoModel>))]
        public async Task<IResult> GetBooksAsync([FromQuery] BookQuery queryOptions, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var query = _context.Set<Book>()
                .Include(book => book.ContentType)
                .Include(book => book.Publisher)
                .Include(book => book.BookGenres)
                    .ThenInclude(bookGenres => bookGenres.Genre)
                .Include(book => book.BookAuthors)
                    .ThenInclude(bookAuthors => bookAuthors.Author);

            var books = await query.Filter(queryOptions)
                .Select(book => new BookInfoModel
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
                        Id = book.Publisher!.Id,
                        Name = book.Publisher.Name
                    },
                    Title = book.Title,
                    Summary = book.Summary,
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

            return Results.Ok(books);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBookAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
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

            var book = new BookInfoModel
            {
                ContentType = new ContentTypeInfoModel
                {
                    Id = entityBook.ContentTypeId,
                    Name = entityBook.ContentType!.Name
                },
                CoverImage = entityBook.CoverImage,
                Id = entityBook.Id,
                Length = entityBook.Length,
                PublicationDate = entityBook.PublicationDate,
                Publisher = new BookPublisherModel
                {
                    Id = entityBook.Publisher!.Id,
                    Name = entityBook.Publisher.Name
                },
                Title = entityBook.Title,
                Summary = entityBook.Summary,
                Genres = entityBook.BookGenres.Select(bookGenres => new GenreInfoModel
                {
                    Id = bookGenres.GenreId,
                    Name = bookGenres.Genre!.Name
                }).ToList(),
                Authors = entityBook.BookAuthors.Select(bookAuthors => new AuthorInfoModel
                {
                    Id = bookAuthors.AuthorId,
                    Name = bookAuthors.Author!.Name
                }).ToList()
            };

            return Results.Ok(book);
        }

        [HttpPost("{id}/purchase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> PurchaseBookAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowUser()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            // Find book.
            var book = await _context.Set<Book>()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (book is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "We couldn't locate a book with the given ID.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            // Get user ID.
            var userId = User.GetSubjectId();

            // Check if user already owns the book.
            var alreadyOwned = await _context.Set<UserBook>()
                .AnyAsync(ub => ub.BookId == id && ub.UserId == userId);
            if (alreadyOwned)
                return Results.Problem(
                    type: DuplicateProblemSpecification.TYPE,
                    statusCode: DuplicateProblemSpecification.STATUS_CODE,
                    title: DuplicateProblemSpecification.TITLE,
                    detail: "A matching book already exists.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", book.Id }
                    });

            // Create a UserBook record.
            await _context.Set<UserBook>().AddAsync(new()
            {
                UserId = userId,
                AccessTypeId = _accessTypeLookupService.GetAccessTypeId(AccessTypeConstants.OWNER_ACCESS_TYPE) ?? Guid.Empty,
                ProgressInProcent = 0,
                AcquireDate = DateTime.UtcNow,
                Book = book
            });
            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when purchasing book: {BookId}", book.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be purchased. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "BookId", book.Id },
                        { "UserId", userId }
                    });
            }

            return Results.Ok();
        }

        [DisableRequestSizeLimit]
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        public async Task<IResult> AddBookAsync([FromBody] BookCreateModel createModel, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowPublisher(createModel.PublisherId)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var books = _context.Set<Book>();
            var existingBook = await books
                .FirstOrDefaultAsync(book =>
                    book.ContentTypeId == createModel.ContentTypeId &&
                    book.PublisherId == createModel.PublisherId &&
                    book.Length == createModel.Length &&
                    book.PublicationDate == createModel.PublicationDate &&
                    book.Title == createModel.Title &&
                    book.Summary == createModel.Summary &&
                    book.BookContent.Equals(createModel.BookContent));

            if (existingBook is not null)
                return Results.Problem(
                    type: DuplicateProblemSpecification.TYPE,
                    statusCode: DuplicateProblemSpecification.STATUS_CODE,
                    title: DuplicateProblemSpecification.TITLE,
                    detail: "A matching book already exists.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", existingBook.Id }
                    });

            var publisher = await _context.Set<Publisher>().FirstOrDefaultAsync(publisher => publisher.Id == createModel.PublisherId);

            if (publisher is null)
                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", createModel.PublisherId }
                    });

            var createdBook = new Book
            {
                BookContent = createModel.BookContent,
                ContentTypeId = createModel.ContentTypeId,
                CoverImage = createModel.CoverImage,
                IsHidden = createModel.IsHidden,
                Length = createModel.Length,
                PublicationDate = createModel.PublicationDate,
                PublisherId = createModel.PublisherId,
                Title = createModel.Title,
                Summary = createModel.Summary
            };

            foreach (var genreId in createModel.GenreIds)
            {
                var genre = await _context.Set<Genre>().FirstOrDefaultAsync(genre => genre.Id == genreId);
                if (genre is null)
                    return Results.Problem(
                        statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                        title: UnprocessableEntitySpecification.TITLE,
                        detail: "No genere with the given ID was found.",
                        extensions: new Dictionary<string, object?>
                        {
                            { "Id", genreId }
                        });

                createdBook.BookGenres.Add(new BookGenre
                {
                    Genre = genre,
                    Book = createdBook
                });
            }

            foreach (var authorId in createModel.AuthorIds)
            {
                var author = await _context.Set<Author>().FirstOrDefaultAsync(author => author.Id == authorId);
                if (author is null)
                    return Results.Problem(
                        statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                        title: UnprocessableEntitySpecification.TITLE,
                        detail: "No author with the given ID was found.",
                        extensions: new Dictionary<string, object?>
                        {
                            { "Id", authorId }
                        });

                createdBook.BookAuthors.Add(new BookAuthor
                {
                    Author = author,
                    Book = createdBook
                });
            }

            await books.AddAsync(createdBook);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when adding book.");

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", createdBook }
                    });
            }

            var bookInfo = await books
                .Include(book => book.ContentType)
                .Include(book => book.Publisher)
                .Include(book => book.BookGenres)
                    .ThenInclude(bookGenres => bookGenres.Genre)
                .Include(book => book.BookAuthors)
                    .ThenInclude(bookAuthors => bookAuthors.Author)
                .Where(book =>
                book.ContentTypeId == createdBook.ContentTypeId &&
                book.PublisherId == createdBook.PublisherId &&
                book.Length == createdBook.Length &&
                book.PublicationDate == createdBook.PublicationDate &&
                book.Title == createdBook.Title &&
                book.Summary == createdBook.Summary &&
                book.BookContent.Equals(createdBook.BookContent))
                .Select(book => new BookInfoModel
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
                        Id = book.Publisher!.Id,
                        Name = book.Publisher.Name
                    },
                    Title = book.Title,
                    Summary = book.Summary,
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
                }).FirstOrDefaultAsync();

            return Results.Created($"books/{bookInfo!.Id}", bookInfo);
        }

        [DisableRequestSizeLimit]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IResult> UpdateBookAsync(Guid id, [FromBody] BookUpdateModel updateModel, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            //  TODO (MSM)  Implement updates on genres and authors
            var books = _context.Set<Book>();
            var existingBook = await books
                .FirstOrDefaultAsync(book => book.Id == id);

            if (existingBook is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No book with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            if (!_accessService.AccessFor(appId, User)
                .AllowPublisher(existingBook.PublisherId)
                .AllowOperator()
                .TryVerify(out IResult? subjectAccessProblem))
                return subjectAccessProblem;

            var publisher = await _context.Set<Publisher>().FirstOrDefaultAsync(publisher => publisher.Id == updateModel.PublisherId);

            if (publisher is null)
                return Results.Problem(
                    statusCode: UnprocessableEntitySpecification.STATUS_CODE,
                    title: UnprocessableEntitySpecification.TITLE,
                    detail: "No publisher with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", updateModel.PublisherId }
                    });

            existingBook.BookContent = updateModel.BookContent;
            existingBook.ContentTypeId = updateModel.ContentTypeId;
            existingBook.CoverImage = updateModel.CoverImage;
            existingBook.IsHidden = updateModel.IsHidden;
            existingBook.Length = updateModel.Length;
            existingBook.PublicationDate = updateModel.PublicationDate;
            existingBook.PublisherId = updateModel.PublisherId;
            existingBook.Title = updateModel.Title;
            existingBook.Summary = updateModel.Summary;

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when updating book: {BookId}", existingBook.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", existingBook },
                        { "GenresIds", updateModel.GenreIds },
                        { "AuthorIds", updateModel.AuthorIds }
                    });
            }

            //foreach (var genreId in updateModel.GenreIds)
            //{
            //    var genre = await _context.Set<Genre>().FirstOrDefaultAsync(genre => genre.Id == genreId);
            //    if (genre is null)
            //        return Results.Problem(
            //            statusCode: UnprocessableEntitySpecification.STATUS_CODE,
            //            title: UnprocessableEntitySpecification.TITLE,
            //            detail: "No genere with the given Id was found",
            //            extensions: new Dictionary<string, object?>
            //            {
            //                {"Id", genreId}
            //            });

            //    existingBook.BookGenres.Add(new BookGenre
            //    {
            //        Genre = genre,
            //        Book = existingBook
            //    });
            //}

            //foreach (var authorId in updateModel.AuthorIds)
            //{
            //    var author = await _context.Set<Author>().FirstOrDefaultAsync(author => author.Id == authorId);
            //    if (author is null)
            //        return Results.Problem(
            //            statusCode: UnprocessableEntitySpecification.STATUS_CODE,
            //            title: UnprocessableEntitySpecification.TITLE,
            //            detail: "No author with the given Id was found",
            //            extensions: new Dictionary<string, object?>
            //            {
            //                {"Id", authorId}
            //            });

            //    existingBook.BookAuthors.Add(new BookAuthor
            //    {
            //        Author = author,
            //        Book = existingBook
            //    });
            //}

            books.Update(existingBook);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when updating book: {BookId}", existingBook.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", existingBook.Id }
                    });
            }

            var bookInfo = await books
                .Include(book => book.ContentType)
                .Include(book => book.Publisher)
                .Include(book => book.BookGenres)
                    .ThenInclude(bookGenres => bookGenres.Genre)
                .Include(book => book.BookAuthors)
                    .ThenInclude(bookAuthors => bookAuthors.Author)
                .Where(book => book.Id == id)
                .Select(book => new BookInfoModel
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
                        Id = book.Publisher!.Id,
                        Name = book.Publisher.Name
                    },
                    Title = book.Title,
                    Summary = book.Summary,
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
                }).FirstOrDefaultAsync();

            return Results.Ok(bookInfo);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeleteBookAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var book = await _context.Set<Book>()
                .FirstOrDefaultAsync(book => book.Id == id);

            if (book is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No book with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            if (!_accessService.AccessFor(appId, User)
                .AllowPublisher(book.PublisherId)
                .AllowOperator()
                .TryVerify(out IResult? subjectAccessProblem))
                return subjectAccessProblem;

            _context.Set<Book>().Remove(book);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when deleting book: {BookId}", book.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", book }
                    });
            }

            return Results.Ok();
        }
    }
}
