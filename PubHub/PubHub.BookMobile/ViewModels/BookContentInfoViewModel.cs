using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.BookMobile.Models;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Models.Books;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class BookContentInfoViewModel : NavigationObject, IQueryAttributable
    {
        public const string BOOK_LISTING_QUERY_NAME = "BookListing";
        private readonly IUserService _userService;
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
        private string? _buttonText;
        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private UserBookContentModel? _currentContent;
        [ObservableProperty]
        private string? _aquiredDate;
        [ObservableProperty]
        private string? _ProgressInProcent;

        public BookContentInfoViewModel(IUserService userService)
        {
            _userService = userService;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsBusy = true;
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

            var result = await _userService.GetUserBookContentAsync(User.Id!.Value, CurrentViewedItem.Id);
            if (!result.IsSuccess || result.Instance is null)
            {
                //  TODO (MSM): Add error handling
            }

            CurrentContent = result.Instance;
            AquiredDate = CurrentContent!.AcquireDate.ToString("dd/MM/yyyy");
            ProgressInProcent = $"{CurrentContent!.ProgressInProcent:00.00}%";

            ButtonText = ((IsEBook) ? ("Read now") : ("Listen now"));
            IsBusy = false;
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

            var result = await _userService.GetUserBookContentAsync(User.Id!.Value, CurrentViewedItem.Id);
            if (!result.IsSuccess || result.Instance is null)
            {
                //  TODO (MSM): Add error handling
            }

            CurrentContent = result.Instance;
            AquiredDate = CurrentContent!.AcquireDate.ToString("dd/MM/yyyy");
            ProgressInProcent = $"{CurrentContent!.ProgressInProcent}%";

            ButtonText = "Read now";
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

            var result = await _userService.GetUserBookContentAsync(User.Id!.Value, CurrentViewedItem.Id);
            if (!result.IsSuccess || result.Instance is null)
            {
                //  TODO (MSM): Add error handling
            }

            CurrentContent = result.Instance;
            AquiredDate = CurrentContent!.AcquireDate.ToString("dd/MM/yyyy");
            ProgressInProcent = $"{CurrentContent!.ProgressInProcent}%";

            ButtonText = "Listen now";
            IsBusy = false;
        }

        [RelayCommand]
        public async Task EngageContent()
        {
            if (IsEBook)
                await NavigateToPageWithParemetersCommand.ExecuteAsync(new PageInfo
                {
                    RouteName = "EBookView",
                    Parameters = new Dictionary<string, object>
                    {
                        { "Content", CurrentContent!.BookContent! },
                        { "BookId", CurrentViewedItem.Id }
                    }
                });
            if (IsAudioBook)
                await NavigateToPageWithParemetersCommand.ExecuteAsync(new PageInfo
                {
                    RouteName = "AudiobookView",
                    Parameters = new Dictionary<string, object>
                    {
                        { "Content", CurrentContent!.BookContent! },
                        { "BookId", CurrentViewedItem.Id }
                    }
                });
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
    }
}
