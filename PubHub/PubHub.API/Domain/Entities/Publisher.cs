using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Name { get; set; }

        #region Navs
        public Account? Account { get; set; }
        public IList<Book> Books { get; set; } = [];
        #endregion
    }
}
