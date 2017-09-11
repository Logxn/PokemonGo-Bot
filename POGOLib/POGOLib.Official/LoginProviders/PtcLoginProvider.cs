using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POGOLib.Official;
using POGOLib.Official.Logging;
using POGOLib.Official.Net.Authentication.Data;

namespace POGOLib.Official.LoginProviders
{

    public class PtcLoginProvider : ILoginProvider
    {
        private readonly string _username;
        private readonly string _password;

        public PtcLoginProvider(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string ProviderId => "ptc";

        public string UserId => _username;

        public string Proxy => Configuration.ProxyAddress;

        public async Task<AccessToken> GetAccessToken()
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.AllowAutoRedirect = false;
                if (Proxy != string.Empty) {
                    httpClientHandler.Proxy = new WebProxy(Proxy, true);
                }                
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Constants.LoginUserAgent);
                    var loginData = await GetLoginData(httpClient);
                    var ticket = await PostLogin(httpClient, _username, _password, loginData);
                    var accessToken = await PostLoginOauth(httpClient, ticket);
                    accessToken.Username = _username;
                    Logger.Debug("Authenticated through PTC.");
                    return accessToken;
                }
            }
        }

        private async Task<LoginData> GetLoginData(HttpClient httpClient)
        {
            var loginDataResponse = await httpClient.GetAsync("");
            LoginData loginData = JsonConvert.DeserializeObject<LoginData>(await loginDataResponse.Content.ReadAsStringAsync());
            return loginData;
        }

        private async Task<string> PostLogin(HttpClient httpClient, string username, string password, LoginData loginData)
        {
            var loginResponse =
                await httpClient.PostAsync(Constants.LoginUrl, new FormUrlEncodedContent(new Dictionary<string, string>
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

            throw new PtcLoginException("Pokemon Trainer Club gave error(s): '"+string.Join(",", loginResponseErrors)+"'");
        }

        private async Task<AccessToken> PostLoginOauth(HttpClient httpClient, string ticket)
        {
            var loginResponse =
                await httpClient.PostAsync(Constants.LoginOauthUrl, new FormUrlEncodedContent(new Dictionary<string, string>
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
                throw new PtcLoginException("Couldn't verify the OAuth login response data '"+loginResponseDataRaw+"'.");

            return new AccessToken
            {
                Token = oAuthData.Groups["accessToken"].Value,
                Expiry = DateTime.UtcNow.AddSeconds(int.Parse(oAuthData.Groups["expires"].Value)),
                ProviderID = ProviderId
            };
        }
    }
}
