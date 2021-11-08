﻿
using Tidal.GoogleMaps.Models.Models;

namespace Tidal.GoogleMaps.DB.Models
{
    public class Map : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Longitude { get; set; }

        public string Latitude { get; set; }
    }
}