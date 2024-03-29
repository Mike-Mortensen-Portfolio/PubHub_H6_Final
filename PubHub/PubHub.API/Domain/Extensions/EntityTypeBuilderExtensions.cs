using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace PubHub.API.Domain.Extensions
{
    internal static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Configures the ID of the entity to be the <see cref="Type"/> of <typeparamref name="T"/> followed by 'Id' (<i>Ex. <typeparamref name="T"/>Id</i>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityTypeBuilder"></param>
        /// <returns>The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the ID</returns>
        public static EntityTypeBuilder<T> ConfigureId<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class
        {
            if (typeof(T).GetProperty("Id") is PropertyInfo propertyInfo)
            {
                var type = typeof(T);
                entityTypeBuilder.Property(propertyInfo.PropertyType, propertyInfo.Name)
                    .HasColumnName($"{type.Name}Id");
                entityTypeBuilder.HasKey(propertyInfo.Name);
            }

            return entityTypeBuilder;
        }

        /// <summary>
        /// Configures the table name of the entity to be <see cref="Type"/> of <typeparamref name="T"/> followed by an 's' to signify a collection (<i>Ex. <typeparamref name="T"/>s</i>)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityTypeBuilder"></param>
        /// <returns>The <see cref="EntityTypeBuilder{TEntity}"/> used to configure the table name</returns>
        public static EntityTypeBuilder<T> TypeToPluralTableName<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class
        {
            var type = typeof(T);
            return entityTypeBuilder.ToTable($"{type.Name}s");
        }
    }
}
