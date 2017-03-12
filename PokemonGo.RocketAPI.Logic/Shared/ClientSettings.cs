/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/03/2017
 * Time: 21:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Util.Device;

namespace PokeMaster.Logic.Shared
{
    /// <summary>
    /// Description of ClientSettings.
    /// </summary>
    public class ClientSettings : PokemonGo.RocketAPI.ISettings
    {
        public AuthType AuthType
        { get { return GlobalVars.acc; } set { GlobalVars.acc = value; } }
        
        public double DefaultLatitude
        { get { return GlobalVars.latitude; } set { GlobalVars.latitude = value; } }
        public double DefaultLongitude
        { get { return GlobalVars.longitude; } set { GlobalVars.longitude = value; } }
        public double DefaultAltitude
        { get { return GlobalVars.altitude; } set { GlobalVars.altitude = value; } }
        public string Password
        { get { return GlobalVars.Password; } set { GlobalVars.Password = value; } }
        public string Username
        { get { return GlobalVars.email; } set { GlobalVars.email = value; } }
        public bool UseProxy
        { get { return GlobalVars.proxySettings.enabled; } set { GlobalVars.proxySettings.enabled = value; } }
        public bool UseProxyAuthentication
        { get { return GlobalVars.proxySettings.useAuth; } set { GlobalVars.proxySettings.useAuth = value; } }
        public string UseProxyHost
        { get { return GlobalVars.proxySettings.hostName; } set { GlobalVars.proxySettings.hostName = value; } }
        public string UseProxyPort
        { get { return "" + GlobalVars.proxySettings.port; } set { GlobalVars.proxySettings.port = int.Parse(value); } }
        public string UseProxyUsername
        { get { return GlobalVars.proxySettings.username; } set { GlobalVars.proxySettings.username = value; } }
        public string UseProxyPassword
        { get { return GlobalVars.proxySettings.password; } set { GlobalVars.proxySettings.password = value; } }
        public bool UsePogoDevHashServer
        { get; set; }
        public bool UseLegacyAPI
        { get; set; }
        public string AuthAPIKey
        { get { return GlobalVars.pFHashKey; } set { GlobalVars.pFHashKey = value; } }
        public bool DisplayVerboseLog
        { get { return GlobalVars.Debug.VerboseMode; } set { GlobalVars.Debug.VerboseMode = value; } }

        public string GoogleRefreshToken{ get; set; }
        public string DevicePlatform { get; set; }
        public string DeviceId { get; set; }
        public string AndroidBoardName { get; set; }
        public string AndroidBootloader { get; set; }
        public string DeviceBrand { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceModelIdentifier { get; set; }
        public string DeviceModelBoot { get; set; }
        public string HardwareManufacturer { get; set; }
        public string HardwareModel { get; set; }
        public string FirmwareBrand { get; set; }
        public string FirmwareTags { get; set; }
        public string FirmwareType { get; set; }
        public string FirmwareFingerprint { get; set; }

    }
}
