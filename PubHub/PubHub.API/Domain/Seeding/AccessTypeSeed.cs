﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;
using static PubHub.API.Domain.Seeding.SeedContants;

namespace PubHub.API.Domain.Seeding
{
    internal class AccessTypeSeed : SeedBase<AccessType, string>
    {
        /// <summary>
        /// Find the <see cref="AccessType"/> where the name matches <paramref name="key"/>
        /// </summary>
        /// <param name="key">The name to search for</param>
        /// <returns><inheritdoc/></returns>
        public override AccessType this[string key]
        {
            get
            {
                var normalizedKey = key.ToUpperInvariant();
                return Seeds.First(at => at.Name.Equals(normalizedKey, StringComparison.InvariantCultureIgnoreCase));
            }
        }
        public override void Configure(EntityTypeBuilder<AccessType> builder)
        {
            Seeds =
            [
                new AccessType { Id = UuidValueGenerator.Next(), Name = OWNER_ACCESS_TYPE },
                new AccessType { Id = UuidValueGenerator.Next(), Name = SUBSCIBER_ACCESS_TYPE },
                new AccessType { Id = UuidValueGenerator.Next(), Name = BORROWER_ACCESS_TYPE },
                new AccessType { Id = UuidValueGenerator.Next(), Name = EXPIRED_ACCESS_TYPE }
            ];

            builder.HasData(Seeds);
        }
    }
}
