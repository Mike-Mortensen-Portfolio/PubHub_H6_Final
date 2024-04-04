using PubHub.API.UT.Utilities;

namespace PubHub.API.UT
{
    public abstract class ControllerTests : IClassFixture<DatabaseFixture>
    {
        protected readonly DatabaseFixture _databaseFixture;
        protected readonly PubHubContext _context;

        protected ControllerTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _context = _databaseFixture.GetDBContext();
        }
    }
}
