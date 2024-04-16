namespace PubHub.Common.Models.Users
{
    public class UserBookInfoModel
    {
        public Guid UserBookId { get; set; }
        public Guid BookId { get; set; }
        public Guid? UserId { get; set; }
        public Guid AccessTypeId { get; set; }
        public float ProgressInProcent { get; set; }
        public DateTime AcquireDate { get; set; }
    }
}
