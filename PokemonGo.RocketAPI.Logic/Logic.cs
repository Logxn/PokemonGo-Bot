using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using PokemonGo.RocketApi.PokeMap;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using Telegram.Bot;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Logic;

using PokemonGo.RocketApi.PokeMap.DataModel;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using POGOProtos.Map.Pokemon;

namespace PokemonGo.RocketAPI.Logic
{
    public class Logic
    {

        #region Members and Constructor

        public static Client objClient;
        public readonly ISettings ClientSettings;
        public TelegramUtil Telegram;
        public BotStats BotStats;
        private readonly Navigation navigation;
        public const double SpeedDownTo = 10 / 3.6;
        private readonly LogicInfoObservable infoObservable;
        private readonly PokeVisionUtil pokevision;
        private int pokemonCatchCount;
        private int pokeStopFarmedCount;
        private double timetorunstamp = -10000;
        private double pausetimestamp = -10000;
        private double resumetimestamp = -10000;
        private double lastlog = -10000;
        private double startingXp = -10000;
        private double currentxp = -10000;
        private bool havelures;
        private bool pokeballoutofstock;
        private bool stopsloaded;
        public static Logic Instance;
        private bool pauseWalking;
        private readonly List<string> lureEncounters = new List<string>();
        private int count;
        public static int FailedSoftban;
        private int level = -1;
        public List<ulong> SkippedPokemon = new List<ulong>();
        public double lastsearchtimestamp;

        #region Pause Walking Function

        public bool PauseWalking
        {
            get
            {
                if (navigation != null)
                {
                    pauseWalking = navigation.pauseWalking;
                }
                return pauseWalking;
            }
            set
            {
                if (navigation == null) return;

                navigation.pauseWalking = value;
                pauseWalking = value;
            }
        }

        #endregion

        public DateTime LastIncenselog;

        #region Snipe Variables
        private readonly PokeSnipers pokeSnipers;
        private bool stateSniper;
        private bool sniperReturn;
        public string Lure = "lureId";
        public PokemonId Luredpokemoncaught = PokemonId.Articuno;
        private PokemonId snipokemonIds;
        private bool addedlure;
        #endregion

        #region Constructor
        public Logic(ISettings clientSettings, LogicInfoObservable infoObservable)
        {
            this.ClientSettings = clientSettings;
            objClient = new Client(ClientSettings);
            objClient.setFailure(new ApiFailureStrat(objClient));
            BotStats = new BotStats();
            navigation = new Navigation(objClient);
            pokevision = new PokeVisionUtil();
            this.infoObservable = infoObservable;
            Instance = this;
            pokeSnipers = new PokeSnipers();
        }
        #endregion

        #endregion

        #region Workflow

        private void FarmPokestopOnBreak(FortData[] pokeStops, Client client)
        {
            //check for overlapping pokestops where we are taking a break
            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Reached break location. Using Lures Enabled");

            var pokestopsWithinRangeStanding = pokeStops
                .Where(i => LocationUtils
                   .CalculateDistanceInMeters(
                       objClient.CurrentLatitude,
                       objClient.CurrentLongitude,
                       i.Latitude,
                       i.Longitude) < 40);

            var pokestopCount = pokestopsWithinRangeStanding.Count();

            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{pokestopCount} Pokestops within range of where you are standing.");

            //Begin farming loop while on break
            do
            {
                foreach (var pokestop in pokestopsWithinRangeStanding)
                {
                    if (ClientSettings.ForceSnipe)
                        break;

                    if (ClientSettings.RelocateDefaultLocation) break;

                    ExecuteCatchAllNearbyPokemons();

                    var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;

                    if ((ClientSettings.UseLureGUIClick && havelures) || (ClientSettings.UseLureAtBreak && havelures && !pokestop.ActiveFortModifier.Any() && !addedlure))
                    {
                        ClientSettings.UseLureGUIClick = false;

                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Adding lure and setting resume walking to 30 minutes");

                        objClient.Fort.AddFortModifier(fortInfo.FortId, ItemId.ItemTroyDisk).Wait();

                        resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 30000;
                        addedlure = true;
                    }

                    var farmed = CheckAndFarmNearbyPokeStop(pokestop, objClient, fortInfo);
                    if (farmed)
                    {
                        pokestop.CooldownCompleteTimestampMs = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500;
                    }

                    SetCheckTimeToRun();

                    RandomHelper.RandomSleep(30000, 40000);

                    // wait for a bit before repeating farm cycle to avoid spamming 
                }

                if (!ClientSettings.ForceSnipe && !ClientSettings.RelocateDefaultLocation) continue;

                resumetimestamp = -10000;
                ClientSettings.pauseAtPokeStop = false;

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Exit Command detected - Ending break");
            } while (ClientSettings.pauseAtPokeStop);
        }

        private int GetRandomWalkspeed()
        {
            var walkspeed = (int)ClientSettings.WalkingSpeedInKilometerPerHour;
            if (!ClientSettings.RandomReduceSpeed) return walkspeed;

            var randomWalkSpeed = new Random();
            if ((int)ClientSettings.WalkingSpeedInKilometerPerHour - ClientSettings.MinWalkSpeed > 1)
            {
                walkspeed = randomWalkSpeed.Next(ClientSettings.MinWalkSpeed,
                    (int)ClientSettings.WalkingSpeedInKilometerPerHour);
            }
            return walkspeed;
        }

        #region Execute Functions

        public void Execute()
        {
            // Check if disabled
            StringUtils.CheckKillSwitch();

            Logger.ColoredConsoleWrite(ConsoleColor.Red, "This bot is absolutely free and open-source!");
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "If you've paid for it. Request a chargeback immediately!");

            #region Log Logger

            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Starting Execute on login server: {ClientSettings.AuthType}", LogLevel.Info);

