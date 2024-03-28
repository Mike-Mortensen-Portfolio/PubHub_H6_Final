namespace PubHub.Common.Models
{
    public class PublisherUpdateModel
    {
        public required string Name { get; set; }
        public AccountUpdateModel? Account { get; set; }
    }
}
