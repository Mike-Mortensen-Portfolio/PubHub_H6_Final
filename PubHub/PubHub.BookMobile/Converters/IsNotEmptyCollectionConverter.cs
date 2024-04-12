using System.Collections;
using System.Globalization;
using CommunityToolkit.Maui.Converters;

namespace PubHub.BookMobile.Converters
{
    public class IsNotEmptyCollectionConverter : BaseConverterOneWay<IList, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; } = false;

        public override bool ConvertFrom(IList value, CultureInfo? culture)
        {
            return value != null && value.Count > 0;
        }
    }
}
