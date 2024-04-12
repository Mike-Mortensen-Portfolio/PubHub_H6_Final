﻿using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class UserService : ServiceRoot, IUserService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public UserService(IHttpClientService clientService) : base(clientService)
        {
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
        /// <returns>A <see cref="UserInfoModel"/> with the users information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<UserInfoModel>> GetUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}"); ;
                }

                var userInfoModel = JsonSerializer.Deserialize<UserInfoModel>(content, _serializerOptions);
                if (userInfoModel == null)
                    return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<UserInfoModel>(response.StatusCode, userInfoModel, $"Successfully retrieved the user: {userInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get user info failed:", ex.Message);
                return new ServiceResult<UserInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all of a user's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="userId">The Id of the user who's books needs retrieval.</param>
        /// <returns>A list of <see cref="BookInfoModel"/></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetUserBooksAsync(Guid userId, BookQuery queryOptions = null!)
        {
            try
            {
                var filter = queryOptions ?? new BookQuery();
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}/books?{filter.ToQuery(ignoreNull: true)}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var books = JsonSerializer.Deserialize<List<BookInfoModel>>(content, _serializerOptions);
                if (books == null)
                    return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, new List<BookInfoModel>(), $"Unable to map the request over to the client.");

                return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, books, "Successfully retrieved user's books!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get user books: {userId}", ex.Message);
                return new ServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.Unused, null, $"Unable to retrieve user's books: {ex.Message}.");
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
        public async Task<ServiceResult<UserInfoModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                ArgumentNullException.ThrowIfNull(userUpdateModel);

                var userModelValues = JsonSerializer.Serialize(userUpdateModel) ?? throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpResponseMessage response = await Client.PutAsync($"users/{userId}", userModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userInfoModel = JsonSerializer.Deserialize<UserInfoModel>(content, _serializerOptions);
                if (userInfoModel == null)
                    return new ServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<UserInfoModel>(response.StatusCode, userInfoModel, $"Successfully updated the user: {userInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the user: {userUpdateModel.Name}, ", ex.Message);
                return new ServiceResult<UserInfoModel>(HttpStatusCode.Unused, null, $"Failed to update the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a user./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(HttpStatusCode.InternalServerError, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the user: {userId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to change the user's account type to suspended./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to have their account type as suspended.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> SuspendUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}/suspend-user");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully suspended the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to suspend the user: {userId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to suspend the user: {ex.Message}.");
            }
        }
    }
}
