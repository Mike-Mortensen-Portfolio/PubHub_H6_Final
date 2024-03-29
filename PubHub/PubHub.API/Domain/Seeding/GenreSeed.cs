using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class GenreSeed : SeedBase<Genre, string>
    {
        public override Genre this[string key]
        {
            get
            {
                var normalizedKey = key.ToUpperInvariant();
                return Seeds.First(g => g.Name.Equals(normalizedKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public override void Configure(EntityTypeBuilder<Genre> builder)
        {
            Seeds =
            [
                new Genre { Id = Guid.Empty, Name = "Romance" },
                new Genre { Id = Guid.Empty, Name = "Horror" },
                new Genre { Id = Guid.Empty, Name = "History" },
                new Genre { Id = Guid.Empty, Name = "Science-Fiction" },
                new Genre { Id = Guid.Empty, Name = "Fiction" },
                new Genre { Id = Guid.Empty, Name = "Novel" },
                new Genre { Id = Guid.Empty, Name = "Fantasy" },
                new Genre { Id = Guid.Empty, Name = "Biography" },
                new Genre { Id = Guid.Empty, Name = "True crime" },
                new Genre { Id = Guid.Empty, Name = "Thriller" },
                new Genre { Id = Guid.Empty, Name = "Young adult" },
                new Genre { Id = Guid.Empty, Name = "Mystery" },
                new Genre { Id = Guid.Empty, Name = "Satire" },
                new Genre { Id = Guid.Empty, Name = "Non-Fiction" },
                new Genre { Id = Guid.Empty, Name = "Self-help" },
                new Genre { Id = Guid.Empty, Name = "Poetry" },
                new Genre { Id = Guid.Empty, Name = "Humor" },
                new Genre { Id = Guid.Empty, Name = "Action" },
                new Genre { Id = Guid.Empty, Name = "Adventure" },
                new Genre { Id = Guid.Empty, Name = "Short story" }
            ];

            builder.HasData(Seeds);
        }
    }
}
