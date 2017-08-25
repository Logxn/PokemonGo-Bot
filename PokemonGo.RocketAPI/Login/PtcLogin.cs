using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using PokemonGo.RocketAPI.Shared;

namespace PokemonGo.RocketAPI.Login
{
    class PtcLogin : ILoginType
    {
        private readonly string _password;
        private readonly string _username;
        private static Cookie jSessionId;
        private static Cookie CASTGC;
        private static string header_ticket;

        public class SessionData
        {
            public string Lt { get; set; }
            public string Execution { get; set; }
        }

        public static SessionData CurrentSessionData;

        public PtcLogin(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string ProviderId => "ptc";
        public string UserId => _username;

        private HttpClientHandler _handler;

        private static HttpClientHandler HandleRedirect = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
            UseProxy = Client.proxy != null,
            Proxy = Client.proxy,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        private static HttpClientHandler HandleNoRedirect = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            AllowAutoRedirect = true,
            UseProxy = Client.proxy != null,
            Proxy = Client.proxy,
            UseCookies = true,
            CookieContainer = new CookieContainer()
        };

        public async Task<string> GetAccessToken(bool ReAuthentification = true)    // true means we do not allow URL redirection. Only false when is 1st authentication at login
        {
            //if (ReAuthentification)
            //{
            //    _handler = HandleNoRedirect;
            //    //_handler.CookieContainer.Add(jSessionId);
            //    //_handler.CookieContainer.Add(CASTGC);
            //}
            //else
            //{
                _handler = HandleRedirect;
            //}
            /*
             * If ReAuthentication is true, we continue with the current session using the stored Cookies
             * If ReAuthentication is false, we want a completelly new session, so clean Cookies and log in again
             */

            if (!ReAuthentification)
            {
                if (CookieHelper.GetAllCookies(_handler.CookieContainer).Count > 0)
                {
                    _handler = new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                        AllowAutoRedirect = true,
                        UseProxy = Client.proxy != null,
                        Proxy = Client.proxy,
                        UseCookies = true,
                        CookieContainer = new CookieContainer()
                    };
                }
            }

            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(_handler, true);

            httpClient.DefaultRequestHeaders.Accept.TryParseAdd(Resources.Header_Login_Accept);
            httpClient.DefaultRequestHeaders.Host = Resources.Header_Login_Host;
            httpClient.DefaultRequestHeaders.Connection.TryParseAdd(Resources.Header_Login_Connection);
            httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(Resources.Header_Login_UserAgent);
            httpClient.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(LocaleInfo.Language);
            httpClient.DefaultRequestHeaders.AcceptEncoding.TryParseAdd(Resources.Header_Login_AcceptEncoding);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation(Resources.Header_Login_Manufactor, Resources.Header_Login_XUnityVersion);
            httpClient.Timeout = Resources.TimeOut;

            if (!ReAuthentification) CurrentSessionData = await GetSession(httpClient).ConfigureAwait(false);

            string ticket = await getTicket(httpClient, _username, _password, CurrentSessionData).ConfigureAwait(false);
            //var accessToken = await PostLoginOauth(httpClient, ticket).ConfigureAwait(false);

            Logger.Debug("Authenticated through PTC.");
            Logger.Debug("CASTGC Data");
            Logger.Debug("     Value: " + CASTGC.Value);
            Logger.Debug("   Expired: " + CASTGC.Expired);
            Logger.Debug("Expiration: " + CASTGC.Expires.ToString());
            Logger.Debug("  Creation: " + CASTGC.TimeStamp.ToString());
            Logger.Debug("   Discard: " + CASTGC.Discard);

            return ticket; // accessToken;
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
                    {"redirect_uri", Resources.UrlVar_RedirectUri},
                    {"locale", LocaleInfo.Language }
                });
            builder.Port = -1; // Removes :443 on builder URI
            builder.Query = query_content.ReadAsStringAsync().Result;

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(builder.ToString()).ConfigureAwait(false);

                Logger.Debug("SessionData: " + response.ToString());

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
            UriBuilder builder = new UriBuilder(Resources.UrlVar_TicketUrl);
            //UriBuilder builder = new UriBuilder(Resources.PtcLoginUrl);
            CookieCollection Cookies = new CookieCollection();

            FormUrlEncodedContent query_content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"service", Resources.UrlVar_Service }
                });
            builder.Port = -1; // Removes :443 on builder URI
            builder.Query = query_content.ReadAsStringAsync().Result;
            try
            {
                HttpResponseMessage loginResponse =
                    await httpClient.PostAsync(builder.ToString(), new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                    { "lt", sessionData.Lt },
                    { "execution", sessionData.Execution },
                    { "_eventId", Resources.UrlVar_EventId },
                    { "username", username },
                    { "password", password },
                    { "locale", LocaleInfo.Language }
                    })).ConfigureAwait(false);

                Logger.Debug("SessionData: " + loginResponse.ToString());

                //var loginResponseDataRaw = await loginResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

                Cookies = CookieHelper.GetAllCookies(_handler.CookieContainer);

                foreach (Cookie cookie in Cookies)
                {
                    if (cookie.Name == "JSESSIONID") jSessionId = cookie;
                    if (cookie.Name == "CASTGC") CASTGC = cookie;
                }

                HandleNoRedirect.CookieContainer = HandleRedirect.CookieContainer;

                header_ticket = loginResponse.RequestMessage.RequestUri.Query;

                //if (!loginResponseDataRaw.Contains("{"))
                //{
                //    var locationQuery = loginResponse.Headers.Location.Query;
                //    var ticketStartPosition = locationQuery.IndexOf("=", StringComparison.Ordinal) + 1;
                //    return locationQuery.Substring(ticketStartPosition, locationQuery.Length - ticketStartPosition);
                //}
                /*
                else
                {
                    var joinedResponseErrors = string.Join(",", (JArray)JObject.Parse(loginResponseDataRaw)["Errors"]);
                    throw new LoginFailedException(joinedResponseErrors);
                }
                */
                return CASTGC.Value;
            }
            catch (Exception ex)
            {
                throw new LoginFailedException(ex);
            }
        }

        // Uses Ticket to obtain the AccessToken necessary for the App
        private async Task<string> PostLoginOauth(System.Net.Http.HttpClient httpClient, string ticket)
        {
            FormUrlEncodedContent query = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", Resources.UrlVar_ClientId },
                    { "redirect_uri", Resources.UrlVar_RedirectUri },
                    { "client_secret", Resources.UrlVar_ClientSecret },
                    { "grant_type", Resources.UrlVar_GrantType },
                    { "code", header_ticket.Split('=')[1] },// ticket },
                    { "locale", LocaleInfo.Language }
                });

            HttpResponseMessage loginResponse =
                await httpClient.PostAsync(Resources.PtcLoginOauthUrl, new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", Resources.UrlVar_ClientId },
                    { "redirect_uri", Resources.UrlVar_RedirectUri },
                    { "client_secret", Resources.UrlVar_ClientSecret },
                    { "grant_type", Resources.UrlVar_GrantType },
                    { "code", header_ticket.Split('=')[1] },// ticket },
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