/*
 * Created by SharpDevelop.
 * User: usuarioIEDC
 * Date: 18/01/2017
 * Time: 14:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.Logic;
using POGOProtos.Enums;
using System.Device.Location;
using Google.Protobuf;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;


namespace PokemonGo.RocketAPI.Logic.Shared
{
    /// <summary>
    /// Description of GlobalSettings.
    /// </summary>
    public static class GlobalSettings
    {
        // Bot Info  Globals (not yet implemented in any function)
        public static Version BotVersion = new Version();
        public static Version BotApiSupportedVersion = new Version();
        public static Version NianticApiVersion = new Version ();
        public static readonly bool BotDebugFlag = true;
        public static readonly bool BotStableFlag = false;

        // Other Globals
        public static Collection<Profile> Profiles = new Collection<Profile>();
        public static string pFHashKey;
        public static string ProfileName = "DefaultProfile";
        public static bool IsDefault = false;
        public static int RunOrder = 0;
        public static string SettingsJSON = "";
        public static Enums.AuthType acc = Enums.AuthType.Google;
        public static string email = "empty";
        public static string password = "empty";
        public static bool defLoc = true;
        public static bool uselastcoords = true;
        public static double latitute = 40.764883;
        public static double longitude = -73.972967;
        public static double altitude = 15.173855;
        public static double speed = 15;
        public static int MinWalkSpeed = 5;
        public static int radius = 5000;
        public static bool transfer = true;
        public static int duplicate = 3;
        public static bool evolve = true;
        public static int maxCp = 999;
        public static int excellentthrow = 25;
        public static int greatthrow = 25;
        public static int nicethrow = 25;
        public static int ordinarythrow = 25;
        public static int pokeball = 100;
        public static int greatball = 100;
        public static int ultraball = 100;
        public static int revive = 100;
        public static int potion = 100;
        public static int superpotion = 100;
        public static int hyperpotion = 100;
        public static int toppotion = 100;
        public static int toprevive = 100;
        public static int berry = 100;
        public static int MinCPforGreatBall = 500;
        public static int MinCPforUltraBall = 1000;
        public static int ivmaxpercent = 0;
        public static bool _pauseTheWalking = false;
        public static bool LimitPokeballUse = false;
        public static bool LimitGreatballUse = false;
        public static bool LimitUltraballUse = false;
        public static bool NextBestBallOnEscape = false;
        public static int Max_Missed_throws = 3;
        public static List<PokemonId> noTransfer;
        public static List<PokemonId> noCatch;
        public static List<PokemonId> doEvolve;
        public static List<PokemonId> NotToSnipe;
        public static string telAPI = string.Empty;
        public static string telName = string.Empty;
        public static int telDelay = 5000;
        public static bool pauseAtPokeStop = false;
        public static bool farmPokestops = true;
        public static bool CatchPokemon = true;
        public static bool BreakAtLure = false;
        public static bool UseAnimationTimes = true;
        public static bool UseLureAtBreak = false;
        public static bool UseGoogleMapsAPI = false;
        public static string GoogleMapsAPIKey;
        public static bool RandomReduceSpeed = false;
        public static bool UseBreakFields = false;
        public static double TimeToRun;
        public static int PokemonCatchLimit = 1000;
        public static int PokestopFarmLimit = 2000;
        public static int XPFarmedLimit = 150000;
        public static int BreakInterval = 0;
        public static int BreakLength = 0;
        public static int navigation_option = 1;
        public static bool useluckyegg = true;
        public static bool useincense = true;
        public static bool userazzberry = true;
        public static double razzberry_chance = 0.35;
        public static bool pokeList = true;
        public static bool consoleInTab = false;
        public static bool keepPokemonsThatCanEvolve = true;
        public static bool TransferFirstLowIV = true;
        public static bool pokevision = false;
        public static bool useLuckyEggIfNotRunning = false;
        public static bool autoIncubate = true;
        public static bool useBasicIncubators = false;
        public static bool sleepatpokemons = true;
        public static string settingsLanguage = "en";
        public static LogicInfoObservable infoObservable = new LogicInfoObservable();
        public static bool Espiral = false;
        public static bool MapLoaded = false;
        public static bool logPokemons = false;
        public static LinkedList<GeoCoordinate> NextDestinationOverride = new LinkedList<GeoCoordinate>();
        public static LinkedList<GeoCoordinate> RouteToRepeat = new LinkedList<GeoCoordinate>();
        public static bool RepeatUserRoute = false;
        public static bool logManualTransfer = false;
        public static bool UseLureGUIClick = false;
        public static bool UseLuckyEggGUIClick = false;
        public static bool UseIncenseGUIClick = false;
        public static bool RelocateDefaultLocation = false;
        public static double RelocateDefaultLocationTravelSpeed = 0;
        public static bool bLogEvolve = false;
        public static bool LogEggs = false;
        public static bool pauseAtEvolve = false;
        public static bool pauseAtEvolve2 = false;
        public static bool AutoUpdate = false;
        public static bool usePwdEncryption = false;
        public static bool CheckWhileRunning = false;
        public static int InventoryBasePokeball = 10;
        public static int InventoryBaseGreatball = 10;
        public static int InventoryBaseUltraball = 10;
        public static bool FirstLoad;
        public static int MinCPtoCatch = 0;
        public static int MinIVtoCatch = 0;
        public static bool AvoidRegionLock = true;
        public static bool simulatedPGO = false;
        public static ByteString SessionHash;

        public static bool No2kmEggs = false;
        public static bool No5kmEggs = false;
        public static bool No10kmEggs = false;
        public static bool EggsAscendingSelection = true;

        public static bool No2kmEggsBasicInc = false;
        public static bool No5kmEggsBasicInc = false;
        public static bool No10kmEggsBasicInc = false;
        public static bool EggsAscendingSelectionBasicInc = false;

        public static bool EnableVerboseLogging = false;

        public static bool farmGyms;
        public static bool CollectDailyBonus;
        
        public static bool SnipePokemon;
        public static bool ForceSnipe = false;

        public static bool PauseTheWalking = false;
        
        public static ManualSnipePokemon SnipeOpts = new ManualSnipePokemon();
        public static ProxySettings proxySettings = new ProxySettings();
        
        public static bool BypassCheckCompatibilityVersion = true;
        
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string accountProfiles = Path.Combine(path, "Profiles.txt");
        
        public static bool Load()
        {
            var loaded = false;
            GlobalSettings.noTransfer = new List<PokemonId>();
            GlobalSettings.noCatch = new List<PokemonId>();
            GlobalSettings.doEvolve = new List<PokemonId>();
            GlobalSettings.NotToSnipe = new List<PokemonId>();
            if (File.Exists(accountProfiles))
            {
                try
                {
                    string JSONstring = File.ReadAllText(accountProfiles);
                    Collection<Profile> profiles = JsonConvert.DeserializeObject<Collection<Profile>>(JSONstring);
                    foreach (Profile _profile in profiles)
                    {
                        GlobalSettings.Profiles.Add(_profile);
                        if (_profile.IsDefault)
                        {
                            LoadProfile(_profile.SettingsJSON);
                            loaded = true;
                        }
                    }
                }
                catch (Exception ex1)
                {
                    Logger.ExceptionInfo("Loading profiles file:" +ex1.ToString());
                }
            }
            return loaded;
        }
        private static void LoadProfile(string configString){
            var jsonSettings = new JsonSerializerSettings
            {
                Error = (sender, args) =>
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            };
            var config = JsonConvert.DeserializeObject<ProfileSettings>(configString, jsonSettings);
            Assign(config);
        }
        private static void Assign(ProfileSettings settings){
            foreach (var field in settings.GetType().GetFields()) {
                typeof(GlobalSettings).GetField(field.Name).SetValue(null,
                     field.GetValue(settings));
            }
        }
    }
}
