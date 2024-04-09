using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain;
using PubHub.API.Domain.Auth;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class ContentTypesController : ControllerBase
    {
        private readonly PubHubContext _context;
        private readonly WhitelistService _whitelistService;

        public ContentTypesController(PubHubContext context, WhitelistService whitelistService)
        {
            _context = context;
            _whitelistService = whitelistService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ContentTypeInfoModel>))]
        public async Task<IResult> GetContentTypesAsync([FromHeader] string appId)
        {
            if (!_whitelistService.TryVerifyApplicationAccess(appId, GetType().Name, out IResult? problem))
                return problem;

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
