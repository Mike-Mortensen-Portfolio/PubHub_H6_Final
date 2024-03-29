using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public class PublisherService : ServiceRoot, IPublisherService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal PublisherService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="PublisherCreateModel"/> to the database.
        /// </summary>
        /// <param name="publisherCreateModel">The <see cref="PublisherCreateModel"/> holding the new publisher information.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<PublisherCreateModel>> AddPublisher(PublisherCreateModel publisherCreateModel)
        {
            try
            {
                if (publisherCreateModel == null)
                    throw new ArgumentNullException($"The User create model wasn't valid: {publisherCreateModel?.Name}");

                var publisherModelValues = JsonSerializer.Serialize(publisherCreateModel);

                if (publisherModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the publisherCreateModel to json.");

                HttpContent httpContent = new StringContent(publisherModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("publishers", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<PublisherCreateModel>(response.StatusCode, publisherCreateModel, $"Successfully added the publisher: {publisherCreateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the publisher: {publisherCreateModel.Name}, ", ex.Message);
                return new ServiceInstanceResult<PublisherCreateModel>(HttpStatusCode.Unused, publisherCreateModel, $"Failed to add the publisher: {publisherCreateModel.Name}");
            }
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all of a publisher's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="publisherId">The Id of the publisher who's books needs retrieval.</param>
        /// <returns>Returns a list of <see cref="BookInfoModel"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<List<BookInfoModel>> GetPublisherBooks(int publisherId)
        {
            try
            {
                if (publisherId <= 0)
                    throw new ArgumentOutOfRangeException($"The publisher Id wasn't a valid Id: {publisherId}");

                HttpResponseMessage response = await Client.GetAsync($"publishers/{publisherId}/books");
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
                Debug.WriteLine($"Unable to get publisher's books: {publisherId}", ex.Message);
                return null!;
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="PublisherInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="publisherId">Id of the publisher wanting information about.</param>
        /// <returns>A <see cref="PublisherInfoModel"/> with the users information.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<PublisherInfoModel?> GetPublisherInfo(int publisherId)
        {
            try
            {
                if (publisherId <= 0)
                    throw new ArgumentOutOfRangeException($"The publisher Id wasn't a valid Id: {publisherId}");

                HttpResponseMessage response = await Client.GetAsync($"publishers/{publisherId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                PublisherInfoModel? publisherInfoModel = JsonSerializer.Deserialize<PublisherInfoModel>(content, _serializerOptions);
                if (publisherInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return publisherInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get publisher info failed: {publisherId}", ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Calls the API endpoint for updating <see cref="PublisherUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="publisherId">The id of the publisher being updated.</param>
        /// <param name="publisherUpdateModel">The <see cref="PublisherUpdateModel"/> holding the updated values.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<PublisherUpdateModel>> UpdatePublisher(int publisherId, PublisherUpdateModel publisherUpdateModel)
        {
            try
            {
                if (publisherId <= 0)
                    throw new ArgumentOutOfRangeException($"The publisher Id wasn't a valid Id: {publisherId}");

                if (publisherUpdateModel == null)
                    throw new ArgumentNullException($"The publisher update model wasn't valid: {publisherUpdateModel?.Name}");

                var publisherModelValues = JsonSerializer.Serialize(publisherUpdateModel);

                if (publisherModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpContent httpContent = new StringContent(publisherModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PutAsync($"publishers/{publisherId}", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<PublisherUpdateModel>(response.StatusCode, publisherUpdateModel, $"Successfully updated the publisher: {publisherUpdateModel.Name}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the publisher: {publisherUpdateModel.Name}, ", ex.Message);
                return new ServiceInstanceResult<PublisherUpdateModel>(HttpStatusCode.Unused, publisherUpdateModel, $"Failed to update the publisher: {publisherUpdateModel.Name}");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a user.
        /// </summary>
        /// <param name="publisherId">The Id of the publisher who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> on how the request was handled.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeletePublisher(int publisherId)
        {
            try
            {
                if (publisherId <= 0)
                    throw new ArgumentOutOfRangeException($"The publisher Id wasn't a valid Id: {publisherId}");

                HttpResponseMessage response = await Client.DeleteAsync($"publishers/{publisherId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the publisher: {publisherId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the publisher: {publisherId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the publisher: {publisherId}");
            }
        }
    }
}
