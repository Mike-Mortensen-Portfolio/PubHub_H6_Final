using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class BookSeed : SeedBase<Book, int>
    {
        private readonly ContentTypeSeed _contentTypeSeed;
        private readonly PublisherSeed _publisherSeed;
        private readonly GenreSeed _genreSeed;

        public BookSeed(ContentTypeSeed contentTypeSeed, PublisherSeed publisherSeed, GenreSeed genreSeed)
        {
            _contentTypeSeed = contentTypeSeed;
            _publisherSeed = publisherSeed;
            _genreSeed = genreSeed;
        }

        /// <summary>
        /// Find a book based on its ID
        /// </summary>
        /// <param name="key">The ID of the <see cref="Book"/></param>
        /// <returns><inheritdoc/></returns>
        public override Book this[int key] => Seeds.First(b => b.Id == key);

        public override void Configure(EntityTypeBuilder<Book> builder)
        {
            Seeds =
            [
                new Book {
                    Id = 1,
                    BookContent = [],
                    ContentType = _contentTypeSeed[AUDIO_CONTENT_TYPE],
                    Length = TimeSpan.FromHours (1).TotalSeconds,
                    PublicationDate = new DateOnly (1955, 12, 1),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "My day in the shoos of Tommy"
                },
                new Book {
                    Id = 2,
                    BookContent = [],
                    ContentType = _contentTypeSeed[E_BOOK_CONTENT_TYPE],
                    Length = 123,
                    PublicationDate = new DateOnly (2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "My horse is the wildest"
                },
            ];

            Seeds[0].Genres.Add(_genreSeed.Seeds[0]);
            Seeds[0].Genres.Add(_genreSeed.Seeds[2]);
            Seeds[0].Genres.Add(_genreSeed.Seeds[8]);
            Seeds[1].Genres.Add(_genreSeed.Seeds[4]);
            Seeds[1].Genres.Add(_genreSeed.Seeds[7]);
            Seeds[1].Genres.Add(_genreSeed.Seeds[1]);

            builder.HasData(Seeds);
        }
    }
}
