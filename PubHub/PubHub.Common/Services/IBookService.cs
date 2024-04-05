using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<List<BookInfoModel>> GetBooksAsync(BookQuery queryOptions);
        Task<BookInfoModel?> GetBookAsync(Guid bookId);
        Task<ServiceInstanceResult<BookCreateModel>> AddBookAsync(BookCreateModel bookCreateModel);
        Task<ServiceInstanceResult<BookUpdateModel>> UpdateBookAsync(Guid bookId, BookUpdateModel bookUpdateModel);
        Task<ServiceResult> DeleteBookAsync(Guid bookId);
    }
}
