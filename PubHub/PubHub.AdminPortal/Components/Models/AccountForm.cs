using PubHub.Common.Models.Accounts;

namespace PubHub.AdminPortal.Components.Models
{
    public class AccountForm
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public AccountCreateModel CreateAccountModel()
        {
            return new AccountCreateModel { Email = Email, Password = Password };
        }
    }
}
