namespace PubHub.Common.Models.Users
{
    public class UserInfoModel
    {
        public required int Id { get; init; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }
        public required string AccountType { get; init; }
    }
}
