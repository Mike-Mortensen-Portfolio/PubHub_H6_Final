using System.Security.Claims;
using Microsoft.Extensions.Options;
using PubHub.API.Domain.Services;

namespace PubHub.API.Domain.Auth
{
    public class AccessService : IAccessService
    {
        private readonly WhitelistOptions _whitelistOptions;
        private readonly TypeLookupService _typeLookupService;

        public AccessService(IOptions<WhitelistOptions> whitelistOptions, TypeLookupService typeLookupService)
        { 
            _whitelistOptions = whitelistOptions.Value;
            _typeLookupService = typeLookupService;
        }

        public IAccessResult AccessFor(string appId, ClaimsPrincipal principal) =>
            new AccessResult(appId, principal, _typeLookupService, _whitelistOptions);

        public IAccessResult AccessFor(string appId, Guid accountTypeId) =>
            new AccessResult(appId, accountTypeId, _typeLookupService, _whitelistOptions);

        public IAccessResult AccessFor(string appId) => 
            new AccessResult(appId, _typeLookupService, _whitelistOptions);
    }
}
