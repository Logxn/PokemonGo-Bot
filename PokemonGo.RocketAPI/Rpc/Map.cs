#region using directives

using System;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using System.Linq;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class Map : BaseRpc
    {
        public Map(Client client) : base(client)
        {
        }

        private DateTime _lastGetMapRequest;
        private int _minSecondsBetweenMapCalls = 30;
        Tuple<GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> _cachedGetMapResponse;

        public async
            Task
                <
                    Tuple
                        <GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse,
                            DownloadSettingsResponse, GetBuddyWalkedResponse>> GetMapObjects()
        {
            var now = DateTime.UtcNow;

            // In case we did last _minSecondsBetweenMapCalls before, we return the cached response
            if (_lastGetMapRequest.AddSeconds(_minSecondsBetweenMapCalls).Ticks > now.Ticks)
            {
                return _cachedGetMapResponse;
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

            // Here we refresh last time this request was done and cache
            _lastGetMapRequest = DateTime.UtcNow;
            _cachedGetMapResponse = response;

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