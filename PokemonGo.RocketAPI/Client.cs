using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
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
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;

        public IApiFailureStrategy ApiFailure { get; set; }
        public ISettings Settings { get; }
        public string AuthToken { get; set; }
        string CaptchaToken;

        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public double CurrentAltitude { get; set; }

        public AuthType AuthType => Settings.AuthType;

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

        public Client(ISettings settings)
        {
            Settings = settings;
            proxy = InitProxy();
            PokemonHttpClient = new PokemonHttpClient();
            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);

            Player.SetCoordinates(settings.DefaultLatitude, settings.DefaultLongitude, settings.DefaultAltitude);

            InventoryLastUpdateTimestamp = 0;

            /*AppVersion = 4303;
            SettingsHash = "";

            CurrentApiEmulationVersion = new Version("0.43.3");*/

            AppVersion = 4500;
            SettingsHash = "";

            CurrentApiEmulationVersion = new Version("0.45.0");
        }


        private WebProxy InitProxy()
        {
            try
            {
                if (!Settings.UseProxyVerified)
                    return null;

                WebProxy p = new WebProxy(new System.Uri($"http://{Settings.UseProxyHost}:{Settings.UseProxyPort}"), false, null);

                if (Settings.UseProxyAuthentication)
                    p.Credentials = new NetworkCredential(Settings.UseProxyUsername, Settings.UseProxyPassword);
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