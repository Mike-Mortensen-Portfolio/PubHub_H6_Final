using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Models.Books;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class BookService : ServiceRoot, IBookService
    {
#pragma warning disable IDE0270 // Use coalesce expression
        private readonly JsonSerializerOptions _serializerOptions;

        internal BookService(IHttpClientService clientService, string clientName) : base(clientService, clientName)
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
        }

        /// <summary>
        /// Calls the API enpoint to retrieve all books through the <see cref="BookInfoModel"/> filtered on the searchQuery.
        /// </summary>
        /// <param name="queryOptions">The query options that is requested.</param>
        /// <returns>A list of <see cref="BookInfoModel"/></returns>
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
                return [];
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="BookInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="bookId">Id of the book we want information about.</param>
        /// <returns>A <see cref="BookInfoModel"/> with a book's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<BookInfoModel?> GetBook(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The book Id wasn't a valid Id: {bookId}");

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

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="BookCreateModel"/> to the database.
        /// </summary>
        /// <param name="bookCreateModel">The <see cref="BookCreateModel"/> holding the new book.</param>
        /// <returns>A status telling if a book was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<BookCreateModel>> AddBook(BookCreateModel bookCreateModel)
        {
            try
            {
                if (bookCreateModel == null)
                    throw new ArgumentNullException($"The book create model wasn't valid: {bookCreateModel?.Title}");

                var bookModelValues = JsonSerializer.Serialize(bookCreateModel);

                if (bookModelValues == null)
                    throw new NullReferenceException($"Unable to serialize the bookCreateModel to json.");

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

        /// <summary>
        /// Calls the API endpoint for updating <see cref="BookUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="bookId">The id of the book being updated.</param>
        /// <param name="bookUpdateModel">The <see cref="BookUpdateModel"/> holding the updated values.</param>
        /// <returns>A status telling if a book was successfully updated in the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceInstanceResult<BookUpdateModel>> UpdateBook(Guid bookId, BookUpdateModel bookUpdateModel)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The user Id wasn't a valid Id: {bookId}");

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

        /// <summary>
        /// Calls the API endpoint to soft-delete a book./>
        /// </summary>
        /// <param name="bookId">The Id of the book who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteUser(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new ArgumentException($"The book Id wasn't a valid Id: {bookId}");

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
