/*
 * Created by SharpDevelop.
 * User: usuarioIEDC
 * Date: 17/01/2017
 * Time: 10:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Newtonsoft.Json;
using System.IO;

namespace PokeMaster.Logic.Shared
{
    /// <summary>
    /// Avatar Settings.
    /// </summary>
    public class AvatarSettings
    {
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string filename = Path.Combine(path, "avatar_settings.json");
        
        [JsonProperty("skin")]
        static public int skin;
        [JsonProperty("hair")]
        static public int hair;
        [JsonProperty("eyes")]
        static public int eyes;
        [JsonProperty("hat")]
        static public int hat;
        [JsonProperty("shirt")]
        static public int shirt;
        [JsonProperty("pants")]
        static public int pants;
        [JsonProperty("shoes")]
        static public int shoes;
        [JsonProperty("backpack")]
        static public int backpack;
        [JsonProperty("Gender")]
        static public int Gender;
        [JsonProperty("starter")]
        static public POGOProtos.Enums.PokemonId starter;
        [JsonProperty("nicknamePrefix")]
        static public string nicknamePrefix;
        [JsonProperty("nicknameSufix")]
        static public string nicknameSufix;
        public static void Save()
        {
            string ProfilesString = JsonConvert.SerializeObject(new AvatarSettings(), Formatting.Indented);
             File.WriteAllText(filename, ProfilesString);

        }
        public static bool Load()
        {
            if (File.Exists(filename)){
                AvatarSettings settings = JsonConvert.DeserializeObject<AvatarSettings>(File.ReadAllText(filename));
                return true;
            }
            return false;
        }
    }

}
