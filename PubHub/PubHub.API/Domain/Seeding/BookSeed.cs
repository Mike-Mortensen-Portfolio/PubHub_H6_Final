using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class BookSeed : SeedBase<Book, Guid>
    {
        private readonly ContentTypeSeed _contentTypeSeed;
        private readonly PublisherSeed _publisherSeed;

        public BookSeed(ContentTypeSeed contentTypeSeed, PublisherSeed publisherSeed)
        {
            _contentTypeSeed = contentTypeSeed;
            _publisherSeed = publisherSeed;
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
                    Id = UuidValueGenerator.Next(),
                    BookContent = [],
                    CoverImage = File.ReadAllBytes(EXQUISITE_COVER_PATH),
                    ContentTypeId = _contentTypeSeed[AUDIO_CONTENT_TYPE].Id,
                    Length = TimeSpan.FromHours(1).TotalSeconds,
                    PublicationDate = new DateOnly (2018, 7, 1),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "Exquisite",
                },
                new Book {
                    Id = UuidValueGenerator.Next(),
                    BookContent = [],
                    CoverImage = File.ReadAllBytes(HORSE_COVER_PATH),
                    ContentTypeId = _contentTypeSeed[E_BOOK_CONTENT_TYPE].Id,
                    Length = 123,
                    PublicationDate = new DateOnly (2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER2_EMAIL].Id,
                    Title = "Horse"
                },
                new Book
                {
                    Id = UuidValueGenerator.Next(),
                    BookContent = [],
                    CoverImage = File.ReadAllBytes(HORSE_COVER_PATH),
                    ContentTypeId = _contentTypeSeed[AUDIO_CONTENT_TYPE].Id,
                    Length = 123,
                    PublicationDate = new DateOnly(2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER2_EMAIL].Id,
                    Title = "Horse"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
