namespace PubHub.Common.Models
{
    public class UserModel
    {
        public required int Id { get; init; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }
        public required string AccountType { get; init; }
    }
}
