using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Authentication;

namespace PubHub.AdminPortal.Components.Models
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Please enter a valid email.")]
        [RegularExpression(Constants.RegexConstants.EMAIL_REGEX, ErrorMessage = "The email isn't correctly formatted.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "The email isn't a valid email, please re-enter it.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
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
