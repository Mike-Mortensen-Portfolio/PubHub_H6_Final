using System.Data.SqlTypes;
using PubHub.API.Domain.Entities;
using PubHub.API.UT.Utilities;

namespace PubHub.API.UT.Data
{
    public sealed class SqlTests : IClassFixture<DatabaseFixture>, IAsyncLifetime
    {
        private readonly DatabaseFixture _databaseFixture;

        public SqlTests(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
        }

        public PubHubContext Context => _databaseFixture.Context;

        public Task InitializeAsync() => Task.CompletedTask;

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _databaseFixture.CleanUpAsync();
        }

        [Fact]
        public async Task InsertWithSequentialUuids()
        {
            // Arrange.
            List<Genre> genres = [];
            var charValueStart = int.Parse("61", System.Globalization.NumberStyles.HexNumber);
            var charValueEnd = int.Parse("7A", System.Globalization.NumberStyles.HexNumber);
            for (int i = charValueStart; i < charValueEnd; i++)
            {
                genres.Add(new() { Name = $"{(char)i}" });
            }

            // Act.
            await Context.Set<Genre>().AddRangeAsync(genres);
            await Context.SaveChangesAsync();

            // Assert.
            var orderedGenres = genres.OrderBy(g => g.Name);
            SqlGuid uuid = SqlGuid.Null;
            SqlGuid previousUuid = SqlGuid.Null;

            foreach (var genre in orderedGenres)
            {
                try
                {
                    uuid = SqlGuid.Parse(genre.Id.ToString());
                    if (previousUuid.IsNull)
                        continue;

                    // Assert that current UUID is greater than the previous UUID.
                    Assert.True(SqlGuid.GreaterThan(uuid, previousUuid).Value);
                }
                finally
                {
                    previousUuid = uuid;
                }
            }
        }
    }
}
