using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<HttpServiceResult<UserInfoModel>> GetUserAsync(Guid userId);
        Task<HttpServiceResult<IReadOnlyList<BookInfoModel>>> GetUserBooksAsync(Guid userId, BookQuery? queryOptions = null);
        Task<HttpServiceResult<UserBookContentModel>> GetUserBookContentAsync(Guid userId, Guid bookId);
        Task<HttpServiceResult<UserInfoModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel);
        Task<HttpServiceResult> DeleteUserAsync(Guid userId);
        Task<HttpServiceResult> SuspendUserAsync(Guid userId);
    }
}
