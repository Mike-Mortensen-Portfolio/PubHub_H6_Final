using System.Security.Claims;
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
            if (Guid.TryParse(claimsPrincipal.FindFirstValue(TokenClaimConstants.ID), out var subjectId))
                return subjectId;

            return Guid.Empty;
        }

        /// <summary>
        /// Get the <see cref="TokenClaimConstants.ACCOUNT_TYPE"/> claim value of a <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <param name="claimsPrincipal"><see cref="ClaimsPrincipal"/> with account type.</param>
        /// <returns><see cref="TokenClaimConstants.ACCOUNT_TYPE"/> value; otherwise <see cref="Guid.Empty"/>.</returns>
        public static Guid GetAccountTypeId(this ClaimsPrincipal claimsPrincipal)
        {
            if (Guid.TryParse(claimsPrincipal.FindFirstValue(TokenClaimConstants.ACCOUNT_TYPE), out var accountTypeId))
                return accountTypeId;

            return Guid.Empty;
        }
    }
}
