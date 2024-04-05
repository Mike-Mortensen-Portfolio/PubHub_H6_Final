using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<List<AuthorInfoModel>> GetAuthorsAsync();
        Task<AuthorInfoModel?> GetAuthorAsync(Guid authorId);
        Task<ServiceInstanceResult<AuthorInfoModel>> AddAuthorAsync(AuthorCreateModel authorCreateModel);
        Task<ServiceResult> DeleteAuthorAsync(Guid authorId);
    }
}
