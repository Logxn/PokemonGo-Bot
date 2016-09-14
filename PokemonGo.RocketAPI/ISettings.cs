using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI.Enums;
using System.Collections.Generic;
using System.Device.Location;

namespace PokemonGo.RocketAPI
{
    public interface ISettings
    { 
 
        /* ===================================[SETTINGS]================================= */
        
        /* AUTHENTICATION */
        AuthType AuthType { get; }
        string PtcPassword { get; }
        string PtcUsername { get; }
        string GoogleUsername { get; }
        string GooglePassword { get; }
        string GoogleRefreshToken { get; set; }

        /* COORDINATES & LOCATION */
        double DefaultLatitude { get; }
        double DefaultLongitude { get; }
        double DefaultAltitude { get; }
        bool UseLastCords { get; }
        bool WalkBackToDefaultLocation { get; }
        double RelocateDefaultLocationTravelSpeed { get; set; }
        bool RelocateDefaultLocation { get; set; }
        int MaxWalkingRadiusInMeters { get; }

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
        int navigation_option { get; }
        double WalkingSpeedInKilometerPerHour { get; }


        LinkedList<GeoCoordinate> NextDestinationOverride { get; set; }
        LinkedList<GeoCoordinate> RouteToRepeat { get; set; }

        /* EVOLVE */
        bool EvolvePokemonsIfEnoughCandy { get; }

        /* TRANSFER */
        bool TransferDoublePokemons { get; }
        bool TransferFirstLowIV { get; }
        int DontTransferWithCPOver { get; }

        /* POKEMONS */
        bool sleepatpokemons { get; }
        bool keepPokemonsThatCanEvolve { get; }
        int HoldMaxDoublePokemons { get; }

        /* ITEMS */
        bool UseLuckyEgg { get; }
        bool UseRazzBerry { get; }
        double razzberry_chance { get; }
        bool UseLuckyEggIfNotRunning { get; }
        bool UseIncense { get; }

        /* EGG HATCHING */
        bool AutoIncubate { get; }
        bool UseBasicIncubators { get; }

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
        bool autoUpdate { get; set; }
        bool CheckWhileRunning { get; set; }

        /* TELEGRAM */
        int TelegramLiveStatsDelay { get; }
        string TelegramAPIToken { get; }
        string TelegramName { get; }

        /* MISC */
        bool pokevision { get; }
        bool UseAnimationTimes { get; set; }
        bool MapLoaded { get; set; }
        bool pauseTheWalking { get; set; }
        bool pauseAtEvolve2 { get; set; }
        bool UseLureGUIClick { get; set; }
        bool UseIncenseGUIClick { get; set; }
        bool UseLuckyEggGUIClick { get; set; }
        //bool CatchLurePokemons { get; set; }
        double TimeToRun { get; set; }
        int ivmaxpercent { get; }
        string SelectedLanguage { get; }
        string GoogleMapsAPIKey { get; }

        ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter { get; set; }
        List<PokemonId> pokemonsToHold { get; set; }
        List<PokemonId> pokemonsToEvolve { get; set; }
        List<PokemonId> catchPokemonSkipList { get; }

        /* ===================================[SETTINGS]================================= */
    }
}
