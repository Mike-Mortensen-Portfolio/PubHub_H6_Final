using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PubHub.API.Controllers;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.API.UT.Extensions;
using PubHub.API.UT.Utilities;
using PubHub.Common.Models.Publishers;
using PubHub.TestUtils.Extensions;

namespace PubHub.API.UT.Controllers
{
    public class PublishersControllerTests : ControllerTests
    {
        private readonly PublishersController _controller;

        public PublishersControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
            : base(databaseFixture, apiDataGeneratorFixture)
        {
            _controller = new(Substitute.For<ILogger<PublishersController>>(), Context, Substitute.For<UserManager<Account>>());
        }

        //[Fact]
        //TODO (SIA): This test currently fails with unknown account type.
        public async Task AddPublisherAsync()
        {
            // Arrange.
            var publisher = Fixture.Create<PublisherCreateModel>();

            // Act.
            var result = await _controller.AddPublisherAsync(publisher);

            // Assert.
            var response = Assert.IsAssignableFrom<Created<PublisherInfoModel>>(result);
            Assert.NotNull(response.Value);

            Context.AssertCreated(publisher, response.Value.Id);
        }

        [Fact]
        public async Task GetPublisherByIdAsync()
        {
            // Arrange.
            var publishers = Fixture.CreateMany<Publisher>(10);
            await Context.Set<Publisher>().AddRangeAsync(publishers);
            await Context.SaveChangesAsync();

            var expectedPublisher = publishers.Random();
            var expectedModel = expectedPublisher.ToInfo();

            // Act.
            var result = await _controller.GetPublisherAsync(expectedPublisher.Id);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<PublisherInfoModel>>(result);
            Assert.NotNull(response.Value);
            Assert.Equivalent(expectedModel, response.Value);
        }

        [Fact]
        public async Task GetAllPublishersAsync()
        {
            // Arrange.
            int publisherCount = 15;
            var publishers = Fixture.CreateMany<Publisher>(publisherCount);
            await Context.Set<Publisher>().AddRangeAsync(publishers);
            await Context.SaveChangesAsync();

            var expectedModels = publishers.ToInfo().ToList();

            var query = new PublisherQuery()
            {
                Max = int.MaxValue,
                Page = 1
            };

            // Act.
            var result = await _controller.GetPublishersAsync(query);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<PublisherInfoModel[]>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(expectedModels.Count, response.Value.Length);
            Assert.Equivalent(expectedModels, response.Value);
        }
    }
}
