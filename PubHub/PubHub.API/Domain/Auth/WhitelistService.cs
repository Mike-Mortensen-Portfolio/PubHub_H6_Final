using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;

namespace PubHub.API.Domain.Auth
{
    public class WhitelistService
    {
        private readonly WhitelistOptions _whitelistOptions;

        public WhitelistService(IOptions<WhitelistOptions> whitelistOptions)
        { 
            _whitelistOptions = whitelistOptions.Value;
        }

        public bool VerifyApplicationAccess(string appId, [CallerMemberName] string methodName = "")
        {
            var methodInfo = new StackTrace().GetFrame(1)?.GetMethod();
            var className = methodInfo?.ReflectedType?.ReflectedType?.Name;
            if (string.IsNullOrEmpty(className) || string.IsNullOrEmpty(methodName))
                return false;

            var whiteList = _whitelistOptions.Apps
                .Where(a => a.AppId == appId)
                .FirstOrDefault();
            
            // No application with the given ID is on the whitelist.
            if (whiteList == null)
                return false;
            
            // The application is not allowed in the given controller.
            if (!whiteList.ControllerEndpoints.TryGetValue(className, out IEnumerable<string>? allowedEndpoints))
                return false;
            
            // The application is not allowed on the given endpoint.
            if (!allowedEndpoints.Contains(methodName))
                return false;

            return true;
        }
    }
}
