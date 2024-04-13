using System.ComponentModel.DataAnnotations;
using PubHub.AdminPortal.Components.Constants;
using PubHub.Common.Models.Accounts;

namespace PubHub.AdminPortal.Components.Models
{
    public class AccountForm
    {
        [Required, Compare(nameof(Email))]
        [DataType(DataType.EmailAddress)]
        [MaxLength(ValidationAnnotationConstants.MAX_EMAIL_LENGTH)]
        public string Email { get; set; } = string.Empty;

        [Required, Compare(nameof(Password))]
        [DataType(DataType.Password)]
        [MaxLength(ValidationAnnotationConstants.MAX_PASSWORD_LENGTH)]
        public string Password { get; set; } = string.Empty;

        public AccountCreateModel CreateAccountModel()
        {
            return new AccountCreateModel { Email = Email, Password = Password };
        }
    }
}
