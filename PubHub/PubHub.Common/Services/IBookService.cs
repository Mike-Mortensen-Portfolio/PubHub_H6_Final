using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<IReadOnlyList<BookInfoModel>> GetBooksAsync(BookQuery queryOptions);
        Task<ServiceResult<BookInfoModel>> GetBookAsync(Guid bookId);
        Task<ServiceResult<BookInfoModel>> AddBookAsync(BookCreateModel bookCreateModel);
        Task<ServiceResult<BookInfoModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel);
        Task<ServiceResult> DeleteBookAsync(Guid bookId);
    }
}
