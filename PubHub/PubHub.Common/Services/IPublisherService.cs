using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;

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
