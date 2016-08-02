using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;

namespace PokemonGo.RocketAPI.Console
{
    public class Settings : ISettings
    {
        AuthType ISettings.AuthType
        {
            get
            {
                return Globals.acc;
            }
        }
         
        public bool UseLastCords
        {
            get
            {
                return true; // Only disable this if your sure what your doing!
            }
        }

        public string PtcUsername => Globals.username;//UserSettings.Default.PtcUsername;
        public string PtcPassword => Globals.password;//UserSettings.Default.PtcPassword;
        public double DefaultLatitude => Globals.latitute;//UserSettings.Default.DefaultLatitude;
        public double DefaultLongitude => Globals.longitude;//UserSettings.Default.DefaultLongitude;
        public double DefaultAltitude => Globals.altitude;//UserSettings.Default.DefaultAltitude;

        public bool WalkBackToDefaultLocation => Globals.defLoc;//UserSettings.Default.WalkBackToDefaultLocation;
        public int MaxWalkingRadiusInMeters => Globals.radius;//UserSettings.Default.MaxWalkingRadiusInMeters;

        public int HoldMaxDoublePokemons => Globals.duplicate;//UserSettings.Default.HoldMaxDoublePokemons;
        public int TelegramLiveStatsDelay => Globals.telDelay;//UserSettings.Default.TelegramLiveStatsDelay;


        public double WalkingSpeedInKilometerPerHour => Globals.speed;//UserSettings.Default.WalkingSpeedInKilometerPerHour;

        public bool TransferDoublePokemons => Globals.transfer;//UserSettings.Default.TransferDoublePokemons;
        public int DontTransferWithCPOver => Globals.maxCp;//UserSettings.Default.DontTransferWithCPOver;
        public int ivmaxpercent => Globals.ivmaxpercent;

        public bool EvolvePokemonsIfEnoughCandy => Globals.evolve;//UserSettings.Default.EvolvePokemonsIfEnoughCandy;

        public string TelegramAPIToken => Globals.telAPI;//UserSettings.Default.TelegramAPIToken;
        public string TelegramName => Globals.telName;//UserSettings.Default.TelegramName;

        public int navigation_option => Globals.navigation_option;

        public bool UseLuckyEgg => Globals.useluckyegg;//UserSettings.Default.UseLuckyEgg;
        public bool keepPokemonsThatCanEvolve => Globals.keepPokemonsThatCanEvolve;//UserSettings.Default.keepPokemonsThatCanEvolve;

        public bool pokevision => Globals.pokevision;

        public bool UserIncense => Globals.useincense;

        public bool Language => Globals.gerNames;//UserSettings.Default.Language;

        List<PokemonId> ISettings.catchPokemonSkipList
        {
            get
            {
                List<PokemonId> catchPokemonSkipList = new List<PokemonId>();
                foreach (PokemonId pokemon in Globals.noCatch)
                    catchPokemonSkipList.Add(pokemon);

                return catchPokemonSkipList;
            }
        }

        List<PokemonId> ISettings.pokemonsToHold
        {
            get
            {
                //Type and amount to keep
                List<PokemonId> pokemonsToHold = new List<PokemonId>();

                foreach (PokemonId pokemon in Globals.noTransfer)
                    pokemonsToHold.Add(pokemon);

                return pokemonsToHold;
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
                List<PokemonId> pokemonsToEvolve = new List<PokemonId>();
                foreach (PokemonId pokemon in Globals.doEvolve)
                    pokemonsToEvolve.Add(pokemon);

                return pokemonsToEvolve;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        ICollection<KeyValuePair<ItemId, int>> ISettings.itemRecycleFilter
        {
            get
            {
                //Type and amount to keep
                return new[]
                {
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, Globals.pokeball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, Globals.greatball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, Globals.ultraball),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, Globals.masterball),
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
                throw new NotImplementedException();
            }
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
    }
}
