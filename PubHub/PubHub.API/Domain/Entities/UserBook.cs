namespace PubHub.API.Domain.Entities
{
    public class UserBook
    {
        public Guid UserBookId { get; set; }
        public Guid BookId { get; set; }
        public Guid? UserId { get; set; }
        public Guid AccessTypeId { get; set; }
        public float ProgressInProcent { get; set; }
        public DateTime AcquireDate { get; set; }

        #region Navs
        public Book? Book { get; set; }
        public User? User { get; set; }
        public AccessType? AccessType { get; set; }
        #endregion
    }
}
