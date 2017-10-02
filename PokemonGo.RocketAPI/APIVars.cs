using System;
using System.Threading;

namespace PokemonGo.RocketAPI
{
    public class APIVars
    {
        public long IOSUnknown25 { get; set; }
        public string AndroidClientVersion { get; set; }
        public string IOSClientVersion { get; set; }
        public string UnknownPtr8Message { get; set; }
        public string EndPoint { get; set; }
        public uint AndroidClientVersionInt { get; set; }

        public ApiKeyHashServerInfo HashServerInfo { get; set; }

        public enum HashServerList
        {
            BOSSLAND = 1,
            GOMANAGER = 2
        }

        public class ApiKeyHashServerInfo
        {
            public HashServerList ID;
            public Uri URL;
        }

        public HashServerList SetAPIKeyHashServerURL (string ServerKey)
        {
            HashServerInfo = new ApiKeyHashServerInfo();

            if (ServerKey.Length == 0)
            {
                Logger.Error("No API HASH KEY defined, review setup. Press a key to Exit.");
                Thread.Sleep(TimeSpan.FromSeconds(5));
                Environment.Exit(-1);
            }

            if (!ServerKey.StartsWith("PH"))
            {
                HashServerInfo.ID = HashServerList.BOSSLAND;
                HashServerInfo.URL = new Uri("https://pokehash.buddyauth.com/");
            }
            else
            {
                HashServerInfo.ID = HashServerList.GOMANAGER;
                HashServerInfo.URL = new Uri("http://hash.goman.io/");
            }

            return HashServerInfo.ID;
        }

        public APIVars(long uk25, string clientversion, uint clientversionInt, string ios_clientversion, string endPoint, string initialPTR8)
        {
            IOSUnknown25 = uk25;
            AndroidClientVersion = clientversion;
            AndroidClientVersionInt = clientversionInt;
            IOSClientVersion = ios_clientversion;
            EndPoint = endPoint;
            UnknownPtr8Message = initialPTR8;
        }
    }
}
