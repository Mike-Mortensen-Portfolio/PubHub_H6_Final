using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection;

namespace PubHub.API.Domain.Extensions
{
    internal static class EntityTypeBuilderExtensions
    {
        public static EntityTypeBuilder<T> ConfigureId<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class
        {
            if(typeof(T).GetProperty("Id") is PropertyInfo propertyInfo)
            {
                entityTypeBuilder.Property(propertyInfo.PropertyType, propertyInfo.Name)
                    .HasColumnName($"{nameof(T)}Id");
                entityTypeBuilder.HasKey(propertyInfo.Name);
            }
    
            return entityTypeBuilder;
        }
    }
}
