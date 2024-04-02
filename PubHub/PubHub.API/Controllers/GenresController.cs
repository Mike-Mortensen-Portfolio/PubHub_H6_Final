﻿using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Genres;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class GenresController : ControllerBase
    {
        private readonly PubHubContext _context;

        public GenresController(PubHubContext context)
        {
            _context = context;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<GenreInfoModel>))]
        public async Task<IResult> GetGenreAsync()
        {
            var genres = await _context.Set<Genre>()
                .Select(genre => new GenreInfoModel
                {
                    Id = genre.Id,
                    Name = genre.Name
                })
                .ToListAsync();

            return Results.Ok(genres);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenreInfoModel))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IResult> GetGenreAsync(Guid id)
        {
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
        public async Task<IResult> AddGenreAsync([FromBody] GenreCreateModel genreModel)
        {
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
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the genre couldn't be created. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Genre", genreModel.Name }
                    });

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
        public async Task<IResult> DeleteGenreAsync(Guid id)
        {
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
                return Results.Problem(
                    statusCode: InternalServerErrorSpecification.STATUS_CODE,
                    title: InternalServerErrorSpecification.TITLE,
                    detail: "Something went wrong and the genre couldn't be deleted. Please try again.",
                    extensions: new Dictionary<string, object?>
                    {
                        { "Id", id }
                    });

            return Results.Ok();
        }
    }
}