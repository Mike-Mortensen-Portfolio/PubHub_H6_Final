using System.Globalization;
using CommunityToolkit.Maui.Converters;
using PubHub.Common.Models.ContentTypes;
using Xamarin.Google.Crypto.Tink.Shaded.Protobuf;

namespace PubHub.BookMobile.Converters
{
    public class IsAudiobookConverter : BaseConverterOneWay<ContentTypeInfoModel, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; }

        public override bool ConvertFrom(ContentTypeInfoModel value, CultureInfo? culture)
        {
            return value is not null && value.Name.ToUpperInvariant() == "AUDIOBOOK";
        }
    }
}
