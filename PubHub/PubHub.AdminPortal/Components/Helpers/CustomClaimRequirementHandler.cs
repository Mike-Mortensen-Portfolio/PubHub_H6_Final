using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace PubHub.AdminPortal.Components.Helpers
{
    public class CustomClaimRequirementHandler : AuthorizationHandler<CustomClaimRequirement>
    {
        /// <summary>
        /// Implemented to set our own policies in the <see cref="IAuthorizationHandler"/> with the use of specific claims.
        /// </summary>
        /// <param name="context">The <see cref="AuthorizationHandlerContext"/>.</param>
        /// <param name="requirement">The claims that will be checked against the authenticated user.</param>
        /// <returns>Set of claims that will be sent with the <see cref="AuthorizationHandlerContext"/>.</returns>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomClaimRequirement requirement)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(context);

                if (!(context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                var claim = context.User.Claims.FirstOrDefault(c => c.Type == requirement.ClaimType && c.Value == requirement.ClaimValue.ToLower());
                if (claim != null)
                {
                    context.Succeed(requirement);
                }
                else { context.Fail(); }

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Couldn't handle the Requirement request,", ex.Message);
                return Task.CompletedTask;
            }

        }
    }
}
