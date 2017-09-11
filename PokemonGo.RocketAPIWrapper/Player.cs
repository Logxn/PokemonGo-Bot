/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2017
 * Time: 15:48
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
    /// Description of Player.
    /// </summary>
    public class Player
    {
        internal Session session;
        internal ISettings settings;

        public string Username {
            get;
            set;
        }
        public bool Warn {
            get;
            set;
        }
        public bool Banned {
            get;
            set;
        }

        public Player(Session session, ISettings settings)
        {
            this.session = session;
            this.settings = settings;
        }
        public async Task<GetPlayerResponse> GetPlayer()
        {
            var msg = new GetPlayerMessage();
            msg.PlayerLocale = new GetPlayerMessage.Types.PlayerLocale();
            msg.PlayerLocale.Country = settings.LocaleCountry;  //"US"
            msg.PlayerLocale.Language = settings.LocaleLanguage;  //"en";
            msg.PlayerLocale.Timezone = settings.LocaleTimezone;  //"US/Los Angeles";
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), System.Reflection.MethodBase.GetCurrentMethod().Name);
            request.RequestMessage = msg.ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request).ConfigureAwait(false);
            var rsp = GetPlayerResponse.Parser.ParseFrom(response);
            Username =rsp.PlayerData.Username;
            Warn = rsp.Warn;
            Banned = rsp.Banned;
            return rsp;
        }
        public async Task<GetPlayerResponse> GetPlayer2()
        {
            string name = System.Reflection.MethodBase.GetCurrentMethod().Name;
            var msg = Activator.CreateInstance(Type.GetType(name +"Message"));
            var playerLocale = new GetPlayerMessage.Types.PlayerLocale();
            playerLocale.Country = settings.LocaleCountry;  //"US"
            playerLocale.Language = settings.LocaleLanguage;  //"en";
            playerLocale.Timezone = settings.LocaleTimezone;  //"US/Los Angeles";
            msg.GetType().GetProperty("PlayerLocale").SetValue(msg, playerLocale);
            var request = new Request();
            request.RequestType = (RequestType) Enum.Parse(typeof(RequestType), name);
            request.RequestMessage = ((IMessage) msg).ToByteString();
            var response = await session.RpcClient.SendRemoteProcedureCallAsync(request).ConfigureAwait(false);
            var parser = Type.GetType(name +"Response").GetProperty("Parser").GetValue(null);
            var result = parser.GetType().GetMethod("ParseFrom").Invoke(parser, new []{response});
            return (GetPlayerResponse) result;
        }
        
        public void SetCoordinates(double latitude, double longitude)
        {
            session.Player.SetCoordinates(latitude, longitude);
        }

    }
}
