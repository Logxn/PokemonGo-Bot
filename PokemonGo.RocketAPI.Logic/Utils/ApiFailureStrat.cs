#region using directives

using System;
using System.Threading;
using System.Threading.Tasks;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Responses;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Shared;
#endregion


namespace PokeMaster.Logic
{
    public class ApiFailureStrat : IApiFailureStrategy
    {
        private readonly Client _session;
        private int _retryCount;
        public static Player _player;
        public static ISettings _settings;
        

        public ApiFailureStrat(Client session)
        {
            _session = session;
        }
        
        public void HandleCaptcha(string challengeUrl, ICaptchaResponseHandler captchaResponseHandler)
        {
           
            /* We recieve the token after the user has completed the captcha
             * The site will want to redirect the user, to the app again
             * So the redirect url would look like this: "unity:some-long-ass-code"
             * This "long-ass-code" is the responseToken
             * */
            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Are you an human?");
            Task.Run(() => {
                var chelper = new Utils.CaptchaHelper(challengeUrl);
                if (chelper.ShowDialog() == DialogResult.OK) {
                    var token = chelper.TOKEN;
                    captchaResponseHandler.SetCaptchaToken(token);
                             
                    // We will send a request, passing the long-ass-token and wait for a response.
                    VerifyChallengeResponse r = _player.VerifyChallenge(token);
                    if (r.Success) {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "TOKEN OK!");
                    } else {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Failure.");
                        HandleCaptcha(challengeUrl, captchaResponseHandler);
                    }
                    RandomHelper.RandomSleep(2100);
                } else {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Canceled.");
                    System.Console.ReadKey();
                    Environment.Exit(0);
                }
            }).Wait();

        }

        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            
            _retryCount = 0;
            
            switch (response.StatusCode) {
                case ResponseEnvelope.Types.StatusCode.OkRpcUrlInResponse:
                    if (!string.IsNullOrEmpty(response.ApiUrl)){
                        _session.ApiUrl = "https://" + response.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + _session.ApiUrl);
                    }
                    break;
            }
            if (response.AuthTicket != null){
                _session.SetAuthTicket (response.AuthTicket);
                Logger.Debug("Received AuthTicket: " + response.AuthTicket);
            }

        }

        public ApiOperation HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            if (_retryCount == 5){
                Logger.Debug("Too many tries. Aborting");
                return ApiOperation.Abort;
            }
            
            switch (response.StatusCode) {
                case ResponseEnvelope.Types.StatusCode.SessionInvalidated:
                case ResponseEnvelope.Types.StatusCode.InvalidAuthToken:
                    throw new AccessTokenExpiredException();
                case ResponseEnvelope.Types.StatusCode.InvalidPlatformRequest:
                    throw new InvalidPlatformException();
                case ResponseEnvelope.Types.StatusCode.Redirect:
                    if (!string.IsNullOrEmpty(response.ApiUrl)){
                        _session.ApiUrl = "https://" + response.ApiUrl + "/rpc";
                        Logger.Debug("New Client.ApiUrl: " + _session.ApiUrl);
                    }                    
                    throw new RedirectException();
                case ResponseEnvelope.Types.StatusCode.BadRequest:
                    return ApiOperation.Abort;
            }
            _retryCount++;
            Logger.Debug($"{response.StatusCode}: Retrying. Try {_retryCount}.");
            return ApiOperation.Retry;
        }
    }
}