using System.Net;
using System.Text.Json;
using PubHub.API.FT.Utilities;
using PubHub.Common.Models.Genres;
using PubHub.TestUtils.Extensions;

namespace PubHub.API.FT
{
    public class LoadTests : IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _apiFixture;

        public LoadTests(ApiFixture apiFixture)
        {
            _apiFixture = apiFixture;
        }

        [Fact]
        public async Task TriggerRateLimiterAsync()
        {
            // Arrange - Endpoint to call.
            List<GenreInfoModel>? genres = null;
            using (var client = _apiFixture.GetClient())
            {
                var response = await client.GetAsync("genres");
                Assert.True(response.IsSuccessStatusCode);
                var content = await response.Content.ReadAsStringAsync();
                genres = JsonSerializer.Deserialize<List<GenreInfoModel>>(content, _apiFixture.SerializerOptions);
                Assert.NotNull(genres);
            }
            var testEndpoint = $"genres/{genres.Random()}";

            // Arrange - Clients.
            var clientCount = 10;
            List<(HttpClient Client, uint RequestCount, uint TooManyRequestsResponseCount)> entries = [];
            for (int i = 0; i < clientCount; i++)
            {
                entries.Add((_apiFixture.GetClient(), 0, 0));
            }

            CancellationTokenSource source = new();
            var cancellationToken = source.Token;
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = clientCount,
                CancellationToken = cancellationToken
            };

            // Act.
            var parallelLoop = Parallel.ForEachAsync(entries, parallelOptions, async (entry, token) =>
            {
                while (!token.IsCancellationRequested)
                {
                    var response = await entry.Client.GetAsync(testEndpoint, token);
                    if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        entry.TooManyRequestsResponseCount++;
                    entry.RequestCount++;
                }
            });
            Thread.Sleep(10000);
            await parallelLoop;

            // Assert.
            uint tooManyRequestsResponseTotalCount = 0;
#pragma warning disable IDE0042 // Deconstruct variable declaration
            foreach (var entry in entries)
            {
                tooManyRequestsResponseTotalCount += entry.TooManyRequestsResponseCount;
            }
#pragma warning restore IDE0042 // Deconstruct variable declaration

            Assert.True(tooManyRequestsResponseTotalCount > 0);
        }
    }
}
