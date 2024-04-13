using System.Security.Claims;
using AutoFixture;
using NSubstitute;
using NSubstitute.Extensions;
using PubHub.API.Domain.Auth;
using PubHub.API.UT.Utilities;

namespace PubHub.API.UT.Controllers
{
    public abstract class ControllerTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiDataGeneratorFixture>, IAsyncLifetime
    {
        protected ControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
        {
            DatabaseFixture = databaseFixture;
            Fixture = apiDataGeneratorFixture.Generator;

            AccessService = Substitute.For<IAccessService>();
            AccessService.AccessFor(Arg.Any<string>(), Arg.Any<ClaimsPrincipal>())
                .Returns(x =>
                {
                    var accessResult = Substitute.For<IAccessResult>();
                    accessResult.ReturnsForAll(accessResult);
                    accessResult.Concluded.Returns(true);
                    if (x[0] is string appId &&
                        appId == AppId)
                        accessResult.Success.Returns(true);
                    else
                        accessResult.Success.Returns(false);

                    return accessResult;
                });
        }

        protected DatabaseFixture DatabaseFixture { get; }
        protected PubHubContext Context => DatabaseFixture.Context;
        protected Fixture Fixture { get; }

        protected string AppId { get; } = Guid.NewGuid().ToString();
        protected IAccessService AccessService { get; }

        public Task InitializeAsync() => Task.CompletedTask;

        async Task IAsyncLifetime.DisposeAsync()
        {
            await DatabaseFixture.CleanUpAsync();
        }
    }
}
