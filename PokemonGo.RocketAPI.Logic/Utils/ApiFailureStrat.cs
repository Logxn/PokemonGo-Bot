#region using directives

using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Rpc;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Responses;

#endregion

namespace PokemonGo.RocketAPI.Logic
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

        public async void HandleCaptcha(string challengeUrl, ICaptchaResponseHandler captchaResponseHandler)
        {
           
            /* We recieve the token after the user has completed the captcha
             * The site will want to redirect the user, to the app again
             * So the redirect url would look like this: "unity:some-long-ass-code"
             * This "long-ass-code" is the responseToken
             * */
            string token = "";
            Logger.Error("We need a Captcha.. (Not implemented yet)");
            Logger.Error($"[DEBUG - PLEASE IGNORE]: Challenge URL!!: {challengeUrl.ToString()}");

            captchaResponseHandler.SetCaptchaToken("bla"); // I dont know why, but lets just set the token here just in case.

            /* Here is where I need the gui.
             * It should have a WebBrowser, that will redirect to the challengeUrl
             * We somehow need to create a function that always looks if the url has changed (it will because the user solves the captcha and they want to redirect)
             * Now we need to remove the "unity:" infront of the url and pass the token to the _player.VerifyChallenge
             * We need to wait for a response and if its a success....
             * ... we restart the bot! ;)
             **/

            VerifyChallengeResponse r = await _player.VerifyChallenge(token); // We will send a request, passing the long-ass-token and wait for a response.

            if (r.Success)
            {
                Console.WriteLine("TOKEN OK!");
            }
            else
            {
                Console.WriteLine("Failure.");
            }

            Console.ReadKey();
            Environment.Exit(0);
        }

        public async Task<ApiOperation> HandleApiFailure()
        {
            if (_retryCount == 11)
                return ApiOperation.Abort;

            await Task.Delay(500);
            _retryCount++;

            if (_retryCount % 5 == 0)
            {
                DoLogin();
            }

            return ApiOperation.Retry;
        }

        public void HandleApiSuccess()
        {
            _retryCount = 0;
        }

        private async void DoLogin()
        {
            try
            {
                if (_session.Settings.AuthType == AuthType.Google || _session.Settings.AuthType == AuthType.Ptc)
                {
                    await _session.Login.DoLogin();
                }
                else
                {
                    Logger.Error("Wrong AuthType?");
                }
            }
            catch (AggregateException ae)
            {
                throw ae.Flatten().InnerException;
            }
            catch (LoginFailedException)
            {
                Logger.Error("Wrong Username or Password?");
            }
            catch (AccessTokenExpiredException)
            {
                Logger.Error("Access Token expired? Retrying in 1 second.");

                await Task.Delay(1000);
            }
            catch (PtcOfflineException)
            {
                Logger.Error("PTC probably offline? Retrying in 15 seconds.");

                await Task.Delay(15000);
            }
            catch (InvalidResponseException)
            {
                Logger.Error("Invalid Response, retrying in 5 seconds.");
                await Task.Delay(5000);
            } catch (NullReferenceException e)
            {
                Logger.Error("Method which calls that: " + e.TargetSite + " Source: " + e.Source + " Data: " + e.Data);
            }
            catch (GoogleException)
            {

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response)
        {
            
            _retryCount = 0;

                /*Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Accuracy: {request.Accuracy}");
                //Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"--------------[AUTH INFO]-------------");
                //Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Auth Provider: {request.AuthInfo.Provider}");
                //Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Auth Token: {request.AuthInfo.Token}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"------------[AUTHTICKET INFO]------------");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"AuthTicket End: {request.AuthTicket.End}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"AuthTicket Expire (MS): {request.AuthTicket.ExpireTimestampMs}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"AuthTicket Start: {request.AuthTicket.Start}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"-----------------------------------------");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Latitude: {request.Latitude}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Longitude: {request.Longitude}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Milliseconds SinceLastLocationFix: {request.MsSinceLastLocationfix}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"PlatformRequests: {request.PlatformRequests}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"RequestId: {request.RequestId}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Requests: {request.Requests}");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Status Code: {request.StatusCode}");*/
            
        }

        public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response)
        {
            if (_retryCount == 11)
                return ApiOperation.Abort;

            await Task.Delay(500);
            _retryCount++;

            if (_retryCount % 5 == 0)
            {
                try
                {
                    DoLogin();
                }
                catch (PtcOfflineException)
                {
                    await Task.Delay(20000);
                }
                catch (AccessTokenExpiredException)
                {
                    await Task.Delay(2000);
                }
                catch (Exception ex) when (ex is InvalidResponseException || ex is TaskCanceledException)
                {
                    await Task.Delay(1000);
                }
            }

            return ApiOperation.Retry;
        }
    }
}