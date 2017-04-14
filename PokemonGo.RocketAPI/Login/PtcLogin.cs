using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PokemonGo.RocketAPI.Login
{
    class PtcLogin : ILoginType
    {
        readonly string password;
        readonly string username;

        public PtcLogin(string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public async Task<string> GetAccessToken()
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.AllowAutoRedirect = false;
                httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip; 
                httpClientHandler.UseProxy = Client.proxy != null; 
                httpClientHandler.Proxy = Client.proxy;

                using (var httpClient = new System.Net.Http.HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Resources.LoginUserAgent);
                    var loginData = await GetLoginData(httpClient);
                    if (loginData == null)
                        return null;
                    var ticket = await PostLogin(httpClient, this.username, this.password, loginData);
                    var accessToken = await PostLoginOauth(httpClient, ticket);
                    Logger.Debug("Authenticated through PTC.");
                    return accessToken;
                }
            }
        }

        /// <summary>
        /// Responsible for retrieving login parameters for <see cref="PostLogin" />.
        /// </summary>
        /// <param name="httpClient">An initialized <see cref="HttpClient" />.</param>
        /// <returns><see cref="LoginData" /> for <see cref="PostLogin" />.</returns>
        private async Task<SessionData> GetLoginData(System.Net.Http.HttpClient httpClient)
        {
            var loginDataResponse = await httpClient.GetAsync(Resources.PtcLoginUrl);
            if (loginDataResponse.StatusCode == HttpStatusCode.OK){
                try {
                    var loginData = JsonConvert.DeserializeObject<SessionData>(await loginDataResponse.Content.ReadAsStringAsync());
                    return loginData;
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
            }
            return null;
        }

        /// <summary>
        /// Responsible for submitting the login request.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="username">The user's PTC username.</param>
        /// <param name="password">The user's PTC password.</param>
        /// <param name="loginData"><see cref="LoginData" /> taken from PTC website using <see cref="GetLoginData" />.</param>
        /// <returns></returns>
        private async Task<string> PostLogin(System.Net.Http.HttpClient httpClient, string username, string password, SessionData loginData)
        {
            var loginResponse =
                await httpClient.PostAsync(Resources.PtcLoginUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"lt", loginData.Lt},
                    {"execution", loginData.Execution},
                    {"_eventId", "submit"},
                    {"username", username},
                    {"password", password}
                }));

            var loginResponseDataRaw = await loginResponse.Content.ReadAsStringAsync();
            if (!loginResponseDataRaw.Contains("{"))
            {
                var locationQuery = loginResponse.Headers.Location.Query;
                var ticketStartPosition = locationQuery.IndexOf("=", StringComparison.Ordinal) + 1;
                return locationQuery.Substring(ticketStartPosition, locationQuery.Length - ticketStartPosition);
            }

            var loginResponseData = JObject.Parse(loginResponseDataRaw);
            var loginResponseErrors = (JArray)loginResponseData["errors"];
            
            var joinedResponseErrors = string.Join(",", loginResponseErrors);
            throw new Exception($"Pokemon Trainer Club gave error(s): '{joinedResponseErrors}'");
        }

        /// <summary>
        /// Responsible for finishing the oauth login request.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        private async Task<string> PostLoginOauth(System.Net.Http.HttpClient httpClient, string ticket)
        {
            var loginResponse =
                await httpClient.PostAsync(Resources.PtcLoginOauth, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"client_id", "mobile-app_pokemon-go"},
                    {"redirect_uri", "https://www.nianticlabs.com/pokemongo/error"},
                    {"client_secret", "w8ScCUXJQc6kXKw8FiOhd8Fixzht18Dq3PEVkUCP5ZPxtgyWsbTvWHFLm2wNY0JR"},
                    {"grant_type", "refresh_token"},
                    {"code", ticket}
                }));

            var loginResponseDataRaw = await loginResponse.Content.ReadAsStringAsync();

            var oAuthData = Regex.Match(loginResponseDataRaw, "access_token=(?<accessToken>.*?)&expires=(?<expires>\\d+)");
            if (!oAuthData.Success)
                throw new Exception($"Couldn't verify the OAuth login response data '{loginResponseDataRaw}'.");

            //return new AccessToken
            //{
            //    Token = oAuthData.Groups["accessToken"].Value,
            //    Expiry = DateTime.UtcNow.AddSeconds(int.Parse(oAuthData.Groups["expires"].Value)),
            //    ProviderID = ProviderId
            //};
            return oAuthData.Groups["accessToken"].Value;
        }

        //private static string ExtractTicketFromResponse(HttpResponseMessage loginResp)
        //{
        //    var location = loginResp.Headers.Location;
        //    if (location == null)
        //        throw new LoginFailedException();

        //    var ticketId = HttpUtility.ParseQueryString(location.Query)["ticket"];

        //    if (ticketId == null)
        //        throw new PtcOfflineException();

        //    return ticketId;
        //}

        //private static IDictionary<string, string> GenerateLoginRequest(SessionData sessionData, string user,
        //    string pass)
        //    => new Dictionary<string, string>
        //    {
        //        {"lt", sessionData.Lt},
        //        {"execution", sessionData.Execution},
        //        {"_eventId", "submit"},
        //        {"username", user},
        //        {"password", pass}
        //    };

        //private static IDictionary<string, string> GenerateTokenVarRequest(string ticketId)
        //    => new Dictionary<string, string>
        //    {
        //        {"client_id", "mobile-app_pokemon-go"},
        //        {"redirect_uri", "https://www.nianticlabs.com/pokemongo/error"},
        //        {"client_secret", "w8ScCUXJQc6kXKw8FiOhd8Fixzht18Dq3PEVkUCP5ZPxtgyWsbTvWHFLm2wNY0JR"},
        //        {"grant_type", "refresh_token"},
        //        {"code", ticketId}
        //    };

        //private static async Task<string> GetLoginTicket(string username, string password,
        //    System.Net.Http.HttpClient tempHttpClient, SessionData sessionData)
        //{
        //    HttpResponseMessage loginResp;
        //    var loginRequest = GenerateLoginRequest(sessionData, username, password);
        //    using (var formUrlEncodedContent = new FormUrlEncodedContent(loginRequest))
        //    {
        //        loginResp =
        //            await tempHttpClient.PostAsync(Resources.PtcLoginUrl, formUrlEncodedContent).ConfigureAwait(false);
        //    }

        //    var ticketId = ExtractTicketFromResponse(loginResp);
        //    return ticketId;
        //}

        //private static async Task<SessionData> GetSessionCookie(System.Net.Http.HttpClient tempHttpClient)
        //{
        //    var sessionResp = await tempHttpClient.GetAsync(Resources.PtcLoginUrl).ConfigureAwait(false);
        //    var data = await sessionResp.Content.ReadAsStringAsync().ConfigureAwait(false);
        //    var sessionData = JsonConvert.DeserializeObject<SessionData>(data);
        //    return sessionData;
        //}

        //private static async Task<string> GetToken(System.Net.Http.HttpClient tempHttpClient, string ticketId)
        //{
        //    HttpResponseMessage tokenResp;
        //    var tokenRequest = GenerateTokenVarRequest(ticketId);
        //    using (var formUrlEncodedContent = new FormUrlEncodedContent(tokenRequest))
        //    {
        //        tokenResp =
        //            await tempHttpClient.PostAsync(Resources.PtcLoginOauth, formUrlEncodedContent).ConfigureAwait(false);
        //    }

        //    var tokenData = await tokenResp.Content.ReadAsStringAsync().ConfigureAwait(false);
        //    return HttpUtility.ParseQueryString(tokenData)["access_token"];
        //}

        private class SessionData
        {
            public string Lt { get; set; }
            public string Execution { get; set; }
        }
    }
}