using AutoMapper;
using Webjet_Movies_Backend.Models.DTO;
using Webjet_Movies_Backend.Models.ViewModels;

namespace Webjet_Movies_Backend.Mappers
{
    public class CinemaWorldMapperProfile: Profile
    {
        public CinemaWorldMapperProfile() 
        {
            CreateMap<CinemaWorldMovieDetailsDTO, MovieViewModel>();
            CreateMap<MovieViewModel, CinemaWorldMovieDetailsDTO>();
        }
    }
}
