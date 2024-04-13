using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersOne.Epub;

namespace PubHub.Common.Services
{
    public interface IEpubReaderService
    {
        Task<ServiceResult<EpubBook>> GetEpubBook(byte[] epubContent);
        ServiceResult<string> GetCurrentBookChapterAsync(EpubBook epubBook);
        ServiceResult<string> DisplayChapterContent(int chapterIndex, EpubBook epubBook);        
    }
}
