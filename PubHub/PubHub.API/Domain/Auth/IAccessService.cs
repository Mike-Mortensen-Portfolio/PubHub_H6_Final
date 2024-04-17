using System.Security.Claims;

namespace PubHub.API.Domain.Auth
{
    public interface IAccessService
    {
        /// <summary>
        /// Create an <see cref="AccessResult"/> ready to apply verification to with allow methods from <see cref="AccessResultExtensions"/>.
        /// End verification with <see cref="AccessResultExtensions.TryVerify"/>.
        /// </summary>
        /// <param name="appId">Unique and secret application ID of an application accessing the PubHub API.</param>
        /// <param name="principal">Subject to verify access for.</param>
        /// <remarks>
        /// When checking access for a user or account type one MUST use at least one 'Allow' extension before calling <see cref="AccessResultExtensions.TryVerify"/>.
        /// <br />Use <see cref="AccessResultExtensions.AllowAny"/> if no additional restrictions should be made for the subject.</remarks>
        /// <returns><see cref="AccessResult"/> for <paramref name="principal"/>.</returns>
        public IAccessResult AccessFor(string appId, ClaimsPrincipal principal);

        /// <inheritdoc cref="AccessFor(string, ClaimsPrincipal)"/>
        /// <param name="accountTypeId">ID of account type to verify access for.</param>
        public IAccessResult AccessFor(string appId, Guid accountTypeId);

        /// <inheritdoc cref="AccessFor(string, ClaimsPrincipal)"/>
        public IAccessResult AccessFor(string appId);
    }
}
