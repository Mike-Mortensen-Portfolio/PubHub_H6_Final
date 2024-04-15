using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.ErrorSpecifications;
using PubHub.Common.Services;
using VersOne.Epub;

namespace PubHub.BookMobile.ViewModels
{
    public partial class EBookViewModel : NavigationObject, IQueryAttributable
    {
        public const string CONTENT_QUERY_NAME = "Content";
        private readonly IEpubReaderService _reader;
        private EpubBook _ebook = null!;

        [ObservableProperty]
        private bool _isBusy = false;
        [ObservableProperty]
        private string? _chapter;
        [ObservableProperty]
        private int _currentChapter = 0;
        [ObservableProperty]
        private string? _webContent;

        public EBookViewModel(IEpubReaderService reader)
        {
            _reader = reader;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsBusy = true;
            if (query[CONTENT_QUERY_NAME] is not byte[] rawBytes || !rawBytes.GetType().IsArray)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                return;
            }

            var result = await _reader.GetEpubBookAsync(rawBytes);
            if (!result.IsSuccess || result.Instance is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                return;
            }

            _ebook = result.Instance;
            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            IsBusy = false;
        }

        [RelayCommand]
        public void GoNext()
        {
            IsBusy = true;
            CurrentChapter++;

            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            IsBusy = false;
        }

        [RelayCommand]
        public void GoBack()
        {
            IsBusy = true;
            CurrentChapter--;

            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            IsBusy = false;
        }
    }
}
