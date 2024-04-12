using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Genres;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class GenresController : ControllerBase
    {
        private readonly PubHubContext _context;
        private readonly AccessService _accessService;
        private readonly ILogger<GenresController> _logger;

        public GenresController(ILogger<GenresController> logger, PubHubContext context, AccessService accessService)
        {
            _context = context;
            _accessService = accessService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<GenreInfoModel>))]
        public async Task<IResult> GetGenresAsync([FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var genres = await _context.Set<Genre>()
                .Select(genre => new GenreInfoModel
                {
                    Id = genre.Id,
                    Name = genre.Name
                })
                .ToListAsync();

            return Results.Ok(genres);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenreInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetGenreAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var genre = await _context.Set<Genre>()
                 .Select(genre => new GenreInfoModel
                 {
                     Id = genre.Id,
                     Name = genre.Name
                 })
                .FirstOrDefaultAsync(genre => genre.Id == id);

            if (genre is null)
                return Results.Problem(
                   statusCode: NotFoundSpecification.STATUS_CODE,
                   title: NotFoundSpecification.TITLE,
                   detail: "No genre with the given ID was found",
                   extensions: new Dictionary<string, object?>
                   {
                        { "Id", id }
                   });

            return Results.Ok(genre);
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenreInfoModel))]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenreInfoModel))]
        public async Task<IResult> AddGenreAsync([FromBody] GenreCreateModel genreModel, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var entityGenre = await _context.Set<Genre>()
                .FirstOrDefaultAsync(genre => genre.Name.ToUpper() == genreModel.Name.ToUpper());

            if (entityGenre is not null)
                return Results.Ok(new GenreInfoModel
                {
                    Name = entityGenre.Name,
                    Id = entityGenre.Id
                });

            var genre = new Genre
            {
                Name = genreModel.Name
            };

            await _context.Set<Genre>().AddAsync(genre);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when adding genre.");

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the genre couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Genre", genreModel.Name }
                    });
            }
            entityGenre = await _context.Set<Genre>()
                .FirstOrDefaultAsync(genre => genre.Name.ToUpper() == genreModel.Name.ToUpper());

            return Results.Created($"genres/{entityGenre!.Id}", new GenreInfoModel
            {
                Name = entityGenre.Name,
                Id = entityGenre.Id
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> DeleteGenreAsync(Guid id, [FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId, User)
                .CheckWhitelistEndpoint(GetType().Name)
                .AllowOperator()
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

            var entityGenre = await _context.Set<Genre>()
                .FirstOrDefaultAsync(genre => genre.Id == id);

            if (entityGenre is null)
                return Results.Problem(
                    statusCode: NotFoundSpecification.STATUS_CODE,
                    title: NotFoundSpecification.TITLE,
                    detail: "No genres with the given ID was found.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            _context.Set<Genre>().Remove(entityGenre);

            if (await _context.SaveChangesAsync() == NO_CHANGES)
            {
                _logger.LogError("Couldn't save changes to the database when deleting genre: {GenreId}", entityGenre.Id);

                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the genre couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });
            }

            return Results.Ok();
        }
    }
}
