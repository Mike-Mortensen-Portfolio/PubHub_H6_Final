using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.Models;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Models.Books;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class BookInfoViewModel : NavigationObject, IQueryAttributable
    {
        public const string BOOK_LISTING_QUERY_NAME = "BookListing";
        private readonly IUserService _userService;
        private readonly IBookService _bookService;

        [ObservableProperty]
        private BookListingViewModel _bookListing = null!;
        [ObservableProperty]
        private BookInfoModel _currentViewedItem = null!;
        [ObservableProperty]
        private bool _isEBook = true;
        [ObservableProperty]
        private bool _isAudioBook = false;
        [ObservableProperty]
        private bool _isAuthenticated = false;
        [ObservableProperty]
        private bool _alreadyOwnsBook;
        private bool CanBuyBook => IsAuthenticated && !AlreadyOwnsBook;
        [ObservableProperty]
        private string? _buttonText;
        [ObservableProperty]
        private bool _isBusy;

        public BookInfoViewModel(IUserService userService, IBookService bookService)
        {
            _userService = userService;
            _bookService = bookService;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var book = query[BOOK_LISTING_QUERY_NAME];
            if (!book.GetType().IsAssignableTo(typeof(BookListingViewModel)))
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                return;
            }

            BookListing = (BookListingViewModel)book;

            CurrentViewedItem = ((BookListing.EBookInStock) ? (BookListing.EBook) : (BookListing.AudioBook))!;
            IsEBook = CurrentViewedItem.ContentType.Name.ToUpperInvariant() == "EBOOK";
            IsAudioBook = !IsEBook;
        }

        [RelayCommand(CanExecute = nameof(IsAudioBook))]
        public async Task ShowEbookBook()
        {
            IsBusy = true;
            if (BookListing.EBook is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                IsBusy = false;
                return;
            }

            CurrentViewedItem = BookListing.EBook;
            IsEBook = true;
            IsAudioBook = false;

            var result = await _userService.GetUserBooksAsync(User.Id!.Value);

            if (!result.IsSuccess)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                await NavigateToPage("..");

                IsBusy = false;
                return;
            }

            AlreadyOwnsBook = User.Id.HasValue && (result.Instance?.Any(book => book.Id == CurrentViewedItem.Id) ?? false);
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(IsEBook))]
        public async Task ShowAudiobook()
        {
            IsBusy = true;
            if (BookListing.AudioBook is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");
                IsBusy = false;
                return;
            }

            CurrentViewedItem = BookListing.AudioBook;
            IsEBook = false;
            IsAudioBook = true;

            var result = await _userService.GetUserBooksAsync(User.Id!.Value);

            if (!result.IsSuccess)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                await NavigateToPage("..");
                IsBusy = false;
                return;
            }

            AlreadyOwnsBook = User.Id.HasValue && (result.Instance?.Any(book => book.Id == CurrentViewedItem.Id) ?? false);
            ButtonText = ((AlreadyOwnsBook) ? ("Owned") : ("Buy"));
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(CanBuyBook))]
        public async Task BuyBook()
        {
            IsBusy = true;
            var result = await _bookService.PurchaseBookAsync(CurrentViewedItem.Id);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);

                IsBusy = false;
                return;
            }

            AlreadyOwnsBook = true;
            await Shell.Current.CurrentPage.DisplayAlert("Success", $"Purchase Complete", "OK");
            IsBusy = false;
        }

        partial void OnIsEBookChanged(bool value)
        {
            ShowEbookBookCommand.NotifyCanExecuteChanged();
            ShowAudiobookCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsAudioBookChanged(bool value)
        {
            OnIsEBookChanged(value);
        }

        partial void OnAlreadyOwnsBookChanged(bool value)
        {
            ButtonText = ((value) ? ("Owned") : ("Buy"));
            BuyBookCommand.NotifyCanExecuteChanged();
        }
    }
}
