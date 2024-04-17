using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public sealed class User
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }

        #region Navs
        public Account? Account { get; set; }
        public IList<UserBook> UserBooks { get; set; } = [];
        #endregion
    }
}
