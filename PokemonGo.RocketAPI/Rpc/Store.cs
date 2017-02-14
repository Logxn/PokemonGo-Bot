/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/02/2017
 * Time: 19:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using Google.Protobuf;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Platform.Requests;
using POGOProtos.Networking.Platform.Responses;
using POGOProtos.Networking.Requests;

namespace PokemonGo.RocketAPI.Rpc
{
    /// <summary>
    /// Description of Store.
    /// </summary>
    public class Store : BaseRpc
    {
        public Store(Client client) : base(client)
        {
        }
        public GetStoreItemsResponse GetStoreItems()
        {
            var message = new GetStoreMessage();
            var result  =  PostProtoPayload<Request, GetStoreItemsResponse>(RequestType.GetItemPack, message).Result;
            return result;
        }
        
        public BuyItemPokeCoinsResponse BuyItemPokeCoins(string item)
        {
            var message = new BuyItemPokeCoinsRequest()
            {
                ItemId = item
            };

            return PostProtoPayload<Request, BuyItemPokeCoinsResponse>(RequestType.BuyItemPack, message).Result;
        }
        
        
    }
    
        
}
