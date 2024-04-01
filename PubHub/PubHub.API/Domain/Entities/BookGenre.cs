namespace PubHub.API.Domain.Entities
{
    public class BookGenre
    {
        public Guid BookId { get; set; }
        public Guid GenreId { get; set; }

        #region Navs
        public Book? Book { get; set; }
        public Genre? Genre { get; set; }
        #endregion
    }
}
