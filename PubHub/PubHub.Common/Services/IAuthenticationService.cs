using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<UserCreatedResponseModel>> RegisterUserAsync(UserCreateModel userCreateModel);
        Task<ServiceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo);
        Task<ServiceResult<TokenResponseModel>> RefreshTokenAsync(TokenInfo tokenInfo);
        Task<ServiceResult> RevokeTokenAsync(TokenInfo tokenInfo);
    }
}
