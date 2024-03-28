using PubHub.Common.ApiService;
using PubHub.Common.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace PubHub.Common.Services
{
    public class UserService : ServiceRoot
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly JsonSerializerOptions _serializerOptions;

        internal UserService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _httpClientFactory = clientFactory;

            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="UserCreateModel"/> to the database.
        /// </summary>
        /// <param name="userCreateModel">The <see cref="UserCreateModel"/> holding the new user.</param>
        /// <returns>A status telling if a user was successfully added to the database.</returns>
        public async Task<string> AddUser(UserCreateModel userCreateModel)
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

                return $"Successfully added the user: {userCreateModel.Name}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the user: {userCreateModel.Name}, ", ex.Message);
                return $"Failed to add the user: {userCreateModel.Name}";
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="UserInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="userId">Id of the user wanting information about.</param>
        /// <returns></returns>
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
                if(userInfoModel == null)
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
        /// <returns>Returns a list of <see cref="BookInfoModel"/></returns>
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

        /// <summary>
        /// Calls the API endpoint for updating <see cref="UserUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="userId">The id of the user being updated.</param>
        /// <param name="userUpdateModel">The <see cref="UserUpdateModel"/> holding the updated values.</param>
        /// <returns>A status telling if a user was successfully updated in the database.</returns>
        public async Task<string> UpdateUser(int userId, UserUpdateModel userUpdateModel)
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

                return $"Successfully updated the user: {userUpdateModel.Name}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the user: {userUpdateModel.Name}, ", ex.Message);
                return $"Failed to update the user: {userUpdateModel.Name}";
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a user./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to be soft-deleted.</param>
        /// <returns></returns>
        public async Task<string> DeleteUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.DeleteAsync("users");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return $"Successfully deleted the user: {userId}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the user: {userId}, ", ex.Message);
                return $"Failed to delete the user: {userId}";
            }
        }

        /// <summary>
        /// Calls the API endpoint to change the user's account type to suspended./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to have their account type as suspended.</param>
        /// <returns></returns>
        public async Task<string> SuspendUser(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {userId}");

                HttpResponseMessage response = await Client.DeleteAsync("users");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return $"Successfully suspended the user: {userId}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to suspend the user: {userId}, ", ex.Message);
                return $"Failed to suspend the user: {userId}";
            }
        }
    }
}
