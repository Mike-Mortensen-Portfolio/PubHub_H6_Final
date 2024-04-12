using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Models;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class LibraryViewModel : NavigationObject
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        [ObservableProperty]
        private bool _isBusy = false;
        [ObservableProperty]
        private string _searchTerm = string.Empty;
        [ObservableProperty]
        private bool _isGenreVisible = false;
        [ObservableProperty]
        private BookQuery _query;
        [ObservableProperty]
        private bool _isAuthenticated;
        [ObservableProperty]
        private bool _hasBooksToShow;

        public LibraryViewModel(IBookService bookService, IGenreService genreService)
        {
            _bookService = bookService;
            _genreService = genreService;
            Query = new BookQuery();
        }

        public ObservableCollection<GenreInfoModel> Genres { get; } = [];
        public ObservableCollection<BookListingViewModel> Books { get; private set; } = [];

        [RelayCommand]
        public void ToggleGenreView()
        {
            IsGenreVisible = !IsGenreVisible;
        }

        [RelayCommand]
        public async Task FetchBooks()
        {
            Books.Clear();
            IsBusy = true;

            var result = await _bookService.GetAllBooksAsync(Query);

            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve books. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
                else
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve books. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");

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
        public async Task NavigateToBookInfo(BookListingViewModel bookListing)
        {
            var parameters = new Dictionary<string, object>
            {
                {
                    BookInfoViewModel.BOOK_LISTING_QUERY_NAME, bookListing
                }
            };

            await NavigateToPageWithParemetersCommand.ExecuteAsync(new PageInfo { PageName = "BookInfo", Parameters = parameters });

            return;
        }

        public async Task FetchGenres()
        {
            IsBusy = true;
            Genres.Clear();

            var result = await _genreService.GetAllGenresAsync();

            if (!result.IsSuccess || result.Instance is null)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve genres. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
                else
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve genres. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");

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

        partial void OnSearchTermChanged(string value)
        {
            Query.SearchKey = value;

            if (value == string.Empty)
                FetchBooksCommand.Execute(null);
        }
    }
}
