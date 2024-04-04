using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Authors;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class AuthorService : ServiceRoot, IAuthorService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        public AuthorService(IHttpClientService clientService) : base(clientService)
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
        public async Task<List<AuthorInfoModel>> GetAuthors()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"authors");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                List<AuthorInfoModel>? authorInfoModel = JsonSerializer.Deserialize<List<AuthorInfoModel>>(content, _serializerOptions);

                if (authorInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return authorInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get authors:", ex.Message);
                return [];
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="AuthorInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="authorId">Id of the author we want information about.</param>
        /// <returns>A <see cref="AuthorInfoModel"/> with an author's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<AuthorInfoModel?> GetAuthor(Guid authorId)
        {
            try
            {
                if (authorId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The author Id wasn't a valid Id: {authorId}");

                HttpResponseMessage response = await Client.GetAsync($"authors/{authorId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                AuthorInfoModel? authorInfoModel = JsonSerializer.Deserialize<AuthorInfoModel>(content, _serializerOptions);
                if (authorInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return authorInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get author info failed:", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="AuthorCreateModel"/> to the database.
        /// </summary>
        /// <param name="authorCreateModel">The <see cref="AuthorCreateModel"/> holding the new author.</param>
        /// <returns>A status telling if an author was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<AuthorInfoModel>> AddAuthor(AuthorCreateModel authorCreateModel)
        {
            try
            {
                if (authorCreateModel == null)
                    throw new ArgumentNullException($"The author create model wasn't valid: {authorCreateModel?.Name}");

                var authorModelValues = JsonSerializer.Serialize(authorCreateModel);

                if (authorModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the authorCreateModel to json.");

                HttpContent httpContent = new StringContent(authorModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("authors", httpContent);
                string content = await response.Content.ReadAsStringAsync();


                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                var authorInfo = JsonSerializer.Deserialize<AuthorInfoModel>(content, _serializerOptions);
                if (authorInfo == null)
                    throw new NullReferenceException($"Unable to handle the author model, status code: {response.StatusCode}");

                return new ServiceInstanceResult<AuthorInfoModel>(response.StatusCode, authorInfo, $"Successfully added the author: {authorCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the author: {authorCreateModel.Name}, ", ex.Message);
                return new ServiceInstanceResult<AuthorInfoModel>(HttpStatusCode.Unused, null, $"Failed to add the author: {authorCreateModel.Name}");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete an author./>
        /// </summary>
        /// <param name="authorId">The Id of the author who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteAuthor(Guid authorId)
        {
            try
            {
                if (authorId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The author Id wasn't a valid Id: {authorId}");

                HttpResponseMessage response = await Client.DeleteAsync($"authors/{authorId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the author: {authorId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the author: {authorId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the author: {authorId}");
            }
        }
    }
}
