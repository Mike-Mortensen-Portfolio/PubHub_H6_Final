using PubHub.Common.Models.ContentTypes;

namespace PubHub.Common.Services
{
    public interface IContentTypeService
    {
        Task<List<ContentTypeInfoModel>> GetContentTypesAsync();
    }
}
