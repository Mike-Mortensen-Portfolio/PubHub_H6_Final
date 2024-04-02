using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Models.Authors;

namespace PubHub.Common.Services
{
    public interface IAuthorService
    {
        Task<List<AuthorInfoModel>> GetAuthors();
        Task<AuthorInfoModel?> GetAuthor(Guid authorId);
        Task<ServiceInstanceResult<AuthorCreateModel>> AddAuthor(AuthorCreateModel authorCreateModel);
        Task<ServiceResult> DeleteAuthor(Guid authorId);
    }
}
