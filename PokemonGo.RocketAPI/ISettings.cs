using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;

namespace PokemonGo.RocketAPI
{    
    public interface ISettings
    { 
 
        /* ===================================[SETTINGS]================================= */
        
        /* AUTHENTICATION */
        string ProfileName { get; set; }
        bool IsDefault { get; set; }
        AuthType AuthType { get; set; }
        string PtcPassword { get; set; }
        string PtcUsername { get; set; }
        string GoogleUsername { get; set; }
        string GooglePassword { get; set; }
        string GoogleRefreshToken { get; set; }
        bool UsePwdEncryption { get; set; }

        /* COORDINATES & LOCATION */
        double DefaultLatitude { get; set; }
        double DefaultLongitude { get; set; }
        double DefaultAltitude { get; set; }
        bool UseLastCords { get; set; }
        bool WalkBackToDefaultLocation { get; set; }
        double RelocateDefaultLocationTravelSpeed { get; set; }
        bool RelocateDefaultLocation { get; set; }
        int MaxWalkingRadiusInMeters { get; set; }

        /* NAVIGATION */
        bool FarmPokestops { get; set; }
        bool CatchPokemon { get; set; }
        bool UseLureAtBreak { get; set; }
        bool UseGoogleMapsAPI { get; set; }
        bool pauseAtPokeStop { get; set; }
        bool BreakAtLure { get; set; }
        bool RandomReduceSpeed { get; set; }
        bool UseBreakFields { get; set; }
        bool Espiral { get; set; }
        bool RepeatUserRoute { get; set; }
        int XPFarmedLimit { get; set; }
        int BreakInterval { get; set; }
        int BreakLength { get; set; }
        int MinWalkSpeed { get; set; }
        int PokemonCatchLimit { get; set; }
        int PokestopFarmLimit { get; set; }
        int navigation_option { get; set; }
        double WalkingSpeedInKilometerPerHour { get; set; }


        LinkedList<GeoCoordinate> NextDestinationOverride { get; set; }
        LinkedList<GeoCoordinate> RouteToRepeat { get; set; }

        /* EVOLVE */
        bool EvolvePokemonsIfEnoughCandy { get; set; }

        /* TRANSFER */
        bool TransferDoublePokemons { get; set; }
        bool TransferFirstLowIV { get; set; }
        int DontTransferWithCPOver { get; set; }

        /* POKEMONS */
        bool sleepatpokemons { get; set; }
        bool keepPokemonsThatCanEvolve { get; set; }
        int HoldMaxDoublePokemons { get; set; }

        /* ITEMS */
        bool UseLuckyEgg { get; set;}
        bool UseRazzBerry { get; set; }
        double razzberry_chance { get; set; }
        bool UseLuckyEggIfNotRunning { get; set; }
        bool UseIncense { get; set; }

        /* EGG HATCHING */
        bool AutoIncubate { get; set; }
        bool UseBasicIncubators { get; set; }

        /* PROXIES */
        bool UseProxyVerified { get; set; }
        bool UseProxyAuthentication { get; set; }
        int UseProxyPort { get; set; }
        string UseProxyHost { get; set; }
        string UseProxyUsername { get; set; }
        string UseProxyPassword { get; set; }

        /* THROW SETTINGS */
        int Pb_Excellent { get; set; }
        int Pb_Great { get; set; }
        int Pb_Nice { get; set; }
        int Pb_Ordinary { get; set; }

        /* LOGGING */
        bool logPokemons { get; set; }
        bool logManualTransfer { get; set; }
        bool bLogEvolve { get; set; }
        bool logEggs { get; set; }

        /* UPDATES */
        bool AutoUpdate { get; set; }
        bool CheckWhileRunning { get; set; }

        /* TELEGRAM */
        int TelegramLiveStatsDelay { get; set; }
        string TelegramAPIToken { get; set; }
        string TelegramName { get; set; }

        /* MISC */
        bool pokevision { get; set; }
        bool LogPokemon { get; set; }
        bool LogTransfer { get; set; }
        bool LogEvolve { get; set; }
        bool LogEggs { get; set; }
        bool UseAnimationTimes { get; set; }
        bool MapLoaded { get; set; }
        bool pauseTheWalking { get; set; }
        bool pauseAtEvolve2 { get; set; }
        bool UseLureGUIClick { get; set; }
        bool UseIncenseGUIClick { get; set; }
        bool UseLuckyEggGUIClick { get; set; }
        bool LimitPokeballUse { get; set; }
        bool LimitGreatballUse { get; set; }
        bool LimitUltraballUse { get; set; }
        int Max_Missed_throws { get; set; }
        int MinCPforGreatBall { get; set; }
        int MinCPforUltraBall { get; set; }
        int InventoryBasePokeball { get; set; }
        int InventoryBaseGreatball { get; set; }
        int InventoryBaseUltraball { get; set; }
        double TimeToRun { get; set; }
        int ivmaxpercent { get; set; }
        string SelectedLanguage { get; set; }
        string GoogleMapsAPIKey { get; set; }
        ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter { get; set; }
        int MaxPokeballs { get; set; }
        int MaxGreatballs { get; set; }
        int MaxUltraballs { get; set; }
        int MaxRevives { get; set; }
        int MaxTopRevives { get; set; }
        int MaxPotions { get; set; }
        int MaxSuperPotions { get; set; }
        int MaxHyperPotions { get; set; }
        int MaxTopPotions { get; set; }
        int MaxBerries { get; set; }
        IList<PokemonId> pokemonsToHold { get; set; }
        IList<PokemonId> pokemonsToEvolve { get; set; }
        IList<PokemonId> catchPokemonSkipList { get; set; }

        /* ===================================[SETTINGS]================================= */
    }
}
