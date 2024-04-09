using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Publishers;
using System.ComponentModel.DataAnnotations;

namespace PubHub.AdminPortal.Components.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Please enter a valid email.")]
        [RegularExpression(@"^[^\\/:*;\)\(]+$", ErrorMessage = "The characters ':', ';', '*', '/' and '\' are not allowed")]
        [DataType(DataType.EmailAddress, ErrorMessage = "The email isn't a valid email, please re-enter it.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [DataType(DataType.Password, ErrorMessage = "The password isn't a valid password, please re-enter it.")]
        public string? Password { get; set; }

        public LoginInfo CreateLoginInfo()
        {
            return new LoginInfo()
            {
                Email = Email ?? string.Empty,
                Password = Password ?? string.Empty
            };
        }
    }
}
