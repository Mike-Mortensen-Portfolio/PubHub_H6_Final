using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public class BookService : ServiceRoot, IBookService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal BookService(IHttpClientFactory clientFactory, string clientName) : base(clientFactory, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        // TODO (JBN): Change to GUIDs instead of int when that has been updated.

        public async Task<List<BookInfoModel>> GetBooks(BookQuery queryOptions)
        {
            try
            {
                if (queryOptions == null)
                    throw new NullReferenceException($"The search query wasn't valid: {queryOptions}");

                HttpResponseMessage response = await Client.GetAsync($"books");
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
                Debug.WriteLine("Unable to get user books:", ex.Message);
                return null!;
            }
        }

        public async Task<BookInfoModel?> GetBook(int bookId)
        {
            try
            {
                if (bookId <= 0)
                    throw new ArgumentOutOfRangeException($"The book Id wasn't a valid Id: {bookId}");

                HttpResponseMessage response = await Client.GetAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                BookInfoModel? bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    throw new NullReferenceException($"Unable to map the request over to the client.");

                return bookInfoModel!;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get book info failed:", ex.Message);
                return null;
            }
        }

        public async Task<ServiceInstanceResult<BookCreateModel>> AddBook(BookCreateModel bookCreateModel)
        {
            try
            {
                if (bookCreateModel == null)
                    throw new ArgumentNullException($"The book create model wasn't valid: {bookCreateModel?.Title}");

                var bookModelValues = JsonSerializer.Serialize(bookCreateModel);

                if (bookModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userCreateModel to json.");

                HttpContent httpContent = new StringContent(bookModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("books", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<BookCreateModel>(response.StatusCode, bookCreateModel, $"Successfully added the book: {bookCreateModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the book: {bookCreateModel.Title}, ", ex.Message);
                return new ServiceInstanceResult<BookCreateModel>(HttpStatusCode.Unused, bookCreateModel, $"Failed to add the book: {bookCreateModel.Title}");
            }
        }

        public async Task<ServiceInstanceResult<BookUpdateModel>> UpdateBook(int bookId, BookUpdateModel bookUpdateModel)
        {
            try
            {
                if (bookId <= 0)
                    throw new ArgumentOutOfRangeException($"The user Id wasn't a valid Id: {bookId}");

                if (bookUpdateModel == null)
                    throw new ArgumentNullException($"The User update model wasn't valid: {bookUpdateModel?.Title}");

                var bookModelValues = JsonSerializer.Serialize(bookUpdateModel);

                if (bookModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpContent httpContent = new StringContent(bookModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PutAsync($"books/{bookId}", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceInstanceResult<BookUpdateModel>(response.StatusCode, bookUpdateModel, $"Successfully updated the book: {bookUpdateModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the book: {bookUpdateModel.Title}, ", ex.Message);
                return new ServiceInstanceResult<BookUpdateModel>(HttpStatusCode.Unused, bookUpdateModel, $"Failed to update the book: {bookUpdateModel.Title}");
            }
        }

        public async Task<ServiceResult> DeleteUser(int bookId)
        {
            try
            {
                if (bookId <= 0)
                    throw new ArgumentOutOfRangeException($"The book Id wasn't a valid Id: {bookId}");

                HttpResponseMessage response = await Client.DeleteAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        throw new NullReferenceException($"Unable to handle the Error response, status code: {response.StatusCode}");

                    throw new Exception($"Unable to retrieve information: {errorResponse!.Detail}");
                }

                return new ServiceResult(response.StatusCode, $"Successfully deleted the book: {bookId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the book: {bookId}, ", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to delete the book: {bookId}");
            }
        }
    }
}
