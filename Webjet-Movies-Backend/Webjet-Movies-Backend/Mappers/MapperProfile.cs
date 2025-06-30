using AutoMapper;
using Webjet_Movies_Backend.Models.DTO;
using Webjet_Movies_Backend.Models.ViewModels;

namespace Webjet_Movies_Backend.Mappers
{
    public class MapperProfile: Profile
    {
        public MapperProfile() 
        {
            CreateMap<CinemaWorldMovieDetailsDTO, MovieViewModel>();
            CreateMap<FilmWorldMovieDetailsDTO, MovieViewModel>();
        }
    }
}
