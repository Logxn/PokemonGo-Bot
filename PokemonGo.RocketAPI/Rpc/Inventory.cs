using Google.Protobuf;
using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Inventory : BaseRpc
    {
        private GetHoloInventoryResponse CachedInventory;

        public Inventory(Client client) : base(client)
        {
        }

        #region Inventory Tasks

        #region --Gets

        /// <summary>
        /// Send a parameter TRUE if you want to force real time invetory check
        /// </summary>
        /// <param name="forceRequest"></param>
        /// <returns></returns>
        public GetHoloInventoryResponse GetHoloInventory(bool forceRequest = false)
        {
            // forceRequest should be used never ()
            if (forceRequest)
                CachedInventory =  PostProtoPayload<Request, GetHoloInventoryResponse>(RequestType.GetHoloInventory, new GetHoloInventoryMessage());
            return CachedInventory; 
        }

        public void SetHoloInventory( GetHoloInventoryResponse inventory)
        {
            CachedInventory = inventory;
        }

        public IEnumerable<ItemData> GetItems()
        {
            var items = GetHoloInventory(true).InventoryDelta.InventoryItems
                .Where(i => i.InventoryItemData.Item !=null);
            return items.Select(i=> i.InventoryItemData.Item);
        }

        public ItemData GetItemData( ItemId itemId)
        {
            return GetItems().FirstOrDefault(p => p.ItemId == itemId);
        }

        public IEnumerable<ItemData> GetItemsToRecycle(ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter)
        {
            var myItems = GetItems();
            
            return myItems
                .Where(x => itemRecycleFilter.Any(f => f.Key == ((ItemId)x.ItemId) && x.Count > f.Value))
                .Select(x => new ItemData { ItemId = x.ItemId, Count = x.Count - itemRecycleFilter.Single(f => f.Key == (ItemId)x.ItemId).Value, Unseen = x.Unseen });
        }

        public int GetItemAmountByType(ItemId type, bool forceUpdate = false)
        {
            var items = GetItems();
            return items.FirstOrDefault(i => (ItemId)i.ItemId == type)?.Count ?? 0;
        }

        public IEnumerable<PlayerStats> GetPlayerStats()
        {
            return CachedInventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        #endregion

        #region --Uses

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var request = new Request
            {
                RequestType = RequestType.RecycleInventoryItem,
                RequestMessage = ((IMessage)new RecycleInventoryItemMessage
                {
                    ItemId = itemId,
                    Count = amount
                }).ToByteString()
            };
            return await PostProtoPayloadCommonR<Request, RecycleInventoryItemResponse>(request).ConfigureAwait(false);
        }

        public UseItemXpBoostResponse UseItemXpBoost(ItemId item)
        {
            var message = new UseItemXpBoostMessage()
            {
                ItemId = item
            };

            return PostProtoPayloadCommonR<Request, UseItemXpBoostResponse>(RequestType.UseItemXpBoost, message).Result;
        }


        public UseItemPotionResponse UseItemPotion(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemPotionMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return PostProtoPayloadCommonR<Request, UseItemPotionResponse>(RequestType.UseItemPotion, message).Result;
        }

        public UseItemReviveResponse UseItemRevive(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemReviveMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return PostProtoPayloadCommonR<Request, UseItemReviveResponse>(RequestType.UseItemRevive, message).Result;
        }

        public UseIncenseResponse UseIncense(ItemId incenseType)
        {
            var message = new UseIncenseMessage()
            {
                IncenseType = incenseType
            };

            return PostProtoPayloadCommonR<Request, UseIncenseResponse>(RequestType.UseIncense, message).Result;
        }

        public  UseItemGymResponse UseItemGym(string gymId, ItemId itemId)
        {
            var message = new UseItemGymMessage()
            {
                ItemId = itemId,
                GymId = gymId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return  PostProtoPayloadCommonR<Request, UseItemGymResponse>(RequestType.UseItemGym, message).Result;
        } // Quarthy - Still not implemented in BOT

        #endregion

        #endregion
        #region Pokemon Tasks

        #region --Get
        public  IEnumerable<PokemonData> GetPokemons()
        {
            var inventory = GetHoloInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);
        }
        #endregion

        public  IEnumerable<PokedexEntry> GetPokedexEntries()
        {
            var inventory = GetHoloInventory();
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokedexEntry)
                    .Where(p => p != null && p?.PokemonId > 0);
        }

        #region --Evolve
        public IEnumerable<PokemonData> GetPokemonToEvolve( IEnumerable<PokemonId> filter = null)
        {
            var myPokemons =  GetPokemons();

            myPokemons = myPokemons.Where(p => p.DeployedFortId == string.Empty).OrderByDescending(p => p.Cp); //Don't evolve pokemon in gyms

            if (filter != null)
            {
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            }
            var pokemons = myPokemons.ToList();

            var myPokemonSettings =  GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies =  GetPokemonFamilies();
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

        public  IEnumerable<PokemonSettings> GetPokemonSettings()
        {
            var templates =  Client.Download.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public  List<Candy> GetPokemonFamilies()
        {
            var inventory = GetHoloInventory();

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

        public EvolvePokemonResponse EvolvePokemon(ulong pokemonId, ItemId item  = ItemId.ItemUnknown)
        {
            var message = new EvolvePokemonMessage
            {
                PokemonId = pokemonId,
            };
            if (item != ItemId.ItemUnknown)
                message.EvolutionItemRequirement = item ;

            var result = PostProtoPayloadCommonR<Request, EvolvePokemonResponse>(RequestType.EvolvePokemon, message).Result;
            
            if (result.Result ==EvolvePokemonResponse.Types.Result.Success)
                DeletePokemons(new List<ulong>{pokemonId});
            return result;
        }
        #endregion

        #region --Transfer
        public ReleasePokemonResponse ReleasePokemon(ulong pokemonId) // Transfer one pokemon
        {
            var message = new ReleasePokemonMessage
            {
                PokemonId = pokemonId
            };

            var result =  PostProtoPayloadCommonR<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message).Result;
            if (result.Result ==ReleasePokemonResponse.Types.Result.Success)
                DeletePokemons(new List<ulong>{pokemonId});
            return result;
        }

        public ReleasePokemonResponse ReleasePokemon(List<ulong> pokemonId) // Transfer a list of pokemon (BULK Transfer)
        {
            var message = new ReleasePokemonMessage { };

            message.PokemonIds.AddRange(pokemonId);

            var result =  PostProtoPayloadCommonR<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message).Result;
            if (result.Result ==ReleasePokemonResponse.Types.Result.Success)
                DeletePokemons(pokemonId);
            return result;
        }

        public IEnumerable<PokemonData> GetDuplicatePokemonToTransfer(int holdMaxDoublePokemons, bool keepPokemonsThatCanEvolve = false, bool orderByIv = false)
        {
            var myPokemon =  GetPokemons();

            var myPokemonList = myPokemon.ToList();

            var pokemonList = myPokemonList.Where(p => p.DeployedFortId == string.Empty && p.Favorite == 0).ToList();

            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .ToList();

                var myPokemonSettings =  GetPokemonSettings();
                var pokemonSettings = myPokemonSettings as IList<PokemonSettings> ?? myPokemonSettings.ToList();

                var myPokemonFamilies =  GetPokemonFamilies();
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
                    .Where(x => x.Any())
                    .SelectMany(p => p.Where(x => x.Favorite == 0)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(holdMaxDoublePokemons)
                    .ToList());

            }
            return pokemonList
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Any())
                .SelectMany(p => p.Where(x => x.Favorite == 0)
                .OrderByDescending(x => x.Cp)
                .ThenBy(n => n.StaminaMax)
                .Skip(holdMaxDoublePokemons)
                .ToList());
        }
        #endregion

        #region --Upgrade
        public UpgradePokemonResponse UpgradePokemon(ulong pokemonId)
        {
            var message = new UpgradePokemonMessage()
            {
                PokemonId = pokemonId
            };

            return  PostProtoPayloadCommonR<Request, UpgradePokemonResponse>(RequestType.UpgradePokemon, message).Result;
        }
        #endregion

        #region --Name
        public NicknamePokemonResponse NicknamePokemon(ulong pokemonId, string nickName)
        {
            var message = new NicknamePokemonMessage()
            {
                PokemonId = pokemonId,
                Nickname = nickName
            };

            return  PostProtoPayloadCommonR<Request, NicknamePokemonResponse>(RequestType.NicknamePokemon, message).Result;
        }
        #endregion

        #region --Favourite
        public SetFavoritePokemonResponse SetFavoritePokemon(long pokemonId, bool isFavorite)
        {
            var message = new SetFavoritePokemonMessage()
            {
                PokemonId = pokemonId,
                IsFavorite = isFavorite
            };

            return  PostProtoPayloadCommonR<Request, SetFavoritePokemonResponse>(RequestType.SetFavoritePokemon, message).Result;
        }
        #endregion

        #region --Buddy
        public SetBuddyPokemonResponse SetBuddyPokemon(ulong pokemonId)
        {
            var message = new SetBuddyPokemonMessage()
            {
                PokemonId = pokemonId
            };

            return  PostProtoPayloadCommonR<Request, SetBuddyPokemonResponse>(RequestType.SetBuddyPokemon, message).Result;
        }
        #endregion

        #region Others

        public IEnumerable<PokemonData> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon =  GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public IEnumerable<PokemonData> GetHighestIVofType(PokemonData pokemon)
        {
            var myPokemon =  GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(x => x.Cp)
                    .ToList();
        }

        public IEnumerable<PokemonData> GetHighestCPofType2(PokemonData pokemon)
        {
            var myPokemon = GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(PokemonInfo.CalculatePokemonPerfection)
                    .ToList();
        }

        public int GetHighestCPofType(PokemonData pokemon)
        {
            var myPokemon = GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                            .OrderByDescending(x => x.Cp)
                            .FirstOrDefault().Cp;
        }

        public void ExportPokemonToCSV(PlayerData player, string filename = "PokemonList.csv")
        {
            if (player == null)
                return;
            var stats =  GetPlayerStats();
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
                    const string header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    File.WriteAllText(pokelist_file, header.Replace(",", ls));

                    var AllPokemon =  GetHighestsPerfect();
                    var myPokemonSettings =  GetPokemonSettings();
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies =  GetPokemonFamilies();
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

                            IsFavorite = pokemon.Favorite != 0 ? "Yes" : "No";

                            var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId).Candy_;
                            string perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string content_part1 = $"{(int)pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
                            string content_part2 = $",{pokemon.Move1},{pokemon.Move2},{pokemon.Stamina},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{familiecandies},{IsInGym},{IsFavorite},http://poke.isitin.org/#{encoded}";
                            var str1 = content_part1.Replace(",", ls);
                            var str2= content_part2.Replace(",", ls);
                            string content = $"{str1}\"{perfection}\"{str2}";
                            w.WriteLine($"{content}");

                        }
                        w.Close();
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Export Player Infos and all Pokemon to \"\\Config\\{filename}\"");
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
        public int GetEggsCount()
        {
            var p = GetEggs();
            var i = p.Count();
            return i;
        }

        public IEnumerable<PokemonData> GetEggs()
        {
            var inventory =  GetHoloInventory();
            return   inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
               .Where(p => p != null && p.IsEgg);
        }

        public UseItemEggIncubatorResponse UseItemEggIncubator(string itemId, ulong pokemonId)
        {
            var message = new UseItemEggIncubatorMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return PostProtoPayloadCommonR<Request, UseItemEggIncubatorResponse>(RequestType.UseItemEggIncubator, message).Result;
        }

        public IEnumerable<EggIncubator> GetEggIncubators()
        {
            var inventory = GetHoloInventory();
            return
                inventory.InventoryDelta.InventoryItems
                    .Where(x => x.InventoryItemData.EggIncubators != null)
                    .SelectMany(i => i.InventoryItemData.EggIncubators.EggIncubator)
                    .Where(i => i != null);
        }

        public static ItemId GeteNeededItemToEvolve(PokemonId pokeId)
        {
                var item = ItemId.ItemUnknown;
                switch (pokeId) {
                    case PokemonId.Seadra:
                        item = ItemId.ItemDragonScale;
                        break;
                    case PokemonId.Poliwhirl:
                    case PokemonId.Slowpoke:
                        item = ItemId.ItemKingsRock;
                        break;
                    case PokemonId.Scyther:
                    case PokemonId.Onix:
                        item = ItemId.ItemMetalCoat;
                        break;
                    case PokemonId.Porygon:
                        item = ItemId.ItemUpGrade;
                        break;
                    case PokemonId.Gloom:
                    case PokemonId.Sunkern:
                        item = ItemId.ItemSunStone;
                        break;
                }
                return item;
        }

        internal void UpdateInventoryItems(InventoryDelta delta)
        {
            if (CachedInventory == null){
                CachedInventory = new GetHoloInventoryResponse();
                CachedInventory.InventoryDelta = delta;
                return;
            }
            var InventoryItems = CachedInventory.InventoryDelta.InventoryItems;

            if (delta?.InventoryItems == null || delta.InventoryItems.All(i => i == null))
            {
                return;
            }
            InventoryItems.AddRange(delta.InventoryItems.Where(i => i != null));
            // Only keep the newest ones
            foreach (var deltaItem in delta.InventoryItems.Where(d => d?.InventoryItemData != null))
            {
                var oldItems = new List<InventoryItem>();
                if (deltaItem.InventoryItemData.PlayerStats != null)
                {
                    var oldValues = 
                        InventoryItems.Where(i => i.InventoryItemData?.PlayerStats != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                        .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.PlayerCurrency != null)
                {
                    var oldValues =
                        InventoryItems.Where(i => i.InventoryItemData?.PlayerCurrency != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                        .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.PlayerCamera != null)
                {
                    var oldValues =
                        InventoryItems.Where(i => i.InventoryItemData?.PlayerCamera != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                        .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.InventoryUpgrades != null)
                {
                    var oldValues=
                        InventoryItems.Where(i => i.InventoryItemData?.InventoryUpgrades != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                            .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.PokedexEntry != null)
                {
                    var oldValues =InventoryItems.Where(
                            i =>
                                i.InventoryItemData?.PokedexEntry != null &&
                                i.InventoryItemData.PokedexEntry.PokemonId ==
                                deltaItem.InventoryItemData.PokedexEntry.PokemonId)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                        .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.Candy != null)
                {
                    var oldValues = InventoryItems.Where(
                            i =>
                                i.InventoryItemData?.Candy != null &&
                                i.InventoryItemData.Candy.FamilyId ==
                                deltaItem.InventoryItemData.Candy.FamilyId)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                        .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.Item != null)
                {
                    var oldValues=
                        InventoryItems.Where(
                            i =>
                                i.InventoryItemData?.Item != null &&
                                i.InventoryItemData.Item.ItemId == deltaItem.InventoryItemData.Item.ItemId)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                            .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.PokemonData != null)
                {
                    var oldValues=
                        InventoryItems.Where(
                            i =>
                                i.InventoryItemData?.PokemonData != null &&
                                i.InventoryItemData.PokemonData.Id == deltaItem.InventoryItemData.PokemonData.Id)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                            .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.AppliedItems != null)
                {
                    var oldValues=
                        InventoryItems.Where(i => i.InventoryItemData?.AppliedItems != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                            .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                if (deltaItem.InventoryItemData.EggIncubators != null)
                {
                    var oldValues=
                        InventoryItems.Where(i => i.InventoryItemData?.EggIncubators != null)
                            .OrderByDescending(i => i.ModifiedTimestampMs)
                            .Skip(1);
                    if (oldValues!=null)
                        oldItems.AddRange(oldValues);
                }
                foreach (var oldItem in oldItems)
                {
                    InventoryItems.Remove(oldItem);
                }
                var deletedPokemons = delta.InventoryItems.Select(i => i.DeletedItem).Where(x => x !=null );
                foreach (var element in deletedPokemons) {
                        var cachedElement = InventoryItems.FirstOrDefault(x => x.InventoryItemData.PokemonData !=null && x.InventoryItemData.PokemonData.Id == element.PokemonId);
                        if (cachedElement !=null)
                            InventoryItems.Remove(cachedElement);
                }
            }
        }
        #endregion

        void DeletePokemons(List<ulong> pokemons)
        {
            foreach (var element in pokemons) {
                var cachedElement = GetHoloInventory().InventoryDelta.InventoryItems.FirstOrDefault(x => x.InventoryItemData.PokemonData !=null && x.InventoryItemData.PokemonData.Id == element);
                        if (cachedElement !=null)
                            GetHoloInventory().InventoryDelta.InventoryItems.Remove(cachedElement);
                };
        }
    }
}