using System.Diagnostics;
using System.Text;
using VersOne.Epub;

namespace PubHub.Common.Services
{
    public class EpubReaderService : IEpubReaderService
    {
        // A thing to be aware of here, is that the first few pages can contain only little information and maybe images, which haven't been dealt with for now.

        /// <summary>
        /// Retrieves the <see cref="EpubBook"/> from the <see cref="byte[]"/> with the book content stored in the db.
        /// </summary>
        /// <param name="epubContent"><see cref="byte[]"/> containing the book content.</param>
        /// <returns>A <see cref="ServiceResult{TResult}"/> with how the request was handled.</returns>
        public async Task<ServiceResult<EpubBook>> GetEpubBookAsync(byte[] epubContent)
        {
            try
            {
                using MemoryStream stream = new MemoryStream(epubContent);
                var epubBook = await EpubReader.ReadBookAsync(stream);
                return new ServiceResult<EpubBook>(epubBook, errorDescriptor: "Successfully retrieved the Epub book!");
            }
            catch (Exception ex)
            {
                return new ServiceResult<EpubBook>(null, errorDescriptor: $"Unable to retrieve epub book: {ex.Message}");
            }
            
        }

        /// <summary>
        /// Retrieves a current book chapter when a user is changing pages or starting a new e-book.
        /// </summary>
        /// <param name="epubBook">The <see cref="EpubBook"/> containing all information about a book.</param>
        /// <returns>A <see cref="ServiceResult{TResult}"/> with how the request was handled.</returns>
        public ServiceResult<string> GetCurrentBookChapter(int currentChapterIndex, EpubBook epubBook)
        {
            try
            {
                var currentChapter = DisplayChapterContent(currentChapterIndex, epubBook);              
                return new ServiceResult<string>(currentChapter.Instance, errorDescriptor: "Successfully retrieved the current chapter.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving the current chapter: {ex.Message}");
                return new ServiceResult<string>(string.Empty, errorDescriptor: $"Error retrieving the current chapter: {ex.Message}.");
            }
        }

        /// <summary>
        /// Gets the next chapter, the interation of chapterIndex++ should happen before calling this method.
        /// </summary>
        /// <param name="chapterIndex">The next chapter index.</param>
        /// <param name="epubBook">The <see cref="EpubBook"/> which needs to be read from.</param>
        /// <returns></returns>
        public ServiceResult<string> GetChapter(int chapterIndex, EpubBook epubBook)
        {
            try
            {
                var nextChapter = DisplayChapterContent(chapterIndex, epubBook);
                return new ServiceResult<string>(nextChapter.Instance, errorDescriptor: "Successfully retrieved the next chapter.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving the next chapter: {ex.Message}");
                return new ServiceResult<string>(string.Empty, errorDescriptor: $"Error retrieving the next chapter: {ex.Message}.");
            }
        }

        /// <summary>
        /// Gets the current chapter of the <see cref="EpubBook"/> and then builds up a string to display in the mobile.
        /// </summary>
        /// <param name="chapterIndex">Chapter index to get the next one.</param>
        /// <param name="epubBook"></param>
        /// <returns>A <see cref="ServiceResult{TResult}"/> with how the request was handled.</returns>
        public ServiceResult<string> DisplayChapterContent(int chapterIndex, EpubBook epubBook)
        {
            try
            {
                if (epubBook != null && chapterIndex >= 0 && chapterIndex < epubBook.ReadingOrder.Count)
                {
                    var chapter = epubBook.ReadingOrder[chapterIndex];
                    var htmlContent = new StringBuilder();
                    var stringHtml = htmlContent.Append(chapter.Content).ToString();
                    return new ServiceResult<string>(stringHtml, errorDescriptor: $"Successfully retrieved the content of the book.");
                }
                else
                {
                    return new ServiceResult<string>(string.Empty, $"Unable to retrieve the content of the book.");
                }
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>(string.Empty, $"Unable to retrieve the content of the book: {ex.Message}.");
            }            
        }
    }
}
