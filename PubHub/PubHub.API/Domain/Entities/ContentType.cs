namespace PubHub.API.Domain.Entities
{
    public class ContentType : ITypeEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
