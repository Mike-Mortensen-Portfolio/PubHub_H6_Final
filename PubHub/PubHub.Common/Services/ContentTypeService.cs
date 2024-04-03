using System.Diagnostics;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.Common.Services
{
    public class ContentTypeService : ServiceRoot, IContentTypeService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        public ContentTypeService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
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
        public async Task<List<ContentTypeInfoModel>> GetContentTypes()
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync($"contentTypes");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                List<ContentTypeInfoModel>? contentTypeInfoModel = JsonSerializer.Deserialize<List<ContentTypeInfoModel>>(content, _serializerOptions);

                if (contentTypeInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return contentTypeInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get content types:", ex.Message);
                return [];
            }
        }
    }
}
