using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PubHub.API.Domain.Seeding
{
    /// <summary>
    /// Serves as the <see langword="base"/> of any <see cref="IEntityTypeConfiguration{TEntity}"/> that targets data seeding
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity seed</typeparam>
    /// <typeparam name="TKey">The type of the indexable key used to find seed entries</typeparam>
    public abstract class SeedBase<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The entity of type <typeparamref name="TEntity"/> that matches the <typeparamref name="TKey"/> value</returns>
        public abstract TEntity this[TKey key] { get; }
        /// <summary>
        /// Contains the seeded <typeparamref name="TEntity"/> data
        /// </summary>
        public IReadOnlyList<TEntity> Seeds { get; protected set; } = [];

        public abstract void Configure(EntityTypeBuilder<TEntity> builder);
    }
}
