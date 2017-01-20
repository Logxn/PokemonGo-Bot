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
        // This Needs to be Changed for every new version
        // ***************************************************************************
        //public long Client_Unknown25 => -8832040574896607694;
        public long Client_Unknown25 => -76506539888958491;

        // ***************************************************************************
        // This value will determine which version of hashing you receive.
        // Currently supported versions:
        // v119 -> Pogo iOS 1.19
        // v121 -> Pogo iOS 1.21
        // v121_2 => IOS 1.22 (0.51.0/2)
        // v123_1 => IOS 1.23 (0.53.0)
        // ***************************************************************************
        public Dictionary<string, string> EndPointDictionary = new Dictionary<string, string>
            {
                {"1.19", "api/v119/hash"},
                {"1.21", "api/v121/hash"},
                {"1.21.2", "api/v121_2/hash"},
                {"0.51.0", "api/v121_2/hash"},
                {"0.53.0", "api/v123_1/hash" }
            };

        private Uri _baseAddress = new Uri("http://pokehash.buddyauth.com/");
        private string _endpoint;
        private string apiKey;

        public PokeHashHasher(string apiKey)
        {
            _endpoint = EndPointDictionary[(CurrentAPIVersion.CurrentNianticAPIVersion).ToString()];
            this.apiKey = apiKey;
        }

        #region Async Methods
        public async Task<HashResponseContent> RequestHashesAsync(HashRequestContent request)
        {
            int retry = 3;
            do {
                try
                {
                    return await InternalRequestHashesAsync(request).ConfigureAwait(false);
                }
                catch (HasherException hashEx)
                {
                    Logger.Write(hashEx.Message);
                    throw hashEx;
                }
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: PokeHasHasher.cs - RequestHashesAsync()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                }
                finally
                {
                    retry--;
                }
            } while (retry > 0);

            throw new HasherException("Pokefamer Hash API server might down");
        }

        private async Task<HashResponseContent> InternalRequestHashesAsync(HashRequestContent request)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = _baseAddress;
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(_endpoint, content).ConfigureAwait(false);

                switch (response.StatusCode)
                {           
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<HashResponseContent>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                    case HttpStatusCode.BadRequest: // Invalid request
                        string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (responseText.Contains("Unauthorized")) throw new HasherException($"[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again! (Pokefamer message : {responseText})");
                        Console.WriteLine($"[HashService] Bad request sent to the hashing server! {responseText}");
                        break;

                    case HttpStatusCode.Unauthorized: // No Valid Key
                        throw new  HasherException("[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again!");

                    case (HttpStatusCode)429: // To many reqeusts => que 
                        Console.WriteLine($"[HashService] Your request has been limited. {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
                        RandomHelper.RandomSleep(2000,2100);
                        return await RequestHashesAsync(request).ConfigureAwait(false);

                    default:
                        throw new HasherException($"[HashService] Pokefamer Hash API ({_baseAddress}{_endpoint}) might down!");
                }
            }

            return null;
        }
        #endregion

        #region Sync Methods
        public HashResponseContent RequestHashes(HashRequestContent request)
        {
            int retry = 3;
            int cyclingRetrys = 40;
            bool doFasterCall ;
            do {
                doFasterCall = false;
                try
                {
                    return InternalRequestHashes(request);
                }
                catch (HasherException hashEx)
                {
                    doFasterCall = true;
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
                if (doFasterCall){
                    var nextKey = Shared.KeyCollection.nextKey();
                    if (nextKey !=""){
                           this.apiKey = nextKey;
                           Logger.Write("Changing KEY to: "+this.apiKey.Substring(0,5));
                    }
                    RandomHelper.RandomSleep(250,300);
                }
                else{
                    RandomHelper.RandomSleep(1000,1100);
                    retry--;                    
                }
            } while (retry > 0);

            throw new HasherException("Pokefamer Hash API server might down");
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
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<HashResponseContent>(response.Content.ReadAsStringAsync().Result);

                    case HttpStatusCode.BadRequest: // Invalid request
                        var responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"[HashService] Bad request sent to the hashing server! {responseText}");

                    case HttpStatusCode.Unauthorized: // No Valid Key
                        Shared.KeyCollection.removeKey(this.apiKey);
                        throw new  HasherException($"[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). ");

                    case (HttpStatusCode)429: // To many reqeusts => que 
                        responseText = response.Content.ReadAsStringAsync().Result;
                        throw new HasherException($"HashService] Your request has been limited (Message : {responseText})");

                    default:
                        RandomHelper.RandomSleep(10000,11000);
                        throw new HasherException($"[HashService] Pokefamer Hash API ({_baseAddress}{_endpoint}) might down!");
                }
            }
        }
        #endregion
    }
}
