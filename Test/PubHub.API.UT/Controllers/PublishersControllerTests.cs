﻿using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PubHub.API.Controllers;
using PubHub.API.Domain.Entities;
using PubHub.API.UT.Utilities;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.UT.Controllers
{
    public class PublishersControllerTests : ControllerTests
    {
        private readonly PublishersController _controller;

        public PublishersControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
            : base(databaseFixture, apiDataGeneratorFixture)
        {
            _controller = new(Substitute.For<ILogger<PublishersController>>(), Context);
        }

        [Fact]
        public async Task GetAllPublishersAsync()
        {
            // Arrange.
            int count = 15;
            var publishers = Fixture.CreateMany<Publisher>(count);
            await Context.Set<Publisher>().AddRangeAsync(publishers);
            await Context.SaveChangesAsync();

            List<PublisherInfoModel> expectedModels = [];
            foreach (var publisher in publishers)
            {
                expectedModels.Add(new PublisherInfoModel() { Id = publisher.Id, Name = publisher.Name, Email = publisher.Account!.Email });
            }

            var query = new PublisherQuery()
            {
                OrderBy = OrderPublisherBy.Name,
                Descending = false,
                Max = count,
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
