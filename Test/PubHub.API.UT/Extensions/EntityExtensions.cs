using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Authors;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.ContentTypes;
using PubHub.Common.Models.Genres;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.UT.Extensions
{
    internal static class EntityExtensions
    {
        #region Publisher
        public static PublisherInfoModel ToInfo(this Publisher publisher) => new()
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Email = publisher.Account!.Email
        };

        public static IEnumerable<PublisherInfoModel> ToInfo(this IEnumerable<Publisher> publishers) =>
            publishers.Select(ToInfo);
        #endregion

        #region Book
        public static BookInfoModel ToInfo(this Book book) => new()
        {
            Id = book.Id,
            Publisher = new() { Id = book.Publisher!.Id, Name = book.Publisher.Name },
            Title = book.Title,
            Summary = book.Summary,
            CoverImage = book.CoverImage,
            ContentType = book.ContentType!.ToInfo(),
            PublicationDate = book.PublicationDate,
            Length = book.Length,
            Genres = book.BookGenres.Select(bg => bg.Genre!.ToInfo()).ToList(),
            Authors = book.BookAuthors.Select(ba => ba.Author!.ToInfo()).ToList()
        };
        #endregion

        #region Author
        public static AuthorInfoModel ToInfo(this Author author) => new()
        {
            Id = author.Id,
            Name = author.Name
        };
        #endregion

        #region Genre
        public static GenreInfoModel ToInfo(this Genre genre) => new()
        {
            Id = genre.Id,
            Name = genre.Name
        };
        #endregion

        #region Types
        public static ContentTypeInfoModel ToInfo(this ContentType contentType) => new()
        {
            Id = contentType.Id,
            Name = contentType.Name
        };
        #endregion
    }
}
