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
                PostProtoPayloadCommonR<Request, ClaimCodenameResponse>(RequestType.ClaimCodename,
                new ClaimCodenameMessage() {
                    Codename = codename
                }).Result;
        }

        public async Task<MarkTutorialCompleteResponse> MarkTutorialComplete(
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
            return await PostProtoPayloadCommonR<Request, MarkTutorialCompleteResponse> (request).ConfigureAwait(false);
        }
        
        public async Task<MarkTutorialCompleteResponse> AceptLegalScreen()
        {
            MarkTutorialCompleteResponse res = await MarkTutorialComplete(new RepeatedField<TutorialState>() {
                TutorialState.LegalScreen
            }).ConfigureAwait(false);

            return res;
        }

        public UpdateNotificationResponse UpdateNotificationMessage() // IEnumerable<string> Notification_Ids, IEnumerable<Int64> TimeStampsMS)
        {
            var request = new Request
            {
                RequestType = RequestType.UpdateNotificationStatus,
                RequestMessage = ((IMessage)new UpdateNotificationMessage
                {
                    //NotificationIds = { Notification_Ids },
                    //CreateTimestampMs = { TimeStampsMS },
                    State = NotificationState.Viewed
                }).ToByteString()
            };
            var response = PostProtoPayloadCommonR<Request, UpdateNotificationResponse>(request).Result;
            return response;
        }
    }
}