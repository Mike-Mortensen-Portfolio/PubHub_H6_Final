using AutoFixture;
using NSubstitute;
using PubHub.Common.Models.Publishers;
using PubHub.Common.Services;

namespace PubHub.Common.UT.Services
{
    public class PublisherServiceTests : ServiceTests
    {
        //private readonly PublisherService _service;
        //private readonly IHttpClientService _client;

        public PublisherServiceTests(DataGeneratorFixture dataGeneratorFixture)
            : base(dataGeneratorFixture)
        {
            //_client = Substitute.For<IHttpClientService>();
            //_service = new(_client);
        }

        //[Fact]
        //public async Task GetPublisherByIdAsync()
        //{
        //    // Arrange.
        //    var expectedModel = Generator.Create<PublisherInfoModel>();
        //    _client.GetAsync(Arg.Any<string>()).Returns(Task.FromResult(new HttpResponseMessage()
        //    {
        //        Content = GetJsonContent(expectedModel)
        //    }));
        //
        //    // Act.
        //    var result = await _service.GetPublisherInfoAsync(expectedModel.Id);
        //    var actualModel = result.Instance;
        //
        //    // Assert.
        //    Assert.Equivalent(expectedModel, actualModel);
        //}
    }
}
