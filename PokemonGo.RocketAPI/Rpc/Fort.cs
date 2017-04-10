#region using directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Data.Battle;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public class Fort : BaseRpc
    {
        public Fort(Client client) : base(client)
        {
        }

        public FortDetailsResponse GetFortOnly(string fortId, double fortLatitude, double fortLongitude)
        {
            var message = new FortDetailsMessage
            {
                FortId = fortId,
                Latitude = fortLatitude,
                Longitude = fortLongitude
            };

            return PostProtoPayload<Request, FortDetailsResponse>(RequestType.FortDetails, message);
        }
        
        public async Task<FortDetailsResponse> GetFort(string fortId, double fortLatitude, double fortLongitude)
        {
            var getFortRequest = new Request
            {
                RequestType = RequestType.FortDetails,
                RequestMessage = ((IMessage)new FortDetailsMessage
                {
                    FortId = fortId,
                    Latitude = fortLatitude,
                    Longitude = fortLongitude
                }).ToByteString()
            };

            var tries = 0;
            while ( tries <10){
                try {
                    var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(getFortRequest, Client));
        
                    Tuple<FortDetailsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                            await PostProtoPayload
                                <Request, FortDetailsResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                    CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);
        
                    CheckChallengeResponse checkChallengeResponse = response.Item2;
                    CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
        
                    GetInventoryResponse getInventoryResponse = response.Item4;
                    CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
        
                    DownloadSettingsResponse downloadSettingsResponse = response.Item6;
                    CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
        
                    return response.Item1;
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    await Client.Login.Reauthenticate().ConfigureAwait(false);;
                    await Task.Delay(1000).ConfigureAwait(false);;
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);;
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null;
        }

        public FortSearchResponse SearchFortOnly(string fortId, double fortLat, double fortLng)
        {
            var message = new FortSearchMessage
            {
                FortId = fortId,
                FortLatitude = fortLat,
                FortLongitude = fortLng,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortSearchResponse>(RequestType.FortSearch, message);
        }
        
        public async Task<FortSearchResponse> SearchFort(string fortId, double fortLat, double fortLng)
        {
            var searchFortRequest = new Request
            {
                RequestType = RequestType.FortSearch,
                RequestMessage = ((IMessage)new FortSearchMessage
                {
                    FortId = fortId,
                    FortLatitude = fortLat,
                    FortLongitude = fortLng,
                    PlayerLatitude = Client.CurrentLatitude,
                    PlayerLongitude = Client.CurrentLongitude
                }).ToByteString()
            };

            var tries = 0;
            while ( tries <10){
                try {
                    Logger.Debug("Using ApiUrl:" + Client.ApiUrl);
                    Logger.Debug("Using AuthToken:" + Client.AuthToken);
                    Logger.Debug("Using AuthTicket:" + Client.AuthTicket);
                    
                        var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(searchFortRequest, Client));
            
                        Tuple<FortSearchResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                            await
                                PostProtoPayload
                                    <Request, FortSearchResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                                        CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);
            
                        CheckChallengeResponse checkChallengeResponse = response.Item2;
                        CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
            
                        GetInventoryResponse getInventoryResponse = response.Item4;
                        CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
            
                        DownloadSettingsResponse downloadSettingsResponse = response.Item6;
                        CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
                        
                        if (Client.Map._cachedGetMapResponse!=null){
                            var fort = Client.Map._cachedGetMapResponse.Item1.MapCells.SelectMany(x=> x.Forts).FirstOrDefault(y => y.Id == fortId);
                            if (fort!=null)
                                fort.CooldownCompleteTimestampMs = Utils.GetTime(true) + 5 * 60 * 1000; // Cooldown is 5 minutes.
                        }
                        
            
                        return response.Item1;
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    await Client.Login.Reauthenticate().ConfigureAwait(false);;
                    await Task.Delay(1000).ConfigureAwait(false);;
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);;
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null;
        }

        public AddFortModifierResponse AddFortModifier(string fortId, ItemId modifierType)
        {
            var message = new AddFortModifierMessage
            {
                FortId = fortId,
                ModifierType = modifierType,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, AddFortModifierResponse>(RequestType.AddFortModifier, message);
        }

        public AttackGymResponse AttackGym(string fortId, string battleId, List<BattleAction> battleActions,
            BattleAction lastRetrievedAction)
        {
            var message = new AttackGymMessage
            {
                BattleId = battleId,
                GymId = fortId,
                LastRetrievedAction = lastRetrievedAction,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude,
                AttackActions = { battleActions }
            };

            message.AttackActions.AddRange(battleActions);

            return PostProtoPayload<Request, AttackGymResponse>(RequestType.AttackGym, message);
        }

        public FortDeployPokemonResponse FortDeployPokemon(string fortId, ulong pokemonId)
        {
            var message = new FortDeployPokemonMessage
            {
                PokemonId = pokemonId,
                FortId = fortId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortDeployPokemonResponse>(RequestType.FortDeployPokemon, message);
        }

        public FortRecallPokemonResponse FortRecallPokemon(string fortId, ulong pokemonId)
        {
            var message = new FortRecallPokemonMessage
            {
                PokemonId = pokemonId,
                FortId = fortId,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, FortRecallPokemonResponse>(RequestType.FortRecallPokemon, message);
        }

        public GetGymDetailsResponse GetGymDetails(string gymId, double gymLat, double gymLng)
        {
            var message = new GetGymDetailsMessage
            {
                GymId = gymId,
                GymLatitude = gymLat,
                GymLongitude = gymLng,
                PlayerLatitude = Client.CurrentLatitude,
                PlayerLongitude = Client.CurrentLongitude
            };

            return PostProtoPayload<Request, GetGymDetailsResponse>(RequestType.GetGymDetails, message);
        }

        public StartGymBattleResponse StartGymBattleOnly(string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            var message = new StartGymBattleMessage ();
            message.GymId = gymId;
            message.DefendingPokemonId = defendingPokemonId;
            message.AttackingPokemonIds.Add(attackingPokemonIds);
            message.PlayerLatitude = Client.CurrentLatitude;
            message.PlayerLongitude = Client.CurrentLongitude;

            return PostProtoPayload<Request, StartGymBattleResponse>(RequestType.StartGymBattle, message);
        }
        
        public async Task<StartGymBattleResponse> StartGymBattle(string gymId, ulong defendingPokemonId,
            IEnumerable<ulong> attackingPokemonIds)
        {
            var startGymBattleRequest = new Request
            {
                RequestType = RequestType.StartGymBattle,
                RequestMessage = ((IMessage)new StartGymBattleMessage
                {
                    GymId = gymId,
                    DefendingPokemonId = defendingPokemonId,
                    AttackingPokemonIds = { attackingPokemonIds },
                    PlayerLatitude = Client.CurrentLatitude,
                    PlayerLongitude = Client.CurrentLongitude
                }).ToByteString()
            };

            var tries = 0;
            while ( tries <10){
                try {
                   
                    var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(startGymBattleRequest, Client));
        
                    Tuple<StartGymBattleResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                        await
                            PostProtoPayload
                                <Request, StartGymBattleResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
                                    CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse>(request).ConfigureAwait(false);
        
                    CheckChallengeResponse checkChallengeResponse = response.Item2;
                    CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
        
                    GetInventoryResponse getInventoryResponse = response.Item4;
                    CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
        
                    DownloadSettingsResponse downloadSettingsResponse = response.Item6;
                    CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
        
                    return response.Item1;
                } catch (AccessTokenExpiredException) {
                    Logger.Warning("Invalid Token. Retrying in 1 second");
                    await Client.Login.Reauthenticate().ConfigureAwait(false);;
                    await Task.Delay(1000).ConfigureAwait(false);;
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);;
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null;

        }
    }
}