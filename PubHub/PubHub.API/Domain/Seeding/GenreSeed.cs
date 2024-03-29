using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;

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
                new Genre { Id = UuidValueGenerator.Next(), Name = "Romance" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Horror" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "History" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Science-Fiction" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Fiction" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Novel" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Fantasy" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Biography" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "True crime" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Thriller" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Young adult" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Mystery" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Satire" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Non-Fiction" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Self-help" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Poetry" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Humor" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Action" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Adventure" },
                new Genre { Id = UuidValueGenerator.Next(), Name = "Short story" }
            ];

            builder.HasData(Seeds);
        }
    }
}
