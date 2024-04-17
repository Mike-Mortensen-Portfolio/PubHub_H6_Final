namespace PubHub.Common.Models.Users
{
    public class UserBookUpdateModel
    {
        public required Guid bookId { get; set; }
        public required float ProgressInProcent { get; init; }
    }
}
