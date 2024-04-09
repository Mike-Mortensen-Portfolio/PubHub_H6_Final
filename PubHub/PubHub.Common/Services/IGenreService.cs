using PubHub.Common.Models.Genres;

namespace PubHub.Common.Services
{
    public interface IGenreService
    {
        Task<ServiceResult<IReadOnlyList<GenreInfoModel>>> GetAllGenresAsync();
        [Obsolete($"Use {nameof(GetAllGenresAsync)} instead.")]
        Task<IReadOnlyList<GenreInfoModel>> GetGenresAsync();
        Task<ServiceResult<GenreInfoModel>> GetGenreAsync(Guid genreId);
        Task<ServiceResult<GenreCreateModel>> AddGenreAsync(GenreCreateModel genreCreateModel);
        Task<ServiceResult> DeleteGenreAsync(Guid genreId);
    }
}
