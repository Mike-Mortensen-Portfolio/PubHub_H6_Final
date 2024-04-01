using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public class Operator
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }

        #region Navs
        public Account? Account { get; set; }
        #endregion
    }
}
