using PubHub.API.Domain.Entities;
using PubHub.API.Domain.Identity;
using PubHub.Common.Models;

namespace PubHub.API.Domain
{
    /// <summary>
    /// Map database entities to models used in data transfer and vice versa.
    /// Methods that maps from entities are all translateable by EF Core and can be used with <see cref="Queryable"/>.
    /// </summary>
    public static class PropertyMapper
    {
        /// <summary>
        /// Map <see cref="PubHubUser"/> to <see cref="UserModel"/>.
        /// </summary>
        /// <param name="user"><see cref="PubHubUser"/> with occupied <see cref="PubHubUser.Account"/> (and then <see cref="Account.AccountType"/>) properties.</param>
        /// <returns><see cref="UserModel"/> with data from <paramref name="user"/>.</returns>
        /// <exception cref="InvalidOperationException">The data to map from is invalid.</exception>
        public static UserModel MapUserModelProperties(this PubHubUser user)
        {
            if (user.Account == null) throw new InvalidOperationException("No account. Did you include user account?");
            if (user.Account.AccountType == null) throw new InvalidOperationException("No account type. Did you include user account type?");

            return new()
            {
                Id = user.Id,
                Email = user.Account.Email ?? user.Account.NormalizedEmail ?? throw new InvalidOperationException("User has no email."),
                Name = user.Name,
                Surname = user.Surname,
                Birthday = user.Birthday,
                AccountType = user.Account.AccountType.Name
            };
        }

        /// <summary>
        /// Map <see cref="Book"/> to <see cref="BookModel"/>.
        /// </summary>
        /// <param name="book"><see cref="Book"/> to map from.</param>
        /// <returns><see cref="BookModel"/> with data from <paramref name="book"/>.</returns>
        public static BookModel MapBookModelProperties(this Book book) => new()
        {
            Title = book.Title,
            PublicationDate = book.PublicationDate
        };
    }
}
