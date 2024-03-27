namespace PubHub.Common.Models
{
    public class UserCreateModel()
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }
        public required AccountCreateModel Account { get; set; }
    }
}
