using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using Google.Protobuf;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;

namespace PokeMaster.Logic.Shared
{
    /// <summary>
    /// Description of GlobalVars.
    /// </summary>
    public static class GlobalVars
    {
        // Bot Info  Globals (not yet implemented in any function)
        public static Version BotApiSupportedVersion = Resources.BotApiSupportedVersion;

        // Command line vars
        public static bool BypassCheckCompatibilityVersion = false;
        public static bool BypassKillSwitch = false;

        // Path Vars
        public static string PathToConfigs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string PathToTranslations = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Translations");
        public static string PathToDevices = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        public static string PathToLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

        public static string FileForProfiles = Path.Combine(GlobalVars.PathToConfigs, "Profiles.txt");
        public static string FileForCoordinates = Path.Combine(GlobalVars.PathToConfigs, "Lastcoords.txt");
        public static string FileForHuntStats = Path.Combine(GlobalVars.PathToConfigs, "HuntStats.txt");
        public static string FileForSession = Path.Combine(GlobalVars.PathToConfigs, "session.json");
        public static string FileForKeys = Path.Combine(GlobalVars.PathToConfigs, "keys.json");

        public static string FileForDeviceSettings = Path.Combine(GlobalVars.PathToDevices, "DeviceInfo.txt");
        public static string FileForDeviceData = Path.Combine(GlobalVars.PathToDevices, "DeviceData.json");

        public static string FileForPokemonsCaught = Path.Combine(GlobalVars.PathToLogs, "Pokelog.txt");
        public static string FileForTransfers = Path.Combine(GlobalVars.PathToLogs, "TransferLog.txt");
        public static string FileForEvolve = Path.Combine(GlobalVars.PathToLogs, "EvolveLog.txt");
        public static string FileForAppLog = Path.Combine(GlobalVars.PathToLogs, "log.txt");
        public static string FileForEggs = Path.Combine(GlobalVars.PathToLogs, "EggLog.txt");

        // Other Globals
        public static string pFHashKey;
        public static string ProfileName = "DefaultProfile";
        public static bool IsDefault = false;
        public static int RunOrder = 0;
        public static string SettingsJSON = "";
        public static AuthType acc = AuthType.Google;
        public static string email = "empty";
        public static string Password = "empty";
        public static long LastTimePlayedTs = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
        public static bool WalkBackToDefaultLocation = true;
        public static bool uselastcoords = true;
        public static double latitude = 40.764883;
        public static double longitude = -73.972967;
        public static double altitude = 15.173855;
        public static string LocaleCountry ="";
        public static string LocaleLanguage ="";
        public static string LocaleTimeZone ="";

        public static double WalkingSpeedInKilometerPerHour = 5;
        public static double MinWalkSpeed = 3;
        public static int radius = 5000;
        public static bool TransferDoublePokemons = true;
        public static bool TransferSlashPokemons = true;
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
        public static int MaxPinapBerries = 100;
        public static int MaxNanabBerries = 100;
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
        public static List<PokemonId> pokemonsToAlwaysTransfer;
        public static List<PokemonId> catchPokemonSkipList;
        public static List<PokemonId> pokemonsToEvolve;
        public static List<PokemonId> ToSnipe;
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
        public static bool SkipRadius = false;
        public static bool UseBreakFields = false;
        public static int BreakInterval = 0;
        public static int BreakLength = 0;
        public static bool AdvancedBreaks = false;
        public static List<BreakSettings> Breaks;
        public static bool ForceAdvancedBreakNow = false;
        public static double TimeToRun;
        public static int RestartAfterRun;
        public static int PokemonCatchLimit = 1000;
        public static int PokestopFarmLimit = 2000;
        public static int XPFarmedLimit = 150000;
        public static int navigation_option = 1;
        public static bool UseLuckyEgg = true;
        public static bool UseIncense = true;
        public static bool UseRazzBerry = true;
        public static double razzberry_chance = 0.35;
        public static int NanabPercent;
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
        public static bool LogPokemons = false;
        public static bool LogTransfer = false;
        public static bool LogEvolve = false;
        public static bool LogEggs = false;
        public static LinkedList<GeoCoordinate> NextDestinationOverride = new LinkedList<GeoCoordinate>();
        public static LinkedList<GeoCoordinate> RouteToRepeat = new LinkedList<GeoCoordinate>();
        public static bool RepeatUserRoute = false;
        public static bool UseLureGUIClick = false;
        public static bool UseLuckyEggGUIClick = false;
        public static bool UseIncenseGUIClick = false;
        public static bool ForceReloginClick = false;
        public static bool RelocateDefaultLocation = false;
        public static double RelocateDefaultLocationTravelSpeed = 0;
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
        public static int MinProbToCatch = 0;
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

