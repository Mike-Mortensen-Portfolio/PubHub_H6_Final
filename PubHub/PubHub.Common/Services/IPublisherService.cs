using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;
using PubHub.Common.Models.Users;

namespace PubHub.Common.Services
{
    public interface IPublisherService
    {
        Task<ServiceResult<PublisherInfoModel>> AddPublisherAsync(PublisherCreateModel publisherCreateModel);
        Task<ServiceResult<PublisherInfoModel>> GetPublisherInfoAsync(Guid publisherId);
        Task<IReadOnlyList<BookInfoModel>> GetPublisherBooksAsync(Guid publisherId);
        Task<ServiceResult<PublisherInfoModel>> UpdatePublisherAsync(Guid publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<List<PublisherInfoModel>> GetPublishersAsync(PublisherQuery queryOptions);
        Task<ServiceResult> DeletePublisherAsync(Guid publisherId);
    }
}
