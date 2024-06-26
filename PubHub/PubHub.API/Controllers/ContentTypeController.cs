﻿using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("limit-by-consumer-id")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class ContentTypesController : ControllerBase
    {
        private readonly PubHubContext _context;
        private readonly IAccessService _accessService;

        public ContentTypesController(PubHubContext context, IAccessService accessService)
        {
            _context = context;
            _accessService = accessService;
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ContentTypeInfoModel>))]
        public async Task<IResult> GetContentTypesAsync([FromHeader] string appId)
        {
            if (!_accessService.AccessFor(appId)
                .CheckWhitelistEndpoint(GetType().Name)
                .TryVerify(out IResult? accessProblem))
                return accessProblem;

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
