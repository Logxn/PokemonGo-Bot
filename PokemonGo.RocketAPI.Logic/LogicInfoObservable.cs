using POGOProtos.Map.Fort;
using System;
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
        /// GeoLocations
        /// </summary>
        public delegate void NewPokemonLocationHandler(List<PokemonGo.RocketApi.PokeMap.DataCollector.PokemonMapData> mapData);
        public event NewPokemonLocationHandler HandleNewPokemonLocations = delegate { };
        public void PushNewPokemonLocations(List<PokemonGo.RocketApi.PokeMap.DataCollector.PokemonMapData> mapData)
        {
            HandleNewPokemonLocations(mapData);
        }

        public delegate void ClearPokemonsHandler();
        public event ClearPokemonsHandler HandleClearPokemon = delegate { };
        public void PushClearPokemons()
        {
 
                HandleClearPokemon();
        }

        public delegate void AvailablePokeStopHandler(FortData[] pokeStops);
        public event AvailablePokeStopHandler HandleAvailablePokeStop = delegate { };
        public void PushAvailablePokeStopLocations(FortData[] pokeStop)
        {
            if (pokeStop != null && pokeStop.Any())
            {
                HandleAvailablePokeStop(pokeStop);
            }
        }

        public delegate void PokeStopInfoUpdateHandler(string pokeStopId, string info);
        public event PokeStopInfoUpdateHandler HandlePokeStopInfoUpdate = delegate { };
        public void PushPokeStopInfoUpdate(string pokeStopId, string info)
        {
            HandlePokeStopInfoUpdate(pokeStopId, info);
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
