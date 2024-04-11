using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Users
{
    public class UserUpdateModel
    {
        public UserUpdateModel() { /*Empty*/ }
        public UserUpdateModel(UserInfoModel user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Birthday = user.Birthday;
            Account = new AccountUpdateModel { Email = user.Email };
        }

        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateOnly Birthday { get; set; } = new DateOnly();
        public AccountUpdateModel Account { get; set; } = new AccountUpdateModel();
    }
}
