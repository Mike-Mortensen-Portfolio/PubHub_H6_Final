﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    internal class BookSeed : SeedBase<Book, Guid>
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
                    BookContentUri = PLACEBOEFFEKTEN_CONTENT_PATH,
                    CoverImageUri = EXQUISITE_COVER_PATH,
                    ContentTypeId = _contentTypeSeed[AUDIO_CONTENT_TYPE].Id,
                    Length = TimeSpan.FromHours(1).TotalSeconds,
                    PublicationDate = new DateOnly (2018, 7, 1),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "Exquisite",
                    Summary = "The exquisite creations embody our best artistic endeavors, and offer a glimpse of the greatness of the civilizations that produced them. Their sheer beauty and charm are enough for us to marvel at, let alone the large sum of resources, efforts and time poured into making them. In this book, we hope to follow our predecessors' footprints in the endless pursuit of exquisite beauty, and to explore the possibilities of how this style might blaze new trails in today's graphic design world."
                },
                new Book {
                    Id = UuidValueGenerator.Next(),
                    BookContentUri = SHADOWGAME_CONTENT_PATH,
                    CoverImageUri = HORSE_COVER_PATH,
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
                    BookContentUri = PLACEBOEFFEKTEN_CONTENT_PATH,
                    CoverImageUri = HORSE_COVER_PATH,
                    ContentTypeId = _contentTypeSeed[AUDIO_CONTENT_TYPE].Id,
                    Length = 123,
                    PublicationDate = new DateOnly(2023, 4, 7),
                    PublisherId = _publisherSeed[PUBLISHER2_EMAIL].Id,
                    Title = "Horse",
                    Summary = "Kentucky, 1850. An enslaved groom named Jarret and a bay foal forge a bond of understanding that will carry the horse to record-setting victories across the South. When the nation erupts in civil war, an itinerant young artist who has made his name on paintings of the racehorse takes up arms for the Union."
                },
                new Book
                {
                    Id = UuidValueGenerator.Next(),
                    BookContentUri = SHADOWGAME_CONTENT_PATH,
                    CoverImageUri = SHADOWGAME_COVER_PATH,
                    ContentTypeId = _contentTypeSeed[E_BOOK_CONTENT_TYPE].Id,
                    Length = 344,
                    PublicationDate = new DateOnly(2021, 9, 28),
                    PublisherId = _publisherSeed[PUBLISHER_EMAIL].Id,
                    Title = "Shadow game",
                    Summary = "I 1600-tallets Aalborg straffes trolddom med døden, og før 14-årige Gry ved af det, sender en hekseanklage hende på bålet.\r\nMen de glubske flammer brænder hende ikke som de burde. \r\nSyttenårige Akela har accepteret dronning Soras tilbud om en plads ved hoffet.\r\nHer skal hun gennemføre skyggespillet; en række magiske dueller der afgør de Udvalgtes fremtid.\r\nKun ved at udmærke sig og opnå en plads som dronningens rådgiver kan Akela beskytte sin søster og forhindre at krigen rammer deres by. Desværre er der seks Udvalgte og blot én plads i hofrådet.\r\nMens oprørerne angriber, og bedrageriske skygger dukker op uden for skyggespillet, optrevler Akela den royale slægts hemmeligheder. Men hvem tør hun stole på når alle bekriger hinanden i dronningens spil?"
                }
            ];

            builder.HasData(Seeds);
        }
    }
}
