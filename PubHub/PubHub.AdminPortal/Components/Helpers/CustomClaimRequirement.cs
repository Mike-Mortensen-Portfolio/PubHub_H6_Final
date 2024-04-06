using Microsoft.AspNetCore.Authorization;

namespace PubHub.AdminPortal.Components
{
    public class CustomClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }
        public string ClaimValue { get; }

        public CustomClaimRequirement(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
    }
}
