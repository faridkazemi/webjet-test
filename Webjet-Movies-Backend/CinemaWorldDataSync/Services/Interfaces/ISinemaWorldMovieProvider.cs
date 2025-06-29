using CinemaWorldDataSync.DTO;

namespace CinemaWorldDataSync.Services.Interfaces
{
    public interface ISinemaWorldMovieProvider
    {
        public Task<CinemaWorldMoviesDTO> GetMoviesAsync(CancellationToken cancellationToken);
    }
}
