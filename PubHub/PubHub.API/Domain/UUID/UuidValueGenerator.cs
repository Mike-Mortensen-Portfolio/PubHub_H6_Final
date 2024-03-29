using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using UUIDNext.Generator;

namespace PubHub.API.Domain.UUID
{
    public class UuidValueGenerator : ValueGenerator<Guid>
    {
        private static readonly UuidV7Generator _generator = new();

        public override bool GeneratesTemporaryValues => false;

        public static Guid Next() => _generator.New();
        public override Guid Next(EntityEntry entry) => _generator.New();
    }
}
