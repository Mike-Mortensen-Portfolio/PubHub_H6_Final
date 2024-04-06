using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Accounts;

namespace PubHub.AdminPortal.Components.Models
{
    public class AccountForm
    {
        [Required, Compare(nameof(Email)), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, Compare(nameof(Password)), DataType(DataType.Password)]
        public string Password { get; set; }

        public AccountCreateModel CreateAccountModel()
        {
            return new AccountCreateModel { Email = Email, Password = Password };
        }
    }
}
