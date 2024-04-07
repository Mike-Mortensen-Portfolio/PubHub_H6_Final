using AutoFixture;
using PubHub.API.UT.Utilities;

namespace PubHub.API.UT.Controllers
{
    public abstract class ControllerTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiDataGeneratorFixture>, IAsyncLifetime
    {
        protected ControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
        {
            DatabaseFixture = databaseFixture;
            Fixture = apiDataGeneratorFixture.Generator;
        }

        protected DatabaseFixture DatabaseFixture { get; }
        protected PubHubContext Context => DatabaseFixture.Context;
        protected Fixture Fixture { get; }

        public Task InitializeAsync() => Task.CompletedTask;

        async Task IAsyncLifetime.DisposeAsync()
        {
            await DatabaseFixture.CleanUpAsync();
        }
    }
}
