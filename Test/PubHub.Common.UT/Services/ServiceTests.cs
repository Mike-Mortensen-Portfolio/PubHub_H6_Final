using System.Text;
using System.Text.Json;
using AutoFixture;

namespace PubHub.Common.UT.Services
{
    public class ServiceTests : IClassFixture<DataGeneratorFixture>
    {
        private readonly JsonSerializerOptions _serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public ServiceTests(DataGeneratorFixture dataGeneratorFixture)
        {
            Generator = dataGeneratorFixture.Generator;
        }

        protected Fixture Generator { get; }

        public StringContent GetJsonContent(object obj) =>
            new(JsonSerializer.Serialize(obj, _serializerOptions), Encoding.UTF8, "application/json");
    }
}
