using PubHub.Common.Models.Books;
using PubHub.Common.Models.Publishers;

namespace PubHub.Common.Services
{
    public interface IPublisherService
    {
        Task<HttpServiceResult<PublisherInfoModel>> AddPublisherAsync(PublisherCreateModel publisherCreateModel);
        Task<HttpServiceResult<PublisherInfoModel>> GetPublisherInfoAsync(Guid publisherId);
        Task<HttpServiceResult<IReadOnlyList<BookInfoModel>>> GetAllPublisherBooksAsync(Guid publisherId);
        Task<HttpServiceResult<BookContentModel>> GetPublisherBookContentAsync(Guid publisherId, Guid bookId);
        Task<HttpServiceResult<PublisherInfoModel>> UpdatePublisherAsync(Guid publisherId, PublisherUpdateModel publisherUpdateModel);
        Task<HttpServiceResult<IReadOnlyList<PublisherInfoModel>>> GetAllPublishersAsync(PublisherQuery queryOptions);
        Task<HttpServiceResult> DeletePublisherAsync(Guid publisherId);
    }
}
