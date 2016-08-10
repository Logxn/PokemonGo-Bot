﻿using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Logic
{
    /// <summary>
    /// "Observable" mades live infos possible
    /// </summary>
    public class LogicInfoObservable
    {
        /// <summary>
        /// GeoLocations
        /// </summary>
        public delegate void GeoLocationHandler(GeoCoordinate value);
        public event GeoLocationHandler HandleNewGeoLocations = delegate { };
        public void PushNewGeoLocations(GeoCoordinate newValue)
        {
            HandleNewGeoLocations(newValue);
        }

        /// <summary>
        /// PokeStops
        /// </summary>
        public delegate void PokeStopGeoLocationHandler(GeoCoordinate value);
        public event PokeStopGeoLocationHandler HandleNewPokeStopsLocations = delegate { };
        public void PushNewPokeStopsLocations(GeoCoordinate newValue)
        {
            HandleNewPokeStopsLocations(newValue);
        }

        /// <summary>
        /// Hunting stats
        /// </summary>
        public delegate void HuntStatsHandler(string value);
        public event HuntStatsHandler HandleNewHuntStats = delegate { };
        public void PushNewHuntStats(string newValue)
        {
            HandleNewHuntStats(newValue);
        }
    }

}
