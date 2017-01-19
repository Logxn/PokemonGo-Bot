using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Hash;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using PokemonGo.RocketAPI.Login;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;
using System.Threading;
using POGOProtos.Enums;

namespace PokemonGo.RocketAPI
{
    public class Client : ICaptchaResponseHandler
    {
        public Rpc.Login Login;
        public IHasher Hasher;
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;

        public IApiFailureStrategy ApiFailure { get; set; }
        public string AuthToken { get; set; }
        string CaptchaToken;

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public double CurrentAltitude { get; set; }

        public AuthType AuthType;
        public string Username;
        public string Password;

        public string ApiUrl { get; set; }
        internal readonly PokemonHttpClient PokemonHttpClient;
        internal AuthTicket AuthTicket { get; set; }

        public long StartTime { get; set; }

        internal string SettingsHash { get; set; }
        internal long InventoryLastUpdateTimestamp { get; set; }
        internal Platform Platform { get; set; }
        internal uint AppVersion { get; set; }

        public Version CurrentApiEmulationVersion { get; set; }
        public Version MinimumClientVersion { get; set; }


        public static WebProxy proxy;
        public bool readyToUse = false;

        public double CurrentAccuracy { get; internal set; }
        public float CurrentSpeed { get; internal set; }

        public bool ShowingStats { get; set; }
        public bool LoadingPokemons { get; set; }

    public void setFailure(IApiFailureStrategy fail)
        {
            ApiFailure = fail;
        }

        public void SetCaptchaToken(string token)
        {
            CaptchaToken = token;
        }

        public Client(string hashKey, double latitude , double longitude, double altitude,
                      string proxyUrl, int proxyPort , string proxyUser,string proxyPass,
                      AuthType userType, string userName, string password
                     )
        {
            AuthType = userType;
            Username = userName;
            Password = password;
            proxy = InitProxy(proxyUrl,proxyPort,proxyUser,proxyPass);
            PokemonHttpClient = new PokemonHttpClient();
            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);
            Hasher = new PokeHashHasher(hashKey);

            Player.SetCoordinates(latitude, longitude, altitude);

            InventoryLastUpdateTimestamp = 0;

            AppVersion = 5120;
            SettingsHash = "";

            CurrentApiEmulationVersion = Settings.BotApiSupportedVersion;// new Version("0.51.2");
        }
        
        private WebProxy InitProxy(string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            if ((proxyHost == "") || (proxyPort ==0))
                return null;
            try
            {
                WebProxy p = new WebProxy(new System.Uri($"http://{proxyHost}:{proxyPort}"), false, null);

                if (proxyUsername!="")
                    p.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                return p;
            } catch (Exception)
            {
                Logger.Error("Something in your Proxy Settings is wrong, or the Proxy is down! Exiting in 5 seconds.");
                Thread.Sleep(5000);
                Environment.Exit(0);
                return null;
            }
        }

        public bool CheckCurrentVersionOutdated()
        {
            return CurrentApiEmulationVersion < MinimumClientVersion;
        }

    }
}