using System.Net.Mime;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;

namespace PubHub.API.FT.Utilities
{
    public class ApiFixture
    {
        private const string API_BASE_ADDRESS = "https://localhost:7097/";
        private const string APP_ID = "adminportal_f9550d49-bc24-4c5f-a88d-ef493cfb3901";
        public const string OPERATOR_EMAIL = "operator@test.com";
        public const string OPERATOR_PASSWORD = "P@ssw0rd";
        public const string PUBLISHER_EMAIL = "publisher@test.com";
        public const string PUBLISHER_PASSWORD = "P@ssw0rd";

        private TokenResponseModel? _lastTokenResponse = null;
        private string _lastEmail = null!;
        private string _lastPassword = null!;

        public JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public static HttpClient GetClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new(API_BASE_ADDRESS)
            };
            client.DefaultRequestHeaders.Add(ApiConstants.APP_ID, APP_ID);

            return client;
        }

        public async Task<HttpClient> GetAuthenticatedClientCopyAsync(string email, string password)
        {
            if (_lastTokenResponse == null && _lastEmail != email && _lastPassword != password)
            {

                return await GetAuthenticatedClientAsync(email, password);
            }

            var client = GetClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _lastTokenResponse!.Token);
            client.DefaultRequestHeaders.Add("refreshToken", _lastTokenResponse.RefreshToken);

            return client;
        }

        public async Task<HttpClient> GetAuthenticatedClientAsync(string email, string password)
        {
            var client = GetClient();
            LoginInfo loginInfo = new() { Email = email, Password = password };
            var serializedLoginInfo = JsonSerializer.Serialize(loginInfo);
            var requestContent = new StringContent(serializedLoginInfo ?? string.Empty, System.Text.Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("auth/token", requestContent);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(responseContent, SerializerOptions);
                if (tokenResponseModel != null)
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponseModel!.Token);
                    client.DefaultRequestHeaders.Add("refreshToken", tokenResponseModel!.RefreshToken);
                    _lastTokenResponse = tokenResponseModel;
                    _lastEmail = email;
                    _lastPassword = password;
                }
            }
            else
            {
                throw new UnauthorizedAccessException("Unable to create and authenticate HttpClient.");
            }

            return client;
        }
    }
}
