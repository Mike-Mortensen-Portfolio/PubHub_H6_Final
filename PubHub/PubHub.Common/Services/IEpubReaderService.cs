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
        Task<EpubBook> GetEpubBook(byte[] epubContent);
        Task<string> GetCurrentBookChapterAsync(byte[] epubContent);
        string DisplayChapterContent(int chapterIndex, EpubBook epubBook);        
    }
}
