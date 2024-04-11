using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;

namespace PubHub.Common.Services
{
    public interface IPublisherService
    {
        Task<ServiceResult<PublisherInfoModel>> AddPublisherAsync(PublisherCreateModel publisherCreateModel);
        Task<ServiceResult<PublisherInfoModel>> GetPublisherInfoAsync(Guid publisherId);
        Task<ServiceResult<IReadOnlyList<BookInfoModel>>> GetAllPublisherBooksAsync(Guid publisherId);
        Task<ServiceResult<PublisherInfoModel>> UpdatePublisherAsync(Guid publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<ServiceResult<IReadOnlyList<PublisherInfoModel>>> GetAllPublishersAsync(PublisherQuery queryOptions);
        Task<ServiceResult> DeletePublisherAsync(Guid publisherId);
    }
}
