using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class AccountTypeSeed : SeedBase<AccountType, string>
    {
        /// <summary>
        /// Find the <see cref="AccountType"/> where the name matches <paramref name="key"/>
        /// </summary>
        /// <param name="key">The name to search for</param>
        /// <returns><inheritdoc/></returns>
        public override AccountType this[string key]
        {
            get
            {
                var normalizedType = key.ToUpperInvariant();
                return Seeds.First(ats => ats.Name.Equals(normalizedType, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public override void Configure(EntityTypeBuilder<AccountType> builder)
        {
            Seeds =
            [
                new AccountType { Id = 1, Name = USER_ACCOUNT_TYPE },
                new AccountType { Id = 2, Name = PUBLISHER_ACCOUNT_TYPE },
                new AccountType { Id = 3, Name = OPERATOR_ACCOUNT_TYPE },
                new AccountType { Id = 4, Name = SUSPENDED_ACCOUNT_TYPE}
            ];

            builder.HasData(Seeds);
        }
    }
}
