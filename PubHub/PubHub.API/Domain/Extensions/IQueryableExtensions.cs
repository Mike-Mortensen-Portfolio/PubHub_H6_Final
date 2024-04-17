namespace PubHub.API.Domain.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Performs actions to divide target query into pages and returns the requested <paramref name="page"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="page">Which page to return</param>
        /// <param name="max">The maximum amount of entries pr. page</param>
        /// <returns>An <see cref="IQueryable{T}"/> containing the result of the pagination action</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int page, int max)
        {
            if (page > 0 && max > 0)
                query = query.Skip((page * max) - max)
                    .Take(max);

            return query;
        }
    }
}