            if (ClientSettings.logPokemons)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled Pokemonlogging. It will be saved to \"\\Logs\\PokeLog.txt\"");
            }

            if (ClientSettings.logManualTransfer)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled manual transfer logging. It will be saved to \"\\Logs\\TransferLog.txt\"");
            }

            if (ClientSettings.bLogEvolve)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled Evolution Logging. It will be saved to \"\\Logs\\EvolutionLog.txt\"");
            }

            #endregion

            #region Set Counters and Location

            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Setting Pokemon Catch Count: to 0 for this session", LogLevel.Info);

            pokemonCatchCount = 0;

            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Setting Pokestop Farmed Count to 0 for this session", LogLevel.Info);

            pokeStopFarmedCount = 0;

            objClient.CurrentAltitude = ClientSettings.DefaultAltitude;
            objClient.CurrentLongitude = ClientSettings.DefaultLongitude;
            objClient.CurrentLatitude = ClientSettings.DefaultLatitude;

            #endregion

            #region Fix Altitude

            if (Math.Abs(objClient.CurrentAltitude) <= 0)
            {
                objClient.CurrentAltitude = LocationUtils.getAltidude(objClient.CurrentLatitude, objClient.CurrentLongitude);
                ClientSettings.DefaultAltitude = objClient.CurrentAltitude;

                Logger.Error($"Altidude was 0, resolved that. New Altidude is now: {objClient.CurrentAltitude}");
            }

            #endregion

            #region Use Proxy

            if (ClientSettings.UseProxyVerified)
            {
                Logger.Error("===============================================");
                Logger.Error("Proxy enabled.");
                Logger.Error($"ProxyIP: { ClientSettings.UseProxyHost }:{ClientSettings.UseProxyPort}");
                Logger.Error("===============================================");
            }

            #endregion

            #region Restart
            //Restart unless killswitch thrown
            while (true)
            {
                try
                {
                    objClient.Login.DoLogin().Wait();

                    #region Instantiate Telegram

                    if (!string.IsNullOrEmpty(ClientSettings.TelegramAPIToken) && !string.IsNullOrEmpty(ClientSettings.TelegramName))
                    {
                        try
                        {
                            Telegram = new TelegramUtil(
                                objClient,
                                new TelegramBotClient(ClientSettings.TelegramAPIToken),
                                ClientSettings,
                                objClient.Inventory);

                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "To activate informations with Telegram, write the bot a message for more informations");

                            var me = Telegram.getClient().GetMeAsync().Result;
                            Telegram.getClient().OnCallbackQuery += Telegram.BotOnCallbackQueryReceived;
                            Telegram.getClient().OnMessage += Telegram.BotOnMessageReceived;
                            Telegram.getClient().OnMessageEdited += Telegram.BotOnMessageReceived;

                            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Telegram Name: {me.Username}");

                            Telegram.getClient().StartReceiving();
                        }
                        catch (Exception)
                        {
                            //TODO: Handle the exception; log then throw error
                        }
                    }

                    #endregion

                    PostLoginExecute();
                }
                catch (Exception ex)
                {
                    #region Log Error 

                    Logger.Error("Error: " + ex.Source);
                    Logger.Error($"{ex}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Trying to Restart.");

                    try
                    {
                        Telegram?.getClient().StopReceiving();
                    }
                    catch (Exception)
                    {
                        //TODO: Handle the exception; log then throw error
                    }

                    #endregion
                }
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Restarting in over 50 - 60 Seconds.");
                RandomHelper.RandomSleep(50000,60000);
            }
            #endregion
        }

        public void PostLoginExecute()
        {
            try
            {
                var profil = objClient.Player.GetPlayer().Result;
                objClient.Inventory.ExportPokemonToCSV(profil.PlayerData).Wait();
                LogStatsEtc();
                ExecuteFarmingPokestopsAndPokemons(objClient);
            }
            catch (AccessTokenExpiredException)
            {
                throw new AccessTokenExpiredException();
            }
            catch (Exception ex)
            {
                Logger.Write($"Exception: {ex}", LogLevel.Error);

                if (ClientSettings.ForceSnipe)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Starting Snipe Routine:");
                }
                else if (ClientSettings.RelocateDefaultLocation)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Detected User Request to Relocate to a new farming spot!");
                }
            }
        }

        #endregion

        #region Update Functions

        public static Version GetNewestVersion()
        {
            try
            {
                var match = DownloadServerVersion();
                var gitVersion = new Version(match);

                return gitVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
            {
                return wC.DownloadString("https://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/ver.md");
            }
        }

        #endregion

        #region Stats log and Session Check Functions

        private void StatsLog(Client client)
        {
            //Enable Pokemon List cause everything is loaded
            objClient.readyToUse = true;

            // Check if disabled
            StringUtils.CheckKillSwitch();

            #region Set Stat Variables

            var profile = client.Player.GetPlayer().Result;
            var inventory = client.Inventory.GetInventory().Result;
            var playerStats = client.Inventory.GetPlayerStats(inventory);  // For dont repeat inventory request
            var stats = playerStats.First();
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            if (startingXp == -10000) startingXp = stats.Experience;

            currentxp = stats.Experience;

            var pokemonToEvolve = (client.Inventory.GetPokemonToEvolve().Result).Count();
            var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(150) * 100;
            var pokedexpercent = Math.Floor(pokedexpercentraw);

            if (curexp == 0 && expneeded == 1000)
            {
                client.Misc.MarkTutorialComplete().Wait();
            }

            var items = client.Inventory.GetItems(inventory); // For dont repeat inventory request
            var pokemonCount = client.Inventory.getPokemonCount().Result;
            var eggCount = client.Inventory.GetEggsCount(inventory);  // For dont repeat inventory request
            var maxPokemonStorage = profile.PlayerData.MaxPokemonStorage;
            var maxItemStorage = profile.PlayerData.MaxItemStorage;
            var stardust = profile.PlayerData.Currencies.ToArray()[1].Amount.ToString("N0");
            var currEXP = curexp.ToString("N0");
            var neededEXP = expneeded.ToString("N0");
            var expPercent = Math.Round(curexppercent, 2);
            #endregion

            #region Log Stats
            client.ShowingStats = true;
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "-----------------------[PLAYER STATS]-----------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Level/EXP: {stats.Level} | {currEXP}/{neededEXP} ({expPercent}%)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"EXP to Level up: {(stats.NextLevelXp - stats.Experience)}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"PokeStops visited: {stats.PokeStopVisits}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"KM Walked: {Math.Round(stats.KmWalked, 2)}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokemon: {pokemonCount}/{maxPokemonStorage} ({pokemonToEvolve} Evolvable)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Eggs: {eggCount}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokedex Completion: {stats.UniquePokedexEntries}/150 [{pokedexpercent}%]");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Stardust: {stardust}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokemon Catch Count this session: {pokemonCatchCount}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"PokeStop Farmed Count this session: {pokeStopFarmedCount}");

            var totalitems = 0;
            foreach (var item in items)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"{item.ItemId} Qty: {item.Count}");

                totalitems += item.Count;
                if (item.ItemId == ItemId.ItemTroyDisk && item.Count > 0)
                {
                    havelures = true;
                }
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Items: {totalitems}/{maxItemStorage} ");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");

            #endregion

            #region Check for Level Up

            if (level == -1)
            {
                level = stats.Level;
            }
            else if (stats.Level > level)
            {
                level = stats.Level;

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Got the level up reward from your level up.");

                var lvlup = client.Player.GetLevelUpRewards(stats.Level).Result;
                var alreadygot = new List<ItemId>();

                foreach (var i in lvlup.ItemsAwarded)
                {
                    if (alreadygot.Contains(i.ItemId)) continue;

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Got Item: {i.ItemId} ({i.ItemCount}x)");
                    alreadygot.Add(i.ItemId);
                }
                alreadygot.Clear();
            }

            #endregion

            #region Set Console Title
            if (!ClientSettings.EnableConsoleInTab)
            {
                System.Console.Title = profile.PlayerData.Username + @" lvl" + stats.Level + @"-(" +
                            (stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"/" +
                            (stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"|" +
                            Math.Round(curexppercent, 2) + @"%)| Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount + @"| " +
                            BotStats;
            }
            #endregion

            #region Check for Update

            if (ClientSettings.CheckWhileRunning)
            {
                if (GetNewestVersion() > Assembly.GetEntryAssembly().GetName().Version)
                {
                    if (ClientSettings.AutoUpdate)
                    {
                        Form update = new Update();
                        update.ShowDialog();
                    }
                    else
                    {
                        var dialogResult = MessageBox.Show(
                            @"There is an Update on Github. do you want to open it ?", $@"Newest Version: {GetNewestVersion()}, MessageBoxButtons.YesNo");

                        switch (dialogResult)
                        {
                            case DialogResult.Yes:
                                Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
                                break;
                            case DialogResult.No:
                                //nothing   
                                break;
                            case DialogResult.None:
                                break;
                            case DialogResult.OK:
                                break;
                            case DialogResult.Cancel:
                                break;
                            case DialogResult.Abort:
                                break;
                            case DialogResult.Retry:
                                break;
                            case DialogResult.Ignore:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }

            #endregion
            client.ShowingStats = false;
        }

        private void LogStatsEtc()
        {
            // reset stat counter
            count = 0;
            if (ClientSettings.UseIncenseGUIClick) UseIncense();

            if (ClientSettings.UseLuckyEggIfNotRunning || ClientSettings.UseLuckyEggGUIClick)
            {
                ClientSettings.UseLuckyEggGUIClick = false;
                objClient.Inventory.UseLuckyEgg(objClient).Wait();
            }

            if (ClientSettings.EvolvePokemonsIfEnoughCandy)
            {
                EvolveAllPokemonWithEnoughCandy();
            }

            if (ClientSettings.AutoIncubate)
            {
                StartIncubation();
            }

            TransferDuplicatePokemon(ClientSettings.keepPokemonsThatCanEvolve, ClientSettings.TransferFirstLowIV);
            RecycleItems();
            StatsLog(objClient);
            SetCheckTimeToRun();
        }

        private void SetCheckTimeToRun()
        {
            
            // Prevent Spamming Logs
            if ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds > lastlog + 60000)
            {
                lastlog = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                #region Time to Run

                if (ClientSettings.TimeToRun > 0)
                {
                    if (timetorunstamp == -10000)
                    {
                        timetorunstamp = ClientSettings.TimeToRun * 60 * 1000 + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    }
                    else
                    {
                        var runTimeRemaining = timetorunstamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        var remainingTime = Math.Round(runTimeRemaining / 1000 / 60, 2);
                        if (runTimeRemaining <= 0)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Time To Run Reached or Exceeded...Walking back to default location and stopping bot");

                            WalkWithRouting(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);

                            StringUtils.CheckKillSwitch(true);
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time to Run: {remainingTime} minutes");
                        }
                    }
                }

                #endregion

                #region Breaks

                if (ClientSettings.UseBreakFields)
                {
                    if (pausetimestamp > -10000)
                    {
                        var walkTimeRemaining = pausetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        if (walkTimeRemaining <= 0)
                        {
                            pausetimestamp = -10000;
                            ClientSettings.pauseAtPokeStop = true;
                            resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + ClientSettings.BreakLength * 60 * 1000;

                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Break Time! Pause walking for {ClientSettings.BreakLength} minutes");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {Math.Round(walkTimeRemaining / 1000 / 60, 2)} minutes");
                        }
                    }
                    else if (resumetimestamp == -10000)
                    {
                        pausetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + ClientSettings.BreakInterval * 60 * 1000;

                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {ClientSettings.BreakInterval} minutes");
                    }
                }

                if (resumetimestamp > -10000)
                {
                    var breakTimeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                    if (breakTimeRemaining <= 0)
                    {
                        resumetimestamp = -10000;
                        ClientSettings.pauseAtPokeStop = false;

                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Break over, back to walking!");
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until resume walking: {Math.Round(breakTimeRemaining / 1000 / 60, 2)} minutes");
                    }
                }

                #endregion

                #region Log Catch Disabled

                //add logging for pokemon catch disabled here for now to prevent spamming
                if (!ClientSettings.CatchPokemon)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Catching Pokemon Disabled in Client Settings - Skipping all pokemon");
                }

                #endregion

                #region Check Run Count Limits

                #region Catch Pokemon Count Check

                if (pokemonCatchCount >= ClientSettings.PokemonCatchLimit)
                {
                    if (ClientSettings.FarmPokestops)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached - Bot will only farm pokestops");

                        ClientSettings.CatchPokemon = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached and not farming pokestops - Bot will return to default location and stop");

                        WalkWithRouting(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);

                        StringUtils.CheckKillSwitch(true);
                    }
                }

                #endregion

                #region Farm Pokestops Check

                if (pokeStopFarmedCount >= ClientSettings.PokestopFarmLimit)
                {
                    if (ClientSettings.CatchPokemon)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached - Bot will only catch pokemon");

                        ClientSettings.FarmPokestops = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached and not catching pokemon - Bot will return to default location and stop");

                        WalkWithRouting(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);

                        StringUtils.CheckKillSwitch(true);
                    }
                }

                #endregion

                #region XP Check

                if (startingXp != -10000 && currentxp != -10000 && (currentxp = -startingXp) >= ClientSettings.XPFarmedLimit)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "XP Farmed Limit Reached - Bot will return to default location and stop");

                    if (ClientSettings.UseGoogleMapsAPI)
                    {
                        WalkWithRouting(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);
                    }
                    else
                    {
                        var walkHome = navigation.HumanLikeWalking(
                            new GeoCoordinate(
                                ClientSettings.DefaultLatitude,
                                ClientSettings.DefaultLongitude),
                            ClientSettings.WalkingSpeedInKilometerPerHour,
                            ExecuteCatchAllNearbyPokemons);
                    }
                    StringUtils.CheckKillSwitch(true);
                }

                #endregion

                #endregion
            }
        }

        #endregion

        #region Catch, Farm and Walk Logic

        #region Snipe Functions

        private bool Snipe(spottedPokeSni p)
        {
            try
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Trying to capture " + p._pokeId + " at " + p._lat + " / " + p._lng);

                var result = objClient.Player.UpdatePlayerLocation(p._lat, p._lng, ClientSettings.DefaultAltitude).Result;

                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Went to sniping location. Waiting for Pokemon to appear...");

                RandomHelper.RandomSleep(ClientSettings.secondsSnipe*1000, ClientSettings.secondsSnipe*1100);

                stateSniper = true;
                sniperReturn = false;

                ExecuteCatchAllNearbyPokemons();

                stateSniper = false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Snipe( PokemonId id, GeoCoordinate coord, int secondsToWait )
        {
            try
            {
                snipokemonIds = id;

                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Trying to capture " + id + " at " + coord.Latitude + " / " + coord.Longitude);

                var result = objClient.Player.UpdatePlayerLocation(coord.Latitude, coord.Longitude, ClientSettings.DefaultAltitude).Result;

                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Went to sniping location. Waiting for Pokemon to appear...");

                RandomHelper.RandomSleep(secondsToWait*1000, secondsToWait*1100);

                stateSniper = true;
                sniperReturn = false;

                ExecuteCatchAllNearbyPokemons();

                stateSniper = false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Archimedean Spiral

        private void Espiral(Client client, FortData[] pokeStops)
        {
            //Intento de pajarera 1...
            ExecuteCatchAllNearbyPokemons();

            Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Starting Archimedean spiral");

            var i2 = 0;
            var salir = true;
            var cantidadvar = 0.0001;
            double recorrido = objClient.Settings.MaxWalkingRadiusInMeters;

            pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude) <= ClientSettings.MaxWalkingRadiusInMeters).ToArray();

            var centerx = objClient.CurrentLatitude;
            var centery = objClient.CurrentLongitude;

            if (recorrido <= 100) cantidadvar = 0.00008;
            if (recorrido > 100 && recorrido <= 500) cantidadvar = 0.00009;
            if (recorrido > 500 && recorrido <= 1000) cantidadvar = 0.0001;
            if (recorrido > 1000) cantidadvar = 0.0002;

            while (salir)
            {
                if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation) break;

                var angle = 0.3 * i2;
                var xx = centerx + cantidadvar * angle * Math.Cos(angle);
                var yy = centery + cantidadvar * angle * Math.Sin(angle);
                var distancia = Navigation.DistanceBetween2Coordinates(centerx, centery, xx, yy);

                if (distancia > recorrido)
                {
                    salir = false;

                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Returning to the starting point...");

                    var update = navigation.HumanLikeWalking(new GeoCoordinate(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude), ClientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);

                    break;
                }

                if (i2 % 10 == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Distance from starting point: " + distancia + " metros...");
                }

                navigation.HumanLikeWalking(
                    new GeoCoordinate(xx, yy),
                    ClientSettings.WalkingSpeedInKilometerPerHour,
                    ExecuteCatchAllNearbyPokemons);

                Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Looking PokeStops who are less than 30 meters...");

                FncPokeStop(client, pokeStops, true);

                i2++;
            }
        }

        #endregion

        private void ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            #region Sniper Logic

            //Sniper
            if (!pokeballoutofstock)
            {
                if (ClientSettings.ManualSnipePokemonID != null && ClientSettings.ManualSnipePokemonLocation != null)
                {
                    var snipesuccess = Snipe((PokemonId)ClientSettings.ManualSnipePokemonID, ClientSettings.ManualSnipePokemonLocation,ClientSettings.secondsSnipe);

                    if (!snipesuccess)
                    {
                        //Leaving this here for now in case we need it for debugging
                    }

                    var result = objClient.Player.UpdatePlayerLocation(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude, ClientSettings.DefaultAltitude).Result;
                }
                else if (ClientSettings.SnipePokemon)
                {
                    foreach (var p in pokeSnipers.CapturarPokemon())
                    {
                        #region Check for region lock

                        var regionlocked = ClientSettings.AvoidRegionLock && (p._pokeId == PokemonId.Farfetchd || p._pokeId == PokemonId.Kangaskhan || p._pokeId == PokemonId.MrMime || p._pokeId == PokemonId.Tauros);

                        #endregion

                        if (!ClientSettings.NotToSnipe.Contains(p._pokeId) && !regionlocked)
                        {
                            snipokemonIds = p._pokeId;

                            var success = Snipe(p);

                            if (!success)
                            {
                                //Leaving this here for now in case we need it for debugging
                            }
                        }
                        else if (regionlocked)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Region Locked Pokemon Encountered - Will not catch");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon in Not to Snipe List - Skipping Pokemon");
                        }
                    }
                    //return to default location before beginning to farm.
                    var result = objClient.Player.UpdatePlayerLocation(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude, ClientSettings.DefaultAltitude).Result;
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon Snipe is Disabled");
                }
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Will resume sniping pokemon after pokeballs restock");
            }

            ClientSettings.ForceSnipe = false;
            ClientSettings.ManualSnipePokemonID = null;
            ClientSettings.ManualSnipePokemonLocation = null;
            ClientSettings.secondsSnipe = 2;

            #endregion

            #region Check and report

            var verifiedLocation = VerifyLocation();            
            var pokeStops = GetNearbyPokeStops();
            var tries = 3;

            do
            {
                // make sure we found pokestops and log if none found
                if (ClientSettings.MaxWalkingRadiusInMeters != 0)
                {
                    if (tries < 3)
                    {
                        RandomHelper.RandomSleep(5000, 6000);
                        pokeStops = GetNearbyPokeStops();
                    }

                    pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude, i.Latitude, i.Longitude) <= ClientSettings.MaxWalkingRadiusInMeters).ToArray();

                    if (!pokeStops.Any())
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops in a range of " + ClientSettings.MaxWalkingRadiusInMeters + "m!");
                    }
                }

                if (!pokeStops.Any())
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops, which are unused! Probably the server are unstable, or you visted them all. Retrying..");
                    tries--;
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " usable PokeStops near your current location.");
                    tries = 0;
                }
            } while (tries > 0);

            #endregion

            #region Start Walk

            // Walk Spiral if enabled
            if (ClientSettings.Espiral)
            {
                Espiral(client, pokeStops);

                return;
            }

            //Normal Walk and Catch between pokestops
            FncPokeStop(objClient, pokeStops, false);

            #endregion
        }

        private FortData[] GetNearbyPokeStops( bool updateMap = true, GetMapObjectsResponse mapObjectsResponse = null)
        {
            #region Get Pokestops

            //Query nearby objects for mapData
            if (mapObjectsResponse == null)
                mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;

            //narrow map data to pokestops within walking distance
            var pokeStops = navigation
                .pathByNearestNeighbour(
                    mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
                    .OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude))
                    .ToArray(), ClientSettings.WalkingSpeedInKilometerPerHour);

            #endregion

            if (!updateMap) return pokeStops;

            #region Get Gyms

            var pokeGyms = navigation
                .pathByNearestNeighbour(
                    mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym)
                    .OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude))
                    .ToArray(), ClientSettings.WalkingSpeedInKilometerPerHour);

            #endregion

            #region Push data to map

            if (!ClientSettings.MapLoaded) return pokeStops;

            if (pokeGyms.Any())
            {
                infoObservable.PushAvailablePokeGymsLocations(pokeGyms);
            }

            //if map open push object data
            if (!pokeStops.Any()) return pokeStops;

            infoObservable.PushAvailablePokeStopLocations(pokeStops);
            stopsloaded = true;

            #endregion

            return pokeStops;
        }
        
        private FortData[] GetNearbyGyms(GetMapObjectsResponse mapObjectsResponse = null)
        {
            if (mapObjectsResponse == null)
                mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;

            var pokeGyms = navigation
                .pathByNearestNeighbour(
                    mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                    .Where(i => i.Type == FortType.Gym)
                    .OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude))
                    .ToArray(), ClientSettings.WalkingSpeedInKilometerPerHour);

            return pokeGyms;
        }
        
        private void FncPokeStop(Client client, FortData[] pokeStopsIn, bool metros30)
        {
            var distanceFromStart = LocationUtils
                .CalculateDistanceInMeters(
                    ClientSettings.DefaultLatitude,
                    ClientSettings.DefaultLongitude,
                    objClient.CurrentLatitude,
                    objClient.CurrentLongitude);

            lureEncounters.Clear();

            // TODO: do it optionable
            // Reordering array randomly to do it a little more difficult to detect.
            // Random rnd=new Random();
            //FortData[] pokeStops = pokeStopsIn.OrderBy(x => rnd.Next()).ToArray();
            var pokeStops = pokeStopsIn;

            //walk between pokestops in default collection
            foreach (var pokeStop in pokeStops)
            {
                //check if map has pokestops loaded and load if not
                if (ClientSettings.MapLoaded && !stopsloaded)
                {
                    infoObservable.PushAvailablePokeStopLocations(pokeStops);
                    stopsloaded = true;
                }

                #region Mystery Check by Cicklow

                // in Archimedean spiral only capture PokeStops if distance is < to 30 meters!
                if (metros30)
                {
                    var distance1 = LocationUtils
                        .CalculateDistanceInMeters(
                            objClient.CurrentLatitude,
                            objClient.CurrentLongitude,
                            pokeStop.Latitude,
                            pokeStop.Longitude);

                    if (distance1 > 31 && FailedSoftban < 2)
                    {
                        //Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop mas: " + distance.ToString());
                        continue; //solo agarrar los pokestop que esten a menos de 30 metros
                    }
                }

                #endregion

                //make sure user defined limits have not been reached
                SetCheckTimeToRun();

                //update user location on map
                infoObservable.PushNewGeoLocations(new GeoCoordinate(objClient.CurrentLatitude, objClient.CurrentLongitude));

                #region Walk defined Route

                if (ClientSettings.NextDestinationOverride.Count > 0)
                {
                    try
                    {
                        do
                        {
                            WalkUserRoute(pokeStops);

                            #region Check for Exit Command

                            if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation)
                            {
                                break;
                            }

                            #endregion

                            if (!ClientSettings.RepeatUserRoute) continue;

                            foreach (var geocoord in ClientSettings.RouteToRepeat)
                            {
                                ClientSettings.NextDestinationOverride.AddLast(geocoord);
                            }
                        } while (ClientSettings.RepeatUserRoute);
                    }
                    catch (Exception e)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                        Logger.AddLog(string.Format("Error in Walk Defined Route: " + e));
                    }
                }

                #endregion

                #region Check for Exit Command           

                if (ClientSettings.ForceSnipe)
                {
                    ClientSettings.DefaultLatitude = objClient.CurrentLatitude;
                    ClientSettings.DefaultLongitude = objClient.CurrentLongitude;

                    break;
                }

                if (ClientSettings.RelocateDefaultLocation)
                {
                    break;
                }

                #endregion

                //get destination pokestop information
                var distance = LocationUtils
                    .CalculateDistanceInMeters(
                        objClient.CurrentLatitude,
                        objClient.CurrentLongitude,
                        pokeStop.Latitude,
                        pokeStop.Longitude);

                var fortInfo = objClient.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;

                //log error if pokestop not found
                if (fortInfo == null)
                {
                    infoObservable.PushPokeStopInfoUpdate(pokeStop, "!!Can't Get PokeStop Information!!");
                    continue;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");

                #region Break At Lure Logic  

                //check if user wants to break at lured pokestop          
                if (ClientSettings.BreakAtLure && fortInfo.Modifiers.Any())
                {
                    pausetimestamp = -10000;
                    resumetimestamp = fortInfo.Modifiers.First().ExpirationTimestampMs;
                    var timeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Active Lure at next Pokestop - Pausing walk for " + Math.Round(timeRemaining / 60 / 1000, 2) + " Minutes");

                    ClientSettings.pauseAtPokeStop = true;
                }

                #endregion

                try
                {
                    WalkWithRouting(pokeStop.Latitude, pokeStop.Longitude);
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in Walk Default Route: " + e));
                }

                // Pause and farm nearby pokestops
                if (ClientSettings.pauseAtPokeStop)
                {
                    FarmPokestopOnBreak(pokeStops, client);
                }
            }
        }

        #endregion

        #region Walk with Routing Functions

        private void WalkUserRoute(FortData[] pokeStops)
        {
            do
            {
                #region Check for Exit Command

                if (ClientSettings.ForceSnipe)
                {
                    ClientSettings.NextDestinationOverride.Clear();

                    break;
                }

                if (ClientSettings.RelocateDefaultLocation)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Relocate Command Detected - Clearing User Defined Route");

                    ClientSettings.NextDestinationOverride.Clear();
                    ClientSettings.RouteToRepeat.Clear();
                    ClientSettings.RepeatUserRoute = false;

                    break;
                }

                #endregion

                try
                {
                    if (ClientSettings.pauseAtPokeStop)
                    {
                        FarmPokestopOnBreak(pokeStops, objClient);
                    }

                    var pokestopCoords = ClientSettings.NextDestinationOverride.First();
                    ClientSettings.NextDestinationOverride.RemoveFirst();

                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Path Override detected! Rerouting to user-selected pokeStop...");

                    WalkWithRouting(pokestopCoords.Latitude, pokestopCoords.Longitude);
                }
                catch
                {
                    //do nothing for now. Just handle to prevent blowing up.
                }
            } while (ClientSettings.NextDestinationOverride.Count > 0);
        }

        private void WalkWithRouting(double latitude, double longitude)
        {
            if (ClientSettings.UseGoogleMapsAPI)
            {
                DoRouteWalking(latitude, longitude);
            }
            else
            {
                var walkspeed = GetRandomWalkspeed();

                navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchandFarm);
            }
        }

        private void DoRouteWalking(double latitude, double longitude)
        {
            var walkspeed = GetRandomWalkspeed();

            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Getting Google Maps Routing");

            if (ClientSettings.GoogleMapsAPIKey != null)
            {
                #region Normalize Lat Long for Google Directions Request

                var longstring = longitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                var latstring = latitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                var sourcelongstring = objClient.CurrentLongitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");
                var sourcelatstring = objClient.CurrentLatitude.ToString(CultureInfo.InvariantCulture).Replace(",", ".");

                #endregion

                #region Google Directions API Request

                var directionsRequest = new DirectionsRequest
                {
                    ApiKey = ClientSettings.GoogleMapsAPIKey,
                    TravelMode = TravelMode.Walking
                };

                #region Set Directions Request Variables based on client settings

                if (walkspeed > 10 && walkspeed < 20)
                {
                    directionsRequest.TravelMode = TravelMode.Bicycling;

                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Using Directions For Bicycling due to max speed setting > 10km/h");
                }

                if (walkspeed > 20)
                {
                    directionsRequest.TravelMode = TravelMode.Bicycling;
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Using Directions For Driving due to max speed setting > 20km/h");
                }

                if (ClientSettings.SelectedLanguage == "de")
                    directionsRequest.Language = "de";
                if (ClientSettings.SelectedLanguage == "spain")
                    directionsRequest.Language = "es";
                if (ClientSettings.SelectedLanguage == "ptBR")
                    directionsRequest.Language = "pt-BR";
                if (ClientSettings.SelectedLanguage == "tr")
                    directionsRequest.Language = "tr";
                if (ClientSettings.SelectedLanguage == "ru")
                    directionsRequest.Language = "ru";
                if (ClientSettings.SelectedLanguage == "france")
                    directionsRequest.Language = "fr";

                #endregion

                directionsRequest.Origin = sourcelatstring + "," + sourcelongstring;
                directionsRequest.Destination = latstring + "," + longstring;

                var directions = GoogleMaps.Directions.Query(directionsRequest);

                #region  Process Google Directions response

                if (directions.Status == DirectionsStatusCodes.OK)
                {
                    var steps = directions.Routes.First().Legs.First().Steps;
                    var stepcount = 0;
                    foreach (var step in steps)
                    {
                        #region Check for Exit Command

                        if (ClientSettings.ForceSnipe)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Exiting Navigation to begin Snipe");
                            break;
                        }
                        if (ClientSettings.RelocateDefaultLocation)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Exiting Navigation to Relocate");
                            break;
                        }

                        #endregion

                        var directiontext = Helpers.Utils.HtmlRemoval.StripTagsRegexCompiled(step.HtmlInstructions);
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, directiontext);
                        var lastpoint = new Location(objClient.CurrentLatitude, objClient.CurrentLongitude);
                        foreach (var point in step.PolyLine.Points)
                        {
                            var distanceDelta = LocationUtils.CalculateDistanceInMeters(new GeoCoordinate(point.Latitude, point.Longitude), new GeoCoordinate(lastpoint.Latitude, lastpoint.Longitude));
                            if (distanceDelta > 10)
                            {
                                var update = navigation.HumanLikeWalking(new GeoCoordinate(point.Latitude, point.Longitude), walkspeed, ExecuteCatchandFarm, true, false);
                            }
                            lastpoint = point;
                        }
                        stepcount++;
                        if (stepcount == steps.Count())
                        {
                            //Make sure we actually made it to the pokestop! 
                            var remainingdistancetostop = LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, latitude, longitude);
                            if (remainingdistancetostop > 40)
                            {
                                var lowestspeed = 5;
                                //use client settings value for min speed if set.
                                if (ClientSettings.MinWalkSpeed != 0)
                                {
                                    lowestspeed = ClientSettings.MinWalkSpeed;
                                }
                                Logger.ColoredConsoleWrite(ConsoleColor.Green, "As close as google can take us, going off-road at walking speed (" + lowestspeed + ")");
                                var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchandFarm);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Destination Reached!");
                        }
                    }
                }
                #endregion

                #region Goggle Directions Response Logging

                //Log any message other than expected directions response
                else if (directions.Status == DirectionsStatusCodes.REQUEST_DENIED)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Request Failed! Bad API key?");
                    var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else if (directions.Status == DirectionsStatusCodes.OVER_QUERY_LIMIT)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Over 2500 queries today! Are you botting unsafely? :)");
                    var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else if (directions.Status == DirectionsStatusCodes.NOT_FOUND)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Geocoding coords failed! Waypoint: " + latitude + "," + longitude + " Bot Location: " + objClient.CurrentLatitude + "," + objClient.CurrentLongitude);
                    var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Unhandled Error occurred when getting route[ STATUS:" + directions.StatusStr + " ERROR MESSAGE:" + directions.ErrorMessage + "] Using default walk method instead.");
                    var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }

                #endregion
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"API Key not found in Client Settings! Using default method instead.");
                var update = navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
            }

            #endregion
        }

        public bool CheckAvailablePokemons(Client client)
        {
            infoObservable.PushClearPokemons();

            var pokeData = DataCollector.GetFastPokeMapData(objClient.CurrentLatitude, objClient.CurrentLongitude).Result;
            var toShow = new List<DataCollector.PokemonMapData>();

            if (pokeData == null) return false;

            toShow.AddRange(pokeData.Where(poke => poke.Coordinates.Latitude.HasValue && poke.Coordinates.Longitude.HasValue));

            if (toShow.Count > 0)
            {
                infoObservable.PushNewPokemonLocations(toShow);
            }

            return true;
        }

        private bool CheckAndFarmNearbyPokeStop(FortData pokeStop, Client client, FortDetailsResponse fortInfo)
        {
            if (count >= 9)
            {
                LogStatsEtc();
            }

            if (pokeStop.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds && ClientSettings.FarmPokestops)
            {
                var fortSearch = objClient.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;
                if(ClientSettings.EnableVerboseLogging)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "================[VERBOSE LOGGING - Pokestop Search]================");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Result: {fortSearch.Result}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"ChainHackSequenceNumber: {fortSearch.ChainHackSequenceNumber}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Cooldown Complete (MS): {fortSearch.CooldownCompleteTimestampMs}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"EXP Award: {fortSearch.ExperienceAwarded}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Gems Award: {fortSearch.GemsAwarded}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Item Award: {fortSearch.ItemsAwarded}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Egg Data: {fortSearch.PokemonDataEgg}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "==================================================================");
                }

                switch (fortSearch.Result.ToString())
                {
                    case "NoResultSet":
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Pokestop Error: We did not recieve a result from the pokestop.");
                        break;
                    case "Success":
                        // It already showed our pokestop Information
                        break;
                    case "OutOfRange":
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Pokestop Error: The pokestop is out of range!");
                        break;
                    case "InCooldownPeriod":
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Pokestop Warning: The current Pokestop is in the cooldown period.");
                        break;
                    case "InventoryFull":
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Pokestop Warning: Your Inventory is full. You did not recieve any items.");
                        break;
                    case "ExceededDailyLimit":
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Pokestop Error: You are above your daily limit of pokestops! You should stop farming pokestops.");
                        break;
                }

                count++;

                var pokeStopInfo = $"{fortInfo.Name}{Environment.NewLine}Visited:{DateTime.Now.ToString("HH:mm:ss")}{Environment.NewLine}";

                if (fortSearch.ExperienceAwarded > 0)
                {
                    var egg = "/";

                    if (fortSearch.PokemonDataEgg != null)
                    {
                        egg = fortSearch.PokemonDataEgg.EggKmWalkedTarget + "km";
                    }

                    var items = "";

                    if (fortSearch.ItemsAwarded != null)
                    {
                        items = StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded);
                    }

                    var logrestock = false;

                    if (fortSearch.ItemsAwarded != null)
                    {
                        foreach (var item in fortSearch.ItemsAwarded)
                        {
                            if (item.ItemId == ItemId.ItemPokeBall || item.ItemId == ItemId.ItemGreatBall || item.ItemId == ItemId.ItemUltraBall)
                            {
                                logrestock = true;
                            }
                        }

                        if (logrestock && pokeballoutofstock)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");

                            ClientSettings.CatchPokemon = true;
                            pokeballoutofstock = false;
                        }

                        FailedSoftban = 0;
                        BotStats.AddExperience(fortSearch.ExperienceAwarded);
                        pokeStopFarmedCount++;

                        Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}{", Egg: " + egg}, Items: {items}", LogLevel.Info);

                        pokeStopInfo += $"{fortSearch.ExperienceAwarded} XP{Environment.NewLine}{fortSearch.GemsAwarded}{Environment.NewLine}{egg}{Environment.NewLine}{items.Replace(",", Environment.NewLine)}";

                        if (pokeStop.LureInfo != null)
                        {
                            var lurePokemon = pokeStop.LureInfo.ActivePokemonId;

                            if (!ClientSettings.catchPokemonSkipList.Contains(lurePokemon))
                            {
                                if (!lureEncounters.Contains(pokeStop.LureInfo.EncounterId.ToString()))
                                {
                                    CatchPokemon(pokeStop.LureInfo.EncounterId, pokeStop.LureInfo.FortId, pokeStop.LureInfo.ActivePokemonId, pokeStop.Longitude, pokeStop.Latitude);

                                    lureEncounters.Add(pokeStop.LureInfo.EncounterId.ToString());
                                }
                                else
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Lure Pokemon: " + pokeStop.LureInfo.ActivePokemonId + "because we have already caught him, or catching pokemon is disabled");
                                }
                            }
                        }

                        double eggs = 0;

                        if (fortSearch.PokemonDataEgg != null)
                        {
                            eggs = fortSearch.PokemonDataEgg.EggKmWalkedTarget;
                        }

                        Telegram?.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Pokestop, fortInfo.Name, fortSearch.ExperienceAwarded, eggs, fortSearch.GemsAwarded, StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded));

                       
                    }
                }

                infoObservable.PushPokeStopInfoUpdate(pokeStop, pokeStopInfo);

                return true;
            }

            if (!ClientSettings.FarmPokestops)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Farm Pokestop option unchecked, skipping and only looking for pokemon");

                return false;
            }

            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop not ready to farm again, skipping and only looking for pokemon");

            return false;
        }

        private bool ExecuteCatchandFarm()
        {
            if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation)
            {
                return false;
            }
            if ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds > lastsearchtimestamp + 10000)
            {
                lastsearchtimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                var mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;
                //narrow map data to pokestops within walking distance
                var pokeStops = GetNearbyPokeStops(false, mapObjectsResponse);
                var pokestopsWithinRangeStanding = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude) < 40);

                var withinRangeStandingList = pokestopsWithinRangeStanding as IList<FortData> ?? pokestopsWithinRangeStanding.ToList();
                if (withinRangeStandingList.Any())
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{withinRangeStandingList.Count} Pokestops within range of user");

                    foreach (var pokestop in withinRangeStandingList)
                    {
                        var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;
                        var farmed = CheckAndFarmNearbyPokeStop(pokestop, objClient, fortInfo);

                        if (farmed)
                        {
                            pokestop.CooldownCompleteTimestampMs = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500;
                        }

                        SetCheckTimeToRun();
                        RandomHelper.RandomSleep(500, 600); // Time between pokestops
                    }
                }
                ExecuteCatchAllNearbyPokemons(mapObjectsResponse);
                
                if (ClientSettings.FarmGyms)
                    ExecutePutInGym();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ExecuteCatchAllNearbyPokemons()
        {
            return ExecuteCatchAllNearbyPokemons(null);
        }

        private bool ExecuteCatchAllNearbyPokemons(GetMapObjectsResponse mapObjectsResponse )
        {
            //update location map with current bot location
            if (!stateSniper)
            {
                infoObservable.PushNewGeoLocations(new GeoCoordinate(objClient.CurrentLatitude, objClient.CurrentLongitude));
            }

            var client = objClient;
            
            //bypass catching pokemon if disabled
            if (ClientSettings.CatchPokemon || (ClientSettings.SnipePokemon && stateSniper))
            {
                if (mapObjectsResponse == null)
                    mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;
                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons).OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude));

                if(ClientSettings.EnableVerboseLogging)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue, $"(DEBUG) - Pokemons: {pokemons.Count()}");
                }

                if (pokemons.Any())
                {
                    var strNames = pokemons.Aggregate("", (current, pokemon) => current + (StringUtils.getPokemonNameByLanguage(ClientSettings, pokemon.PokemonId) + ", "));
                    strNames = strNames.Substring(0, strNames.Length - 2);

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s): " + strNames);
                    
                    //ShowNearbyPokemons(pokemons);
                }
                else
                {
                    if (stateSniper)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "No Pokemon Found!");
                    }
                }

                //catch them all!
                foreach (var pokemon in pokemons)
                {
                    #region Stats Log

                    //increment log stats counter and log stats
                    count++;

                    if (count >= 9 && !stateSniper)
                    {
                        LogStatsEtc();
                    }

                    #endregion

                    //Capture only Snipe pokemon
                    if (stateSniper)
                    {
                        if (snipokemonIds != pokemon.PokemonId) continue;
                    }

                    #region Skip pokemon if in list

                    if (ClientSettings.catchPokemonSkipList.Contains(pokemon.PokemonId))
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Pokemon: " + pokemon.PokemonId);
                        continue;
                    }

                    #endregion

                    //get distance to pokemon
                    var distance = LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);

                    RandomHelper.RandomSleep(distance > 100 ? 1000 : 100,distance > 100 ? 1100 : 110);

                    // Do Catch here
                    CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude);
                }
                return true;
            }
            return false;
        }

