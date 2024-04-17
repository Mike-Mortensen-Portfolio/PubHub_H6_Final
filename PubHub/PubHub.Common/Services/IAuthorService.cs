using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<HttpServiceResult<IReadOnlyList<AuthorInfoModel>>> GetAllAuthorsAsync();
        Task<HttpServiceResult<AuthorInfoModel>> GetAuthorAsync(Guid authorId);
        Task<HttpServiceResult<AuthorInfoModel>> AddAuthorAsync(AuthorCreateModel authorCreateModel);
        Task<HttpServiceResult> DeleteAuthorAsync(Guid authorId);
    }
}
