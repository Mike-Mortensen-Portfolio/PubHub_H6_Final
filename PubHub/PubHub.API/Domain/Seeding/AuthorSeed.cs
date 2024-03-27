using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class AuthorSeed : SeedBase<Author, int>
    {
        /// <summary>
        /// Find the first <see cref="Author"/> in the seed data where the ID matches the <paramref name="key"/>
        /// </summary>
        /// <param name="key">The ID of the <see cref="Author"/></param>
        /// <returns><inheritdoc/></returns>
        public override Author this[int key] => Seeds.First(a => a.Id == key);

        public override void Configure(EntityTypeBuilder<Author> builder)
        {
            Seeds =
            [
                new Author {Id = 1, Name = "Jhon Doe"},
                new Author {Id = 2, Name = "Jane Doe"},
                new Author {Id = 3, Name = "Dan Turéll"}
            ];

            builder.HasData(Seeds);
        }
    }
}
