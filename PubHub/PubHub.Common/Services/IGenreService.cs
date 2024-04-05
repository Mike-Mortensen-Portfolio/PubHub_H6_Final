using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubHub.Common.Models.Genres;

namespace PubHub.Common.Services
{
    public interface IGenreService
    {
        Task<List<GenreInfoModel>> GetGenresAsync();
        Task<GenreInfoModel?> GetGenreAsync(Guid genreId);
        Task<ServiceInstanceResult<GenreCreateModel>> AddGenreAsync(GenreCreateModel genreCreateModel);
        Task<ServiceResult> DeleteGenreAsync(Guid genreId);
    }
}
