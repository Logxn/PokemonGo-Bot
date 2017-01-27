﻿using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Inventory : BaseRpc
    {
        private GetInventoryResponse _cachedInventory;
        private DateTime _lastInventoryRequest;
        private int _minSecondsBetweenInventoryCalls = 30;
        private DateTime _lastegguse;

        public Inventory(Client client) : base(client)
        {
        }

        #region Inventory Tasks

        #region --Gets

        /// <summary>
        /// Send a parameter TRUE if you want to force real time invetory check
        /// otherwise it will be checked if _lastInventoryRequest was done more than
        /// _minSecondsBetweenInventoryCalls
        /// </summary>
        /// <param name="forceRequest"></param>
        /// <returns></returns>
        public async Task<GetInventoryResponse> GetInventory(bool forceRequest = false)
        {
            if (forceRequest)
            {
                // If forceRequest is TRUE we make the call
                _lastInventoryRequest = DateTime.UtcNow;
                _cachedInventory = await PostProtoPayload<Request, GetInventoryResponse>(RequestType.GetInventory, new GetInventoryMessage()).ConfigureAwait(false);
                return _cachedInventory;
            }
            else
            {
                if (_lastInventoryRequest.AddSeconds(_minSecondsBetweenInventoryCalls).Ticks > DateTime.UtcNow.Ticks)
                {
                    // If forceRequest is default/FALSE and last request made less than _minSecondsBetweenInventoryCalls seconds ago, we return _cachedInventory
                    return _cachedInventory;
                }
                else
                {
                    // If forceRequest is default/FALSE and last request made more than _minSecondsBetweenInventoryCalls seconds ago, 
                    // we make the call and also update _cachedInventory
                    _lastInventoryRequest = DateTime.UtcNow;
                    _cachedInventory = await PostProtoPayload<Request, GetInventoryResponse>(RequestType.GetInventory, new GetInventoryMessage()).ConfigureAwait(false);
                    return _cachedInventory;
                }
            }
        }


        public async Task<IEnumerable<ItemData>> GetItems(bool forceRequest = false)
        {
            var inventory = await GetInventory(forceRequest).ConfigureAwait(false);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        // |------getInventoryCount()---------------------[TELEGRAM ONLY]---------
        // |Quarthy - only used by Telegram. Remove if Telegram support is dropped
        // |----------------------------------------------------------------------
        public async Task<int> getInventoryCount()
        {
            var items = await GetItems().ConfigureAwait(false);
            var totalitems = 0;
            foreach (var item in items)
            {
                totalitems += item.Count;
            }
            return totalitems;
        }

        // Quarthy - Needs revision, probably can be removed. Awaiting logic.cs major update
        public IEnumerable<ItemData> GetItems(GetInventoryResponse inventory)
        {
            if (inventory != null)
                return inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.Item)
                    .Where(p => p != null);
            return null;
        }

        public async Task<IEnumerable<ItemData>> GetItemsToRecycle(ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter)
        {
            var myItems = await GetItems().ConfigureAwait(false);
            
            return myItems
                .Where(x => itemRecycleFilter.Any(f => f.Key == ((ItemId)x.ItemId) && x.Count > f.Value))
                .Select(x => new ItemData { ItemId = x.ItemId, Count = x.Count - itemRecycleFilter.Single(f => f.Key == (ItemId)x.ItemId).Value, Unseen = x.Unseen });
        }

        public async Task<int> GetItemAmountByType(ItemId type)
        {
            var pokeballs = await GetItems().ConfigureAwait(false);
            return pokeballs.FirstOrDefault(i => (ItemId)i.ItemId == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
        {
            var inv = await GetInventory().ConfigureAwait(false);
            return inv.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public IEnumerable<PlayerStats> GetPlayerStats(GetInventoryResponse inventory)
        {
            if (inventory != null)
                return inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PlayerStats)
                    .Where(p => p != null);
            return null;
        }

        #endregion

        #region --Uses

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var message = new RecycleInventoryItemMessage
            {
                ItemId = itemId,
                Count = amount
            };

            return await PostProtoPayload<Request, RecycleInventoryItemResponse>(RequestType.RecycleInventoryItem, message).ConfigureAwait(false);
        }

        public async Task<UseItemXpBoostResponse> UseItemXpBoost(ItemId item)
        {
            var message = new UseItemXpBoostMessage()
            {
                ItemId = item
            };

            return await PostProtoPayload<Request, UseItemXpBoostResponse>(RequestType.UseItemXpBoost, message).ConfigureAwait(false);
        }


        public async Task<UseItemPotionResponse> UseItemPotion(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemPotionMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, UseItemPotionResponse>(RequestType.UseItemPotion, message).ConfigureAwait(false);
        } // Quarthy - Still not implemented in BOT

        public async Task<UseItemEggIncubatorResponse> UseItemRevive(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemReviveMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, UseItemEggIncubatorResponse>(RequestType.UseItemEggIncubator, message).ConfigureAwait(false);
        } // Quarthy - REVIEW - BAD TYPE

        public async Task<UseIncenseResponse> UseIncense(ItemId incenseType)
        {
            var message = new UseIncenseMessage()
            {
                IncenseType = incenseType
            };

            return await PostProtoPayload<Request, UseIncenseResponse>(RequestType.UseIncense, message).ConfigureAwait(false);
        }

        public async Task<UseItemGymResponse> UseItemInGym(string gymId, ItemId itemId)
        {
            var message = new UseItemGymMessage()
            {
                ItemId = itemId,
                GymId = gymId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return await PostProtoPayload<Request, UseItemGymResponse>(RequestType.UseItemGym, message).ConfigureAwait(false);
        } // Quarthy - Still not implemented in BOT

        #endregion

        #endregion
        #region Pokemon Tasks

        #region --Get
        public async Task<IEnumerable<PokemonData>> GetPokemons(bool forceRequest = false)
        {
            var inventory = await GetInventory(forceRequest).ConfigureAwait(false);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);
        } // Returns pokemon inventory. Send TRUE if you want to force an inventory refresh

        #endregion

        #region --Evolve
        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(IEnumerable<PokemonId> filter = null)
        {
            var myPokemons = await GetPokemons().ConfigureAwait(false);

            myPokemons = myPokemons.Where(p => p.DeployedFortId == string.Empty).OrderByDescending(p => p.Cp); //Don't evolve pokemon in gyms

            if (filter != null)
            {
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            }
            var pokemons = myPokemons.ToList();

            var myPokemonSettings = await GetPokemonSettings().ConfigureAwait(false);
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies = await GetPokemonFamilies().ConfigureAwait(false);
            var pokemonFamilies = myPokemonFamilies.ToArray();

            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId);

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                if (settings == null || familyCandy == null)
                {
                    continue;
                }

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(
                    p => pokemonSettings.SingleOrDefault(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId)
                    * settings.CandyToEvolve;

                if (familyCandy.Candy_ - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }
            return pokemonToEvolve;
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await Client.Download.GetItemTemplates().ConfigureAwait(false);
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<List<Candy>> GetPokemonFamilies()
        {
            var inventory = await GetInventory().ConfigureAwait(false);

            var families = from item in inventory.InventoryDelta.InventoryItems
                           where item.InventoryItemData?.Candy != null
                           where item.InventoryItemData?.Candy.FamilyId != PokemonFamilyId.FamilyUnset
                           group item by item.InventoryItemData?.Candy.FamilyId into family
                           select new Candy
                           {
                               FamilyId = family.FirstOrDefault().InventoryItemData.Candy.FamilyId,
                               Candy_ = family.FirstOrDefault().InventoryItemData.Candy.Candy_
                           };

            return families.ToList();
        }

        public async Task<EvolvePokemonResponse> EvolvePokemon(ulong pokemonId)
        {
            var message = new EvolvePokemonMessage
            {
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, EvolvePokemonResponse>(RequestType.EvolvePokemon, message).ConfigureAwait(false);
        }
        #endregion

        #region --Transfer
        public async Task<ReleasePokemonResponse> TransferPokemon(ulong pokemonId) // Transfer one pokemon
        {
            var message = new ReleasePokemonMessage
            {
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message).ConfigureAwait(false);
        }

        public async Task<ReleasePokemonResponse> TransferPokemon(List<ulong> pokemonId) // Transfer a list of pokemon (BULK Transfer)
        {
            var message = new ReleasePokemonMessage { };

            message.PokemonIds.AddRange(pokemonId);

            return await PostProtoPayload<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(int holdMaxDoublePokemons, bool keepPokemonsThatCanEvolve = false, bool orderByIv = false)
        {
            var myPokemon = await GetPokemons(true).ConfigureAwait(false);

            var myPokemonList = myPokemon.ToList();

            var pokemonList = myPokemonList.Where(p => p.DeployedFortId == string.Empty && p.Favorite == 0).ToList();

            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .ToList();

                var myPokemonSettings = await GetPokemonSettings().ConfigureAwait(false);
                var pokemonSettings = myPokemonSettings as IList<PokemonSettings> ?? myPokemonSettings.ToList();

                var myPokemonFamilies = await GetPokemonFamilies().ConfigureAwait(false);
                var pokemonFamilies = myPokemonFamilies.ToArray();

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId);
                    var amountToSkip = 0;

                    if (settings.CandyToEvolve != 0)
                    {
                        amountToSkip = familyCandy.Candy_ / settings.CandyToEvolve;
                    }

                    if (holdMaxDoublePokemons > amountToSkip)
                    {
                        amountToSkip = holdMaxDoublePokemons;
                    }
                    if (orderByIv)
                    {
                        results.AddRange( (IEnumerable<PokemonData>) pokemonList.Where(x => x.PokemonId == pokemon.Key)
                                                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                                                    .ThenBy(n => n.StaminaMax)
                                                    .Skip(amountToSkip)
                                                    .ToList());
                    }
                    else
                    {
                        results.AddRange( (IEnumerable<PokemonData>) pokemonList.Where(x => x.PokemonId == pokemon.Key)
                            .OrderByDescending(x => x.Cp)
                            .ThenBy(n => n.StaminaMax)
                            .Skip(amountToSkip)
                            .ToList());
                    }

                }

                return results;
            }

            if (orderByIv)
            {
                return pokemonList
                    .GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 0)
                    .SelectMany(p => p.Where(x => x.Favorite == 0)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(holdMaxDoublePokemons)
                    .ToList());

            }
            else
            {
                return pokemonList
                    .GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 0)
                    .SelectMany(p => p.Where(x => x.Favorite == 0)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(holdMaxDoublePokemons)
                    .ToList());
            }
        }
        #endregion

        #region --Upgrade
        public async Task<UpgradePokemonResponse> UpgradePokemon(ulong pokemonId)
        {
            var message = new UpgradePokemonMessage()
            {
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, UpgradePokemonResponse>(RequestType.UpgradePokemon, message).ConfigureAwait(false);
        }
        #endregion

        #region --Name
        public async Task<NicknamePokemonResponse> NicknamePokemon(ulong pokemonId, string nickName)
        {
            var message = new NicknamePokemonMessage()
            {
                PokemonId = pokemonId,
                Nickname = nickName
            };

            return await PostProtoPayload<Request, NicknamePokemonResponse>(RequestType.NicknamePokemon, message).ConfigureAwait(false);
        }
        #endregion

        #region --Favourite
        public async Task<SetFavoritePokemonResponse> SetFavoritePokemon(long pokemonId, bool isFavorite)
        {
            var message = new SetFavoritePokemonMessage()
            {
                PokemonId = pokemonId,
                IsFavorite = isFavorite
            };

            return await PostProtoPayload<Request, SetFavoritePokemonResponse>(RequestType.SetFavoritePokemon, message).ConfigureAwait(false);
        }
        #endregion

        #region --Buddy
        public async Task<SetBuddyPokemonResponse> SetBuddyPokemon(ulong pokemonId)
        {
            var message = new SetBuddyPokemonMessage()
            {
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, SetBuddyPokemonResponse>(RequestType.SetBuddyPokemon, message).ConfigureAwait(false);
        }
        #endregion

        #region Others

        public async Task<IEnumerable<PokemonData>> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon = await GetPokemons().ConfigureAwait(false);
            var pokemons = myPokemon.ToList();
            return pokemons.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public async Task<IEnumerable<PokemonData>> GetHighestIVofType(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons().ConfigureAwait(false);
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(x => x.Cp)
                    .ToList();
        }

        public async Task<IEnumerable<PokemonData>> GetHighestCPofType2(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons().ConfigureAwait(false);
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(PokemonInfo.CalculatePokemonPerfection)
                    .ToList();
        }

        public async Task<int> GetHighestCPofType(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons().ConfigureAwait(false);
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                            .OrderByDescending(x => x.Cp)
                            .FirstOrDefault().Cp;
        }

        public async Task ExportPokemonToCSV(PlayerData player, string filename = "PokemonList.csv")
        {
            if (player == null)
                return;
            var stats = await GetPlayerStats().ConfigureAwait(false);
            var stat = stats.FirstOrDefault();
            if (stat == null)
                return;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

            if (Directory.Exists(path))
            {
                try
                {
                    string pokelist_file = Path.Combine(path, $"Profile_{player.Username}_{filename}");
                    if (File.Exists(pokelist_file))
                        File.Delete(pokelist_file);
                    string ls = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    string header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    File.WriteAllText(pokelist_file, $"{header.Replace(",", $"{ls}")}");

                    var AllPokemon = await GetHighestsPerfect().ConfigureAwait(false);
                    var myPokemonSettings = await GetPokemonSettings().ConfigureAwait(false);
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies = await GetPokemonFamilies().ConfigureAwait(false);
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    int trainerLevel = stat.Level;
                    int[] exp_req = new[] { 0, 1000, 3000, 6000, 10000, 15000, 21000, 28000, 36000, 45000, 55000, 65000, 75000, 85000, 100000, 120000, 140000, 160000, 185000, 210000, 260000, 335000, 435000, 560000, 710000, 900000, 1100000, 1350000, 1650000, 2000000, 2500000, 3000000, 3750000, 4750000, 6000000, 7500000, 9500000, 12000000, 15000000, 20000000 };
                    int exp_req_at_level = exp_req[stat.Level - 1];

                    using (var w = File.AppendText(pokelist_file))
                    {
                        w.WriteLine("");
                        foreach (var pokemon in AllPokemon)
                        {
                            string toEncode = $"{(int)pokemon.PokemonId}" + "," + trainerLevel + "," + PokemonInfo.GetLevel(pokemon) + "," + pokemon.Cp + "," + pokemon.Stamina;
                            //Generate base64 code to make it viewable here http://poke.isitin.org/#MTUwLDIzLDE3LDE5MDIsMTE4
                            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toEncode));

                            string IsInGym = string.Empty;
                            string IsFavorite = string.Empty;

                            if (pokemon.Favorite != 0)
                                IsFavorite = "Yes";
                            else
                                IsFavorite = "No";

                            var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId).Candy_;
                            string perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string content_part1 = $"{(int)pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
                            string content_part2 = $",{pokemon.Move1},{pokemon.Move2},{pokemon.Stamina},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{familiecandies},{IsInGym},{IsFavorite},http://poke.isitin.org/#{encoded}";
                            string content = $"{content_part1.Replace(",", $"{ls}")}\"{perfection}\"{content_part2.Replace(",", $"{ls}")}";
                            w.WriteLine($"{content}");

                        }
                        w.Close();
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Export Player Infos and all Pokemon to \"\\Config\\{filename}\"", LogLevel.Info);
                }
                catch
                {
                    Logger.Error("Export Player Infos and all Pokemons to CSV not possible. File seems be in use!"/*, LogLevel.Warning*/);
                }
            }
        }
        #endregion

        #endregion

        #region Eggs Tasks
        public async Task<int> GetEggsCount()
        {
            var p = await GetEggs().ConfigureAwait(false);
            var i = p.Count();
            return i;
        }

        public async Task<IEnumerable<PokemonData>> GetEggs()
        {

            var inventory = await GetInventory().ConfigureAwait(false);
            if (inventory == null)
            {
                await GetEggs().ConfigureAwait(false);
            }
            return

           inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
               .Where(p => p != null && p.IsEgg);
        }

        public async Task UseLuckyEgg(Client client)
        {
            var inventory = await GetItems().ConfigureAwait(false);
            var luckyEgg = inventory.FirstOrDefault(p => (ItemId)p.ItemId == ItemId.ItemLuckyEgg);

            if (_lastegguse > DateTime.Now.AddSeconds(5))
            {
                TimeSpan duration = _lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0) { return; }

            await client.Inventory.UseItemXpBoost(ItemId.ItemLuckyEgg).ConfigureAwait(false);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            _lastegguse = DateTime.Now.AddMinutes(30);
            RandomHelper.RandomSleep(3000, 3100);
        }

        public async Task<UseItemEggIncubatorResponse> UseItemEggIncubator(string itemId, ulong pokemonId)
        {
            var message = new UseItemEggIncubatorMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return await PostProtoPayload<Request, UseItemEggIncubatorResponse>(RequestType.UseItemEggIncubator, message).ConfigureAwait(false);
        }

        public async Task<GetHatchedEggsResponse> GetHatchedEgg()
        {
            return await PostProtoPayload<Request, GetHatchedEggsResponse>(RequestType.GetHatchedEggs, new GetHatchedEggsMessage()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<EggIncubator>> GetEggIncubators()
        {
            var inventory = await GetInventory().ConfigureAwait(false);
            return
                inventory.InventoryDelta.InventoryItems
                    .Where(x => x.InventoryItemData.EggIncubators != null)
                    .SelectMany(i => i.InventoryItemData.EggIncubators.EggIncubator)
                    .Where(i => i != null);
        }

        public int GetEggsCount(GetInventoryResponse inventory)
        {
            var p = GetEggs(inventory);
            var i = p.Count();
            return i;
        }

        public IEnumerable<PokemonData> GetEggs(GetInventoryResponse inventory)
        {
            if (inventory != null)
                return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                .Where(p => p != null && p.IsEgg);
            return null;
        }

        public IEnumerable<EggIncubator> GetEggIncubators(GetInventoryResponse inventory)
        {
            return
                inventory.InventoryDelta.InventoryItems
                    .Where(x => x.InventoryItemData.EggIncubators != null)
                    .SelectMany(i => i.InventoryItemData.EggIncubators.EggIncubator)
                    .Where(i => i != null);
        }
        #endregion
    }
}