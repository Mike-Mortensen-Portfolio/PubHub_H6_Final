using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Users
{
    public class UserUpdateModel
    {
        public UserUpdateModel(UserInfoModel user)
        {
            Name = user.Name;
            Surname = user.Surname;
            Birthday = user.Birthday;
            Account = new AccountUpdateModel { Email = user.Email };
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly Birthday { get; set; }
        public AccountUpdateModel Account { get; set; }
    }
}
