using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class PublisherSeed : SeedBase<Publisher, string>
    {
        private readonly AccountSeed _accountSeed;

        public PublisherSeed(AccountSeed accountSeed)
        {
            _accountSeed = accountSeed;
        }

        public override Publisher this[string key] => Seeds.First(p => p.Account!.Email.Equals(key, StringComparison.CurrentCultureIgnoreCase));

        public override void Configure(EntityTypeBuilder<Publisher> builder)
        {
            Seeds =
            [
                new Publisher
                {
                    Id = 1,
                    AccountId = _accountSeed[PUBLISHER_EMAIL].Id,
                    Name = "Gyldendal"
                },
                new Publisher
                {
                    Id = 2,
                    AccountId = _accountSeed[PUBLISHER2_EMAIL].Id,
                    Name = "Forlaget Als"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
