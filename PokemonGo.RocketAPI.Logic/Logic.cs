using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using System.Net;
using System.IO;
using System.Device.Location;
using PokemonGo.RocketAPI.Helpers;
using System.Web.Script.Serialization;
using POGOProtos.Map.Pokemon;
using POGOProtos.Inventory.Item;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using POGOProtos.Map.Fort;
using POGOProtos.Data;
using System.Threading;
using POGOProtos.Inventory;
using Newtonsoft.Json;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using GoogleMapsApi.Entities.Geocoding.Request;
using GoogleMapsApi.Entities.Geocoding.Response;
using GoogleMapsApi.StaticMaps;
using GoogleMapsApi.StaticMaps.Entities;
using PokemonGo.RocketApi.PokeMap;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace PokemonGo.RocketAPI.Logic
{
    public class Logic
    {
        #region Define Session Variables
        public static Client _client;
        public readonly ISettings _clientSettings;
        public TelegramUtil _telegram;
        public BotStats _botStats;
        private readonly Navigation _navigation;
        public const double SpeedDownTo = 10 / 3.6;
        private readonly LogicInfoObservable _infoObservable;
        private readonly PokeVisionUtil _pokevision;
        private int pokemonCatchCount;
        private int pokeStopFarmedCount;
        private double timetorunstamp = -10000;
        private double pausetimestamp = -10000;
        private double resumetimestamp = -10000;
        private double lastlog = -10000;
        private double startingXP = -10000;
        private double currentxp = -10000;
        private bool havelures = false;
        private bool pokeballoutofstock = false;
        private bool stopsloaded = false;
        public static Logic _instance;
        private bool _pauseWalking = false;
        private List<string> lureEncounters = new List<string>();
        private int count = 0;
        public static int failed_softban = 0;
        int level = -1;
        public bool pauseWalking
        {
            get
            {
                if (_navigation != null)
                {
                    _pauseWalking = _navigation.pauseWalking;
                }
                return _pauseWalking;
            }
            set
            {
                if (_navigation != null)
                {
                    _navigation.pauseWalking = value;
                    _pauseWalking = value;
                }
            }
        }
        public DateTime lastIncenselog;
        //Snipe vars
        private readonly PokeSnipers _pokeSnipers;
        private bool StateSniper = false;

        public Logic(ISettings clientSettings, LogicInfoObservable infoObservable)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
            _client.setFailure(new ApiFailureStrat(_client));
            _botStats = new BotStats();
            _navigation = new Navigation(_client);
            _pokevision = new PokeVisionUtil();
            _infoObservable = infoObservable;
            _instance = this;
            _pokeSnipers = new PokeSnipers();
        }
        public String lure = "lureId";
        public PokemonId luredpokemoncaught = PokemonId.Articuno;
        #endregion

        #region Execute Functions

        public async Task Execute()
        {
            // Check if disabled
            StringUtils.CheckKillSwitch();
            await SetCheckTimeToRun();
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "/////////////////////////////////|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||////////////////////////////////////////");
            for (int i = 0; i < 4; i++)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Don't login with the new App Version (0.3.7) (0.3.5 is ok!), or you will probably get Banned if you use the bot again after that!");
                await RandomHelper.RandomDelay(100, 500);
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "/////////////////////////////////|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||/////////////////////////////////////////");

            Logger.ColoredConsoleWrite(ConsoleColor.Red, "This bot is absolutely free and open-source!");
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "If you've paid for it. Request a chargeback immediately!");
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Starting Execute on login server: {_clientSettings.AuthType}", LogLevel.Info);
            if (_clientSettings.logPokemons)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled Pokemonlogging. It will be saved to \"\\Logs\\PokeLog.txt\"");
            }
            if (_clientSettings.logManualTransfer)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled manual transfer logging. It will be saved to \"\\Logs\\TransferLog.txt\"");
            }
            if (_clientSettings.bLogEvolve)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You enabled Evolution Logging. It will be saved to \"\\Logs\\EvolutionLog.txt\"");
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Setting Pokemon Catch Count: to 0 for this session", LogLevel.Info);
            pokemonCatchCount = 0;
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Setting Pokestop Farmed Count to 0 for this session", LogLevel.Info);
            pokeStopFarmedCount = 0;
            _client.CurrentAltitude = _client.Settings.DefaultAltitude;
            _client.CurrentLongitude = _client.Settings.DefaultLongitude;
            _client.CurrentLatitude = _client.Settings.DefaultLatitude;

            if (_client.CurrentAltitude == 0)
            {
                _client.CurrentAltitude = LocationUtils.getAltidude(_client.CurrentLatitude, _client.CurrentLongitude);
                Logger.Error("Altidude was 0, resolved that. New Altidude is now: " + _client.CurrentAltitude);
            }

            if (_clientSettings.UseProxyVerified)
            {
                Logger.Error("===============================================");
                Logger.Error("Proxy enabled.");
                Logger.Error("ProxyIP: " + _clientSettings.UseProxyHost + ":" + _clientSettings.UseProxyPort);
                Logger.Error("===============================================");
            }

            while (true)
            {
                try
                {
                    await _client.Login.DoLogin();

                    if (!string.IsNullOrEmpty(_clientSettings.TelegramAPIToken) && !string.IsNullOrEmpty(_clientSettings.TelegramName))
                    {
                        try
                        {
                            _telegram = new TelegramUtil(_client, new Telegram.Bot.TelegramBotClient(_clientSettings.TelegramAPIToken), _clientSettings, _client.Inventory);

                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "To activate informations with Telegram, write the bot a message for more informations");
                            var me = await _telegram.getClient().GetMeAsync();
                            _telegram.getClient().OnCallbackQuery += _telegram.BotOnCallbackQueryReceived;
                            _telegram.getClient().OnMessage += _telegram.BotOnMessageReceived;
                            _telegram.getClient().OnMessageEdited += _telegram.BotOnMessageReceived;
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Telegram Name: " + me.Username);
                            _telegram.getClient().StartReceiving();
                        }
                        catch (Exception) { }
                    }

                    await PostLoginExecute();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error: " + ex.Source);
                    Logger.Error($"{ex}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Trying to Restart.");
                    try
                    {
                        if (_telegram != null)
                        {
                            _telegram.getClient().StopReceiving();
                        }
                    }
                    catch (Exception) { }
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Restarting in 10 Seconds.");
                await Task.Delay(10000);
            }
        }

        private PokemonId SnipokemonIds;
        public async Task PostLoginExecute()
        {
            while (true)
            {
                var _restart = true;
                try
                {
                    //Sniper
                    if (_clientSettings.SnipePokemon)
                    {
                        foreach (spottedPokeSni p in await _pokeSnipers.CapturarPokemon())
                        {
                            SnipokemonIds = p._pokeId;
                            await CapturarSniper(p);
                        }
                        //Exit...
                        StringUtils.CheckKillSwitch(true);
                    }

                    var profil = await _client.Player.GetPlayer();
                    await _client.Inventory.ExportPokemonToCSV(profil.PlayerData);
                    await StatsLog(_client);

                    if (_clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await EvolveAllPokemonWithEnoughCandy();
                    }

                    if (_clientSettings.AutoIncubate)
                    {
                        await StartIncubation();
                    }

                    await TransferDuplicatePokemon(_clientSettings.keepPokemonsThatCanEvolve, _clientSettings.TransferFirstLowIV);
                    await RecycleItems();
                    await ExecuteFarmingPokestopsAndPokemons(_client);
                }
                catch (AccessTokenExpiredException)
                {
                    throw new AccessTokenExpiredException();
                }
                catch (Exception ex)
                {
                    Logger.Write($"Exception: {ex}", LogLevel.Error);
                    if (ex.ToString().Contains("Time To Run Reached"))
                    {
                        _restart = false;
                    }
                }
                if (_restart)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Starting again in 60 seconds...");
                    await Task.Delay(60000);
                }
                else
                {
                    throw new Exception("Time To Run Reached");
                }
            }
        }

        #endregion

        #region Update Functions

        public static Version getNewestVersion()
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
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/ver.md");
        }

        #endregion

        #region Stats log and Session Check Functions

        private async Task StatsLog(Client client)
        {
            //
            _client.readyToUse = true;  // Enable Pokemon List cause everything is loaded

            // Check if disabled
            StringUtils.CheckKillSwitch();
            await SetCheckTimeToRun();
            var profile = await _client.Player.GetPlayer();
            var playerStats = await _client.Inventory.GetPlayerStats();
            var stats = playerStats.First();

            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;
            if (startingXP == -10000) startingXP = stats.Experience;
            currentxp = stats.Experience;
            var pokemonToEvolve = (await _client.Inventory.GetPokemonToEvolve()).Count();
            var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(150) * 100;
            var pokedexpercent = Math.Floor(pokedexpercentraw);

            if (curexp == 0 && expneeded == 1000)
            {
                // Do Tutorial
                await _client.Misc.MarkTutorialComplete();
            }
            var items = await _client.Inventory.GetItems();
            var pokeBallCollection = await GetPokeballQty();

            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "-----------------------[PLAYER STATS]-----------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Level/EXP: {stats.Level} | {curexp.ToString("N0")}/{expneeded.ToString("N0")} ({Math.Round(curexppercent, 2)}%)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP to Level up: " + (stats.NextLevelXp - stats.Experience)); ;
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokeStops visited: " + stats.PokeStopVisits);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "KM Walked: " + Math.Round(stats.KmWalked, 2));
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemon: " + await _client.Inventory.getPokemonCount() + " + " + await _client.Inventory.GetEggsCount() + " Eggs /" + profile.PlayerData.MaxPokemonStorage + " (" + pokemonToEvolve + " Evolvable)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokedex Completion: " + stats.UniquePokedexEntries + "/150 " + "[" + pokedexpercent + "%]");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount.ToString("N0"));
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemon Catch Count this session: " + pokemonCatchCount);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokeStop Farmed Count this session: " + pokeStopFarmedCount);

            var totalitems = 0;
            foreach (var item in items)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, item.ItemId + " Qty: " + item.Count);
                totalitems += item.Count;
                if (item.ItemId == ItemId.ItemTroyDisk && item.Count > 0) { havelures = true; }
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Items: " + totalitems + "/" + profile.PlayerData.MaxItemStorage);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");
            if (profile.PlayerData.MaxItemStorage - totalitems <= 20)
            {
                await RecycleItems();
            }
            if (level == -1)
            {
                level = stats.Level;
            }
            else if (stats.Level > level)
            {
                level = stats.Level;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Got the level up reward from your level up.");
                var lvlup = await _client.Player.GetLevelUpRewards(stats.Level);
                List<ItemId> alreadygot = new List<ItemId>();
                foreach (var i in lvlup.ItemsAwarded)
                {
                    if (!alreadygot.Contains(i.ItemId))
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Got Item: " + i.ItemId + " " + i.ItemCount + "x");
                        alreadygot.Add(i.ItemId);
                    }
                }
                alreadygot.Clear();
            }

            Console.Title = profile.PlayerData.Username + " lvl" + stats.Level + "-(" + (stats.Experience - stats.PrevLevelXp -
                StringUtils.getExpDiff(stats.Level)).ToString("N0") + "/" + (stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + "|" + Math.Round(curexppercent, 2) + "%)| Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount + "| " + _botStats.ToString();
            if (_clientSettings.CheckWhileRunning)
            {
                if (getNewestVersion() > Assembly.GetEntryAssembly().GetName().Version)
                {
                    if (_clientSettings.AutoUpdate)
                    {
                        Form update = new Update();
                        update.ShowDialog();
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("There is an Update on Github. do you want to open it ?", "Newest Version: " + getNewestVersion(), MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            //nothing   
                        }
                    }
                }
            }
        }

        private async Task LogStatsEtc()
        {
            if (_clientSettings.UseIncenseGUIClick) await UseIncense();

            count = 0;
            if (_clientSettings.UseLuckyEggIfNotRunning || _clientSettings.UseLuckyEggGUIClick)
            {
                _clientSettings.UseLuckyEggGUIClick = false;
                await _client.Inventory.UseLuckyEgg(_client);
            }

            if (_clientSettings.EvolvePokemonsIfEnoughCandy)
            {
                await EvolveAllPokemonWithEnoughCandy();
            }

            if (_clientSettings.AutoIncubate)
            {
                await StartIncubation();
            }

            await TransferDuplicatePokemon(_clientSettings.keepPokemonsThatCanEvolve, _clientSettings.TransferFirstLowIV);
            //await RecycleItems();               
            await StatsLog(_client);
            await SetCheckTimeToRun();
        }

        private async Task SetCheckTimeToRun()
        {
            if (((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds) > (lastlog + 60000))
            {
                lastlog = ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                #region Time to Run
                if (_clientSettings.TimeToRun > 0)
                {
                    if (timetorunstamp == -10000)
                    {
                        timetorunstamp = (_clientSettings.TimeToRun * 60 * 1000) + ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds);
                    }
                    else
                    {
                        var runTimeRemaining = timetorunstamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        if (runTimeRemaining <= 0)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Time To Run Reached or Exceeded...Walking back to default location and stopping bot");

                            await WalkWithRouting(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchandFarm);

                            StringUtils.CheckKillSwitch(true);
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, String.Format("Remaining Time to Run: {0} minutes", Math.Round(runTimeRemaining / 1000 / 60, 2)));
                        }
                    }
                }

                #endregion

                #region Breaks
                if (_clientSettings.UseBreakFields)
                {
                    if (pausetimestamp > -10000)
                    {
                        var walkTimeRemaining = pausetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        if (walkTimeRemaining <= 0)
                        {
                            pausetimestamp = -10000;
                            _clientSettings.pauseAtPokeStop = true;
                            resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + _clientSettings.BreakLength * 60 * 1000;
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, String.Format("Break Time! Pause walking for {0} minutes", _clientSettings.BreakLength));
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, String.Format("Remaining Time until break: {0} minutes", Math.Round(walkTimeRemaining / 1000 / 60, 2)));
                        }
                    }
                    else if (resumetimestamp == -10000)
                    {
                        pausetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + _clientSettings.BreakInterval * 60 * 1000;
                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, String.Format("Remaining Time until break: {0} minutes", _clientSettings.BreakInterval));
                    }
                }
                if (resumetimestamp > -10000)
                {
                    var breakTimeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    if (breakTimeRemaining <= 0)
                    {
                        resumetimestamp = -10000;
                        _clientSettings.pauseAtPokeStop = false;
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Break over, back to walking!");
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, String.Format("Remaining Time until resume walking: {0} minutes", Math.Round(breakTimeRemaining / 1000 / 60, 2)));
                    }
                }

                #endregion

                #region Log Catch Disabled
                //add logging for pokemon catch disabled here for now to prevent spamming
                if (!_clientSettings.CatchPokemon)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Catching Pokemon Disabled in Client Settings - Skipping all pokemon");
                }
                #endregion

                #region Check Run Count Limits

                #region Catch Pokemon Count Check
                if (pokemonCatchCount >= _clientSettings.PokemonCatchLimit)
                {

                    if (_clientSettings.FarmPokestops)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached - Bot will only farm pokestops");
                        _clientSettings.CatchPokemon = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached and not farming pokestops - Bot will return to default location and stop");

                        await WalkWithRouting(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchandFarm);

                        StringUtils.CheckKillSwitch(true);
                    }
                }
                #endregion

                #region Farm Pokestops Check
                if (pokeStopFarmedCount >= _clientSettings.PokestopFarmLimit)
                {
                    if (_clientSettings.CatchPokemon)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached - Bot will only catch pokemon");
                        _clientSettings.FarmPokestops = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached and not catching pokemon - Bot will return to default location and stop");

                        await WalkWithRouting(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchandFarm);

                        StringUtils.CheckKillSwitch(true);
                    }
                }
                #endregion

                #region XP Check
                if (startingXP != -10000 && currentxp != -10000 && (currentxp = -startingXP) >= _clientSettings.XPFarmedLimit)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "XP Farmed Limit Reached - Bot will return to default location and stop");
                    if (_clientSettings.UseGoogleMapsAPI)
                    {
                        await WalkWithRouting(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchandFarm);
                    }
                    else
                    {
                        var walkHome = await _navigation.HumanLikeWalking(new GeoCoordinate(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
                    }
                    StringUtils.CheckKillSwitch(true);
                }
                #endregion

                #endregion
            }
        }

        #endregion

        #region Catch, Farm and Walk Logic

        private async Task CapturarSniper(spottedPokeSni _p)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Trying to capture: " + _p._pokeId);
            var result = await _client.Player.UpdatePlayerLocation(_p._lat, _p._lng, _clientSettings.DefaultAltitude);
            StateSniper = true;
            await ExecuteCatchAllNearbyPokemons();
            StateSniper = false;
        }

        private async Task Espiral(Client client, FortData[] pokeStops)
        {
            //Intento de pajarera 1...
            await ExecuteCatchAllNearbyPokemons();
            Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Starting Archimedean spiral");
            int i2 = 0;
            double angle;
            double centerx;
            double centery;
            double xx;
            double yy;
            Boolean salir = true;
            double distancia;
            double cantidadvar = 0.0001;
            double recorrido = _client.Settings.MaxWalkingRadiusInMeters;

            pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude) <= _clientSettings.MaxWalkingRadiusInMeters).ToArray();

            centerx = _client.CurrentLatitude;
            centery = _client.CurrentLongitude;

            if (recorrido <= 100) cantidadvar = 0.00008;
            if (recorrido > 100 && recorrido <= 500) cantidadvar = 0.00009;
            if (recorrido > 500 && recorrido <= 1000) cantidadvar = 0.0001;
            if (recorrido > 1000) cantidadvar = 0.0002;

            while (salir)
            {
                angle = 0.3 * i2;
                xx = centerx + (cantidadvar * angle) * Math.Cos(angle);
                yy = centery + (cantidadvar * angle) * Math.Sin(angle);

                distancia = Navigation.DistanceBetween2Coordinates(centerx, centery, xx, yy);

                if (distancia > recorrido)
                {
                    salir = false;
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Returning to the starting point...");
                    var update = await _navigation.HumanLikeWalking(new GeoCoordinate(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
                    break;
                }
                if (i2 % 10 == 0) Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Distance from starting point: " + distancia.ToString() + " metros...");

                await _navigation.HumanLikeWalking(new GeoCoordinate(xx, yy), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);

                //_infoObservable.PushAvailablePokeStopLocations(pokeStops);
                Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Looking PokeStops who are less than 30 meters...");

                await fncPokeStop(_client, pokeStops, true);
                i2++;
            }
        }

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            #region Check and report
            // Check Distance from start
            var verifiedLocation = VerifyLocation().Result;
            // Get PokeStops         
            var pokeStops = GetNearbyPokeStops().Result;
            // make sure we found pokestops and log if none found
            if (_clientSettings.MaxWalkingRadiusInMeters != 0)
            {
                pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, i.Latitude, i.Longitude) <= _clientSettings.MaxWalkingRadiusInMeters).ToArray();
                if (pokeStops.Count() == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops in a range of " + _clientSettings.MaxWalkingRadiusInMeters + "m!");
                    await ExecuteCatchAllNearbyPokemons();
                }
            }
            if (pokeStops.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops, which are unused! Probably the server are unstable, or you visted them all. Retrying..");
                await ExecuteCatchAllNearbyPokemons();
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " usable PokeStops near your current location.");
            }
            #endregion

            #region Start Walk
            // Walk Spiral if enabled
            if (_clientSettings.Espiral)
            {
                await Espiral(client, pokeStops);
                return;
            }
            //Normal Walk and Catch between pokestops
            await fncPokeStop(_client, pokeStops, false);
            #endregion
        }

        private async Task<FortData[]> GetNearbyPokeStops()
        {
            #region Get Pokestops
            //Query nearby objects for mapData
            var mapObjects = await _client.Map.GetMapObjects();
            //narrow map data to pokestops within walking distance
            var pokeStops =
            _navigation.pathByNearestNeighbour(
            mapObjects.Item1.MapCells.SelectMany(i => i.Forts)
            .Where(
                i =>
                i.Type == FortType.Checkpoint &&
                i.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
                .OrderBy(
                i =>
                LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude)).ToArray(), _clientSettings.WalkingSpeedInKilometerPerHour);
            #endregion

            #region Get Gyms
            var pokeGyms =
            _navigation.pathByNearestNeighbour(
            mapObjects.Item1.MapCells.SelectMany(i => i.Forts)
            .Where(
                i =>
                i.Type == FortType.Gym)
                .OrderBy(
                i =>
                LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude)).ToArray(), _clientSettings.WalkingSpeedInKilometerPerHour);
            #endregion

            #region Push data to map
            if (_clientSettings.MapLoaded)
            {
                if (pokeGyms.Any())
                {
                    _infoObservable.PushAvailablePokeGymsLocations(pokeGyms);
                }
                //if map open push object data
                if (pokeStops.Any())
                {
                    _infoObservable.PushAvailablePokeStopLocations(pokeStops);
                    stopsloaded = true;
                }
            }
            #endregion

            return pokeStops;
        }

        private async Task fncPokeStop(Client client, FortData[] pokeStopsIn, bool metros30)
        {
            //calculate player distance from start to use later
            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _client.CurrentLatitude, _client.CurrentLongitude);
            //Clear this every so often to prevent bloat on long runs
            lureEncounters.Clear();
            
            // TODO: do it optionable
            // Reordering array randomly to do it a little more difficult to detect.
            // Random rnd=new Random();
			//FortData[] pokeStops = pokeStopsIn.OrderBy(x => rnd.Next()).ToArray();
			FortData[] pokeStops = pokeStopsIn;
			
            //walk between pokestops in default collection
			foreach (var pokeStop in pokeStops)
            {
                //check if map has pokestops loaded and load if not
                if (_clientSettings.MapLoaded && !stopsloaded)
                {
                    _infoObservable.PushAvailablePokeStopLocations(pokeStops);
                    stopsloaded = true;
                }

                #region Mystery Check by Cicklow
                // in Archimedean spiral only capture PokeStops if distance is < to 30 meters!
                if (metros30)
                {
                    var distance1 = LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude);
                    if (distance1 > 31 && failed_softban < 2)
                    {
                        //Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop mas: " + distance.ToString());
                        continue; //solo agarrar los pokestop que esten a menos de 30 metros
                    }
                }
                #endregion

                //make sure user defined limits have not been reached
                await SetCheckTimeToRun();
                //catch nearby pokemon before beginning walk to next pokestop         
                await ExecuteCatchAllNearbyPokemons();

                #region Navigate to nearby pokemon if found by SkipLagged API
                //SkipLagged API
                if (_clientSettings.pokevision)
                {
                    foreach (spottedPoke p in await _pokevision.GetNearPokemons(_client.CurrentLatitude, _client.CurrentLongitude))
                    {
                        var dist = LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, p._lat, p._lng);
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"SkipLagged: There is a {StringUtils.getPokemonNameByLanguage(_clientSettings, p._pokeId)} to {dist:0.##} meters. Trying to Capture...");
                        var upd = await _navigation.HumanLikeWalking(new GeoCoordinate(p._lat, p._lng), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
                    }
                }
                #endregion

                //update user location on map
                _infoObservable.PushNewGeoLocations(new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude));

                //get destination pokestop information
                var distance = LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await _client.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");

                //log error if pokestop not found
                if (fortInfo == null)
                {
                    _infoObservable.PushPokeStopInfoUpdate(pokeStop, "!!Can't Get PokeStop Information!!");
                    continue;
                }

                #region Break At Lure Logic  
                //check if user wants to break at lured pokestop              
                if (_clientSettings.BreakAtLure && fortInfo.Modifiers.Any())
                {
                    pausetimestamp = -10000;
                    resumetimestamp = fortInfo.Modifiers.First().ExpirationTimestampMs;
                    var timeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Active Lure at next Pokestop - Pausing walk for " + Math.Round(timeRemaining / 60 / 1000, 2) + " Minutes");
                    _clientSettings.pauseAtPokeStop = true;
                }
                #endregion

                #region Walk defined Route
                if (_clientSettings.NextDestinationOverride.Count > 0)
                {
                    try
                    {
                        do
                        {
                            await WalkUserRoute(pokeStops);

                            if (_clientSettings.RepeatUserRoute)
                            {
                                foreach (var geocoord in _clientSettings.RouteToRepeat)
                                {
                                    _clientSettings.NextDestinationOverride.AddLast(geocoord);
                                }
                            }
                        }
                        while (_clientSettings.RepeatUserRoute);
                    }
                    catch (Exception e)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                        Logger.AddLog(string.Format("Error in Walk Defined Route: " + e.ToString()));
                    }
                }
                #endregion

                #region Walk default route
                //walk with routing if enabled or directly to pokestop if not
                var walkspeed = GetRandomWalkspeed();

                try
                {
                    await WalkWithRouting(pokeStop, walkspeed, ExecuteCatchandFarm);
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in Walk Default Route: " + e.ToString()));
                }

                #endregion

                #region Farm Pokestop and continue walk or catch pokemon and continue farming nearby pokestops on break
                // flag to indicate whether bot has added lure if lure on break enabled
                var addedlure = false;
                // Pause and farm nearby pokestops
                if (_clientSettings.pauseAtPokeStop)
                {
                    await FarmPokestopOnBreak(pokeStops, client, addedlure);
                }
                else
                {
                    await ExecuteCatchAllNearbyPokemons();
                    var farmed = await CheckAndFarmNearbyPokeStop(pokeStop, _client, fortInfo);
                    await RandomHelper.RandomDelay(50, 2000); // wait to start moving again 
                }
                #endregion
            }
        }

        private async Task FarmPokestopOnBreak(FortData[] pokeStops, Client client, bool addedlure)
        {
            //check for overlapping pokestops where we are taking a break
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Reached break location. Using Lures Enabled");
            var pokestopsWithinRangeStanding = pokeStops.Where(i => (LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude)) < 40);
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{pokestopsWithinRangeStanding.Count().ToString()} Pokestops within range of where you are standing.");
            //Begin farming loop while on break
            do
            {
                foreach (var Pokestop in pokestopsWithinRangeStanding)
                {
                    await ExecuteCatchAllNearbyPokemons();
                    var FortInfo = await _client.Fort.GetFort(Pokestop.Id, Pokestop.Latitude, Pokestop.Longitude);

                    if ((_clientSettings.UseLureGUIClick && havelures) || (_clientSettings.UseLureAtBreak && havelures && !Pokestop.ActiveFortModifier.Any() && !addedlure))
                    {
                        _clientSettings.UseLureGUIClick = false;
                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Use Lure at break enabled - Adding lure and setting resume walking to 30 minutes");
                        await client.Fort.AddFortModifier(FortInfo.FortId, ItemId.ItemTroyDisk);
                        resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 30000;
                        addedlure = true;
                    }
                    var farmed = await CheckAndFarmNearbyPokeStop(Pokestop, _client, FortInfo);
                    if (farmed) { Pokestop.CooldownCompleteTimestampMs = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500; }
                    await SetCheckTimeToRun();
                    await RandomHelper.RandomDelay(30000, 40000); // wait for a bit before repeating farm cycle to avoid spamming 
                }
            }
            while (_clientSettings.pauseAtPokeStop);
        }

        private int GetRandomWalkspeed()
        {
            var walkspeed = (int)_clientSettings.WalkingSpeedInKilometerPerHour;
            if (_clientSettings.RandomReduceSpeed)
            {
                Random r = new Random();
                if ((int)_clientSettings.WalkingSpeedInKilometerPerHour - (int)_clientSettings.MinWalkSpeed > 1)
                {
                    walkspeed = r.Next((int)_clientSettings.MinWalkSpeed, (int)_clientSettings.WalkingSpeedInKilometerPerHour);
                }
            }
            return walkspeed;
        }

        #region Walk with Routing Functions

        private async Task WalkUserRoute(FortData[] pokeStops)
        {
            do
            {
                try
                {
                    if (_clientSettings.pauseAtPokeStop)
                    {
                        await FarmPokestopOnBreak(pokeStops, _client, true);
                    }
                    var walkspeed = GetRandomWalkspeed();
                    var pokestopCoords = _clientSettings.NextDestinationOverride.First();
                    FortData targetPokeStop = null;
                    targetPokeStop = pokeStops.Where(i =>
                i.Latitude == pokestopCoords.Latitude &&
                i.Longitude == pokestopCoords.Longitude
                ).FirstOrDefault();
                    _clientSettings.NextDestinationOverride.RemoveFirst();
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Path Override detected! Rerouting to user-selected pokeStop...");

                    await WalkWithRouting(targetPokeStop.Latitude, targetPokeStop.Longitude, walkspeed, ExecuteCatchandFarm);

                    var FortInfo = await _client.Fort.GetFort(targetPokeStop.Id, targetPokeStop.Latitude, targetPokeStop.Longitude);
                    await CheckAndFarmNearbyPokeStop(targetPokeStop, _client, FortInfo);
                }
                catch
                {
                    //do nothing for now. Just handle to prevent blowing up.
                }
            }
            while (_clientSettings.NextDestinationOverride.Count > 0);
        }

        private async Task WalkWithRouting(FortData pokeStop, double walkspeed, Func<Task> task)
        {
            if (_clientSettings.UseGoogleMapsAPI)
            {
                await DoRouteWalking(pokeStop.Latitude, pokeStop.Longitude, walkspeed, task);
            }
            else
            {
                await _navigation.HumanLikeWalking(new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
            }
        }

        private async Task WalkWithRouting(double latitude, double longitude, double walkspeed, Func<Task> task)
        {
            if (_clientSettings.UseGoogleMapsAPI)
            {
                await DoRouteWalking(latitude, longitude, walkspeed, task);
            }
            else
            {
                await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
            }
        }

        private async Task DoRouteWalking(double latitude, double longitude, double walkspeed, Func<Task> task)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Getting Google Maps Routing");
            if (_clientSettings.GoogleMapsAPIKey != null)
            {
                #region Normalize Lat Long for Google Directions Request

                //replace european "misuse" of comma :)
                var longstring = longitude.ToString().Replace(",", ".");
                var latstring = latitude.ToString().Replace(",", ".");
                var sourcelongstring = _client.CurrentLongitude.ToString().Replace(",", ".");
                var sourcelatstring = _client.CurrentLatitude.ToString().Replace(",", ".");
                #endregion

                #region Google Directions API Request
                DirectionsRequest directionsRequest = new DirectionsRequest();
                directionsRequest.ApiKey = _clientSettings.GoogleMapsAPIKey;
                directionsRequest.TravelMode = TravelMode.Walking;

                #region Set Directions Request Variables based on client settings
                if (walkspeed > 10 && walkspeed < 20)
                {
                    directionsRequest.TravelMode = TravelMode.Bicycling;
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Using Directions For Bicycling due to max speed setting > 10km/h");
                }
                if (walkspeed > 20)
                {
                    directionsRequest.TravelMode = TravelMode.Bicycling;
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Using Directions For Driving due to max speed setting > 20km/h");
                }

                if (_clientSettings.SelectedLanguage == "de")
                    directionsRequest.Language = "de";
                if (_clientSettings.SelectedLanguage == "spain")
                    directionsRequest.Language = "es";
                if (_clientSettings.SelectedLanguage == "ptBR")
                    directionsRequest.Language = "pt-BR";
                if (_clientSettings.SelectedLanguage == "tr")
                    directionsRequest.Language = "tr";
                if (_clientSettings.SelectedLanguage == "ru")
                    directionsRequest.Language = "ru";
                if (_clientSettings.SelectedLanguage == "france")
                    directionsRequest.Language = "fr";
                #endregion

                directionsRequest.Origin = sourcelatstring + "," + sourcelongstring;
                directionsRequest.Destination = latstring + "," + longstring;

                DirectionsResponse directions = GoogleMaps.Directions.Query(directionsRequest);

                #region  Process Google Directions response
                if (directions.Status == DirectionsStatusCodes.OK)
                {
                    var steps = directions.Routes.First().Legs.First().Steps;
                    var stepcount = 0;
                    foreach (var step in steps)
                    {
                        var directiontext = Helpers.Utils.HtmlRemoval.StripTagsRegexCompiled(step.HtmlInstructions);
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, directiontext);
                        var lastpoint = new Location(_client.CurrentLatitude, _client.CurrentLongitude);
                        foreach (var point in step.PolyLine.Points)
                        {
                            var distanceDelta = LocationUtils.CalculateDistanceInMeters(new GeoCoordinate(point.Latitude, point.Longitude), new GeoCoordinate(lastpoint.Latitude, lastpoint.Longitude));
                            if (distanceDelta > 10)
                            {
                                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(point.Latitude, point.Longitude), walkspeed, task, true, false);
                            }
                            lastpoint = point;
                        }
                        stepcount++;
                        if (stepcount == steps.Count())
                        {
                            //Make sure we actually made it to the pokestop! 
                            var remainingdistancetostop = LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, latitude, longitude);
                            if (remainingdistancetostop > 40)
                            {
                                var lowestspeed = 5;
                                //use client settings value for min speed if set.
                                if (_clientSettings.MinWalkSpeed != 0) { lowestspeed = _clientSettings.MinWalkSpeed; }
                                Logger.ColoredConsoleWrite(ConsoleColor.Green, "As close as google can take us, going off-road at walking speed (" + lowestspeed + ")");
                                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
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
                    var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else if (directions.Status == DirectionsStatusCodes.OVER_QUERY_LIMIT)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Over 2500 queries today! Are you botting unsafely? :)");
                    var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else if (directions.Status == DirectionsStatusCodes.NOT_FOUND)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Geocoding coords failed! Waypoint: " + latitude + "," + longitude + " Bot Location: " + _client.CurrentLatitude + "," + _client.CurrentLongitude);
                    var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Unhandled Error occurred when getting route[ STATUS:" + directions.StatusStr + " ERROR MESSAGE:" + directions.ErrorMessage + "] Using default walk method instead.");
                    var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
                }
                #endregion
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"API Key not found in Client Settings! Using default method instead.");
                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchAllNearbyPokemons);
            }
            #endregion
        }
        #endregion

        public async Task<bool> CheckAvailablePokemons(Client _client)
        {
            _infoObservable.PushClearPokemons();
            var pokeData = await DataCollector.GetFastPokeMapData(_client.CurrentLatitude, _client.CurrentLongitude);
            var toShow = new List<DataCollector.PokemonMapData>();
            if (pokeData != null)
            {
                foreach (var poke in pokeData)
                {
                    if (poke.Coordinates.Latitude.HasValue && poke.Coordinates.Longitude.HasValue)
                    {
                        toShow.Add(poke);
                    }
                }
                if (toShow.Count > 0)
                {
                    _infoObservable.PushNewPokemonLocations(toShow);
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> CheckAndFarmNearbyPokeStop(FortData pokeStop, Client _client, FortDetailsResponse fortInfo)
        {
            if (count >= 9)
            {
                await LogStatsEtc();
            }
            if (pokeStop.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds && _clientSettings.FarmPokestops)
            {
                var fortSearch = await _client.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                count++;
                var pokeStopInfo = $"{fortInfo.Name}{Environment.NewLine}Visited:{DateTime.Now.ToString("HH:mm:ss")}{Environment.NewLine}";
                if (fortSearch.ExperienceAwarded > 0)
                {
                    string egg = "/";
                    if (fortSearch.PokemonDataEgg != null)
                    {
                        egg = fortSearch.PokemonDataEgg.EggKmWalkedTarget + "km";
                    }
                    string items = "";
                    if (fortSearch.ItemsAwarded != null)
                    {
                        items = StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded);
                    }
                    failed_softban = 0;
                    _botStats.AddExperience(fortSearch.ExperienceAwarded);
                    pokeStopFarmedCount++;
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}{", Egg: " + egg}, Items: {items}", LogLevel.Info);

                    pokeStopInfo += $"{fortSearch.ExperienceAwarded} XP{Environment.NewLine}{fortSearch.GemsAwarded}{Environment.NewLine}{egg}{Environment.NewLine}{items.Replace(",", Environment.NewLine)}";

                    if (pokeStop != null && pokeStop.LureInfo != null)
                    {
                        var lure_Pokemon = pokeStop.LureInfo.ActivePokemonId;
                        if (!_clientSettings.catchPokemonSkipList.Contains(lure_Pokemon))
                        {
                            if (!lureEncounters.Contains(pokeStop.LureInfo.EncounterId.ToString()))
                            {
                                await catchPokemon(pokeStop.LureInfo.EncounterId, pokeStop.LureInfo.FortId, pokeStop.LureInfo.ActivePokemonId, pokeStop.Longitude, pokeStop.Latitude);
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
                    if (_telegram != null)
                        _telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Pokestop, fortInfo.Name, fortSearch.ExperienceAwarded, eggs, fortSearch.GemsAwarded, StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded));
                }
                _infoObservable.PushPokeStopInfoUpdate(pokeStop, pokeStopInfo);
                return true;
            }
            else if (!_clientSettings.FarmPokestops)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Farm Pokestop option unchecked, skipping and only looking for pokemon");
                return false;
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop not ready to farm again, skipping and only looking for pokemon");
                return false;
            }
        }

        private async Task ExecuteCatchandFarm()
        {
            if (_clientSettings.CatchPokemon)
                await UseIncense();
            //Get nearby pokestops
            var mapObjects = await _client.Map.GetMapObjects();
            //narrow map data to pokestops within walking distance
            var pokeStops =
            _navigation.pathByNearestNeighbour(
            mapObjects.Item1.MapCells.SelectMany(i => i.Forts)
            .Where(
                i =>
                i.Type == FortType.Checkpoint &&
                i.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
                .OrderBy(
                i =>
                LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude)).ToArray(), _clientSettings.WalkingSpeedInKilometerPerHour);
            var pokestopsWithinRangeStanding = pokeStops.Where(i => (LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude)) < 40);
            if (pokestopsWithinRangeStanding.Count() > 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{pokestopsWithinRangeStanding.Count().ToString()} Pokestops within range of user");
                foreach (var Pokestop in pokestopsWithinRangeStanding)
                {
                    var FortInfo = await _client.Fort.GetFort(Pokestop.Id, Pokestop.Latitude, Pokestop.Longitude);
                    var farmed = await CheckAndFarmNearbyPokeStop(Pokestop, _client, FortInfo);
                    if (farmed) { Pokestop.CooldownCompleteTimestampMs = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500; }
                    await SetCheckTimeToRun();
                    await RandomHelper.RandomDelay(100, 200); // wait for a bit before repeating farm cycle to avoid spamming 
                }
            }
            await ExecuteCatchAllNearbyPokemons();
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            //update location map with current bot location
            _infoObservable.PushNewGeoLocations(new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude));
            var client = _client;
            //bypass catching pokemon if disabled
            if (_clientSettings.CatchPokemon)
            {
                // identify nearby pokemon
                var mapObjects = await client.Map.GetMapObjects();
                var pokemons =
                   mapObjects.Item1.MapCells.SelectMany(i => i.CatchablePokemons)
                   .OrderBy(
                       i =>
                       LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, i.Latitude, i.Longitude));
                if (pokemons != null && pokemons.Any())
                {
                    string strNames = "";
                    foreach (var pokemon in pokemons)
                    {
                        strNames += StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId) + ", ";
                    }
                    strNames = strNames.Substring(0, strNames.Length - 2);
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s): " + strNames);
                    await ShowNearbyPokemons(pokemons).ConfigureAwait(false);
                }
                //catch them all!
                foreach (var pokemon in pokemons)
                {
                    #region Stats Log
                    //increment log stats counter and log stats
                    count++;
                    if (count >= 9)
                    {
                        await LogStatsEtc();
                    }
                    #endregion

                    //Capture only Snipe pokemon
                    if (StateSniper) { if (SnipokemonIds != pokemon.PokemonId) continue; }

                    #region Skip pokemon if in list
                    if (_clientSettings.catchPokemonSkipList.Contains(pokemon.PokemonId))
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Pokemon: " + pokemon.PokemonId);
                        continue;
                    }
                    #endregion

                    //get distance to pokemon
                    var distance = LocationUtils.CalculateDistanceInMeters(_client.CurrentLatitude, _client.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);

                    await Task.Delay(distance > 100 ? 1000 : 100);

                    // Do Catch here
                    await catchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude);
                }
            }
        }

        private async Task<bool> VerifyLocation()
        {
            #region Stay within defined radius
            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _client.CurrentLatitude, _client.CurrentLongitude);
            //walk back to default location if outside of defined radius
            if ((_clientSettings.MaxWalkingRadiusInMeters != 0 && distanceFromStart > _clientSettings.MaxWalkingRadiusInMeters) || _clientSettings.RelocateDefaultLocation)
            {
                var walkingspeed = _clientSettings.WalkingSpeedInKilometerPerHour;
                if (_clientSettings.RelocateDefaultLocation)
                {
                    if (_clientSettings.RelocateDefaultLocationTravelSpeed > 0)
                        walkingspeed = _clientSettings.RelocateDefaultLocationTravelSpeed;
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Relocating to new Default Location! Travelling at " + walkingspeed + "km/h");
                    _clientSettings.RelocateDefaultLocation = false;
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "You're outside of the defined max. walking radius. Walking back!");
                }

                await WalkWithRouting(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, walkingspeed, ExecuteCatchandFarm);

                return true;
            }
            else
            {
                return false;
            }
            #endregion
        }

        private async Task catchPokemon(ulong encounter_id, string spawnpoint_id, PokemonId pokeid, double poke_long = 0, double poke_lat = 0)
        {
            EncounterResponse encounterPokemonResponse;

            //Offset Misscount here to account for user setting.
            var missCount = 0;
            if (_clientSettings.Max_Missed_throws <= 1)
                missCount = 2;
            if (_clientSettings.Max_Missed_throws == 2)
                missCount = 1;
            var forceHit = false;
            try
            {
                if (StateSniper)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "I went to capture the pokemon...");
                    var result = await _client.Player.UpdatePlayerLocation(poke_lat, poke_long, _clientSettings.DefaultAltitude);
                }
                encounterPokemonResponse = await _client.Encounter.EncounterPokemon(encounter_id, spawnpoint_id);
            }
            finally
            {
                if (StateSniper)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "I returned before starting the capture (trick)...");
                    var result = await _client.Player.UpdatePlayerLocation(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _clientSettings.DefaultAltitude);
                }
            }
            if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
            {
                var bestPokeball = await GetBestBall(encounterPokemonResponse?.WildPokemon, false);
                if (bestPokeball == ItemId.ItemUnknown)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}%");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");
                    pokeballoutofstock = true;
                    _clientSettings.CatchPokemon = false;
                    return;
                }
                var inventoryBerries = await _client.Inventory.GetItems();
                var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();
                CatchPokemonResponse caughtPokemonResponse;
                bool escaped = false;
                bool berryThrown = false;
                bool berryOutOfStock = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Encountered {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% Probability {Math.Round(probability.Value * 100)}%");
                if (encounterPokemonResponse.WildPokemon.PokemonData.Cp > _clientSettings.MinCPtoCatch)
                {
                    bool used = false;
                    do
                    {
                        // Check if the best ball is still valid
                        if (bestPokeball == ItemId.ItemUnknown)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}%");
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");
                            pokeballoutofstock = true;
                            _clientSettings.CatchPokemon = false;
                            return;
                        }
                        if (((probability.HasValue && probability.Value < _clientSettings.razzberry_chance) || escaped) && _clientSettings.UseRazzBerry && !used)
                        {
                            var bestBerry = await GetBestBerry(encounterPokemonResponse?.WildPokemon);
                            if (bestBerry != ItemId.ItemUnknown)
                            {
                                var berries = inventoryBerries.Where(p => (ItemId)p.ItemId == bestBerry).FirstOrDefault();
                                if (berries.Count <= 0) berryOutOfStock = true;
                                if (!berryOutOfStock)
                                {
                                    //Throw berry
                                    var useRaspberry = await _client.Encounter.UseCaptureItem(encounter_id, bestBerry, spawnpoint_id);
                                    berryThrown = true;
                                    used = true;
                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Thrown {bestBerry}. Remaining: {berries.Count}.", LogLevel.Info);
                                    await RandomHelper.RandomDelay(50, 200);
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
                        //TODO eventually make the max miss count client configurable;
                        Random r = new Random();
                        switch (missCount)
                        {
                            case 0:
                                if (bestPokeball == ItemId.ItemMasterBall)
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"No messing around with your Master Balls! Forcing a hit on target.");
                                    forceHit = true;
                                }
                                break;
                            case 1:
                                if (bestPokeball == ItemId.ItemUltraBall)
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Not wasting more of your Ultra Balls! Forcing a hit on target.");
                                    forceHit = true;
                                }
                                break;
                            case 2:
                                //adding another chance of forcing hit here to improve overall odds after 2 misses                                
                                int rInt = r.Next(0, 2);
                                if (rInt == 1)
                                {
                                    // lets hit
                                    forceHit = true;
                                }
                                break;
                            default:
                                // default to force hit after 3 wasted balls of any kind.
                                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Enough misses! Forcing a hit on target.");
                                forceHit = true;
                                break;
                        }
                        if (missCount > 0)
                        {
                            //adding another chance of forcing hit here to improve overall odds after 1st miss                            
                            int rInt = r.Next(0, 3);
                            if (rInt == 1)
                            {
                                // lets hit
                                forceHit = true;
                            }
                        }
                        caughtPokemonResponse = await CatchPokemonWithRandomVariables(encounter_id, spawnpoint_id, bestPokeball, forceHit);
                        if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Missed {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} while using {bestPokeball}");
                            missCount++;
                            await RandomHelper.RandomDelay(1500, 6000);
                        }
                        else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"{StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} escaped while using {bestPokeball}");
                            escaped = true;
                            //reset forceHit in case we randomly triggered on last throw.
                            forceHit = false;
                            await RandomHelper.RandomDelay(1500, 6000);
                        }
                        // Update the best ball to ensure we can still throw
                        bestPokeball = await GetBestBall(encounterPokemonResponse?.WildPokemon, escaped);
                    }
                    while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        foreach (int xp in caughtPokemonResponse.CaptureAward.Xp)
                            _botStats.AddExperience(xp);

                        DateTime curDate = DateTime.Now;
                        _infoObservable.PushNewHuntStats(String.Format("{0}/{1};{2};{3};{4}", poke_lat, poke_long, pokeid, curDate.Ticks, curDate.ToString()) + Environment.NewLine);

                        string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        string logs = System.IO.Path.Combine(logPath, "PokeLog.txt");
                        var date = DateTime.Now;
                        if (caughtPokemonResponse.CaptureAward.Xp.Sum() >= 500)
                        {
                            if (_clientSettings.logPokemons == true)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught new {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.White,
                                $"Caught New {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% using {bestPokeball} got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP.");
                            pokemonCatchCount++;

                        }
                        else
                        {
                            if (_clientSettings.logPokemons == true)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray,
                                $"Caught {StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% using {bestPokeball} got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP.");
                            pokemonCatchCount++;

                            if (_telegram != null)
                                _telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Catch, StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid), encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00"), bestPokeball, caughtPokemonResponse.CaptureAward.Xp.Sum());

                            _botStats.AddPokemon(1);
                            await RandomHelper.RandomDelay(1500, 2000);
                        }
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{StringUtils.getPokemonNameByLanguage(_clientSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData).ToString("0.00")}% got away while using {bestPokeball}..");
                        failed_softban++;
                        if (failed_softban > 10)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Soft Ban Detected - Stopping Bot to prevent perma-ban. Try again in 4-24 hours and be more careful next time!");
                            StringUtils.CheckKillSwitch(true);
                        }
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon CP lower than Configured Min to Catch - Skipping Pokemon");
                }
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Error catching Pokemon: {encounterPokemonResponse?.Status}");
            }
            await RandomHelper.RandomDelay(1500, 2000);
        }

        private async Task<CatchPokemonResponse> CatchPokemonWithRandomVariables(ulong encounter_id, string spawnpoint_id, ItemId bestPokeball, bool forceHit)
        {
            #region Reset Function Variables
            double normalizedRecticleSize = 1.95;
            var hitTxt = "Default Perfect";
            var spinModifier = 1.0;
            var spinTxt = "Curve";
            int Pb_Excellent = _clientSettings.Pb_Excellent;
            int Pb_Great = _clientSettings.Pb_Excellent;
            int Pb_Nice = _clientSettings.Pb_Nice;
            int Pb_Ordinary = _clientSettings.Pb_Ordinary;
            var r = new Random();
            int rInt = r.Next(0, 99);
            #endregion

            #region Randomize Throw Type
            if (rInt >= 0 && rInt < Pb_Excellent)
            {
                normalizedRecticleSize = r.NextDouble() * (1.95 - 1.7) + 1.7;
                hitTxt = "Excellent";
            }
            else if (rInt >= Pb_Excellent && rInt < Pb_Excellent + Pb_Great)
            {
                normalizedRecticleSize = r.NextDouble() * (1.7 - 1.3) + 1.3;
                hitTxt = "Great";
            }
            else if (rInt >= Pb_Excellent + Pb_Great && rInt < Pb_Excellent + Pb_Great + Pb_Nice)
            {
                normalizedRecticleSize = r.NextDouble() * (1.3 - 1) + 1;
                hitTxt = "Nice";
            }
            else if (rInt >= Pb_Excellent + Pb_Great + Pb_Nice && rInt < Pb_Excellent + Pb_Great + Pb_Nice + Pb_Ordinary)
            {
                normalizedRecticleSize = r.NextDouble() * (1 - 0.1) + 0.1;
                hitTxt = "Ordinary";
            }
            else
            {
                normalizedRecticleSize = r.NextDouble() * (1 - 0.1) + 0.1;
                hitTxt = "Ordinary";
            }

            int rIntSpin = r.Next(0, 2);
            if (rIntSpin == 0)
            {
                spinModifier = 0.0;
                spinTxt = "Straight";
            }
            int rIntHit = r.Next(0, 2);
            if (rIntHit == 0)
            {
                forceHit = true;
            }
            #endregion

            //round to 2 decimals  
            normalizedRecticleSize = Math.Round(normalizedRecticleSize, 2);
            //if not miss, log throw variables
            if (forceHit) { Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"{hitTxt} throw as {spinTxt} ball."); }
            return await _client.Encounter.CatchPokemon(encounter_id, spawnpoint_id, bestPokeball, forceHit, normalizedRecticleSize, spinModifier);
        }

        #endregion

        #region Evlove Transfer Functions

        private async Task EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter = null)
        {
            var pokemonToEvolve = await _client.Inventory.GetPokemonToEvolve(filter);
            if (pokemonToEvolve.Count() != 0)
            {
                if (_clientSettings.UseLuckyEgg)
                {
                    await _client.Inventory.UseLuckyEgg(_client);
                }
            }

            if (_clientSettings.pauseAtEvolve2)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stoping to evolve some Pokemons.");
                _clientSettings.pauseTheWalking = true;
            }

            foreach (var pokemon in pokemonToEvolve)
            {

                if (!_clientSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                {
                    continue;
                }

                count++;
                if (count == 6)
                {
                    count = 0;
                    await StatsLog(_client);
                }

                var evolvePokemonOutProto = await _client.Inventory.EvolvePokemon((ulong)pokemon.Id);
                var date = DateTime.Now.ToString();
                string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string evolvelog = System.IO.Path.Combine(logPath, "EvolveLog.txt");

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    if (_clientSettings.bLogEvolve)
                    {
                        File.AppendAllText(evolvelog, $"[{date}] - Evolved {StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)} CP {pokemon.Cp} {PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00")}%  to {StringUtils.getPokemonNameByLanguage(_clientSettings, evolvePokemonOutProto.EvolvedPokemonData.PokemonId)} CP: {evolvePokemonOutProto.EvolvedPokemonData.Cp} for {evolvePokemonOutProto.ExperienceAwarded.ToString("N0")}xp");
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved {StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)} CP {pokemon.Cp} {PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00")}%  to {StringUtils.getPokemonNameByLanguage(_clientSettings, evolvePokemonOutProto.EvolvedPokemonData.PokemonId)} CP: {evolvePokemonOutProto.EvolvedPokemonData.Cp} for {evolvePokemonOutProto.ExperienceAwarded.ToString("N0")}xp", LogLevel.Info);
                    _botStats.AddExperience(evolvePokemonOutProto.ExperienceAwarded);

                    if (_telegram != null)
                        _telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Evolve, StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId), pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"), StringUtils.getPokemonNameByLanguage(_clientSettings, evolvePokemonOutProto.EvolvedPokemonData.PokemonId), evolvePokemonOutProto.EvolvedPokemonData.Cp, evolvePokemonOutProto.ExperienceAwarded.ToString("N0"));

                }
                else
                {
                    if (evolvePokemonOutProto.Result != EvolvePokemonResponse.Types.Result.Success)
                    {
                        if (_clientSettings.bLogEvolve)
                        {
                            File.AppendAllText(evolvelog, $"[{date}] - Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}");
                        }
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}", LogLevel.Info);
                    }
                }
                if (_clientSettings.UseAnimationTimes)
                {
                    await RandomHelper.RandomDelay(30000, 35000);
                }
                else
                {
                    await RandomHelper.RandomDelay(500, 600);
                }
            }
            if (_clientSettings.pauseAtEvolve2)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons evolved. Time to continue our journey!");
                _clientSettings.pauseTheWalking = false;
            }
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false, bool TransferFirstLowIV = false)
        {
            if (_clientSettings.TransferDoublePokemons)
            {
                if (_clientSettings.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stoping to transfer some Pokemons.");
                    _clientSettings.pauseTheWalking = true;
                }

                var duplicatePokemons = await _client.Inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve, TransferFirstLowIV);

                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!_clientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp >= _clientSettings.DontTransferWithCPOver || PokemonInfo.CalculatePokemonPerfection(duplicatePokemon) >= _client.Settings.ivmaxpercent)
                        {
                            continue;
                        }

                        var bestPokemonOfType = await _client.Inventory.GetHighestCPofType(duplicatePokemon);
                        var bestPokemonsCPOfType = await _client.Inventory.GetHighestCPofType2(duplicatePokemon);
                        var bestPokemonsIVOfType = await _client.Inventory.GetHighestIVofType(duplicatePokemon);

                        var transfer = await _client.Inventory.TransferPokemon(duplicatePokemon.Id);

                        string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        string logs = System.IO.Path.Combine(logPath, "TransferLog.txt");
                        string date = DateTime.Now.ToString();

                        if (TransferFirstLowIV)
                        {
                            if (_clientSettings.logManualTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best IV: {PokemonInfo.CalculatePokemonPerfection(bestPokemonsIVOfType.First()).ToString("0.00")} %)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best IV: {PokemonInfo.CalculatePokemonPerfection(bestPokemonsIVOfType.First()).ToString("0.00")} %)", LogLevel.Info);
                        }
                        else
                        {
                            if (_clientSettings.logManualTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best: {bestPokemonsCPOfType.First().Cp} CP)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} CP {duplicatePokemon.Cp} IV {PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00")} % (Best: {bestPokemonsCPOfType.First().Cp} CP)", LogLevel.Info);
                        }

                        if (_telegram != null)
                            _telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Transfer,
                            StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId), duplicatePokemon.Cp,
                            PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00"), bestPokemonOfType);

                        await RandomHelper.RandomDelay(5000, 6000);
                    }
                }
                if (_clientSettings.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons transfered. Time to continue our journey!");
                    _clientSettings.pauseTheWalking = false;
                }
            }
        }

        #endregion

        #region Best Ball and Berry Functions

        private async Task<Dictionary<string, int>> GetPokeballQty()
        {
            Dictionary<string, int> pokeBallCollection = new Dictionary<string, int>();
            var items = await _client.Inventory.GetItems();
            var balls = items.Where(i => ((ItemId)i.ItemId == ItemId.ItemPokeBall
                                      || (ItemId)i.ItemId == ItemId.ItemGreatBall
                                      || (ItemId)i.ItemId == ItemId.ItemUltraBall
                                      || (ItemId)i.ItemId == ItemId.ItemMasterBall) && i.ItemId > 0).GroupBy(i => ((ItemId)i.ItemId)).ToList();

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

        private async Task<ItemId> GetBestBall(WildPokemon pokemon, bool escaped)
        {
            //pokemon cp to determine ball type
            var pokemonCp = pokemon?.PokemonData?.Cp;
            var pokeballCollection = await GetPokeballQty();

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
                if ((pokeballqty <= _clientSettings.InventoryBasePokeball || _clientSettings.InventoryBasePokeball == 0) && _clientSettings.LimitPokeballUse)
                {
                    pokeBalls = false;
                }
            }
            if (pokeballCollection.ContainsKey("greatBalls"))
            {
                greatBalls = true;
                if ((greatballqty <= _clientSettings.InventoryBaseGreatball || _clientSettings.InventoryBaseGreatball == 0) && _clientSettings.LimitGreatballUse)
                {
                    greatBalls = false;
                }
            }

            if (pokeballCollection.ContainsKey("ultraBalls"))
            {
                ultraBalls = true;
                if ((ultraballqty <= _clientSettings.InventoryBaseUltraball || _clientSettings.InventoryBaseUltraball == 0) && _clientSettings.LimitUltraballUse)
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

            var _lowestAppropriateBall = ItemId.ItemUnknown;
                        
            var minCPforGreatBall = 500;
            var minCPforUltraBall = 1000;

            if (_clientSettings.MinCPforGreatBall > 0 &&
                _clientSettings.MinCPforUltraBall > 0 &&
                _clientSettings.MinCPforGreatBall < _clientSettings.MinCPforUltraBall)
            {
                minCPforGreatBall = _clientSettings.MinCPforGreatBall;
                minCPforUltraBall = _clientSettings.MinCPforUltraBall;
            }

            var getMyLowestAppropriateBall = new Dictionary<Func<int?, bool>, Action>
            {
                { x => x < minCPforGreatBall  ,() => _lowestAppropriateBall = ItemId.ItemPokeBall   },
                { x => x < minCPforUltraBall ,() => _lowestAppropriateBall = ItemId.ItemGreatBall  },
                { x => x < 2000 ,() => _lowestAppropriateBall = ItemId.ItemUltraBall  },
                { x => x >= 2000,() => _lowestAppropriateBall = ItemId.ItemMasterBall  }
            };
            getMyLowestAppropriateBall.First(sw => sw.Key(pokemonCp)).Value();
            //use next best ball if pokemon has escped before
            if (escaped)
            {
                switch (_lowestAppropriateBall)
                {
                    case ItemId.ItemGreatBall:
                        {
                            _lowestAppropriateBall = ItemId.ItemUltraBall;
                            break;
                        }
                    case ItemId.ItemUltraBall:
                        {
                            _lowestAppropriateBall = ItemId.ItemMasterBall;
                            break;
                        }
                    case ItemId.ItemMasterBall:
                        {
                            _lowestAppropriateBall = ItemId.ItemMasterBall;
                            break;
                        }
                    default:
                        {
                            _lowestAppropriateBall = ItemId.ItemGreatBall;
                            break;
                        }
                }
            }
            //handle appropriate ball out of stock
            switch (_lowestAppropriateBall)
            {
                case ItemId.ItemGreatBall:
                    {
                        if (greatBalls) return ItemId.ItemGreatBall;
                        else if (ultraBalls) return ItemId.ItemUltraBall;
                        else if (masterBalls) return ItemId.ItemMasterBall;
                        else if (pokeBalls) return ItemId.ItemPokeBall;
                        else return ItemId.ItemUnknown;
                    }
                case ItemId.ItemUltraBall:
                    {
                        if (ultraBalls) return ItemId.ItemUltraBall;
                        else if (masterBalls) return ItemId.ItemMasterBall;
                        else if (greatBalls) return ItemId.ItemGreatBall;
                        else if (pokeBalls) return ItemId.ItemPokeBall;
                        else return ItemId.ItemUnknown;
                    }
                case ItemId.ItemMasterBall:
                    {
                        if (masterBalls) return ItemId.ItemMasterBall;
                        else if (ultraBalls) return ItemId.ItemUltraBall;
                        else if (greatBalls) return ItemId.ItemGreatBall;
                        else if (pokeBalls) return ItemId.ItemPokeBall;
                        else return ItemId.ItemUnknown;
                    }
                default:
                    {
                        if (pokeBalls) return ItemId.ItemPokeBall;
                        else if (greatBalls) return ItemId.ItemGreatBall;
                        else if (ultraBalls) return ItemId.ItemUltraBall;
                        else if (pokeBalls) return ItemId.ItemMasterBall;
                        else return ItemId.ItemUnknown;
                    }
            }
            #endregion
        }

        private async Task<ItemId> GetBestBerry(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await _client.Inventory.GetItems();
            var berries = items.Where(i => (ItemId)i.ItemId == ItemId.ItemRazzBerry
                                        || (ItemId)i.ItemId == ItemId.ItemBlukBerry
                                        || (ItemId)i.ItemId == ItemId.ItemNanabBerry
                                        || (ItemId)i.ItemId == ItemId.ItemWeparBerry
                                        || (ItemId)i.ItemId == ItemId.ItemPinapBerry).GroupBy(i => (ItemId)i.ItemId).ToList();
            if (berries.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Berrys to select! - Using next best ball instead", LogLevel.Info);
                return ItemId.ItemUnknown;
            }

            var razzBerryCount = await _client.Inventory.GetItemAmountByType(ItemId.ItemRazzBerry);
            var blukBerryCount = await _client.Inventory.GetItemAmountByType(ItemId.ItemBlukBerry);
            var nanabBerryCount = await _client.Inventory.GetItemAmountByType(ItemId.ItemNanabBerry);
            var weparBerryCount = await _client.Inventory.GetItemAmountByType(ItemId.ItemWeparBerry);
            var pinapBerryCount = await _client.Inventory.GetItemAmountByType(ItemId.ItemPinapBerry);

            if (pinapBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemPinapBerry;
            else if (weparBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemWeparBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemNanabBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return ItemId.ItemBlukBerry;

            if (weparBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemWeparBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemNanabBerry;
            else if (blukBerryCount > 0 && pokemonCp >= 1500)
                return ItemId.ItemBlukBerry;

            if (nanabBerryCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemNanabBerry;
            else if (blukBerryCount > 0 && pokemonCp >= 1000)
                return ItemId.ItemBlukBerry;

            if (blukBerryCount > 0 && pokemonCp >= 500)
                return ItemId.ItemBlukBerry;

            return berries.OrderBy(g => g.Key).First().Key;
        }
        #endregion

        #region Recycle and Incense Functions

        private async Task RecycleItems(bool forcerefresh = false)
        {
            var items = await _client.Inventory.GetItemsToRecycle(_clientSettings);

            foreach (var item in items)
            {
                if ((item.ItemId == ItemId.ItemPokeBall || item.ItemId == ItemId.ItemGreatBall || item.ItemId == ItemId.ItemUltraBall || item.ItemId == ItemId.ItemMasterBall) && pokeballoutofstock)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");
                    _clientSettings.CatchPokemon = true;
                    pokeballoutofstock = false;
                }
                var transfer = await _client.Inventory.RecycleItem((ItemId)item.ItemId, item.Count);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(ItemId)item.ItemId}", LogLevel.Info);
                await RandomHelper.RandomDelay(1000, 5000);
            }
        }

        DateTime lastincenseuse;
        public async Task UseIncense()
        {
            if (_clientSettings.UseIncense || _clientSettings.UseIncenseGUIClick)
            {
                _clientSettings.UseIncenseGUIClick = false;
                var inventory = await _client.Inventory.GetItems();
                var incsense = inventory.Where(p => (ItemId)p.ItemId == ItemId.ItemIncenseOrdinary).FirstOrDefault();
                TimeSpan loginterval = lastIncenselog - DateTime.Now;
                if (lastincenseuse > DateTime.Now.AddSeconds(5) && loginterval.Seconds >= 60)
                {
                    TimeSpan duration = lastincenseuse - DateTime.Now;
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Incense still running: {duration.Minutes}m{duration.Seconds}s");
                    return;
                }
                if (incsense == null || incsense.Count <= 0) { return; }

                await _client.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                await Task.Delay(3000);
            }
        }

        #endregion

        #region Incubator Functions
        private async Task StartIncubation()
        {
            try
            {
                await _client.Inventory.RefreshCachedInventory(); // REFRESH
                var incubators = (await _client.Inventory.GetEggIncubators()).ToList();
                var unusedEggs = (await _client.Inventory.GetEggs()).Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart).ToList();
                var pokemons = (await _client.Inventory.GetPokemons()).ToList();

                var playerStats = await _client.Inventory.GetPlayerStats();
                var stats = playerStats.First();

                var kmWalked = stats.KmWalked;

                var rememberedIncubatorsFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\Configs", "incubators.json");
                var rememberedIncubators = GetRememberedIncubators(rememberedIncubatorsFilePath);

                string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string logs = System.IO.Path.Combine(logPath, "EggLog.txt");
                string date = DateTime.Now.ToString();

                foreach (var incubator in rememberedIncubators)
                {
                    var hatched = pokemons.FirstOrDefault(x => !x.IsEgg && x.Id == incubator.PokemonId);
                    if (hatched == null) continue;

                    //Hier diggi
                    if (_clientSettings.logEggs)
                    {
                        File.AppendAllText(logs, $"[{date}] - Egg hatched and we got a {hatched.PokemonId} (CP: {hatched.Cp} | MaxCP: {PokemonInfo.CalculateMaxCP(hatched)} | Level: {PokemonInfo.GetLevel(hatched)} | IV: {PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00")}% )" + Environment.NewLine);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg hatched and we got a " + hatched.PokemonId + " CP: " + hatched.Cp + " MaxCP: " + PokemonInfo.CalculateMaxCP(hatched) + " Level: " + PokemonInfo.GetLevel(hatched) + " IV: " + PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00") + "%");
                }

                var newRememberedIncubators = new List<IncubatorUsage>();

                foreach (var incubator in incubators)
                {
                    if (incubator.PokemonId == 0)
                    {
                        // Unlimited incubators prefer short eggs, limited incubators prefer long eggs
                        // Special case: If only one incubator is available at all, it will prefer long eggs
                        var egg = (incubator.ItemId == ItemId.ItemIncubatorBasicUnlimited && incubators.Count > 1)
                            ? unusedEggs.FirstOrDefault()
                            : unusedEggs.LastOrDefault();
                        
                        // When you're down to your last incubator, prefer 2km then 10km then 5km so you
                        // can clean out smaller eggs first.
                        if(incubator.ItemId == ItemId.ItemIncubatorBasicUnlimited && incubators.Count == 1 && egg.EggKmWalkedTarget < 10) {
                            egg = unusedEggs.FirstOrDefault();
                        }

                        if (egg == null)
                            continue;

                        if (egg.EggKmWalkedTarget < 5 && incubator.ItemId != ItemId.ItemIncubatorBasicUnlimited)
                            continue;

                        var response = await _client.Inventory.UseItemEggIncubator(incubator.Id, egg.Id);
                        unusedEggs.Remove(egg);

                        newRememberedIncubators.Add(new IncubatorUsage { IncubatorId = incubator.Id, PokemonId = egg.Id });

                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Added Egg which needs " + egg.EggKmWalkedTarget + "km");
                        // We need some sleep here or this shit explodes
                        await RandomHelper.RandomDelay(100, 200);
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
                Logger.Error(e.StackTrace.ToString());
            }
        }
        private static List<IncubatorUsage> GetRememberedIncubators(string filePath)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

            if (File.Exists(filePath))
                return JsonConvert.DeserializeObject<List<IncubatorUsage>>(File.ReadAllText(filePath, Encoding.UTF8));

            return new List<IncubatorUsage>(0);
        }

        private static void SaveRememberedIncubators(List<IncubatorUsage> incubators, string filePath)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

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

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (int i = 0; i < repeat; i++)
            {
                await action();
            }
        }

        private double _distance(double Lat1, double Lng1, double Lat2, double Lng2)
        {
            double r_earth = 6378137;
            double d_lat = (Lat2 - Lat1) * Math.PI / 180;
            double d_lon = (Lng2 - Lng1) * Math.PI / 180;
            double alpha = Math.Sin(d_lat / 2) * Math.Sin(d_lat / 2)
                + Math.Cos(Lat1 * Math.PI / 180) * Math.Cos(Lat2 * Math.PI / 180)
                * Math.Sin(d_lon / 2) * Math.Sin(d_lon / 2);
            double d = 2 * r_earth * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            return d;
        }
        #endregion
        public async Task<bool> ShowNearbyPokemons( IEnumerable<MapPokemon>  pokeData )
        {
            _infoObservable.PushClearPokemons();
            var toShow = new List<DataCollector.PokemonMapData>();
            if (pokeData != null)
            {
                foreach (var poke in pokeData)
                {
                	var poke2 = new  DataCollector.PokemonMapData();
                	poke2.Id = poke.SpawnPointId;
                	poke2.PokemonId = poke.PokemonId;
                	poke2.Coordinates = new PokemonGo.RocketApi.PokeMap.DataModel.LatitudeLongitude ();
                	poke2.Coordinates.Coordinates = new List<double>();
                	poke2.Coordinates.Coordinates.Add(poke.Longitude);
                	poke2.Coordinates.Coordinates.Add(poke.Latitude);
                	
                	poke2.ExpiresAt = new DateTime(poke.ExpirationTimestampMs * 10000).AddYears(1969).AddDays(-1);
                    toShow.Add(poke2);                    
                }
                if (toShow.Count > 0)
                {
                    _infoObservable.PushNewPokemonLocations(toShow);
                }

                return true;
            }
            else
            {
                return false;
            }
        }        
    }
}
