using CommunityToolkit.Mvvm.ComponentModel;
using PubHub.Common.ErrorSpecifications;

namespace PubHub.BookMobile.ViewModels
{
    public partial class AudiobookViewModel : NavigationObject, IQueryAttributable
    {
        [ObservableProperty]
        private bool _isBusy;

        public const string CONTENT_QUERY_NAME = "Content";
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsBusy = true;
            if (query[CONTENT_QUERY_NAME] is not byte[] rawBytes || !rawBytes.GetType().IsArray)
            {
                await Shell.Current.CurrentPage.DisplayAlert(NotFoundError.TITLE, NotFoundError.ERROR_MESSAGE, NotFoundError.BUTTON_TEXT);

                await NavigateToPage("..");

                return;
            }

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
