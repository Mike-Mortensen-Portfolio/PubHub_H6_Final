using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<IReadOnlyList<AuthorInfoModel>> GetAuthorsAsync();
        Task<ServiceResult<AuthorInfoModel>> GetAuthorAsync(Guid authorId);
        Task<ServiceResult<AuthorInfoModel>> AddAuthorAsync(AuthorCreateModel authorCreateModel);
        Task<ServiceResult> DeleteAuthorAsync(Guid authorId);
    }
}
