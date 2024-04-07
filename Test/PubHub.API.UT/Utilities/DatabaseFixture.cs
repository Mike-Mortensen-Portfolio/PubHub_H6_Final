﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.MsSql;

namespace PubHub.API.UT.Utilities
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

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
                {
                    return true;
                }

                throw new InvalidOperationException($"{nameof(DatabaseFixture)} wasn't configured correctly.");
            }
        }

        private DbContextOptions<PubHubContext> _options = new();
        private SqlConnection? _connection;
        private PubHubContext? _context;
        private Respawner? _respawner;

        /// <summary>
        /// Database context to the current test database.
        /// </summary>
        public PubHubContext Context => _context ??= new PubHubContext(_options) { ApplySeed = false };

        public async Task InitializeAsync()
        {
            await _msSqlContainer.StartAsync();
            await CreateDatabaseAsync();
            await ConfigureRespawner();
        }

        /// <summary>
        /// Clean up the database after a test.
        /// </summary>
        public async Task CleanUpAsync()
        {
            if (IsConfigured)
            {
                await _respawner.ResetAsync(_connection);
            }
        }

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
        /// Create delete script for reseting.
        /// </summary>
        /// <exception cref="NullReferenceException"><see cref="_connection"/> was null.</exception>
        private async Task ConfigureRespawner()
        {
            if (_connection == null)
            {
                throw new NullReferenceException("Connection was null when trying to configure Respawner.");
            }

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