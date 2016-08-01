using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Collections.Generic;

namespace PokemonGo.RocketAPI
{
    public interface ISettings
    {
        AuthType AuthType { get; }
        double DefaultLatitude { get; }
        double DefaultLongitude { get; }
        double DefaultAltitude { get; }
        string GoogleRefreshToken { get; set; }
        string PtcPassword { get; }
        string PtcUsername { get; }

        bool UseLastCords { get; }

        bool WalkBackToDefaultLocation { get; }
        int MaxWalkingRadiusInMeters { get; }
        int HoldMaxDoublePokemons { get; }

        int TelegramLiveStatsDelay { get; }

        double WalkingSpeedInKilometerPerHour { get; }

        bool EvolvePokemonsIfEnoughCandy { get; }
        bool TransferDoublePokemons { get; }

        int DontTransferWithCPOver { get; }
        int ivmaxpercent { get; }

        string TelegramAPIToken { get; }
        string TelegramName { get; }

        int navigation_option { get; }

        bool UseLuckyEgg { get; }
        bool keepPokemonsThatCanEvolve { get; }
        bool UserIncense { get; }

        bool pokevision { get; }

        bool Language { get; }

        ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter { get; set; }

        List<PokemonId> pokemonsToHold { get; set; }
        List<PokemonId> pokemonsToEvolve { get; set; }
        List<PokemonId> catchPokemonSkipList { get; }


    }
}
