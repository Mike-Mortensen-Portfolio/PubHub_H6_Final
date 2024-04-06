using AutoFixture;
using PubHub.API.UT.Utilities;

namespace PubHub.API.UT.Controllers
{
    public abstract class ControllerTests : IClassFixture<DatabaseFixture>, IClassFixture<ApiDataGeneratorFixture>, IAsyncDisposable
    {
        protected ControllerTests(DatabaseFixture databaseFixture, ApiDataGeneratorFixture apiDataGeneratorFixture)
        {
            DatabaseFixture = databaseFixture;
            Context = DatabaseFixture.GetDBContext();
            Fixture = apiDataGeneratorFixture.Generator;
        }

        protected DatabaseFixture DatabaseFixture { get; }
        protected PubHubContext Context { get; }
        protected Fixture Fixture { get; }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await DatabaseFixture.CleanUpAsync();
            GC.SuppressFinalize(this);
        }
    }
}
