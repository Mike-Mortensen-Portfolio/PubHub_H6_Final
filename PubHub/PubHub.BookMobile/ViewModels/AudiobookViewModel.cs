using Android.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using PubHub.Common;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class AudiobookViewModel : NavigationObject, IQueryAttributable
    {
        public const string BOOK_ID_QUERY_NAME = "BookId";
        public const string CONTENT_QUERY_NAME = "Content";

        private readonly IAudioManager _audioManager;
        private readonly IBookService _bookService;

        private Guid _bookId;

        [ObservableProperty]
        private bool _isBusy;

        public AudiobookViewModel(IAudioManager audioManager, IBookService bookService)
        {
            _audioManager = audioManager;
            _bookService = bookService;
        }

        [RelayCommand]
        public async Task PlayAudio()
        {
            await Task.Run(async () =>
            {
                var audioPlayer = _audioManager.CreatePlayer((await _bookService.GetBookStreamAsync(_bookId)).Instance!);

                audioPlayer.Play();
            });
        }

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

            _bookId = idResult;

            //var result = await _reader.GetEpubBookAsync(rawBytes);
            //if (!result.IsSuccess || result.Instance is null)
            //{
            //    await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

            //    await NavigateToPage("..");

            //    return;
            //}

            //_ebook = result.Instance;
            //Title = _ebook.Title;
            //CurrentChapter = START_OF_BOOK;
            //OnCurrentChapterChanged(CurrentChapter);
            //var contentResult = _reader.GetChapter(CurrentChapter, _ebook);
            //if (!contentResult.IsSuccess || contentResult.Instance is null)
            //{
            //    //  TODO (MSM): Handle erorr
            //}

            //WebContent = contentResult.Instance;
            IsBusy = false;
        }
    }
}
