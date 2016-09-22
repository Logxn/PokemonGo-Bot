using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;

namespace PokemonGo.RocketAPI.Console
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
    }
    public class SaveLoadStatus
    {
        public bool Status
        { get; set; }
        public string StatusMessage
        { get; set; }
    }    
    public class Settings : ISettings
    {
        /* ===================================[SETTINGS]================================= */

        /* AUTHENTICATION */

        public string ProfileName
        {
            get { return Globals.ProfileName; }
            set { Globals.ProfileName = value; }
        }
        public bool IsDefault
        {
            get { return Globals.IsDefault; }
            set { Globals.IsDefault = value; }
        }
        public Enums.AuthType AuthType
        {
            get { return Globals.acc; }
            set { Globals.acc = value; }
        }
        public string PtcUsername
        {
            get { return Globals.email; }
            set { Globals.email = value; }
        }
        public string PtcPassword
        {
            get { return Globals.password; }
            set { Globals.password = value; }
        }
        public string GoogleUsername
        {
            get { return Globals.email; }
            set { Globals.email = value; }
        }
        public string GooglePassword
        {
            get { return Globals.password; }
            set { Globals.password = value; }
        }
        public string GoogleRefreshToken
        {
            get { return UserSettings.Default.GoogleRefreshToken; }
            set
            {
                UserSettings.Default.GoogleRefreshToken = value;
                UserSettings.Default.Save();
            }
        }

        /* COORDINATES & LOCATION */
        public double DefaultLatitude
        {
            get { return Globals.latitute; }
            set { Globals.latitute = value; }
        }
        public double DefaultLongitude
        {
            get { return Globals.longitude; }
            set { Globals.longitude = value; }
        }
        public double DefaultAltitude
        {
            get { return Globals.altitude; }
            set { Globals.altitude = value; }
        }
        public bool UseLastCords
        {
            get { return Globals.uselastcoords; }
            set { Globals.uselastcoords = value; }
        }// Only disable this if your sure what you're doing!
        public bool WalkBackToDefaultLocation
        {
            get { return Globals.defLoc; }
            set { Globals.defLoc = value; }
        }
        public double RelocateDefaultLocationTravelSpeed
        {
            get { return Globals.RelocateDefaultLocationTravelSpeed; }
            set { Globals.RelocateDefaultLocationTravelSpeed = value; }
        }
        public bool RelocateDefaultLocation
        {
            get { return Globals.RelocateDefaultLocation; }
            set { Globals.RelocateDefaultLocation = value; }
        }
        public int MaxWalkingRadiusInMeters
        {
            get { return Globals.radius; }
            set { Globals.radius = value; }
        }

        /* NAVIGATION */
        public bool FarmPokestops
        {
            get { return Globals.farmPokestops; }
            set { Globals.farmPokestops = value; }
        }
        public bool CatchPokemon
        {
            get { return Globals.CatchPokemon; }
            set { Globals.CatchPokemon = value; }
        }
        public double WalkingSpeedInKilometerPerHour
        {
            get { return Globals.speed; }
            set { Globals.speed = value; }
        }
        public int MinWalkSpeed
        {
            get { return Globals.MinWalkSpeed; }
            set { Globals.MinWalkSpeed = value; }
        }
        public bool TransferDoublePokemons
        {
            get { return Globals.transfer; }
            set { Globals.transfer = value; }
        }
        public int HoldMaxDoublePokemons
        {
            get { return Globals.duplicate; }
            set { Globals.duplicate = value; }
        }
        public bool EvolvePokemonsIfEnoughCandy
        {
            get { return Globals.evolve; }
            set { Globals.evolve = value; }
        }
        public int DontTransferWithCPOver
        {
            get { return Globals.maxCp; }
            set { Globals.maxCp = value; }
        }
        public int Pb_Excellent
        {
            get { return Globals.excellentthrow; }
            set { Globals.excellentthrow = value; }
        }

        public int Pb_Great
        {
            get { return Globals.greatthrow; }
            set { Globals.greatthrow = value; }
        }

        public int Pb_Nice
        {
            get { return Globals.nicethrow; }
            set { Globals.nicethrow = value; }
        }

        public int Pb_Ordinary
        {
            get { return Globals.ordinarythrow; }
            set { Globals.ordinarythrow = value; }
        }
        public int MaxPokeballs
        {
            get { return Globals.pokeball; }
            set { Globals.pokeball = value; }
        }
        public int MaxGreatballs
        {
            get { return Globals.greatball; }
            set { Globals.greatball = value; }
        }
        public int MaxUltraballs
        {
            get { return Globals.ultraball; }
            set { Globals.ultraball = value; }
        }
        public int MaxRevives
        {
            get { return Globals.revive; }
            set { Globals.revive = value; }
        }
        public int MaxTopRevives
        {
            get { return Globals.toprevive; }
            set { Globals.toprevive = value; }
        }
        public int MaxPotions
        {
            get { return Globals.potion; }
            set { Globals.potion = value; }
        }
        public int MaxSuperPotions
        {
            get { return Globals.superpotion; }
            set { Globals.superpotion = value; }
        }
        public int MaxHyperPotions
        {
            get { return Globals.hyperpotion; }
            set { Globals.hyperpotion = value; }
        }
        public int MaxTopPotions
        {
            get { return Globals.toppotion; }
            set { Globals.toppotion = value; }
        }
        public int MaxBerries
        {
            get { return Globals.berry; }
            set { Globals.berry = value; }
        }
        public bool UseIncense
        {
            get { return Globals.useincense; }
            set { Globals.useincense = value; }
        }

        public int TelegramLiveStatsDelay
        {
            get { return Globals.telDelay; }
            set { Globals.telDelay = value; }
        }
        public bool sleepatpokemons
        {
            get { return Globals.sleepatpokemons; }
            set { Globals.sleepatpokemons = value; }
        }
        public int ivmaxpercent
        {
            get { return Globals.ivmaxpercent; }
            set { Globals.ivmaxpercent = value; }
        }
        public int MinCPforGreatBall
        {
            get { return Globals.MinCPforGreatBall; }
            set { Globals.MinCPforGreatBall = value; }
        }
        public int MinCPtoCatch
        {
            get { return Globals.MinCPtoCatch; }
            set { Globals.MinCPtoCatch = value; }
        }
        public int MinIVtoCatch
        {
            get { return Globals.MinIVtoCatch; }
            set { Globals.MinIVtoCatch = value; }
        }
        public int MinCPforUltraBall
        {
            get { return Globals.MinCPforUltraBall; }
            set { Globals.MinCPforUltraBall = value; }
        }
        public string TelegramAPIToken
        {
            get { return Globals.telAPI; }
            set { Globals.telAPI = value; }
        }
        public string TelegramName
        {
            get { return Globals.telName; }
            set { Globals.telName = value; }
        }
        public bool AvoidRegionLock
        {
            get { return Globals.AvoidRegionLock; }
            set { Globals.AvoidRegionLock = value; }
        }
        public int navigation_option
        {
            get { return Globals.navigation_option; }
            set { Globals.navigation_option = value; }
        }
        public bool UseLuckyEgg
        {
            get { return Globals.useluckyegg; }
            set { Globals.useluckyegg = value; }
        }
        public bool UseRazzBerry
        {
            get { return Globals.userazzberry; }
            set { Globals.userazzberry = value; }
        }
        public double razzberry_chance
        {
            get { return Globals.razzberry_chance; }
            set { Globals.razzberry_chance = value; }
        }
        public bool keepPokemonsThatCanEvolve
        {
            get { return Globals.keepPokemonsThatCanEvolve; }
            set { Globals.keepPokemonsThatCanEvolve = value; }
        }
        public bool TransferFirstLowIV
        {
            get { return Globals.TransferFirstLowIV; }
            set { Globals.TransferFirstLowIV = value; }
        }
        public bool UseBasicIncubators
        {
            get { return Globals.useBasicIncubators; }
            set { Globals.useBasicIncubators = value; }
        }
        public bool pokevision
        {
            get { return Globals.pokevision; }
            set { Globals.pokevision = value; }
        }
        public bool AutoIncubate
        {
            get { return Globals.autoIncubate; }
            set { Globals.autoIncubate = value; }
        }
        public bool UseLuckyEggIfNotRunning
        {
            get { return Globals.useLuckyEggIfNotRunning; }
            set { Globals.useLuckyEggIfNotRunning = value; }
        }

        public bool SnipePokemon
        {
            get { return Globals.SnipePokemon; }
            set { Globals.SnipePokemon = value; }
        }
        public bool Espiral
        {
            get { return Globals.Espiral; }
            set { Globals.Espiral = value; }
        }
        public bool MapLoaded
        {
            get { return Globals.MapLoaded; }
            set { Globals.MapLoaded = value; }
        }
        public string SelectedLanguage
        {
            get { return Globals.settingsLanguage; }
            set { Globals.settingsLanguage = value; }
        }
        public string UseProxyHost
        {
            get { return UserSettings.Default.UseProxyHost; }
            set
            {
                UserSettings.Default.UseProxyHost = value;
                UserSettings.Default.Save();
            }
        }

        public int UseProxyPort
        {
            get { return UserSettings.Default.UseProxyPort; }
            set
            {
                UserSettings.Default.UseProxyPort = value;
                UserSettings.Default.Save();
            }
        }

        public string UseProxyUsername
        {
            get { return UserSettings.Default.UseProxyUsername; }
            set
            {
                UserSettings.Default.UseProxyUsername = value;
                UserSettings.Default.Save();
            }
        }

        public string UseProxyPassword
        {
            get { return UserSettings.Default.UseProxyPassword; }
            set
            {
                UserSettings.Default.UseProxyPassword = value;
                UserSettings.Default.Save();
            }
        }

        public bool UseProxyVerified
        {
            get { return UserSettings.Default.UseProxyVerified; }
            set
            {
                UserSettings.Default.UseProxyVerified = value;
                UserSettings.Default.Save();
            }
        }

        public bool UseProxyAuthentication
        {
            get { return UserSettings.Default.UseProxyAuthentication; }
            set
            {
                UserSettings.Default.UseProxyAuthentication = value;
                UserSettings.Default.Save();
            }
        }

        public List<PokemonId> catchPokemonSkipList
        {
            get { return Globals.noCatch; }
            set { Globals.noCatch = value; }
        }

        public List<PokemonId> pokemonsToHold
        {
            get { return Globals.noTransfer; }
            set { Globals.noTransfer = value; }

        }

        public List<PokemonId> pokemonsToEvolve
        {
            get { return Globals.doEvolve; }
            set { Globals.doEvolve = value; }
        }

        public List<PokemonId> NotToSnipe
        {
            get { return Globals.NotToSnipe; }
            set { Globals.NotToSnipe = value; }
        }

        public ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter
        {
            get
            {
                //Type and amount to keep
                return new[]
                {
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, Globals.pokeball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, Globals.greatball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, Globals.ultraball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRevive, Globals.revive),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPotion, Globals.potion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, Globals.superpotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, Globals.hyperpotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, Globals.berry),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, Globals.toppotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, Globals.toprevive)
                };
            }
            set
            {
                foreach (KeyValuePair<ItemId, int> item in value)
                {
                    switch (item.Key)
                    {
                        case ItemId.ItemPokeBall:
                            Globals.pokeball = item.Value;
                            break;
                        case ItemId.ItemGreatBall:
                            Globals.greatball = item.Value;
                            break;
                        case ItemId.ItemUltraBall:
                            Globals.ultraball = item.Value;
                            break;
                        case ItemId.ItemRevive:
                            Globals.revive = item.Value;
                            break;
                        case ItemId.ItemMaxRevive:
                            Globals.toprevive = item.Value;
                            break;
                        case ItemId.ItemPotion:
                            Globals.potion = item.Value;
                            break;
                        case ItemId.ItemSuperPotion:
                            Globals.superpotion = item.Value;
                            break;
                        case ItemId.ItemHyperPotion:
                            Globals.hyperpotion = item.Value;
                            break;
                        case ItemId.ItemMaxPotion:
                            Globals.toppotion = item.Value;
                            break;
                        case ItemId.ItemRazzBerry:
                            Globals.berry = item.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public bool pauseAtPokeStop
        {
            get { return Globals.pauseAtPokeStop; }
            set { Globals.pauseAtPokeStop = value; }
        }
        public bool BreakAtLure
        {
            get { return Globals.BreakAtLure; }
            set { Globals.BreakAtLure = value; }
        }
        public bool UseAnimationTimes
        {
            get { return Globals.UseAnimationTimes; }
            set { Globals.UseAnimationTimes = value; }
        }
        public bool UseLureAtBreak
        {
            get { return Globals.UseLureAtBreak; }
            set { Globals.UseLureAtBreak = value; }
        }
        public bool UseGoogleMapsAPI
        {
            get { return Globals.UseGoogleMapsAPI; }
            set { Globals.UseGoogleMapsAPI = value; }
        }
        public string GoogleMapsAPIKey
        {
            get { return Globals.GoogleMapsAPIKey; }
            set { Globals.GoogleMapsAPIKey = value; }
        }
        public bool RandomReduceSpeed
        {
            get { return Globals.RandomReduceSpeed; }
            set { Globals.RandomReduceSpeed = value; }
        }
        public double TimeToRun
        {
            get { return Globals.TimeToRun; }
            set { Globals.TimeToRun = value; }
        }
        public int PokemonCatchLimit
        {
            get { return Globals.PokemonCatchLimit; }
            set { Globals.PokemonCatchLimit = value; }
        }
        public int PokestopFarmLimit
        {
            get { return Globals.PokestopFarmLimit; }
            set { Globals.PokestopFarmLimit = value; }
        }
        public int XPFarmedLimit
        {
            get { return Globals.XPFarmedLimit; }
            set { Globals.XPFarmedLimit = value; }
        }
        public int BreakInterval
        {
            get { return Globals.BreakInterval; }
            set { Globals.BreakInterval = value; }
        }
        public int BreakLength
        {
            get { return Globals.BreakLength; }
            set { Globals.BreakLength = value; }
        }
        public bool UseBreakFields
        {
            get { return Globals.UseBreakFields; }
            set { Globals.UseBreakFields = value; }
        }
        bool ISettings.pauseAtEvolve2
        {
            get { return Globals.pauseAtEvolve2; }
            set { Globals.pauseAtEvolve2 = value; }
        }
        bool ISettings.Espiral
        {
            get { return Globals.Espiral; }
            set { Globals.Espiral = value; }
        }
        bool ISettings.logPokemons
        {
            get { return Globals.logPokemons; }
            set { Globals.logPokemons = value; }
        }
        bool ISettings.logManualTransfer
        {
            get { return Globals.logManualTransfer; }
            set { Globals.logManualTransfer = value; }
        }
        bool ISettings.bLogEvolve
        {
            get { return Globals.bLogEvolve; }
            set { Globals.bLogEvolve = value; }
        }
        bool ISettings.CheckWhileRunning
        {
            get { return Globals.CheckWhileRunning; }
            set { Globals.CheckWhileRunning = value; }
        }
        bool ISettings.AutoUpdate
        {
            get { return Globals.AutoUpdate; }
            set { Globals.AutoUpdate = value; }
        }
        bool ISettings.logEggs
        {
            get { return Globals.LogEggs; }
            set { Globals.LogEggs = value; }
        }
        public LinkedList<GeoCoordinate> NextDestinationOverride
        {
            get { return Globals.NextDestinationOverride; }
            set { Globals.NextDestinationOverride = value; }
        }
        public LinkedList<GeoCoordinate> RouteToRepeat
        {
            get { return Globals.RouteToRepeat; }
            set { Globals.RouteToRepeat = value; }
        }
        public bool RepeatUserRoute
        {
            get { return Globals.RepeatUserRoute; }
            set { Globals.RepeatUserRoute = value; }
        }
        public bool UseLureGUIClick
        {
            get { return Globals.UseLureGUIClick; }
            set { Globals.UseLureGUIClick = value; }
        }
        public bool UseLuckyEggGUIClick
        {
            get { return Globals.UseLuckyEggGUIClick; }
            set { Globals.UseLuckyEggGUIClick = value; }
        }
        public bool UseIncenseGUIClick
        {
            get { return Globals.UseIncenseGUIClick; }
            set { Globals.UseIncenseGUIClick = value; }
        }
        public bool LimitPokeballUse
        {
            get { return Globals.LimitPokeballUse; }
            set { Globals.LimitPokeballUse = value; }
        }
        public bool LimitGreatballUse
        {
            get { return Globals.LimitGreatballUse; }
            set { Globals.LimitGreatballUse = value; }
        }
        public bool LimitUltraballUse
        {
            get { return Globals.LimitUltraballUse; }
            set { Globals.LimitUltraballUse = value; }
        }
        public int Max_Missed_throws
        {
            get { return Globals.Max_Missed_throws; }
            set { Globals.Max_Missed_throws = value; }
        }
        public bool LogPokemon
        {
            get { return Globals.logPokemons; }
            set { Globals.logPokemons = value; }
        }
        public bool LogTransfer
        {
            get { return Globals.logManualTransfer; }
            set { Globals.logManualTransfer = value; }
        }
        public bool LogEvolve
        {
            get { return Globals.bLogEvolve; }
            set { Globals.bLogEvolve = value; }
        }
        public bool LogEggs
        {
            get { return Globals.LogEggs; }
            set { Globals.LogEggs = value; }
        }
        public bool AutoUpdate
        {
            get { return Globals.AutoUpdate; }
            set { Globals.AutoUpdate = value; }
        }
        public bool CheckWhileRunning
        {
            get { return Globals.CheckWhileRunning; }
            set { Globals.CheckWhileRunning = value; }
        }
        public int InventoryBasePokeball
        {
            get { return Globals.InventoryBasePokeball; }
            set { Globals.InventoryBasePokeball = value; }
        }
        public int InventoryBaseGreatball
        {
            get { return Globals.InventoryBaseGreatball; }
            set { Globals.InventoryBaseGreatball = value; }
        }
        public int InventoryBaseUltraball
        {
            get { return Globals.InventoryBaseUltraball; }
            set { Globals.InventoryBaseUltraball = value; }
        }
        public bool UsePwdEncryption
        {
            get { return Globals.usePwdEncryption; }
            set { Globals.usePwdEncryption = value; }
        }
        bool ISettings.pauseTheWalking
        {
            get { return Globals._pauseTheWalking; }
            set
            {
                if (Logic.Logic._instance != null)
                {
                    Logic.Logic._instance.pauseWalking = value;
                    Globals._pauseTheWalking = value;
                }
            }
        }
        public bool pauseAtEvolve
        {
            get { return Globals.pauseAtEvolve; }
            set { Globals.pauseAtEvolve = value; }
        }
        public bool EnablePokeList
        {
            get { return Globals.pokeList; }
            set { Globals.pokeList = value; }
        }
    }
}
