using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<ServiceInstanceResult<UserCreateModel>> AddUser(UserCreateModel userCreateModel);
        Task<UserInfoModel?> GetUserInfo(Guid userId);
        Task<List<BookInfoModel>> GetUserBooks(Guid userId);
        Task<ServiceInstanceResult<UserUpdateModel>> UpdateUser(Guid userId, UserUpdateModel userUpdateModel);
        Task<ServiceResult> DeleteUser(Guid userId);
        Task<ServiceResult> SuspendUser(Guid userId);
    }
}
