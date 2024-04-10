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
                    Summary = "The exquisite creations embody our best artistic endeavors, and offer a glimpse of the greatness of the civilizations that produced them. Their sheer beauty and charm are enough for us to marvel at, let alone the large sum of resources, efforts and time poured into making them. In this book, we hope to follow our predecessors' footprints in the endless pursuit of exquisite beauty, and to explore the possibilities of how this style might blaze new trails in today's graphic design world."
                },
                new Book {
                    Id = UuidValueGenerator.Next(),
                    BookContent = [],
                    CoverImage = File.ReadAllBytes(HORSE_COVER_PATH),
                    ContentTypeId = _contentTypeSeed[E_BOOK_CONTENT_TYPE].Id,
                    Length = 123,
                    PublicationDate = new DateOnly (2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER2_EMAIL].Id,
                    Title = "Horse",
                    Summary = "Kentucky, 1850. An enslaved groom named Jarret and a bay foal forge a bond of understanding that will carry the horse to record-setting victories across the South. When the nation erupts in civil war, an itinerant young artist who has made his name on paintings of the racehorse takes up arms for the Union."
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
                    Title = "Horse",
                    Summary = "Kentucky, 1850. An enslaved groom named Jarret and a bay foal forge a bond of understanding that will carry the horse to record-setting victories across the South. When the nation erupts in civil war, an itinerant young artist who has made his name on paintings of the racehorse takes up arms for the Union."
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
