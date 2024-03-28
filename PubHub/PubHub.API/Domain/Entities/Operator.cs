using PubHub.API.Domain.Identity;

namespace PubHub.API.Domain.Entities
{
    public class Operator
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }

        #region Navs
        public Account? Account { get; set; };
        #endregion
    }
}
