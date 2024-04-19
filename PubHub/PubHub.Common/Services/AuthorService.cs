using System.Diagnostics;
using System.Net;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Authors;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class AuthorService : ServiceRoot, IAuthorService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public AuthorService(IHttpClientService clientService,
            Func<Task<TokenInfo>> getTokenInfoAsync,
            Action<TokenInfo> setTokenInfoAsync,
            Action removeTokenInfoAsync) : base(clientService, getTokenInfoAsync, setTokenInfoAsync, removeTokenInfoAsync)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all authors through the <see cref="AuthorInfoModel"/>.
        /// </summary>
        /// <returns>A list of <see cref="AuthorInfoModel"/></returns>
        public async Task<HttpServiceResult<IReadOnlyList<AuthorInfoModel>>> GetAllAuthorsAsync()
        {
            await SetTokensAsync();
            await TryRefreshTokenAsync();

            try
            {
                HttpResponseMessage response = await Client.GetAsync($"authors");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>> (response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var authorInfoModel = JsonSerializer.Deserialize<List<AuthorInfoModel>>(content, _serializerOptions);

                if (authorInfoModel == null)
                    return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>>(response.StatusCode, authorInfoModel, $"Successfully retrieved all authors!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get authors:", ex.Message);
                return new HttpServiceResult<IReadOnlyList<AuthorInfoModel>>(HttpStatusCode.Unused, null, $"Unable to get authors: {ex.Message}");
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="AuthorInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="authorId">Id of the author we want information about.</param>
        /// <returns>A <see cref="AuthorInfoModel"/> with an author's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<AuthorInfoModel>> GetAuthorAsync(Guid authorId)
        {
            await SetTokensAsync();
            await TryRefreshTokenAsync();

            try
            {
                if (authorId == INVALID_ENTITY_ID)
                    throw new NullReferenceException("The author Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"authors/{authorId}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var authorInfoModel = JsonSerializer.Deserialize<AuthorInfoModel>(content, _serializerOptions);
                if (authorInfoModel == null)
                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, authorInfoModel, $"Successfully retrieved the author: {authorInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get author info failed: ", ex.Message);
                return new HttpServiceResult<AuthorInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve the author: {ex.Message}");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="AuthorCreateModel"/> to the database.
        /// </summary>
        /// <param name="authorCreateModel">The <see cref="AuthorCreateModel"/> holding the new author.</param>
        /// <returns>A status telling if an author was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<AuthorInfoModel>> AddAuthorAsync(AuthorCreateModel authorCreateModel)
        {
            await SetTokensAsync();
            await TryRefreshTokenAsync();

            try
            {
                ArgumentNullException.ThrowIfNull(authorCreateModel);

                var authorModelValues = JsonSerializer.Serialize(authorCreateModel) ?? throw new NullReferenceException($"Unable to serialize the authorCreateModel to json.");

                HttpResponseMessage response = await Client.PostAsync("authors", authorModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var authorInfo = JsonSerializer.Deserialize<AuthorInfoModel>(content, _serializerOptions);
                if (authorInfo == null)
                    return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, null, $"Unable to handle the author model, status code: {response.StatusCode}");

                return new HttpServiceResult<AuthorInfoModel>(response.StatusCode, authorInfo, $"Successfully added the author: {authorCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the author: {authorCreateModel.Name}, ", ex.Message);
                return new HttpServiceResult<AuthorInfoModel>(HttpStatusCode.Unused, null, $"Failed to add the author: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete an author./>
        /// </summary>
        /// <param name="authorId">The Id of the author who needs to be soft-deleted.</param>
        /// <returns>A <see cref="HttpServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> DeleteAuthorAsync(Guid authorId)
        {
            await SetTokensAsync();
            await TryRefreshTokenAsync();

            try
            {
                if (authorId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The author Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"authors/{authorId}");
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

                return new HttpServiceResult(response.StatusCode, $"Successfully deleted the author: {authorId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the author: {authorId}, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to delete the author: {ex.Message}.");
            }
        }
    }
}
