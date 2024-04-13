namespace PubHub.BookMobile.Models
{
    /// <summary>
    /// Represents a routable <see cref="ContentPage"/> that takes in parameters
    /// </summary>
    public class PageInfo
    {
        public required string RouteName { get; init; }
        public required Dictionary<string, object> Parameters { get; init; }

    }
}
