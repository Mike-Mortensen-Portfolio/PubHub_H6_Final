using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class PublisherService : ServiceRoot, IPublisherService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public PublisherService(IHttpClientService clientService) : base(clientService)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all publishers through the <see cref="PublisherInfoModel"/> filtered on the searchQuery.
        /// </summary>
        /// <param name="queryOptions">The query options that is requested.</param>
        /// <returns>A list of <see cref="PublisherInfoModel"/></returns>
        public async Task<ServiceResult<IReadOnlyList<PublisherInfoModel>>> GetAllPublishersAsync(PublisherQuery queryOptions)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(queryOptions);

                HttpResponseMessage response = await Client.GetAsync($"publishers?{queryOptions.ToQuery()}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<IReadOnlyList<PublisherInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<IReadOnlyList<PublisherInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                List<PublisherInfoModel>? publisherInfoModel = JsonSerializer.Deserialize<List<PublisherInfoModel>>(content, _serializerOptions);

                if (publisherInfoModel == null)
                    return new ServiceResult<IReadOnlyList<PublisherInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<IReadOnlyList<PublisherInfoModel>>(response.StatusCode, publisherInfoModel, "Successfully retrieved all publishers.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get publishers:", ex.Message);
                return new ServiceResult<IReadOnlyList<PublisherInfoModel>>(HttpStatusCode.Unused, null, $"Unable retrieve all publishers: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="PublisherCreateModel"/> to the database.
        /// </summary>
        /// <param name="publisherCreateModel">The <see cref="PublisherCreateModel"/> holding the new publisher information.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<PublisherInfoModel>> AddPublisherAsync(PublisherCreateModel publisherCreateModel)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(publisherCreateModel);

                var publisherModelValues = JsonSerializer.Serialize(publisherCreateModel) ?? throw new NullReferenceException($"Unable to serialize the publisherCreateModel to json.");

                HttpResponseMessage response = await Client.PostAsync("publishers", publisherModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var publisherInfoModel = JsonSerializer.Deserialize<PublisherInfoModel>(content, _serializerOptions);
                if (publisherInfoModel == null)
                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<PublisherInfoModel>(response.StatusCode, publisherInfoModel, $"Successfully added the publisher: {publisherInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the publisher: {publisherCreateModel.Name}, ", ex.Message);
                return new ServiceResult<PublisherInfoModel>(HttpStatusCode.Unused, null, $"Failed to add the publisher: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all of a publisher's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="publisherId">The Id of the publisher who's books needs retrieval.</param>
        /// <returns>A list of <see cref="BookInfoModel"/></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetAllPublisherBooksAsync(Guid publisherId)
        {
            try
            {
                if (publisherId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The publisher Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"publishers/{publisherId}/books");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<List<BookInfoModel>>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, bookInfoModel, "Successfully retrieved publisher's books.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get publisher's books: {publisherId}", ex.Message);
                return new ServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.Unused, null, $"Unable to get publisher's books: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to retrieve the content of a specific book.
        /// </summary>
        /// <param name="publisherId">Id of the Publisher trying to access the content.</param>
        /// <param name="bookId">Id of the book where content is requested.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<BookContentModel>> GetPublisherBookContentAsync(Guid publisherId, Guid bookId)
        {
            try
            {
                if (publisherId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The publisher Id wasn't a valid Id.");

                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"publishers/{publisherId}/books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<BookContentModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<BookContentModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookContentModel = JsonSerializer.Deserialize<BookContentModel>(content, _serializerOptions);
                if (bookContentModel == null)
                    return new ServiceResult<BookContentModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<BookContentModel>(response.StatusCode, bookContentModel, $"Successfully retrieved the content of the book.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get book content failed: {publisherId}", ex.Message);
                return new ServiceResult<BookContentModel>(HttpStatusCode.Unused, null, $"Failed to retrieve content of the book: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="PublisherInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="publisherId">Id of the publisher wanting information about.</param>
        /// <returns>A <see cref="PublisherInfoModel"/> with the users information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<PublisherInfoModel>> GetPublisherInfoAsync(Guid publisherId)
        {
            try
            {
                if (publisherId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The publisher Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"publishers/{publisherId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var publisherInfoModel = JsonSerializer.Deserialize<PublisherInfoModel>(content, _serializerOptions);
                if (publisherInfoModel == null)
                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<PublisherInfoModel>(response.StatusCode, publisherInfoModel, $"Successfully retrieved info of the publisher: {publisherInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get publisher info failed: {publisherId}", ex.Message);
                return new ServiceResult<PublisherInfoModel>(HttpStatusCode.Unused, null, $"Failed to retrieve info of the publisher: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for updating <see cref="PublisherUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="publisherId">The id of the publisher being updated.</param>
        /// <param name="publisherUpdateModel">The <see cref="PublisherUpdateModel"/> holding the updated values.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<PublisherInfoModel>> UpdatePublisherAsync(Guid publisherId, PublisherUpdateModel publisherUpdateModel)
        {
            try
            {
                if (publisherId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The publisher Id wasn't a valid Id.");

                ArgumentNullException.ThrowIfNull(publisherUpdateModel);

                var publisherModelValues = JsonSerializer.Serialize(publisherUpdateModel) ?? throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpResponseMessage response = await Client.PutAsync($"publishers/{publisherId}", publisherModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var publisherInfoModel = JsonSerializer.Deserialize<PublisherInfoModel>(content, _serializerOptions);
                if (publisherInfoModel == null)
                    return new ServiceResult<PublisherInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<PublisherInfoModel>(response.StatusCode, publisherInfoModel, $"Successfully updated the publisher: {publisherInfoModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the publisher: {publisherUpdateModel.Name}, ", ex.Message);
                return new ServiceResult<PublisherInfoModel>(HttpStatusCode.Unused, null, $"Failed to update the publisher: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a user.
        /// </summary>
        /// <param name="publisherId">The Id of the publisher who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeletePublisherAsync(Guid publisherId)
        {
            try
            {
                if (publisherId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The publisher Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"publishers/{publisherId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the publisher: {publisherId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the publisher: {publisherId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the publisher: {ex.Message}.");
            }
        }
    }
}
