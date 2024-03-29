using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Publishers
{
    public class PublisherCreateModel
    {
        public required string Name { get; set; }
        public required AccountCreateModel Account { get; set; }
    }
}
