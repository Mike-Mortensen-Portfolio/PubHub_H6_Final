using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.UT.Extensions
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Assert that a publisher has been created correctly from a given <see cref="PublisherCreateModel"/>.
        /// </summary>
        /// <param name="id">ID of publisher.</param>
        public static void AssertCreated(this PubHubContext context, PublisherCreateModel publisher, Guid id)
        {
            // Get created publisher from database.
            var actualPublishers = context.Set<Publisher>()
                .Include(p => p.Account)
                .Where(p => p.Id == id)
                .ToList();
            var actualPublisher = Assert.Single(actualPublishers);

            // Assert properties of actual publisher.
            Assert.Equal(publisher.Name, actualPublisher.Name);
            Assert.NotNull(actualPublisher.Account);
            Assert.Equal(publisher.Account.Email, actualPublisher.Account.Email);
        }

        /// <summary>
        /// Assert that an account has been soft deleted.
        /// </summary>
        public static void AssertDeleted(this PubHubContext context, Account account)
        {
            // Get account.
            var actualAccounts = context.Set<Account>()
                .IgnoreQueryFilters()
                .Where(a => a.Id == account.Id && a.DeletedDate < DateTime.UtcNow)
                .ToList();
            var actualAccount = Assert.Single(actualAccounts);
        }
    }
}
