using PubHub.Common.Models.Authors;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;

namespace PubHub.Common.Models.Books
{
    public class BookInfoModel
    {
        public Guid Id { get; init; }
        public required BookPublisherModel Publisher { get; init; } = null!;
        public required string Title { get; set; }
        public byte[]? CoverImage { get; set; }
        public required ContentTypeInfoModel ContentType { get; init; }
        public DateOnly PublicationDate { get; set; }
        public double Length { get; init; }
        public IList<GenreInfoModel> Genres { get; set; } = [];
        public IList<AuthorInfoModel> Authors { get; set; } = [];
    }
}
