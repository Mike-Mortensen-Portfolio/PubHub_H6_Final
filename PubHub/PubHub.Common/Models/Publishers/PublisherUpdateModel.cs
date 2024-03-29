using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Publishers
{
    public class PublisherUpdateModel
    {
        public required string Name { get; set; }
        public AccountUpdateModel? Account { get; set; }
    }
}
