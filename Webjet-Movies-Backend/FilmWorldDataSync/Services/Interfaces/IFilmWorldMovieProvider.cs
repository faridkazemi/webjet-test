using FilmWorldDataSync.DTO;

namespace filmWorldDataSync.Services.Interfaces
{
    public interface IFilmWorldMovieProvider
    {
        public Task<FilmWorldMoviesDTO> FetchMoviesAsync(CancellationToken cancellationToken);
        public Task<FilmWorldMovieDetailsDTO> FetchMoviesDetailsAsync(string id, CancellationToken cancellationToken);
        public Task<List<FilmWorldMovieDetailsDTO>> GetFullMovieDeatilsAsync(CancellationToken cancellationToken);
    }
}
