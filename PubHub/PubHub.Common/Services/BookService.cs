using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using PubHub.Common.ApiService;
using PubHub.Common.Extensions;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Books;
using static PubHub.Common.IntegrityConstants;

namespace PubHub.Common.Services
{
    public class BookService : ServiceRoot, IBookService
    {
        private readonly JsonSerializerOptions _serializerOptions;

        public BookService(IHttpClientService clientService) : base(clientService)
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
        public async Task<HttpServiceResult<IReadOnlyList<BookInfoModel>>> GetAllBooksAsync(BookQuery queryOptions)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(queryOptions);

                HttpResponseMessage response = await Client.GetAsync($"books?{queryOptions.ToQuery(ignoreNull: true)}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<List<BookInfoModel>>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, bookInfoModel, "Successfully retrieved the books!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get user books:", ex.Message);
                return new HttpServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.Unused, null, $"Unable to get user's books: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="BookInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="bookId">Id of the book we want information about.</param>
        /// <returns>A <see cref="BookInfoModel"/> with a book's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<BookInfoModel>> GetBookAsync(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully retrieved the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get book info failed:", ex.Message);
                return new HttpServiceResult<BookInfoModel>(HttpStatusCode.Unused, null, $"Failed to get the book: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="BookCreateModel"/> to the database.
        /// </summary>
        /// <param name="bookCreateModel">The <see cref="BookCreateModel"/> holding the new book.</param>
        /// <returns>A status telling if a book was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult<BookInfoModel>> AddBookAsync(BookCreateModel bookCreateModel)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(bookCreateModel);

                var bookModelValues = JsonSerializer.Serialize(bookCreateModel) ?? throw new NullReferenceException($"Unable to serialize the bookCreateModel to json.");

                HttpResponseMessage response = await Client.PostAsync("books", bookModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new HttpServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully added the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the book: {bookCreateModel.Title}, ", ex.Message);
                return new HttpServiceResult<BookInfoModel>(HttpStatusCode.Unused, null, $"Failed to add the book: {ex.Message}.");
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
        public async Task<HttpServiceResult<BookInfoModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The user Id wasn't a valid Id.");

                ArgumentNullException.ThrowIfNull(bookUpdateModel);

                var bookModelValues = JsonSerializer.Serialize(bookUpdateModel) ?? throw new NullReferenceException($"Unable to serialize the userUpdateModel to json.");

                HttpResponseMessage response = await Client.PutAsync($"books/{bookId}", bookModelValues);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new HttpServiceResult<BookInfoModel>(response.StatusCode, null, "Unable to map the request over to the client.");

                return new HttpServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully updated the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the book: {bookUpdateModel.Title}, ", ex.Message);
                return new HttpServiceResult<BookInfoModel>(HttpStatusCode.Unused, null, $"Failed to update the book: {ex.Message}.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a book./>
        /// </summary>
        /// <param name="bookId">The Id of the book who needs to be soft-deleted.</param>
        /// <returns>A <see cref="HttpServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<HttpServiceResult> DeleteBookAsync(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new HttpServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new HttpServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new HttpServiceResult(response.StatusCode, $"Successfully deleted the book: {bookId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to delete the book: {bookId}, ", ex.Message);
                return new HttpServiceResult(HttpStatusCode.Unused, $"Failed to delete the book: {ex.Message}");
            }
        }

        /// <summary>
        /// Simulate a purchase of the book with the given <paramref name="bookId"/> through the API
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This doesn't perform any payment process and simply create the necessary installments to simulate a purchase.
        /// </summary>
        /// <param name="bookCreateModel">The <see cref="BookCreateModel"/> holding the new book.</param>
        /// <returns>A <see cref="Task"/> that represents the <see langword="asynchronous"/> operation, yielding a <see cref="ServiceResult"/> that defines the outcome of the API request</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> PurchaseBookAsync(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    throw new NullReferenceException($"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.PostAsync($"books/{bookId}/purchase");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                return new ServiceResult(response.StatusCode, $"Purchase complete");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get book info failed:", ex.Message);
                return new ServiceResult(HttpStatusCode.Unused, $"Failed to purchase book: {ex.Message}.");
            }
        }
    }
}
