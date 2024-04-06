using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Authors;
using PubHub.Common.Models.Genres;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class GenreService : ServiceRoot, IGenreService
    {
#pragma warning disable IDE0270 // Use coalesce expression
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
        public async Task<IReadOnlyList<GenreInfoModel>> GetGenresAsync()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"genres");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($"Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var genreInfoModel = JsonSerializer.Deserialize<List<GenreInfoModel>>(content, _serializerOptions);

                if (genreInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return genreInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get genres:", ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="GenreInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="genreId">Id of the genre we want information about.</param>
        /// <returns>A <see cref="GenreInfoModel"/> with a genre's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<GenreInfoModel>> GetGenreAsync(Guid genreId)
        {
            try
            {
                if (genreId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The genre Id wasn't a valid Id: {genreId}");

                HttpResponseMessage response = await Client.GetAsync($"genres/{genreId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($"Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var genreInfoModel = JsonSerializer.Deserialize<GenreInfoModel>(content, _serializerOptions);
                if (genreInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return new ServiceResult<GenreInfoModel>(response.StatusCode, genreInfoModel, $"Successfully retrieved the genre: {genreInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get genre info failed:", ex.Message);
                return new ServiceResult<GenreInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve the genre.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="GenreCreateModel"/> to the database.
        /// </summary>
        /// <param name="genreCreateModel">The <see cref="GenreCreateModel"/> holding the new genre.</param>
        /// <returns>A status telling if a genre was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<GenreCreateModel>> AddGenreAsync(GenreCreateModel genreCreateModel)
        {
            try
            {
                if (genreCreateModel == null)
                    throw new ArgumentNullException($"The genre create model wasn't valid: {genreCreateModel?.Name}");

                var genreModelValues = JsonSerializer.Serialize(genreCreateModel);

                if (genreModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the genreCreateModel to json.");

                HttpContent httpContent = new StringContent(genreModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("genres", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($"Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult<GenreCreateModel>(response.StatusCode, genreCreateModel, $"Successfully added the genre: {genreCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the genre: {genreCreateModel.Name}, ", ex.Message);
                return new ServiceResult<GenreCreateModel>(HttpStatusCode.Unused, genreCreateModel, $"Failed to add the genre: {genreCreateModel.Name}");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a genre./>
        /// </summary>
        /// <param name="genreId">The Id of the genre who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteGenreAsync(Guid genreId)
        {
            try
            {
                if (genreId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The genre Id wasn't a valid Id: {genreId}");

                HttpResponseMessage response = await Client.DeleteAsync($"genres/{genreId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($"Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the genre: {genreId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the genre: {genreId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the genre: {genreId}");
            }
        }
    }
}
