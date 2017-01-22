﻿/*
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
    /// Description of GlobalVars.
    /// </summary>
    public static class GlobalVars
    {
        // Bot Info  Globals (not yet implemented in any function)
        //public static Version BotApiSupportedVersion = new Version("0.51.2");
        public static Version BotApiSupportedVersion = new Version("0.53.0");
        public static readonly bool BotDebugFlag = true;
        public static readonly bool BotStableFlag = false;

        // Other Globals
        public static string pFHashKey;
        public static string ProfileName = "DefaultProfile";
        public static bool IsDefault = false;
        public static int RunOrder = 0;
        public static string SettingsJSON = "";
        public static Enums.AuthType acc = Enums.AuthType.Google;
        public static string email = "empty";
        public static string Password = "empty";
        public static bool WalkBackToDefaultLocation = true;
        public static bool uselastcoords = true;
        public static double latitude = 40.764883;
        public static double longitude = -73.972967;
        public static double altitude = 15.173855;
        public static double WalkingSpeedInKilometerPerHour = 15;
        public static int MinWalkSpeed = 5;
        public static int radius = 5000;
        public static bool TransferDoublePokemons = true;
        public static int HoldMaxDoublePokemons = 3;
        public static bool EvolvePokemonsIfEnoughCandy = true;
        public static int DontTransferWithCPOver = 999;
        public static int excellentthrow = 25;
        public static int greatthrow = 25;
        public static int nicethrow = 25;
        public static int ordinarythrow = 25;
        public static int MaxPokeballs = 100;
        public static int MaxGreatballs = 100;
        public static int MaxUltraballs = 100;
        public static int MaxRevives = 100;
        public static int MaxPotions = 100;
        public static int MaxSuperPotions = 100;
        public static int MaxHyperPotions = 100;
        public static int MaxTopPotions = 100;
        public static int MaxTopRevives = 100;
        public static int MaxBerries = 100;
        public static int MinCPforGreatBall = 500;
        public static int MinCPforUltraBall = 1000;
        public static int ivmaxpercent = 0;
        public static bool _pauseTheWalking = false;
        public static bool LimitPokeballUse = false;
        public static bool LimitGreatballUse = false;
        public static bool LimitUltraballUse = false;
        public static bool NextBestBallOnEscape = false;
        public static int Max_Missed_throws = 3;
        public static List<PokemonId> pokemonsToHold;
        public static List<PokemonId> catchPokemonSkipList;
        public static List<PokemonId> pokemonsToEvolve;
        public static List<PokemonId> NotToSnipe;
        public static string TelegramAPIToken = string.Empty;
        public static string TelegramName = string.Empty;
        public static int TelegramLiveStatsDelay = 5000;
        public static bool pauseAtPokeStop = false;
        public static bool FarmPokestops = true;
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
        public static bool UseLuckyEgg = true;
        public static bool UseIncense = true;
        public static bool UseRazzBerry = true;
        public static double razzberry_chance = 0.35;
        public static bool EnablePokeList = true;
        public static bool EnableConsoleInTab = false;
        public static bool keepPokemonsThatCanEvolve = true;
        public static bool TransferFirstLowIV = true;
        public static bool pokevision = false;
        public static bool UseLuckyEggIfNotRunning = false;
        public static bool AutoIncubate = true;
        public static bool UseBasicIncubators = false;
        public static bool sleepatpokemons = true;
        public static string SelectedLanguage = "en";
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
        public static bool UsePwdEncryption = false;
        public static bool CheckWhileRunning = false;
        public static int InventoryBasePokeball = 10;
        public static int InventoryBaseGreatball = 10;
        public static int InventoryBaseUltraball = 10;
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

        public static bool FarmGyms;
        public static bool CollectDailyBonus;
        
        public static bool SnipePokemon;
        public static bool ForceSnipe = false;

        public static bool PauseTheWalking = false;
        
        public static ManualSnipePokemon SnipeOpts = new ManualSnipePokemon();
        public static ProxySettings proxySettings = new ProxySettings();
        
        public static bool BypassCheckCompatibilityVersion = false;
        
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string accountProfiles = Path.Combine(path, "Profiles.txt");
        
        public static bool Load()
        {
            var loaded = false;
            GlobalVars.pokemonsToHold = new List<PokemonId>();
            GlobalVars.catchPokemonSkipList = new List<PokemonId>();
            GlobalVars.pokemonsToEvolve = new List<PokemonId>();
            GlobalVars.NotToSnipe = new List<PokemonId>();
            if (File.Exists(accountProfiles))
            {
                try
                {
                    string JSONstring = File.ReadAllText(accountProfiles);
                    Collection<Profile> profiles = JsonConvert.DeserializeObject<Collection<Profile>>(JSONstring);
                    foreach (Profile _profile in profiles)
                    {
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
            /*
             * AuthType, acc
             *  
             */
            foreach (var field in settings.GetType().GetFields()) {
                var fieldname = field.Name;
                if (fieldname == "AuthType")
                    fieldname = "acc";
                if (fieldname == "Username")
                    fieldname = "email";
                if (fieldname == "DefaultLatitude")
                    fieldname = "latitude";
                if (fieldname == "DefaultLongitude")
                    fieldname = "longitude";
                if (fieldname == "DefaultAltitude")
                    fieldname = "altitude";
                typeof(GlobalVars).GetField(fieldname).SetValue(null,
                     field.GetValue(settings));
            }
        }
    }
}