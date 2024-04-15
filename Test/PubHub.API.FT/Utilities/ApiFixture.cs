using System.Text.Json;

namespace PubHub.API.FT.Utilities
{
    public class ApiFixture
    {
        private const string API_BASE_ADDRESS = "";

        public JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public HttpClient GetClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = 
            };

            return client;
        }
    }
}
