namespace PubHub.API.Domain.Entities
{
    public class AccountType : ITypeEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }
}
