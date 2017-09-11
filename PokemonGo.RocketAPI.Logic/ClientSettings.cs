/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 16:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PokemonGo.RocketAPIWrapper;

namespace PokemonGo.RocketAPI.Logic
{
    /// <summary>
    /// Description of ClientSettings.
    /// </summary>
    public class ClientSettings : ISettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HashKey { get;  }
        public double Latitude { get;  }
        public double Longitude { get;  }
        public string LocaleCountry { get; set; } = "US";
        public string LocaleLanguage { get; set; } = "en";
        public string LocaleTimezone { get; set; } = "US/Los Angeles";
        
        public ClientSettings(string user, string pwd, string hashk, double lat, double lon)
        {
            Username = user;
            Password = pwd;
            HashKey = hashk;
            Latitude = lat;
            Longitude = lon;
        }
    }
}
