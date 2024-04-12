using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Books;

namespace PubHub.API.Domain.Extensions
{
    public static class UserBookExtensions
    {
        /// <summary>
        /// Filter the <paramref name="query"/> result and performs pagination based on the provided <paramref name="options"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns>The filtered <see cref="IQueryable{T}"/> of type <see cref="UserBook"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<UserBook> Filter(this IQueryable<UserBook> query, BookQuery options)
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
        /// Finds all userBooks in the <paramref name="query"/> where the <paramref name="searchTerms"/> can be identified.
        /// <br/>
        /// <br/>
        /// <strong>Note:</strong> This will search for the key in the userBooks title, publisher and authors
        /// </summary>
        /// <param name="query"></param>
        /// <param name="searchTerms"></param>
        /// <returns>An <see cref="IQueryable{T}"/> of type <see cref="UserBook"/> where the <paramref name="searchTerms"/> could be found</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<UserBook> Find(this IQueryable<UserBook> query, BookQuery searchTerms)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            var normalizedSearchKey = searchTerms.SearchKey?.ToUpper();
            query = query.Where(ub =>
            (normalizedSearchKey == null ||
                ub.Book!.BookAuthors.Any(ba => ba.Author != null && ba.Author.Name.ToUpper().Contains(normalizedSearchKey)) ||
                ub.Book.Title.ToUpper().Contains(normalizedSearchKey) ||
                (ub.Book.Publisher != null && ub.Book.Publisher.Name.ToUpper().Contains(normalizedSearchKey))));

            if (searchTerms.Genres != null && searchTerms.Genres.Length > 0)
                query = query.Where(ub => ub.Book!.BookGenres.Select(bg => bg.GenreId).Intersect(searchTerms.Genres).Any());

            return query;
        }

        /// <summary>
        /// Orders the <paramref name="query"/> of type <see cref="UserBook"/> according to <paramref name="orderBy"/> and <paramref name="descending"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orderBy"></param>
        /// <param name="descending"></param>
        /// <returns>An <see cref="IQueryable{T}"/> of type <see cref="UserBook"/> as an ordered collection</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<UserBook> Order(this IQueryable<UserBook> query, OrderBooksBy orderBy, bool descending = true)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            switch (orderBy)
            {
                case OrderBooksBy.PublicationDate:
                    if (descending)
                        query = query.OrderByDescending(ub => ub.Book!.PublicationDate)
                            .ThenBy(ub => ub.Book!.Title);
                    else
                        query = query.OrderBy(ub => ub.Book!.PublicationDate)
                            .ThenBy(ub => ub.Book!.Title);
                    break;
            }

            return query;
        }
    }
}
