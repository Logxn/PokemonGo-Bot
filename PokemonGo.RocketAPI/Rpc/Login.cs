using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Login;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI.Rpc
{
    public delegate void GoogleDeviceCodeDelegate(string code, string uri);
    public class Login : BaseRpc
    {
        //public event GoogleDeviceCodeDelegate GoogleDeviceCodeEvent;
        private ILoginType login;

        public Login(Client client) : base(client)
        {
            login = SetLoginType(client.Settings);
            Client.ApiUrl = Resources.RpcUrl;
        }

        private static ILoginType SetLoginType(ISettings settings)
        {
            switch (settings.AuthType)
            {
                case AuthType.Google:
                    return new GoogleLogin(settings.GoogleUsername, settings.GooglePassword);
                case AuthType.Ptc:
                    return new PtcLogin(settings.PtcUsername, settings.PtcPassword, settings);
                default:
                    throw new ArgumentOutOfRangeException(nameof(settings.AuthType), "Unknown AuthType");
            }
        }

        public async Task DoLogin()
        {
            Client.AuthToken = await login.GetAccessToken().ConfigureAwait(false);
            Client.StartTime = Utils.GetTime(true);

            await Worker(CommonRequest.GetDownloadRemoteConfigVersionMessageRequest(Client)).ConfigureAwait(false);
            await Worker(CommonRequest.GetGetAssetDigestMessageRequest(Client)).ConfigureAwait(false);
        }

        private async Task Worker(Request request)
        {


            var requests = CommonRequest.FillRequest(request, Client);


            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests, true);
            var serverResponse = await PostProto<Request>(serverRequest);

            if (!string.IsNullOrEmpty(serverResponse.ApiUrl))
                Client.ApiUrl = "http://"+serverResponse.ApiUrl;

            Logger.Error("CLIENT API URL: " + Client.ApiUrl);

            if (serverResponse.AuthTicket != null)
                Client.AuthTicket = serverResponse.AuthTicket;
            
            
            switch (serverResponse.StatusCode)
            {
                case ResponseEnvelope.Types.StatusCode.InvalidAuthToken:
                    Client.AuthToken = null;
                    throw new AccessTokenExpiredException();
                case ResponseEnvelope.Types.StatusCode.Redirect:
                    // 53 means that the api_endpoint was not correctly set, should be at this point, though, so redo the request
                    await Worker(request);
                    return;
                case ResponseEnvelope.Types.StatusCode.BadRequest:
                    // Your account may be banned! please try from the official client.
                    throw new LoginFailedException("Your account may be banned! please try from the official client.");
                case ResponseEnvelope.Types.StatusCode.Unknown:
                    break;
                case ResponseEnvelope.Types.StatusCode.Ok:
                    break;
                case ResponseEnvelope.Types.StatusCode.OkRpcUrlInResponse:
                    break;
                case ResponseEnvelope.Types.StatusCode.InvalidRequest:
                    break;
                case ResponseEnvelope.Types.StatusCode.InvalidPlatformRequest:
                    break;
                case ResponseEnvelope.Types.StatusCode.SessionInvalidated:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var responses = serverResponse.Returns;
            if (responses != null)
            {
                var getInventoryResponse = new GetInventoryResponse();
                if (4 <= responses.Count)
                {
                    getInventoryResponse.MergeFrom(responses[3]);

                    CommonRequest.ProcessGetInventoryResponse(Client, getInventoryResponse);
                }

                var downloadSettingsResponse = new DownloadSettingsResponse();
                if (6 <= responses.Count)
                {
                    downloadSettingsResponse.MergeFrom(responses[5]);

                    CommonRequest.ProcessDownloadSettingsResponse(Client, downloadSettingsResponse);
                }
            }
        }
    }
}
