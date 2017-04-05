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
        private const int _minSecondsBetweenInventoryCalls = 20;

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
            if (_lastInventoryRequest.AddSeconds(_minSecondsBetweenInventoryCalls).Ticks > DateTime.UtcNow.Ticks && _cachedInventory!=null && !forceRequest)
            {
                // If forceRequest is default/FALSE and last request made less than _minSecondsBetweenInventoryCalls seconds ago, we return _cachedInventory
                return _cachedInventory;
            }
            // If forceRequest is default/FALSE and last request made more than _minSecondsBetweenInventoryCalls seconds ago, 
            // we make the call and also update _cachedInventory
            _lastInventoryRequest = DateTime.UtcNow;
            
            var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.GetCommonRequests(Client));

            Tuple<CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                await
                    PostProtoPayload
                        <Request, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                            CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);

            CheckChallengeResponse checkChallengeResponse = response.Item1;
            CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);

            _cachedInventory = response.Item3;
            CommonRequest.ProcessGetInventoryResponse(Client, _cachedInventory);

            DownloadSettingsResponse downloadSettingsResponse = response.Item5;
            CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);

            return _cachedInventory;
        }


        public IEnumerable<ItemData> GetItemsOld(bool forceRequest = false)
        {
            var inventory = GetInventory(forceRequest).Result;
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public IEnumerable<ItemData> GetItems(bool forceRequest = false)
        {
            var items = GetInventory(forceRequest).Result.InventoryDelta.InventoryItems
                .Where(i => i.InventoryItemData.Item !=null);
            return items.Select(i=> i.InventoryItemData.Item);
        }

        public ItemData GetItemData( ItemId itemId)
        {
            return GetItems()?.FirstOrDefault(p => p.ItemId == itemId);
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
            var items = GetItems(forceUpdate);
            return items.FirstOrDefault(i => (ItemId)i.ItemId == type)?.Count ?? 0;
        }



        #endregion

        #region --Uses

        public RecycleInventoryItemResponse RecycleItemOnly(ItemId itemId, int amount)
        {
            var message = new RecycleInventoryItemMessage
            {
                ItemId = itemId,
                Count = amount
            };

            return PostProtoPayload<Request, RecycleInventoryItemResponse>(RequestType.RecycleInventoryItem, message);
        }

        public async Task<RecycleInventoryItemResponse> RecycleItem(ItemId itemId, int amount)
        {
            var recycleItemRequest = new Request
            {
                RequestType = RequestType.RecycleInventoryItem,
                RequestMessage = ((IMessage)new RecycleInventoryItemMessage
                {
                    ItemId = itemId,
                    Count = amount
                }).ToByteString()
            };

            var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(recycleItemRequest, Client));

            Tuple<RecycleInventoryItemResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                await
                    PostProtoPayload
                        <Request, RecycleInventoryItemResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                            CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);

            CheckChallengeResponse checkChallengeResponse = response.Item2;
            CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);

            GetInventoryResponse getInventoryResponse = response.Item4;
            CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);

            DownloadSettingsResponse downloadSettingsResponse = response.Item6;
            CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);

            return response.Item1;
        }        

        public UseItemXpBoostResponse UseItemXpBoost(ItemId item)
        {
            var message = new UseItemXpBoostMessage()
            {
                ItemId = item
            };

            return PostProtoPayload<Request, UseItemXpBoostResponse>(RequestType.UseItemXpBoost, message);
        }


        public UseItemPotionResponse UseItemPotion(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemPotionMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return PostProtoPayload<Request, UseItemPotionResponse>(RequestType.UseItemPotion, message);
        }

        public UseItemReviveResponse UseItemRevive(ItemId itemId, ulong pokemonId)
        {
            var message = new UseItemReviveMessage()
            {
                ItemId = itemId,
                PokemonId = pokemonId
            };

            return PostProtoPayload<Request, UseItemReviveResponse>(RequestType.UseItemRevive, message);
        }

        public UseIncenseResponse UseIncense(ItemId incenseType)
        {
            var message = new UseIncenseMessage()
            {
                IncenseType = incenseType
            };

            return PostProtoPayload<Request, UseIncenseResponse>(RequestType.UseIncense, message);
        }

        public  UseItemGymResponse UseItemInGym(string gymId, ItemId itemId)
        {
            var message = new UseItemGymMessage()
            {
                ItemId = itemId,
                GymId = gymId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return  PostProtoPayload<Request, UseItemGymResponse>(RequestType.UseItemGym, message);
        } // Quarthy - Still not implemented in BOT

        #endregion

        #endregion
        #region Pokemon Tasks

        #region --Get
        public  IEnumerable<PokemonData> GetPokemons(bool forceRequest = false)
        {
            var inventory = GetInventory(forceRequest).Result;
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);
        } // Returns pokemon inventory. Send TRUE if you want to force an inventory refresh

        #endregion

        #region --Evolve


        public EvolvePokemonResponse EvolvePokemon(ulong pokemonId, ItemId item  = ItemId.ItemUnknown)
        {
            var message = new EvolvePokemonMessage
            {
                PokemonId = pokemonId,
            };
            if (item != ItemId.ItemUnknown)
                message.EvolutionItemRequirement = item ;

            return PostProtoPayload<Request, EvolvePokemonResponse>(RequestType.EvolvePokemon, message);
        }
        #endregion

        #region --Transfer
        public ReleasePokemonResponse TransferPokemon(ulong pokemonId) // Transfer one pokemon
        {
            var message = new ReleasePokemonMessage
            {
                PokemonId = pokemonId
            };

            return PostProtoPayload<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message);
        }

        public ReleasePokemonResponse TransferPokemons(List<ulong> pokemonId) // Transfer a list of pokemon (BULK Transfer)
        {
            var message = new ReleasePokemonMessage { };

            message.PokemonIds.AddRange(pokemonId);

            return  PostProtoPayload<Request, ReleasePokemonResponse>(RequestType.ReleasePokemon, message);
        }

        #endregion

        #region --Upgrade
        public UpgradePokemonResponse UpgradePokemon(ulong pokemonId)
        {
            var message = new UpgradePokemonMessage()
            {
                PokemonId = pokemonId
            };

            return  PostProtoPayload<Request, UpgradePokemonResponse>(RequestType.UpgradePokemon, message);
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

            return  PostProtoPayload<Request, NicknamePokemonResponse>(RequestType.NicknamePokemon, message);
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

            return  PostProtoPayload<Request, SetFavoritePokemonResponse>(RequestType.SetFavoritePokemon, message);
        }
        #endregion

        #region --Buddy
        public SetBuddyPokemonResponse SetBuddyPokemon(ulong pokemonId)
        {
            var message = new SetBuddyPokemonMessage()
            {
                PokemonId = pokemonId
            };

            return  PostProtoPayload<Request, SetBuddyPokemonResponse>(RequestType.SetBuddyPokemon, message);
        }
        #endregion

        #region Others



        #endregion

        #endregion

        #region Eggs Tasks
        public int GetEggsCount()
        {
            var p = GetEggs();
            var i = p.Count();
            return i;
        }

        public IEnumerable<PokemonData> GetEggs(bool forceRefress = false)
        {
            var inventory =  GetInventory(forceRefress).Result;
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

            return PostProtoPayload<Request, UseItemEggIncubatorResponse>(RequestType.UseItemEggIncubator, message);
        }

        public GetHatchedEggsResponse GetHatchedEgg()
        {
            return  PostProtoPayload<Request, GetHatchedEggsResponse>(RequestType.GetHatchedEggs, new GetHatchedEggsMessage());
        }

        public IEnumerable<EggIncubator> GetEggIncubators()
        {
            var inventory = GetInventory().Result;
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


        #endregion
    }
}