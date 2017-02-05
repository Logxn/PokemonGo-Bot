using POGOProtos.Map.Fort;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using POGOProtos.Map.Pokemon;

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
        public delegate void NewPokemonLocationHandler(IEnumerable<MapPokemon> mapData);
        public event NewPokemonLocationHandler HandleNewPokemonLocations = delegate { };
        public void PushNewPokemonLocations(IEnumerable<MapPokemon> mapData)
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

        public delegate void AvailablePokeGymHandler(FortData[] pokeGyms);
        public event AvailablePokeGymHandler HandleAvailablePokeGym = delegate { };
        public void PushAvailablePokeGymsLocations(FortData[] pokeGyms)
        {
            if (pokeGyms != null && pokeGyms.Any())
            {
                HandleAvailablePokeGym(pokeGyms);
            }
        }

        public delegate void PokeStopInfoUpdateHandler(FortData pokeStop, string info);
        public event PokeStopInfoUpdateHandler HandlePokeStopInfoUpdate = delegate { };
        public void PushPokeStopInfoUpdate(FortData pokeStop, string info)
        {
            HandlePokeStopInfoUpdate(pokeStop, info);
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
         /// <summary>
        /// GeoLocations
        /// </summary>
        public delegate void DeletePokemonLocationHandler(string pokemon_Id);
        public event DeletePokemonLocationHandler HandleDeletePokemonLocation = delegate { };
        public void PushDeletePokemonLocation(string pokemon_Id)
        {
            HandleDeletePokemonLocation(pokemon_Id);
        }
    }

}
