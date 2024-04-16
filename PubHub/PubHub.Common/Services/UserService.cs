using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;
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
        public async Task<HttpServiceResult<UserInfoModel>> GetUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}"); ;
                }

                var userInfoModel = JsonSerializer.Deserialize<UserInfoModel>(content, _serializerOptions);
                if (userInfoModel == null)
                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<UserInfoModel>(response.StatusCode, userInfoModel, $"Successfully retrieved the user: {userInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get user info failed:", ex.Message);
                return new HttpServiceResult<UserInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all of a user's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="userId">The Id of the user who's books needs retrieval.</param>
        /// <returns>A list of <see cref="BookInfoModel"/></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<IReadOnlyList<BookInfoModel>>> GetUserBooksAsync(Guid userId, BookQuery? queryOptions = null)
        {
            try
            {
                var filter = queryOptions ?? new BookQuery();
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}/books?{filter.ToQuery(ignoreNull: true)}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var books = JsonSerializer.Deserialize<List<BookInfoModel>>(content, _serializerOptions);
                if (books == null)
                    return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, new List<BookInfoModel>(), $"Unable to map the request over to the client.");

                return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, books, "Successfully retrieved user's books!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get user books: {userId}", ex.Message);
                return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.Unused, null, $"Unable to retrieve user's books: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to retrieve the content of a specific book that the user should own.
        /// </summary>
        /// <param name="userId">Id of the user trying to access the book content.</param>
        /// <param name="bookId">Id of the book where content is requested.</param>
        /// <returns>A <see cref="HttpServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<UserBookContentModel>> GetUserBookContentAsync(Guid userId, Guid bookId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"users/{userId}/books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<UserBookContentModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<UserBookContentModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<UserBookContentModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userBookContent = JsonSerializer.Deserialize<UserBookContentModel>(content, _serializerOptions);
                if (userBookContent == null)
                    return new HttpServiceResult<UserBookContentModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<UserBookContentModel>(response.StatusCode, userBookContent, $"Successfully retrieved the content of the book.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get book content failed: {userId}", ex.Message);
                return new HttpServiceResult<UserBookContentModel>(HttpStatusCode.Unused, null, $"Failed to retrieve content of the book: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for updating <see cref="UserUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="userId">The id of the user being updated.</param>
        /// <param name="userUpdateModel">The <see cref="UserUpdateModel"/> holding the updated values.</param>
        /// <returns>A <see cref="HttpServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<UserInfoModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                ArgumentNullException.ThrowIfNull(userUpdateModel);

                var userModelValues = JsonSerializer.Serialize(userUpdateModel) ?? throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpResponseMessage response = await Client.PutAsync($"users/{userId}", userModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userInfoModel = JsonSerializer.Deserialize<UserInfoModel>(content, _serializerOptions);
                if (userInfoModel == null)
                    return new HttpServiceResult<UserInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<UserInfoModel>(response.StatusCode, userInfoModel, $"Successfully updated the user: {userInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the user: {userUpdateModel.Name}, ", ex.Message);
                return new HttpServiceResult<UserInfoModel>(HttpStatusCode.Unused, null, $"Failed to update the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a user./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to be soft-deleted.</param>
        /// <returns>A <see cref="HttpServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> DeleteUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult(response.StatusCode, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult(HttpStatusCode.InternalServerError, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new HttpServiceResult(response.StatusCode, $"Successfully deleted the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the user: {userId}, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to delete the user: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to change the user's account type to suspended./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to have their account type as suspended.</param>
        /// <returns>A <see cref="HttpServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> SuspendUserAsync(Guid userId)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"users/{userId}/suspend-user");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult(response.StatusCode, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new HttpServiceResult(response.StatusCode, $"Successfully suspended the user: {userId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to suspend the user: {userId}, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to suspend the user: {ex.Message}.");
            }
        }

        public async Task<HttpServiceResult<UserBookInfoModel>> UpdateBookProgressAsync(Guid userId, UserBookUpdateModel updateModel)
        {
            try
            {
                if (userId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                var jsonString = JsonSerializer.Serialize<UserBookUpdateModel>(updateModel, _serializerOptions);

                HttpResponseMessage response = await Client.PutAsync($"users/{userId}/books", jsonString);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<UserBookInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<UserBookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<UserBookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var userBook = JsonSerializer.Deserialize<UserBookInfoModel>(content, _serializerOptions);
                if (userBook == null)
                    return new HttpServiceResult<UserBookInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<UserBookInfoModel>(response.StatusCode, userBook, "Successfully retrieved user's books!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get user books: {userId}", ex.Message);
                return new HttpServiceResult<UserBookInfoModel>(HttpStatusCode.Unused, null, $"Unable to update user's book: {ex.Message}.");
            }
        }
    }
}
