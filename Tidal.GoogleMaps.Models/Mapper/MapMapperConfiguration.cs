using AutoMapper;
using Tidal.GoogleMaps.Models.dto;
using Tidal.GoogleMaps.Models.Models;

namespace Tidal.GoogleMaps.Models.Mapper
{
    public class MapMapperConfiguration : Profile
    {
        public MapMapperConfiguration()
        {
            CreateMap<Map, CreateUpdateMarkerDto>();
            CreateMap<CreateUpdateMarkerDto, Map>();
        }
    }
}