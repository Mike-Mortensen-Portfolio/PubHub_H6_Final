using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class AccountTypeSeed : IEntityTypeConfiguration<AccountType>
    {
        public void Configure(EntityTypeBuilder<AccountType> builder)
        {
            builder.HasData(
                new AccountType { Id = 1, Name = "User" },
                new AccountType { Id = 2, Name = "Publisher" },
                new AccountType { Id = 3, Name = "Operator" });
        }
    }
}
