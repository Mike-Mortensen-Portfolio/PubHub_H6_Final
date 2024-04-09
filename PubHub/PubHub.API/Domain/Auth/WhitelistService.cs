using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using PubHub.API.Controllers.Problems;

namespace PubHub.API.Domain.Auth
{
    public class WhitelistService
    {
        private readonly WhitelistOptions _whitelistOptions;
        private readonly Func<IResult> _unauthorizedResult = () => Results.Problem(statusCode: UnauthorizedSpecification.STATUS_CODE, title: UnauthorizedSpecification.TITLE, detail: "Unauthorized access to resource.");

        public WhitelistService(IOptions<WhitelistOptions> whitelistOptions)
        { 
            _whitelistOptions = whitelistOptions.Value;
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
                problem = _unauthorizedResult.Invoke();

                return false;
            }

            // Get whitelisted application.
            var appWhitelist = _whitelistOptions.Apps
                .Where(a => a.AppId == appId)
                .FirstOrDefault();
            
            // No application with the given ID is on the whitelist.
            if (appWhitelist == null)
            {
                problem = _unauthorizedResult.Invoke();

                return false;
            }

            // The application is not allowed in the given controller.
            if (!appWhitelist.ControllerEndpoints.TryGetValue(controllerName, out IEnumerable<string>? allowedEndpoints))
            {
                problem = _unauthorizedResult.Invoke();

                return false;
            }

            // The application is not allowed on the given endpoint.
            if (!allowedEndpoints.Contains(methodName))
            {
                problem = _unauthorizedResult.Invoke();

                return false;
            }

            problem = null;

            return true;
        }
    }
}
