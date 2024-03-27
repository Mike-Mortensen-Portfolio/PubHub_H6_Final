namespace PubHub.API.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public int PublisherId { get; set; }
        public required string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public required byte[] BookContent { get; set; }
        public required ContentType ContentType { get; set; }
        public DateOnly PublicationDate { get; set; }
        public double Length { get; set; }
        public bool IsHidden { get; set; }

        #region Navs
        public IList<Author> Authors { get; set; } = [];
        public IList<UserBook> UserBooks { get; set; } = [];
        public IList<Genre> Genres { get; set; } = [];
        public Publisher Publisher { get; set; } = null!;
        #endregion
    }
}
