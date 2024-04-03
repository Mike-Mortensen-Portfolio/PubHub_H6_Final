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
        Task<List<GenreInfoModel>> GetGenres();
        Task<GenreInfoModel?> GetGenre(Guid genreId);
        Task<ServiceInstanceResult<GenreCreateModel>> AddGenre(GenreCreateModel genreCreateModel);
        Task<ServiceResult> DeleteGenre(Guid genreId);
    }
}
