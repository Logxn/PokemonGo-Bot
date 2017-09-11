/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2017
 * Time: 0:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading.Tasks;
using POGOLib.Official.Net;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using Google.Protobuf;

namespace PokemonGo.RocketAPIWrapper
{
    /// <summary>
    /// Description of Fort.
    /// </summary>
    public class Fort
    {
        internal Session session;
        public Fort(Session session)
        {
            this.session = session;
        }
        public async Task<FortDetailsResponse> FortDetails(string id, double latitude, double longitude)
        {
            var msg = new FortDetailsMessage();
            msg.FortId = id;
            msg.Latitude = latitude;
            msg.Longitude = longitude;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return FortDetailsResponse.Parser.ParseFrom(response);
        }
        public async Task<FortSearchResponse> FortSearch(string id, double latitude, double longitude)
        {
            var msg = new FortSearchMessage();
            msg.FortId = id;
            msg.FortLatitude = latitude;
            msg.FortLongitude = longitude;
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request);
            return FortSearchResponse.Parser.ParseFrom(response);
        }
    }
}
