using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<List<AuthorInfoModel>> GetAuthors();
        Task<AuthorInfoModel?> GetAuthor(Guid authorId);
        Task<ServiceInstanceResult<AuthorInfoModel>> AddAuthor(AuthorCreateModel authorCreateModel);
        Task<ServiceResult> DeleteAuthor(Guid authorId);
    }
}
