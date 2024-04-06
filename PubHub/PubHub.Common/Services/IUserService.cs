﻿using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        Task<ServiceResult<UserInfoModel>> GetUserAsync(Guid userId);
        Task<IReadOnlyList<BookInfoModel>> GetUserBooksAsync(Guid userId);
        Task<ServiceResult<UserInfoModel>> UpdateUserAsync(Guid userId, UserUpdateModel userUpdateModel);
        Task<ServiceResult> DeleteUserAsync(Guid userId);
        Task<ServiceResult> SuspendUserAsync(Guid userId);
    }
}
