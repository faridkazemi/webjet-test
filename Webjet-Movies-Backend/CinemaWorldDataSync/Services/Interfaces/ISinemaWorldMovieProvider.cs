using CinemaWorldDataSync.DTO;

namespace CinemaWorldDataSync.Services.Interfaces
{
    public interface ISinemaWorldMovieProvider
    {
        public Task<CinemaWorldMoviesDTO> FetchMoviesAsync(CancellationToken cancellationToken);
        public Task<CinemaWorldMovieDetailsDTO> FetchMoviesDetailsAsync(string id, CancellationToken cancellationToken);
        public Task<List<CinemaWorldMovieDetailsDTO>> GetFullMovieDeatilsAsync(CancellationToken cancellationToken);
    }
}
