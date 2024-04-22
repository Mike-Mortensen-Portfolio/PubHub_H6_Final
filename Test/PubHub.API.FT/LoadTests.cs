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
            public ClientEntry(HttpClient client, int index = 0)
            {
                Client = client;
                Index = index;
            }

            public int Index { get; set; }
            public HttpClient Client { get; set; } = null!;
            public uint RequestCount { get; set; } = 0;
            public uint TooManyRequestsResponseCount { get; set; } = 0;
        }

        [Fact]
        public async Task TriggerRateLimiterAsync()
        {
            // Arrange - Endpoint to call.
            uint requestTotalCount = 0;
            uint tooManyRequestsResponseTotalCount = 0;
            List<GenreInfoModel>? genres = null;
            using (var client = await _apiFixture.GetAuthenticatedClientAsync(ApiFixture.OPERATOR_EMAIL, ApiFixture.OPERATOR_PASSWORD))
            {
                var response = await client.GetAsync("genres");
                requestTotalCount++;
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
                //if (i == 0)
                //    clientEntries.Add(new(await _apiFixture.GetAuthenticatedClientCopyAsync(ApiFixture.PUBLISHER_EMAIL, ApiFixture.PUBLISHER_PASSWORD), i));
                //else
                clientEntries.Add(new(await _apiFixture.GetAuthenticatedClientCopyAsync(ApiFixture.OPERATOR_EMAIL, ApiFixture.OPERATOR_PASSWORD), i));
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

                        await Task.Delay(TimeSpan.FromSeconds(10), token);
                    }
                });
                source.CancelAfter(TimeSpan.FromSeconds(10));
                await parallelLoop;
            }
            catch (Exception)
            { }

            // Assert.
            for (int i = 0; i < clientEntries.Count; i++)
            {
                requestTotalCount += clientEntries[i].RequestCount;
                tooManyRequestsResponseTotalCount += clientEntries[i].TooManyRequestsResponseCount;
                var successRate = (float)clientEntries[i].TooManyRequestsResponseCount / clientEntries[i].RequestCount * 100f;
                _testOutputHelper.WriteLine($"Client {i}: Sent {clientEntries[i].RequestCount} - Rejected {clientEntries[i].TooManyRequestsResponseCount} > Failure Rate: {successRate:00.00}%");

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
            using (var client = await _apiFixture.GetAuthenticatedClientAsync(ApiFixture.OPERATOR_EMAIL, ApiFixture.OPERATOR_PASSWORD))
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
                clientEntries.Add(new(await _apiFixture.GetAuthenticatedClientCopyAsync(ApiFixture.OPERATOR_EMAIL, ApiFixture.OPERATOR_PASSWORD), i));
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

                        await Task.Delay(TimeSpan.FromSeconds(5), token);
                    }
                });
                source.CancelAfter(TimeSpan.FromSeconds(10));
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
                var successRate = (float)tooManyRequestsResponseTotalCount / requestTotalCount * 100f;
                _testOutputHelper.WriteLine($"Client {i}: Sent {clientEntries[i].RequestCount} - Rejected {clientEntries[i].TooManyRequestsResponseCount} > Failure Rate: {successRate:00.00}%");
            }

            _testOutputHelper.WriteLine($"Total number of requests made: {requestTotalCount}");
            _testOutputHelper.WriteLine($"Total '429 Too Many Requests' responses received: {tooManyRequestsResponseTotalCount}");
            Assert.True(requestTotalCount > 0);
            Assert.True(tooManyRequestsResponseTotalCount == 0);
        }
    }
}
