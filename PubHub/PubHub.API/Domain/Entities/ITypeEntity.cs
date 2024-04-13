namespace PubHub.API.Domain.Entities
{
    public interface ITypeEntity
    {
        public Guid Id { get; }
        public string Name { get; }
    }
}
