using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Identity;
using PubHub.API.Domain.UUID;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class AccountSeed : SeedBase<Account, string>
    {
        private readonly AccountTypeSeed _accountTypeSeed;

        public AccountSeed(AccountTypeSeed accountTypeSeed)
        {
            _accountTypeSeed = accountTypeSeed;
        }

        /// <summary>
        /// Find the first <see cref="Account"/> where the email matches the <paramref name="key"/>
        /// </summary>
        /// <param name="key">The email of the <see cref="Account"/></param>
        /// <returns><inheritdoc/></returns>
        public override Account this[string key]
        {
            get
            {
                return Seeds.First(a => a.Email.Equals(key, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public override void Configure(EntityTypeBuilder<Account> builder)
        {
            var hasher = new PasswordHasher<Account>();

            Seeds =
            [
                new Account
                {
                    Id = UuidValueGenerator.Next(),
                    AccountTypeId = _accountTypeSeed["User"].Id,
                    ConcurrencyStamp = "UserSeedConcurrencyStamp",
                    Email = USER_EMAIL,
                    EmailConfirmed = true,
                    LastSignIn = DateTime.UtcNow,
                    SecurityStamp = "UserSeedSecurityStamp",
                    PhoneNumber = "4587654321",
                    LockoutEnabled = true
                },
                new Account
                {
                    Id = UuidValueGenerator.Next(),
                    AccountTypeId = _accountTypeSeed["Publisher"].Id,
                    ConcurrencyStamp = "PublisherSeedConcurrencyStamp",
                    Email = PUBLISHER_EMAIL,
                    EmailConfirmed = true,
                    LastSignIn = DateTime.UtcNow,
                    SecurityStamp = "PublisherSeedSecurityStamp",
                    PhoneNumber = "4576543210",
                    LockoutEnabled = true
                },
                new Account
                {
                    Id = UuidValueGenerator.Next(),
                    AccountTypeId = _accountTypeSeed["Publisher"].Id,
                    ConcurrencyStamp = "Publisher2SeedConcurrencyStamp",
                    Email = PUBLISHER2_EMAIL,
                    EmailConfirmed = true,
                    LastSignIn = DateTime.UtcNow,
                    SecurityStamp = "Publisher2SeedSecurityStamp",
                    PhoneNumber = "4565432109",
                    LockoutEnabled = true
                },
                new Account
                {
                    Id = UuidValueGenerator.Next(),
                    AccountTypeId = _accountTypeSeed["Operator"].Id,
                    ConcurrencyStamp = "OperatorSeedConcurrencyStamp",
                    Email = OPERATOR_EMAIL,
                    EmailConfirmed = true,
                    LastSignIn = DateTime.UtcNow,
                    SecurityStamp = "OperatorSeedSecurityStamp",
                    PhoneNumber = "4554321098",
                    LockoutEnabled = true
                },
            ];

            this[USER_EMAIL].PasswordHash = hasher.HashPassword(this[USER_EMAIL], "P@ssw0rd");
            this[PUBLISHER_EMAIL].PasswordHash = hasher.HashPassword(this[PUBLISHER_EMAIL], "P@ssw0rd");
            this[PUBLISHER2_EMAIL].PasswordHash = hasher.HashPassword(this[PUBLISHER2_EMAIL], "P@ssw0rd");
            this[OPERATOR_EMAIL].PasswordHash = hasher.HashPassword(this[OPERATOR_EMAIL], "P@ssw0rd");

            builder.HasData(Seeds);
        }
    }
}
