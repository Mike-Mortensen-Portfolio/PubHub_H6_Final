using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using PubHub.API.Controllers.Problems;
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
        /// Verify whether an application is allowed to access a given controller and endpoint.
        /// </summary>
        /// <param name="appId">Unique and secret application ID of an application accessing the PubHub API.</param>
        /// <param name="controllerName">Name of the controller trying to be accessed.</param>
        /// <param name="methodName">Name of controller method trying to be accessed.</param>
        /// <param name="problem">Null when returning true; otherwise problem from <see cref="UnauthorizedSpecification"/>.</param>
        /// <returns><see langword="true"/> if the application has access; otherwise <see langword="false"/>.</returns>
        public bool TryVerifyApplicationAccess(string appId, string controllerName, [NotNullWhen(false)] out IResult? problem, [CallerMemberName] string methodName = "")
        {
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(methodName))
            {
                problem = ProblemResults.UnauthorizedResult();

                return false;
            }

            // Get whitelisted application.
            var appWhitelist = _whitelistOptions.Apps
                .Where(a => a.AppId == appId)
                .FirstOrDefault();
            
            // No application with the given ID is on the whitelist.
            if (appWhitelist == null)
            {
                problem = ProblemResults.UnauthorizedResult();

                return false;
            }

            // The application is not allowed in the given controller.
            if (!appWhitelist.ControllerEndpoints.TryGetValue(controllerName, out IEnumerable<string>? allowedEndpoints))
            {
                problem = ProblemResults.UnauthorizedResult();

                return false;
            }

            // The application is not allowed on the given endpoint.
            if (!allowedEndpoints.Contains(methodName))
            {
                problem = ProblemResults.UnauthorizedResult();

                return false;
            }

            problem = null;

            return true;
        }

        /// <summary>
        /// Create an <see cref="AccessResult"/> ready to apply verification to with allow methods from <see cref="AccessResultExtensions"/>.
        /// End verification with <see cref="AccessResultExtensions.TryVerify(AccessResult, out IResult?)"/>.
        /// </summary>
        /// <param name="principal">Subject to verify access for.</param>
        /// <returns><see cref="AccessResult"/> for <paramref name="principal"/>.</returns>
        public AccessResult Access(ClaimsPrincipal principal, string appId) =>
            new(principal, appId, _typeLookupService);
    }
}
