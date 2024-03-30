using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace PubHub.API.Domain.UUID
{
    public class UuidValueGenerator : ValueGenerator<Guid>
    {
        private static readonly UUIDNext.Database db = UUIDNext.Database.SqlServer;

        public override bool GeneratesTemporaryValues => false;

        public static Guid Next() => GenerateNewGuid();
        public override Guid Next(EntityEntry entry) => GenerateNewGuid();

        private static Guid GenerateNewGuid() =>
            UUIDNext.Uuid.NewDatabaseFriendly(db);
    }
}
