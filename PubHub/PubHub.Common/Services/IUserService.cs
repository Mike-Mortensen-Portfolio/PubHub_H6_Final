using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<ServiceResult<UserInfoModel>> GetUserAsync(Guid userId);
        Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetUserBooksAsync(Guid userId, BookQuery queryOptions = null!);
        Task<ServiceResult<UserBookContentModel>> GetUserBookContentAsync(Guid userId, Guid bookId);
        Task<ServiceResult<UserInfoModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel);
        Task<ServiceResult> DeleteUserAsync(Guid userId);
        Task<ServiceResult> SuspendUserAsync(Guid userId);
    }
}
