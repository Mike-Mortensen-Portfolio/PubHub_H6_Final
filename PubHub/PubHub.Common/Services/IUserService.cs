using PubHub.Common.Models.Books;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Calls the API endpoint for adding a <see cref="UserCreateModel"/> to the database.
        /// </summary>
        /// <param name="userCreateModel">The <see cref="UserCreateModel"/> holding the new user.</param>
        /// <returns>A status telling if a user was successfully added to the database.</returns>
        Task<ServiceInstanceResult<UserCreateModel>> AddUser(UserCreateModel userCreateModel);
        /// <summary>
        /// Calls the API end point for retrieving <see cref="UserInfoModel">, to use in the client applications.
        /// </summary>
        /// <param name="userId">Id of the user wanting information about.</param>
        /// <returns></returns>
        Task<UserInfoModel?> GetUserInfo(int userId);
        /// <summary>
        /// Calls the API enpoint to retrieve all of a user's books through the <see cref="BookInfoModel"/>
        /// </summary>
        /// <param name="userId">The Id of the user who's books needs retrieval.</param>
        /// <returns>Returns a list of <see cref="BookInfoModel"/></returns>
        Task<List<BookInfoModel>> GetUserBooks(int userId);
        /// <summary>
        /// Calls the API endpoint for updating <see cref="UserUpdateModel"/> values in the database.
        /// </summary>
        /// <param name="userId">The id of the user being updated.</param>
        /// <param name="userUpdateModel">The <see cref="UserUpdateModel"/> holding the updated values.</param>
        /// <returns>A status telling if a user was successfully updated in the database.</returns>
        Task<ServiceInstanceResult<UserUpdateModel>> UpdateUser(int userId, UserUpdateModel userUpdateModel);
        /// <summary>
        /// Calls the API endpoint to soft-delete a user./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to be soft-deleted.</param>
        /// <returns></returns>
        Task<ServiceResult> DeleteUser(int userId);
        /// <summary>
        /// Calls the API endpoint to change the user's account type to suspended./>
        /// </summary>
        /// <param name="userId">The Id of the user who needs to have their account type as suspended.</param>
        /// <returns></returns>
        Task<ServiceResult> SuspendUser(int userId);
    }
}
