using Tidal.GoogleMaps.Contracts.Contracts;
using Tidal.GoogleMaps.DB.Context;
using Tidal.GoogleMaps.DB.Models;

namespace Tidal.GoogleMaps.Infrastructure.Repositories
{
    public class MapRepository : Repository<Map> , IMapRepository
    {
        public MapRepository(GoogleMapsDbContext dbContext) : base(dbContext)
        {
        }
    }
}