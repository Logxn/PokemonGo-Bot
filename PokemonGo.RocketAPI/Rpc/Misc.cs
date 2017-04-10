using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Misc : BaseRpc
    {
        public Misc(Client client)
            : base(client)
        {
        }

        public ClaimCodenameResponse ClaimCodename(string codename)
        {
            return
                PostProtoPayload<Request, ClaimCodenameResponse>(RequestType.ClaimCodename,
                new ClaimCodenameMessage() {
                    Codename = codename
                });
        }

        public EchoResponse SendEcho()
        {
            return PostProtoPayload<Request, EchoResponse>(RequestType.Echo, new EchoMessage());
        }

        public async Task<EncounterTutorialCompleteResponse> MarkTutorialComplete(
            RepeatedField<TutorialState> toComplete)
        {
            var markTutorialCompleteRequest = new Request
            {
                RequestType = RequestType.MarkTutorialComplete,
                RequestMessage = ((IMessage)new MarkTutorialCompleteMessage
                {
                    SendMarketingEmails = false,
                    SendPushNotifications = false,
                    TutorialsCompleted = { toComplete }
                }).ToByteString()
            };

            var tries = 0;
            while ( tries <10){
                try {
                    Logger.Debug("Using ApiUrl:" + Client.ApiUrl);
                    Logger.Debug("Using AuthToken:" + Client.AuthToken);
                    Logger.Debug("Using AuthTicket:" + Client.AuthTicket);
                    
                    var request = GetRequestBuilder().GetRequestEnvelope(CommonRequest.FillRequest(markTutorialCompleteRequest, Client));
        
                    Tuple<EncounterTutorialCompleteResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse, CheckAwardedBadgesResponse, DownloadSettingsResponse, GetBuddyWalkedResponse> response =
                        await
                            PostProtoPayload
                                <Request, EncounterTutorialCompleteResponse, CheckChallengeResponse, GetHatchedEggsResponse, GetInventoryResponse,
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
                    await Client.Login.Reauthenticate().ConfigureAwait(false);
                    await Task.Delay(1000).ConfigureAwait(false);
                } catch (RedirectException) {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                tries ++;
            }
            Logger.Error("Too many tries. Returning");
            return null;

        }
        
        public async Task<EncounterTutorialCompleteResponse> AceptLegalScreen()
        {
            EncounterTutorialCompleteResponse res = await MarkTutorialComplete(new RepeatedField<TutorialState>() {
                TutorialState.LegalScreen
            }).ConfigureAwait(false);

            return res;
        }
    }
}