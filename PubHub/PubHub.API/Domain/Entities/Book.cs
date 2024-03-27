namespace PubHub.API.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public int ContentTypeId { get; set; }
        public int PublisherId { get; set; }
        public required string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public required byte[] BookContent { get; set; }
        public DateOnly PublicationDate { get; set; }
        public double Length { get; set; }
        public bool IsHidden { get; set; }

        #region Navs
        public IList<BookAuthor> BookAuthors { get; set; } = [];
        public IList<UserBook> UserBooks { get; set; } = [];
        public IList<BookGenre> BookGenres { get; set; } = [];
        public Publisher? Publisher { get; set; } = null!;
        public ContentType? ContentType { get; set; } = null!;
        #endregion
    }
}
