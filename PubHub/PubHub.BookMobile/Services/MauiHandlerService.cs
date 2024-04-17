using PubHub.BookMobile.Extensions;

namespace PubHub.BookMobile.Services
{
    /// <summary>
    /// Defines a set of custom style mappings for Maui controls
    /// </summary>
    internal static class MauiHandlerService
    {
        /// <summary>
        /// Call this to map all custom mapping
        /// </summary>
        public static void AppendCustomMapping()
        {
            AppenCustomEntryMapping();
            AppendCustomSearchBarMapping();
        }

        public static void AppenCustomEntryMapping()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = ((Entry)view).BackgroundColorOrNoUnderline();
                handler.PlatformView.SetPadding(10, 0, 10, 0);
            });
        }

        public static void AppendCustomSearchBarMapping()
        {
            Microsoft.Maui.Handlers.SearchBarHandler.Mapper.AppendToMapping(nameof(SearchBar), (handler, view) =>
            {
                handler.PlatformView.BackgroundTintList = ((SearchBar)view).BackgroundColorOrNoUnderline();
            });
        }
    }
}
