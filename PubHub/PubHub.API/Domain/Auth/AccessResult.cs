using System.Security.Claims;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Services;

namespace PubHub.API.Domain.Auth
{
    /// <summary>
    /// Allow or disallow access to resources for a given <see cref="ClaimsPrincipal"/>.
    /// Will disallow access unless <see cref="Allow"/> is called.
    /// Use <see cref="AccessResultExtensions"/> to modify and then validate the result with <see cref="AccessResultExtensions.TryVerify(AccessResult, out IResult?)"/>.
    /// </summary>
    public class AccessResult
    {
        private Guid? _accountTypeId;
        private Guid? _subjectId;

        public AccessResult(ClaimsPrincipal principal, TypeLookupService typeLookupService)
        {
            Principal = principal;
            TypeLookupService = typeLookupService;
        }

        public TypeLookupService TypeLookupService { get; init; }
        public ClaimsPrincipal Principal { get; init; }
        public bool Success { get; private set; } = false;
        public Guid SubjectId => _subjectId ??= Principal.GetSubjectId();
        public Guid AccountTypeId => _accountTypeId ??= Principal.GetAccountTypeId();

        /// <summary>
        /// Make this <see cref="AccessResult"/> allow access for <see cref="Principal"/>.
        /// </summary>
        public void Allow()
        {
            Success = true;
        }
    }
}
