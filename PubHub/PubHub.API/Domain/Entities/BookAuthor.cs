namespace PubHub.API.Domain.Entities
{
    public class BookAuthor
    {
        public int BookId { get; set; }
        public int AuthorId { get; set; }

        #region Navs
        public Book? Book { get; set; }
        public Author? Author { get; set; }
        #endregion
    }
}
