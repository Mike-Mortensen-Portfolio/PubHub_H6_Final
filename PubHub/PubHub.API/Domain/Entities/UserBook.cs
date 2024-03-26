namespace PubHub.API.Domain.Entities
{
    public class UserBook
    {
        public int BookId { get; set; }
        public int UserId { get; set; }
        public int AccessTypeId { get; set; }
        public float ProgressInProcent { get; set; }
        public DateTime AcquireDate { get; set; }

        #region Navs
        public required Book Book { get; set; }
        public required User User { get; set; }
        public required AccessType AccessType { get; set; }
        #endregion
    }
}
