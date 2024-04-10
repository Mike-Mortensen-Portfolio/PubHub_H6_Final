﻿using System.Diagnostics.CodeAnalysis;
using PubHub.API.Controllers.Problems;
using System.Runtime.CompilerServices;

namespace PubHub.API.Domain.Auth
{
    public static class AccessResultExtensions
    {
        /// <summary>
        /// Verify if the given <see cref="AccessResult"/> will provides access.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to verify.</param>
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
        /// Check whether an application is allowed to access a given controller and endpoint.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <param name="controllerName">Name of the controller trying to be accessed.</param>
        /// <param name="methodName">Name of controller method trying to be accessed.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult CheckWhitelistEndpoint(this AccessResult accessResult, string controllerName, [CallerMemberName] string methodName = "")
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

            return accessResult;
        }

        /// <summary>
        /// Check if <see cref="AccessResult.SubjectName"/> is included in the <see cref="AccessResult.AppWhitelist"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult CheckWhitelistSubject(this AccessResult accessResult)
        {
            if (accessResult.Concluded)
                return accessResult;

            if (!(accessResult.SubjectName != null && (accessResult.AppWhitelist?.Subjects.Contains(accessResult.SubjectName) ?? false)))
            {
                accessResult.Disallow();
            }

            return accessResult;
        }

        /// <summary>
        /// For users: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowUser(this AccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsUser(accessResult.AccountTypeId))
            {
                accessResult.Allow();
            }

            return accessResult;
        }

        /// <summary>
        /// For users: Allow only user with <paramref name="userId"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <param name="userId">ID of user to allow.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowUser(this AccessResult accessResult, Guid userId)
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
        /// For publishers: Allow any.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowPublisher(this AccessResult accessResult)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsPublisher(accessResult.AccountTypeId))
            {
                accessResult.Allow();
            }

            return accessResult;
        }

        /// <summary>
        /// For publishers: Allow only publisher with <paramref name="publisherId"/>.
        /// </summary>
        /// <param name="accessResult"><see cref="AccessResult"/> to extend.</param>
        /// <param name="publisherId">ID of publisher to allow.</param>
        /// <returns><paramref name="accessResult"/></returns>
        public static AccessResult AllowPublisher(this AccessResult accessResult, Guid publisherId)
        {
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsPublisher(accessResult.AccountTypeId))
            {
                if (publisherId == accessResult.SubjectId)
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
            if (accessResult.Concluded || accessResult.Success)
                return accessResult;

            if (accessResult.TypeLookupService.IsOperator(accessResult.AccountTypeId))
            {
                accessResult.Allow();
            }

            return accessResult;
        }
    }
}
