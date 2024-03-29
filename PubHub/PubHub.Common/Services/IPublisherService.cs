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
        Task<PublisherInfoModel?> GetPublisherInfo(int publisherId);
        Task<List<BookInfoModel>> GetPublisherBooks(int publisherId);
        Task<ServiceInstanceResult<PublisherUpdateModel>> UpdatePublisher(int publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<ServiceResult> DeletePublisher(int publisherId);
    }
}
