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
        Task<ServiceInstanceResult<PublisherCreateModel>> AddPublisher(PublisherCreateModel publisherCreateModel);
        Task<PublisherInfoModel?> GetPublisherInfo(Guid publisherId);
        Task<List<BookInfoModel>> GetPublisherBooks(Guid publisherId);
        Task<ServiceInstanceResult<PublisherUpdateModel>> UpdatePublisher(Guid publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<ServiceResult> DeletePublisher(Guid publisherId);
    }
}
