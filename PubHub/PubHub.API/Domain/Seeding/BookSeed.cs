using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class BookSeed : SeedBase<Book, Guid>
    {
        private readonly ContentTypeSeed _contentTypeSeed;
        private readonly PublisherSeed _publisherSeed;
        private readonly GenreSeed _genreSeed;
        private readonly AuthorSeed _authorSeed;

        public BookSeed(ContentTypeSeed contentTypeSeed, PublisherSeed publisherSeed, GenreSeed genreSeed, AuthorSeed authorSeed)
        {
            _contentTypeSeed = contentTypeSeed;
            _publisherSeed = publisherSeed;
            _genreSeed = genreSeed;
            _authorSeed = authorSeed;
        }

        /// <summary>
        /// Find a book based on its ID
        /// </summary>
        /// <param name="key">The ID of the <see cref="Book"/></param>
        /// <returns><inheritdoc/></returns>
        public override Book this[Guid key] => Seeds.First(b => b.Id == key);

        public override void Configure(EntityTypeBuilder<Book> builder)
        {
            Seeds =
            [
                new Book {
                    Id = Guid.Empty,
                    BookContent = [],
                    ContentTypeId = _contentTypeSeed[AUDIO_CONTENT_TYPE].Id,
                    Length = TimeSpan.FromHours (1).TotalSeconds,
                    PublicationDate = new DateOnly (1955, 12, 1),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "My day in the shoos of Tommy",
                },
                new Book {
                    Id = Guid.Empty,
                    BookContent = [],
                    ContentTypeId = _contentTypeSeed[E_BOOK_CONTENT_TYPE].Id,
                    Length = 123,
                    PublicationDate = new DateOnly (2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "My horse is the wildest"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
