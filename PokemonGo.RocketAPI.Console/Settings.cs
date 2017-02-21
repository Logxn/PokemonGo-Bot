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
        #region HashInfo
        public ByteString SessionHash
        {
            get { return GlobalVars.SessionHash; }
            set { GlobalVars.SessionHash = value; }
        }
        public string pFHashKey
        {
            get { return GlobalVars.pFHashKey; }
            set { GlobalVars.pFHashKey = value; }
        }
        #endregion

        #region Authentication & Profile
        public string ProfileName
        {
            get { return GlobalVars.ProfileName; }
            set { GlobalVars.ProfileName = value; }
        }
        public bool IsDefault
        {
            get { return GlobalVars.IsDefault; }
            set { GlobalVars.IsDefault = value; }
        }
        public Enums.AuthType AuthType
        {
            get { return GlobalVars.acc; }
            set { GlobalVars.acc = value; }
        }
        public string Username
        {
            get { return GlobalVars.email; }
            set { GlobalVars.email = value; }
        }
        public string Password
        {
            get { return GlobalVars.Password; }
            set { GlobalVars.Password = value; }
        }
        #endregion

        /* COORDINATES & LOCATION */
        public double DefaultLatitude
        {
            get { return GlobalVars.latitude; }
            set { GlobalVars.latitude = value; }
        }
        public double DefaultLongitude
        {
            get { return GlobalVars.longitude; }
            set { GlobalVars.longitude = value; }
        }
        public double DefaultAltitude
        {
            get { return GlobalVars.altitude; }
            set { GlobalVars.altitude = value; }
        }
        public bool UseLastCords
        {
            get { return GlobalVars.uselastcoords; }
            set { GlobalVars.uselastcoords = value; }
        }// Only disable this if your sure what you're doing!
        public bool WalkBackToDefaultLocation
        {
            get { return GlobalVars.WalkBackToDefaultLocation; }
            set { GlobalVars.WalkBackToDefaultLocation = value; }
        }
        public double RelocateDefaultLocationTravelSpeed
        {
            get { return GlobalVars.RelocateDefaultLocationTravelSpeed; }
            set { GlobalVars.RelocateDefaultLocationTravelSpeed = value; }
        }
        public bool RelocateDefaultLocation
        {
            get { return GlobalVars.RelocateDefaultLocation; }
            set { GlobalVars.RelocateDefaultLocation = value; }
        }
        public int MaxWalkingRadiusInMeters
        {
            get { return GlobalVars.radius; }
            set { GlobalVars.radius = value; }
        }

        /* NAVIGATION */
        public bool FarmPokestops
        {
            get { return GlobalVars.FarmPokestops; }
            set { GlobalVars.FarmPokestops = value; }
        }
        public bool CatchPokemon
        {
            get { return GlobalVars.CatchPokemon; }
            set { GlobalVars.CatchPokemon = value; }
        }
        public double WalkingSpeedInKilometerPerHour
        {
            get { return GlobalVars.WalkingSpeedInKilometerPerHour; }
            set { GlobalVars.WalkingSpeedInKilometerPerHour = value; }
        }
        public int MinWalkSpeed
        {
            get { return GlobalVars.MinWalkSpeed; }
            set { GlobalVars.MinWalkSpeed = value; }
        }
        public bool TransferDoublePokemons
        {
            get { return GlobalVars.TransferDoublePokemons; }
            set { GlobalVars.TransferDoublePokemons = value; }
        }
        public int HoldMaxDoublePokemons
        {
            get { return GlobalVars.HoldMaxDoublePokemons; }
            set { GlobalVars.HoldMaxDoublePokemons = value; }
        }
        public bool EvolvePokemonsIfEnoughCandy
        {
            get { return GlobalVars.EvolvePokemonsIfEnoughCandy; }
            set { GlobalVars.EvolvePokemonsIfEnoughCandy = value; }
        }
        public int DontTransferWithCPOver
        {
            get { return GlobalVars.DontTransferWithCPOver; }
            set { GlobalVars.DontTransferWithCPOver = value; }
        }
        public int excellentthrow
        {
            get { return GlobalVars.excellentthrow; }
            set { GlobalVars.excellentthrow = value; }
        }

        public int greatthrow
        {
            get { return GlobalVars.greatthrow; }
            set { GlobalVars.greatthrow = value; }
        }

        public int nicethrow
        {
            get { return GlobalVars.nicethrow; }
            set { GlobalVars.nicethrow = value; }
        }

        public int ordinarythrow
        {
            get { return GlobalVars.ordinarythrow; }
            set { GlobalVars.ordinarythrow = value; }
        }
        public int MaxPokeballs
        {
            get { return GlobalVars.MaxPokeballs; }
            set { GlobalVars.MaxPokeballs = value; }
        }
        public int MaxGreatballs
        {
            get { return GlobalVars.MaxGreatballs; }
            set { GlobalVars.MaxGreatballs = value; }
        }
        public int MaxUltraballs
        {
            get { return GlobalVars.MaxUltraballs; }
            set { GlobalVars.MaxUltraballs = value; }
        }
        public int MaxRevives
        {
            get { return GlobalVars.MaxRevives; }
            set { GlobalVars.MaxRevives = value; }
        }
        public int MaxTopRevives
        {
            get { return GlobalVars.MaxTopRevives; }
            set { GlobalVars.MaxTopRevives = value; }
        }
        public int MaxPotions
        {
            get { return GlobalVars.MaxPotions; }
            set { GlobalVars.MaxPotions = value; }
        }
        public int MaxSuperPotions
        {
            get { return GlobalVars.MaxSuperPotions; }
            set { GlobalVars.MaxSuperPotions = value; }
        }
        public int MaxHyperPotions
        {
            get { return GlobalVars.MaxHyperPotions; }
            set { GlobalVars.MaxHyperPotions = value; }
        }
        public int MaxTopPotions
        {
            get { return GlobalVars.MaxTopPotions; }
            set { GlobalVars.MaxTopPotions = value; }
        }
        public int MaxBerries
        {
            get { return GlobalVars.MaxBerries; }
            set { GlobalVars.MaxBerries = value; }
        }
        public bool UseIncense
        {
            get { return GlobalVars.UseIncense; }
            set { GlobalVars.UseIncense = value; }
        }

        public int TelegramLiveStatsDelay
        {
            get { return GlobalVars.TelegramLiveStatsDelay; }
            set { GlobalVars.TelegramLiveStatsDelay = value; }
        }
        public bool sleepatpokemons
        {
            get { return GlobalVars.sleepatpokemons; }
            set { GlobalVars.sleepatpokemons = value; }
        }
        public int ivmaxpercent
        {
            get { return GlobalVars.ivmaxpercent; }
            set { GlobalVars.ivmaxpercent = value; }
        }
        public int MinCPforGreatBall
        {
            get { return GlobalVars.MinCPforGreatBall; }
            set { GlobalVars.MinCPforGreatBall = value; }
        }
        public int MinCPtoCatch
        {
            get { return GlobalVars.MinCPtoCatch; }
            set { GlobalVars.MinCPtoCatch = value; }
        }
        public int MinIVtoCatch
        {
            get { return GlobalVars.MinIVtoCatch; }
            set { GlobalVars.MinIVtoCatch = value; }
        }
        public int MinCPforUltraBall
        {
            get { return GlobalVars.MinCPforUltraBall; }
            set { GlobalVars.MinCPforUltraBall = value; }
        }
        public string TelegramAPIToken
        {
            get { return GlobalVars.TelegramAPIToken; }
            set { GlobalVars.TelegramAPIToken = value; }
        }
        public string TelegramName
        {
            get { return GlobalVars.TelegramName; }
            set { GlobalVars.TelegramName = value; }
        }
        public bool AvoidRegionLock
        {
            get { return GlobalVars.AvoidRegionLock; }
            set { GlobalVars.AvoidRegionLock = value; }
        }
        public int navigation_option
        {
            get { return GlobalVars.navigation_option; }
            set { GlobalVars.navigation_option = value; }
        }
        public bool UseLuckyEgg
        {
            get { return GlobalVars.UseLuckyEgg; }
            set { GlobalVars.UseLuckyEgg = value; }
        }
        public bool UseRazzBerry
        {
            get { return GlobalVars.UseRazzBerry; }
            set { GlobalVars.UseRazzBerry = value; }
        }
        public double razzberry_chance
        {
            get { return GlobalVars.razzberry_chance; }
            set { GlobalVars.razzberry_chance = value; }
        }
        public bool keepPokemonsThatCanEvolve
        {
            get { return GlobalVars.keepPokemonsThatCanEvolve; }
            set { GlobalVars.keepPokemonsThatCanEvolve = value; }
        }
        public bool TransferFirstLowIV
        {
            get { return GlobalVars.TransferFirstLowIV; }
            set { GlobalVars.TransferFirstLowIV = value; }
        }
        public bool UseBasicIncubators
        {
            get { return GlobalVars.UseBasicIncubators; }
            set { GlobalVars.UseBasicIncubators = value; }
        }
        public bool pokevision
        {
            get { return GlobalVars.pokevision; }
            set { GlobalVars.pokevision = value; }
        }
        public bool AutoIncubate
        {
            get { return GlobalVars.AutoIncubate; }
            set { GlobalVars.AutoIncubate = value; }
        }
        public bool UseLuckyEggIfNotRunning
        {
            get { return GlobalVars.UseLuckyEggIfNotRunning; }
            set { GlobalVars.UseLuckyEggIfNotRunning = value; }
        }

        public bool SnipePokemon
        {
            get { return GlobalVars.SnipePokemon; }
            set { GlobalVars.SnipePokemon = value; }
        }
        public bool Espiral
        {
            get { return GlobalVars.Espiral; }
            set { GlobalVars.Espiral = value; }
        }
        public bool MapLoaded
        {
            get { return GlobalVars.MapLoaded; }
            set { GlobalVars.MapLoaded = value; }
        }
        public string SelectedLanguage
        {
            get { return GlobalVars.SelectedLanguage; }
            set { GlobalVars.SelectedLanguage = value; }
        }

        public List<PokemonId> catchPokemonSkipList
        {
            get { return GlobalVars.catchPokemonSkipList; }
            set { GlobalVars.catchPokemonSkipList = value; }
        }

        public List<PokemonId> pokemonsToHold
        {
            get { return GlobalVars.pokemonsToHold; }
            set { GlobalVars.pokemonsToHold = value; }

        }

        public List<PokemonId> pokemonsToEvolve
        {
            get { return GlobalVars.pokemonsToEvolve; }
            set { GlobalVars.pokemonsToEvolve = value; }
        }

        public List<PokemonId> NotToSnipe
        {
            get { return GlobalVars.NotToSnipe; }
            set { GlobalVars.NotToSnipe = value; }
        }


        public bool pauseAtPokeStop
        {
            get { return GlobalVars.pauseAtPokeStop; }
            set { GlobalVars.pauseAtPokeStop = value; }
        }
        public bool BreakAtLure
        {
            get { return GlobalVars.BreakAtLure; }
            set { GlobalVars.BreakAtLure = value; }
        }
        public bool UseAnimationTimes
        {
            get { return GlobalVars.UseAnimationTimes; }
            set { GlobalVars.UseAnimationTimes = value; }
        }
        public bool UseLureAtBreak
        {
            get { return GlobalVars.UseLureAtBreak; }
            set { GlobalVars.UseLureAtBreak = value; }
        }
        public bool UseGoogleMapsAPI
        {
            get { return GlobalVars.UseGoogleMapsAPI; }
            set { GlobalVars.UseGoogleMapsAPI = value; }
        }
        public string GoogleMapsAPIKey
        {
            get { return GlobalVars.GoogleMapsAPIKey; }
            set { GlobalVars.GoogleMapsAPIKey = value; }
        }
        public bool RandomReduceSpeed
        {
            get { return GlobalVars.RandomReduceSpeed; }
            set { GlobalVars.RandomReduceSpeed = value; }
        }
        public double TimeToRun
        {
            get { return GlobalVars.TimeToRun; }
            set { GlobalVars.TimeToRun = value; }
        }
        public int PokemonCatchLimit
        {
            get { return GlobalVars.PokemonCatchLimit; }
            set { GlobalVars.PokemonCatchLimit = value; }
        }
        public int PokestopFarmLimit
        {
            get { return GlobalVars.PokestopFarmLimit; }
            set { GlobalVars.PokestopFarmLimit = value; }
        }
        public int XPFarmedLimit
        {
            get { return GlobalVars.XPFarmedLimit; }
            set { GlobalVars.XPFarmedLimit = value; }
        }
        public int BreakInterval
        {
            get { return GlobalVars.BreakInterval; }
            set { GlobalVars.BreakInterval = value; }
        }
        public int BreakLength
        {
            get { return GlobalVars.BreakLength; }
            set { GlobalVars.BreakLength = value; }
        }
        public bool UseBreakFields
        {
            get { return GlobalVars.UseBreakFields; }
            set { GlobalVars.UseBreakFields = value; }
        }
        bool ISettings.pauseAtEvolve2
        {
            get { return GlobalVars.pauseAtEvolve2; }
            set { GlobalVars.pauseAtEvolve2 = value; }
        }
        bool ISettings.Espiral
        {
            get { return GlobalVars.Espiral; }
            set { GlobalVars.Espiral = value; }
        }
        bool ISettings.CheckWhileRunning
        {
            get { return GlobalVars.CheckWhileRunning; }
            set { GlobalVars.CheckWhileRunning = value; }
        }
        bool ISettings.AutoUpdate
        {
            get { return GlobalVars.AutoUpdate; }
            set { GlobalVars.AutoUpdate = value; }
        }
        public LinkedList<GeoCoordinate> NextDestinationOverride
        {
            get { return GlobalVars.NextDestinationOverride; }
            set { GlobalVars.NextDestinationOverride = value; }
        }
        public LinkedList<GeoCoordinate> RouteToRepeat
        {
            get { return GlobalVars.RouteToRepeat; }
            set { GlobalVars.RouteToRepeat = value; }
        }
        public bool RepeatUserRoute
        {
            get { return GlobalVars.RepeatUserRoute; }
            set { GlobalVars.RepeatUserRoute = value; }
        }
        public bool UseLureGUIClick
        {
            get { return GlobalVars.UseLureGUIClick; }
            set { GlobalVars.UseLureGUIClick = value; }
        }
        public bool UseLuckyEggGUIClick
        {
            get { return GlobalVars.UseLuckyEggGUIClick; }
            set { GlobalVars.UseLuckyEggGUIClick = value; }
        }
        public bool UseIncenseGUIClick
        {
            get { return GlobalVars.UseIncenseGUIClick; }
            set { GlobalVars.UseIncenseGUIClick = value; }
        }
        public bool LimitPokeballUse
        {
            get { return GlobalVars.LimitPokeballUse; }
            set { GlobalVars.LimitPokeballUse = value; }
        }
        public bool LimitGreatballUse
        {
            get { return GlobalVars.LimitGreatballUse; }
            set { GlobalVars.LimitGreatballUse = value; }
        }
        public bool LimitUltraballUse
        {
            get { return GlobalVars.LimitUltraballUse; }
            set { GlobalVars.LimitUltraballUse = value; }
        }
        public int Max_Missed_throws
        {
            get { return GlobalVars.Max_Missed_throws; }
            set { GlobalVars.Max_Missed_throws = value; }
        }
        public bool LogPokemons
        {
            get { return GlobalVars.LogPokemons; }
            set { GlobalVars.LogPokemons = value; }
        }
        public bool LogTransfer
        {
            get { return GlobalVars.LogTransfer; }
            set { GlobalVars.LogTransfer = value; }
        }
        public bool LogEvolve
        {
            get { return GlobalVars.LogEvolve; }
            set { GlobalVars.LogEvolve = value; }
        }
        public bool LogEggs
        {
            get { return GlobalVars.LogEggs; }
            set { GlobalVars.LogEggs = value; }
        }
        public bool AutoUpdate
        {
            get { return GlobalVars.AutoUpdate; }
            set { GlobalVars.AutoUpdate = value; }
        }
        public bool CheckWhileRunning
        {
            get { return GlobalVars.CheckWhileRunning; }
            set { GlobalVars.CheckWhileRunning = value; }
        }
        public int InventoryBasePokeball
        {
            get { return GlobalVars.InventoryBasePokeball; }
            set { GlobalVars.InventoryBasePokeball = value; }
        }
        public int InventoryBaseGreatball
        {
            get { return GlobalVars.InventoryBaseGreatball; }
            set { GlobalVars.InventoryBaseGreatball = value; }
        }
        public int InventoryBaseUltraball
        {
            get { return GlobalVars.InventoryBaseUltraball; }
            set { GlobalVars.InventoryBaseUltraball = value; }
        }
        public bool UsePwdEncryption
        {
            get { return GlobalVars.UsePwdEncryption; }
            set { GlobalVars.UsePwdEncryption = value; }
        }
        public bool PauseTheWalking
        {
            get { return GlobalVars.PauseTheWalking; }
            set { GlobalVars.PauseTheWalking = value; }
        }
        public bool pauseAtEvolve
        {
            get { return GlobalVars.pauseAtEvolve; }
            set { GlobalVars.pauseAtEvolve = value; }
        }
        public bool EnablePokeList
        {
            get { return GlobalVars.EnablePokeList; }
            set { GlobalVars.EnablePokeList = value; }
        }
        public bool EnableConsoleInTab
        {
            get { return GlobalVars.EnableConsoleInTab; }
            set { GlobalVars.EnableConsoleInTab = value; }
        }
        public bool ForceSnipe
        {
            get { return GlobalVars.ForceSnipe; }
            set { GlobalVars.ForceSnipe = value; }
        }
        public ManualSnipePokemon ManualSnipePokemonID
        {
            get { return GlobalVars.SnipeOpts; }
            set { GlobalVars.SnipeOpts = value; }
        }
        public bool NextBestBallOnEscape
        {
            get { return GlobalVars.NextBestBallOnEscape; }
            set { GlobalVars.NextBestBallOnEscape = value; }
        }
        public bool simulatedPGO
        {
            get { return GlobalVars.simulatedPGO; }
            set { GlobalVars.simulatedPGO = value; }
        }

        public bool No2kmEggs
        {
            get { return GlobalVars.No2kmEggs; }
            set { GlobalVars.No2kmEggs = value; }
        }

        public bool No5kmEggs
        {
            get { return GlobalVars.No5kmEggs; }
            set { GlobalVars.No5kmEggs = value; }
        }

        public bool No10kmEggs
        {
            get { return GlobalVars.No10kmEggs; }
            set { GlobalVars.No10kmEggs = value; }
        }

        public bool EggsAscendingSelection
        {
            get { return GlobalVars.EggsAscendingSelection; }
            set { GlobalVars.EggsAscendingSelection = value; }
        }
        public bool No2kmEggsBasicInc
        {
            get { return GlobalVars.No2kmEggsBasicInc; }
            set { GlobalVars.No2kmEggsBasicInc = value; }
        }

        public bool No5kmEggsBasicInc
        {
            get { return GlobalVars.No5kmEggsBasicInc; }
            set { GlobalVars.No5kmEggsBasicInc = value; }
        }

        public bool No10kmEggsBasicInc
        {
            get { return GlobalVars.No10kmEggsBasicInc; }
            set { GlobalVars.No10kmEggsBasicInc = value; }
        }

        public bool EggsAscendingSelectionBasicInc
        {
            get { return GlobalVars.EggsAscendingSelectionBasicInc; }
            set { GlobalVars.EggsAscendingSelectionBasicInc = value; }
        }

        public bool EnableVerboseLogging
        {
            get { return GlobalVars.EnableVerboseLogging; }
            set { GlobalVars.EnableVerboseLogging = value; }
        }


        public bool CollectDailyBonus
        {
            get { return GlobalVars.CollectDailyBonus; }
            set { GlobalVars.CollectDailyBonus = value; }
        }

        public ProxySettings proxySettings
        {
            get { return GlobalVars.proxySettings; }
            set { GlobalVars.proxySettings = value; }
        }

        public bool UsePinapBerry
        {
            get { return GlobalVars.UsePinapBerry; }
            set { GlobalVars.UsePinapBerry = value; }
        }

        public bool UseNanabBerry
        {
            get { return GlobalVars.UseNanabBerry; }
            set { GlobalVars.UseNanabBerry = value; }
        }
    }
}
