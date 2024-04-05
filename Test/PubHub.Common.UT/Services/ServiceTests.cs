using AutoFixture;

namespace PubHub.Common.UT.Services
{
    public class ServiceTests : IClassFixture<DataGeneratorFixture>
    {
        protected Fixture Generator { get; }

        public ServiceTests(DataGeneratorFixture dataGeneratorFixture)
        {
            Generator = dataGeneratorFixture.Generator;
        }
    }
}
