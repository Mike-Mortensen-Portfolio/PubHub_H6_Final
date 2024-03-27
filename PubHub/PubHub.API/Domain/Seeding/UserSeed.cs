using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using System.Reflection.Metadata.Ecma335;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class UserSeed : SeedBase<User, string>
    {
        private readonly AccountSeed _accountSeed;
        public UserSeed(AccountSeed accountSeed)
        {
            _accountSeed = accountSeed;
        }

        public override User this[string key] => Seeds.First(u => u.Account!.Email.Equals(key, StringComparison.CurrentCultureIgnoreCase));

        public override void Configure(EntityTypeBuilder<User> builder)
        {
            Seeds =
            [
                new User
                {
                    Id = 1,
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
