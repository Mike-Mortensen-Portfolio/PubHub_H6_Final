using VersOne.Epub;

namespace PubHub.Common.Services
{
    public interface IEpubReaderService
    {
        Task<ServiceResult<EpubBook>> GetEpubBookAsync(byte[] epubContent);
        ServiceResult<string> GetChapter(int currentChapterIndex, EpubBook epubBook);
        ServiceResult<string> DisplayChapterContent(int chapterIndex, EpubBook epubBook);        
    }
}
