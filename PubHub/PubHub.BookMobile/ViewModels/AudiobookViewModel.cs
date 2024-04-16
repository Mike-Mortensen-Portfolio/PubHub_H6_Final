using Android.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using PubHub.BookMobile.Auth;
using PubHub.Common;
using PubHub.Common.ErrorSpecifications;
using PubHub.Common.Models.Users;
using PubHub.Common.Services;

namespace PubHub.BookMobile.ViewModels
{
    public partial class AudiobookViewModel : NavigationObject, IQueryAttributable
    {
        public const string BOOK_ID_QUERY_NAME = "BookId";
        public const string COVER_IMAGE_QUERY_NAME = "CoverImage";

        private readonly IAudioManager _audioManager;
        private readonly IBookService _bookService;
        private readonly IUserService _userService;
        private IAudioPlayer? _audioPlayer = null!;
        private Guid _bookId;
        private bool _isPaused = false;

        [ObservableProperty]
        private bool _isBusy;
        [ObservableProperty]
        private byte[] coverImage = null!;
        [ObservableProperty]
        private string _totalTimeString = string.Empty;
        [ObservableProperty]
        private double _volume = 0f;
        [ObservableProperty]
        private double _totalDuration;
        [ObservableProperty]
        private double _currentPosition = 0f;
        [ObservableProperty]
        private string _timeLeft = string.Empty;

        public AudiobookViewModel(IAudioManager audioManager, IBookService bookService, IUserService userService)
        {
            _audioManager = audioManager;
            _bookService = bookService;
            _userService = userService;
        }

        public bool CanPlay => _audioPlayer is null || !_audioPlayer.IsPlaying;
        public bool CanPause => (_audioPlayer is not null) && _audioPlayer.IsPlaying && !_isPaused;
        public bool CanStop => (_audioPlayer is not null) && (_audioPlayer.IsPlaying || _isPaused);

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsBusy = true;
            Guid idResult = IntegrityConstants.INVALID_ENTITY_ID;
            if ((query[COVER_IMAGE_QUERY_NAME] is not byte[] rawBytes || !rawBytes.GetType().IsArray) || query[BOOK_ID_QUERY_NAME] is not null && !Guid.TryParse(query[BOOK_ID_QUERY_NAME].ToString(), out idResult))
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                return;
            }

            CoverImage = rawBytes;
            _bookId = idResult;
            IsBusy = false;
        }

        [RelayCommand(CanExecute = nameof(CanPlay))]
        public async Task PlayAudio()
        {
            if (_audioPlayer is null)
                await Task.Run(async () =>
                {
                    _audioPlayer = _audioManager.CreatePlayer((await _bookService.GetBookStreamAsync(_bookId)).Instance!);
                    Volume = _audioPlayer.Volume;
                    TotalDuration = _audioPlayer.Duration;
                    TotalTimeString = ((_audioPlayer is not null) ? ($"{_audioPlayer?.Duration:00:00}") : ("00:00"));
                });

            _audioPlayer!.Play();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                while (_audioPlayer.IsPlaying)
                {
                    CurrentPosition = _audioPlayer.CurrentPosition;
                }
            }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _isPaused = false;
            PlayAudioCommand.NotifyCanExecuteChanged();
            PauseAudioCommand.NotifyCanExecuteChanged();
            StopAudioCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanPause))]
        public void PauseAudio()
        {
            if (_audioPlayer is null)
                return;

            _audioPlayer.Pause();
            _isPaused = true;
            PlayAudioCommand.NotifyCanExecuteChanged();
            PauseAudioCommand.NotifyCanExecuteChanged();
            StopAudioCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanStop))]
        public void StopAudio()
        {
            if (_audioPlayer is null)
                return;

            _audioPlayer.Stop();
            CurrentPosition = 0f;

            _isPaused = false;
            PlayAudioCommand.NotifyCanExecuteChanged();
            PauseAudioCommand.NotifyCanExecuteChanged();
            StopAudioCommand.NotifyCanExecuteChanged();
        }

        public async Task UpdateProgress()
        {
            IsBusy = true;

            if (_audioPlayer is null)
                return;

            var progressInProcentage = CurrentPosition / TotalDuration * 100f;

            var result = await _userService.UpdateBookProgressAsync(User.Id!.Value, new UserBookUpdateModel
            {
                bookId = _bookId,
                ProgressInProcent = (float)progressInProcentage
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

        partial void OnVolumeChanged(double value)
        {
            if (_audioPlayer is null)
                return;

            _audioPlayer!.Volume = value;
        }

        partial void OnCurrentPositionChanged(double value)
        {
            if (_audioPlayer is null)
                return;

            var timeLeft = _audioPlayer.Duration - _audioPlayer.CurrentPosition;
            TimeLeft = $"{timeLeft:00:00}";
        }
    }
}
