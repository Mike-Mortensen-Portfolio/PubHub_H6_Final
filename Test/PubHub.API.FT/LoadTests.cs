using System.Net;
using System.Text.Json;
using PubHub.API.FT.Utilities;
using PubHub.Common.Models.Genres;
using PubHub.TestUtils.Extensions;
using Xunit.Abstractions;

namespace PubHub.API.FT
{
    public class LoadTests : IClassFixture<ApiFixture>
    {
        private readonly ApiFixture _apiFixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public LoadTests(ApiFixture apiFixture, ITestOutputHelper testOutputHelper)
        {
            _apiFixture = apiFixture;
            _testOutputHelper = testOutputHelper;
        }

        public class ClientEntry
        {
            public ClientEntry(HttpClient client, uint requestCount, uint tooManyRequestsResponseCount)
            {
                Client = client;
                RequestCount = requestCount;
                TooManyRequestsResponseCount = tooManyRequestsResponseCount;
            }

            public HttpClient Client { get; set; } = null!;
            public uint RequestCount { get; set; }
            public uint TooManyRequestsResponseCount { get; set; }
        }

        [Fact]
        public async Task TriggerRateLimiterAsync()
        {
            // Arrange - Endpoint to call.
            List<GenreInfoModel>? genres = null;
            using (var client = await _apiFixture.GetAuthenticatedClientAsync())
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
            List<ClientEntry> clientEntries = [];
            for (int i = 0; i < clientCount; i++)
            {
                clientEntries.Add(new(await _apiFixture.GetAuthenticatedClientCopyAsync(), 0, 0));
            }

            CancellationTokenSource source = new();
            var cancellationToken = source.Token;
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = clientCount,
                CancellationToken = cancellationToken
            };

            // Act.
            try
            {
                var parallelLoop = Parallel.ForEachAsync(clientEntries, parallelOptions, async (clientEntry, token) =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var response = await clientEntry.Client.GetAsync(testEndpoint, token);
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                            clientEntry.TooManyRequestsResponseCount += 1;
                        clientEntry.RequestCount += 1;
                    }
                });
                source.CancelAfter(10000);
                await parallelLoop;
            }
            catch (Exception)
            { }

            // Assert.
            uint requestTotalCount = 0;
            uint tooManyRequestsResponseTotalCount = 0;
            for (int i = 0; i < clientEntries.Count; i++)
            {
                requestTotalCount += clientEntries[i].RequestCount;
                tooManyRequestsResponseTotalCount += clientEntries[i].TooManyRequestsResponseCount;
                _testOutputHelper.WriteLine($"Client {i}: Sent {clientEntries[i].RequestCount} - Rejected {clientEntries[i].TooManyRequestsResponseCount}");

            }

            _testOutputHelper.WriteLine($"Total number of requests made: {requestTotalCount}");
            _testOutputHelper.WriteLine($"Total '429 Too Many Requests' responses received: {tooManyRequestsResponseTotalCount}");
            Assert.True(requestTotalCount > 0);
            Assert.True(tooManyRequestsResponseTotalCount > 0);
        }

        [Fact]
        public async Task DontTriggerRateLimiterAsync()
        {
            // Arrange - Endpoint to call.
            List<GenreInfoModel>? genres = null;
            using (var client = await _apiFixture.GetAuthenticatedClientAsync())
            {
                var response = await client.GetAsync("genres");
                Assert.True(response.IsSuccessStatusCode);
                var content = await response.Content.ReadAsStringAsync();
                genres = JsonSerializer.Deserialize<List<GenreInfoModel>>(content, _apiFixture.SerializerOptions);
                Assert.NotNull(genres);
            }
            var testEndpoint = $"genres/{genres.Random()}";

            // Arrange - Clients.
            var clientCount = 5;
            List<ClientEntry> clientEntries = [];
            for (int i = 0; i < clientCount; i++)
            {
                clientEntries.Add(new(await _apiFixture.GetAuthenticatedClientCopyAsync(), 0, 0));
            }

            CancellationTokenSource source = new();
            var cancellationToken = source.Token;
            ParallelOptions parallelOptions = new()
            {
                MaxDegreeOfParallelism = clientCount,
                CancellationToken = cancellationToken
            };

            // Act.
            try
            {
                var parallelLoop = Parallel.ForEachAsync(clientEntries, parallelOptions, async (clientEntry, token) =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var response = await clientEntry.Client.GetAsync(testEndpoint, token);
                        if (response.StatusCode == HttpStatusCode.TooManyRequests)
                            clientEntry.TooManyRequestsResponseCount += 1;
                        clientEntry.RequestCount += 1;
                    }
                });
                source.CancelAfter(10000);
                await parallelLoop;
            }
            catch (Exception)
            { }

            // Assert.
            uint requestTotalCount = 0;
            uint tooManyRequestsResponseTotalCount = 0;
            for (int i = 0; i < clientEntries.Count; i++)
            {
                requestTotalCount += clientEntries[i].RequestCount;
                tooManyRequestsResponseTotalCount += clientEntries[i].TooManyRequestsResponseCount;
                _testOutputHelper.WriteLine($"Client {i}: Sent {clientEntries[i].RequestCount} - Rejected {clientEntries[i].TooManyRequestsResponseCount}");

            }

            _testOutputHelper.WriteLine($"Total number of requests made: {requestTotalCount}");
            _testOutputHelper.WriteLine($"Total '429 Too Many Requests' responses received: {tooManyRequestsResponseTotalCount}");
            Assert.True(requestTotalCount > 0);
            Assert.True(tooManyRequestsResponseTotalCount == 0);
        }
    }
}
