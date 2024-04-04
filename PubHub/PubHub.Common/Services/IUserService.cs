using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<ServiceInstanceResult<UserCreateModel>> AddUserAsync(UserCreateModel userCreateModel);
        Task<UserInfoModel?> GetUserInfoAsync(Guid userId);
        Task<List<BookInfoModel>> GetUserBooksAsync(Guid userId);
        Task<ServiceInstanceResult<UserUpdateModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel);
        Task<ServiceResult> DeleteUserAsync(Guid userId);
        Task<ServiceResult> SuspendUserAsync(Guid userId);
    }
}
