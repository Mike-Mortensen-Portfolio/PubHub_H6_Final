using PubHub.API.Domain.Entities;

namespace PubHub.API.UT.Utilities
{
    public class ApiDataGeneratorFixture : DataGeneratorFixture
    {
        public ApiDataGeneratorFixture() : base()
        {
            Generator.Customize<Publisher>(x => x
                .Without(p => p.Books));
        }
    }
}
