using Newtonsoft.Json;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Hash
{
    public class PokeHashHasher : IHasher
    {
        // ***************************************************************************
        // This value will determine which version of hashing you receive.
        // ***************************************************************************

        int MaxRequestCount;
        // RPM Value
        DateTime RatePeriodEnd;
        // End of running minute
        int RateRequestRemaining;
        // Unused Requests this minute
        int RateLimitSeconds;
        // Rate Limit Period (always 60)
        DateTime AuthTokenExpiration;
        // Expiration
        int ExpirationCounter = 1;
        // Only show message every 1000 requests (see if down)
        bool NoValidKey = false;
        // Will be true if no valid key is found (testing)

        private static Uri _baseAddress;
        //private static readonly Uri _baseAddress = new Uri("https://pokehash.buddyauth.com/");
        //private static readonly Uri _baseAddress2 = new Uri("http://pokehash.buddyauth.com/");

        //private Uri _availableHashVersionsCheck = new Uri("https://pokehash.buddyauth.com/api/hash/versions");
        private readonly string _endpoint;
        private string apiKey;

        public PokeHashHasher(string apiKey)
        {
            Resources.Api.SetAPIKeyHashServerURL(apiKey);
            _baseAddress = new Uri(Resources.Api.HashServerInfo.URL.ToString());
            _endpoint = Resources.Api.EndPoint;
            this.apiKey = apiKey;
        }

        #region Sync Methods
        public HashResponseContent RequestHashes(HashRequestContent request)
        {
            int retry = 3;
            int cyclingRetrys = 40;
            bool changeKey;
            do {
                changeKey = false;
                try {
                    if (!NoValidKey)
                        return InternalRequestHashes(request);
                } catch (HasherException hashEx) {
                    changeKey = Shared.KeyCollection.ExistsFile();
                    cyclingRetrys--;
                    Logger.Error(hashEx.Message);
                    if (cyclingRetrys < 0)
                        throw hashEx;
                } catch (Exception ex) {
                    Logger.Debug("Error: PokeHashHasher.cs - RequestHashes()");
                    Logger.Debug( ex.Message);
                }

                if (changeKey) {
                    var nextKey = Shared.KeyCollection.nextKey();
                    if (nextKey != "") {
                        this.apiKey = nextKey;
                        Logger.Debug("Changing KEY to: " + this.apiKey.Substring(0, 5));
                    } else {
                        NoValidKey = true;
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: PokeHashHasher.cs - NO VALID KEY FOUND - STOPPING");
                        System.Console.ReadKey();
                        Environment.Exit(-1);
                    }
                    RandomHelper.RandomSleep(250, 300);
                } else {
                    RandomHelper.RandomSleep(1000, 1100);
                    retry--;
                }
            } while (retry > 0);

            throw new HasherException("[HasherExeption] Default: pokefamer Hash API server might down");
        }

        private HashResponseContent InternalRequestHashes(HashRequestContent request)
        {
            using (var client = new System.Net.Http.HttpClient()) {
                client.BaseAddress = _baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = null;  
                try {
                    response = client.PostAsync(_endpoint, content).Result;
                } catch (Exception) {
                    //client.BaseAddress = _baseAddress2;
                    //response = client.PostAsync(_endpoint, content).Result;
                }

                switch (response.StatusCode) {
                    case HttpStatusCode.OK: //200
                        AuthTokenExpiration = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-AuthTokenExpiration"))[0])).ToLocalTime();
                        MaxRequestCount = Convert.ToUInt16(((string[])response.Headers.GetValues("X-MaxRequestCount"))[0]);
                        RatePeriodEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-RatePeriodEnd"))[0])).ToLocalTime();
                        RateRequestRemaining = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateRequestsRemaining"))[0]);
                        RateLimitSeconds = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateLimitSeconds"))[0]);
                        var remainingSeconds = (DateTime.Now - RatePeriodEnd).TotalSeconds * -1;

                        Logger.Debug($"{RateRequestRemaining}/{MaxRequestCount} requests remaining for the next {remainingSeconds} seconds. Key expires on: {AuthTokenExpiration}");
                        if ((AuthTokenExpiration - DateTime.Now).TotalDays <= 3) {
                            if (ExpirationCounter-- == 0) {
                                ExpirationCounter = 1000;
                                Logger.Warning( $"Attention! Your key is expiring in {(AuthTokenExpiration - DateTime.Now).Days} days and {(AuthTokenExpiration - DateTime.Now).Hours} hours! Expiration date: {AuthTokenExpiration}");
                            }
                        }
                        return JsonConvert.DeserializeObject<HashResponseContent>(response.Content.ReadAsStringAsync().Result);

                    case HttpStatusCode.BadRequest: // 400
                        var responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"[HashService] 400: Your key is probably expired! {responseText}");

                    case HttpStatusCode.Unauthorized: // 401
                        Shared.KeyCollection.removeKey(this.apiKey);
                        throw new HasherException("[HashService] 401: Your PF-Hashkey you provided is incorrect (or not valid anymore). ");

                    case (HttpStatusCode)429: // To many requests
                        responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"[HashService] 429: Your request has been limited (Message : {responseText})");

                    case HttpStatusCode.ServiceUnavailable:
                        responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"[HashService] 503: It seems PokeFarmer server {_baseAddress}{_endpoint} is unavailable (Message : {responseText}) ");

                    default:
                        RandomHelper.RandomSleep(10000, 11000);
                        throw new HasherException($"[HashService] Unknown: Pokefamer Hash API ({_baseAddress}{_endpoint}) might down!");
                }
            }
        }

        public static string[] GetInformation(string apiKey)
        {
            var result = new []{"",""};
            var client = new System.Net.Http.HttpClient();

            Resources.Api.SetAPIKeyHashServerURL(apiKey);
            _baseAddress = Resources.Api.HashServerInfo.URL;

            client.BaseAddress = _baseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-AuthToken", apiKey);
            
            var hashRequest = new HashRequestContent();
            
            var content = new StringContent(JsonConvert.SerializeObject( hashRequest), Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage response = null;  
            try {
                response = client.PostAsync(Resources.Api.EndPoint, content).Result;
            } catch (Exception) {
                //client.BaseAddress = _baseAddress2;
                //response = client.PostAsync(Resources.Api.EndPoint, content).Result;
            }
            try {
                    var AuthTokenExpiration = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-AuthTokenExpiration"))[0])).ToLocalTime();
                    var MaxRequestCount = Convert.ToUInt16(((string[])response.Headers.GetValues("X-MaxRequestCount"))[0]);
                    var RatePeriodEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-RatePeriodEnd"))[0])).ToLocalTime();
                    var RateRequestRemaining = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateRequestsRemaining"))[0]);
                    var RateLimitSeconds = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateLimitSeconds"))[0]);
                    var remainingSeconds = (DateTime.Now - RatePeriodEnd).TotalSeconds * -1;
                    Logger.Info($"{apiKey} : {RateRequestRemaining}/{MaxRequestCount} requests remaining for the next {remainingSeconds} seconds. Key expires on: {AuthTokenExpiration}");
                    result[0] = MaxRequestCount.ToString();
                    result[1] = AuthTokenExpiration.ToString("dd/MM/yyyy HH:mm:ss");
                
            } catch (Exception) {
                result[1] = response.StatusCode.ToString();
            }

            return result;
        }
        #endregion
    }
}
