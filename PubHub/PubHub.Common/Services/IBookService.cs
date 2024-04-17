using Microsoft.AspNetCore.Mvc;
using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<HttpServiceResult<IReadOnlyList<BookInfoModel>>> GetAllBooksAsync(BookQuery queryOptions);
        Task<HttpServiceResult<BookInfoModel>> GetBookAsync(Guid bookId);
        Task<HttpServiceResult<Stream>> GetBookStreamAsync(Guid bookId);
        Task<HttpServiceResult<BookInfoModel>> AddBookAsync(BookCreateModel bookCreateModel);
        Task<HttpServiceResult<BookInfoModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel);
        Task<HttpServiceResult> DeleteBookAsync(Guid bookId);
        Task<HttpServiceResult> PurchaseBookAsync(Guid id);
    }
}
