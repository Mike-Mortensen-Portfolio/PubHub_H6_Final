using PubHub.API.Domain.Services;
using System.Security.Claims;

namespace PubHub.API.Domain.Auth
{
    /// <summary>
    /// Allow or disallow access to resources for a given <see cref="ClaimsPrincipal"/> and application.
    /// Will disallow access unless <see cref="Allow"/> is called.
    /// Use <see cref="AccessResultExtensions"/> to modify and then validate the result with <see cref="AccessResultExtensions.TryVerify(AccessResult, out IResult?)"/>.
    /// </summary>
    public interface IAccessResult
    {
        public TypeLookupService TypeLookupService { get; init; }
        public ClaimsPrincipal? Principal { get; init; }

        public bool Concluded { get; }
        public bool Success { get; }
        public bool HasSubject { get; }

        public Guid AccountTypeId { get; }
        public Guid SubjectId { get; }
        public string? SubjectName { get; }
        /// <summary>
        /// Whitelist for application with <see cref="_appId"/>. Is <see langword="null"/> if the application isn't whitelisted.
        /// </summary>
        public AppWhitelist? AppWhitelist { get; }

        /// <summary>
        /// Make this <see cref="AccessResult"/> allow access for <see cref="Principal"/>.
        /// </summary>
        /// <remarks>Overruled by any call to <see cref="Disallow"/>.</remarks>
        public void Allow();

        /// <summary>
        /// Make this <see cref="AccessResult"/> disallow access for <see cref="Principal"/>.
        /// </summary>
        /// <remarks>Overrules all calls to <see cref="Allow"/>.</remarks>
        public void Disallow();
    }
}
