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
        public async Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetBooksAsync(BookQuery queryOptions)
        {
            try
            {
                if (queryOptions == null)
                    return new ServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.InternalServerError, null, $"The search query wasn't valid.");

                HttpResponseMessage response = await Client.GetAsync($"books?{queryOptions.ToQuery(ignoreNull: true)}");
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

                return new ServiceResult<IReadOnlyList<BookInfoModel>>(response.StatusCode, bookInfoModel, "Successfully retrieved the books!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to get user books:", ex.Message);
                return new ServiceResult<IReadOnlyList<BookInfoModel>>(HttpStatusCode.InternalServerError, null, "Unable to get user's books.");
            }
        }

        /// <summary>
        /// Calls the API end point for retrieving <see cref="BookInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="bookId">Id of the book we want information about.</param>
        /// <returns>A <see cref="BookInfoModel"/> with a book's information.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<BookInfoModel>> GetBookAsync(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.GetAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully retrieved the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Get book info failed:", ex.Message);
                return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"Failed to get the book.");
            }
        }

        /// <summary>
        /// Calls the API endpoint for adding a <see cref="BookCreateModel"/> to the database.
        /// </summary>
        /// <param name="bookCreateModel">The <see cref="BookCreateModel"/> holding the new book.</param>
        /// <returns>A status telling if a book was successfully added to the database.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult<BookInfoModel>> AddBookAsync(BookCreateModel bookCreateModel)
        {
            try
            {
                if (bookCreateModel == null)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"The book create model wasn't valid.");

                var bookModelValues = JsonSerializer.Serialize(bookCreateModel);

                if (bookModelValues == null)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"Unable to serialize the bookCreateModel to json.");

                HttpContent httpContent = new StringContent(bookModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PostAsync("books", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to map the request over to the client.");

                return new ServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully added the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add the book: {bookCreateModel.Title}, ", ex.Message);
                return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"Failed to add the book.");
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
        public async Task<ServiceResult<BookInfoModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"The user Id wasn't a valid Id.");

                if (bookUpdateModel == null)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"The User update model wasn't valid.");

                var bookModelValues = JsonSerializer.Serialize(bookUpdateModel);

                if (bookModelValues == null)
                    return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"Unable to serialize the userUpdateModel to json.");

                HttpContent httpContent = new StringContent(bookModelValues.ToString(), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await Client.PutAsync($"books/{bookId}", httpContent);
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
                }

                var bookInfoModel = JsonSerializer.Deserialize<BookInfoModel>(content, _serializerOptions);
                if (bookInfoModel == null)
                    return new ServiceResult<BookInfoModel>(response.StatusCode, null, "Unable to map the request over to the client.");

                return new ServiceResult<BookInfoModel>(response.StatusCode, bookInfoModel, $"Successfully updated the book: {bookInfoModel.Title}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update the book: {bookUpdateModel.Title}, ", ex.Message);
                return new ServiceResult<BookInfoModel>(HttpStatusCode.InternalServerError, null, $"Failed to update the book.");
            }
        }

        /// <summary>
        /// Calls the API endpoint to soft-delete a book./>
        /// </summary>
        /// <param name="bookId">The Id of the book who needs to be soft-deleted.</param>
        /// <returns>A <see cref="ServiceResult"/> telling if the request was successful.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        public async Task<ServiceResult> DeleteBookAsync(Guid bookId)
        {
            try
            {
                if (bookId == INVALID_ENTITY_ID)
                    return new ServiceResult(HttpStatusCode.InternalServerError, $"The book Id wasn't a valid Id.");

                HttpResponseMessage response = await Client.DeleteAsync($"books/{bookId}");
                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ErrorResponse? errorResponse = JsonSerializer.Deserialize<ErrorResponse>(content, _serializerOptions);
                    if (errorResponse == null)
                        return new ServiceResult(response.StatusCode, $"Unable to handle the Error response, status code: {response.StatusCode}");

                    return new ServiceResult(response.StatusCode, $"Unable to retrieve information: {errorResponse.Title}{((errorResponse.Detail != null) ? ($" Details: {errorResponse.Detail}") : (string.Empty))}");
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
