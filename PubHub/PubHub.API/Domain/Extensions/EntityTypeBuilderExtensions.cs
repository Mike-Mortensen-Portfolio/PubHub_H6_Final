using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PubHub.API;

internal static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<T> ConfigureId<T>(this EntityTypeBuilder<T> entityTypeBuilder) where T : class
    {
        if(entityTypeBuilder.GetType().GetProperty("Id") != null)
        {

        }

        return entityTypeBuilder;
    }
}
