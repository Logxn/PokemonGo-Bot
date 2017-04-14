﻿#region using directives

using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
using POGOProtos.Networking.Platform.Responses;
using POGOProtos.Settings;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Login;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Responses;
using POGOProtos.Networking.Platform;

#endregion

namespace PokemonGo.RocketAPI.Rpc
{
    public delegate void GoogleDeviceCodeDelegate(string code, string uri);
    

    public class Login : BaseRpc
    {
        private readonly ILoginType _login;
        

        public Login(Client client) : base(client)
        {
            _login = SetLoginType(client.AuthType, client.Username,client.Password);
            Client.ApiUrl = Resources.RpcUrl;
        }

        private static ILoginType SetLoginType(AuthType type, string username, string password)
        {
            switch (type)
            {
                case AuthType.Google:
                    return new GoogleLogin(username, password);
                case AuthType.Ptc:
                    return new PtcLogin(username, password);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), "Unknown AuthType");
            }
        }

        public async Task Reauthenticate()
        {
            var tries = 0;
            Client.AuthToken = null;
            Client.AuthTicket = null;
            while (Client.AuthToken == null && tries <10)
            {
                Client.AuthToken = await _login.GetAccessToken().ConfigureAwait(false);
                if (Client.AuthToken == null){
                    tries++;
                    Logger.Warning("Access Token is null. Retrying in 1 second. Try " + tries);
                    await Task.Delay(1000).ConfigureAwait(false);
                }
            }
        }
        public async Task DoLogin()
        {
            await Reauthenticate().ConfigureAwait(false);
            
            if ( Client.AuthToken == null){
                throw new LoginFailedException("Connection with Server failed. Please, check if niantic servers are up");
            }

            Client.StartTime = Utils.GetTime(true);

            await RandomHelper.RandomDelay(1500).ConfigureAwait(false);

            await
                FireRequestBlock(CommonRequest.GetPlayerMessageRequest())
                    .ConfigureAwait(false);

            await RandomHelper.RandomDelay(2000).ConfigureAwait(false);

            await Client.Map.GetMapObjects().ConfigureAwait(false);
            
            MakeTuturial();
             
            foreach (var element in Client.Player.PlayerResponse.PlayerData.TutorialState) {
                Logger.Debug(element.ToString());
            } 
        }

        private void MakeTuturial()
        {
            
            var state = Client.Player.PlayerResponse.PlayerData.TutorialState;
            if (state == null || !state.Any()){
                var res = Client.Misc.AceptLegalScreen().Result;
                if (res.Result != EncounterTutorialCompleteResponse.Types.Result.Success)
                    return;
                Client.OnMakeTutorial();
            }
        }

        public async Task FireRequestBlock(Request request)
        {
            var requests = CommonRequest.AddChallengeRequest(request, Client);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests, true);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            var platfResponses = serverResponse.PlatformReturns;
            if (platfResponses != null)
            {
                var mapPlatform = platfResponses.FirstOrDefault(x => x.Type == PlatformRequestType.UnknownPtr8);
                if (mapPlatform != null)
                {
                    var unknownPtr8Response = UnknownPtr8Response.Parser.ParseFrom(mapPlatform.Response);
                    Resources.Api.UnknownPtr8Message = unknownPtr8Response.Message;
                }
            }

            switch (serverResponse.StatusCode)
            {
                case ResponseEnvelope.Types.StatusCode.SessionInvalidated:
                case ResponseEnvelope.Types.StatusCode.InvalidAuthToken:
                    Logger.Debug("Invalid token.");
                    Client.AuthToken = null;
                    throw new AccessTokenExpiredException();
                case ResponseEnvelope.Types.StatusCode.InvalidPlatformRequest:
                    Logger.Debug("Invalid Platform.");
                    Client.AuthToken = null;
                    throw new InvalidPlatformException();
                case ResponseEnvelope.Types.StatusCode.Redirect:
                    // 53 means that the api_endpoint was not correctly set, should be at this point, though, so redo the request
                    if (!string.IsNullOrEmpty(serverResponse.ApiUrl)){
                        Client.ApiUrl = "https://" + serverResponse.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + Client.ApiUrl);
                    }
                    Logger.Debug("Redirecting");
                    await FireRequestBlock(request).ConfigureAwait(false);
                    return;
                case ResponseEnvelope.Types.StatusCode.BadRequest:
                    // Your account may be banned! please try from the official client.
                    throw new LoginFailedException("Your account may be banned! please try from the official client.");
                case ResponseEnvelope.Types.StatusCode.Unknown:
                    break;
                case ResponseEnvelope.Types.StatusCode.Ok:
                    break;
                case ResponseEnvelope.Types.StatusCode.OkRpcUrlInResponse:
                    if (!string.IsNullOrEmpty(serverResponse.ApiUrl)){
                        Client.ApiUrl = "https://" + serverResponse.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + Client.ApiUrl);
                    }
                    break;
                case ResponseEnvelope.Types.StatusCode.InvalidRequest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (serverResponse.AuthTicket != null){
                Client.AuthTicket = serverResponse.AuthTicket;
                Logger.Debug("Received AuthTicket: " + Client.AuthTicket);
            }

            var responses = serverResponse.Returns;
            if (responses != null)
            {
                var getPlayerResponse = new GetPlayerResponse();
                if (1 <= responses.Count)
                {
                    getPlayerResponse.MergeFrom(responses[0]);
                    CommonRequest.ProcessGetPlayerResponse( Client, getPlayerResponse);
                }
                var checkChallengeResponse = new CheckChallengeResponse();
                if (2 <= responses.Count)
                {
                    checkChallengeResponse.MergeFrom(responses[1]);
                    CommonRequest.ProcessCheckChallengeResponse(Client, checkChallengeResponse);
                }
            }

        }

    }
}