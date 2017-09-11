/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 15:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using POGOLib.Official.Net;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using Google.Protobuf;

namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Misc.
    /// </summary>
    public class Misc
    {
        internal Session session;
        public Misc(Session session)
        {
            this.session = session;
        }

        public async Task<MarkTutorialCompleteResponse> MarkTutorialComplete(RepeatedField<TutorialState> tutorials)
        {
            var msg = new MarkTutorialCompleteMessage();
            msg.TutorialsCompleted.AddRange(tutorials);
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request).ConfigureAwait(false);
            return MarkTutorialCompleteResponse.Parser.ParseFrom(response);
        }

        public async Task<MarkTutorialCompleteResponse> AceptLegalScreen()
        {
           return await MarkTutorialComplete(new RepeatedField<TutorialState>() { TutorialState.LegalScreen }).ConfigureAwait(false);
        }

    }
}
