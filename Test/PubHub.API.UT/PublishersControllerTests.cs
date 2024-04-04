using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PubHub.API.Controllers;
using PubHub.API.Domain.Entities;
using PubHub.API.UT.Utilities;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.UT
{
    public class PublishersControllerTests : ControllerTests
    {
        private readonly PublishersController _controller;

        public PublishersControllerTests(DatabaseFixture databaseFixture)
            : base(databaseFixture)
        {
            _controller = new(Substitute.For<ILogger<PublishersController>>(), _context);
        }

        [Fact]
        public async Task GetAllPublishersAsync()
        {
            // Arrange.
            // TODO (SIA): Simplify and extend test data creation.
            Fixture fixture = new Fixture();
            fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            fixture.Customize<Publisher>(x => x
                .Without(p => p.Books));

            var publisher = fixture.Create<Publisher>();

            await _context.Set<Publisher>().AddRangeAsync(publisher);
            var query = new PublisherQuery()
            {
                OrderBy = OrderPublisherBy.Name,
                Descending = true,
                Max = 1,
                Page = 1
            };

            // Act.
            var result = await _controller.GetPublishersAsync(query);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<PublisherInfoModel[]>>(result);
            var valueResult = Assert.IsAssignableFrom<IValueHttpResult<PublisherInfoModel[]>>(result);

            // TODO (SIA): Assert value.
        }
    }
}
