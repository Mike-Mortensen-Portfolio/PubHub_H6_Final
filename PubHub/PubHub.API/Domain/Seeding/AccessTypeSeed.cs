using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    internal class AccessTypeSeed : SeedBase<AccessType, string>
    {
        public override AccessType this[string key]
        {
            get
            {
                var normalizedKey = key.ToUpperInvariant();
                return Seeds.First(at => at.Name.Equals(normalizedKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public override void Configure(EntityTypeBuilder<AccessType> builder)
        {
            Seeds =
            [
                new AccessType { Id = 1, Name = OWNER_ACCESS_TYPE },
                new AccessType { Id = 2, Name = SUBSCIBER_ACCESS_TYPE },
                new AccessType { Id = 3, Name = BORROWER_ACCESS_TYPE },
                new AccessType { Id = 4, Name = EXPIRED_ACCESS_TYPE }
            ];

            builder.HasData(Seeds);
        }
    }
}
