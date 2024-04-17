using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    internal class UserSeed : SeedBase<User, string>
    {
        private readonly AccountSeed _accountSeed;
        public UserSeed(AccountSeed accountSeed)
        {
            _accountSeed = accountSeed;
        }

        /// <summary>
        /// Find the <see cref="User"/> where the email of the <see cref="Identity.Account"/> matches <paramref name="key"/>
        /// </summary>
        /// <param name="key">The email of <see cref="Identity.Account"/> associated with the <see cref="User"/> to find</param>
        /// <returns><inheritdoc/></returns>
        public override User this[string key] => Seeds.First(u => _accountSeed[key].Id == u.AccountId);

        public override void Configure(EntityTypeBuilder<User> builder)
        {
            Seeds =
            [
                new User
                {
                    Id = UuidValueGenerator.Next(),
                    AccountId = _accountSeed[USER_EMAIL].Id,
                    Birthday = new DateOnly (1993, 4, 12),
                    Name = "Thomas",
                    Surname = "Berlin"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
