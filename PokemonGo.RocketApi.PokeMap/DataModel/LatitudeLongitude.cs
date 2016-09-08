using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace PokemonGo.RocketApi.PokeMap.DataModel
{
    public class LatitudeLongitude
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }

        [JsonIgnore]
        public double? Latitude
        {
            get
            {
                if (Coordinates != null)
                {
                    return Coordinates[1];
                }
                else
                {
                    return null;
                }
            }
        }
        [JsonIgnore]
        public double? Longitude { get
            {
                if (Coordinates != null)
                {
                    return Coordinates[0];
                }
                else
                {
                    return null;
                }
            } }
    }
}
