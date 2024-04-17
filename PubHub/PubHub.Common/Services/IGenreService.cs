using PubHub.Common.Models.Genres;

namespace PubHub.Common.Services
{
    public interface IGenreService
    {
        Task<HttpServiceResult<IReadOnlyList<GenreInfoModel>>> GetAllGenresAsync();
        Task<HttpServiceResult<GenreInfoModel>> GetGenreAsync(Guid genreId);
        Task<HttpServiceResult<GenreCreateModel>> AddGenreAsync(GenreCreateModel genreCreateModel);
        Task<HttpServiceResult> DeleteGenreAsync(Guid genreId);
    }
}
