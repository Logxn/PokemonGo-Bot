namespace PokemonGo.RocketAPI.Helpers
{
    using Newtonsoft.Json.Linq;

    public class JsonHelper
    {
        public static string GetValue(string json, string key)
        {
            var jObject = JObject.Parse(json);
            return jObject[key].ToString();
        }
    }
}