using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tidal.GoogleMaps.Contracts.Contracts;
using Tidal.GoogleMaps.Models.dto;

namespace Tidal.GoogleMaps.Controllers
{
    public class MapController : Controller
    {
        private readonly IMapService _mapService;

        public MapController(IMapService mapService)
        {
            _mapService = mapService;
        }
        public async Task<IActionResult> AddMarker(string name, string description, string lat, string lng)
        {
            var mapDto = new CreateUpdateMarkerDto
            {
                Description = description,
                Latitude = lat,
                Longitude = lng,
                Name = name
            };
            var newMarker = await _mapService.AddNewMarker(mapDto);
            return Json("");
        }

        public async Task<IActionResult> GetAllMarkers()
        {
            var maps = await _mapService.GetAllMarkers();
            return Json(maps);
        }
    }
}