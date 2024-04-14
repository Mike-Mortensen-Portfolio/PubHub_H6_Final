using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.ErrorSpecifications;
using PubHub.BookMobile.Models;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class PersonalLibraryViewModel : NavigationObject
    {
        private const string BOOK_CONTENT_INFO_PAGE_NAME = "BookContentInfo";
        private readonly IUserService _userService;
        private readonly IGenreService _genreService;

        [ObservableProperty]
        private bool _isBusy = false;
        [ObservableProperty]
        private string _searchTerm = string.Empty;
        [ObservableProperty]
        private bool _isGenreVisible = false;
        [ObservableProperty]
        private BookQuery _query = new();
        [ObservableProperty]
        private bool _isAuthenticated = false;
        [ObservableProperty]
        private bool _hasBooksToShow;

        public PersonalLibraryViewModel(IUserService userService, IGenreService genreService)
        {
            _userService = userService;
            _genreService = genreService;
            IsAuthenticated = User.IsAuthenticated;
        }

        public ObservableCollection<GenreInfoModel> Genres { get; } = [];
        public ObservableCollection<BookListingViewModel> Books { get; private set; } = [];

        [RelayCommand]
        public async Task NavigateToBookInfo(BookListingViewModel bookListing)
        {
            var parameters = new Dictionary<string, object>
            {
                {
                    BookContentInfoViewModel.BOOK_LISTING_QUERY_NAME, bookListing
                }
            };

            await NavigateToPageWithParemetersCommand.ExecuteAsync(new PageInfo { RouteName = BOOK_CONTENT_INFO_PAGE_NAME, Parameters = parameters });

            return;
        }

        [RelayCommand]
        public async Task FetchBooks()
        {
            Books.Clear();
            IsBusy = true;

            var result = await _userService.GetUserBooksAsync(User.Id!.Value, Query);

            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                HasBooksToShow = Books.Any();
                IsBusy = false;
                return;
            }

            var books = result.Instance!;
            foreach (var book in books)
            {
                var existingBookListing = Books.FirstOrDefault(b => b.Title == book.Title && b.PublicationDate == book.PublicationDate);

                var bookListing = existingBookListing ?? new BookListingViewModel
                {
                    PublicationDate = book.PublicationDate,
                    Title = book.Title
                };

                if (book.ContentType.Name.ToUpperInvariant() == "EBOOK")
                    bookListing.EBook = book;
                else
                    bookListing.AudioBook = book;

                if (existingBookListing == null)
                    Books.Add(bookListing);
            }

            HasBooksToShow = Books.Any();
            IsBusy = false;
        }

        [RelayCommand]
        public async Task FetchGenres()
        {
            IsBusy = true;
            Genres.Clear();

            var result = await _genreService.GetAllGenresAsync();

            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                IsBusy = false;
                return;
            }

            var genres = result.Instance!;
            genres = [.. genres.OrderBy(genre => genre.Name)];

            foreach (var genre in genres)
            {
                Genres.Add(genre);
            }

            IsBusy = false;
        }

        [RelayCommand]
        public void ToggleGenreView()
        {
            IsGenreVisible = !IsGenreVisible;
        }

        partial void OnSearchTermChanged(string value)
        {
            Query.SearchKey = value;

            if (value == string.Empty)
                FetchBooksCommand.Execute(null);
        }
    }
}
