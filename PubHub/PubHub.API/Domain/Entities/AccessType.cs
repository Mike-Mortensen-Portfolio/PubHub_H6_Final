namespace PubHub.API.Domain.Entities
{
    public class AccessType : ITypeEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
