using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    class PokeVisionUtil
    {

        private List<spottedPoke> _newSpotted;
        private List<spottedPoke> _alreadySpotted;
        private List<spottedPoke> _alreadyCaught;
        private HttpClient _httpClient;
        private readonly ISettings _clientSettings;
        private int _distance;
        private bool _debug = false;

        public PokeVisionUtil(ISettings clientSettings)
        {
            _newSpotted = new List<spottedPoke>();
            _httpClient = new HttpClient();
            _clientSettings = clientSettings;
            _distance = 150;
            if (System.IO.File.Exists("alreadyCaught.xml") && System.IO.File.Exists("alreadySpotted.xml"))
            {
                _alreadySpotted = XmlHelper.DeSerializeObject<List<spottedPoke>>("alreadySpotted.xml");
                _alreadyCaught = XmlHelper.DeSerializeObject<List<spottedPoke>>("alreadyCaught.xml");
            }
            else
            {
                _alreadySpotted = new List<spottedPoke>();
                _alreadyCaught = new List<spottedPoke>();
            }
        }

        public async Task<List<spottedPoke>> GetNearPokemons(double lat, double lng)
        {
            _newSpotted.Clear();
            ClearByTime();

            HttpResponseMessage response = await _httpClient.GetAsync("https://pokevision.com/map/data/" + lat.ToString().Replace(",", ".") + "/" + lng.ToString().Replace(",", "."));
            HttpContent content = response.Content;
            string result = await content.ReadAsStringAsync();

            dynamic stuff = JsonConvert.DeserializeObject(result);
            if (stuff.status == "success")
            {
                try
                {
                    for (int i = 0; stuff.pokemon[i].id > 0; i++)
                    {

                        if (CatchThisPokemon((Int32)stuff.pokemon[i].pokemonId) &&
                            inRange(lat, lng, (Double)(stuff.pokemon[i].latitude), (Double)(stuff.pokemon[i].longitude)) &&
                            !AlreadySpotted((Int32)(stuff.pokemon[i].id)) &&
                            !AlreadyCaught((PokemonId)((Int32)stuff.pokemon[i].pokemonId), (Int32)stuff.pokemon[i].expiration_time, (Double)stuff.pokemon[i].latitude, (Double)stuff.pokemon[i].longitude))
                        {
                            _newSpotted.Add(new spottedPoke((Int32)stuff.pokemon[i].pokemonId, (Double)stuff.pokemon[i].latitude, (Double)stuff.pokemon[i].longitude, (Int32)stuff.pokemon[i].expiration_time, (Int32)stuff.pokemon[i].id));
                            _alreadySpotted.Add(new spottedPoke((Int32)stuff.pokemon[i].pokemonId, (Double)stuff.pokemon[i].latitude, (Double)stuff.pokemon[i].longitude, (Int32)stuff.pokemon[i].expiration_time, (Int32)stuff.pokemon[i].id));
                            // DEBUG INFORMATION
                            if (_debug)
                                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Vision: Pokemon ID: " + stuff.pokemon[i].id + " lat: " + (Double)stuff.pokemon[i].latitude + " lng: " + (Double)stuff.pokemon[i].longitude + " TS: " + stuff.pokemon[i].expiration_time);
                        }

                    }
                }
                catch (Exception e)
                {

                }
            }
            XmlHelper.SerializeObject<List<spottedPoke>>(_alreadySpotted, "alreadySpotted.xml");
            return _newSpotted;
        }

        public void addCaughtPoke(MapPokemon poke)
        {
            if (_debug)
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Caught Poke: " + poke.PokemonId.ToString() + " lat " + poke.Latitude + " lng " + poke.Longitude + " TS: " + (Int32)(poke.ExpirationTimestampMs / 1000.00));
            if (poke.ExpirationTimestampMs != 0)
            {
                _alreadyCaught.Add(new spottedPoke(poke.PokemonId, poke.Latitude, poke.Longitude, (Int32)(poke.ExpirationTimestampMs / 1000.00)));
                XmlHelper.SerializeObject<List<spottedPoke>>(_alreadyCaught, "alreadyCaught.xml");
            }
        }

        private bool CatchThisPokemon(int id)
        {
            if (_clientSettings.catchPokemonSkipList.Contains((PokemonId)id))
            {
                return false;
            }
            return true;
        }

        private bool inRange(double curLat, double curLng, double pokeLat, double pokeLng)
        {
            return LocationUtils.CalculateDistanceInMeters(curLat, curLng, pokeLat, pokeLng) < _distance;
        }

        private void ClearByTime()
        {
            int actUnixTime = UnixTimeNow();
            for (int i = 0; i < _alreadySpotted.Count; ++i)
            {
                if (_alreadySpotted[i]._expTime < actUnixTime)
                {
                    if (_debug)
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Delete already Spotted: " + _alreadyCaught[i]._pokeId + " Now/Poke: " + actUnixTime + "/" + _alreadyCaught[i]._expTime);
                    _alreadySpotted.RemoveAt(i);
                    --i;
                }
            }
            for (int i = 0; i < _alreadyCaught.Count; ++i)
            {
                if (_alreadyCaught[i]._expTime < actUnixTime)
                {
                    if (_debug)
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Delete already Catched: " + _alreadyCaught[i]._pokeId + " Now/Poke: " + actUnixTime + "/" + _alreadyCaught[i]._expTime);
                    _alreadyCaught.RemoveAt(i);
                    --i;
                }
            }
        }

        public bool AlreadyCaught(PokemonId pokid, int exp, double lat, double lng)
        {
            foreach (spottedPoke poke in _alreadyCaught)
            {
                if ((poke._expTime / 10) == (exp / 10) && // Div 100 for approximation
                    poke._pokeId == pokid &&
                    LocationUtils.CalculateDistanceInMeters(lat, lng, poke._lat, poke._lng) < _distance)
                    return true;
            }
            return false;
        }

        private bool AlreadySpotted(int id)
        {
            foreach (spottedPoke poke in _alreadySpotted)
            {
                if (poke._visionId == id)
                    return true;
            }
            return false;
        }

        private int UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)timeSpan.TotalSeconds;
        }

    }

    public class spottedPoke
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

        public spottedPoke(PokemonId id, double lat, double lng, int expTime)
        {
            _pokeId = id;
            _lat = lat;
            _lng = lng;
            _expTime = expTime;
            _visionId = 0;
        }

        public spottedPoke()
        {

        }
    }
}
