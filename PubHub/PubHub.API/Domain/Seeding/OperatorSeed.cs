using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class OperatorSeed : SeedBase<Operator, string>
    {
        private readonly AccountSeed _accountSeed;

        public OperatorSeed(AccountSeed accountSeed)
        {
            _accountSeed = accountSeed;
        }

        /// <summary>
        /// Find the <see cref="Operator"/> where the email of the <see cref="Identity.Account"/> matches <paramref name="key"/>
        /// </summary>
        /// <param name="key">The email of <see cref="Identity.Account"/> associated with the <see cref="Operator"/> to find</param>
        /// <returns><inheritdoc/></returns>
        public override Operator this[string key] => Seeds.First(o => _accountSeed[key].Id == o.AccountId);

        public override void Configure(EntityTypeBuilder<Operator> builder)
        {
            Seeds =
            [
                new Operator
                {
                    Id = Guid.Empty,
                    AccountId = _accountSeed[OPERATOR_EMAIL].Id,
                    Name = "Selena",
                    Surname = "Gomez"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
