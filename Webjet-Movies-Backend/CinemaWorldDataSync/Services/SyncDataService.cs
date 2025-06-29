using CinemaWorldDataSync.DTO;
using CinemaWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.Logging;
using RedisLibrary;

namespace CinemaWorldDataSync.Services
{
    public class SyncDataService: ISyncDataService
    {
        private ILogger<SyncDataService> _logger;
        private ISinemaWorldMovieProvider _movieProvider;
        private IRedisService _redisService;

        public SyncDataService(
            ILogger<SyncDataService> logger,
            ISinemaWorldMovieProvider movieProvider,
            IRedisService redisService)
        {
            _logger = logger;
            _movieProvider = movieProvider;
            _redisService = redisService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var test = new CinemaWorldMoviesDTO();
                    test.Movies = new List<CinemaWorldMovieDTO> { new CinemaWorldMovieDTO { ID="111", Title="asdfaf" } };
                    await _redisService.SetAsync("movies:cinemaWorldMovies", test.Movies);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Faild to run CinemaWorldDataSync");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            
        }
    }
}
