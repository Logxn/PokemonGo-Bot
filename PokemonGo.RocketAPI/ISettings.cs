namespace PokemonGo.RocketAPI
{
    using System.Collections.Generic;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.GeneratedCode;

    public interface ISettings
    {
        AuthType AuthType
        {
            get;
        }

        List<PokemonId> catchPokemonSkipList
        {
            get;
        }

        double DefaultAltitude
        {
            get;
        }

        double DefaultLatitude
        {
            get;
        }

        double DefaultLongitude
        {
            get;
        }

        int DontTransferWithCPOver
        {
            get;
        }

        bool EvolvePokemonsIfEnoughCandy
        {
            get;
        }

        string GoogleRefreshToken
        {
            get;
            set;
        }

        int HoldMaxDoublePokemons
        {
            get;
        }

        ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter
        {
            get;
            set;
        }

        bool Language
        {
            get;
        }

        int MaxWalkingRadiusInMeters
        {
            get;
        }

        int navigation_option
        {
            get;
        }

        List<PokemonId> pokemonsToEvolve
        {
            get;
            set;
        }

        List<PokemonId> pokemonsToHold
        {
            get;
            set;
        }

        string PtcPassword
        {
            get;
        }

        string PtcUsername
        {
            get;
        }

        string TelegramAPIToken
        {
            get;
        }

        int TelegramLiveStatsDelay
        {
            get;
        }

        string TelegramName
        {
            get;
        }

        bool TransferDoublePokemons
        {
            get;
        }

        bool UseLastCords
        {
            get;
        }

        bool UseLuckyEgg
        {
            get;
        }

        bool UserIncense
        {
            get;
        }

        bool WalkBackToDefaultLocation
        {
            get;
        }

        double WalkingSpeedInKilometerPerHour
        {
            get;
        }
    }
}