namespace PubHub.Common.Models
{
    public class PublisherCreateModel
    {
        public required string Name { get; set; }
        public required AccountCreateModel Account { get; set; }
    }
}
