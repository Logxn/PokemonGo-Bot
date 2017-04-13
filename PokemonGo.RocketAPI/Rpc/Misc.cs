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
            var request = new Request
            {
                RequestType = RequestType.MarkTutorialComplete,
                RequestMessage = ((IMessage)new MarkTutorialCompleteMessage
                {
                    SendMarketingEmails = false,
                    SendPushNotifications = false,
                    TutorialsCompleted = { toComplete }
                }).ToByteString()
            };
            return await PostProtoPayloadCommonR<Request, EncounterTutorialCompleteResponse>(request).ConfigureAwait(false);
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