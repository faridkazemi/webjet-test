﻿using filmWorldDataSync.Services.Interfaces;
using FilmWorldDataSync.DTO;
using FilmWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.Logging;
using RedisLibrary;

namespace FilmWorldDataSync.Services
{
    public class SyncDataService: ISyncDataService
    {
        private ILogger<SyncDataService> _logger;
        private IFilmWorldMovieProvider _movieProvider;
        private IRedisService _redisService;

        public SyncDataService(
            ILogger<SyncDataService> logger,
            IFilmWorldMovieProvider movieProvider,
            IRedisService redisService)
        {
            _logger = logger;
            _movieProvider = movieProvider;
            _redisService = redisService;
        }

        /// <summary>
        /// A background job that can be scheduled to fetch the movies from FilmWorldMovie server
        /// and save them in redis cache.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RunAsync(CancellationToken cancellationToken, int interval)
        {
            var counter = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (counter >= 1 && interval == 0)
                    {
                        break;
                    }

                    var serverResponseMovie = await _movieProvider.GetFullMovieDeatilsAsync(cancellationToken);

                    if (serverResponseMovie != null && serverResponseMovie.Count() > 0) 
                    {
                        
                        // TODO move the key to the config values
                        await _redisService.SetAsync("movies:filmWorldMovies", serverResponseMovie);
                        
                    }

                    counter++;
                }
                catch (Exception ex)
                {
                    // We don't want to throw exception. Just log and go.
                    _logger.LogError(ex, $"Faild to run FilmWorldDataSync");
                }

                // To schedule the serviece to fetch data and save in redis every 5 minut.
                // TODO move it to config values.
                await Task.Delay(TimeSpan.FromSeconds(interval), cancellationToken);
            }
            
        }
    }
}
