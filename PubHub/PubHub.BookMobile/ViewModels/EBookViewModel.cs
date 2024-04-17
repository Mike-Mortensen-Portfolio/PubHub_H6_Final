using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Models.Users;
using PubHub.Common.Services;
using VersOne.Epub;

namespace PubHub.BookMobile.ViewModels
{
    public partial class EBookViewModel : NavigationObject, IQueryAttributable
    {
        private const int START_OF_BOOK = 0;
        public const string CONTENT_QUERY_NAME = "Content";
        public const string BOOK_ID_QUERY_NAME = "BookId";

        private readonly IEpubReaderService _reader;
        private readonly IUserService _userService;
        private EpubBook _ebook = null!;
        private Guid _bookId;

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

        public EBookViewModel(IEpubReaderService reader, IUserService userService)
        {
            _reader = reader;
            _userService = userService;
        }

        public Func<ScrollView> ScrollView { get; set; } = null!;
        public bool IsNotFirstChapter => CurrentChapter != START_OF_BOOK;
        public bool IsNotLastChapter => CurrentChapter != (_ebook?.ReadingOrder?.Count ?? 0);

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsBusy = true;
            Guid idResult = IntegrityConstants.INVALID_ENTITY_ID;
            if ((query[CONTENT_QUERY_NAME] is not byte[] rawBytes || !rawBytes.GetType().IsArray) || (query[BOOK_ID_QUERY_NAME] is not null && !Guid.TryParse(query[BOOK_ID_QUERY_NAME].ToString(), out idResult)))
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
            _bookId = idResult;
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

        public async Task UpdateProgress()
        {
            IsBusy = true;
            var progressInProcentage = (float)CurrentChapter / (float)_ebook.ReadingOrder.Count * 100f;

            var result = await _userService.UpdateBookProgressAsync(User.Id!.Value, new UserBookUpdateModel
            {
                bookId = _bookId,
                ProgressInProcent = progressInProcentage
            });

            if (!result.IsSuccess)
            {
                if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    await Shell.Current.CurrentPage.DisplayAlert(UnauthorizedError.TITLE, UnauthorizedError.ERROR_MESSAGE, UnauthorizedError.BUTTON_TEXT);
                else
                    await Shell.Current.CurrentPage.DisplayAlert(NoConnectionError.TITLE, NoConnectionError.ERROR_MESSAGE, NoConnectionError.BUTTON_TEXT);
            }
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
            GoNextCommand.NotifyCanExecuteChanged();
            GoBackCommand.NotifyCanExecuteChanged();
            Chapter = $"Section {CurrentChapter + 1}";
        }
    }
}
