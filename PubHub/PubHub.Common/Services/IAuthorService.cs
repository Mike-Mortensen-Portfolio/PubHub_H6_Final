using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<ServiceResult<IReadOnlyList<AuthorInfoModel>>> GetAllAuthorsAsync();
        Task<ServiceResult<AuthorInfoModel>> GetAuthorAsync(Guid authorId);
        Task<ServiceResult<AuthorInfoModel>> AddAuthorAsync(AuthorCreateModel authorCreateModel);
        Task<ServiceResult> DeleteAuthorAsync(Guid authorId);
    }
}
