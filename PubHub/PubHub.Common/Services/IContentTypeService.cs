using PubHub.Common.Models.ContentTypes;

namespace PubHub.Common.Services
{
    public interface IContentTypeService
    {
        Task<HttpServiceResult<IReadOnlyList<ContentTypeInfoModel>>> GetAllContentTypesAsync();
    }
}
