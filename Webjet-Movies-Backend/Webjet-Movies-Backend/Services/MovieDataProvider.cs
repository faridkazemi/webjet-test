using RedisLibrary;
using Webjet_Movies_Backend.Services.Interfaces;

namespace Webjet_Movies_Backend.Services
{
    public class MovieDataProvider<T>: IMovieDataProvider<T>
    {
        private IRedisService _redisService;
        private ILogger<MovieDataProvider<T>> _logger;

        public MovieDataProvider(IRedisService redisService, ILogger<MovieDataProvider<T>> logger)
        {
            _redisService = redisService;
            _logger = logger;
        }

        public async Task<T> GetMovies(string key)
        {
            try
            {
                var result = await _redisService.GetAsync<T>(key);

                return result;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Faild to get movies from server: {key}");
                throw;
            }
        }
    }
}
