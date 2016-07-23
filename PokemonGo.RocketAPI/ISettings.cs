using AllEnum;
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

        double WalkingSpeedInKilometerPerHour { get; }

        bool EvolvePokemonsIfEnoughCandy { get; }
        bool TransferDoublePokemons { get; }

        string TelegramAPIToken { get; }
        string TelegramName { get; }

        double StepDistance { get; }

        ICollection<KeyValuePair<AllEnum.ItemId, int>> itemRecycleFilter { get; set; }

        List<PokemonId> pokemonsToHold { get; set; }
        List<PokemonId> pokemonsToEvolve { get; set; }
        
    }
}