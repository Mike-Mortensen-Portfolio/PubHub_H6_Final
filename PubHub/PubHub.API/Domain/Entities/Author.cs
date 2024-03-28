namespace PubHub.API.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        #region Navs
        public IList<BookAuthor> BookAuthors { get; set; } = [];
        #endregion 
    }
}
