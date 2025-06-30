using CinemaWorldDataSync.DTO;

namespace CinemaWorldDataSync.Services.Interfaces
{
    public interface ICinemaWorldMovieProvider
    {
        public Task<CinemaWorldMoviesDTO> FetchMoviesAsync(CancellationToken cancellationToken);
        public Task<CinemaWorldMovieDetailsDTO> FetchMoviesDetailsAsync(string id, CancellationToken cancellationToken);
        public Task<List<CinemaWorldMovieDetailsDTO>> GetFullMovieDeatilsAsync(CancellationToken cancellationToken);
    }
}
