namespace PubHub.TestUtils.Extensions
{
    public static class CollectionExtensions
    {
        private static readonly Random _rnd = new();

        /// <summary>
        /// Return a random element from <paramref name="items"/>.
        /// </summary>
        /// <typeparam name="T">Type of item.</typeparam>
        /// <param name="items">Collection of items.</param>
        /// <returns>Random item from <paramref name="items"/>.</returns>
        public static T Random<T>(this IEnumerable<T> items) =>
            items.ElementAt(_rnd.Next(0, items.Count()));

        /// <inheritdoc cref="Random"/>
        public static T Random<T>(this IList<T> items) =>
            items[_rnd.Next(0, items.Count)];
    }
}
