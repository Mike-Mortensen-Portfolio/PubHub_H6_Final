using AutoFixture;
using PubHub.Common.Models.Books;
using PubHub.TestUtils;

namespace PubHub.Common.UT.Services
{
    public class MyServiceTests : ServiceTests
    {
        public MyServiceTests(DataGeneratorFixture dataGeneratorFixture) : base(dataGeneratorFixture)
        { }

        [Fact]
        public void Test1()
        {
            var test = Generator.Create<BookInfoModel>();
        }
    }
}
