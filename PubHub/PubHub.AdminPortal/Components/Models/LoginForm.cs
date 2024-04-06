using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Publishers;
using System.ComponentModel.DataAnnotations;

namespace PubHub.AdminPortal.Components.Models
{
    public class LoginForm
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;

        public LoginInfo CreateLoginInfo()
        {
            return new LoginInfo()
            {
                Email = Email,
                Password = Password
            };
        }
    }
}
