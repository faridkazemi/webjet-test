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
                    var serverResponseMovie = await _movieProvider.GetMoviesAsync(cancellationToken);

                    if (serverResponseMovie != null && serverResponseMovie.Movies.Count() > 0) 
                    {
                        // TODO move the key to the config values
                        await _redisService.SetAsync("movies:cinemaWorldMovies", serverResponseMovie);
                    }
                    
                }
                catch (Exception ex)
                {
                    // We don't want to throw exception. Just log and go.
                    _logger.LogError(ex, $"Faild to run CinemaWorldDataSync");
                }

                // To schedule the serviece to fetch data and save in redis every 5 minut.
                // TODO move it to config values.
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
            
        }
    }
}
