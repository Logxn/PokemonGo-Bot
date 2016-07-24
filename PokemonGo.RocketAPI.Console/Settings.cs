﻿using System.Configuration;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using AllEnum;

namespace PokemonGo.RocketAPI.Console
{
    public class Settings : ISettings
    {
        AuthType ISettings.AuthType
        {
            get
            {
                return Enums.AuthType.Ptc;
            }
        }
         
        public bool UseLastCords
        {
            get
            {
                return true; // Only disable this if your sure what your doing!
            }
        }
        
        public string PtcUsername => UserSettings.Default.PtcUsername;
        public string PtcPassword => UserSettings.Default.PtcPassword;
        public double DefaultLatitude => UserSettings.Default.DefaultLatitude;
        public double DefaultLongitude => UserSettings.Default.DefaultLongitude;
        public double DefaultAltitude => UserSettings.Default.DefaultAltitude;

        public bool WalkBackToDefaultLocation => UserSettings.Default.WalkBackToDefaultLocation;
        public int MaxWalkingRadiusInMeters => UserSettings.Default.MaxWalkingRadiusInMeters;

        public double WalkingSpeedInKilometerPerHour => UserSettings.Default.WalkingSpeedInKilometerPerHour;

        public bool TransferDoublePokemons => UserSettings.Default.TransferDoublePokemons;
        public int DontTransferWithCPOver => UserSettings.Default.DontTransferWithCPOver;

        public bool EvolvePokemonsIfEnoughCandy => UserSettings.Default.EvolvePokemonsIfEnoughCandy;

        public string TelegramAPIToken => UserSettings.Default.TelegramAPIToken;
        public string TelegramName => UserSettings.Default.TelegramName;

        public int TelegramLiveStatsDelay => UserSettings.Default.TelegramLiveStatsDelay;

        List<PokemonId> ISettings.pokemonsToHold
        {
            get
            {
                //Type and amount to keep
                List<PokemonId> pokemonsToHold = new List<PokemonId>();

                //pokemonsToHold.Add(PokemonId.Pidgey);    // We hold all Pidgeys
                //pokemonsToHold.Add(PokemonId.Rattata);

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
                pokemonsToEvolve.Add(PokemonId.Pidgey);
                pokemonsToEvolve.Add(PokemonId.Caterpie);
                pokemonsToEvolve.Add(PokemonId.Weedle);
                pokemonsToEvolve.Add(PokemonId.Metapod);
                pokemonsToEvolve.Add(PokemonId.Kakuna);
                pokemonsToEvolve.Add(PokemonId.Spearow);
                pokemonsToEvolve.Add(PokemonId.Pidgeotto);
                pokemonsToEvolve.Add(PokemonId.Rattata);
                pokemonsToEvolve.Add(PokemonId.Zubat); 
                pokemonsToEvolve.Add(PokemonId.Oddish); 
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
                    new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, 20),
                    new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, 50),
                    new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, 100),
                    new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, 200),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRevive, 20),
                    new KeyValuePair<ItemId, int>(ItemId.ItemPotion, 0),
                    new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, 0),
                    new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, 50),
                    new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, 80)
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
