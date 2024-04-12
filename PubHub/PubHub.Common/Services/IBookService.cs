using Microsoft.AspNetCore.Mvc;
using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetAllBooksAsync(BookQuery queryOptions);
        Task<ServiceResult<BookInfoModel>> GetBookAsync(Guid bookId);
        Task<ServiceResult<BookInfoModel>> AddBookAsync(BookCreateModel bookCreateModel);
        Task<ServiceResult<BookInfoModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel);
        Task<ServiceResult> PurchaseBookAsync(Guid id);
        Task<ServiceResult> DeleteBookAsync(Guid bookId);
    }
}
