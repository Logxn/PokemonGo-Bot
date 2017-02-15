using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Misc : BaseRpc
    {
        public Misc(Client client) : base(client)
        {
        }

        public async Task<ClaimCodenameResponse> ClaimCodename(string codename)
        {
            return
                    PostProtoPayload<Request, ClaimCodenameResponse>(RequestType.ClaimCodename,
                        new ClaimCodenameMessage()
                        {
                            Codename = codename
                        });
        }

        public async Task<EchoResponse> SendEcho()
        {
            return PostProtoPayload<Request, EchoResponse>(RequestType.Echo, new EchoMessage());
        }

        public async Task<EncounterTutorialCompleteResponse> MarkTutorialComplete()
        {
            return PostProtoPayload<Request, EncounterTutorialCompleteResponse>(RequestType.MarkTutorialComplete, new MarkTutorialCompleteMessage());
        }
    }
}