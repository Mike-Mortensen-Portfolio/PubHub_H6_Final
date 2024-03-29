namespace PubHub.Common.Models
{
    public class PublisherInfoModel
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
