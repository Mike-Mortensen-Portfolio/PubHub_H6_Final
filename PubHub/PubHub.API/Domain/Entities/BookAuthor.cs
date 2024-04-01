namespace PubHub.API.Domain.Entities
{
    public class BookAuthor
    {
        public Guid BookId { get; set; }
        public Guid AuthorId { get; set; }

        #region Navs
        public Book? Book { get; set; }
        public Author? Author { get; set; }
        #endregion
    }
}
