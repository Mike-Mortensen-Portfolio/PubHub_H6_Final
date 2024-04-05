using PubHub.Common.Models.Accounts;

namespace PubHub.AdminPortal.Components.Models
{
    public class AccountForm
    {
        public string Email { get; set; }

        public AccountCreateModel CreateAccountModel()
        {
            return new AccountCreateModel { Email = Email };
        }
    }
}
