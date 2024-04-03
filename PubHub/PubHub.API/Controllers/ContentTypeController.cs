using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Controllers.Problems;
using PubHub.API.Domain;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class ContentTypesController : ControllerBase
    {
        private readonly PubHubContext _context;

        public ContentTypesController(PubHubContext context)
        {
            _context = context;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ContentTypeInfoModel>))]
        public async Task<IResult> GetContentTypesAsync()
        {
            var contentTypes = await _context.Set<Domain.Entities.ContentType>()
                .Select(contentType => new ContentTypeInfoModel
                {
                    Id = contentType.Id,
                    Name = contentType.Name
                })
                .ToListAsync();

            return Results.Ok(contentTypes);
        }
    }
}
