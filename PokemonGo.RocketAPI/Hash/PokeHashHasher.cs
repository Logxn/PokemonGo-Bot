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
                    return await InternalRequestHashesAsync(request);
                }
                catch (HasherException hashEx)
                {
                    throw hashEx;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    retry--;
                }
                await Task.Delay(1000);
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

                // Being Polite :)
                client.DefaultRequestHeaders.Add("X-AuthToken", this.apiKey);

                // Changed from ASCII to UTF8
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(endpoint, content);

                // TODO: Fix this up with proper retry-after when we get rate limited.
                // TODO: Get limits and show to Console
                switch (response.StatusCode)
                {           
                    case HttpStatusCode.OK:
                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Hasher Server " + response.Headers.GetValues("X-RateRequestsRemaining").FirstOrDefault() + " rpm of " + response.Headers.GetValues("X-MaxRequestCount").FirstOrDefault(), LogLevel.Debug);
                        return JsonConvert.DeserializeObject<HashResponseContent>(await response.Content.ReadAsStringAsync());
                    #region ErrorCodes
                    case HttpStatusCode.BadRequest: // Invalid request
                        string responseText = await response.Content.ReadAsStringAsync();
                        if (responseText.Contains("Unauthorized")) throw new HasherException($"[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again! (Pokefamer message : {responseText})");
                        Console.WriteLine($"[HashService] Bad request sent to the hashing server! {responseText}");
                        break;
                    case HttpStatusCode.Unauthorized: // No Valid Key
                        throw new  HasherException("[HashService] Your PF-Hashkey you provided is incorrect (or not valid anymore). Please check again!");
                    case (HttpStatusCode)429: // To many requests
                        Console.WriteLine($"[HashService] Your request has been limited. {await response.Content.ReadAsStringAsync()}");
                        await Task.Delay(2000);  //stop for 2 sec
                        return await RequestHashesAsync(request);
                    default:
                        throw new HasherException($"[HashService] Pokefamer Hash API ({client.BaseAddress}{endpoint}) might down!");
                    #endregion
                }

            }
            return null;
        }
    }
}
