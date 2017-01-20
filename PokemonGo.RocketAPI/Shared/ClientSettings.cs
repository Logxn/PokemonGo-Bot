/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 19/01/2017
 * Time: 13:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using PokemonGo.RocketAPI.Enums;

namespace PokemonGo.RocketAPI.Shared
{
    /// <summary>
    /// Description of BotSettings.
    /// </summary>
    public class ClientSettings
    {
        public string hashKey{get;set;}
        public double latitude{get;set;}
        public double longitude{get;set;}
        public double altitude{get;set;}
        public string proxyUrl{get;set;}
        public int proxyPort{get;set;}
        public string proxyUser{get;set;}
        public string proxyPass{get;set;}
        public AuthType userType{get;set;}
        public string userName{get;set;}
        public string password{get;set;}
        public Version currentApi{get;set;}

        
        public ClientSettings(string hashKey, double latitude , double longitude, double altitude,
                      string proxyUrl, int proxyPort , string proxyUser,string proxyPass,
                      AuthType userType, string userName, string password, Version currentApi)
        {
            this.hashKey= hashKey;
            this.latitude= latitude;
            this.longitude= longitude;
            this.altitude= altitude;
            this.proxyUrl= proxyUrl;
            this.proxyPort= proxyPort;
            this.proxyUser= proxyUser;
            this.proxyPass= proxyPass;
            this.userType= userType;
            this.userName= userName;
            this.password= password;
            this.currentApi= currentApi;            
        }
    }
}
