using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using POGOProtos.Enums;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    class PokeVisionUtil
    {

        private List<spottedPoke> _newSpotted;
        private List<spottedPoke> _alreadySpotted;
        HttpClient.PokemonHttpClient _httpClient;

        public PokeVisionUtil()
        {
            _newSpotted = new List<spottedPoke>();
            _alreadySpotted = new List<spottedPoke>();
            _httpClient = new HttpClient.PokemonHttpClient();
        }


        public async Task<List<spottedPoke>> GetNearPokemons(double lat, double lng)
        {
            _newSpotted.Clear();
            ClearAlreadySpottedByTime();
            //https://skiplagged.com/api/pokemon.php?bounds=22.229977,114.068312,22.334655,114.299883
            double Lat10 = lat + 0.10;
            double Lon10 = lng + 0.10;

            HttpResponseMessage response = await _httpClient.GetAsync("https://skiplagged.com/api/pokemon.php?bounds=" + lat.ToString().Replace(",", ".") + "," + lng.ToString().Replace(",", ".") + "," + Lat10.ToString().Replace(",", ".") + "," + Lon10.ToString().Replace(",", "."));
            HttpContent content = response.Content;
            string result = await content.ReadAsStringAsync();

            dynamic stuff = JsonConvert.DeserializeObject(result);
            try
            {
                for (int i = 0; stuff.pokemons[i].pokemon_id > 0; i++)
                {
                    if (CatchThisPokemon((Int32)stuff.pokemons[i].pokemon_id) &&
                        inRange(lat, lng, (Double)(stuff.pokemons[i].latitude), (Double)(stuff.pokemons[i].longitude)) &&
                        !AlreadyCatched((Int32)(stuff.pokemons[i].pokemon_id)))
                    {
                        _newSpotted.Add(new spottedPoke((Int32)stuff.pokemons[i].pokemon_id, (Double)stuff.pokemons[i].latitude, (Double)stuff.pokemons[i].longitude, (Int32)stuff.pokemons[i].expires, (Int32)stuff.pokemons[i].pokemon_id));
                        _alreadySpotted.Add(new spottedPoke((Int32)stuff.pokemons[i].pokemon_id, (Double)stuff.pokemons[i].latitude, (Double)stuff.pokemons[i].longitude, (Int32)stuff.pokemons[i].expires, (Int32)stuff.pokemons[i].pokemon_id));

                    }

                }
            }
            catch (Exception)
            {
                // ignored
            }

            return _newSpotted;
        }

        bool CatchThisPokemon(int id)
        {
            return true;
        }

        bool inRange(double curLat, double curLng, double pokeLat, double pokeLng)
        {
            return LocationUtils.CalculateDistanceInMeters(curLat, curLng, pokeLat, pokeLng) < 125;
        }

        void ClearAlreadySpottedByTime()
        {
            int actUnixTime = UnixTimeNow();
            for (int i = 0; i < _alreadySpotted.Count; ++i)
            {
                if (_alreadySpotted[i]._expTime < actUnixTime)
                {
                    _alreadySpotted.RemoveAt(i);
                    --i;
                }
            }
        }

        bool AlreadyCatched(int id)
        {
            foreach (spottedPoke poke in _alreadySpotted)
            {
                if (poke._visionId == id)
                    return true;
            }
            return false;
        }

        public int UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)timeSpan.TotalSeconds;
        }

    }

    class spottedPoke
    {
        public PokemonId _pokeId = 0;
        public int _visionId;
        public double _lat;
        public double _lng;
        public int _expTime;

        public spottedPoke(int id, double lat, double lng, int expTime, int visionid)
        {
            _pokeId = (PokemonId)id;
            _lat = lat;
            _lng = lng;
            _expTime = expTime;
            _visionId = visionid;
        }


    }
}