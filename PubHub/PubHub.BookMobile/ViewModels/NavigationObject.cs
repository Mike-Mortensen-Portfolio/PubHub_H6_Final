using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Models;

namespace PubHub.BookMobile.ViewModels
{
    /// <summary>
    /// A base class for <see langword="objects"/> that can navigate between pages
    /// </summary>
    public abstract partial class NavigationObject : ObservableObject
    {

        /// <summary>
        /// Navigate to a page specified by <paramref name="navInfo"/> with the provided <see cref="PageInfo.Parameters"/>
        /// </summary>
        /// <param name="navInfo"></param>
        /// <returns>The <see cref="Task"/> that represents the <see langword="asynchronous"/> operation</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [RelayCommand]
        public static async Task NavigateToPageWithParemeters(PageInfo navInfo)
        {
            if (navInfo is null)
                throw new ArgumentNullException(nameof(navInfo), "Page route can't be null");

            await Shell.Current.GoToAsync(navInfo.PageName, navInfo.Parameters);
        }

        /// <summary>
        /// Navigate to a page specified by <paramref name="pageName"/>
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns>The <see cref="Task"/> that represents the <see langword="asynchronous"/> operation</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [RelayCommand]
        public static async Task NavigateToPage(string pageName)
        {
            if (pageName is null)
                throw new ArgumentNullException(nameof(pageName), "Page route can't be null");

            await Shell.Current.GoToAsync(pageName);
        }
    }
}
