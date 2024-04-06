using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace PubHub.API.UT.Utilities
{
    public class DatabaseFixture : IAsyncDisposable
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
        private readonly Task _containerStartTask;

        private DbContextOptions<PubHubContext> _options = new();
        private SqlConnection? _connection;

        public DatabaseFixture()
        {
            _containerStartTask = _msSqlContainer.StartAsync();
            CreateDatabaseAsync().GetAwaiter().GetResult();
        }

        public async Task CreateDatabaseAsync()
        {
            // Wait until container is started.
            await _containerStartTask;

            // Connect to database container.
            _connection ??= new(_msSqlContainer.GetConnectionString());
            _connection.Open();

            // Create options.
            _options = new DbContextOptionsBuilder<PubHubContext>()
                .UseSqlServer(_connection)
                .Options;

            // Build database schema.
            using var context = GetDBContext();
            context.Database.EnsureCreated();
        }

        /// <summary>
        /// Get database context to the current test database.
        /// </summary>
        /// <returns>A new <see cref="PubHubContext"/>.</returns>
        public PubHubContext GetDBContext()
        {
            return new PubHubContext(_options) { ApplySeed = false };
        }

        /// <summary>
        /// Clean up the database after a test.
        /// </summary>
        public async Task CleanUpAsync()
        {
            using var context = GetDBContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _msSqlContainer.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
