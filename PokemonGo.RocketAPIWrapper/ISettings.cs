/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 15:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPIWrapper
{
    public interface ISettings
    {
        string Username { get; set; }
        string Password { get; set; }
        string HashKey { get;  }
        double Latitude { get;  }
        double Longitude { get;  }
        string LocaleCountry { get; set; }
        string LocaleLanguage { get; set; }
        string LocaleTimezone { get; set; } 
    }

}
