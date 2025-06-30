using filmWorldDataSync.Services.Interfaces;
using FilmWorldDataSync.DTO;
using FilmWorldDataSync.OptionModels;
using FilmWorldDataSync.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace FilmWorldDataSync.Services
{
    public class FilmWorldMovieProvider: IFilmWorldMovieProvider
    {
        private FilmWorldConfigurationOption _configs;
        private IHttpClientFactory _httpClientFactory;
        private ILogger<FilmWorldMovieProvider> _logger;

        public FilmWorldMovieProvider(
            IOptions<FilmWorldConfigurationOption> configOption,
            IHttpClientFactory httpClientFactory,
            ILogger<FilmWorldMovieProvider> logger) 
        {
            _configs = configOption.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        public async Task<FilmWorldMoviesDTO> FetchMoviesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching movies from FilmWorldMovies...");

                //var response = new FilmWorldMoviesDTO();
                //response.Movies = new List<FilmWorldMovieDTO> { new FilmWorldMovieDTO {ID="111", Title="AAAA" } };
                var response = await _httpClientFactory.CreateClient(_configs.FilmMovieHttpClientName)
                    .GetFromJsonAsync<FilmWorldMoviesDTO>("movies", cancellationToken);

                return response;
            }
            catch(Exception ex) 
            {
                // We don't throw any exception. We just want to log it.
                _logger.LogError(ex, $"Faild to Get Movies from server.");
                return new FilmWorldMoviesDTO();
            }
        }

        public async Task<FilmWorldMovieDetailsDTO> FetchMoviesDetailsAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching movie details from FilmWorldMovies. movieId: {id}...");

                //var response = new FilmWorldMovieDetailsDTO { ID = "111", Title = "AAAA" };

                // TODO added for testing and needs to be removed
                var client = _httpClientFactory.CreateClient(_configs.FilmMovieHttpClientName);

                var requestUri = new Uri(client.BaseAddress, $"movie/{id}");

                Console.WriteLine($"************************** {requestUri}****");
                
                var response = await _httpClientFactory.CreateClient(_configs.FilmMovieHttpClientName)
                    .GetFromJsonAsync<FilmWorldMovieDetailsDTO>($"movie/{id}", cancellationToken);
                
                return response;
            }
            catch (Exception ex)
            {
                // We don't throw any exception. We just want to log it.
                _logger.LogError(ex, $"Faild to Get Movies from server. movieId: {id}");
                return new FilmWorldMovieDetailsDTO();
            }
        }

        public async Task<List<FilmWorldMovieDetailsDTO>> GetFullMovieDeatilsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var movieSummary = await FetchMoviesAsync(cancellationToken);

                if (movieSummary == null || movieSummary.Movies == null || movieSummary.Movies?.Count() == 0)
                {
                    return new List<FilmWorldMovieDetailsDTO>();
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
                        var movieDetails = new FilmWorldMovieDetailsDTO();

                        try
                        {
                            movieDetails = await FetchMoviesDetailsAsync(movie.ID, cancellationToken);

                            // Since FilmWorldMovieDetails contains all the fields of FilmWrldMovie, no mapping and data merging required.
                            return movieDetails;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Faild to get full movie details.");

                            // If there is any problem with fetching movie details from the server
                            // we can retun the least data we have from the summary api
                            return new FilmWorldMovieDetailsDTO
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
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to get movie details.");
                return new List<FilmWorldMovieDetailsDTO>();
            }
        }
    }
}
