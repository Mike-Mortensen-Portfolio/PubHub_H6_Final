using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IAuthenticationService
    {
        Task<HttpServiceResult<UserCreatedResponseModel>> RegisterUserAsync(UserCreateModel userCreateModel);
        Task<HttpServiceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo);
        Task<HttpServiceResult<TokenResponseModel>> RefreshTokenAsync();
        Task<HttpServiceResult> RevokeTokenAsync();
    }
}
