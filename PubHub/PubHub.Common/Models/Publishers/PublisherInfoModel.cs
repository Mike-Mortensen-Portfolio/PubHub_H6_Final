namespace PubHub.Common.Models.Publishers
{
    public class PublisherInfoModel
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
    }
}
