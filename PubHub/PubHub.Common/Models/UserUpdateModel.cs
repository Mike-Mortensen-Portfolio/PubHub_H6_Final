namespace PubHub.Common.Models
{
    public class UserUpdateModel(UserModel user)
    {
        public required string Email { get; set; } = user.Email;
        public required string Name { get; set; } = user.Name;
        public required string Surname { get; set; } = user.Surname;
        public required DateOnly Birthday { get; set; } = user.Birthday;
    }
}
