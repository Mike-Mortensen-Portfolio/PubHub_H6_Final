using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Users
{
    public class UserUpdateModel(UserInfoModel user)
    {
        public required string Name { get; set; } = user.Name;
        public required string Surname { get; set; } = user.Surname;
        public required DateOnly Birthday { get; set; } = user.Birthday;
        public required AccountUpdateModel Account { get; set; } = new()
        {
            Email = user.Email
        };
    }
}
