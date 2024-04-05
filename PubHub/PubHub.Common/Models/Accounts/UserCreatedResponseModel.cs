using PubHub.Common.Models.Users;

namespace PubHub.Common.Models.Accounts
{
    public class UserCreatedResponseModel
    {
        public required UserInfoModel UserInfo { get; init; }
        public required TokenResponseModel TokenResponseModel { get; init; }
    }
}
