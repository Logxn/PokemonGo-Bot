using System.Collections.Generic;
using Newtonsoft.Json;

namespace PokemonGo.RocketApi.PokeMap.DataModel
{
    public class ResultNearby
    {
        [JsonProperty("active_fort_modifier")]
        public List<string> ActiveFortModifier { get; set; }
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }
        [JsonProperty("encounter_id")]
        public string EncounterId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("last_modified_timestamp_ms")]
        public string LastModifiedTimestampMs { get; set; }
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("lure_info")]
        public LureInfo LureInfo { get; set; }
        [JsonProperty("pokemon_id")]
        public string PokemonId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("zone")]
        public string Zone { get; set; }
    }
}