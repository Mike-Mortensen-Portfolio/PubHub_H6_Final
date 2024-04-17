using System.Data;
using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PubHub.API.Controllers;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.API.UT.Extensions;
using PubHub.API.UT.Utilities;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;
using PubHub.TestUtils.Extensions;

namespace PubHub.API.UT.Controllers
{
    public class PublishersControllerTests : ControllerTests
    {
        private readonly PublishersController _controller;
        private readonly UserManager<Account> _userManager;

        public PublishersControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
            : base(databaseFixture, apiDataGeneratorFixture)
        {
            _userManager = Substitute.For<UserManager<Account>>(
                Substitute.For<IUserStore<Account>>(),
                Substitute.For<IOptions<IdentityOptions>>(),
                Substitute.For<IPasswordHasher<Account>>(),
                Substitute.For<IEnumerable<IUserValidator<Account>>>(),
                Substitute.For<IEnumerable<IPasswordValidator<Account>>>(),
                Substitute.For<ILookupNormalizer>(),
                Substitute.For<IdentityErrorDescriber>(),
                Substitute.For<IServiceProvider>(),
                Substitute.For<ILogger<UserManager<Account>>>());

            _controller = new(Substitute.For<ILogger<PublishersController>>(), Context, _userManager, AccessService);
        }

        [Fact]
        public async Task AddPublisherAsync()
        {
            // Arrange.
            var publisher = Fixture.Create<PublisherCreateModel>();
            var expectedAccount = new Account
            {
                Email = publisher.Account.Email,
                AccountTypeId = DatabaseFixture.GetTypeId(AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE)
            };
            _userManager.CreateAsync(Arg.Any<Account>(), Arg.Any<string>())
                .Returns(Task.FromResult(IdentityResult.Success));

            // Act.
            var result = await _controller.AddPublisherAsync(publisher, AppId);

            // Assert.
            var response = Assert.IsAssignableFrom<Created<PublisherInfoModel>>(result);
            Assert.NotNull(response.Value);

            await _userManager.Received()
                .CreateAsync(
                    Arg.Is<Account>(actualAccount => actualAccount.Email == expectedAccount.Email && actualAccount.AccountTypeId == expectedAccount.AccountTypeId),
                    Arg.Is(publisher.Account.Password));
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
            var result = await _controller.GetPublisherAsync(expectedPublisher.Id, AppId);

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
            var result = await _controller.GetPublishersAsync(query, AppId);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<PublisherInfoModel[]>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(expectedModels.Count, response.Value.Length);
            Assert.Equivalent(expectedModels, response.Value);
        }

        [Fact]
        public async Task GetAllBooksOfPublisher()
        {
            // Arrange.
            var expectedBookCount = 10;
            var expectedPublisher = Fixture.Create<Publisher>();
            var otherPublishers = Fixture.CreateMany<Publisher>(10);
            var expectedBooks = Fixture.CreateMany<Book>(expectedBookCount).ToList();
            expectedBooks.ForEach(b => b.Publisher = expectedPublisher);
            var otherBooks = Fixture.CreateMany<Book>(expectedBookCount).ToList();
            otherBooks.ForEach(b => b.Publisher = otherPublishers.Random());

            await Context.Set<Publisher>().AddAsync(expectedPublisher);
            await Context.Set<Publisher>().AddRangeAsync(otherPublishers);
            await Context.Set<Book>().AddRangeAsync(expectedBooks);
            await Context.Set<Book>().AddRangeAsync(otherBooks);
            await Context.SaveChangesAsync();

            var expectedBookModels = expectedBooks.Select(x => x.ToInfo()).ToList();

            // Act.
            var result = await _controller.GetBooksAsync(expectedPublisher.Id, AppId);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<List<BookInfoModel>>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(expectedBookCount, response.Value.Count);
            Assert.Equivalent(expectedBookModels, response.Value);
        }

        [Fact]
        public async Task DeletePublisher()
        {
            // Arrange.
            var publishers = Fixture.CreateMany<Publisher>();
            var expectedPublisher = publishers.Random();
            await Context.Set<Publisher>().AddRangeAsync(publishers);
            await Context.SaveChangesAsync();

            // Act.
            var result = await _controller.DeletePublisherAsync(expectedPublisher.Id, AppId);

            // Assert.
            Assert.IsAssignableFrom<Ok>(result);
            Context.AssertDeleted(expectedPublisher.Account!);
        }

        [Fact]
        public async Task GetBooksOfDeletedPublisher()
        {
            // Arrange.
            var expectedBookCount = 10;
            var expectedPublisher = Fixture.Create<Publisher>();
            expectedPublisher.Account!.DeletedDate = DateTime.UtcNow;
            var otherPublishers = Fixture.CreateMany<Publisher>(10);
            var expectedBooks = Fixture.CreateMany<Book>(expectedBookCount).ToList();
            expectedBooks.ForEach(b => b.Publisher = expectedPublisher);
            var otherBooks = Fixture.CreateMany<Book>(expectedBookCount).ToList();
            otherBooks.ForEach(b => b.Publisher = otherPublishers.Random());

            await Context.Set<Publisher>().AddAsync(expectedPublisher);
            await Context.Set<Publisher>().AddRangeAsync(otherPublishers);
            await Context.Set<Book>().AddRangeAsync(expectedBooks);
            await Context.Set<Book>().AddRangeAsync(otherBooks);
            await Context.SaveChangesAsync();

            var expectedBookModels = expectedBooks.Select(x => x.ToInfo()).ToList();

            // Act.
            var result = await _controller.GetBooksAsync(expectedPublisher.Id, AppId);

            // Assert.
            var response = Assert.IsAssignableFrom<Ok<List<BookInfoModel>>>(result);
            Assert.NotNull(response.Value);
            Assert.Equal(expectedBookCount, response.Value.Count);
            Assert.Equivalent(expectedBookModels, response.Value);
            Context.AssertDeleted(expectedPublisher.Account!);
        }
    }
}
