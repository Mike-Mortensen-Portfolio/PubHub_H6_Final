using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Extensions
{
    public static class GenericExtensions
    {
        /// <summary>
        /// Converts this <typeparamref name="TObject"/> into a query representation of itself
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="obj"></param>
        /// <param name="ignoreNull">Whether or not <see langword="null"/> values should be excluded from the query <see langword="string"/></param>
        /// <param name="enforceLowercase">Whether or not property names should be formatted as <strong>lowercase</strong> characters</param>
        /// <returns>A <see langword="string"/> that represents <paramref name="obj"/> as query parameters</returns>
        public static string ToQuery<TObject>(this TObject obj, bool ignoreNull = false, bool enforceLowercase = false) where TObject : new()
        {
            string queryString = string.Empty;
            var properties = typeof(TObject).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = ((enforceLowercase) ? (properties[i].Name.ToLowerInvariant()) : (properties[i].Name));
                var propertyValue = properties[i].GetValue(obj);

                if (ignoreNull && propertyValue == null)
                    continue;

                queryString += $"{propertyName}={propertyValue}{((i + 1 < properties.Length) ? ("&") : (string.Empty))}";
            }

            return queryString;
        }
    }
}
