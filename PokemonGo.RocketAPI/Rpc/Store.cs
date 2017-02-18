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
using POGOProtos.Networking.Envelopes;
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
        public Store(Client client)
            : base(client)
        {
        }

        public GetStoreItemsResponse GetStoreItems()
        {
            var plafReques = new RequestEnvelope.Types.PlatformRequest();
            plafReques.Type = POGOProtos.Networking.Platform.PlatformRequestType.GetStoreItems;
            var result = PostProtoPayload<Request, GetStoreItemsResponse>(plafReques);
            return result;
        }

        public BuyItemPokeCoinsResponse BuyItemPokeCoins(string item)
        {
            var message = new BuyItemPokeCoinsRequest() {
                ItemId = item
            };

            var plafReques = new RequestEnvelope.Types.PlatformRequest() {
                Type = POGOProtos.Networking.Platform.PlatformRequestType.BuyItemPokecoins,
                RequestMessage = message.ToByteString()
            };

            var result = PostProtoPayload<Request, BuyItemPokeCoinsResponse>(plafReques);
            return result;
        }
    }
}
