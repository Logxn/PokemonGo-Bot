namespace PokemonGo.RocketAPI.Console
{
    using System;
    using System.Collections.Generic;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.GeneratedCode;

    public class Settings : ISettings
    {
        public double DefaultAltitude => Globals.altitude; // UserSettings.Default.DefaultAltitude;
        public double DefaultLatitude => Globals.latitute; // UserSettings.Default.DefaultLatitude;
        public double DefaultLongitude => Globals.longitude; // UserSettings.Default.DefaultLongitude;
        public int DontTransferWithCPOver => Globals.maxCp; // UserSettings.Default.DontTransferWithCPOver;

        public bool EvolvePokemonsIfEnoughCandy => Globals.evolve; // UserSettings.Default.EvolvePokemonsIfEnoughCandy;

        public string GoogleRefreshToken
        {
            get
            {
                return UserSettings.Default.GoogleRefreshToken;
            }

            set
            {
                UserSettings.Default.GoogleRefreshToken = value;
                UserSettings.Default.Save();
            }
        }

        public int HoldMaxDoublePokemons => Globals.duplicate; // UserSettings.Default.HoldMaxDoublePokemons;

        public bool Language => Globals.gerNames; // UserSettings.Default.Language;
        public int MaxWalkingRadiusInMeters => Globals.radius; // UserSettings.Default.MaxWalkingRadiusInMeters;

        public int navigation_option => Globals.navigation_option;
        public string PtcPassword => Globals.password; // UserSettings.Default.PtcPassword;

        public string PtcUsername => Globals.username; // UserSettings.Default.PtcUsername;

        public string TelegramAPIToken => Globals.telAPI; // UserSettings.Default.TelegramAPIToken;
        public int TelegramLiveStatsDelay => Globals.telDelay; // UserSettings.Default.TelegramLiveStatsDelay;
        public string TelegramName => Globals.telName; // UserSettings.Default.TelegramName;

        public bool TransferDoublePokemons => Globals.transfer; // UserSettings.Default.TransferDoublePokemons;

        public bool UseLastCords
        {
            get
            {
                return true; // Only disable this if your sure what your doing!
            }
        }

        public bool UseLuckyEgg => Globals.useluckyegg; // UserSettings.Default.UseLuckyEgg;
        public bool UserIncense => Globals.useincense;

        public bool WalkBackToDefaultLocation => Globals.defLoc; // UserSettings.Default.WalkBackToDefaultLocation;

        public double WalkingSpeedInKilometerPerHour => Globals.speed; // UserSettings.Default.WalkingSpeedInKilometerPerHour;

        AuthType ISettings.AuthType
        {
            get
            {
                return Globals.acc;
            }
        }

        List<PokemonId> ISettings.catchPokemonSkipList
        {
            get
            {
                var catchPokemonSkipList = new List<PokemonId>();
                foreach (var pokemon in Globals.noCatch)
                    catchPokemonSkipList.Add(pokemon);

                return catchPokemonSkipList;
            }
        }

        ICollection<KeyValuePair<ItemId, int>> ISettings.itemRecycleFilter
        {
            get
            {
                // Type and amount to keep
                return new[]
                       {
                           new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, Globals.pokeball), new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, Globals.greatball), new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, Globals.ultraball), new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, Globals.masterball), new KeyValuePair<ItemId, int>(ItemId.ItemRevive, Globals.revive), new KeyValuePair<ItemId, int>(ItemId.ItemPotion, Globals.potion), new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, Globals.superpotion), new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, Globals.hyperpotion), new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, Globals.berry), new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, Globals.toppotion), new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, Globals.toprevive)
                       };
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        List<PokemonId> ISettings.pokemonsToEvolve
        {
            get
            {
                var pokemonsToEvolve = new List<PokemonId>();
                foreach (var pokemon in Globals.doEvolve)
                    pokemonsToEvolve.Add(pokemon);

                return pokemonsToEvolve;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        List<PokemonId> ISettings.pokemonsToHold
        {
            get
            {
                // Type and amount to keep
                var pokemonsToHold = new List<PokemonId>();

                foreach (var pokemon in Globals.noTransfer)
                    pokemonsToHold.Add(pokemon);

                return pokemonsToHold;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}