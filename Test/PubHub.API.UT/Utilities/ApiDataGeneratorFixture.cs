using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;

namespace PubHub.API.UT.Utilities
{
    public class ApiDataGeneratorFixture : DataGeneratorFixture
    {
        public ApiDataGeneratorFixture() : base()
        {
            Generator.Customize<Publisher>(x => x
                .Without(p => p.AccountId)
                .Without(p => p.Books));
            Generator.Customize<Account>(x => x
                .With(a => a.DeletedDate, (DateTime?)null));
        }
    }
}
