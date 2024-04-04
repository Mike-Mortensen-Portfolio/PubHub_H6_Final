using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Authors;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthorsController : ControllerBase
    {
        private readonly ILogger<AuthorsController> _logger;
        private readonly PubHubContext _context;

        public AuthorsController(ILogger<AuthorsController> logger, PubHubContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<AuthorInfoModel>))]
        public async Task<IResult> GetAuthorsAsync()
        {
            var authors = await _context.Set<Author>()
                .Select(author => new AuthorInfoModel
                {
                    Id = author.Id,
                    Name = author.Name
                })
                .ToListAsync();

            return Results.Ok(authors);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetAuthorAsync(Guid id)
        {
            var author = await _context.Set<Author>()
                 .Select(author => new AuthorInfoModel
                 {
                     Id = author.Id,
                     Name = author.Name
                 })
                .FirstOrDefaultAsync(author => author.Id == id);

            if (author is null)
                return Results.Problem(
                   statusCode: NotFoundSpecification.STATUS_CODE,
                   title: NotFoundSpecification.TITLE,
                   detail: "No author with the given ID was found",
                   extensions: new Dictionary<string, object?>
                   {
                        { "Id", id }
                   });

            return Results.Ok(author);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthorInfoModel))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthorInfoModel))]
        public async Task<IResult> AddAuthorAsync([FromBody] AuthorCreateModel authorModel)
        {
            var entityAuthor = await _context.Set<Author>()
                .FirstOrDefaultAsync(author => author.Name.ToUpper() == authorModel.Name.ToUpper());

            if (entityAuthor is not null)
                return Results.Ok(new AuthorInfoModel
                {
                    Name = entityAuthor.Name,
                    Id = entityAuthor.Id
                });

            var author = new Author
            {
                Name = authorModel.Name
            };

            await _context.Set<Author>().AddAsync(author);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when adding author.");

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the author couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Author", authorModel.Name }
                    });
            }
                

            entityAuthor = await _context.Set<Author>()
                .FirstOrDefaultAsync(author => author.Name.ToUpper() == authorModel.Name.ToUpper());

            return Results.Created($"authors/{entityAuthor!.Id}", new AuthorInfoModel
            {
                Name = entityAuthor.Name,
                Id = entityAuthor.Id
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeleteAuthorAsync(Guid id)
        {
            var entityAuthor = await _context.Set<Author>()
                .FirstOrDefaultAsync(author => author.Id == id);

            if (entityAuthor is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No authors with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            _context.Set<Author>().Remove(entityAuthor);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when removing author: {AuthorId}", entityAuthor.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the author couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            return Results.Ok();
        }
    }
}
