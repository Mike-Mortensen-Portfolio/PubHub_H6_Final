using PubHub.Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PubHub.Common.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public UserService()
        {
            _httpClient = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
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

                Uri uri = new Uri(string.Format(AccountTypeConstants.REST_URL, $"users/{userId}"));

                HttpResponseMessage response = await _httpClient.GetAsync(uri);
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
        /// Calls the API endpoint for adding a <see cref="UserCreateModel"/> to the database.
        /// </summary>
        /// <param name="userCreateModel">The <see cref="UserCreateModel"/> holding the new user.</param>
        /// <returns>A status telling if a user was successfully added to the database.</returns>
        public async Task<string> CreateUser(UserCreateModel userCreateModel)
        {
            try
            {
                if (userCreateModel == null)
                    throw new ArgumentNullException($"The User create model wasn't valid: {userCreateModel?.Name}");

                Uri uri = new Uri(string.Format(AccountTypeConstants.REST_URL, $"users"));

                var userModelValues = JsonSerializer.Serialize(userCreateModel);

                if (userModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userCreateModel to json.");

                HttpContent httpContent = new StringContent(userModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(uri.ToString(), httpContent);
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

                Uri uri = new Uri(string.Format(AccountTypeConstants.REST_URL, $"users/{userId}"));

                var userModelValues = JsonSerializer.Serialize(userUpdateModel);

                if (userModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpContent httpContent = new StringContent(userModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(uri.ToString(), httpContent);
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
    }
}
