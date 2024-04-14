using System.ComponentModel.DataAnnotations;
using PubHub.Common;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Publishers;

namespace PubHub.AdminPortal.Components.Models
{
    public class PublisherForm
    {
        [Required(ErrorMessage = "Please enter the name of the publisher.")]
        [MaxLength(ValidationAnnotationConstants.MAX_NAME_LENGTH)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please enter a valid email.")]
        [RegularExpression(Constants.RegexConstants.EMAIL_REGEX)]
        [DataType(DataType.EmailAddress, ErrorMessage = "The email isn't a valid email, please re-enter it.")]
        [MaxLength(ValidationAnnotationConstants.MAX_EMAIL_LENGTH)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords must be the same.")]
        [DataType(DataType.Password)]
        [MaxLength(ValidationAnnotationConstants.MAX_PASSWORD_LENGTH)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Please enter the same password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords must be the same.")]
        [DataType(DataType.Password)]
        [MaxLength(ValidationAnnotationConstants.MAX_PASSWORD_LENGTH)]
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
