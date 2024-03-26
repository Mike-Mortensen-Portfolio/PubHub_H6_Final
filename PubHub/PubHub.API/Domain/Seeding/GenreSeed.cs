using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class GenreSeed : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.HasData(
                new Genre { Id = 1, Name = "Romance" },
                new Genre { Id = 2, Name = "Horror" },
                new Genre { Id = 3, Name = "History" },
                new Genre { Id = 4, Name = "Science-Fiction" },
                new Genre { Id = 5, Name = "Fiction" },
                new Genre { Id = 6, Name = "Novel" },
                new Genre { Id = 7, Name = "Fantasy" },
                new Genre { Id = 8, Name = "Biography" },
                new Genre { Id = 9, Name = "True crime" },
                new Genre { Id = 10, Name = "Thriller" },
                new Genre { Id = 11, Name = "Young adult" },
                new Genre { Id = 12, Name = "Mystery" },
                new Genre { Id = 13, Name = "Satire" },
                new Genre { Id = 14, Name = "Non-Fiction" },
                new Genre { Id = 15, Name = "Self-help" },
                new Genre { Id = 16, Name = "Poetry" },
                new Genre { Id = 17, Name = "Humor" },
                new Genre { Id = 18, Name = "Action" },
                new Genre { Id = 19, Name = "Adventure" },
                new Genre { Id = 20, Name = "Short story" });
        }
    }
}
