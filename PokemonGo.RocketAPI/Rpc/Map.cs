#region using directives

using System;
using System.Threading.Tasks;
using System.Linq;
using Google.Protobuf;
using POGOProtos.Map;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Responses;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class Map : BaseRpc
    {
        public Map(Client client) : base(client)
        {
        }

        private DateTime _lastGetMapRequest;
        private const int _minSecondsBetweenMapCalls = 20;
        
        public GetMapObjectsResponse _cachedGetMapResponse;

        public async
            Task
                <GetMapObjectsResponse> GetMapObjects(bool forceRequest = false)
        {
            // In case we did last _minSecondsBetweenMapCalls before, we return the cached response
            if (_lastGetMapRequest.AddSeconds(_minSecondsBetweenMapCalls).Ticks > DateTime.UtcNow.Ticks && !forceRequest)
            {
                return _cachedGetMapResponse;
            }

            #region Messages
            
            var cellIds = S2Helper.GetNearbyCellIds(Client.CurrentLongitude, Client.CurrentLatitude).ToArray();
            var sinceTimeMs = new long[cellIds.Length];
            for  (var index = 0; index < cellIds.Length; index++)
            {   
                /*MapCell cell = null;
                if (_cachedGetMapResponse!=null)
                    cell = _cachedGetMapResponse.Item1.MapCells.FirstOrDefault(x => x.S2CellId == cellIds[index]);
                sinceTimeMs[index] = cell != null ? cell.CurrentTimestampMs : 0;
                */
               sinceTimeMs[index] = 0;
            }
            var getMapObjectsMessage = new GetMapObjectsMessage
            {
                CellId = {cellIds},
                SinceTimestampMs = {sinceTimeMs},
                Latitude = Client.CurrentLatitude,
                Longitude = Client.CurrentLongitude
            };

            var getHatchedEggsMessage = new GetHatchedEggsMessage();

            var getInventoryMessage = new GetInventoryMessage
            {
                LastTimestampMs = Client.InventoryLastUpdateTimestamp
            };

            var checkAwardedBadgesMessage = new CheckAwardedBadgesMessage();

            var downloadSettingsMessage = new DownloadSettingsMessage
            {
                Hash = Client.SettingsHash
            };

            #endregion

            var request = new Request
            {
                RequestType = RequestType.GetMapObjects,
                RequestMessage = getMapObjectsMessage.ToByteString()
            };
            return await PostProtoPayloadCommonR<Request, GetMapObjectsResponse>(request);

        }

        public GetIncensePokemonResponse GetIncensePokemons()
        {
            var message = new GetIncensePokemonMessage
            {
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return  PostProtoPayload<Request, GetIncensePokemonResponse>(RequestType.GetIncensePokemon, message);
        }
    }
}