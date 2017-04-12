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
        
        public Tuple<GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> _cachedGetMapResponse;

        public async
            Task
                <
                    Tuple
                        <GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse,
                            DownloadSettingsResponse, GetBuddyWalkedResponse>> GetMapObjects(bool forceRequest = false)
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

            var getMapObjectsRequest = new Request
            {
                RequestType = RequestType.GetMapObjects,
                RequestMessage = getMapObjectsMessage.ToByteString()
            };
            var tries = 0;
            while ( tries <10){
                try {
                   
                    var request =  GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(getMapObjectsRequest, Client));
                    var _getMapObjectsResponse =
                        await
                            PostProtoPayload
                                <Request, GetMapObjectsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                                    CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);
                    GetInventoryResponse getInventoryResponse = _getMapObjectsResponse.Item4;
                    CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
                    
                    DownloadSettingsResponse downloadSettingsResponse = _getMapObjectsResponse.Item6;
                    CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
                    
                    CheckChallengeResponse checkChallengeResponse = _getMapObjectsResponse.Item2;
                    CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
                    
                    // Here we refresh last time this request was done and cache
                    _lastGetMapRequest = DateTime.UtcNow;
                    _cachedGetMapResponse = _getMapObjectsResponse;
                    
                    return _getMapObjectsResponse;
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    await Client.Login.Reauthenticate().ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                } catch (InvalidPlatformException) {
                    Logger.Warning("Invalid Platform. Retrying in 1 second");
                    Client.Login.DoLogin().Wait();
                    Task.Delay(1000).Wait();
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null;
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