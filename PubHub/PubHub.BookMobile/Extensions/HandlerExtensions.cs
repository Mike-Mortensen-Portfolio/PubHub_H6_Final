using Android.Content.Res;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace PubHub.BookMobile.Extensions
{
    internal static class HandlerExtensions
    {
        public static ColorStateList BackgroundColorOrNoUnderline<TView>(this TView view) where TView : VisualElement
        {
            /*
                Either transparent or the provided background color
                Source: https://github.com/dotnet/maui/issues/7906#issuecomment-1793075906
            */
            return view.BackgroundColor.IsDefault() ? ColorStateList.ValueOf(Colors.Transparent.ToAndroid())
            : ColorStateList.ValueOf(view.BackgroundColor.ToAndroid());
        }
    }
}