private int GetGymLevel(long value)
        {
            if (value >= 50000)
                return 10;
            if (value >= 40000)
                return 9;
            if (value >= 30000)
                return 8;
            if (value >= 20000)
                return 7;
            if (value >= 16000)
                return 6;
            if (value >= 12000)
                return 5;
            if (value >= 8000)
                return 4;
            if (value >= 4000)
                return 3;
            if (value >= 2000)
                return 2;
            return 1;
        }
        private static List<string> gymsVisited = new List<string>();
        private bool CheckAndPutInNearbyGym(FortData gym, Client client, FortDetailsResponse fortInfo)
        {
            var gymColorLog = ConsoleColor.DarkGray;

            if (gymsVisited.IndexOf(gym.Id) > -1  ){
                Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym was already visited.");
                return false;
            }
            if (ClientSettings.FarmGyms)
            {
                var pokemons = (client.Inventory.GetPokemons().Result).ToList();
                var pokemon = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId == "") )).OrderBy(x => x.Cp).FirstOrDefault();
                if (pokemon == null)
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There are no pokemons to assign.");
                    return false;
                }
                RandomHelper.RandomSleep(100, 200);
                var profile = client.Player.GetPlayer().Result;
                if ( (gym.OwnedByTeam ==  profile.PlayerData.Team) || (gym.OwnedByTeam == POGOProtos.Enums.TeamColor.Neutral ))
                {
                    RandomHelper.RandomSleep(100, 200);
                    var gymDetails = client.Fort.GetGymDetails(gym.Id,gym.Latitude,gym.Longitude).Result;
                    Logger.ColoredConsoleWrite(gymColorLog, "Members: " +gymDetails.GymState.Memberships.Count +". Level: "+ GetGymLevel(gym.GymPoints));
                    if (gymDetails.GymState.Memberships.Count < GetGymLevel(gym.GymPoints))
                    {
                        RandomHelper.RandomSleep(100, 200);
                        var fortSearch = client.Fort.FortDeployPokemon(gym.Id, pokemon.Id).Result;
                        if (fortSearch.Result.ToString().ToLower() == "success" ){
                            Logger.ColoredConsoleWrite(gymColorLog, StringUtils.getPokemonNameByLanguage(ClientSettings, (PokemonId)pokemon.PokemonId) +" inserted into the gym");
                            gymsVisited.Add(gym.Id);
                            var pokesInGym = pokemons.Where(x => ( (!x.IsEgg) && (x.DeployedFortId != "") )).OrderBy(x => x.Cp).ToList().Count();
                            Logger.ColoredConsoleWrite(gymColorLog, "pokesInGym: "+ pokesInGym);
                            if (pokesInGym >9 )
                            { 
                                var res = client.Player.CollectDailyDefenderBonus().Result;
                                Logger.ColoredConsoleWrite(gymColorLog, $"(Gym) - Collected: {res.CurrencyAwarded} Coins.");
                            }
                        }
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - There is no free space in the gym");
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(gymColorLog, "(Gym) - This gym is not your team.");
                    // TO-DO ATTACK ;)?
                    //var getPokemon = getpokemons.Where(x => ((!x.IsEgg) && (x.DeployedFortId != ""))).OrderBy(x => x.Cp);
                    //var getOwnPokemon = client.Inventory.GetPokemons().Result.Where(x => !x.IsEgg).OrderBy(x => x.Cp);

                    //var resp = client.Fort.StartGymBattle(gym.Id, getPokemon, getOwnPokemon)
                    //We need a list for the "getOwnPokemons" that can attack. I think its a max of 6 that can attack. Not sure tho
                }
            }
            return true;
        }

        private void ExecutePutInGym()
        {

            //narrow map data to gyms within walking distance
            var gyms = GetNearbyGyms();
            var gymsWithinRangeStanding = gyms.Where(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude) < 40);

            var withinRangeStandingList = gymsWithinRangeStanding as IList<FortData> ?? gymsWithinRangeStanding.ToList();
            var inRange = withinRangeStandingList.Count;
            if (withinRangeStandingList.Any())
            {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, $"(Gym) - {inRange} gyms are within range of the user");

                foreach (var gym in withinRangeStandingList)
                {
                    var fortInfo = objClient.Fort.GetFort(gym.Id, gym.Latitude, gym.Longitude).Result;
                    CheckAndPutInNearbyGym(gym, objClient, fortInfo);
                    SetCheckTimeToRun();
                    RandomHelper.RandomSleep(100, 200);
                }
            }
        }
        
        private bool VerifyLocation()
        {
            #region Stay within defined radius

            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude, objClient.CurrentLatitude, objClient.CurrentLongitude);
            
            //walk back to default location if outside of defined radius
            if ((ClientSettings.MaxWalkingRadiusInMeters == 0 ||
                !(distanceFromStart > ClientSettings.MaxWalkingRadiusInMeters)) &&
                !ClientSettings.RelocateDefaultLocation)
            {
                return false;
            }

            var walkingspeed = ClientSettings.WalkingSpeedInKilometerPerHour;

            if (ClientSettings.RelocateDefaultLocation)
            {
                if (ClientSettings.RelocateDefaultLocationTravelSpeed > 0)
                {
                    walkingspeed = ClientSettings.RelocateDefaultLocationTravelSpeed;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Relocating to new Default Location! Travelling at " + walkingspeed + "km/h");

                ClientSettings.RelocateDefaultLocation = false;
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You're outside of the defined max. walking radius. Walking back!");
            }

            WalkWithRouting(ClientSettings.DefaultLatitude, ClientSettings.DefaultLongitude);

            return true;

            #endregion
        }

        private void CatchPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong = 0, double pokeLat = 0)
        {
            EncounterResponse encounterPokemonResponse;

            //Offset Miss count here to account for user setting.
            var missCount = 0;

            if (ClientSettings.Max_Missed_throws <= 1)
            {
                missCount = 2;
            }

            if (ClientSettings.Max_Missed_throws == 2)
            {
                missCount = 1;
            }

            var forceHit = false;

            try
            {
                if (stateSniper)
                {
                    // I think we were doing this twice! Yes, we are because otherwise bot can not catch multiple pokemon.
                    // Example: We snipe Squirtle, but there are 3 of them. 
                    if (sniperReturn)
                    {
                        var result = objClient.Player.UpdatePlayerLocation(pokeLat, pokeLong, ClientSettings.DefaultAltitude).Result;
                    }
                }
                encounterPokemonResponse = objClient.Encounter.EncounterPokemon(encounterId, spawnpointId).Result;
            }
            finally
            {
                if (stateSniper)
                {
                    var result = objClient.Player.UpdatePlayerLocation(
                        ClientSettings.DefaultLatitude,
                        ClientSettings.DefaultLongitude,
                        ClientSettings.DefaultAltitude).Result;

                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "I found it! Returning before starting the capture (trick)...");

                    sniperReturn = true;
                }
            }

            if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
            {
                if (SkippedPokemon.Contains(encounterPokemonResponse.WildPokemon.EncounterId))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Previously Skipped this Pokemon - Skipping Again!");
                    return;
                }

                var bestPokeball = GetBestBall(encounterPokemonResponse?.WildPokemon, false);

                if (bestPokeball == ItemId.ItemUnknown)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}%");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");

                    pokeballoutofstock = true;
                    ClientSettings.CatchPokemon = false;

                    return;
                }

                var inventoryBerries = objClient.Inventory.GetItems().Result;
                var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();

                var escaped = false;
                var berryThrown = false;
                var berryOutOfStock = false;

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Encountered {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% Probability {Math.Round(probability.Value * 100)}%");

                var iv = PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData);
                if (encounterPokemonResponse.WildPokemon.PokemonData != null &&
                    encounterPokemonResponse.WildPokemon.PokemonData.Cp > ClientSettings.MinCPtoCatch &&
                    iv > ClientSettings.MinIVtoCatch)
                {
                    var used = false;
                    CatchPokemonResponse caughtPokemonResponse;

                    do
                    {
                        // Check if the best ball is still valid
                        if (bestPokeball == ItemId.ItemUnknown)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}%");
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");

                            pokeballoutofstock = true;
                            ClientSettings.CatchPokemon = false;

                            return;
                        }

                        if (((probability.Value < ClientSettings.razzberry_chance) || escaped) && ClientSettings.UseRazzBerry && !used)
                        {
                            var bestBerry = GetBestBerry(encounterPokemonResponse?.WildPokemon);
                            if (bestBerry != ItemId.ItemUnknown)
                            {
                                var berriesInInventory = inventoryBerries as IList<ItemData> ?? inventoryBerries.ToList();
                                var berryList = inventoryBerries as IList<ItemData> ?? berriesInInventory.ToList();
                                var berries = berryList.FirstOrDefault(p => p.ItemId == bestBerry);

                                if (berries.Count <= 0) berryOutOfStock = true;

                                if (!berryOutOfStock)
                                {
                                    //Throw berry
                                    var useRaspberry = objClient.Encounter.UseCaptureItem(encounterId, bestBerry, spawnpointId).Result;
                                    berryThrown = true;
                                    used = true;

                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Thrown {bestBerry}. Remaining: {berries.Count}.", LogLevel.Info);

                                    RandomHelper.RandomSleep(50, 200);
                                }
                                else
                                {
                                    berryThrown = true;
                                    escaped = true;
                                    used = true;
                                }
                            }
                            else
                            {
                                berryThrown = true;
                                escaped = true;
                                used = true;
                            }
                        }

                        // limit number of balls wasted by misses and log for UX because fools be tripin                        
                        var r = new Random();
                        switch (missCount)
                        {
                            case 0:
                                if (bestPokeball == ItemId.ItemMasterBall)
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "No messing around with your Master Balls! Forcing a hit on target.");
                                    forceHit = true;
                                }
                                break;
                            case 1:
                                if (bestPokeball == ItemId.ItemUltraBall)
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Not wasting more of your Ultra Balls! Forcing a hit on target.");
                                    forceHit = true;
                                }
                                break;
                            case 2:
                                //adding another chance of forcing hit here to improve overall odds after 2 misses                                
                                var rInt = r.Next(0, 2);
                                if (rInt == 1)
                                {
                                    // lets hit
                                    forceHit = true;
                                }
                                break;
                            default:
                                // default to force hit after 3 wasted balls of any kind.
                                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Enough misses! Forcing a hit on target.");
                                forceHit = true;
                                break;
                        }
                        if (missCount > 0)
                        {
                            //adding another chance of forcing hit here to improve overall odds after 1st miss                            
                            var rInt = r.Next(0, 3);
                            if (rInt == 1)
                            {
                                // lets hit
                                forceHit = true;
                            }
                        }

                        caughtPokemonResponse = CatchPokemonWithRandomVariables(encounterId, spawnpointId, bestPokeball, forceHit);

                        if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Missed {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} while using {bestPokeball}");
                            missCount++;
                            RandomHelper.RandomSleep(1500, 6000);
                        }
                        else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"{StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} escaped while using {bestPokeball}");
                            escaped = true;
                            //reset forceHit in case we randomly triggered on last throw.
                            forceHit = false;
                            RandomHelper.RandomSleep(1500, 6000);
                        }
                        // Update the best ball to ensure we can still throw
                        bestPokeball = GetBestBall(encounterPokemonResponse?.WildPokemon, escaped);
                    } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        //DeletePokemonFromMap(encounterPokemonResponse.WildPokemon.SpawnPointId).Wait();
                        foreach (var xp in caughtPokemonResponse.CaptureAward.Xp)
                            BotStats.AddExperience(xp);

                        var curDate = DateTime.Now;
                        infoObservable.PushNewHuntStats($"{pokeLat}/{pokeLong};{pokeid};{curDate.Ticks};{curDate}" + Environment.NewLine);

                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        var logs = Path.Combine(logPath, "PokeLog.txt");
                        var date = DateTime.Now;
                        if (caughtPokemonResponse.CaptureAward.Xp.Sum() >= 500)
                        {
                            if (ClientSettings.logPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught new {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.White, $"Caught New {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% using {bestPokeball} got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP.");
                            pokemonCatchCount++;
                        }
                        else
                        {
                            if (ClientSettings.logPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Caught {StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% using {bestPokeball} got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP.");
                            pokemonCatchCount++;

                            if (Telegram != null)
                                Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Catch, StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid), encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00"), bestPokeball, caughtPokemonResponse.CaptureAward.Xp.Sum());

                            BotStats.AddPokemon(1);
                            RandomHelper.RandomSleep(1500, 2000);
                        }
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{StringUtils.getPokemonNameByLanguage(ClientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% got away while using {bestPokeball}..");
                        FailedSoftban++;
                        if (FailedSoftban > 10)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Soft Ban Detected - Stopping Bot to prevent perma-ban. Try again in 4-24 hours and be more careful next time!");
                            StringUtils.CheckKillSwitch(true);
                        }
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon CP or IV lower than Configured Min to Catch - Skipping Pokemon");
                    SkippedPokemon.Add(encounterPokemonResponse.WildPokemon.EncounterId);
                }
                RandomHelper.RandomSleep(1000, 2000); // wait 1 second to simulate catch.
            }
            
        }

        private CatchPokemonResponse CatchPokemonWithRandomVariables(ulong encounterId, string spawnpointId, ItemId bestPokeball, bool forceHit)
        {
            #region Reset Function Variables

            var normalizedRecticleSize = 1.95;
            var hitTxt = "Default Perfect";
            var spinModifier = 1.0;
            var spinTxt = "Curve";
            var pbExcellent = ClientSettings.Pb_Excellent;
            var pbGreat = ClientSettings.Pb_Excellent;
            var pbNice = ClientSettings.Pb_Nice;
            var pbOrdinary = ClientSettings.Pb_Ordinary;
            var r = new Random();
            var rInt = r.Next(0, 100);

            #endregion

            #region Randomize Throw Type

            if (rInt >= 0 && rInt < pbExcellent)
            {
                normalizedRecticleSize = r.NextDouble() * (1.95 - 1.7) + 1.7;
                hitTxt = "Excellent";
            }
            else if (rInt >= pbExcellent && rInt < pbExcellent + pbGreat)
            {
                normalizedRecticleSize = r.NextDouble() * (1.7 - 1.3) + 1.3;
                hitTxt = "Great";
            }
            else if (rInt >= pbExcellent + pbGreat && rInt < pbExcellent + pbGreat + pbNice)
            {
                normalizedRecticleSize = r.NextDouble() * (1.3 - 1) + 1;
                hitTxt = "Nice";
            }
            else if (rInt >= pbExcellent + pbGreat + pbNice && rInt < pbExcellent + pbGreat + pbNice + pbOrdinary)
            {
                normalizedRecticleSize = r.NextDouble() * (1 - 0.1) + 0.1;
                hitTxt = "Ordinary";
            }
            else
            {
                normalizedRecticleSize = r.NextDouble() * (1 - 0.1) + 0.1;
                hitTxt = "Ordinary";
            }

            var rIntSpin = r.Next(0, 2);
            if (rIntSpin == 0)
            {
                spinModifier = 0.0;
                spinTxt = "Straight";
            }
            var rIntHit = r.Next(0, 2);
            if (rIntHit == 0)
            {
                forceHit = true;
            }

            #endregion

            //round to 2 decimals  
            normalizedRecticleSize = Math.Round(normalizedRecticleSize, 2);
            //if not miss, log throw variables
            if (forceHit)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"{hitTxt} throw as {spinTxt} ball.");
            }
            return objClient.Encounter.CatchPokemon(encounterId, spawnpointId, bestPokeball, forceHit, normalizedRecticleSize, spinModifier).Result;
        }

        #endregion

        #region Evlove Transfer Functions

        private void EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter = null)
        {
            int evolvecount = 0;

            if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation)
            {
                return;
            }
            var pokemonToEvolve = objClient.Inventory.GetPokemonToEvolve(filter).Result;
            if (pokemonToEvolve.Count() != 0)
            {
                if (ClientSettings.UseLuckyEgg)
                {
                    objClient.Inventory.UseLuckyEgg(objClient).Wait();
                }
            }

            foreach (var pokemon in pokemonToEvolve)
            {
                if (!ClientSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                {
                    continue;
                }
                var evolvePokemonOutProto = objClient.Inventory.EvolvePokemon(pokemon.Id).Result;
                var date = DateTime.Now.ToString();
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                var evolvelog = Path.Combine(logPath, "EvolveLog.txt");

                var getPokemonName = StringUtils.getPokemonNameByLanguage(ClientSettings, pokemon.PokemonId);
                var cp = pokemon.Cp;
                var calcPerf = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                var getEvolvedName = StringUtils.getPokemonNameByLanguage(ClientSettings, evolvePokemonOutProto.EvolvedPokemonData.PokemonId);
                var getEvolvedCP = evolvePokemonOutProto.EvolvedPokemonData.Cp;
                var getXP = evolvePokemonOutProto.ExperienceAwarded.ToString("N0");

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    if(evolvecount == 0)
                    {
                        if (ClientSettings.pauseAtEvolve2)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stopping to evolve some Pokemons.");
                            ClientSettings.pauseTheWalking = true;
                        }
                    }

                    if (ClientSettings.bLogEvolve)
                    {
                        File.AppendAllText(evolvelog, $"[{date}] - Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {getXP}xp");
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {getXP}xp", LogLevel.Info);
                    BotStats.AddExperience(evolvePokemonOutProto.ExperienceAwarded);

                    if (Telegram != null)
                        Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Evolve, StringUtils.getPokemonNameByLanguage(ClientSettings, pokemon.PokemonId), pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"), StringUtils.getPokemonNameByLanguage(ClientSettings, evolvePokemonOutProto.EvolvedPokemonData.PokemonId), evolvePokemonOutProto.EvolvedPokemonData.Cp, evolvePokemonOutProto.ExperienceAwarded.ToString("N0"));
                    evolvecount++;
                }
                else
                {
                    if (evolvePokemonOutProto.Result != EvolvePokemonResponse.Types.Result.Success)
                    {
                        if (ClientSettings.bLogEvolve)
                        {
                            File.AppendAllText(evolvelog, $"[{date}] - Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}");
                        }
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}", LogLevel.Info);
                        evolvecount++;
                    }
                }
                if (ClientSettings.UseAnimationTimes)
                {
                    RandomHelper.RandomSleep(30000, 35000);
                }
                else
                {
                    RandomHelper.RandomSleep(500, 600);
                }
            }
            if(evolvecount > 0)
            {
                if (ClientSettings.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons evolved. Time to continue our journey!");
                    ClientSettings.pauseTheWalking = false;
                }
            }

        }

        private void TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false, bool transferFirstLowIv = false)
        {
            if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation)
            {
                return;
            }
            if (ClientSettings.TransferDoublePokemons)
            {
                var duplicatePokemons = objClient.Inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve, transferFirstLowIv).Result;
                if (ClientSettings.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stopping to transfer some Pokemons.");
                    ClientSettings.pauseTheWalking = true;
                }
                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!ClientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp >= ClientSettings.DontTransferWithCPOver || PokemonInfo.CalculatePokemonPerfection(duplicatePokemon) >= objClient.Settings.ivmaxpercent)
                        {
                            continue; // Isnt this wrong? Shouldnt it return instead of continueing?
                        }

                        var bestPokemonOfType = objClient.Inventory.GetHighestCPofType(duplicatePokemon).Result;
                        var bestPokemonsCpOfType = objClient.Inventory.GetHighestCPofType2(duplicatePokemon).Result;
                        var bestPokemonsIvOfType = objClient.Inventory.GetHighestIVofType(duplicatePokemon).Result;

                        var transfer = objClient.Inventory.TransferPokemon(duplicatePokemon.Id).Result;

                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        var logs = Path.Combine(logPath, "TransferLog.txt");
                        var date = DateTime.Now.ToString();

                        if (transferFirstLowIv)
                        {
                            if (ClientSettings.logManualTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {StringUtils.getPokemonNameByLanguage(ClientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best IV: {PokemonInfo.CalculatePokemonPerfection(bestPokemonsIvOfType.First()).ToString("0.00")} %)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(ClientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best IV: {PokemonInfo.CalculatePokemonPerfection(bestPokemonsIvOfType.First()).ToString("0.00")} %)", LogLevel.Info);
                        }
                        else
                        {
                            if (ClientSettings.logManualTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {StringUtils.getPokemonNameByLanguage(ClientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best: {bestPokemonsCpOfType.First().Cp} CP)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(ClientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best: {bestPokemonsCpOfType.First().Cp} CP)", LogLevel.Info);
                        }

                        if (Telegram != null)
                            Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Transfer, StringUtils.getPokemonNameByLanguage(ClientSettings, duplicatePokemon.PokemonId), duplicatePokemon.Cp, PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00"), bestPokemonOfType);

                        RandomHelper.RandomSleep(500, 600); // Make faster used to now is poosible transfer several pokemons at same time
                    }
                }
                if (ClientSettings.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons transfered. Time to continue our journey!");
                    ClientSettings.pauseTheWalking = false;
                }
            }
        }

        #endregion

        #region Best Ball and Berry Functions

        private Dictionary<string, int> GetPokeballQty()
        {
            var pokeBallCollection = new Dictionary<string, int>();
            var items = objClient.Inventory.GetItems().Result;
            var balls = items.Where(i => (i.ItemId == ItemId.ItemPokeBall || i.ItemId == ItemId.ItemGreatBall || i.ItemId == ItemId.ItemUltraBall || i.ItemId == ItemId.ItemMasterBall) && i.ItemId > 0).GroupBy(i => i.ItemId).ToList();

            #region Log Pokeball types out of stock

            if (balls.Any(g => g.Key == ItemId.ItemPokeBall))
                if (balls.First(g => g.Key == ItemId.ItemPokeBall).First().Count > 0)
                    pokeBallCollection.Add("pokeBalls", balls.First(g => g.Key == ItemId.ItemPokeBall).First().Count);
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"FYI - PokeBall Count is Zero", LogLevel.Info);

            if (balls.Any(g => g.Key == ItemId.ItemGreatBall))
                if (balls.First(g => g.Key == ItemId.ItemGreatBall).First().Count > 0)
                    pokeBallCollection.Add("greatBalls", balls.First(g => g.Key == ItemId.ItemGreatBall).First().Count);
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"FYI - GreatBall Count is Zero", LogLevel.Info);

            if (balls.Any(g => g.Key == ItemId.ItemUltraBall))
                if (balls.First(g => g.Key == ItemId.ItemUltraBall).First().Count > 0)
                    pokeBallCollection.Add("ultraBalls", balls.First(g => g.Key == ItemId.ItemUltraBall).First().Count);
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"FYI - UltraBall Count is Zero", LogLevel.Info);

            if (balls.Any(g => g.Key == ItemId.ItemMasterBall))
                if (balls.First(g => g.Key == ItemId.ItemMasterBall).First().Count > 0)
                    pokeBallCollection.Add("masterBalls", balls.First(g => g.Key == ItemId.ItemMasterBall).First().Count);
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"FYI - MasterBall Count is Zero", LogLevel.Info);

            #endregion

            return pokeBallCollection;
        }

        private ItemId GetBestBall(WildPokemon pokemon, bool escaped)
        {
            //pokemon cp to determine ball type
            var pokemonCp = pokemon?.PokemonData?.Cp;
            var pokeballCollection = GetPokeballQty();

            #region Set Available ball types

            var pokeBalls = false;
            var greatBalls = false;
            var ultraBalls = false;
            var masterBalls = false;
            var pokeballqty = 0;
            var greatballqty = 0;
            var ultraballqty = 0;

            foreach (var pokeballtype in pokeballCollection)
            {
                switch (pokeballtype.Key)
                {
                    case "pokeBalls":
                        {
                            pokeballqty = pokeballtype.Value;
                            break;
                        }
                    case "greatBalls":
                        {
                            greatballqty = pokeballtype.Value;
                            break;
                        }
                    case "ultraBalls":
                        {
                            ultraballqty = pokeballtype.Value;
                            break;
                        }
                }
            }
            if (pokeballCollection.ContainsKey("pokeBalls"))
            {
                pokeBalls = true;
                if ((pokeballqty <= ClientSettings.InventoryBasePokeball || ClientSettings.InventoryBasePokeball == 0) && ClientSettings.LimitPokeballUse)
                {
                    pokeBalls = false;
                }
            }
            if (pokeballCollection.ContainsKey("greatBalls"))
            {
                greatBalls = true;
                if ((greatballqty <= ClientSettings.InventoryBaseGreatball || ClientSettings.InventoryBaseGreatball == 0) && ClientSettings.LimitGreatballUse)
                {
                    greatBalls = false;
                }
            }

            if (pokeballCollection.ContainsKey("ultraBalls"))
            {
                ultraBalls = true;
                if ((ultraballqty <= ClientSettings.InventoryBaseUltraball || ClientSettings.InventoryBaseUltraball == 0) && ClientSettings.LimitUltraballUse)
                {
                    ultraBalls = false;
                }
            }

            if (pokeballCollection.ContainsKey("masterBalls"))
            {
                masterBalls = true;
            }

            #endregion

            #region Get Lowest Appropriate Ball by CP and escape status

            var lowestAppropriateBall = ItemId.ItemUnknown;

            var minCPforGreatBall = 500;
            var minCPforUltraBall = 1000;

            if (ClientSettings.MinCPforGreatBall > 0 && ClientSettings.MinCPforUltraBall > 0 && ClientSettings.MinCPforGreatBall < ClientSettings.MinCPforUltraBall)
            {
                minCPforGreatBall = ClientSettings.MinCPforGreatBall;
                minCPforUltraBall = ClientSettings.MinCPforUltraBall;
            }

            var getMyLowestAppropriateBall = new Dictionary<Func<int?, bool>, Action>
            {
                {x => x < minCPforGreatBall, () => lowestAppropriateBall = ItemId.ItemPokeBall}, {x => x < minCPforUltraBall, () => lowestAppropriateBall = ItemId.ItemGreatBall}, {x => x < 2000, () => lowestAppropriateBall = ItemId.ItemUltraBall}, {x => x >= 2000, () => lowestAppropriateBall = ItemId.ItemMasterBall}
            };
            getMyLowestAppropriateBall.First(sw => sw.Key(pokemonCp)).Value();
            //use next best ball if pokemon has escped before
            if (escaped && ClientSettings.NextBestBallOnEscape)
            {
                switch (lowestAppropriateBall)
                {
                    case ItemId.ItemGreatBall:
                        {
                            lowestAppropriateBall = ItemId.ItemUltraBall;
                            break;
                        }
                    case ItemId.ItemUltraBall:
                        {
                            lowestAppropriateBall = ItemId.ItemMasterBall;
                            break;
                        }
                    case ItemId.ItemMasterBall:
                        {
                            lowestAppropriateBall = ItemId.ItemMasterBall;
                            break;
                        }
                    default:
                        {
                            lowestAppropriateBall = ItemId.ItemGreatBall;
                            break;
                        }
                }
            }
            //handle appropriate ball out of stock
            switch (lowestAppropriateBall)
            {
                case ItemId.ItemGreatBall:
                    {
                        if (greatBalls) return ItemId.ItemGreatBall;
                        if (ultraBalls) return ItemId.ItemUltraBall;
                        if (masterBalls) return ItemId.ItemMasterBall;
                        if (pokeBalls) return ItemId.ItemPokeBall;
                        return ItemId.ItemUnknown;
                    }
                case ItemId.ItemUltraBall:
                    {
                        if (ultraBalls) return ItemId.ItemUltraBall;
                        if (masterBalls) return ItemId.ItemMasterBall;
                        if (greatBalls) return ItemId.ItemGreatBall;
                        if (pokeBalls) return ItemId.ItemPokeBall;
                        return ItemId.ItemUnknown;
                    }
                case ItemId.ItemMasterBall:
                    {
                        if (masterBalls) return ItemId.ItemMasterBall;
                        if (ultraBalls) return ItemId.ItemUltraBall;
                        if (greatBalls) return ItemId.ItemGreatBall;
                        if (pokeBalls) return ItemId.ItemPokeBall;
                        return ItemId.ItemUnknown;
                    }
                default:
                    {
                        if (pokeBalls) return ItemId.ItemPokeBall;
                        if (greatBalls) return ItemId.ItemGreatBall;
                        if (ultraBalls) return ItemId.ItemUltraBall;
                        if (pokeBalls) return ItemId.ItemMasterBall;
                        return ItemId.ItemUnknown;
                    }
            }

            #endregion
        }

        private ItemId GetBestBerry(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = objClient.Inventory.GetItems().Result;
            var berries = items.Where(i => i.ItemId == ItemId.ItemRazzBerry || i.ItemId == ItemId.ItemBlukBerry || i.ItemId == ItemId.ItemNanabBerry || i.ItemId == ItemId.ItemWeparBerry || i.ItemId == ItemId.ItemPinapBerry).GroupBy(i => i.ItemId).ToList();
            if (berries.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Berrys to select! - Using next best ball instead", LogLevel.Info);
                return ItemId.ItemUnknown;
            }

            var razzBerryCount = objClient.Inventory.GetItemAmountByType(ItemId.ItemRazzBerry).Result;
            var blukBerryCount = objClient.Inventory.GetItemAmountByType(ItemId.ItemBlukBerry).Result;
            var nanabBerryCount = objClient.Inventory.GetItemAmountByType(ItemId.ItemNanabBerry).Result;
            var weparBerryCount = objClient.Inventory.GetItemAmountByType(ItemId.ItemWeparBerry).Result;
            var pinapBerryCount = objClient.Inventory.GetItemAmountByType(ItemId.ItemPinapBerry).Result;

            if (pinapBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemPinapBerry;
            if (weparBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemWeparBerry;
            if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemNanabBerry;
            if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemBlukBerry;

            if (weparBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemWeparBerry;
            if (nanabBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemNanabBerry;
            if (blukBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemBlukBerry;

            if (nanabBerryCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemNanabBerry;
            if (blukBerryCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemBlukBerry;

            if (blukBerryCount > 0 && pokemonCp >= 500)
                return ItemId.ItemBlukBerry;

            return berries.OrderBy(g => g.Key).First().Key;
        }

        #endregion

        #region Recycle and Incense Functions

        private void RecycleItems(bool forcerefresh = false)
        {
            if (ClientSettings.ForceSnipe)
            {
                return;
            }
            if (ClientSettings.RelocateDefaultLocation)
                return;
            var items = objClient.Inventory.GetItemsToRecycle(ClientSettings).Result;

            foreach (var item in items)
            {
                if ((item.ItemId == ItemId.ItemPokeBall || item.ItemId == ItemId.ItemGreatBall || item.ItemId == ItemId.ItemUltraBall || item.ItemId == ItemId.ItemMasterBall) && pokeballoutofstock)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");
                    ClientSettings.CatchPokemon = true;
                    pokeballoutofstock = false;
                }
                var transfer = objClient.Inventory.RecycleItem(item.ItemId, item.Count).Result;
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {item.ItemId}", LogLevel.Info);
                RandomHelper.RandomSleep(1000, 5000);
            }
        }

        private DateTime lastincenseuse;

        public void UseIncense()
        {
            if (ClientSettings.ForceSnipe)
            {
                return;
            }
            if (ClientSettings.RelocateDefaultLocation)
                return;
            if (ClientSettings.UseIncense || ClientSettings.UseIncenseGUIClick)
            {
                ClientSettings.UseIncenseGUIClick = false;
                var inventory = objClient.Inventory.GetItems().Result;
                var incsense = inventory.Where(p => p.ItemId == ItemId.ItemIncenseOrdinary).FirstOrDefault();
                var loginterval = DateTime.Now - LastIncenselog;
                if (lastincenseuse > DateTime.Now.AddSeconds(5))
                {
                    var duration = lastincenseuse - DateTime.Now;
                    var minute = DateTime.Now.AddMinutes(1) - DateTime.Now;
                    if (loginterval > minute)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Incense still running: {duration.Minutes}m{duration.Seconds}s");
                        LastIncenselog = DateTime.Now;
                    }
                    return;
                }

                if (incsense == null || incsense.Count <= 0)
                {
                    return;
                }

                objClient.Inventory.UseIncense(ItemId.ItemIncenseOrdinary).Wait();
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                RandomHelper.RandomSleep(3000,3100);
            }
        }

        #endregion

        #region Incubator Functions
        private List<POGOProtos.Data.PokemonData> eggsHatchingAllowed(List<POGOProtos.Data.PokemonData> eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> (eggs);
            if(ClientSettings.No2kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=2).ToList();
            }
            if(ClientSettings.No5kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=5).ToList();
            }
            if(ClientSettings.No10kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }
        
        private List<POGOProtos.Data.PokemonData> eggsHatchingAllowedBasicInc(List<POGOProtos.Data.PokemonData> eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> (eggs);
            if(ClientSettings.No2kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=2).ToList();
            }
            if(ClientSettings.No5kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=5).ToList();
            }
            if(ClientSettings.No10kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }

        private void StartIncubation()
        {
            try
            {
                if (ClientSettings.ForceSnipe || ClientSettings.RelocateDefaultLocation)
                {
                    return;
                }
                var inventory = objClient.Inventory.GetInventory().Result;
                var incubators = objClient.Inventory.GetEggIncubators(inventory).ToList();
                var unusedEggs = (objClient.Inventory.GetEggs(inventory)).Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart).ToList();
                var pokemons = (objClient.Inventory.GetPokemons().Result).ToList();

                var playerStats = objClient.Inventory.GetPlayerStats(inventory);
                var stats = playerStats.First();

                var kmWalked = stats.KmWalked;

                var rememberedIncubatorsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Configs", "incubators.json");
                var rememberedIncubators = GetRememberedIncubators(rememberedIncubatorsFilePath);

                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                var logs = Path.Combine(logPath, "EggLog.txt");
                var date = DateTime.Now.ToString();

                var unusedEggsBasicInc = eggsHatchingAllowedBasicInc(unusedEggs); 
                unusedEggs = eggsHatchingAllowed(unusedEggs);
                

                foreach (var incubator in rememberedIncubators)
                {
                    var hatched = pokemons.FirstOrDefault(x => !x.IsEgg && x.Id == incubator.PokemonId);
                    if (hatched == null) continue;

                    //Hier diggi
                    if (ClientSettings.logEggs)
                    {
                        File.AppendAllText(logs, $"[{date}] - Egg hatched and we got a {hatched.PokemonId} (CP: {hatched.Cp} | MaxCP: {PokemonInfo.CalculateMaxCP(hatched)} | Level: {PokemonInfo.GetLevel(hatched)} | IV: {PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00")}% )" + Environment.NewLine);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg hatched and we got a " + hatched.PokemonId + " CP: " + hatched.Cp + " MaxCP: " + PokemonInfo.CalculateMaxCP(hatched) + " Level: " + PokemonInfo.GetLevel(hatched) + " IV: " + PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00") + "%");
                }

                if ((unusedEggs.Count < 1) && (unusedEggs.Count < 1))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "There is not Allowed Eggs to hatch");
                    return;
                }

                var newRememberedIncubators = new List<IncubatorUsage>();

                foreach (var incubator in incubators)
                {
                    if (incubator.PokemonId == 0)
                    {

                        // If is basic incubator and user don't want use it, we go to the next incubator
                        if (    (incubator.ItemId == ItemId.ItemIncubatorBasic) 
                             && ( ! ClientSettings.UseBasicIncubators) )
                            continue;

                        POGOProtos.Data.PokemonData egg;
                        if (incubator.ItemId == ItemId.ItemIncubatorBasic) 
                            egg = ClientSettings.EggsAscendingSelectionBasicInc ? unusedEggsBasicInc.FirstOrDefault() : unusedEggsBasicInc.LastOrDefault();
                        else 
                            egg = ClientSettings.EggsAscendingSelection ? unusedEggs.FirstOrDefault() : unusedEggs.LastOrDefault();

                        // If there is not eggs then we finish this function
                        if (egg == null)
                            return;

                        var response = objClient.Inventory.UseItemEggIncubator(incubator.Id, egg.Id).Result;
                        try{
                            unusedEggs.Remove(egg);
                            unusedEggsBasicInc.Remove(egg);
                        } catch (Exception e){

                        }
                        newRememberedIncubators.Add(new IncubatorUsage { IncubatorId = incubator.Id, PokemonId = egg.Id });
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Added Egg which needs " + egg.EggKmWalkedTarget + "km");
                        // We need some sleep here or this shit explodes
                        RandomHelper.RandomSleep(100, 200);
                    }
                    else
                    {
                        newRememberedIncubators.Add(new IncubatorUsage
                        {
                            IncubatorId = incubator.Id,
                            PokemonId = incubator.PokemonId
                        });

                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg (" + (incubator.TargetKmWalked - incubator.StartKmWalked) + "km) need to walk " + Math.Round(incubator.TargetKmWalked - kmWalked, 2) + " km.");
                    }
                }

                if (!newRememberedIncubators.SequenceEqual(rememberedIncubators))
                    SaveRememberedIncubators(newRememberedIncubators, rememberedIncubatorsFilePath);
            }
            catch (Exception e)
            {
                // Leave this here: Logger.Error(e.StackTrace);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg: We dont have any eggs we could incubate.");
            }
        }

        private static List<IncubatorUsage> GetRememberedIncubators(string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
                return JsonConvert.DeserializeObject<List<IncubatorUsage>>(File.ReadAllText(filePath, Encoding.UTF8));

            return new List<IncubatorUsage>(0);
        }

        private static void SaveRememberedIncubators(List<IncubatorUsage> incubators, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            File.WriteAllText(filePath, JsonConvert.SerializeObject(incubators), Encoding.UTF8);
        }

        private class IncubatorUsage : IEquatable<IncubatorUsage>
        {
            public string IncubatorId;
            public ulong PokemonId;

            public bool Equals(IncubatorUsage other)
            {
                return other != null && other.IncubatorId == IncubatorId && other.PokemonId == PokemonId;
            }
        }

        #endregion

        #region Unused Functions

        //TODO: Delete; If not used why keep?
        public void ShowNearbyPokemonsRun(IEnumerable<MapPokemon> pokeData)
        {
            infoObservable.PushClearPokemons();
            var toShow = new List<DataCollector.PokemonMapData>();

            if (pokeData == null) return;

            foreach (var poke in pokeData)
            {
                var poke2 = new DataCollector.PokemonMapData
                {
                    Id = poke.SpawnPointId,
                    PokemonId = poke.PokemonId,
                    Coordinates = new LatitudeLongitude
                    {
                        Coordinates = new List<double> { poke.Longitude, poke.Latitude }
                    }
                };

                try
                {
                    var numberOfTicks = poke.ExpirationTimestampMs;
                    numberOfTicks *= 10000; // convert MS in Ticks

                    if (numberOfTicks >= DateTime.MinValue.Ticks && numberOfTicks <= DateTime.MaxValue.Ticks)
                    {
                        poke2.ExpiresAt = new DateTime(numberOfTicks).AddYears(1969).AddDays(-1);
                    }
                    else
                    {
                        Logger.AddLog("Read invalid Date");
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Read invalid Date");
                }
                toShow.Add(poke2);
            }

            if (toShow.Count > 0)
            {
                infoObservable.PushNewPokemonLocations(toShow);
            }
        }

        public void ShowNearbyPokemons(IEnumerable<MapPokemon> pokeData)
        {
            Task.Factory.StartNew(() => ShowNearbyPokemonsRun(pokeData));
        }

        public void DeletePokemonFromMap(string spawnPointId)
        {
            Task.Factory.StartNew(() => infoObservable.PushDeletePokemonLocation(spawnPointId));
        }

        private double _distance(double lat1, double lng1, double lat2, double lng2)
        {
            const double rEarth = 6378137;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lng2 - lng1) * Math.PI / 180;
            var alpha = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var d = 2 * rEarth * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            return d;
        }

        #endregion

        #endregion
    }
}