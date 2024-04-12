using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Models;
using PubHub.Common.Models.Books;

namespace PubHub.BookMobile.ViewModels
{
    public partial class BookInfoViewModel : ObservableObject, IQueryAttributable
    {
        public const string BOOK_LISTING_QUERY_NAME = "BookListing";

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

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookListing = (BookListingViewModel)query[BOOK_LISTING_QUERY_NAME];

            CurrentViewedItem = ((BookListing.EBookInStock) ? (BookListing.EBook) : (BookListing.AudioBook))!;
            IsEBook = CurrentViewedItem.ContentType.Name.ToUpperInvariant() == "EBOOK";
            IsAudioBook = !IsEBook;
        }

        [RelayCommand(CanExecute = nameof(IsAudioBook))]
        public void ShowEbookBook()
        {
            CurrentViewedItem = BookListing.EBook!;
            IsEBook = true;
            IsAudioBook = false;
        }

        [RelayCommand(CanExecute = nameof(IsEBook))]
        public void ShowAudiobook()
        {
            CurrentViewedItem = BookListing.EBook!;
            IsEBook = false;
            IsAudioBook = true;
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
