using PubHub.Common.Models.Accounts;

namespace PubHub.Common.Models.Users
{
    public class UserCreateModel()
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }
        public required AccountCreateModel Account { get; set; }
    }
}
