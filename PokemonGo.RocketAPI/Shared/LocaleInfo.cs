/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 08/08/2017
 * Time: 0:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPI.Shared
{
    /// <summary>
    /// Description of LocaleInfo.
    /// </summary>
    public static class LocaleInfo
    {
        public static string Country = "US";
        public static string Language = "en";
        public static string TimeZone = "America/Los Angeles";
        public static string POSIX = "en-us";

        public static void SetValues(string country, string language, string timezone, string posix){
            Country = country;
            Language = language;
            TimeZone = timezone;
            POSIX = posix;
        }
    }
}
