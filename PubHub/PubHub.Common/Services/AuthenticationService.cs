using System.Diagnostics;
using System.Net;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;

namespace PubHub.Common.Services
{
    public class AuthenticationService : ServiceRoot, IAuthenticationService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal AuthenticationService(IHttpClientService clientService, string clientName) : base(clientService, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API endpoint to retrieve a token to login through the <see cref="LoginInfo"/>.
        /// </summary>
        /// <param name="loginInfo">Model containing the login information</param>
        /// <returns>A <see cref="TokenResponseModel"/> which contains the signed in user's token.</returns>
        /// <exception cref="ArgumentNullException"></exception>>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo)
        {
            try
            {
                if (loginInfo == null)
                    throw new ArgumentNullException($"The login info wasn't valid.");
                
                HttpContent httpContent = new StringContent(string.Empty);
                httpContent.Headers.Add("Email", loginInfo.Email);
                httpContent.Headers.Add("Password", loginInfo.Password);

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

                return new ServiceInstanceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully logging in.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to login, ", ex.Message);
                return new ServiceInstanceResult<TokenResponseModel>(HttpStatusCode.Unused, new TokenResponseModel(string.Empty, string.Empty), $"Failed to login.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to refresh an account's token.
        /// </summary>
        /// <param name="tokenInfo">A <see cref="TokenInfo"/> holding the token information.</param>
        /// <returns>A <see cref="ServiceInstanceResult{TokenResponseModel}"/> with the new token and refresh token.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<TokenResponseModel>> RefreshTokenAsync(TokenInfo tokenInfo)
        {
            try
            {
                if (tokenInfo == null)
                    throw new ArgumentNullException($"The token info wasn't valid.");

                HttpContent httpContent = new StringContent(string.Empty);
                httpContent.Headers.Add("Token", tokenInfo.Token);
                httpContent.Headers.Add("RefreshToken", tokenInfo.RefreshToken);

                HttpResponseMessage response = await Client.PostAsync($"auth/refresh", httpContent);
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

                return new ServiceInstanceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully refreshed token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to refresh the token, ", ex.Message);
                return new ServiceInstanceResult<TokenResponseModel>(HttpStatusCode.Unused, new TokenResponseModel(string.Empty, string.Empty), $"Failed to refresh token.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to revoke an account's token.
        /// </summary>
        /// <param name="tokenInfo">A <see cref="TokenInfo"/> containing the token and refresh token.</param>
        /// <returns>A <see cref="ServiceResult"/> which will tell how the request was handled.</returns>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> RevokeTokenAsync(TokenInfo tokenInfo)
        {
            try
            {
                if (tokenInfo == null)
                    throw new ArgumentNullException($"The token info wasn't valid.");

                HttpContent httpContent = new StringContent(string.Empty);
                httpContent.Headers.Add("Token", tokenInfo.Token);
                httpContent.Headers.Add("RefreshToken", tokenInfo.RefreshToken);

                HttpResponseMessage response = await Client.PostAsync($"auth/revoke", httpContent);
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

                return new ServiceInstanceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully revoked the token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to revoke token, ", ex.Message);
                return new ServiceInstanceResult<TokenResponseModel>(HttpStatusCode.Unused, new TokenResponseModel(string.Empty, string.Empty), $"Failed to revoke token.");
            }
        }
    }
}
