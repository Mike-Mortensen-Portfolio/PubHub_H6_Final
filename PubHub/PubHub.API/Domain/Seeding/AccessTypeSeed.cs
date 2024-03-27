using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class AccessTypeSeed : IEntityTypeConfiguration<AccessType>
    {
        public void Configure(EntityTypeBuilder<AccessType> builder)
        {
            builder.HasData(
                new AccessType { Id = 1, Name = "Owner" },
                new AccessType { Id = 2, Name = "Subscriber" },
                new AccessType { Id = 3, Name = "Borrower" },
                new AccessType { Id = 4, Name = "Expired" });
        }
    }
}
