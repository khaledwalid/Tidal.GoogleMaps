using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Tidal.GoogleMaps.Contracts.Contracts;
using Tidal.GoogleMaps.DB.Models;
using Tidal.GoogleMaps.Models.dto;

namespace Tidal.GoogleMaps.Business.Services
{
    public class MapService : IMapService
    {
        private readonly IRepository<Map> _mapRepository;
        private readonly IMapper _mapper;

        public MapService(IRepository<Map> mapRepository , IMapper mapper)
        {
            _mapRepository = mapRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<Map> AddNewMarker(CreateUpdateMarkerDto createUpdateMarkerDto)
        {
            var map = new Map
            {
                Name = createUpdateMarkerDto.Name,
                Description = createUpdateMarkerDto.Description,
                Latitude = createUpdateMarkerDto.Latitude,
                Longitude = createUpdateMarkerDto.Longitude
            };
            await _mapRepository.CreateAsync(map);
            var result = await _mapRepository.GetAsync(a => a.Id == map.Id);
            return result;
        }

        [HttpGet]
        public async Task<List<Map>> GetAllMarkers()
        {
            var maps = await _mapRepository.FindAllAsync(null);
            return maps;
        }
    }


    }
