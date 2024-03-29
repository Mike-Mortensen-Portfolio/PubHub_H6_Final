using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<ServiceInstanceResult<UserCreateModel>> AddUser(UserCreateModel userCreateModel);
        Task<UserInfoModel?> GetUserInfo(int userId);
        Task<List<BookInfoModel>> GetUserBooks(int userId);
        Task<ServiceInstanceResult<UserUpdateModel>> UpdateUser(int userId, UserUpdateModel userUpdateModel);
        Task<ServiceResult> DeleteUser(int userId);
        Task<ServiceResult> SuspendUser(int userId);
    }
}
