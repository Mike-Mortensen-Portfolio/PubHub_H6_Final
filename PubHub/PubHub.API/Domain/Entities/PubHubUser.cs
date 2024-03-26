using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public sealed class PubHubUser
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required DateOnly Birthday { get; set; }

        #region Navs
        public Account? Account { get; set; }
        public required IList<UserBook> UserBooks { get; set; }
        #endregion
    }
}
