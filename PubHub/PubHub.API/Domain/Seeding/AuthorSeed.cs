using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class AuthorSeed : SeedBase<Author, int>
    {
        public override Author this[int key] => Seeds.First (a => a.Id == key);

        public override void Configure(EntityTypeBuilder<Author> builder)
        {
            Seeds =
            [
                new Author {Id = 1, Name = "Jhon Doe"},
                new Author {Id = 2, Name = "Jane Doe"},
                new Author {Id = 3, Name = "Dan Turéll"}
            ];
        }
    }
}
