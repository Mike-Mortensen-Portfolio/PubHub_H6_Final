using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Converters;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.BookMobile.Converters
{
    public class IsEbookConverter : BaseConverterOneWay<ContentTypeInfoModel, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; }

        public override bool ConvertFrom(ContentTypeInfoModel value, CultureInfo? culture)
        {
            return value.Name.ToUpperInvariant() == "EBOOK";
        }
    }
}
