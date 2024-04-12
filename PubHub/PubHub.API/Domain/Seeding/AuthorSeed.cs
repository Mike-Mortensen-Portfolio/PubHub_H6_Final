using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;

namespace PubHub.API.Domain.Seeding
{
    internal class AuthorSeed : SeedBase<Author, Guid>
    {
        /// <summary>
        /// Find the first <see cref="Author"/> in the seed data where the ID matches the <paramref name="key"/>
        /// </summary>
        /// <param name="key">The ID of the <see cref="Author"/></param>
        /// <returns><inheritdoc/></returns>
        public override Author this[Guid key] => Seeds.First(a => a.Id == key);

        public override void Configure(EntityTypeBuilder<Author> builder)
        {
            Seeds =
            [
                new Author {Id = UuidValueGenerator.Next(), Name = "Jhon Doe"},
                new Author {Id = UuidValueGenerator.Next(), Name = "Jane Doe"},
                new Author {Id = UuidValueGenerator.Next(), Name = "Dan Turéll"}
            ];

            builder.HasData(Seeds);
        }
    }
}
