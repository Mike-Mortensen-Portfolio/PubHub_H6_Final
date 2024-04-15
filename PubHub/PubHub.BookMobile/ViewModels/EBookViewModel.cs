using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.ErrorSpecifications;
using PubHub.Common.Services;
using VersOne.Epub;

namespace PubHub.BookMobile.ViewModels
{
    public partial class EBookViewModel : NavigationObject, IQueryAttributable
    {
        private const int START_OF_BOOK = 0;
        public const string CONTENT_QUERY_NAME = "Content";

        private readonly IEpubReaderService _reader;
        private EpubBook _ebook = null!;

        [ObservableProperty]
        private bool _isBusy = false;
        [ObservableProperty]
        private string? _chapter;
        [ObservableProperty]
        private int _currentChapter;
        [ObservableProperty]
        private string? _webContent;
        [ObservableProperty]
        private string? _title;

        public EBookViewModel(IEpubReaderService reader)
        {
            _reader = reader;
        }

        public Func<ScrollView> ScrollView { get; set; } = null!;
        public bool IsNotFirstChapter => CurrentChapter != START_OF_BOOK;
        public bool IsNotLastChapter => CurrentChapter != (_ebook?.ReadingOrder?.Count ?? 0);

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
            Title = _ebook.Title;
            CurrentChapter = START_OF_BOOK;
            OnCurrentChapterChanged(CurrentChapter);
            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(IsNotLastChapter))]
        public async Task GoNext()
        {
            if (ScrollView is null)
                throw new NullReferenceException($"{nameof(ScrollView)} can't be null. Did you forget to set when loading the page?");

            IsBusy = true;
            CurrentChapter++;

            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            await ScrollView.Invoke().ScrollToAsync(0, 0, false);
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(IsNotFirstChapter))]
        public async Task GoBack()
        {
            if (ScrollView is null)
                throw new NullReferenceException($"{nameof(ScrollView)} can't be null. Did you forget to set when loading the page?");

            IsBusy = true;
            CurrentChapter--;

            var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            if (!contentResult.IsSuccess || contentResult.Instance is null)
            {
                //  TODO (MSM): Handle erorr
            }

            WebContent = contentResult.Instance;
            await ScrollView.Invoke().ScrollToAsync(0, 0, false);
            IsBusy = false;
        }

        partial void OnCurrentChapterChanged(int value)
        {
            //  TODO (MSM): Save current chapter progress
            GoNextCommand.NotifyCanExecuteChanged();
            GoBackCommand.NotifyCanExecuteChanged();
            Chapter = $"Section {CurrentChapter + 1}";
        }
    }
}
