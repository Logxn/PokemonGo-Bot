using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using System;
using System.Collections.Generic;
using System.Device.Location;
using Google.Protobuf;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console
{
   
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

        public ByteString SessionHash
        {
            get { return GlobalSettings.SessionHash; }
            set { GlobalSettings.SessionHash = value; }
        }

        public string pFHashKey
        {
            get { return GlobalSettings.pFHashKey; }
            set { GlobalSettings.pFHashKey = value;  }
        }
        /* AUTHENTICATION */

        public string ProfileName
        {
            get { return GlobalSettings.ProfileName; }
            set { GlobalSettings.ProfileName = value; }
        }
        public bool IsDefault
        {
            get { return GlobalSettings.IsDefault; }
            set { GlobalSettings.IsDefault = value; }
        }
        public Enums.AuthType AuthType
        {
            get { return GlobalSettings.acc; }
            set { GlobalSettings.acc = value; }
        }
        public string PtcUsername
        {
            get { return GlobalSettings.email; }
            set { GlobalSettings.email = value; }
        }
        public string PtcPassword
        {
            get { return GlobalSettings.password; }
            set { GlobalSettings.password = value; }
        }
        public string GoogleUsername
        {
            get { return GlobalSettings.email; }
            set { GlobalSettings.email = value; }
        }
        public string GooglePassword
        {
            get { return GlobalSettings.password; }
            set { GlobalSettings.password = value; }
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
            get { return GlobalSettings.latitute; }
            set { GlobalSettings.latitute = value; }
        }
        public double DefaultLongitude
        {
            get { return GlobalSettings.longitude; }
            set { GlobalSettings.longitude = value; }
        }
        public double DefaultAltitude
        {
            get { return GlobalSettings.altitude; }
            set { GlobalSettings.altitude = value; }
        }
        public bool UseLastCords
        {
            get { return GlobalSettings.uselastcoords; }
            set { GlobalSettings.uselastcoords = value; }
        }// Only disable this if your sure what you're doing!
        public bool WalkBackToDefaultLocation
        {
            get { return GlobalSettings.defLoc; }
            set { GlobalSettings.defLoc = value; }
        }
        public double RelocateDefaultLocationTravelSpeed
        {
            get { return GlobalSettings.RelocateDefaultLocationTravelSpeed; }
            set { GlobalSettings.RelocateDefaultLocationTravelSpeed = value; }
        }
        public bool RelocateDefaultLocation
        {
            get { return GlobalSettings.RelocateDefaultLocation; }
            set { GlobalSettings.RelocateDefaultLocation = value; }
        }
        public int MaxWalkingRadiusInMeters
        {
            get { return GlobalSettings.radius; }
            set { GlobalSettings.radius = value; }
        }

        /* NAVIGATION */
        public bool FarmPokestops
        {
            get { return GlobalSettings.farmPokestops; }
            set { GlobalSettings.farmPokestops = value; }
        }
        public bool CatchPokemon
        {
            get { return GlobalSettings.CatchPokemon; }
            set { GlobalSettings.CatchPokemon = value; }
        }
        public double WalkingSpeedInKilometerPerHour
        {
            get { return GlobalSettings.speed; }
            set { GlobalSettings.speed = value; }
        }
        public int MinWalkSpeed
        {
            get { return GlobalSettings.MinWalkSpeed; }
            set { GlobalSettings.MinWalkSpeed = value; }
        }
        public bool TransferDoublePokemons
        {
            get { return GlobalSettings.transfer; }
            set { GlobalSettings.transfer = value; }
        }
        public int HoldMaxDoublePokemons
        {
            get { return GlobalSettings.duplicate; }
            set { GlobalSettings.duplicate = value; }
        }
        public bool EvolvePokemonsIfEnoughCandy
        {
            get { return GlobalSettings.evolve; }
            set { GlobalSettings.evolve = value; }
        }
        public int DontTransferWithCPOver
        {
            get { return GlobalSettings.maxCp; }
            set { GlobalSettings.maxCp = value; }
        }
        public int Pb_Excellent
        {
            get { return GlobalSettings.excellentthrow; }
            set { GlobalSettings.excellentthrow = value; }
        }

        public int Pb_Great
        {
            get { return GlobalSettings.greatthrow; }
            set { GlobalSettings.greatthrow = value; }
        }

        public int Pb_Nice
        {
            get { return GlobalSettings.nicethrow; }
            set { GlobalSettings.nicethrow = value; }
        }

        public int Pb_Ordinary
        {
            get { return GlobalSettings.ordinarythrow; }
            set { GlobalSettings.ordinarythrow = value; }
        }
        public int MaxPokeballs
        {
            get { return GlobalSettings.pokeball; }
            set { GlobalSettings.pokeball = value; }
        }
        public int MaxGreatballs
        {
            get { return GlobalSettings.greatball; }
            set { GlobalSettings.greatball = value; }
        }
        public int MaxUltraballs
        {
            get { return GlobalSettings.ultraball; }
            set { GlobalSettings.ultraball = value; }
        }
        public int MaxRevives
        {
            get { return GlobalSettings.revive; }
            set { GlobalSettings.revive = value; }
        }
        public int MaxTopRevives
        {
            get { return GlobalSettings.toprevive; }
            set { GlobalSettings.toprevive = value; }
        }
        public int MaxPotions
        {
            get { return GlobalSettings.potion; }
            set { GlobalSettings.potion = value; }
        }
        public int MaxSuperPotions
        {
            get { return GlobalSettings.superpotion; }
            set { GlobalSettings.superpotion = value; }
        }
        public int MaxHyperPotions
        {
            get { return GlobalSettings.hyperpotion; }
            set { GlobalSettings.hyperpotion = value; }
        }
        public int MaxTopPotions
        {
            get { return GlobalSettings.toppotion; }
            set { GlobalSettings.toppotion = value; }
        }
        public int MaxBerries
        {
            get { return GlobalSettings.berry; }
            set { GlobalSettings.berry = value; }
        }
        public bool UseIncense
        {
            get { return GlobalSettings.useincense; }
            set { GlobalSettings.useincense = value; }
        }

        public int TelegramLiveStatsDelay
        {
            get { return GlobalSettings.telDelay; }
            set { GlobalSettings.telDelay = value; }
        }
        public bool sleepatpokemons
        {
            get { return GlobalSettings.sleepatpokemons; }
            set { GlobalSettings.sleepatpokemons = value; }
        }
        public int ivmaxpercent
        {
            get { return GlobalSettings.ivmaxpercent; }
            set { GlobalSettings.ivmaxpercent = value; }
        }
        public int MinCPforGreatBall
        {
            get { return GlobalSettings.MinCPforGreatBall; }
            set { GlobalSettings.MinCPforGreatBall = value; }
        }
        public int MinCPtoCatch
        {
            get { return GlobalSettings.MinCPtoCatch; }
            set { GlobalSettings.MinCPtoCatch = value; }
        }
        public int MinIVtoCatch
        {
            get { return GlobalSettings.MinIVtoCatch; }
            set { GlobalSettings.MinIVtoCatch = value; }
        }
        public int MinCPforUltraBall
        {
            get { return GlobalSettings.MinCPforUltraBall; }
            set { GlobalSettings.MinCPforUltraBall = value; }
        }
        public string TelegramAPIToken
        {
            get { return GlobalSettings.telAPI; }
            set { GlobalSettings.telAPI = value; }
        }
        public string TelegramName
        {
            get { return GlobalSettings.telName; }
            set { GlobalSettings.telName = value; }
        }
        public bool AvoidRegionLock
        {
            get { return GlobalSettings.AvoidRegionLock; }
            set { GlobalSettings.AvoidRegionLock = value; }
        }
        public int navigation_option
        {
            get { return GlobalSettings.navigation_option; }
            set { GlobalSettings.navigation_option = value; }
        }
        public bool UseLuckyEgg
        {
            get { return GlobalSettings.useluckyegg; }
            set { GlobalSettings.useluckyegg = value; }
        }
        public bool UseRazzBerry
        {
            get { return GlobalSettings.userazzberry; }
            set { GlobalSettings.userazzberry = value; }
        }
        public double razzberry_chance
        {
            get { return GlobalSettings.razzberry_chance; }
            set { GlobalSettings.razzberry_chance = value; }
        }
        public bool keepPokemonsThatCanEvolve
        {
            get { return GlobalSettings.keepPokemonsThatCanEvolve; }
            set { GlobalSettings.keepPokemonsThatCanEvolve = value; }
        }
        public bool TransferFirstLowIV
        {
            get { return GlobalSettings.TransferFirstLowIV; }
            set { GlobalSettings.TransferFirstLowIV = value; }
        }
        public bool UseBasicIncubators
        {
            get { return GlobalSettings.useBasicIncubators; }
            set { GlobalSettings.useBasicIncubators = value; }
        }
        public bool pokevision
        {
            get { return GlobalSettings.pokevision; }
            set { GlobalSettings.pokevision = value; }
        }
        public bool AutoIncubate
        {
            get { return GlobalSettings.autoIncubate; }
            set { GlobalSettings.autoIncubate = value; }
        }
        public bool UseLuckyEggIfNotRunning
        {
            get { return GlobalSettings.useLuckyEggIfNotRunning; }
            set { GlobalSettings.useLuckyEggIfNotRunning = value; }
        }

        public bool SnipePokemon
        {
            get { return GlobalSettings.SnipePokemon; }
            set { GlobalSettings.SnipePokemon = value; }
        }
        public bool Espiral
        {
            get { return GlobalSettings.Espiral; }
            set { GlobalSettings.Espiral = value; }
        }
        public bool MapLoaded
        {
            get { return GlobalSettings.MapLoaded; }
            set { GlobalSettings.MapLoaded = value; }
        }
        public string SelectedLanguage
        {
            get { return GlobalSettings.settingsLanguage; }
            set { GlobalSettings.settingsLanguage = value; }
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
            get { return GlobalSettings.noCatch; }
            set { GlobalSettings.noCatch = value; }
        }

        public List<PokemonId> pokemonsToHold
        {
            get { return GlobalSettings.noTransfer; }
            set { GlobalSettings.noTransfer = value; }

        }

        public List<PokemonId> pokemonsToEvolve
        {
            get { return GlobalSettings.doEvolve; }
            set { GlobalSettings.doEvolve = value; }
        }

        public List<PokemonId> NotToSnipe
        {
            get { return GlobalSettings.NotToSnipe; }
            set { GlobalSettings.NotToSnipe = value; }
        }

        public ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter
        {
            get
            {
                //Type and amount to keep
                return new[]
                {
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, GlobalSettings.pokeball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, GlobalSettings.greatball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, GlobalSettings.ultraball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRevive, GlobalSettings.revive),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPotion, GlobalSettings.potion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, GlobalSettings.superpotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, GlobalSettings.hyperpotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, GlobalSettings.berry),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, GlobalSettings.toppotion),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, GlobalSettings.toprevive)
                };
            }
            set
            {
                foreach (KeyValuePair<ItemId, int> item in value)
                {
                    switch (item.Key)
                    {
                        case ItemId.ItemPokeBall:
                            GlobalSettings.pokeball = item.Value;
                            break;
                        case ItemId.ItemGreatBall:
                            GlobalSettings.greatball = item.Value;
                            break;
                        case ItemId.ItemUltraBall:
                            GlobalSettings.ultraball = item.Value;
                            break;
                        case ItemId.ItemRevive:
                            GlobalSettings.revive = item.Value;
                            break;
                        case ItemId.ItemMaxRevive:
                            GlobalSettings.toprevive = item.Value;
                            break;
                        case ItemId.ItemPotion:
                            GlobalSettings.potion = item.Value;
                            break;
                        case ItemId.ItemSuperPotion:
                            GlobalSettings.superpotion = item.Value;
                            break;
                        case ItemId.ItemHyperPotion:
                            GlobalSettings.hyperpotion = item.Value;
                            break;
                        case ItemId.ItemMaxPotion:
                            GlobalSettings.toppotion = item.Value;
                            break;
                        case ItemId.ItemRazzBerry:
                            GlobalSettings.berry = item.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public bool pauseAtPokeStop
        {
            get { return GlobalSettings.pauseAtPokeStop; }
            set { GlobalSettings.pauseAtPokeStop = value; }
        }
        public bool BreakAtLure
        {
            get { return GlobalSettings.BreakAtLure; }
            set { GlobalSettings.BreakAtLure = value; }
        }
        public bool UseAnimationTimes
        {
            get { return GlobalSettings.UseAnimationTimes; }
            set { GlobalSettings.UseAnimationTimes = value; }
        }
        public bool UseLureAtBreak
        {
            get { return GlobalSettings.UseLureAtBreak; }
            set { GlobalSettings.UseLureAtBreak = value; }
        }
        public bool UseGoogleMapsAPI
        {
            get { return GlobalSettings.UseGoogleMapsAPI; }
            set { GlobalSettings.UseGoogleMapsAPI = value; }
        }
        public string GoogleMapsAPIKey
        {
            get { return GlobalSettings.GoogleMapsAPIKey; }
            set { GlobalSettings.GoogleMapsAPIKey = value; }
        }
        public bool RandomReduceSpeed
        {
            get { return GlobalSettings.RandomReduceSpeed; }
            set { GlobalSettings.RandomReduceSpeed = value; }
        }
        public double TimeToRun
        {
            get { return GlobalSettings.TimeToRun; }
            set { GlobalSettings.TimeToRun = value; }
        }
        public int PokemonCatchLimit
        {
            get { return GlobalSettings.PokemonCatchLimit; }
            set { GlobalSettings.PokemonCatchLimit = value; }
        }
        public int PokestopFarmLimit
        {
            get { return GlobalSettings.PokestopFarmLimit; }
            set { GlobalSettings.PokestopFarmLimit = value; }
        }
        public int XPFarmedLimit
        {
            get { return GlobalSettings.XPFarmedLimit; }
            set { GlobalSettings.XPFarmedLimit = value; }
        }
        public int BreakInterval
        {
            get { return GlobalSettings.BreakInterval; }
            set { GlobalSettings.BreakInterval = value; }
        }
        public int BreakLength
        {
            get { return GlobalSettings.BreakLength; }
            set { GlobalSettings.BreakLength = value; }
        }
        public bool UseBreakFields
        {
            get { return GlobalSettings.UseBreakFields; }
            set { GlobalSettings.UseBreakFields = value; }
        }
        bool ISettings.pauseAtEvolve2
        {
            get { return GlobalSettings.pauseAtEvolve2; }
            set { GlobalSettings.pauseAtEvolve2 = value; }
        }
        bool ISettings.Espiral
        {
            get { return GlobalSettings.Espiral; }
            set { GlobalSettings.Espiral = value; }
        }
        bool ISettings.logPokemons
        {
            get { return GlobalSettings.logPokemons; }
            set { GlobalSettings.logPokemons = value; }
        }
        bool ISettings.logManualTransfer
        {
            get { return GlobalSettings.logManualTransfer; }
            set { GlobalSettings.logManualTransfer = value; }
        }
        bool ISettings.bLogEvolve
        {
            get { return GlobalSettings.bLogEvolve; }
            set { GlobalSettings.bLogEvolve = value; }
        }
        bool ISettings.CheckWhileRunning
        {
            get { return GlobalSettings.CheckWhileRunning; }
            set { GlobalSettings.CheckWhileRunning = value; }
        }
        bool ISettings.AutoUpdate
        {
            get { return GlobalSettings.AutoUpdate; }
            set { GlobalSettings.AutoUpdate = value; }
        }
        bool ISettings.logEggs
        {
            get { return GlobalSettings.LogEggs; }
            set { GlobalSettings.LogEggs = value; }
        }
        public LinkedList<GeoCoordinate> NextDestinationOverride
        {
            get { return GlobalSettings.NextDestinationOverride; }
            set { GlobalSettings.NextDestinationOverride = value; }
        }
        public LinkedList<GeoCoordinate> RouteToRepeat
        {
            get { return GlobalSettings.RouteToRepeat; }
            set { GlobalSettings.RouteToRepeat = value; }
        }
        public bool RepeatUserRoute
        {
            get { return GlobalSettings.RepeatUserRoute; }
            set { GlobalSettings.RepeatUserRoute = value; }
        }
        public bool UseLureGUIClick
        {
            get { return GlobalSettings.UseLureGUIClick; }
            set { GlobalSettings.UseLureGUIClick = value; }
        }
        public bool UseLuckyEggGUIClick
        {
            get { return GlobalSettings.UseLuckyEggGUIClick; }
            set { GlobalSettings.UseLuckyEggGUIClick = value; }
        }
        public bool UseIncenseGUIClick
        {
            get { return GlobalSettings.UseIncenseGUIClick; }
            set { GlobalSettings.UseIncenseGUIClick = value; }
        }
        public bool LimitPokeballUse
        {
            get { return GlobalSettings.LimitPokeballUse; }
            set { GlobalSettings.LimitPokeballUse = value; }
        }
        public bool LimitGreatballUse
        {
            get { return GlobalSettings.LimitGreatballUse; }
            set { GlobalSettings.LimitGreatballUse = value; }
        }
        public bool LimitUltraballUse
        {
            get { return GlobalSettings.LimitUltraballUse; }
            set { GlobalSettings.LimitUltraballUse = value; }
        }
        public int Max_Missed_throws
        {
            get { return GlobalSettings.Max_Missed_throws; }
            set { GlobalSettings.Max_Missed_throws = value; }
        }
        public bool LogPokemon
        {
            get { return GlobalSettings.logPokemons; }
            set { GlobalSettings.logPokemons = value; }
        }
        public bool LogTransfer
        {
            get { return GlobalSettings.logManualTransfer; }
            set { GlobalSettings.logManualTransfer = value; }
        }
        public bool LogEvolve
        {
            get { return GlobalSettings.bLogEvolve; }
            set { GlobalSettings.bLogEvolve = value; }
        }
        public bool LogEggs
        {
            get { return GlobalSettings.LogEggs; }
            set { GlobalSettings.LogEggs = value; }
        }
        public bool AutoUpdate
        {
            get { return GlobalSettings.AutoUpdate; }
            set { GlobalSettings.AutoUpdate = value; }
        }
        public bool CheckWhileRunning
        {
            get { return GlobalSettings.CheckWhileRunning; }
            set { GlobalSettings.CheckWhileRunning = value; }
        }
        public int InventoryBasePokeball
        {
            get { return GlobalSettings.InventoryBasePokeball; }
            set { GlobalSettings.InventoryBasePokeball = value; }
        }
        public int InventoryBaseGreatball
        {
            get { return GlobalSettings.InventoryBaseGreatball; }
            set { GlobalSettings.InventoryBaseGreatball = value; }
        }
        public int InventoryBaseUltraball
        {
            get { return GlobalSettings.InventoryBaseUltraball; }
            set { GlobalSettings.InventoryBaseUltraball = value; }
        }
        public bool UsePwdEncryption
        {
            get { return GlobalSettings.usePwdEncryption; }
            set { GlobalSettings.usePwdEncryption = value; }
        }
        public bool pauseAtEvolve
        {
            get { return GlobalSettings.pauseAtEvolve; }
            set { GlobalSettings.pauseAtEvolve = value; }
        }
        public bool EnablePokeList
        {
            get { return GlobalSettings.pokeList; }
            set { GlobalSettings.pokeList = value; }
        }
        public bool EnableConsoleInTab
        {
            get { return GlobalSettings.consoleInTab; }
            set { GlobalSettings.consoleInTab = value; }
        }
        public bool ForceSnipe
        {
            get { return GlobalSettings.ForceSnipe; }
            set { GlobalSettings.ForceSnipe = value; }
        }
        public ManualSnipePokemon ManualSnipePokemonID
        {
            get { return GlobalSettings.SnipeOpts; }
            set { GlobalSettings.SnipeOpts = value; }
        }
        public bool NextBestBallOnEscape
        {
            get { return GlobalSettings.NextBestBallOnEscape; }
            set { GlobalSettings.NextBestBallOnEscape = value; }
        }
        public bool simulatedPGO
        {
            get { return GlobalSettings.simulatedPGO; }
            set { GlobalSettings.simulatedPGO = value; }
        }      
        
        public bool No2kmEggs
        {
            get { return GlobalSettings.No2kmEggs; }
            set { GlobalSettings.No2kmEggs = value; }
        }

        public bool No5kmEggs
        {
            get { return GlobalSettings.No5kmEggs; }
            set { GlobalSettings.No5kmEggs = value; }
        }

        public bool No10kmEggs
        {
            get { return GlobalSettings.No10kmEggs; }
            set { GlobalSettings.No10kmEggs = value; }
        }
        
        public bool EggsAscendingSelection
        {
            get { return GlobalSettings.EggsAscendingSelection; }
            set { GlobalSettings.EggsAscendingSelection = value; }
        }
        public bool No2kmEggsBasicInc
        {
            get { return GlobalSettings.No2kmEggsBasicInc; }
            set { GlobalSettings.No2kmEggsBasicInc = value; }
        }

        public bool No5kmEggsBasicInc
        {
            get { return GlobalSettings.No5kmEggsBasicInc; }
            set { GlobalSettings.No5kmEggsBasicInc = value; }
        }

        public bool No10kmEggsBasicInc
        {
            get { return GlobalSettings.No10kmEggsBasicInc; }
            set { GlobalSettings.No10kmEggsBasicInc = value; }
        }
        
        public bool EggsAscendingSelectionBasicInc
        {
            get { return GlobalSettings.EggsAscendingSelectionBasicInc; }
            set { GlobalSettings.EggsAscendingSelectionBasicInc = value; }
        }

        public bool EnableVerboseLogging
        {
            get { return GlobalSettings.EnableVerboseLogging; }
            set { GlobalSettings.EnableVerboseLogging = value; }
        }

        public bool FarmGyms
        {
            get { return GlobalSettings.farmGyms; }
            set { GlobalSettings.farmGyms = value; }
        }

        public bool CollectDailyBonus
        {
            get { return GlobalSettings.CollectDailyBonus; }
            set { GlobalSettings.CollectDailyBonus = value; }
        }

        public bool PauseTheWalking
        {
            get { return GlobalSettings.PauseTheWalking; }
            set { GlobalSettings.PauseTheWalking = value; }
        }
        
    }
}
