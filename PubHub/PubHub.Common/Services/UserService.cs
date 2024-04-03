using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class UserService : ServiceRoot, IUserService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        public UserService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="UserCreateModel"/> to the database.
        /// </summary>
        /// <param name="userCreateModel">The <see cref="UserCreateModel"/> holding the new user information.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
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

        /// <summary>
        /// Calls the API end point for retrieving <see cref="UserInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="userId">Id of the user wanting information about.</param>
        /// <returns>A <see cref="UserInfoModel"/> with the users information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<UserInfoModel?> GetUserInfo(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {userId}");

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

        /// <summary>
        /// Calls the API enpoint to retrieve all of a user's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="userId">The Id of the user who's books needs retrieval.</param>
        /// <returns>A list of <see cref="BookInfoModel"/></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<List<BookInfoModel>> GetUserBooks(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {userId}");

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
                Debug.WriteLine($"Unable to get user books: {userId}", ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Calls the API endpoint for updating <see cref="UserUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="userId">The id of the user being updated.</param>
        /// <param name="userUpdateModel">The <see cref="UserUpdateModel"/> holding the updated values.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<UserUpdateModel>> UpdateUser(Guid userId, UserUpdateModel userUpdateModel)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {userId}");

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

        /// <summary>
        /// Calls the API endpoint to soft-delete a user./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteUser(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {userId}");

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

        /// <summary>
        /// Calls the API endpoint to change the user's account type to suspended./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to have their account type as suspended.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> SuspendUser(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {userId}");

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
