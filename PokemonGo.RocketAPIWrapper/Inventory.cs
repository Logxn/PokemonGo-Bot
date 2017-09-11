/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 22:17
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using POGOLib.Official.Net;
using POGOProtos.Data;
using POGOProtos.Data.Player;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using System.Linq;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using Google.Protobuf;
namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Inventory.
    /// </summary>
    public class Inventory
    {
        internal Session session;
        public Inventory(Session session)
        {
            this.session = session;
        }

        public RepeatedField<InventoryItem> GetInventoryItems()
        {
            return session.Player.Inventory.InventoryItems;
        }
        public IEnumerable<PokemonData> GetEggs()
        {
            var inventory =  GetInventoryItems();
            return   inventory.Select(i => i.InventoryItemData?.PokemonData)
               .Where(p => p != null && p.IsEgg);
        }

        public IEnumerable<EggIncubator> GetEggIncubators()
        {
            var inventory = GetInventoryItems();
            return
                inventory
                    .Where(x => x.InventoryItemData.EggIncubators != null)
                    .SelectMany(i => i.InventoryItemData.EggIncubators.EggIncubator)
                    .Where(i => i != null);
        }
        public IEnumerable<ItemData> GetItems()
        {
            var inventory = session.Player.Inventory;
            return inventory.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
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
            var items = GetItems();
            return items.FirstOrDefault(i => (ItemId)i.ItemId == type)?.Count ?? 0;
        }

        public IEnumerable<PlayerStats> GetPlayerStats()
        {
            var inventory = session.Player.Inventory;
            return inventory.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public  IEnumerable<PokemonData> GetPokemons()
        {
            var inventory = session.Player.Inventory;
            return
                inventory.InventoryItems.Select(i => i.InventoryItemData?.PokemonData)
                    .Where(p => p != null && p?.PokemonId > 0);
        }

        public  IEnumerable<PokedexEntry> GetPokedexEntries()
        {
            var inventory = session.Player.Inventory;
            return
                inventory.InventoryItems.Select(i => i.InventoryItemData?.PokedexEntry)
                    .Where(p => p != null && p?.PokemonId > 0);
        }

        public async Task<EvolvePokemonResponse> EvolvePokemon(ulong id)
        {
            var msg = new EvolvePokemonMessage();
            msg.PokemonId = id;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return EvolvePokemonResponse.Parser.ParseFrom(response);
        }

        public async Task<ReleasePokemonResponse> ReleasePokemon(ulong id)
        {
            var msg = new ReleasePokemonMessage();
            msg.PokemonId = id;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return ReleasePokemonResponse.Parser.ParseFrom(response);
        }
        public async Task<ReleasePokemonResponse> ReleasePokemon(List<ulong>  ids)
        {
            var msg = new ReleasePokemonMessage();
            msg.PokemonIds.AddRange(ids);
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return ReleasePokemonResponse.Parser.ParseFrom(response);
        }

        public async Task<RecycleInventoryItemResponse> RecycleInventoryItem(ItemId itemId, int count)
        {
            var msg = new RecycleInventoryItemMessage();
            msg.ItemId = itemId;
            msg.Count = count;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return RecycleInventoryItemResponse.Parser.ParseFrom(response);
        }

        public async Task<UseItemCaptureResponse> UseItemCapture(ulong encounterId, ItemId itemRazzBerry, string spawnPointId)
        {
            var msg = new UseItemCaptureMessage();
            msg.EncounterId = encounterId;
            msg.ItemId = itemRazzBerry;
            msg.SpawnPointId = spawnPointId;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return UseItemCaptureResponse.Parser.ParseFrom(response);
        }

    }
}
