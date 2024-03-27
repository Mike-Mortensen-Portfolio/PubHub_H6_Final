﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    public class ContentTypeSeed : SeedBase<ContentType, string>
    {
        public override ContentType this[string key]
        {
            get
            {
                var normalizedType = key.ToUpperInvariant();
                return Seeds.First(ct => ct.Name.Equals(normalizedType, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public override void Configure(EntityTypeBuilder<ContentType> builder)
        {
            Seeds =
            [
                new ContentType { Id = 1, Name = AUDIO_CONTENT_TYPE },
                new ContentType { Id = 2, Name = E_BOOK_CONTENT_TYPE }
            ];

            builder.HasData(Seeds);
        }
    }
}
