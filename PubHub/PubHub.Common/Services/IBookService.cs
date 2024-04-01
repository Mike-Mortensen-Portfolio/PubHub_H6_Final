using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<List<BookInfoModel>> GetBooks(BookQuery queryOptions);
        Task<BookInfoModel?> GetBook(Guid bookId);
        Task<ServiceInstanceResult<BookCreateModel>> AddBook(BookCreateModel bookCreateModel);
        Task<ServiceInstanceResult<BookUpdateModel>> UpdateBook(Guid bookId, BookUpdateModel bookUpdateModel);
        Task<ServiceResult> DeleteUser(Guid bookId);
    }
}
