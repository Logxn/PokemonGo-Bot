using Newtonsoft.Json;

namespace PokemonGo.RocketApi.PokeMap.DataModel
{
    public class LureInfo
    {
        [JsonProperty("active_pokemon_id")]
        public string ActivePokemonId { get; set; }
        [JsonProperty("encounter_id")]
        public string EncounterId { get; set; }
        [JsonProperty("fort_id")]
        public string FortId { get; set; }
        [JsonProperty("lure_expires_timestamp_ms")]
        public string LureExpiresTimestampMs { get; set; }
    }
}