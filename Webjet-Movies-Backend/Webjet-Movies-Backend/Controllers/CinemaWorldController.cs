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
        private readonly IMovieDataProvider<List<CinemaWorldMovieDetailsDTO>> _moveDataProvider;
        private readonly IMapper _mapper;

        public CinemaWorldMovieController(ILogger<CinemaWorldMovieController> logger,
            IMovieDataProvider<List<CinemaWorldMovieDetailsDTO>> movieDataProvider,
            IMapper mapper)
        {
            _logger = logger;
            _moveDataProvider = movieDataProvider;
            _mapper = mapper;
        }

        [HttpGet(Name = "GetAll")]
        public async Task<List<MovieViewModel>> Get([FromQuery] PagingParameters pagingParams)
        {
            //return new List<CinemaWorldMovieDTO>();
            // TODO Move the key to the config values

            _logger.LogInformation($"Fetching movies ... ");
            //return new List<CinemaWorldMovieDTO>();
            var result = await _moveDataProvider.GetMovies("movies:cinemaWorldMovies");

            var resultViewModel = _mapper.Map<List<MovieViewModel>>(result);

            return resultViewModel;
            //var students = _dbContext.Students
            //    .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
            //    .Take(pagingParams.PageSize)
            //    .ToList();

            //return Ok(students);
        }
    }
}
