using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public class Publisher
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Name { get; set; }
        public bool IsDeleted { get; set; }

        #region Navs
        public Account? Account { get; set; }
        #endregion
    }
}