        public static bool CollectDailyBonus;
        
        public static bool SnipePokemon;
        public static bool ForceSnipe = false;

        public static bool PauseTheWalking = false;
        
        public static ManualSnipePokemon SnipeOpts = new ManualSnipePokemon();
        public static ProxySettings proxySettings = new ProxySettings();
      
        public static bool UseLastCords = false;
        public static bool BlockAltitude = false;
        public static bool UseSpritesFolder = false;
        public static bool ShowPokemons = false;
        public static int EvolveAt = 0;
        public static string FortsFile = "";
        public static bool SaveForts = false;

        public static bool UseNanabBerry = false;
        
        public static string DeviceTradeName ="";
        public static string DeviceID ="";

        public static int MaxDragonScale =100;
        public static int MaxSunStone =100;
        public static int MaxKingsRock =100;
        public static int MaxMetalCoat =100;
        public static int MaxUpGrade =100;
        
        public static GymSettings Gyms = new GymSettings();
        public static DebugSettings Debug = new DebugSettings();
        public static bool CompleteTutorial = false;

        public static bool SaveLocations =false;
        public static int MinIVSave = 100;
        public static string SaveLocationsFile = "";
        public static bool ShowStats = false;

        public static bool ContinueLatestSession =false;
        public static bool WalkInLoop = false;
        public static bool WalkRandomly = false;

        public static List<PokemonId> PokemonPinap;

        public static bool SendToDiscord ;
        public static string DiscordUser ;
        public static string DiscordPassword;
        public static ulong DiscordServerID;
        
        public static bool UseItemAtEvolve = true;


        /// <summary>
        /// Copy all values from ProfileSettings to GlobalVars.
        /// </summary>
        /// <param name="settings"></param>
        public static void Assign(ProfileSettings settings){
            foreach (var property in settings.GetType().GetProperties()) {
                var fieldname = property.Name;
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
                if (fieldname == "MaxWalkingRadiusInMeters")
                    fieldname = "radius";
                try {
                    typeof(GlobalVars).GetField(fieldname).SetValue(null,
                         property.GetValue(settings));
                } catch (Exception ex1) {
                    
                    Logger.ExceptionInfo($"setting {fieldname}: {ex1.ToString()}");
                }
            }
        }
        public static ProfileSettings GetSettings()
        {   
            var settings = new ProfileSettings();
            foreach (var property in settings.GetType().GetProperties()) {
                var fieldname = property.Name;
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
                if (fieldname == "MaxWalkingRadiusInMeters")
                    fieldname = "radius";
                try {
                    property.SetValue(settings,typeof(GlobalVars)
                                   .GetField(fieldname).GetValue(null));
                } catch (Exception ex1) {
                    Logger.ExceptionInfo($"setting {fieldname}: {ex1.ToString()}");
                }
            }
            return settings;
        }
        public static ICollection<KeyValuePair<ItemId, int>> GetItemFilter()
        {
                return new[]
                {
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, Shared.GlobalVars.MaxPokeballs),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, Shared.GlobalVars.MaxGreatballs),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, Shared.GlobalVars.MaxUltraballs),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRevive, Shared.GlobalVars.MaxRevives),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPotion, Shared.GlobalVars.MaxPotions),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, Shared.GlobalVars.MaxSuperPotions),
                    new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, Shared.GlobalVars.MaxHyperPotions),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, Shared.GlobalVars.MaxBerries),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, Shared.GlobalVars.MaxTopPotions),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, Shared.GlobalVars.MaxTopRevives),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPinapBerry, Shared.GlobalVars.MaxPinapBerries),
                    new KeyValuePair<ItemId, int>(ItemId.ItemNanabBerry, Shared.GlobalVars.MaxNanabBerries),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSunStone, Shared.GlobalVars.MaxSunStone),
                    new KeyValuePair<ItemId, int>(ItemId.ItemKingsRock, Shared.GlobalVars.MaxKingsRock),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMetalCoat, Shared.GlobalVars.MaxMetalCoat),
                    new KeyValuePair<ItemId, int>(ItemId.ItemDragonScale, Shared.GlobalVars.MaxDragonScale),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUpGrade, Shared.GlobalVars.MaxUpGrade)
                };
        }

    }
}
