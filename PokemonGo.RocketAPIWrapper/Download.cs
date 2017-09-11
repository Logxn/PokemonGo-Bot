/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2017
 * Time: 0:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using POGOLib.Official.Net;
using System.Linq;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using Google.Protobuf;


namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Download.
    /// </summary>
    public class Download
    {
        internal Session session;
        public Download(Session session)
        {
            this.session = session;
        }
        public async Task<DownloadItemTemplatesResponse> DownloadItemTemplates()
        {
            var msg = new DownloadItemTemplatesMessage();
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return DownloadItemTemplatesResponse.Parser.ParseFrom(response);
        }
        
    }
}
