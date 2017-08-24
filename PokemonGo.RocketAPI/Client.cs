using System;
using System.Net;
using System.Threading;
using POGOProtos.Enums;
using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Hash;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using PokemonGo.RocketAPI.Encryption;

namespace PokemonGo.RocketAPI
{
    public class Client : ICaptchaResponseHandler
    {
        public event EventHandler<EventArgs> EvMakeTutorial;
        public Rpc.Login Login;
        public IHasher Hasher;
        public ICrypt Crypter;
        public Rpc.Player Player;
        public Rpc.Download Download;
        public Rpc.Inventory Inventory;
        public Rpc.Map Map;
        public Rpc.Fort Fort;
        public Rpc.Encounter Encounter;
        public Rpc.Misc Misc;
        public Rpc.Store Store;

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
        public bool ReadyToUse { get; set; }

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

        public Client(Shared.ClientSettings settings)
        {
            ReadyToUse = false;
            AuthType = settings.userType;
            Username = settings.userName;
            Password = settings.password;
            proxy = InitProxy(settings.proxyUrl,settings.proxyPort,settings.proxyUser,settings.proxyPass);
            PokemonHttpClient = new PokemonHttpClient();
            Login = new Rpc.Login(this);
            Player = new Rpc.Player(this);
            Download = new Rpc.Download(this);
            Inventory = new Rpc.Inventory(this);
            Map = new Rpc.Map(this);
            Fort = new Rpc.Fort(this);
            Encounter = new Rpc.Encounter(this);
            Misc = new Rpc.Misc(this);
            Crypter = new Cipher();
            Hasher = new PokeHashHasher(settings.hashKey);
            Store = new PokemonGo.RocketAPI.Rpc.Store(this);
            
            if (DeviceSetup.SelectedDevice.OSType == "iOS")
                Platform = POGOProtos.Enums.Platform.Ios;
            else
                Platform = POGOProtos.Enums.Platform.Android;
            
            Logger.Info("Platform:" +Platform);


            InventoryLastUpdateTimestamp = 0;

            AppVersion = Resources.Api.AndroidClientVersionInt;
            SettingsHash = "";

            CurrentApiEmulationVersion = settings.currentApi;
        }
        
        private WebProxy InitProxy(string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            if ((proxyHost == "") || (proxyPort ==0))
                return null;
            try
            {
                var proxyUri = $"http://{proxyHost}:{proxyPort}";
                Logger.Info("proxyUri: "+proxyUri);
                Logger.Info("proxyUsername: "+proxyUsername);
                Logger.Info("proxyPassword: "+(proxyPassword != ""));
                var p = new WebProxy(new System.Uri(proxyUri), false, null);

                if (proxyUsername!="")
                    p.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                return p;
            } catch (Exception ex)
            {
                Logger.Error("Something in your Proxy Settings is wrong, or the Proxy is down! Exiting in 5 seconds.");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Exception in Client.cs - InitProxy: " + ex.Message);
                Thread.Sleep(5000);
                Environment.Exit(0);
                return null;
            }
        }

        public bool CheckCurrentVersionOutdated()
        {
            return CurrentApiEmulationVersion < MinimumClientVersion;
        }

        public void SetAuthTicket(AuthTicket authTicket)
        {
            AuthTicket = authTicket;
            Logger.Debug("===================> Received AuthTicket: " + AuthTicket + " Expire: " + Utils.TimeMStoString((long)AuthTicket.ExpireTimestampMs, @"mm\:ss"));
        }

        internal void OnMakeTutorial()
        {
           EvMakeTutorial?.Invoke(this, EventArgs.Empty);
        }

        public int PageOffset { get; set; }

    }
}