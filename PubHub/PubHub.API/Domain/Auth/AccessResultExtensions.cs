using System.Diagnostics.CodeAnalysis;
using PubHub.API.Controllers.Problems;

namespace PubHub.API.Domain.Auth
{
    public static class AccessResultExtensions
    {
        /// <summary>
        /// Verify if the given <see cref="AccessResult"/> will provides access.
        /// </summary>
        /// <param name="accessResult"></param>
        /// <param name="problem">Null when returning true; otherwise problem from <see cref="UnauthorizedSpecification"/>.</param>
        /// <returns><see langword="true"/> if access can be provided; otherwise <see langword="false"/>.</returns>
        public static bool TryVerify(this AccessResult accessResult, [NotNullWhen(false)] out IResult? problem)
        {
            if (!accessResult.Success)
            {
                problem = ProblemResults.UnauthorizedResult();

                return false;
            }

            problem = null;

            return true;
        }

        /// <summary>
        /// For users: Only allow the user itself.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <param name="userId">ID of user to allow.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowUser(this AccessResult accessResult, Guid userId)
        {
            if (accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
            {
                if (userId != accessResult.SubjectId)
                    accessResult.Allow();
            }

            return accessResult;
        }

        /// <summary>
        /// For operators: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowOperator(this AccessResult accessResult)
        {
            if (accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsOperator(accessResult.AccountTypeId))
            {
                accessResult.Allow();
            }

            return accessResult;
        }
    }
}
