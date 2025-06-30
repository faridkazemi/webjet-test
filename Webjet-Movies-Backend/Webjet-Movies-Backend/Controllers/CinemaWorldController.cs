using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Webjet_Movies_Backend.Models;
using Webjet_Movies_Backend.Models.DTO;
using Webjet_Movies_Backend.Models.ViewModels;
using Webjet_Movies_Backend.Services.Interfaces;


namespace Webjet_Movies_Backend.Controllers
{
    [ApiController]
    [Route("api/v{v:apiVersion}/movies")]
    public class CinemaWorldMovieController : ControllerBase
    {
        private readonly ILogger<CinemaWorldMovieController> _logger;
        private readonly IMovieDataProvider<List<CinemaWorldMovieDetailsDTO>> _cinemaMoveDataProvider;
        private readonly IMovieDataProvider<List<FilmWorldMovieDetailsDTO>> _filmMoveDataProvider;
        private readonly IMapper _mapper;

        public CinemaWorldMovieController(ILogger<CinemaWorldMovieController> logger,
            IMovieDataProvider<List<CinemaWorldMovieDetailsDTO>> cinemaMovieDataProvider,
            IMovieDataProvider<List<FilmWorldMovieDetailsDTO>> filmMovieDataProvider,
            IMapper mapper)
        {
            _logger = logger;
            _cinemaMoveDataProvider = cinemaMovieDataProvider;
            _filmMoveDataProvider = filmMovieDataProvider;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAll")]
        public async Task<List<MovieViewModel>> Get([FromQuery] PagingParameters pagingParams)
        {
            _logger.LogInformation($"Fetching movies ... ");

            // TODO Move the key to the config values
            var task1 = _cinemaMoveDataProvider.GetMovies("movies:cinemaWorldMovies");
            var task2 = _filmMoveDataProvider.GetMovies("movies:filmWorldMovies");

            await Task.WhenAll(task1, task2);

            var cinemaMovies = await task1;
            var filmMovies = await task2;

            var cinemaViewModel = _mapper.Map<List<MovieViewModel>>(cinemaMovies);
            var filmViewModel = _mapper.Map<List<MovieViewModel>>(filmMovies);

            var resultViewModel = cinemaViewModel.Concat(filmViewModel).OrderBy(x => x.Price).ToList();


            return resultViewModel;
        }
    }
}
