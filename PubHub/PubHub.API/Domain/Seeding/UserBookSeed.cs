using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.UUID;

namespace PubHub.API.Domain.Seeding
{
    internal class UserBookSeed : IEntityTypeConfiguration<UserBook>
    {
        private readonly UserSeed _userSeed;
        private readonly BookSeed _bookSeed;
        private readonly AccessTypeSeed _accessTypeSeed;
        public UserBookSeed(UserSeed userSeed, BookSeed bookSeed, AccessTypeSeed accessTypeSeed)
        {
            _userSeed = userSeed;
            _bookSeed = bookSeed;
            _accessTypeSeed = accessTypeSeed;
        }

        public void Configure(EntityTypeBuilder<UserBook> builder)
        {
            builder.HasData(
                new UserBook()
                {
                    UserBookId = UuidValueGenerator.Next(),
                    AccessTypeId = _accessTypeSeed[SeedContants.OWNER_ACCESS_TYPE].Id,
                    AcquireDate = DateTime.UtcNow,
                    BookId = _bookSeed.Seeds[0].Id,
                    UserId = _userSeed.Seeds[0].Id,
                    ProgressInProcent = 45.34f
                },
                new UserBook()
                {
                    UserBookId = UuidValueGenerator.Next(),
                    AccessTypeId = _accessTypeSeed[SeedContants.OWNER_ACCESS_TYPE].Id,
                    AcquireDate = DateTime.UtcNow,
                    BookId = _bookSeed.Seeds[1].Id,
                    UserId = _userSeed.Seeds[0].Id,
                    ProgressInProcent = 0f
                });
        }
    }
}
