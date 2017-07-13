#region using directives

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


            await RandomHelper.RandomDelay(500).ConfigureAwait(false);

            await DoEmptyRequest().ConfigureAwait(false);
            await GetPlayer().ConfigureAwait(false);
            await DownloadRemoteConfig().ConfigureAwait(false);
            // await GetAssetDigest().ConfigureAwait(false);
            // await DownloadItemTemplates().ConfigureAwait(false);
            // await GetDownloadUrls().ConfigureAwait(false);
            MakeTuturial();
            
            // This call (GetPlayerProfile) is only needed if the tutorial is done.
            // TODO: Check if tutorial is done to do not do GetPlayerProfile.
            await GetPlayerProfile().ConfigureAwait(false);

            await LevelUpRewards().ConfigureAwait(false);

            await RandomHelper.RandomDelay(2000).ConfigureAwait(false);

            await Client.Map.GetMapObjects().ConfigureAwait(false);
            
             
            foreach (var element in Client.Player.PlayerResponse.PlayerData.TutorialState) {
                Logger.Debug(element.ToString());
            } 
        }

        private void MakeTuturial()
        {
            
            var state = Client.Player.PlayerResponse.PlayerData.TutorialState;
            if (state == null )
                state = new Google.Protobuf.Collections.RepeatedField<POGOProtos.Enums.TutorialState>();
            if (!state.Contains(POGOProtos.Enums.TutorialState.LegalScreen)){
                var res = Client.Misc.AceptLegalScreen().Result;
                if (res.Result != EncounterTutorialCompleteResponse.Types.Result.Success)
                    return;
            }
            Client.OnMakeTutorial();
        }

        public async Task DoEmptyRequest()
        {

            var serverRequest = GetRequestBuilder().GetRequestEnvelope( new Request[]{}, true);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);
            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await DoEmptyRequest().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;
            if (responses != null)
            {
                for (int i = 0; i < responses.Count; i++) {
                    Logger.Debug($"Response {i}: responses[i]");
                }
            }
        }

        public void ParseServerResponse ( ResponseEnvelope serverResponse)
        {
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
        }

        public async Task GetPlayer()
        {
            var request = CommonRequest.GetPlayerMessageRequest();

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(new[]{request});

            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);

            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await GetPlayer().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;
            if (responses != null)
            {
                var getPlayerResponse = new GetPlayerResponse();
                if (responses.Count > 0)
                {
                    getPlayerResponse.MergeFrom(responses[0]);
                    CommonRequest.ProcessGetPlayerResponse( Client, getPlayerResponse);
                }
            }
        }

        public async Task DownloadRemoteConfig()
        {
            var request = CommonRequest.GetDownloadRemoteConfigVersionMessageRequest(Client);
            
            var requests = CommonRequest.FillRequest(request, Client, false, false);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);
            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await DownloadRemoteConfig().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if ( (responses != null) && ( responses.Count > 0 ) )
            {
                var downloadRemoteConfigVersionResponse = new DownloadRemoteConfigVersionResponse();
                downloadRemoteConfigVersionResponse.MergeFrom(responses[0]);
                CommonRequest.ProcessDownloadRemoteConfigVersionResponse( Client, downloadRemoteConfigVersionResponse);
                
                CommonRequest.ProcessCommonResponses( Client, responses, false, false);
            }
        }

        public async Task GetPlayerProfile()
        {
            var request = CommonRequest.GetPlayerProfileMessageRequest(Client.Player.PlayerResponse.PlayerData.Username);
            
            var requests = CommonRequest.FillRequest(request, Client, false, false);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);
            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await GetPlayerProfile().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if ( (responses != null) && ( responses.Count > 0 ) )
            {
                var getPlayerProfileResponse = new GetPlayerProfileResponse();
                getPlayerProfileResponse.MergeFrom(responses[0]);
                CommonRequest.ProcessGetPlayerProfileResponse( Client, getPlayerProfileResponse);
                CommonRequest.ProcessCommonResponses( Client, responses, false, false);
            }
        }

        public async Task LevelUpRewards()
        {
            var stat = Client.Inventory.GetPlayerStats( Client.Inventory.CachedInventory).FirstOrDefault();
            var request = CommonRequest.LevelUpRewardsMessageRequest(stat.Level);
            
            var requests = CommonRequest.FillRequest(request, Client, false, false);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);
            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await LevelUpRewards().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if ( (responses != null) && ( responses.Count > 0 ) )
            {
                var levelUpRewardsResponse = new LevelUpRewardsResponse();
                levelUpRewardsResponse.MergeFrom(responses[0]);
                CommonRequest.ProcessLevelUpRewardsResponse( Client, levelUpRewardsResponse);
                CommonRequest.ProcessCommonResponses( Client, responses, false, false);
            }
        }
        public async Task GetAssetDigest()
        {
            var request = CommonRequest.GetGetAssetDigestMessageRequest(Client);
            
            var requests = CommonRequest.FillRequest(request, Client, false, false);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);

            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await GetAssetDigest().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if (responses != null)
                if ( responses.Count > 0 )
                {
                    var getAssetDigestResponse = new GetAssetDigestResponse();
                    getAssetDigestResponse.MergeFrom(responses[0]);
                    CommonRequest.ProcessGetAssetDigestResponse( Client, getAssetDigestResponse);
                    CommonRequest.ProcessCommonResponses( Client, responses, false, false);
                }
        }
        public async Task DownloadItemTemplates()
        {
            var request = CommonRequest.DownloadItemTemplatesRequest(Client);
            
            var requests = CommonRequest.FillRequest(request, Client, false, false);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);

            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await DownloadItemTemplates().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if (responses != null)
                if ( responses.Count > 0 )
                {
                    var downloadItemTemplatesResponse = new DownloadItemTemplatesResponse();
                    downloadItemTemplatesResponse.MergeFrom(responses[0]);
                    CommonRequest.ProcessDownloadItemTemplatesResponse( Client, downloadItemTemplatesResponse);
                    CommonRequest.ProcessCommonResponses( Client, responses, false, false);
                }
        }
        public async Task GetDownloadUrls()
        {
            var request = CommonRequest.GetDownloadUrlsRequest(Client);
            
            var requests = CommonRequest.FillRequest(request, Client, false, true);

            var serverRequest = GetRequestBuilder().GetRequestEnvelope(requests);
            var serverResponse = await PostProto<Request>(serverRequest).ConfigureAwait(false);

            ParseServerResponse( serverResponse);

            if (serverResponse.StatusCode == ResponseEnvelope.Types.StatusCode.Redirect){
                await GetDownloadUrls().ConfigureAwait(false);
                return;
            }

            var responses = serverResponse.Returns;

            if (responses != null)
                if ( responses.Count > 0 )
                {
                    var getDownloadUrlsResponse = new GetDownloadUrlsResponse();
                    getDownloadUrlsResponse.MergeFrom(responses[0]);
                    CommonRequest.ProcessGetDownloadUrlsResponse( Client, getDownloadUrlsResponse);
                    CommonRequest.ProcessCommonResponses( Client, responses, false, true);
                }
        }
    }
}