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
        public Dictionary<string, string> EndPointDictionary = new Dictionary<string, string>
            {
                {"1.19", "api/v119/hash"},
                {"1.21", "api/v121/hash"},
                {"1.21.2", "api/v121_2/hash"},
                {"0.51.0", "api/v121_2/hash"},
                {"0.53.0", "api/v123_1/hash"},
                {"1.23.1", "api/v123_1/hash"},
                {"0.53.1", "api/v123_1/hash" },
                {"1.23.2", "api/v123_1/hash"},
                {"0.53.2", "api/v123_1/hash" },
                {"0.55.0", "api/v125_1/hash" }, // not released yet at 29/01/2017
            };

            int MaxRequestCount;            // RPM Value
            DateTime RatePeriodEnd;              // End of running minute
            int RateRequestRemaining;       // Unused Requests this minute
            int RateLimitSeconds;           // Rate Limit Period (always 60)
            DateTime AuthTokenExpiration;   // Expiration

        private Uri _baseAddress = new Uri("http://pokehash.buddyauth.com/");
        private Uri _availableHashVersionsCheck = new Uri("https://pokehash.buddyauth.com/api/hash/versions");
        private string _endpoint;
        private string apiKey;

        public PokeHashHasher(string apiKey)
        {
            //_endpoint = EndPointDictionary[(CurrentAPIVersion.CurrentNianticAPIVersion).ToString()];
            _endpoint = EndPointDictionary[(Resources.BotApiSupportedVersion).ToString()];
            this.apiKey = apiKey;
        }

        #region Sync Methods
        public HashResponseContent RequestHashes(HashRequestContent request)
        {
            int retry = 3;
            int cyclingRetrys = 40;
            bool changeKey ;
            do {
                changeKey = false;
                try
                {
                    return InternalRequestHashes(request);
                }
                catch (HasherException hashEx)
                {
                    changeKey = true;
                    cyclingRetrys --;
                    Logger.Write(hashEx.Message);
                    if (cyclingRetrys < 0)
                        throw hashEx;
                }
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: PokeHashHasher.cs - RequestHashes()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                }
                if (changeKey){
                    var nextKey = Shared.KeyCollection.nextKey();
                    if (nextKey !=""){
                           this.apiKey = nextKey;
                           Logger.Debug("Changing KEY to: "+this.apiKey.Substring(0,5));
                    }
                    RandomHelper.RandomSleep(250,300);
                }
                else{
                    RandomHelper.RandomSleep(1000,1100);
                    retry--;
                }
            } while (retry > 0);

            throw new HasherException("[HasherExeption] Default: pokefamer Hash API server might down");
        }

        private HashResponseContent InternalRequestHashes(HashRequestContent request)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = _baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = client.PostAsync(_endpoint, content).Result;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK: //200
                        AuthTokenExpiration = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-AuthTokenExpiration"))[0])).ToLocalTime();
                        MaxRequestCount = Convert.ToUInt16(((string[])response.Headers.GetValues("X-MaxRequestCount"))[0]);
                        RatePeriodEnd = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddSeconds(Convert.ToUInt32(((String[])response.Headers.GetValues("X-RatePeriodEnd"))[0])).ToLocalTime();
                        RateRequestRemaining = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateRequestsRemaining"))[0]);
                        RateLimitSeconds = Convert.ToUInt16(((string[])response.Headers.GetValues("X-RateLimitSeconds"))[0]);
                        var remainingSeconds = (DateTime.Now - RatePeriodEnd).TotalSeconds * -1;

                        Logger.Debug($"{RateRequestRemaining}/{MaxRequestCount} requests remaining for the next {remainingSeconds} seconds. Key expires on: {AuthTokenExpiration}");
                        return JsonConvert.DeserializeObject<HashResponseContent>(response.Content.ReadAsStringAsync().Result);

                    case HttpStatusCode.BadRequest: // 400
                        var responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"[HashService] 400: Bad request sent to the hashing server! {responseText}");

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
                        RandomHelper.RandomSleep(10000,11000);
                        throw new HasherException($"[HashService] Unknown: Pokefamer Hash API ({_baseAddress}{_endpoint}) might down!");
                }
            }
        }
        #endregion
    }
}
