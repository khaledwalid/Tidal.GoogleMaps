using System.Collections.Generic;
using System.Threading.Tasks;
using Tidal.GoogleMaps.DB.Models;
using Tidal.GoogleMaps.Models.dto;

namespace Tidal.GoogleMaps.Contracts.Contracts
{
    public interface IMapService
    {
        Task<Map> AddNewMarker(CreateUpdateMarkerDto createUpdateMarkerDto);
        Task<List<Map>> GetAllMarkers();
    }
}