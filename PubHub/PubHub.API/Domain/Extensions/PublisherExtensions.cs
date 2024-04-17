using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.Domain.Extensions
{
    public static class PublisherExtensions
    {
#pragma warning disable CA1862 // Use the 'StringComparison' method overloads to perform case-insensitive string comparisons

        /// <summary>
        /// Filter the <paramref name="query"/> result and performs pagination based on the provided <paramref name="options"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns>The filtered <see cref="IQueryable{T}"/> of type <see cref="Book"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<Publisher> Filter(this IQueryable<Publisher> query, PublisherQuery options)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            query = Find(query, options)
                .Order(options.OrderBy, options.Descending);

            if (options.Max.HasValue && options.Max > 0)
                query = query.Paginate(options.Page, options.Max.Value);

            return query;
        }
        /// <summary>
        /// Finds all books in the <paramref name="query"/> where the <paramref name="searchTerms"/> can be identified.
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This will search for the key in the books title, publisher and authors
        /// </summary>
        /// <param name="query"></param>
        /// <param name="searchTerms"></param>
        /// <returns>An <see cref="IQueryable{T}"/> of type <see cref="Book"/> where the <paramref name="searchTerms"/> could be found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<Publisher> Find(this IQueryable<Publisher> query, PublisherQuery searchTerms)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            var normalizedSearchKey = searchTerms.SearchKey?.ToUpper();
            query = query.Where(p =>
            (normalizedSearchKey == null ||
                p.Name.ToUpperInvariant().Contains(normalizedSearchKey)));

            return query;
        }

        /// <summary>
        /// Orders the <paramref name="query"/> of type <see cref="Book"/> according to <paramref name="orderBy"/> and <paramref name="descending"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="descending"></param>
        /// <returns>An <see cref="IQueryable{T}"/> of type <see cref="Book"/> as an ordered collection</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<Publisher> Order(this IQueryable<Publisher> query, OrderPublisherBy orderBy, bool descending = true)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            switch (orderBy)
            {
                case OrderPublisherBy.Name:
                    if (descending)
                        query = query.OrderByDescending(p => p.Name);
                    else
                        query = query.OrderBy(p => p.Name);
                    break;
            }

            return query;
        }
    }
}
