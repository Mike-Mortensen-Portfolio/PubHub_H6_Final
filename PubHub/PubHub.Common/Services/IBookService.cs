using PubHub.Common.Models.Books;

namespace PubHub.Common.Services
{
    public interface IBookService
    {
        Task<List<BookInfoModel>> GetBooks(BookQuery queryOptions);
        Task<BookInfoModel?> GetBook(int bookId);
        Task<ServiceInstanceResult<BookCreateModel>> AddBook(BookCreateModel bookCreateModel);
        Task<ServiceInstanceResult<BookUpdateModel>> UpdateBook(int bookId, BookUpdateModel bookUpdateModel);
        Task<ServiceResult> DeleteUser(int bookId);
    }
}
