namespace PubHub.API.Domain.Entities
{
    public class BookGenre
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }

        #region Navs
        public Book? Book { get; set; }
        public Genre? Genre { get; set; }
        #endregion
    }
}
