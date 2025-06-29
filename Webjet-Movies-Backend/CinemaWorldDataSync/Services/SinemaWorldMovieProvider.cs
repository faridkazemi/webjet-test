using CinemaWorldDataSync.DTO;
using CinemaWorldDataSync.OptionModels;
using CinemaWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace CinemaWorldDataSync.Services
{
    public class SinemaWorldMovieProvider: ISinemaWorldMovieProvider
    {
        private CinemaWorldConfigurationOption _configs;
        private IHttpClientFactory _httpClientFactory;
        private ILogger<SinemaWorldMovieProvider> _logger;

        public SinemaWorldMovieProvider(
            IOptions<CinemaWorldConfigurationOption> configOption,
            IHttpClientFactory httpClientFactory,
            ILogger<SinemaWorldMovieProvider> logger) 
        {
            _configs = configOption.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<CinemaWorldMoviesDTO> GetMoviesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching movies from CinemaWorldMovies...");

                var response = new CinemaWorldMoviesDTO();
                response.Movies = new List<CinemaWorldMovieDTO> { new CinemaWorldMovieDTO {ID="111", Title="AAAA" } };
                //var response = await _httpClientFactory.CreateClient(_configs.CinemaMovieHttpClientName)
                //    .GetFromJsonAsync<CinemaWorldMoviesDTO>("", cancellationToken);

                return response;
            }
            catch(Exception ex) 
            {
                // We don't throw any exception. We just want to log it.
                _logger.LogError(ex, $"Faild to Get Movies from server.");
                return new CinemaWorldMoviesDTO();
            }
        }
    }
}
