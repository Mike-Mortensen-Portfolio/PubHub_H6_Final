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
        public async Task<ServiceResult<UserCreatedResponseModel>> RegisterUserAsync(UserCreateModel userCreateModel)
        {           
            try
            {
                if (userCreateModel == null)
                    return new ServiceResult<UserCreatedResponseModel>(HttpStatusCode.InternalServerError, null, $"The user create model wasn't valid.");

                var userModelValues = JsonSerializer.Serialize(userCreateModel);

                if (userModelValues == null)
                    return new ServiceResult<UserCreatedResponseModel>(HttpStatusCode.InternalServerError, null, $"Unable to serialize the userCreateModel to json.");

                HttpContent httpContent = new StringContent(userModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("auth/user", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userCreatedResponseModel = JsonSerializer.Deserialize<UserCreatedResponseModel>(content, _serializerOptions);
                if (userCreatedResponseModel == null)
                    return new ServiceResult<UserCreatedResponseModel>(response.StatusCode, null, $"Unable to handle the user create response, status code: {response.StatusCode}");

                return new ServiceResult<UserCreatedResponseModel>(response.StatusCode, userCreatedResponseModel, $"Successfully added the user: {userCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the name: {userCreateModel.Name}, ", ex.Message);
                return new ServiceResult<UserCreatedResponseModel>(HttpStatusCode.InternalServerError, null, $"Failed to add the user.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to retrieve a token to login through the <see cref="LoginInfo"/>.
        /// </summary>
        /// <param name="loginInfo">Model containing the login information</param>
        /// <returns>A <see cref="TokenResponseModel"/> which contains the signed in user's token.</returns>
        /// <exception cref="ArgumentNullException"></exception>>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(loginInfo);

                HttpContent httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                httpContent.Headers.Add("Email", loginInfo.Email);
                httpContent.Headers.Add("Password", loginInfo.Password);

                HttpResponseMessage response = await Client.PostAsync($"auth/token", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(content, _serializerOptions);
                if (tokenResponseModel == null)
                    return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Token response, status code: {response.StatusCode}");

                return new ServiceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully logging in.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to login, ", ex.Message);
                return new ServiceResult<TokenResponseModel>(HttpStatusCode.Unauthorized, null, $"Failed to login.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to refresh an account's token.
        /// </summary>
        /// <param name="tokenInfo">A <see cref="TokenInfo"/> holding the token information.</param>
        /// <returns>A <see cref="ServiceInstanceResult{TokenResponseModel}"/> with the new token and refresh token.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<TokenResponseModel>> RefreshTokenAsync(TokenInfo tokenInfo)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(tokenInfo);

                HttpContent httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                httpContent.Headers.Add("ExpiredToken", tokenInfo.Token);
                httpContent.Headers.Add("RefreshToken", tokenInfo.RefreshToken);

                HttpResponseMessage response = await Client.PostAsync($"auth/refresh", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var tokenResponseModel = JsonSerializer.Deserialize<TokenResponseModel>(content, _serializerOptions);
                if (tokenResponseModel == null)
                    return new ServiceResult<TokenResponseModel>(response.StatusCode, null, $"Unable to handle the Token response, status code: {response.StatusCode}");

                return new ServiceResult<TokenResponseModel>(response.StatusCode, tokenResponseModel, $"Successfully refreshed token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to refresh the token, ", ex.Message);
                return new ServiceResult<TokenResponseModel>(HttpStatusCode.Unauthorized, null, $"Failed to refresh token.");
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
                ArgumentNullException.ThrowIfNull(tokenInfo);

                HttpContent httpContent = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                httpContent.Headers.Add("Token", tokenInfo.Token);
                httpContent.Headers.Add("RefreshToken", tokenInfo.RefreshToken);

                HttpResponseMessage response = await Client.PostAsync($"auth/revoke", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully revoked the token.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to revoke token, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unauthorized, $"Failed to revoke token.");
            }
        }
    }
}
