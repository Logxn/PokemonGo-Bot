using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using POGOProtos.Enums;
using PokemonGo.RocketApi.PokeMap.DataModel;
using RestSharp;

namespace PokemonGo.RocketApi.PokeMap
{
    public class DataCollector
    {
        public static string FastPokeMapApiUrl = "https://api.fastpokemap.se/";
        public static string FastPokeMapCacheUrl = "https://cache.fastpokemap.se/";
        public static string HeaderAccept = "application/json, text/javascript, */*; q=0.01";
        public static string HeaderAcceptEncoding = "Accept-Encoding: gzip, deflate, sdch, br";
        public static string HeaderAuthority = "api.fastpokemap.se";
        public static string HeaderOriginUrl = "https://fastpokemap.se";
        public static string HeaderUserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.0 Safari/537.36";
        public static int MaxRetryCount = 10;

        public static string ParameterKey = "allow-all";
        public static string ParameterTs = "0";
        public static string ResultOverLoad = "{\"error\":\"overload\"}";
        public static int TimeoutBetweenRetries = 1000;

        private static RestClient mRestClient;

        /// <summary>
        /// 
        /// </summary>
        public enum PokemonMapDataType
        {
            Cached = 0,
            Nearby = 1,
        }

        public static async Task<List<PokemonMapData>> GetFastPokeMapData(double latitude, double longitude)
        {
            var res = await getCacheData(latitude.ToString(), longitude.ToString());
            var res2 = await getNearbyData(latitude.ToString(), longitude.ToString());

            var retVal = createPokemonMapData(res, res2);
           
            return retVal;
        }
 
        private static RestClient createClient(string url)
        {
            RestSharp.RestClient client = new RestSharp.RestClient(url);
            mRestClient = client;
            return client;
        }
        
        /// <summary>
        /// Creates the pokemon map data.
        /// </summary>
        /// <param name="cached">The cached.</param>
        /// <param name="nearbys">The nearbys.</param>
        /// <returns></returns>
        private static List<PokemonMapData> createPokemonMapData(List<ResultCache> cached, List<ResultNearby> nearbys)
        {
            List <PokemonMapData> retVal = null;
            if (cached != null && cached.Count > 0)
            {
                if (retVal == null)
                {
                    retVal = new List<PokemonMapData>();
                }
                foreach (var cache in cached)
                {
                    retVal.Add(new PokemonMapData()
                    {
                        Id = cache.Id,
                        Type = PokemonMapDataType.Cached,
                        PokemonId = (PokemonId)Enum.Parse(typeof(PokemonId), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cache.PokemonId.ToLower().Replace("_", " ")).Replace(" ", "")),
                        Coordinates = cache.LatitudeLongitude,
                        ExpiresAt = DateTime.Parse(cache.ExpireAt),
                    });
                }
            }
            if (nearbys != null && nearbys.Count > 0)
            {
                if (retVal == null)
                {
                    retVal = new List<PokemonMapData>();
                }
                foreach (var nearby in nearbys.Where(x => x.PokemonId != null))
                {
                    
                    retVal.Add(new PokemonMapData()
                    {
                        Id = nearby.Id,
                        Type = PokemonMapDataType.Nearby,
                        PokemonId = (PokemonId)Enum.Parse(typeof(PokemonId), CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nearby.PokemonId.ToLower().Replace("_", " ")).Replace(" ", "")),
                        Coordinates = new LatitudeLongitude() { Coordinates = new List<double>()
                        {
                            nearby.Latitude,
                            nearby.Longitude
                        } },
                        ExpiresAt = null,
                    });
                }
            }
            return retVal;
        }

        private static RestRequest createRequest(string latitude, string longitude)
        {
            //curl "https://cache.fastpokemap.se/?key=allow-all&ts=0&lat=40.7686142466727&lng=-73.98459434509279" 
            //-H "Accept: application/json, text/javascript, */*; q=0.01" 
            //-H "Connection: keep-alive" 
            //-H "Origin: https://fastpokemap.se" 
            //-H "Accept-Encoding: gzip, deflate, sdch, br" 
            //-H "Accept-Language: de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4" 
            //-H "User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.0 Safari/537.36" --compressed
            var req = new RestSharp.RestRequest("?key={key}&ts={ts}&lat={lat}&lng={lng}", Method.GET);
            req.AddHeader("Accept", HeaderAccept);
            req.AddHeader("Origin", HeaderOriginUrl);
            req.AddHeader("Accept-Encoding", HeaderAcceptEncoding);
            req.AddHeader("Accept-Language", "de-DE,de;q=0.8,en-US;q=0.6,en;q=0.4");
            req.AddHeader("User-Agent", HeaderUserAgent);
            req.AddHeader("authority", HeaderAuthority);

            latitude = latitude.Replace(",", ".");
            longitude = longitude.Replace(",", ".");
            
            req.AddUrlSegment("key", ParameterKey);
            req.AddUrlSegment("ts", ParameterTs);
            req.AddUrlSegment("lat", latitude);
            req.AddUrlSegment("lng", longitude);

            req.JsonSerializer.ContentType = "application/json; charset=utf-8";

            return req;
        }
 
        private static async Task<T> executeRequest<T>(RestClient client, RestRequest request) where T : class, new()
        {
            T returnValue = default(T);
            Stopwatch startExecution = null;
            startExecution = Stopwatch.StartNew();

            int counter = 0;

            do
            {
                Debug.WriteLine("Starting try #{0}", counter);
                var response = await client.ExecuteTaskAsync(request);
            
                startExecution.Stop();                                            
                Debug.WriteLine("RestRequest execution time: {0}, {1}ms, {2} ticks", startExecution.Elapsed, startExecution.ElapsedMilliseconds, startExecution.ElapsedTicks);
            
                if (response.ErrorException != null)
                {
                    throw response.ErrorException;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response.Content == ResultOverLoad)
                    {
                        Debug.WriteLine("currently fastpokemaps service is overloaded");
                    }
                    else
                    {
                        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(response.Content);
                        returnValue = data;
                        return returnValue;
                    }
                }
                counter++;
                Thread.Sleep(TimeoutBetweenRetries);
            }
            while (returnValue == null && counter < MaxRetryCount);
            return returnValue;
        }

        private static async Task<List<ResultCache>> getCacheData(string latitude, string longitude)
        {
            return await getData<List<ResultCache>>(latitude, longitude, FastPokeMapCacheUrl);
        }

        private static async Task<T> getData<T>(string latitude, string longitude, string url) where T : class, new()
        {
            T returnValue = default(T);
            RestSharp.RestClient client = createClient(url);

            var req = createRequest(latitude, longitude);
            
            Stopwatch startExecution = null;
            startExecution = Stopwatch.StartNew();
            
            return await executeRequest<T>(client, req);
        }

        private static async Task<List<ResultNearby>> getNearbyData(string latitude, string longitude)
        {
            var retVal = await getData<ResultsNearbyList>(latitude, longitude, FastPokeMapApiUrl);
            if (retVal == null)
            {
                return null;
            }
            else
            {
                return retVal.Results;
            }
        }

        public class PokemonMapData
        {
            public PokemonId PokemonId { get; set; }
            public PokemonMapDataType Type { get; set; }
            public LatitudeLongitude Coordinates { get; set; }
            public DateTime? ExpiresAt { get; set; }
            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            public string Id { get; set; }

            public TimeSpan? TimeSpanRemaining
            {
                get
                {
                    if (ExpiresAt != null)
                    {
                        return ExpiresAt - DateTime.Now;
                    }
                    return null;
                }
            }
        }
    }
}