namespace PubHub.BookMobile.Models
{
    public class PageInfo
    {
        public required string PageName { get; init; }
        public required Dictionary<string, object> Parameters { get; init; }

    }
}
