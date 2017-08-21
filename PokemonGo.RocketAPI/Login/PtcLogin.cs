using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Shared;
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
        private readonly string _password;
        private readonly string _username;

        private class SessionData
        {
            public string Lt { get; set; }
            public string Execution { get; set; }
        }

        public PtcLogin(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string ProviderId => "ptc";
        public string UserId => _username;

        private static readonly HttpClientHandler Handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
            UseProxy = Client.proxy != null,
            Proxy = Client.proxy,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        public async Task<string> GetAccessToken()
        {
            //using (var httpClientHandler = new HttpClientHandler())
            //{
            //    httpClientHandler.AllowAutoRedirect = true;
            //    httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //    httpClientHandler.UseProxy = Client.proxy != null;
            //    httpClientHandler.Proxy = Client.proxy;
            //    CookieContainer CookieContainer = new CookieContainer();

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(Handler, true); //(new RetryHandler(Handler));
            //System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            //using (var httpClient = new HttpClient.LoginHttpClient()) 
            //using (var httpClient = new System.Net.Http.HttpClient(new RetryHandler(Handler)))
            //{
                    httpClient.DefaultRequestHeaders.Accept.TryParseAdd(Resources.Header_Login_Accept);
                    httpClient.DefaultRequestHeaders.Host = Resources.Header_Login_Host;
                    httpClient.DefaultRequestHeaders.Connection.TryParseAdd(Resources.Header_Login_Connection);
                    httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Resources.Header_Login_UserAgent);
                    httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(LocaleInfo.Language);
                    httpClient.DefaultRequestHeaders.AcceptEncoding.TryParseAdd(Resources.Header_Login_AcceptEncoding);
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Resources.Header_Login_Manufactor, Resources.Header_Login_XUnityVersion);
                    httpClient.Timeout = Resources.TimeOut;

                    SessionData sessionData = await GetSession(httpClient).ConfigureAwait(false);
                    string ticket = await getTicket(httpClient, _username, _password, sessionData).ConfigureAwait(false);
                    var accessToken = await PostLoginOauth(httpClient, ticket).ConfigureAwait(false);

                    Logger.Debug("Authenticated through PTC.");

                    return accessToken;
                //}
            //}
        }

        // Get SessionData to start the login process (Lt and Execution)
        private async Task<SessionData> GetSession(System.Net.Http.HttpClient httpClient)
        {
            SessionData sessionResponse = new SessionData();
            UriBuilder builder = new UriBuilder(Resources.PtcLoginAuthorizeUrl);
            //UriBuilder builder = new UriBuilder(Resources.PtcLoginUrl);

            FormUrlEncodedContent query_content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    //{"service", Resources.UrlVar_Service }
                    {"client_id", Resources.UrlVar_ClientId },
                    {"redirect_uri", Resources.UrlVar_RedirectUri}//,
                    //{"locale", LocaleInfo.Language }
                });
            builder.Port = -1; // Removes :443 on builder URI
            builder.Query = query_content.ReadAsStringAsync().Result;

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(builder.ToString()).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode) throw new LoginFailedException("Error while trying PTC login: " + response.StatusCode);
                sessionResponse = JsonConvert.DeserializeObject<SessionData>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                throw new LoginFailedException(ex);
            }
            
            return sessionResponse;
        }

        // Uses SessionData to obtain a Ticket from the server
        private async Task<string> getTicket(System.Net.Http.HttpClient httpClient, string username, string password, SessionData sessionData)
        {
            HttpResponseMessage loginResponse =
                await httpClient.PostAsync(Resources.PtcLoginUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "lt", sessionData.Lt },
                    { "execution", sessionData.Execution },
                    { "_eventId", Resources.UrlVar_EventId },
                    { "username", username },
                    { "password", password }
                    //{ "locale", LocaleInfo.Language }
                })).ConfigureAwait(false);

            var loginResponseDataRaw = await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            //var kk = httpClient.Cookies.Count; // Ticket es una cookie
            //CASTGC TGT-16587 - WKK4watKBJdmCbwYRHsfLZzHbmdwhXfkf0ScZCuOopFpF7rEaY - sso.pokemon.com.sso.pokemon.com / sso / Session 82      ✓		
            //JSESSIONID  1CF2C27198F81706A210A62DA7F06D92 - n1 sso.pokemon.com / sso / Session 45  ✓

            if (loginResponse.RequestMessage.RequestUri.Query.Length > 0 && loginResponse.RequestMessage.RequestUri.Query.Contains("ticket="))
            {
                string ticket = loginResponse.RequestMessage.RequestUri.Query.Split('=')[1];
                return ticket;
            }

            //if (!loginResponseDataRaw.Contains("{"))
            //{
            //    var locationQuery = loginResponse.Headers.Location.Query;
            //    var ticketStartPosition = locationQuery.IndexOf("=", StringComparison.Ordinal) + 1;
            //    return locationQuery.Substring(ticketStartPosition, locationQuery.Length - ticketStartPosition);
            //}
            else
            {
                var joinedResponseErrors = string.Join(",", (JArray)JObject.Parse(loginResponseDataRaw)["Errors"]);
                throw new LoginFailedException(joinedResponseErrors);
            }
        }

        // Uses Ticket to obtain the AccessToken necessary for the App
        private async Task<string> PostLoginOauth(System.Net.Http.HttpClient httpClient, string ticket)
        {
            HttpResponseMessage loginResponse =
                await httpClient.PostAsync(Resources.PtcLoginOauthUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", Resources.UrlVar_ClientId },
                    { "redirect_uri", Resources.UrlVar_RedirectUri },
                    { "client_secret", Resources.UrlVar_ClientSecret },
                    { "grant_type", Resources.UrlVar_GrantType },
                    { "code", ticket },
                    { "locale", LocaleInfo.Language }
                }));

            if (loginResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new LoginFailedException(loginResponse.StatusCode);
            }

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


    }
}