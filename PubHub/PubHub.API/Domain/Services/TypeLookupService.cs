using PubHub.API.Domain.Entities;
using PubHub.Common;

namespace PubHub.API.Domain.Services
{
    public class TypeLookupService(PubHubContext context)
    {
        private readonly PubHubContext _context = context;

        public bool IsUser(Guid accountTypeId) => _context.Set<AccountType>()
            .Any(at => at.Name == AccountTypeConstants.USER_ACCOUNT_TYPE && at.Id == accountTypeId);

        public bool IsPublisher(Guid accountTypeId) => _context.Set<AccountType>()
            .Any(at => at.Name == AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE && at.Id == accountTypeId);

        public bool IsOperator(Guid accountTypeId) => _context.Set<AccountType>()
            .Any(at => at.Name == AccountTypeConstants.OPERATOR_ACCOUNT_TYPE && at.Id == accountTypeId);

        public string? GetAccountTypeName(Guid accountTypeId) => _context.Set<AccountType>()
            .FirstOrDefault(at => at.Id == accountTypeId)?.Name;
    }
}
