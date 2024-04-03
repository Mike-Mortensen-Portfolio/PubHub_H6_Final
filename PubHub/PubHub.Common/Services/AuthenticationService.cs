using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PubHub.Common.ApiService;
using PubHub.Common.Models;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;

namespace PubHub.Common.Services
{
    public class AuthenticationService : ServiceRoot
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal AuthenticationService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        public async Task<ServiceInstanceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo)
        {
            try
            {
                if (loginInfo == null)
                    throw new ArgumentNullException($"The login info wasn't valid.");

                var loginValues = JsonSerializer.Serialize(loginInfo, _serializerOptions);

                if (loginValues == null)
                    throw new NullReferenceException($"Unable to serialize the loginInfo to json.");
                
                HttpContent httpContent = new StringContent(string.Empty);
                httpContent.Headers.Add("email", loginInfo.Email);
                httpContent.Headers.Add("password", loginInfo.Password);

                HttpResponseMessage response = await Client.PostAsync($"auth/token", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                TokenResponseModel? tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(content, _serializerOptions);
                if (tokenResponseModel == null)
                    throw new NullReferenceException($"Unable to handle the Token response, status code: {response.StatusCode}");

                return new ServiceInstanceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully logging in: {loginInfo.Email}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to login: {loginInfo.Email}, ", ex.Message);
                return new ServiceInstanceResult<TokenResponseModel>(HttpStatusCode.Unused, new TokenResponseModel(string.Empty, string.Empty), $"Failed to login: {loginInfo.Email}");
            }
        }

        public async Task<ServiceInstanceResult<TokenResponseModel>> RefreshTokenAsync()
        {

        }
    }
}
