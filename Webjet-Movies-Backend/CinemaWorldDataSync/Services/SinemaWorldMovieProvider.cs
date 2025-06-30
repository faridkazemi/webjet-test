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
        public async Task<CinemaWorldMoviesDTO> FetchMoviesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching movies from CinemaWorldMovies...");

                //var response = new CinemaWorldMoviesDTO();
                //response.Movies = new List<CinemaWorldMovieDTO> { new CinemaWorldMovieDTO {ID="111", Title="AAAA" } };
                var response = await _httpClientFactory.CreateClient(_configs.CinemaMovieHttpClientName)
                    .GetFromJsonAsync<CinemaWorldMoviesDTO>("movies", cancellationToken);

                return response;
            }
            catch(Exception ex) 
            {
                // We don't throw any exception. We just want to log it.
                _logger.LogError(ex, $"Faild to Get Movies from server.");
                return new CinemaWorldMoviesDTO();
            }
        }

        public async Task<CinemaWorldMovieDetailsDTO> FetchMoviesDetailsAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching movie details from CinemaWorldMovies. movieId: {id}...");

                //var response = new CinemaWorldMovieDetailsDTO { ID = "111", Title = "AAAA" };

                // TODO added for testing and needs to be removed
                var client = _httpClientFactory.CreateClient(_configs.CinemaMovieHttpClientName);

                var requestUri = new Uri(client.BaseAddress, $"movie/{id}");

                Console.WriteLine($"************************** {requestUri}****");
                
                var response = await _httpClientFactory.CreateClient(_configs.CinemaMovieHttpClientName)
                    .GetFromJsonAsync<CinemaWorldMovieDetailsDTO>($"movie/{id}", cancellationToken);
                
                return response;
            }
            catch (Exception ex)
            {
                // We don't throw any exception. We just want to log it.
                _logger.LogError(ex, $"Faild to Get Movies from server. movieId: {id}");
                return new CinemaWorldMovieDetailsDTO();
            }
        }

        public async Task<List<CinemaWorldMovieDetailsDTO>> GetFullMovieDeatilsAsync(CancellationToken cancellationToken)
        {
            var movieSummary = await FetchMoviesAsync(cancellationToken);

            if (movieSummary == null || movieSummary.Movies.Count() == 0)
            {
                return new List<CinemaWorldMovieDetailsDTO>();
            }
            else
            {
                var movies = movieSummary.Movies;

                // To handle possible API rate limiting and manage performance and scalability
                // I decided to use SemaphoreSlim to limit the number of concurrent parallel calls. 
                // it is more useful in the real world scenarios where we might have many movies
                var semaphore = new SemaphoreSlim(10);

                var tasks = movies.Select(async movie =>
                {
                    await semaphore.WaitAsync();
                    var movieDetails = new CinemaWorldMovieDetailsDTO();

                    try
                    {
                        movieDetails = await FetchMoviesDetailsAsync(movie.ID, cancellationToken);

                        // Since CinemaWorldMovieDetails contains all the fields of CinemaWrldMovie, no mapping and data merging required.
                        return movieDetails;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Faild to get full movie details.");

                        // If there is any problem with fetching movie details from the server
                        // we can retun the least data we have from the summary api
                        return new CinemaWorldMovieDetailsDTO
                        {
                            ID = movie.ID,
                            Title = movie.Title,
                            Year = movie.Year,
                            Type = movie.Type,
                            Poster = movie.Poster,
                        };
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                var result = await Task.WhenAll(tasks);

                return result.ToList();
            }
        }
    }
}
