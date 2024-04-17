using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;

namespace PubHub.API.UT.Utilities
{
    public class ApiDataGeneratorFixture : DataGeneratorFixture
    {
        public Func<Guid>? ContentTypeIdFunc { get; set; }

        public ApiDataGeneratorFixture() : base()
        { }

        public override void RebuildGenerator()
        {
            base.RebuildGenerator();

            Generator.Customize<Publisher>(x => x
                .Without(p => p.AccountId)
                .Without(p => p.Books));
            Generator.Customize<Account>(x => x
                .With(a => a.DeletedDate, (DateTime?)null));
            Generator.Customize<Book>(x => x
                .Without(b => b.ContentType)
                .With(b => b.ContentTypeId, () => ContentTypeIdFunc?.Invoke() ?? throw new InvalidOperationException("sratiohratn afp"))
                .Without(b => b.Publisher)
                .Without(b => b.PublisherId)
                .Without(b => b.UserBooks)
                .With(b => b.IsHidden, false)
                .With(b => b.PublicationDate, DateOnly.FromDateTime(DateTime.UtcNow)));
        }
    }
}
