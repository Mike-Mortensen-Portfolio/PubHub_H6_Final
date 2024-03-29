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

        /// <summary>
        /// Find the <see cref="Publisher"/> where the email of the <see cref="Identity.Account"/> matches <paramref name="key"/>
        /// </summary>
        /// <param name="key">The email of <see cref="Identity.Account"/> associated with the <see cref="Publisher"/> to find</param>
        /// <returns><inheritdoc/></returns>
        public override Publisher this[string key] => Seeds.First(p => _accountSeed[key].Id == p.AccountId);

        public override void Configure(EntityTypeBuilder<Publisher> builder)
        {
            Seeds =
            [
                new Publisher
                {
                    Id = Guid.Empty,
                    AccountId = _accountSeed[PUBLISHER_EMAIL].Id,
                    Name = "Gyldendal"
                },
                new Publisher
                {
                    Id = Guid.Empty,
                    AccountId = _accountSeed[PUBLISHER2_EMAIL].Id,
                    Name = "Forlaget Als"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
