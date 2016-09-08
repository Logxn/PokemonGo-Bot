using Newtonsoft.Json;

namespace PokemonGo.RocketApi.PokeMap.DataModel
{
    public class ResultCache
    {
        [JsonProperty("_id")]
        public string Id { get; set; }
        [JsonProperty("encounter_id")]
        public string EncounterId { get; set; }
        [JsonProperty("expireAt")]
        public string ExpireAt { get; set; }
        [JsonProperty("lnglat")]
        public LatitudeLongitude LatitudeLongitude { get; set; }
        [JsonProperty("pokemon_id")]
        public string PokemonId { get; set; }
        [JsonProperty("spawn_id")]
        public string SpawnId { get; set; }

    }
}