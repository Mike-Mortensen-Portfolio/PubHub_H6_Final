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
        private readonly WhitelistOptions _whiteListOptions;
        private readonly string _appId;
        
        private bool? _success;
        private Guid? _accountTypeId;
        private Guid? _subjectId;
        private string? _subjectName;
        private AppWhitelist? _appWhitelist;

        public AccessResult(ClaimsPrincipal principal, string appId, TypeLookupService typeLookupService, WhitelistOptions whiteListOptions)
        {
            _whiteListOptions = whiteListOptions;
            _appId = appId;
            Principal = principal;
            TypeLookupService = typeLookupService;
        }

        public TypeLookupService TypeLookupService { get; init; }
        public ClaimsPrincipal Principal { get; init; }

        public bool Concluded => _success == false;
        public bool Success => _success ?? false;

        public Guid AccountTypeId => _accountTypeId ??= Principal.GetAccountTypeId();
        public Guid SubjectId => _subjectId ??= Principal.GetSubjectId();
        public string? SubjectName => _subjectName ??= TypeLookupService.GetAccountTypeName(AccountTypeId);
        /// <summary>
        /// Whitelist for application with <see cref="_appId"/>. Is <see langword="null"/> if the application isn't whitelisted.
        /// </summary>
        public AppWhitelist? AppWhitelist => _appWhitelist ??= _whiteListOptions.Apps.Where(a => a.AppId == _appId).FirstOrDefault();

        /// <summary>
        /// Make this <see cref="AccessResult"/> allow access for <see cref="Principal"/>.
        /// </summary>
        /// <remarks>Overruled by any call to <see cref="Disallow"/>.</remarks>
        public void Allow()
        {
            _success ??= true;
        }

        /// <summary>
        /// Make this <see cref="AccessResult"/> disallow access for <see cref="Principal"/>.
        /// </summary>
        /// <remarks>Overrules all calls to <see cref="Allow"/>.</remarks>
        public void Disallow()
        {
            _success = false;
        }
    }
}
