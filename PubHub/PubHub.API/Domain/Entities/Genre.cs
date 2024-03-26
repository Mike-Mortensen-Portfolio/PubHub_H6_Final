namespace PubHub.API.Domain.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        #region Navs
        public IList<Book> Books { get; set; } = [];
        #endregion
    }
}
