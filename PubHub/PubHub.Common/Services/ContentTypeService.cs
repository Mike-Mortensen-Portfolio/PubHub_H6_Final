using System.Diagnostics;
using System.Net;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.Common.Services
{
    public class ContentTypeService : ServiceRoot, IContentTypeService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public ContentTypeService(IHttpClientService clientService) : base(clientService)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all content types through the <see cref="ContentTypeInfoModel"/>.
        /// </summary>
        /// <param name="queryOptions">The query options that is requested.</param>
        /// <returns>A list of <see cref="ContentTypeInfoModel"/></returns>
        public async Task<HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>> GetAllContentTypesAsync()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"contentTypes");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var contentTypeInfoModel = JsonSerializer.Deserialize<List<ContentTypeInfoModel>>(content, _serializerOptions);

                if (contentTypeInfoModel == null)
                    return new HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>(response.StatusCode, contentTypeInfoModel, "Successfully retrieved all content types!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get content types:", ex.Message);
                return new HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>(HttpStatusCode.Unused, null, $"Unable to retrieve all content types: {ex.Message}.");
            }
        }
    }
}
