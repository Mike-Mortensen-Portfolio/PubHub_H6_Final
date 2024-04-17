using System.Security.Claims;
using PubHub.API.Domain.Extensions;
using PubHub.API.Domain.Services;

namespace PubHub.API.Domain.Auth
{
    public class AccessResult : IAccessResult
    {
        private readonly WhitelistOptions _whiteListOptions;
        private readonly string _appId;
        
        private bool? _success;
        private Guid? _accountTypeId;
        private Guid? _subjectId;
        private string? _subjectName;
        private AppWhitelist? _appWhitelist;

        public AccessResult(string appId, ClaimsPrincipal principal, TypeLookupService typeLookupService, WhitelistOptions whiteListOptions)
        {
            _whiteListOptions = whiteListOptions;
            _appId = appId;
            Principal = principal;
            TypeLookupService = typeLookupService;
        }

        public AccessResult(string appId, Guid accountTypeId, TypeLookupService typeLookupService, WhitelistOptions whiteListOptions)
        {
            _whiteListOptions = whiteListOptions;
            _appId = appId;
            _accountTypeId = accountTypeId;
            TypeLookupService = typeLookupService;
        }

        public AccessResult(string appId, TypeLookupService typeLookupService, WhitelistOptions whiteListOptions)
        {
            _whiteListOptions = whiteListOptions;
            _appId = appId;
            TypeLookupService = typeLookupService;
        }

        public TypeLookupService TypeLookupService { get; init; }
        public ClaimsPrincipal? Principal { get; init; }

        public bool Concluded => _success == false;
        public bool Success => _success ?? false;
        public bool HasSubject => !(Principal == null && AccountTypeId == Guid.Empty);

        public Guid AccountTypeId => _accountTypeId ??= Principal?.GetAccountTypeId() ?? Guid.Empty;
        public Guid SubjectId => _subjectId ??= Principal?.GetSubjectId() ?? Guid.Empty;
        public string? SubjectName => _subjectName ??= TypeLookupService.GetAccountTypeName(AccountTypeId);
        public AppWhitelist? AppWhitelist => _appWhitelist ??= _whiteListOptions.Apps.Where(a => a.AppId == _appId).FirstOrDefault();

        public void Allow()
        {
            _success ??= true;
        }
        
        public void Disallow()
        {
            _success = false;
        }
    }
}
