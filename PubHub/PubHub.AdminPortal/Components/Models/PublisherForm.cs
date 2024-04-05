using System.ComponentModel.DataAnnotations;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Publishers;

namespace PubHub.AdminPortal.Components.Models
{
    public class PublisherForm
    {
        [Required]
        public string Name { get; set; }
        public AccountCreateModel Account { get; set; }

        public PublisherCreateModel CreatePublisherModel()
        {
            return new PublisherCreateModel()
            {
                Name = Name,
                Account = Account
            };
        }
    }
    
}
