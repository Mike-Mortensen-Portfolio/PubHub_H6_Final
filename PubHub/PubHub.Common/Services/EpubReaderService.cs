using System.Diagnostics;
using System.Text;
using VersOne.Epub;
using Xamarin.Essentials;

namespace PubHub.Common.Services
{
    public class EpubReaderService : IEpubReaderService
    {
        private EpubBook? _epubBook;
        private int _currentChapterIndex = 0;

        public async Task<ServiceResult<EpubBook>> GetEpubBook(byte[] epubContent)
        {
            using (MemoryStream stream = new MemoryStream(epubContent))
            {
                return _epubBook = await EpubReader.ReadBookAsync(stream);
            }
        }

        public async Task<string> GetCurrentBookChapterAsync(EpubBook epubBook)
        {
            try
            {
                var currentChapter = DisplayChapterContent(_currentChapterIndex, epubBook);
                return currentChapter;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading the ePub: {ex.Message}");
                return string.Empty;
            }
        }

        public string DisplayChapterContent(int chapterIndex, EpubBook epubBook)
        {
            if (epubBook != null && chapterIndex >= 0 && chapterIndex < epubBook.ReadingOrder.Count)
            {
                var chapter = epubBook.ReadingOrder[chapterIndex];
                var htmlContent = new StringBuilder();
                return htmlContent.Append(chapter.Content).ToString();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
