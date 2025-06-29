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
                var response = await _httpClientFactory.CreateClient(_configs.CinemaMovieHttpClientName)
                    .GetFromJsonAsync<CinemaWorldMoviesDTO>("", cancellationToken);

                return response;
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Faild to Get Movies from server.");
                throw;
            }
        }
    }
}
