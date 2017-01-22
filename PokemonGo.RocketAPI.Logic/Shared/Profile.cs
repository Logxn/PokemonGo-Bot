/*
 * Created by SharpDevelop.
 * User: usuarioIEDC
 * Date: 18/01/2017
 * Time: 15:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace PokemonGo.RocketAPI.Logic.Shared
{
    public class Profile
    {
        public string ProfileName
        { get; set; }
        public bool IsDefault
        { get; set; }
        public int RunOrder
        { get; set; }
        public string SettingsJSON
        { get; set; }
        public ProfileSettings Settings
        { get; set; }
        
    }
}
