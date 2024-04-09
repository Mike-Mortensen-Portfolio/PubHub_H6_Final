using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Publishers;

namespace PubHub.AdminPortal.Components.Models
{
    public class PublisherForm
    {
        [Required(ErrorMessage = "Please enter the name of the publisher.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a valid email.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords must be the same.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please enter the same password.")]
        [RegularExpression(@"^[^\\/:*;\.\)\(]+$", ErrorMessage = "The characters ':', '.' ';', '*', '/' and '\' are not allowed")]
        [Compare(nameof(Password), ErrorMessage = "Passwords must be the same.")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        public PublisherCreateModel CreatePublisherModel()
        {
            return new PublisherCreateModel()
            {
                Name = Name ?? string.Empty,
                Account = new AccountCreateModel() 
                { 
                    Email = Email ?? string.Empty, 
                    Password = Password ?? string.Empty 
                },
            };
        }
    }
    
}
