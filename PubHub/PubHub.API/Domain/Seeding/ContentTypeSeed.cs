using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;

namespace PubHub.API.Domain.Seeding
{
    public class ContentTypeSeed : IEntityTypeConfiguration<ContentType>
    {
        public void Configure(EntityTypeBuilder<ContentType> builder)
        {
            builder.HasData(
                new ContentType { Id = 1, Name = "AudioBook" },
                new ContentType { Id = 2, Name = "EBook" });
        }
    }
}
