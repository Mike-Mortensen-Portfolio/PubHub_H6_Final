namespace PubHub.Common.Models.Books
{
    public class BookQuery : PaginationQuery
    {
        public OrderBooksBy OrderBy { get; set; }
        public int[]? Genres { get; set; }
    }
}
