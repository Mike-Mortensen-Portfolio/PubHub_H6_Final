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
        Task<ServiceInstanceResult<PublisherCreateModel>> AddPublisherAsync(PublisherCreateModel publisherCreateModel);
        Task<List<PublisherInfoModel>> GetPublishersAsync(PublisherQuery queryOptions);
        Task<PublisherInfoModel?> GetPublisherInfoAsync(Guid publisherId);
        Task<List<BookInfoModel>> GetPublisherBooksAsync(Guid publisherId);
        Task<ServiceInstanceResult<PublisherUpdateModel>> UpdatePublisherAsync(Guid publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<ServiceResult> DeletePublisherAsync(Guid publisherId);
    }
}
