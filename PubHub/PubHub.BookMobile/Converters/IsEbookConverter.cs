﻿using System.Globalization;
using CommunityToolkit.Maui.Converters;
using PubHub.Common.Models.ContentTypes;

namespace PubHub.BookMobile.Converters
{
    public class IsEbookConverter : BaseConverterOneWay<ContentTypeInfoModel, bool>
    {
        public override bool DefaultConvertReturnValue { get; set; }

        public override bool ConvertFrom(ContentTypeInfoModel value, CultureInfo? culture)
        {
            return value is not null && value.Name.ToUpperInvariant() == "EBOOK";
        }
    }
}
