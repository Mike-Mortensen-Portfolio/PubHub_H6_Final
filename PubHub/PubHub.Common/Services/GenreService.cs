using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class GenreService : ServiceRoot, IGenreService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public GenreService(IHttpClientService clientService) : base(clientService)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all genres through the <see cref="GenreInfoModel"/>.
        /// </summary>
        /// <returns>A list of <see cref="GenreInfoModel"/></returns>
        public async Task<HttpServiceResult<IReadOnlyList<GenreInfoModel>>> GetAllGenresAsync()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"genres");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var genreInfoModel = JsonSerializer.Deserialize<List<GenreInfoModel>>(content, _serializerOptions);

                if (genreInfoModel == null)
                    return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(response.StatusCode, genreInfoModel, "Successfully retrieved all genres.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get genres:", ex.Message);
                return new HttpServiceResult<IReadOnlyList<GenreInfoModel>>(HttpStatusCode.Unused, null, $"Unable retrieve all genres: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="GenreInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="genreId">Id of the genre we want information about.</param>
        /// <returns>A <see cref="GenreInfoModel"/> with a genre's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<GenreInfoModel>> GetGenreAsync(Guid genreId)
        {
            try
            {
                if (genreId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The genre Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"genres/{genreId}");
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<GenreInfoModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<GenreInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<GenreInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var genreInfoModel = JsonSerializer.Deserialize<GenreInfoModel>(content, _serializerOptions);
                if (genreInfoModel == null)
                    return new HttpServiceResult<GenreInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<GenreInfoModel>(response.StatusCode, genreInfoModel, $"Successfully retrieved the genre: {genreInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get genre info failed:", ex.Message);
                return new HttpServiceResult<GenreInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve the genre: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="GenreCreateModel"/> to the database.
        /// </summary>
        /// <param name="genreCreateModel">The <see cref="GenreCreateModel"/> holding the new genre.</param>
        /// <returns>A status telling if a genre was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<GenreCreateModel>> AddGenreAsync(GenreCreateModel genreCreateModel)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(genreCreateModel);

                var genreModelValues = JsonSerializer.Serialize(genreCreateModel) ?? throw new NullReferenceException($"Unable to serialize the genreCreateModel to json.");

                HttpResponseMessage response = await Client.PostAsync("genres", genreModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                    return new HttpServiceResult<GenreCreateModel>(response.StatusCode, null, $"Too many requests. Try again later, status code: {(int)response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<GenreCreateModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<GenreCreateModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new HttpServiceResult<GenreCreateModel>(response.StatusCode, genreCreateModel, $"Successfully added the genre: {genreCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the genre: {genreCreateModel.Name}, ", ex.Message);
                return new HttpServiceResult<GenreCreateModel>(HttpStatusCode.Unused, null, $"Failed to add the genre: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a genre./>
        /// </summary>
        /// <param name="genreId">The Id of the genre who needs to be soft-deleted.</param>
        /// <returns>A <see cref="HttpServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> DeleteGenreAsync(Guid genreId)
        {
            try
            {
                if (genreId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The genre Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"genres/{genreId}");
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

                return new HttpServiceResult(response.StatusCode, $"Successfully deleted the genre: {genreId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the genre: {genreId}, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to delete the genre: {ex.Message}.");
            }
        }
    }
}
