namespace PubHub.Common.Models
{
    public class UserInfoModel
    {
        public required Guid Id { get; init; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }
        public required string AccountType { get; init; }
    }
}
