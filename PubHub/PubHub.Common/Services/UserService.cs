using PubHub.Common.ApiService;
using PubHub.Common.Models;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PubHub.Common.Services
{
    public class UserService : ServiceRoot, IUserService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal UserService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        // TODO (JBN): Change to GUIDs instead of int when that has been updated.

        public async Task<ServiceInstanceResult<UserCreateModel>> AddUser(UserCreateModel userCreateModel)
        {
            try
            {
                if (userCreateModel == null)
                    throw new ArgumentNullException($"The User create model wasn't valid: {userCreateModel?.Name}");

                var userModelValues = JsonSerializer.Serialize(userCreateModel);

                if (userModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userCreateModel to json.");

                HttpContent httpContent = new StringContent(userModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("users", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<UserCreateModel>(response.StatusCode, userCreateModel, $"Successfully added the user: {userCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the user: {userCreateModel.Name}, ", ex.Message);
                return new ServiceInstanceResult<UserCreateModel>(HttpStatusCode.Unused, userCreateModel, $"Failed to add the user: {userCreateModel.Name}");
            }
        }

        public async Task<UserInfoModel?> GetUserInfo(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                UserInfoModel? userInfoModel = JsonSerializer.Deserialize<UserInfoModel>(content, _serializerOptions);
                if (userInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return userInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get user info failed:", ex.Message);
                return null;
            }
        }

        public async Task<List<BookInfoModel>> GetUserBooks(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}/books");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                List<BookInfoModel>? bookInfoModel = JsonSerializer.Deserialize<List<BookInfoModel>>(content, _serializerOptions);
                if (bookInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return bookInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get user books:", ex.Message);
                return null!;
            }
        }

        public async Task<ServiceInstanceResult<UserUpdateModel>> UpdateUser(int userId, UserUpdateModel userUpdateModel)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                if (userUpdateModel == null)
                    throw new ArgumentNullException($"The User update model wasn't valid: {userUpdateModel?.Name}");

                var userModelValues = JsonSerializer.Serialize(userUpdateModel);

                if (userModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpContent httpContent = new StringContent(userModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PutAsync($"users/{userId}", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<UserUpdateModel>(response.StatusCode, userUpdateModel, $"Successfully updated the user: {userUpdateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the user: {userUpdateModel.Name}, ", ex.Message);
                return new ServiceInstanceResult<UserUpdateModel>(HttpStatusCode.Unused, userUpdateModel, $"Failed to update the user: {userUpdateModel.Name}");
            }
        }

        public async Task<ServiceResult> DeleteUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the user: {userId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the user: {userId}");
            }
        }

        public async Task<ServiceResult> SuspendUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}/suspend-user");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully suspended the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to suspend the user: {userId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to suspend the user: {userId}");
            }
        }
    }
}
