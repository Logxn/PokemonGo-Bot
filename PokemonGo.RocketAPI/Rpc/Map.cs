#region using directives

using System;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
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
        private const int _minSecondsBetweenMapCalls = 30;
        private Tuple<GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> _cachedGetMap;
        
        public async
            Task
                <
                    Tuple
                        <GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse,
                            DownloadSettingsResponse, GetBuddyWalkedResponse>> GetMapObjects()
        {

            if (_lastGetMapRequest.AddSeconds(_minSecondsBetweenMapCalls).Ticks > DateTime.UtcNow.Ticks)
            {
                return _cachedGetMap;
            }

            #region Messages

            var getMapObjectsMessage = new GetMapObjectsMessage
            {
                CellId = { S2Helper.GetNearbyCellIds(Client.CurrentLongitude, Client.CurrentLatitude) },
                SinceTimestampMs = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
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

            var getMapObjectsRequest = new Request
            {
                RequestType = RequestType.GetMapObjects,
                RequestMessage = getMapObjectsMessage.ToByteString()
            };

            var request = await GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(getMapObjectsRequest, Client)).ConfigureAwait(false);
            //var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(getMapObjectsRequest, Client));

            Tuple<GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                await
                    PostProtoPayload
                        <Request, GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                            CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);

            GetInventoryResponse getInventoryResponse = response.Item4;
            CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);

            DownloadSettingsResponse downloadSettingsResponse = response.Item6;
            CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);

            CheckChallengeResponse checkChallengeResponse = response.Item2;
            CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);

            _lastGetMapRequest = DateTime.UtcNow;
            _cachedGetMap = response;

            return response;
        }

        public async Task<GetIncensePokemonResponse> GetIncensePokemons()
        {
            var message = new GetIncensePokemonMessage
            {
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return await PostProtoPayload<Request, GetIncensePokemonResponse>(RequestType.GetIncensePokemon, message).ConfigureAwait(false);
        }
    }
}