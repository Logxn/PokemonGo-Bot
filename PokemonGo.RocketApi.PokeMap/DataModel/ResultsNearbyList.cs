using System.Collections.Generic;
using Newtonsoft.Json;
using PokemonGo.RocketApi.PokeMap.DataModel;

namespace PokemonGo.RocketApi.PokeMap.DataModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ResultsNearbyList
    {
        [JsonProperty("result")]
        public List<ResultNearby> Results { get; set; }
    }
}