namespace PubHub.Common.Models.Books
{
    public class BookQuery : PaginationQuery
    {
        public OrderBooksBy OrderBy { get; set; }
        public Guid[]? Genres { get; set; }
    }
}
