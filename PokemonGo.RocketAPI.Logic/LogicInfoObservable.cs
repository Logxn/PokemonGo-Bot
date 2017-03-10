using POGOProtos.Map.Fort;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using POGOProtos.Map.Pokemon;

namespace PokeMaster.Logic
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
        /// PokemonLocations
        /// </summary>
        public delegate void NewPokemonLocationHandler(MapPokemon mapData);
        public event NewPokemonLocationHandler HandleNewPokemonLocation = delegate { };
        public void PushNewPokemonLocation(MapPokemon mapData)
        {
            HandleNewPokemonLocation(mapData);
        }

        public delegate void NewPokemonLocatiosnHandler(IEnumerable<MapPokemon> mapData);
        public event NewPokemonLocatiosnHandler HandleNewPokemonLocations = delegate { };
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

        /// <summary>
        /// DeletePokemonLocation
        /// </summary>
        public delegate void DeletePokemonLocationHandler(string pokemon_Id);
        public event DeletePokemonLocationHandler HandleDeletePokemonLocation = delegate { };
        public void PushDeletePokemonLocation(string pokemon_Id)
        {
            HandleDeletePokemonLocation(pokemon_Id);
        }
        

        /// <summary>
        /// AvailablePokeStopLocations
        /// </summary>
        public delegate void AvailablePokeStopHandler(FortData[] pokeStops);
        public event AvailablePokeStopHandler HandleAvailablePokeStop = delegate { };
        public void PushAvailablePokeStopLocations(FortData[] pokeStop)
        {
            if (pokeStop != null && pokeStop.Any())
            {
                HandleAvailablePokeStop(pokeStop);
            }
        }

        /// <summary>
        /// PokeStopInfoUpdate
        /// </summary>
        public delegate void PokeStopInfoUpdateHandler(FortData pokeStop, string info);
        public event PokeStopInfoUpdateHandler HandlePokeStopInfoUpdate = delegate { };
        public void PushPokeStopInfoUpdate(FortData pokeStop, string info)
        {
            HandlePokeStopInfoUpdate(pokeStop, info);
        }

        /// <summary>
        /// AvailablePokeGymsLocations
        /// </summary>
        public delegate void AvailablePokeGymHandler(FortData[] pokeGyms);
        public event AvailablePokeGymHandler HandleAvailablePokeGym = delegate { };
        public void PushAvailablePokeGymsLocations(FortData[] pokeGyms)
        {
            if (pokeGyms != null && pokeGyms.Any())
            {
                HandleAvailablePokeGym(pokeGyms);
            }
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
        /// UpdatePokeGym
        /// </summary>
        public delegate void UpdatePokeGym(FortData pokeGym);
        public event UpdatePokeGym HandleUpdatePokeGym = delegate { };
        public void PushUpdatePokeGym(FortData pokeGym)
        {
            HandleUpdatePokeGym(pokeGym);
        }
    }

}
