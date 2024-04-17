using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Books;

namespace PubHub.API.Domain.Extensions
{
    public static class BookExtensions
    {
        /// <summary>
        /// Filter the <paramref name="query"/> result and performs pagination based on the provided <paramref name="options"/>
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns>The filtered <see cref="IQueryable{T}"/> of type <see cref="Book"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<Book> Filter(this IQueryable<Book> query, BookQuery options)
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
        public static IQueryable<Book> Find(this IQueryable<Book> query, BookQuery searchTerms)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            var normalizedSearchKey = searchTerms.SearchKey?.ToUpper();
            query = query.Where(b =>
            (normalizedSearchKey == null ||
                b.BookAuthors.Any(ba => ba.Author != null && ba.Author.Name.ToUpper().Contains(normalizedSearchKey)) ||
                b.Title.ToUpper().Contains(normalizedSearchKey) ||
                (b.Publisher != null && b.Publisher.Name.ToUpper().Contains(normalizedSearchKey))));

            if (searchTerms.Genres != null && searchTerms.Genres.Length > 0)
                query = query.Where(b => b.BookGenres.Select(bg => bg.GenreId).Intersect(searchTerms.Genres).Any());

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
        public static IQueryable<Book> Order(this IQueryable<Book> query, OrderBooksBy orderBy, bool descending = true)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query), "Query can't be null");

            switch (orderBy)
            {
                case OrderBooksBy.PublicationDate:
                    if (descending)
                        query = query.OrderByDescending(b => b.PublicationDate)
                            .ThenBy(b => b.Title);
                    else
                        query = query.OrderBy(b => b.PublicationDate)
                            .ThenBy(b => b.Title);
                    break;
            }

            return query;
        }
    }
}
