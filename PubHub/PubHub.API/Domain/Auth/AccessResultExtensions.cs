using System.Diagnostics.CodeAnalysis;
using PubHub.API.Controllers.Problems;
using System.Runtime.CompilerServices;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Auth
{
    public static class AccessResultExtensions
    {
        /// <summary>
        /// Verify if the given <see cref="IAccessResult"/> will provides access.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to verify.</param>
        /// <param name="problem">Null when returning true; otherwise problem from <see cref="UnauthorizedSpecification"/>.</param>
        /// <returns><see langword="true"/> if access can be provided; otherwise <see langword="false"/>.</returns>
        public static bool TryVerify(this IAccessResult accessResult, [NotNullWhen(false)] out IResult? problem)
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
        /// Check whether an application is allowed to access a given controller and endpoint.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="controllerName">Name of the controller trying to be accessed.</param>
        /// <param name="methodName">Name of controller method trying to be accessed.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult CheckWhitelistEndpoint(this IAccessResult accessResult, string controllerName, [CallerMemberName] string methodName = "")
        {
            if (accessResult.Concluded)
                return accessResult;

            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(methodName))
            {
                accessResult.Disallow();
                 
                return accessResult;
            }

            // No application with the given ID is on the whitelist.
            if (accessResult.AppWhitelist == null)
            {
                accessResult.Disallow();

                return accessResult;
            }

            // The application is not allowed in the given controller.
            if (!accessResult.AppWhitelist.ControllerEndpoints.TryGetValue(controllerName, out IEnumerable<string>? allowedEndpoints))
            {
                accessResult.Disallow();

                return accessResult;
            }

            // The application is not allowed on the given endpoint.
            if (!allowedEndpoints.Contains(methodName))
            {
                accessResult.Disallow();

                return accessResult;
            }

            // Allow if not validating subject.
            if (!accessResult.HasSubject)
                accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// Check if <see cref="IAccessResult.SubjectName"/> is included in the <see cref="IAccessResult.AppWhitelist"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="subjectNameFallback">Name of subject to use if <see cref="IAccessResult.SubjectName"/> is null.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult CheckWhitelistSubject(this IAccessResult accessResult, string? subjectNameFallback = null)
        {
            if (accessResult.Concluded)
                return accessResult;

            var subjectName = accessResult.SubjectName ?? subjectNameFallback;
            if (!(subjectName != null && (accessResult.AppWhitelist?.Subjects.Contains(subjectName) ?? false)))
                accessResult.Disallow();

            // Allow if not validating subject.
            if (!accessResult.HasSubject)
                accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// For users: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowUser(this IAccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
                accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// For users: Allow only user with <paramref name="userId"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="userId">ID of user to allow.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowUser(this IAccessResult accessResult, Guid userId)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
            {
                if (userId == accessResult.SubjectId)
                    accessResult.Allow();
            }

            return accessResult;
        }

        /// <summary>
        /// For users: Allow only user with access to specific book.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="bookId">Id of the book.</param>
        /// <param name="context">The <see cref="PubHubContext"/> to query the database.</param>
        /// <returns></returns>
        public static IAccessResult AllowUserOnlyIfOwns(this IAccessResult accessResult, Guid bookId, PubHubContext context)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
            {
                var ownsBook = context.Set<UserBook>()
                    .Any(x => x.UserId == accessResult.SubjectId && x.BookId == bookId);

                if (ownsBook)
                    accessResult.Allow();
                else
                    accessResult.Disallow();
            }

            return accessResult;
        }

        /// <summary>
        /// For publishers: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowPublisher(this IAccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsPublisher(accessResult.AccountTypeId))
                accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// For publishers: Allow only publisher with <paramref name="publisherId"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="publisherId">ID of publisher to allow.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowPublisher(this IAccessResult accessResult, Guid publisherId)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsPublisher(accessResult.AccountTypeId))
                if (publisherId == accessResult.SubjectId)
                    accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// For users: Allow only publisher with access to specific book.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <param name="bookId">Id of the book.</param>
        /// <param name="context">The <see cref="PubHubContext"/> to query the database.</param>
        /// <returns></returns>
        public static IAccessResult AllowPublisherOnlyIfOwns(this IAccessResult accessResult, Guid bookId, PubHubContext context)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
            {
                var ownsBook = context.Set<Book>()
                    .Any(x => x.PublisherId == accessResult.SubjectId && x.Id == bookId);

                if (ownsBook)
                    accessResult.Allow();
            }

            return accessResult;
        }

        /// <summary>
        /// For operators: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowOperator(this IAccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsOperator(accessResult.AccountTypeId))
                accessResult.Allow();

            return accessResult;
        }

        /// <summary>
        /// Allow any subject, only superseded by the whitelist.
        /// </summary>
        /// <param name="accessResult"><see cref="IAccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static IAccessResult AllowAny(this IAccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.HasSubject)
                accessResult.Allow();

            return accessResult;
        }
    } 
}
