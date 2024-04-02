using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Extensions;
using PubHub.Common.Models.Books;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public sealed class BooksController : ControllerBase
    {
        private readonly PubHubContext _context;

        public BooksController(PubHubContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all books that matches the criteria <paramref name="queryOptions"/>
        /// </summary>
        /// <param name="BookQuery">Filtering options</param>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<BookInfoModel>))]
        public async Task<IResult> GetBooksAsync([FromQuery] BookQuery queryOptions)
        {
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetBookAsync(Guid id)
        {
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

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
        public async Task<IResult> AddBookAsync([FromBody] BookCreateModel createModel)
        {
            var books = _context.Set<Book>();
            var existingBook = await books
                .FirstOrDefaultAsync(book =>
                    book.ContentTypeId == createModel.ContentTypeId &&
                    book.PublisherId == createModel.PublisherId &&
                    book.Length == createModel.Length &&
                    book.PublicationDate == createModel.PublicationDate &&
                    book.Title == createModel.Title &&
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
                Title = createModel.Title
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

            if (!(await _context.SaveChangesAsync() > 0))
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", createdBook }
                    });

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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BookInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity, Type = typeof(ProblemDetails))]
        public async Task<IResult> UpdateBookAsync(Guid id, [FromBody] BookUpdateModel updateModel)
        {
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

            if (!(await _context.SaveChangesAsync() > 0))
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

            if (!(await _context.SaveChangesAsync() > 0))
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", existingBook }
                    });

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
        public async Task<IResult> DeleteBookAsync(Guid id)
        {
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

            _context.Set<Book>().Remove(book);

            if (!(await _context.SaveChangesAsync() > 0))
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the book couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Book", book }
                    });

            return Results.Ok();
        }
    }
}
