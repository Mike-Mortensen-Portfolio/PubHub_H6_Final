using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Models.Accounts;
using PubHub.Common.Models.Authentication;

namespace PubHub.Common.Services
{
    public interface IAuthenticationService
    {
        Task<ServiceInstanceResult<TokenResponseModel>> LoginAsync(LoginInfo loginInfo);
        Task<ServiceInstanceResult<TokenResponseModel>> RefreshTokenAsync(TokenInfo tokenInfo);
        Task<ServiceResult> RevokeTokenAsync(TokenInfo tokenInfo);
    }
}
