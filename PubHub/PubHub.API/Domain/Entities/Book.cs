namespace PubHub.API.Domain.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Title { get; set; }
        public required Genre Genre { get; set; }
        public byte[]? CoverImage { get; set; }
        public required Stream Content { get; set; }
        public required ContentType ContentType { get; set; }
        public DateTime PublicationYear { get; set; }
        public double Length { get; set; }
        public bool IsDeleted { get; set; }

        #region Navs
        public required IList<Author> Authors { get; set; }
        public required Publisher Publisher { get; set; }
        #endregion
    }
}
