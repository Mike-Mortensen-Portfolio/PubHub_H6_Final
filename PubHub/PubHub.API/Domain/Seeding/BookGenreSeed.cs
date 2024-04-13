using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    internal class BookGenreSeed : IEntityTypeConfiguration<BookGenre>
    {
        private readonly BookSeed _bookSeed;
        private readonly GenreSeed _genreSeed;

        public BookGenreSeed(BookSeed bookSeed, GenreSeed genreSeed)
        {
            _bookSeed = bookSeed;
            _genreSeed = genreSeed;
        }

        public void Configure(EntityTypeBuilder<BookGenre> builder)
        {
            builder.HasData(
                new BookGenre { BookId = _bookSeed.Seeds[0].Id, GenreId = _genreSeed.Seeds[0].Id },
                new BookGenre { BookId = _bookSeed.Seeds[0].Id, GenreId = _genreSeed.Seeds[2].Id },
                new BookGenre { BookId = _bookSeed.Seeds[0].Id, GenreId = _genreSeed.Seeds[8].Id },
                new BookGenre { BookId = _bookSeed.Seeds[1].Id, GenreId = _genreSeed.Seeds[4].Id },
                new BookGenre { BookId = _bookSeed.Seeds[1].Id, GenreId = _genreSeed.Seeds[7].Id },
                new BookGenre { BookId = _bookSeed.Seeds[1].Id, GenreId = _genreSeed.Seeds[1].Id },
                new BookGenre { BookId = _bookSeed.Seeds[1].Id, GenreId = _genreSeed.Seeds[5].Id },
                new BookGenre { BookId = _bookSeed.Seeds[2].Id, GenreId = _genreSeed.Seeds[4].Id },
                new BookGenre { BookId = _bookSeed.Seeds[2].Id, GenreId = _genreSeed.Seeds[7].Id },
                new BookGenre { BookId = _bookSeed.Seeds[2].Id, GenreId = _genreSeed.Seeds[1].Id },
                new BookGenre { BookId = _bookSeed.Seeds[2].Id, GenreId = _genreSeed.Seeds[5].Id });
        }
    }
}
