namespace PubHub.API.Domain.Entities
{
    public class Author
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        #region Navs
        public IList<BookAuthor> BookAuthors { get; set; } = [];
        #endregion 
    }
}
