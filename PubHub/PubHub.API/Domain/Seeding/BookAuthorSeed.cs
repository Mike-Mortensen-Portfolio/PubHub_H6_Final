using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class BookAuthorSeed : IEntityTypeConfiguration<BookAuthor>
    {
        private readonly BookSeed _bookSeed;
        private readonly AuthorSeed _authorSeed;

        public BookAuthorSeed(BookSeed bookSeed, AuthorSeed authorSeed)
        {
            _bookSeed = bookSeed;
            _authorSeed = authorSeed;
        }

        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.HasData(
                new BookAuthor { BookId = _bookSeed.Seeds[0].Id, AuthorId = _authorSeed.Seeds[0].Id },
                new BookAuthor { BookId = _bookSeed.Seeds[1].Id, AuthorId = _authorSeed.Seeds[1].Id },
                new BookAuthor { BookId = _bookSeed.Seeds[1].Id, AuthorId = _authorSeed.Seeds[2].Id },
                new BookAuthor { BookId = _bookSeed.Seeds[2].Id, AuthorId = _authorSeed.Seeds[1].Id },
                new BookAuthor { BookId = _bookSeed.Seeds[2].Id, AuthorId = _authorSeed.Seeds[2].Id });
        }
    }
}
