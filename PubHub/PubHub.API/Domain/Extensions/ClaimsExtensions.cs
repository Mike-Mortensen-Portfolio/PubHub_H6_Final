using System.Security.Claims;
using PubHub.API.Domain.Entities;
using PubHub.Common;

namespace PubHub.API.Domain.Extensions
{
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Get the <see cref="TokenClaimConstants.ID"/> claim value of a <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/> with subject.</param>
        /// <returns><see cref="TokenClaimConstants.ID"/> value; otherwise <see cref="Guid.Empty"/>.</returns>
        public static Guid GetSubjectId(this ClaimsPrincipal claimsPrincipal)
        {
            if (Guid.TryParse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == TokenClaimConstants.ID)?.Value, out var subjectId))
            {
                return subjectId;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Get the <see cref="TokenClaimConstants.ACCOUNT_TYPE"/> claim value of a <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/> with account type.</param>
        /// <returns><see cref="TokenClaimConstants.ACCOUNT_TYPE"/> value; otherwise <see cref="Guid.Empty"/>.</returns>
        public static Guid GetAccountTypeId(this ClaimsPrincipal claimsPrincipal)
        {
            if (Guid.TryParse(claimsPrincipal.Claims.FirstOrDefault(c => c.Type == TokenClaimConstants.ACCOUNT_TYPE)?.Value, out var accountTypeId))
            {
                return accountTypeId;
            }

            return Guid.Empty;
        }
    }
}
