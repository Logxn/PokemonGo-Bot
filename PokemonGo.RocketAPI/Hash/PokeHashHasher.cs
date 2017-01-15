using Newtonsoft.Json;
using PokemonGo.RocketAPI.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Helpers;

namespace PokemonGo.RocketAPI.Hash
{
    public class PokeHashHasher : IHasher
    {
        public long Client_Unknown25 => -8832040574896607694;
        private string apiKey;
        public PokeHashHasher(string apiKey)
        {
            this.apiKey = apiKey;
        }
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
                RandomHelper.RandomSleep(1000,1100);
            } while (retry > 0);

            throw new HasherException("Pokefamer Hash API server might down");

        }
        private  async Task<HashResponseContent> InternalRequestHashesAsync(HashRequestContent request)
        {
            // This value will determine which version of hashing you receive.
            // Currently supported versions:
            // v119 -> Pogo iOS 1.19
            // v121 -> Pogo iOS 1.21
            // v121_2 => IOS 1.22
            const string endpoint = "api/v121_2/hash";


            using (var client = new System.Net.Http.HttpClient())
            {

                client.BaseAddress = new Uri("http://pokehash.buddyauth.com/");


                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // GIVE US YOUR FUCKING MONEY
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                // TODO: Fix this up with proper retry-after when we get rate limited.
                switch (response.StatusCode)
                {           
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<HashResponseContent>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    #region ErrorCodes
                    case HttpStatusCode.BadRequest: // Invalid request
                        string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (responseText.Contains("Unauthorized")) throw new HasherException($"[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again! (Pokefamer message : {responseText})");
                        Console.WriteLine($"[HashService] Bad request sent to the hashing server! {responseText}");
                        break;
                    case HttpStatusCode.Unauthorized: // No Valid Key
                        throw new  HasherException("[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again!");
                    case (HttpStatusCode)429: // To many reqeusts => que 
                        Console.WriteLine($"[HashService] Your request has been limited. {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
                        RandomHelper.RandomSleep(2000,2100);  //stop for 2 sec (WHY THE FUCK U USE 2*100 INSTEAD OF 2000 ?!?!?!?!?!)
                        return await RequestHashesAsync(request).ConfigureAwait(false);
                    default:
                        throw new HasherException($"[HashService] Pokefamer Hash API ({client.BaseAddress}{endpoint}) might down!");
                        #endregion
                }
            }

            return null;
        }

        public HashResponseContent RequestHashes(HashRequestContent request)
        {
            int retry = 3;
            do {
                try
                {
                    return InternalRequestHashes(request);
                }
                catch (HasherException hashEx)
                {
                    throw hashEx;
                }
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: PokeHashHasher.cs - RequestHashes()");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                }
                finally
                {
                    retry--;
                }
                RandomHelper.RandomSleep(1000,1100);
            } while (retry > 0);

            throw new HasherException("Pokefamer Hash API server might down");

        }
        private   HashResponseContent InternalRequestHashes(HashRequestContent request)
        {
            const string endpoint = "api/v121_2/hash";
            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri("http://pokehash.buddyauth.com/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.PostAsync(endpoint, content).Result;
                switch (response.StatusCode)
                {           
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<HashResponseContent>(response.Content.ReadAsStringAsync().Result);
                    case HttpStatusCode.BadRequest: // Invalid request
                        string responseText = response.Content.ReadAsStringAsync().Result;
                        if (responseText.Contains("Unauthorized")) throw new HasherException($"[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again! (Pokefamer message : {responseText})");
                        Console.WriteLine($"[HashService] Bad request sent to the hashing server! {responseText}");
                        break;
                    case HttpStatusCode.Unauthorized: // No Valid Key
                        throw new  HasherException("[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again!");
                    case (HttpStatusCode)429: // To many reqeusts => que 
                        Console.WriteLine($"[HashService] Your request has been limited. {response.Content.ReadAsStringAsync().Result}");
                        RandomHelper.RandomSleep(2000,2100);
                        return RequestHashesAsync(request).Result;
                    default:
                        throw new HasherException($"[HashService] Pokefamer Hash API ({client.BaseAddress}{endpoint}) might down!");
                }
            }
            return null;
        }

    }
}
