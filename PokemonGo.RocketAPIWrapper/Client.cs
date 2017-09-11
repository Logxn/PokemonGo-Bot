using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using POGOLib.Official.Net;
using POGOLib.Official.Util.Hash;
using POGOLib.Official.LoginProviders;
using POGOLib.Official.Net.Authentication;
using System.Linq;
using POGOProtos.Map;
using Google.Protobuf.Collections;
using POGOProtos.Networking.Responses;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Requests;
using Google.Protobuf;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPIWrapper;

namespace PokemonGo.RocketAPIWrapper
{
    public class Client
    {
        public Session session;
        public ISettings _clientSettings;
        public Download Download;
        public Fort Fort;
        public Encounter Encounter;
        public Store Store;
        public PokemonGo.RocketAPIWrapper.Inventory Inventory;
        public Map Map;
        public Player Player;
        public Misc Misc;
        public Login Login;
        public double CurrentLatitude {get{return session.Player.Latitude;}}
        public double CurrentLongitude {get{return session.Player.Longitude;}}
        public double CurrentAltitude {get;set;}
        public bool ReadyToUse = false;
        


        public Client(ISettings clientSettings)
        {
            _clientSettings = clientSettings;
            POGOLib.Official.Configuration.Hasher = new PokeHashHasher(clientSettings.HashKey);
            Download = new Download(session);
            Fort = new Fort(session);
            Encounter = new Encounter(session);
            Store = new Store(session);
            Inventory = new Inventory(session);
            Map = new Map(session);
            Player = new Player(session,clientSettings);
            Misc = new Misc(session);
            Login = new Login(this);
        }

        public static long ToUnixTime( DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }

        public static string StripTagsRegexCompiled(string source)
        {
            var _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
            return _htmlRegex.Replace(source, string.Empty);
        }


    }
}