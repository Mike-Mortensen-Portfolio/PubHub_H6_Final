using System.Security.Claims;
using Microsoft.Extensions.Options;
using PubHub.API.Domain.Services;

namespace PubHub.API.Domain.Auth
{
    public class AccessService
    {
        private readonly WhitelistOptions _whitelistOptions;
        private readonly TypeLookupService _typeLookupService;

        public AccessService(IOptions<WhitelistOptions> whitelistOptions, TypeLookupService typeLookupService)
        { 
            _whitelistOptions = whitelistOptions.Value;
            _typeLookupService = typeLookupService;
        }

        /// <summary>
        /// Create an <see cref="AccessResult"/> ready to apply verification to with allow methods from <see cref="AccessResultExtensions"/>.
        /// End verification with <see cref="AccessResultExtensions.TryVerify(AccessResult, out IResult?)"/>.
        /// </summary>
        /// <param name="principal">Subject to verify access for.</param>
        /// <param name="appId">Unique and secret application ID of an application accessing the PubHub API.</param>
        /// <returns><see cref="AccessResult"/> for <paramref name="principal"/>.</returns>
        public AccessResult AccessFor(ClaimsPrincipal principal, string appId) =>
            new(principal, appId, _typeLookupService, _whitelistOptions);

        /// <inheritdoc cref="AccessFor(ClaimsPrincipal, string)"/>
        /// <param name="accountTypeId">ID of account type to verify access for.</param>
        public AccessResult AccessFor(Guid accountTypeId, string appId) =>
            new(accountTypeId, appId, _typeLookupService, _whitelistOptions);
    }
}
