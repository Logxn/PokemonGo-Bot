using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using POGOProtos.Enums;
using System.Linq;
using POGOProtos.Networking.Responses;
using POGOProtos.Inventory.Item;
using System.Threading;

namespace PokemonGo.RocketAPI.Logic.Utils
{
    class PokeSnipers
    {
        HttpClient.PokemonHttpClient _httpClient;
        private List<spottedPokeSni> _newSpotted;
        private List<spottedPokeSni> _alreadySpotted;

        public PokeSnipers()
        {
            _httpClient = new HttpClient.PokemonHttpClient();
            _newSpotted = new List<spottedPokeSni>();
            _alreadySpotted = new List<spottedPokeSni>();
        }

        public async Task<List<spottedPokeSni>> CapturarPokemon()
        {
            //https://github.com/xxxx0107/PokeSniper2/blob/a8f40c29b531e33c7e0342b8a1b8a06ec1850608/PogoLocationFeeder/Repository/PokeSniperRarePokemonRepository.cs
            PokemonId idPoke = 0;
            _newSpotted.Clear();
            ClearAlreadySpottedByTime();

            HttpResponseMessage response = await _httpClient.GetAsync("http://pokesnipers.com/api/v1/pokemon.json");
            HttpContent content = response.Content;
            string result = await content.ReadAsStringAsync();

            dynamic Snipers = JsonConvert.DeserializeObject(result);
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Looking for Pokemons to snipe....");
            try
            {
                for (int i = 0; Snipers.results[i].id > 0; i++)
                {
                    idPoke = PokemonParser.ParsePokemon("" + Snipers.results[i].name);
                    //Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "" + Snipers.results[i].name);
                    string c = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string coords1 = Snipers.results[i].coords;
                    coords1 = coords1.Replace(',', '|');
                    if (c == ",")
                        coords1 = coords1.Replace('.', ',');
                    double[] coords = coords1.Split('|').Select(double.Parse).ToArray();
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "" + idPoke + " = " + coords[0] + " / " + coords[1]);

                    _newSpotted.Add(new spottedPokeSni((Int32)idPoke, (Double)coords[0], (Double)coords[1], (Int32)DateTimeToUnixTimestamp((DateTime)Snipers.results[i].until), (Int32)idPoke, (Int32)VerTipo("" + Snipers.results[i].rarity)));
                    _alreadySpotted.Add(new spottedPokeSni((Int32)idPoke, (Double)coords[0], (Double)coords[1], (Int32)DateTimeToUnixTimestamp((DateTime)Snipers.results[i].until), (Int32)idPoke, (Int32)VerTipo("" + Snipers.results[i].rarity)));
                }
            }
            catch (Exception e)
            {
                //Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error PokeSnipers: \n" + e.Message + "\n" +  e.HelpLink);
            }

            return _newSpotted;
        }

        int VerTipo(string cual)
        {
            switch (cual)
            {
                case "rare": return 0;
                case "uncommon": return 1;
                case "special": return 2;
                case "very_rare": return 3;
            }
            return 1;
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

        bool CatchThisPokemon(int id)
        {
            return true;
        }

        bool AlreadyCatched(int id)
        {
            foreach (spottedPokeSni poke in _alreadySpotted)
            {
                if (poke._visionId == id)
                    return true;
            }
            return false;
        }

        int DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (int)(TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }

        public int UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)timeSpan.TotalSeconds;
        }

        /*
            foreach(spottedPokeSni p in await _pokeSnipers.CapturarPokemon()){ 
                await _pokeSnipers.CapturarSniper(p, _clientSettings, _client);
            }
            StringUtils.CheckKillSwitch(true);
        */
    }

    class spottedPokeSni
    {
        public PokemonId _pokeId = 0;
        public int _visionId;
        public double _lat;
        public double _lng;
        public int _expTime;
        public int _tipo;

        public spottedPokeSni(int id, double lat, double lng, int expTime, int visionid, int raro)
        {
            _pokeId = (PokemonId)id;
            _lat = lat;
            _lng = lng;
            _expTime = expTime;
            _visionId = visionid;
            _tipo = raro;
        }
    }
}
