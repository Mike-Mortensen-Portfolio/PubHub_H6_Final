using System.Globalization;
using CommunityToolkit.Maui.Converters;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.BookMobile.Converters
{
    public class IsAudiobookConverter : BaseConverterOneWay<ContentTypeInfoModel, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; }

        public override bool ConvertFrom(ContentTypeInfoModel value, CultureInfo? culture)
        {
            return value.Name.ToUpperInvariant() == "AUDIOBOOK";
        }
    }
}
