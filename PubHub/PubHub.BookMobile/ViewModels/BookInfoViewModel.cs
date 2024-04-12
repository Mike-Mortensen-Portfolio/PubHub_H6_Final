using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.Models;
using PubHub.Common;
using PubHub.Common.Models.Books;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class BookInfoViewModel : ObservableObject, IQueryAttributable
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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookListing = (BookListingViewModel)query[BOOK_LISTING_QUERY_NAME];

            CurrentViewedItem = ((BookListing.EBookInStock) ? (BookListing.EBook) : (BookListing.AudioBook))!;
            IsEBook = CurrentViewedItem.ContentType.Name.ToUpperInvariant() == "EBOOK";
            IsAudioBook = !IsEBook;
        }

        [RelayCommand(CanExecute = nameof(IsAudioBook))]
        public async Task ShowEbookBook()
        {
            CurrentViewedItem = BookListing.EBook!;
            IsEBook = true;
            IsAudioBook = false;
            AlreadyOwnsBook = ((User.Id.HasValue) && (await _userService.GetUserBooksAsync(User.Id!.Value)).Instance!.Any(book => book.Id == CurrentViewedItem.Id));
        }

        [RelayCommand(CanExecute = nameof(IsEBook))]
        public async Task ShowAudiobook()
        {
            CurrentViewedItem = BookListing.AudioBook!;
            IsEBook = false;
            IsAudioBook = true;
            AlreadyOwnsBook = ((User.Id.HasValue) && (await _userService.GetUserBooksAsync(User.Id!.Value)).Instance!.Any(book => book.Id == CurrentViewedItem.Id));
        }

        [RelayCommand(CanExecute = nameof(CanBuyBook))]
        public async Task BuyBook()
        {
            IsBusy = true;
            var result = await _bookService.PurchaseBookAsync(CurrentViewedItem.Id);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve books. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.UNAUTHORIZED}", "OK");
                else
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Couldn't retrieve books. Please try again, or contact PubHub support if the problem persists{Environment.NewLine}Error: {ErrorsCodeConstants.NO_CONNECTION}", "OK");

                IsBusy = false;
                return;
            }

            AlreadyOwnsBook = true;
            await Application.Current!.MainPage!.DisplayAlert("Success", $"Purchase Complete", "OK");
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
