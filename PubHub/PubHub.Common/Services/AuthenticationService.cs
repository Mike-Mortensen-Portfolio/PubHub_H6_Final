using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public class AuthenticationService : ServiceRoot, IAuthenticationService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public AuthenticationService(IHttpClientService clientService) : base(clientService)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API endpoint to register a new user with the <see cref="UserInfoModel"/>.
        /// </summary>
        /// <param name="userCreateModel">The user that wants to be registered</param>
        /// <returns>A <see cref="ServiceInstanceResult{UserCreatedResponseModel}"/> with the newly created user's tokens.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<UserCreatedResponseModel>> RegisterUserAsync(UserCreateModel userCreateModel)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(userCreateModel);

                var userModelValues = JsonSerializer.Serialize(userCreateModel) ?? throw new NullReferenceException($"Unable to serialize the userCreateModel to json.");

                HttpResponseMessage response = await Client.PostAsync("auth/user", userModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userCreatedResponseModel = JsonSerializer.Deserialize<UserCreatedResponseModel>(content, _serializerOptions);
                if (userCreatedResponseModel == null)
                    return new HttpServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to handle the user create response, status code: {response.StatusCode}");

                return new HttpServiceResult<UserCreatedResponseModel>(response.StatusCode, userCreatedResponseModel, $"Successfully added the user: {userCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the name: {userCreateModel.Name}, ", ex.Message);
                return new HttpServiceResult<UserCreatedResponseModel>(HttpStatusCode.Unused, null, $"Failed to add the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to retrieve a token to login through the <see cref="LoginInfo"/>.
        /// </summary>
        /// <param name="loginInfo">Model containing the login information</param>
        /// <returns>A <see cref="TokenResponseModel"/> which contains the signed in user's token.</returns>
        /// <exception cref="ArgumentNullException"></exception>>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(loginInfo);

                var loginInfoJson = JsonSerializer.Serialize(loginInfo);

                HttpResponseMessage response = await Client.PostAsync($"auth/token", loginInfoJson);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(content, _serializerOptions);
                if (tokenResponseModel == null)
                    return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Token response, status code: {response.StatusCode}");

                return new HttpServiceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully logging in.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to login, ", ex.Message);
                return new HttpServiceResult<TokenResponseModel>(HttpStatusCode.Unused, null, $"Failed to login: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to refresh an account's token.
        /// </summary>
        /// <param name="tokenInfo">A <see cref="TokenInfo"/> holding the token information.</param>
        /// <returns>A <see cref="ServiceInstanceResult{TokenResponseModel}"/> with the new token and refresh token.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<TokenResponseModel>> RefreshTokenAsync()
        {
            try
            {
                HttpResponseMessage response = await Client.PostAsync($"auth/refresh");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(content, _serializerOptions);
                if (tokenResponseModel == null)
                    return new HttpServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Token response, status code: {response.StatusCode}");

                return new HttpServiceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully refreshed token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to refresh the token, ", ex.Message);
                return new HttpServiceResult<TokenResponseModel>(HttpStatusCode.Unused, null, $"Failed to refresh token: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to revoke an account's token.
        /// </summary>
        /// <param name="tokenInfo">A <see cref="TokenInfo"/> containing the token and refresh token.</param>
        /// <returns>A <see cref="HttpServiceResult"/> which will tell how the request was handled.</returns>
        /// /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> RevokeTokenAsync()
        {
            try
            {
                HttpResponseMessage response = await Client.PostAsync($"auth/revoke");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new HttpServiceResult(response.StatusCode, $"Successfully revoked the token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to revoke token, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to revoke token: {ex.Message}.");
            }
        }
    }
}
