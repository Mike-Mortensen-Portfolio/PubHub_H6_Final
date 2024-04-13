using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.Common;
using Respawn;
using Testcontainers.MsSql;

namespace PubHub.API.UT.Utilities
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
        private readonly Dictionary<string, Guid> _types = [];

        private DbContextOptions<PubHubContext> _options = new();
        private SqlConnection? _connection;
        private PubHubContext? _context;
        private Respawner? _respawner;

        /// <summary>
        /// Whether the <see cref="DatabaseFixture"/> has been configured correctly.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [MemberNotNullWhen(true, nameof(_connection))]
        [MemberNotNullWhen(true, nameof(_respawner))]
        private bool IsConfigured
        {
            get
            {
                if (_connection != null && _respawner != null)
                    return true;

                throw new InvalidOperationException($"{nameof(DatabaseFixture)} wasn't configured correctly.");
            }
        }

        /// <summary>
        /// Database context to the current test database.
        /// </summary>
        public PubHubContext Context => _context ??= new PubHubContext(_options) { ApplySeed = false };

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            await CreateDatabaseAsync();
            await ConfigureRespawner();
            await SeedDatabaseAsync();
        }

        /// <summary>
        /// Clean up the database after a test.
        /// </summary>
        public async Task CleanUpAsync()
        {
            if (IsConfigured)
            {
                await _respawner.ResetAsync(_connection);
                await SeedDatabaseAsync();
            }
        }

        /// <summary>
        /// Get the ID of any type by its name.
        /// </summary>
        /// <param name="name">Name of type. Use <see cref="AccessTypeConstants"/>, <see cref="AccountTypeConstants"/> or <see cref="ContentTypeConstants"/>.</param>
        /// <returns>ID of type record in the database.</returns>
        public Guid GetTypeId(string name) =>
            _types.GetValueOrDefault(name);

        /// <summary>
        /// Connect to Docker container and create the database.
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Connect to database container.
            string connectionString = _msSqlContainer.GetConnectionString() + ";Persist Security Info=true";
            _connection ??= new(connectionString);
            await _connection.OpenAsync();

            // Create options.
            _options = new DbContextOptionsBuilder<PubHubContext>()
                .UseSqlServer(_connection)
                .Options;

            // Build database schema.
            await Context.Database.EnsureCreatedAsync();
        }

        /// <summary>
        /// Add seeding data to the database.
        /// </summary>
        /// <exception cref="DbUpdateException"></exception>
        private async Task SeedDatabaseAsync()
        {
            _types.Clear();
            RememberType((await Context.Set<AccountType>().AddAsync(new() { Name = AccountTypeConstants.USER_ACCOUNT_TYPE })).Entity);
            RememberType((await Context.Set<AccountType>().AddAsync(new() { Name = AccountTypeConstants.PUBLISHER_ACCOUNT_TYPE })).Entity);
            RememberType((await Context.Set<AccountType>().AddAsync(new() { Name = AccountTypeConstants.OPERATOR_ACCOUNT_TYPE })).Entity);
            RememberType((await Context.Set<AccountType>().AddAsync(new() { Name = AccountTypeConstants.SUSPENDED_ACCOUNT_TYPE })).Entity);
            
            RememberType((await Context.Set<AccessType>().AddAsync(new() { Name = AccessTypeConstants.OWNER_ACCESS_TYPE })).Entity);
            RememberType((await Context.Set<AccessType>().AddAsync(new() { Name = AccessTypeConstants.SUBSCRIBER_ACCESS_TYPE })).Entity);
            RememberType((await Context.Set<AccessType>().AddAsync(new() { Name = AccessTypeConstants.BORROWER_ACCESS_TYPE })).Entity);
            RememberType((await Context.Set<AccessType>().AddAsync(new() { Name = AccessTypeConstants.EXPIRED_ACCESS_TYPE })).Entity);

            RememberType((await Context.Set<ContentType>().AddAsync(new() { Name = ContentTypeConstants.E_BOOK_CONTENT_TYPE })).Entity);
            RememberType((await Context.Set<ContentType>().AddAsync(new() { Name = ContentTypeConstants.AUDIO_CONTENT_TYPE })).Entity);

            if (await Context.SaveChangesAsync() == 0)
                throw new DbUpdateException("Unable to update database with seeding data.");
        }

        /// <summary>
        /// Store type in <see cref="_types"/>.
        /// </summary>
        /// <param name="type">Information to store. <see cref="ITypeEntity.Name"/> must be globally unique.</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void RememberType(ITypeEntity type)
        {
            if (!_types.TryAdd(type.Name, type.Id))
                throw new InvalidOperationException($"Unable to store \"{type.Name}\" (key) and '{type.Id}' (value) in the '{nameof(_types)}' dictionary.");
        }

        /// <summary>
        /// Create delete script for reseting.
        /// </summary>
        /// <exception cref="NullReferenceException"><see cref="_connection"/> was null.</exception>
        private async Task ConfigureRespawner()
        {
            if (_connection == null)
                throw new NullReferenceException("Connection was null when trying to configure Respawner.");

            _respawner = await Respawner.CreateAsync(_connection, new()
            {
                DbAdapter = DbAdapter.SqlServer
            });
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            // Remove the Docker container.
            await _msSqlContainer.DisposeAsync();
        }
    }
}
