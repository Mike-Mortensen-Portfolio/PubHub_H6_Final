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
        /// <returns>Null if the application can be granted access; otherwise problem from <see cref="UnauthorizedSpecification"/>.</returns>
        public IResult? VerifyApplicationAccess(string appId, string controllerName, [CallerMemberName] string methodName = "")
        {
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(methodName))
                return _unauthorizedResult.Invoke();

            // Get whitelisted application.
            var appWhitelist = _whitelistOptions.Apps
                .Where(a => a.AppId == appId)
                .FirstOrDefault();
            
            // No application with the given ID is on the whitelist.
            if (appWhitelist == null)
                return _unauthorizedResult.Invoke();

            // The application is not allowed in the given controller.
            if (!appWhitelist.ControllerEndpoints.TryGetValue(controllerName, out IEnumerable<string>? allowedEndpoints))
                return _unauthorizedResult.Invoke();

            // The application is not allowed on the given endpoint.
            if (!allowedEndpoints.Contains(methodName))
                return _unauthorizedResult.Invoke();

            return null;
        }
    }
}
