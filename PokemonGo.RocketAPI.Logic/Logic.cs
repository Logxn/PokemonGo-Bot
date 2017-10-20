using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using POGOProtos.Data.Player;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic;
using PokeMaster.Logic.Functions;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using Google.Protobuf.Collections;
using PokemonGo.RocketAPI.Shared;
using System.Net.Http;
using System.Text;

namespace PokeMaster.Logic
{
    public class Logic
    {

        #region Members and Constructor

        public static Client objClient;
        public readonly ISettings BotSettings;
        public TelegramUtil Telegram;
        public BotStats BotStats;
        public readonly Navigation navigation;
        public const double SpeedDownTo = 10 / 3.6;
        public readonly LogicInfoObservable infoObservable;
        private readonly PokeVisionUtil pokevision;
        public bool pokeballoutofstock;
        public static Logic Instance;
        private readonly List<string> lureEncounters = new List<string>();
        public static int FailedSoftban = 0;
        public double lastsearchtimestamp = 0;


        public string Lure = "lureId";
        private bool addedlure = false;
        public Sniper sniperLogic;
        public static bool restartLogic = true;

        public TutorialFunctions Tutorial = new TutorialFunctions();

        #endregion

        #region Constructor
        public Logic(ISettings botSettings, LogicInfoObservable infoObservable)
        {
            this.BotSettings = botSettings;
            var clientSettings = new PokemonGo.RocketAPI.Shared.ClientSettings(botSettings.pFHashKey, botSettings.DefaultLatitude, botSettings.DefaultLongitude, botSettings.DefaultAltitude,
                      botSettings.proxySettings.hostName, botSettings.proxySettings.port, botSettings.proxySettings.username, botSettings.proxySettings.password,
                      botSettings.AuthType, botSettings.Username, botSettings.Password, botSettings.LastTimePlayedTs, GlobalVars.BotApiSupportedVersion);
            objClient = new Client(clientSettings);
            objClient.setFailure(new ApiFailureStrat(objClient));
            objClient.EvMakeTutorial += MakeTutorial;
            LocaleInfo.SetValues(GlobalVars.LocaleCountry, GlobalVars.LocaleLanguage, GlobalVars.LocaleTimeZone, GlobalVars.LocaleLanguage + "-" + GlobalVars.LocaleCountry);
            BotStats = new BotStats();
            navigation = new Navigation(objClient, botSettings);
            pokevision = new PokeVisionUtil();
            this.infoObservable = infoObservable;
            Instance = this;
            sniperLogic = new Sniper(objClient, botSettings);
            PokemonGo.RocketAPI.Shared.KeyCollection.Load();

            #region Set Session values
            Setout.sessionStart = DateTime.UtcNow;
            Setout.pokemonCatchCount = 0;
            Setout.pokeStopFarmedCount = 0;
            if (GlobalVars.ContinueLatestSession)
            {
                Setout.LoadSession();
                Logger.Info("Last Session Loaded");
                Logger.Info($"Session Start: {Setout.sessionStart}");
                Logger.Info($"Pokemon Catch Count: {Setout.pokemonCatchCount}");
                Logger.Info($"Pokestop Farmed Count: {Setout.pokeStopFarmedCount}");
            }
            else
            {
                Logger.Info($"Session Started");
            }
            Logger.Info($"Session Start: {Setout.sessionStart}");
            Logger.Info($"Pokemon Catch Count: {Setout.pokemonCatchCount}");
            Logger.Info($"Pokestop Farmed Count: {Setout.pokeStopFarmedCount}");
            #endregion
        }
        #endregion

        #region Workflow

        #region Start and run the Bot
        public void Execute()
        {
            Logger.SelectedLevel = LogLevel.Error;
            Logger.Warning("Source code and binary files of this bot are absolutely free and open-source!");
            Logger.Warning("If you've paid for it. Request a chargeback immediately!");
            Logger.Warning("You only need pay for a key to access to Hash Service");

            if (GlobalVars.Debug.VerboseMode)
            {
                Logger.SelectedLevel = LogLevel.Debug;
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"LogLevel set to {Logger.SelectedLevel}. Many logs will be generated.");
            }


            // Quarthy PID
           // StringUtils.PidToFile(BotSettings);

            // Here we set which server to be used for the hashing service
            Logger.Info("Using " + Resources.Api.HashServerInfo.ID + " for Hash Key service.");

            #region Log Logger

            Logger.Info($"Starting Execute on login server: {BotSettings.AuthType}");

            if (BotSettings.LogPokemons)
            {
                Logger.Info("Pokemon Log will be saved to " + GlobalVars.FileForPokemonsCaught);
            }

            if (BotSettings.LogTransfer)
            {
                Logger.Info("Transfer Log will be saved to " + GlobalVars.FileForTransfers);
            }

            if (BotSettings.LogEvolve)
            {
                Logger.Info("Evolution Log will be saved to " + GlobalVars.FileForEvolve);
            }
            #endregion

            objClient.CurrentAltitude = BotSettings.DefaultAltitude;
            objClient.CurrentLongitude = BotSettings.DefaultLongitude;
            objClient.CurrentLatitude = BotSettings.DefaultLatitude;


            #region Fix Altitude

            if (Math.Abs(objClient.CurrentAltitude) < 0.001)
            {
                objClient.CurrentAltitude = LocationUtils.GetAltitude(objClient.CurrentLatitude, objClient.CurrentLongitude);
                BotSettings.DefaultAltitude = objClient.CurrentAltitude;

                Logger.Warning($"Altitude was 0, resolved that. New Altitude is now: {objClient.CurrentAltitude}");
            }

            #endregion

            #region Use Proxy

            if (BotSettings.proxySettings.enabled)
            {
                Logger.Error("===============================================");
                Logger.Error("Proxy enabled.");
                Logger.Error($"ProxyIP: { BotSettings.proxySettings.hostName }:{BotSettings.proxySettings.port}");
                Logger.Error("===============================================");
            }

            #endregion

            #region Login & Start
            //Restart unless killswitch thrown
            while (restartLogic)
            {
                try
                {
                    objClient.Login.DoLogin().Wait();
                    Logger.Debug("login done");

                    TelegramLogic.Instantiante();
                    DiscordLogic.Init();

                    /*********************************************************/
                    // BreakSettings Timer
                    BreakSettings AdvancedBreaks = new BreakSettings();
                    AdvancedBreaks.CheckEnabled(BotSettings);
                    /********************************************************/
                    PostLoginExecute();

                    Logger.Info("All Pokestops in range was already visited.");
                }
                catch (PtcOfflineException)
                {
                    Logger.Error("PTC Servers are probably down.");
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.Flatten().InnerExceptions)
                    {
                        if (e is LoginFailedException)
                        {
                            Logger.Error(e.Message);
                            Logger.Error("Exiting in 10 Seconds.");
                            RandomHelper.RandomSleep(10000, 10001);
                            Environment.Exit(0);
                        }
                        else if (e is GoogleException)
                        {
                            Logger.Error("Login Failed. Your credentials are wrong or Google Account is banned.");
                            Logger.Error("Exiting in 10 Seconds.");
                            RandomHelper.RandomSleep(10000, 10001);
                            Environment.Exit(0);
                        }
                        else if (e is AccountNotVerifiedException)
                        {
                            Logger.Error("Your PTC Account is not activated.");
                            Logger.Error("Exiting in 10 Seconds.");
                            RandomHelper.RandomSleep(10000, 10001);
                            Environment.Exit(0);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    #region Log Error 
                    Exception realerror = ex;
                    while (realerror.InnerException != null)
                        realerror = realerror.InnerException;
                    Logger.ExceptionInfo(ex.Message + "/" + realerror + "/" + ex.GetType());
                    #endregion
                }

                TelegramLogic.Stop();
                DiscordLogic.Finish();
                const int msToWait = 50000;
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Restarting in over {(msToWait + 5000) / 1000} Seconds.");
                RandomHelper.RandomSleep(msToWait, msToWait + 10000);
            }
            #endregion
        }
        #endregion

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
            while (BotSettings.pauseAtPokeStop)
            {
                foreach (var pokestop in pokestopsWithinRangeStanding)
                {

                    if (BotSettings.RelocateDefaultLocation) break;

                    CatchingLogic.Execute();

                    var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;

                    if (BotSettings.UseLureGUIClick || (BotSettings.UseLureAtBreak && !pokestop.ActiveFortModifier.Any() && !addedlure))
                    {
                        if (objClient.Inventory.GetItemAmountByType(ItemId.ItemTroyDisk) > 0)
                        {
                            BotSettings.UseLureGUIClick = false;

                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Adding lure and setting resume walking to 30 minutes");

                            objClient.Fort.AddFortModifier(fortInfo.FortId, ItemId.ItemTroyDisk);

                            Setout.resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 30000;
                            addedlure = true;
                        }
                    }

                    var farmed = CheckAndFarmNearbyPokeStop(pokestop, objClient, fortInfo);
                    if (farmed)
                    {
                        pokestop.CooldownCompleteTimestampMs = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500;
                    }

                    Setout.SetCheckTimeToRun();

                    RandomHelper.RandomSleep(30000, 40000);

                    // wait for a bit before repeating farm cycle to avoid spamming 
                }

                if (!BotSettings.RelocateDefaultLocation) continue;

                Setout.resumetimestamp = -10000;
                BotSettings.pauseAtPokeStop = false;

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Exit Command detected - Ending break");
            }
        }

        private int GetRandomWalkspeed()
        {
            BotSettings.WalkingSpeedInKilometerPerHour = GlobalVars.WalkingSpeedInKilometerPerHour;
            BotSettings.MinWalkSpeed = GlobalVars.MinWalkSpeed;

            var walkspeed = (int)BotSettings.WalkingSpeedInKilometerPerHour;
            if (!BotSettings.RandomReduceSpeed) return walkspeed;

            var randomWalkSpeed = new Random();
            if ((int)BotSettings.WalkingSpeedInKilometerPerHour - BotSettings.MinWalkSpeed > 1)
            {
                walkspeed = randomWalkSpeed.Next((int)BotSettings.MinWalkSpeed,
                    (int)BotSettings.WalkingSpeedInKilometerPerHour + 1);
            }
            Logger.Debug($"WalkingSpeedInKilometerPerHour: {BotSettings.WalkingSpeedInKilometerPerHour}, MinWalkSpeed: {BotSettings.MinWalkSpeed}, walkspeed: {walkspeed}");
            return walkspeed;
        }

        #region Execute Functions
        
        public void PostLoginExecute()
        {
            try
            {
                //update user location on map
                Task.Factory.StartNew(() => Logic.Instance.infoObservable.PushNewGeoLocations(new GeoCoordinate(GlobalVars.latitude, GlobalVars.longitude)));
                GetPlayerResponse profil = objClient.Player.GetPlayer();
                objClient.Inventory.ExportPokemonToCSV(profil.PlayerData);
                Setout.Execute();
                while (GlobalVars.pauseAtPokeStop)
                {
                    RandomHelper.RandomDelay(2000).Wait();
                }
                ExecuteFarmingPokestopsAndPokemons(objClient);
            }
            catch (AccessTokenExpiredException)
            {
                throw new AccessTokenExpiredException();
            }
            catch (InvalidPlatformException)
            {
                throw new InvalidPlatformException();
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception: {ex}");

                if (BotSettings.RelocateDefaultLocation)
                {
                    Logger.Info("Detected User Request to Relocate to a new farming spot!");
                }
            }
        }

        #endregion

        #region Catch, Farm and Walk Logic


        #region Archimedean Spiral

        private void Espiral(Client client, FortData[] pokeStops, int MaxWalkingRadiusInMeters)
        {
            CatchingLogic.Execute();

            Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Starting Archimedean spiral");

            var i2 = 0;
            var salir = true;
            var cantidadvar = 0.0001;
            double recorrido = MaxWalkingRadiusInMeters;

            pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude) <= BotSettings.MaxWalkingRadiusInMeters).ToArray();

            var centerx = objClient.CurrentLatitude;
            var centery = objClient.CurrentLongitude;

            if (recorrido <= 100) cantidadvar = 0.00008;
            if (recorrido > 100 && recorrido <= 500) cantidadvar = 0.00009;
            if (recorrido > 500 && recorrido <= 1000) cantidadvar = 0.0001;
            if (recorrido > 1000) cantidadvar = 0.0002;

            while (salir)
            {
                if (BotSettings.RelocateDefaultLocation) break;

                var angle = 0.3 * i2;
                var xx = centerx + cantidadvar * angle * Math.Cos(angle);
                var yy = centery + cantidadvar * angle * Math.Sin(angle);
                var distancia = Navigation.DistanceBetween2Coordinates(centerx, centery, xx, yy);

                if (distancia > recorrido)
                {
                    salir = false;

                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Returning to the starting point...");

                    navigation.HumanLikeWalking(new GeoCoordinate(BotSettings.DefaultLatitude, BotSettings.DefaultLongitude), BotSettings.WalkingSpeedInKilometerPerHour, CatchingLogic.Execute);

                    break;
                }

                if (i2 % 10 == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Distance from starting point: " + distancia + " metros...");
                }

                navigation.HumanLikeWalking(
                    new GeoCoordinate(xx, yy),
                    BotSettings.WalkingSpeedInKilometerPerHour,
                    CatchingLogic.Execute);

                Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Looking PokeStops who are less than 30 meters...");

                FncPokeStop(client, pokeStops, true);

                i2++;
            }
        }

        #endregion

        private void ExecuteFarmingPokestopsAndPokemons(Client client)
        {

            #region Check and report
            var verifiedLocation = VerifyLocation();
            var pokeStops = GetNearbyPokeStops();
            var tries = 3;
            do
            {
                // make sure we found pokestops and log if none found
                if (BotSettings.MaxWalkingRadiusInMeters != 0)
                {
                    if (tries < 3)
                    {
                        RandomHelper.RandomSleep(5000, 6000);
                        pokeStops = GetNearbyPokeStops();
                    }

                    pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(BotSettings.DefaultLatitude, BotSettings.DefaultLongitude, i.Latitude, i.Longitude) <= BotSettings.MaxWalkingRadiusInMeters).ToArray();

                    if (!pokeStops.Any())
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops in a range of " + BotSettings.MaxWalkingRadiusInMeters + "m!");
                    }
                }

                if (!pokeStops.Any())
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "We can't find any PokeStops, which are unused! Probably the server are unstable, or you visted them all. Retrying..");
                    tries--;
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, pokeStops.Count() + " usable PokeStops found near your current location.");
                    tries = 0;
                }
            } while (tries > 0);

            #endregion

            #region Start Walk

            // Walk Spiral if enabled
            if (BotSettings.Espiral)
            {
                Espiral(client, pokeStops, BotSettings.MaxWalkingRadiusInMeters);

                return;
            }

            //Normal Walk and Catch between pokestops
            FncPokeStop(objClient, pokeStops, false);

            #endregion
        }

        private FortData[] GetNearbyPokeStops(bool updateMap = true, GetMapObjectsResponse mapObjectsResponse = null)
        {

            //Query nearby objects for mapData
            if (mapObjectsResponse == null)
                mapObjectsResponse = objClient.Map.GetMapObjects().Result;

            //narrow map data to pokestops within walking distance

            var unixNow = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

            var pokeStops = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < unixNow);

            IEnumerable<FortData> pokeGyms = new List<FortData>();

            if (GlobalVars.Gyms.Farm || GlobalVars.Gyms.Spin)
                pokeGyms = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                 .Where(i => i.Type == FortType.Gym && i.CooldownCompleteTimestampMs < unixNow);

            var both = pokeStops.Concat(pokeGyms)
                .OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude));
                
            FortData[] inRange = null;

            if (GlobalVars.SkipRadius)
                inRange = both.ToArray();
            else
                inRange = both
                    .Where(i => LocationUtils.CalculateDistanceInMeters(GlobalVars.latitude, GlobalVars.longitude, i.Latitude, i.Longitude) <= GlobalVars.radius)
                    .ToArray();

            var forts = Navigation.pathByNearestNeighbour(inRange);

            if (updateMap)
            {
                if (pokeStops.Any())
                    Task.Factory.StartNew(() => infoObservable.PushAvailablePokeStopLocations(pokeStops.ToArray()));
                if (pokeGyms.Any())
                    Task.Factory.StartNew(() => infoObservable.PushAvailablePokeGymsLocations(pokeGyms.ToArray()));
            }

            return forts;
        }

        private Queue<FortData> RefreshNearbyPokeStops(Queue<FortData> fortQueue)
        {
            FortData[] newPokeStops = GetNearbyPokeStops();

            if (newPokeStops.Any())
            {
                var rnd = new Random();
                var pokeStops = GlobalVars.WalkRandomly ? newPokeStops.OrderBy(x => rnd.Next()).ToArray() : newPokeStops;

                var pokestopsQueue = new Queue<FortData>();
                foreach (var pokeStop in pokeStops)
                    pokestopsQueue.Enqueue(pokeStop);

                if (GlobalVars.WalkInLoop)
                    for (var i = pokeStops.Length - 2; i > 0; i--)
                        pokestopsQueue.Enqueue(pokeStops[i]);

                return pokestopsQueue;
            }
            else return fortQueue;
        }

        private void FncPokeStop(Client client, FortData[] pokeStopsIn, bool metros30)
        {
            int PokeStopVisitCounter = 0;

            var distanceFromStart = LocationUtils
                .CalculateDistanceInMeters(
                    BotSettings.DefaultLatitude,
                    BotSettings.DefaultLongitude,
                    objClient.CurrentLatitude,
                    objClient.CurrentLongitude);

            lureEncounters.Clear();

            var rnd = new Random();
            var pokeStops = GlobalVars.WalkRandomly ? pokeStopsIn.OrderBy(x => rnd.Next()).ToArray() : pokeStopsIn;

            var pokestopsQueue = new Queue<FortData>();
            foreach (var pokeStop in pokeStops)
                pokestopsQueue.Enqueue(pokeStop);


            if (GlobalVars.WalkInLoop)
                for (var i = pokeStops.Length - 2; i > 0; i--)
                    pokestopsQueue.Enqueue(pokeStops[i]);

            //walk between pokestops in default collection
            while (pokestopsQueue.Any())
            {
                var pokeStop = pokestopsQueue.Dequeue();
                if (GlobalVars.WalkInLoop)
                    pokestopsQueue.Enqueue(pokeStop);

                //make sure user defined limits have not been reached
                Setout.SetCheckTimeToRun();

                #region Walk defined Route

                if (GlobalVars.NextDestinationOverride.Count > 0)
                {
                    try
                    {
                        do
                        {
                            WalkUserRoute(pokeStops);

                            #region Check for Exit Command

                            if (BotSettings.RelocateDefaultLocation)
                            {
                                break;
                            }

                            #endregion

                            if (!BotSettings.RepeatUserRoute) continue;

                            foreach (var geocoord in GlobalVars.RouteToRepeat)
                            {
                                GlobalVars.NextDestinationOverride.AddLast(geocoord);
                            }
                        } while (BotSettings.RepeatUserRoute);
                    }
                    catch (Exception e)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                        Logger.AddLog(string.Format("Error in Walk Defined Route: " + e));
                    }
                }

                #endregion

                #region Check for Exit Command 


                if (BotSettings.RelocateDefaultLocation)
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

                FortDetailsResponse fortInfo = objClient.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;

                //log error if pokestop not found
                if (fortInfo == null)
                {
                    Task.Factory.StartNew(() => infoObservable.PushPokeStopInfoUpdate(pokeStop, "!!Can't Get PokeStop Information!!"));
                    continue;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop is {fortInfo.Name} at {distance:0.##}m. ({fortInfo.Longitude}:{fortInfo.Latitude}) [{PokeStopVisitCounter}]");

                #region Break At Lure Logic  

                //check if user wants to break at lured pokestop
                if (BotSettings.BreakAtLure && fortInfo.Modifiers.Any())
                {
                    Setout.pausetimestamp = -10000;
                    Setout.resumetimestamp = fortInfo.Modifiers.First().ExpirationTimestampMs;
                    var timeRemaining = Setout.resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Active Lure at next Pokestop - Pausing walk for " + Math.Round(timeRemaining / 60 / 1000, 2) + " Minutes");

                    BotSettings.pauseAtPokeStop = true;
                }

                #endregion

                try
                {
                    WalkWithRouting(pokeStop.Latitude, pokeStop.Longitude);
                }
                catch (Exception e)
                {
                    Logger.ExceptionInfo(string.Format("Error in Walk Default Route: " + e));
                }

                // Pause and farm nearby pokestops
                if (BotSettings.pauseAtPokeStop)
                {
                    FarmPokestopOnBreak(pokeStops, client);
                }

                PokeStopVisitCounter++;
                if ((PokeStopVisitCounter % 10) == 0)
                {
                    pokestopsQueue = RefreshNearbyPokeStops(pokestopsQueue);
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

                if (BotSettings.RelocateDefaultLocation)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Relocate Command Detected - Clearing User Defined Route");

                    GlobalVars.NextDestinationOverride.Clear();
                    GlobalVars.RouteToRepeat.Clear();
                    BotSettings.RepeatUserRoute = false;

                    break;
                }

                #endregion

                try
                {
                    if (BotSettings.pauseAtPokeStop)
                    {
                        FarmPokestopOnBreak(pokeStops, objClient);
                    }

                    var pokestopCoords = GlobalVars.NextDestinationOverride.First();
                    GlobalVars.NextDestinationOverride.RemoveFirst();

                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Path Override detected! Rerouting to user-selected pokeStop...");

                    WalkWithRouting(pokestopCoords.Latitude, pokestopCoords.Longitude);
                }
                catch (Exception ex1)
                {
                    Logger.ExceptionInfo(ex1.ToString());
                }
            } while (GlobalVars.NextDestinationOverride.Count > 0);
        }

        public void WalkWithRouting(double latitude, double longitude)
        {
            if (BotSettings.UseGoogleMapsAPI)
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

            if (BotSettings.GoogleMapsAPIKey != null)
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
                    ApiKey = BotSettings.GoogleMapsAPIKey,
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

                if (BotSettings.SelectedLanguage == "de")
                    directionsRequest.Language = "de";
                if (BotSettings.SelectedLanguage == "spain")
                    directionsRequest.Language = "es";
                if (BotSettings.SelectedLanguage == "ptBR")
                    directionsRequest.Language = "pt-BR";
                if (BotSettings.SelectedLanguage == "tr")
                    directionsRequest.Language = "tr";
                if (BotSettings.SelectedLanguage == "ru")
                    directionsRequest.Language = "ru";
                if (BotSettings.SelectedLanguage == "france")
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

                        if (BotSettings.RelocateDefaultLocation)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Exiting Navigation to Relocate");
                            break;
                        }

                        #endregion

                        var directiontext = PokemonGo.RocketAPI.Helpers.Utils.HtmlRemoval.StripTagsRegexCompiled(step.HtmlInstructions);
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, directiontext);
                        var lastpoint = new Location(objClient.CurrentLatitude, objClient.CurrentLongitude);
                        foreach (var point in step.PolyLine.Points)
                        {
                            var distanceDelta = LocationUtils.CalculateDistanceInMeters(new GeoCoordinate(point.Latitude, point.Longitude), new GeoCoordinate(lastpoint.Latitude, lastpoint.Longitude));
                            if (distanceDelta > 10)
                            {
                                navigation.HumanLikeWalking(new GeoCoordinate(point.Latitude, point.Longitude), walkspeed, ExecuteCatchandFarm, true, false);
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
                                if (BotSettings.MinWalkSpeed != 0)
                                {
                                    lowestspeed = (int)BotSettings.MinWalkSpeed;
                                }
                                Logger.ColoredConsoleWrite(ConsoleColor.Green, "As close as google can take us, going off-road at walking speed (" + lowestspeed + ")");
                                navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, ExecuteCatchandFarm);
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
                    navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, CatchingLogic.Execute);
                }
                else if (directions.Status == DirectionsStatusCodes.OVER_QUERY_LIMIT)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Over 2500 queries today! Are you botting unsafely? :)");
                    navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, CatchingLogic.Execute);
                }
                else if (directions.Status == DirectionsStatusCodes.NOT_FOUND)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Geocoding coords failed! Waypoint: " + latitude + "," + longitude + " Bot Location: " + objClient.CurrentLatitude + "," + objClient.CurrentLongitude);
                    navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, CatchingLogic.Execute);
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Unhandled Error occurred when getting route[ STATUS:" + directions.StatusStr + " ERROR MESSAGE:" + directions.ErrorMessage + "] Using default walk method instead.");
                    navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, CatchingLogic.Execute);
                }

                #endregion
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"API Key not found in Client Settings! Using default method instead.");
                navigation.HumanLikeWalking(new GeoCoordinate(latitude, longitude), walkspeed, CatchingLogic.Execute);
            }

            #endregion
        }


        private bool CheckAndFarmNearbyPokeStop(FortData pokeStop, Client client, FortDetailsResponse fortInfo)
        {
            if (Setout.count >= 9)
            {
                Setout.Execute();
            }

            if (pokeStop.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds && BotSettings.FarmPokestops)
            {
                var fortSearch = objClient.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;
                //Logger.Debug("================[VERBOSE LOGGING - Pokestop Search]================");
                //Logger.Debug($"Result: {fortSearch.Result}");
                //Logger.Debug($"ChainHackSequenceNumber: {fortSearch.ChainHackSequenceNumber}");
                //Logger.Debug($"Cooldown Complete (MS): {fortSearch.CooldownCompleteTimestampMs}");
                //Logger.Debug($"EXP Award: {fortSearch.ExperienceAwarded}");
                //Logger.Debug($"Gems Award: {fortSearch.GemsAwarded}");
                //Logger.Debug($"Item Award: {fortSearch.ItemsAwarded}");
                //Logger.Debug($"Egg Data: {fortSearch.PokemonDataEgg}");
                //Logger.Debug("==================================================================");

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

                Setout.count++;
                var strDate = DateTime.Now.ToString("HH:mm:ss");
                var pokeStopInfo = $"{fortInfo.Name}{Environment.NewLine}Visited:{strDate}{Environment.NewLine}";

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
                                break;
                            }
                        }

                        if (logrestock && pokeballoutofstock)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");

                            CatchingLogic.AllowCatchPokemon = true;
                            pokeballoutofstock = false;
                        }

                        FailedSoftban = 0;
                        BotStats.AddExperience(fortSearch.ExperienceAwarded);
                        Setout.RefreshConsoleTitle(client);
                        Setout.pokeStopFarmedCount++;
                        Setout.SaveSession();

                        Logger.Info($"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Egg: {egg}, Items: {items}");

                        var strItems = items.Replace(",", Environment.NewLine);
                        pokeStopInfo += $"{fortSearch.ExperienceAwarded} XP{Environment.NewLine}{fortSearch.GemsAwarded}{Environment.NewLine}{egg}{Environment.NewLine}{strItems}";

                        Logger.Debug("LureInfo: " + pokeStop.LureInfo);
                        if (pokeStop.LureInfo != null)
                        {
                            var pokedata = new MapPokemon();
                            pokedata.EncounterId = pokeStop.LureInfo.EncounterId;
                            pokedata.PokemonId = pokeStop.LureInfo.ActivePokemonId;
                            pokedata.Latitude = pokeStop.Latitude;
                            pokedata.Longitude = pokeStop.Longitude;
                            pokedata.ExpirationTimestampMs = pokeStop.LureInfo.LureExpiresTimestampMs;
                            pokedata.SpawnPointId = pokeStop.LureInfo.FortId;

                            infoObservable.PushNewPokemonLocation(pokedata);
                            Logger.Debug("Lured Pokemon: " + pokedata);

                            if (!BotSettings.catchPokemonSkipList.Contains(pokedata.PokemonId) && GlobalVars.CatchPokemon)
                            {
                                if (!lureEncounters.Contains(pokedata.EncounterId.ToString()))
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Catching Lured Pokemon");
                                    CatchingLogic.CatchLuredPokemon(pokedata.EncounterId, pokedata.SpawnPointId, pokedata.PokemonId, pokedata.Longitude, pokedata.Latitude);

                                    lureEncounters.Add(pokedata.EncounterId.ToString());
                                }
                                else
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Lure Pokemon: " + pokedata.PokemonId + " because we have already caught him, or catching pokemon is disabled");
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

                Task.Factory.StartNew(() => infoObservable.PushPokeStopInfoUpdate(pokeStop, pokeStopInfo));

                return true;
            }

            if (!BotSettings.FarmPokestops)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Farm Pokestop option unchecked, skipping and only looking for pokemon");

                return false;
            }

            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop not ready to farm again, skipping and only looking for pokemon");

            return false;
        }

        public bool runGymLogic = true;

        public bool ExecuteCatchandFarm()
        {
            if (BotSettings.RelocateDefaultLocation)
            {
                return false;
            }

            if (GlobalVars.ForceReloginClick)
            {
                GlobalVars.ForceReloginClick = false;
                Logger.Info("Forcing Relogin");
                RandomHelper.RandomDelay(1000).Wait();
                objClient.Login.DoLogin().Wait();
            }

            // Do NOT update GMO unless 10 seconds have passed
            if ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds > lastsearchtimestamp + 10000)
            {
                lastsearchtimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                var mapObjectsResponse = objClient.Map.GetMapObjects().Result;
                if (mapObjectsResponse == null)
                    return false;
                //narrow map data to pokestops within walking distance
                var pokeStops = GetNearbyPokeStops(false, mapObjectsResponse);
                var pokestopsWithinRangeStanding = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude) < 30);

                var withinRangeStandingList = pokestopsWithinRangeStanding as IList<FortData> ?? pokestopsWithinRangeStanding.ToList();
                if (withinRangeStandingList.Any())
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"{withinRangeStandingList.Count} Pokestops within range of user");

                    foreach (var pokestop in withinRangeStandingList)
                    {
                        if (!GlobalVars.Gyms.Spin)
                            if (pokestop.Type != FortType.Checkpoint)
                                continue;

                        if (pokestop.Type == FortType.Gym)
                        {
                            //Simulate entering into the gym.
                            var gymdet = objClient.Fort.GymGetInfo(pokestop.Id, pokestop.Latitude, pokestop.Longitude);
                            Logger.Info("Spinning Gym: " + gymdet.Name);
                        }

                        Setout.RecycleItems();
                        var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;
                        var farmed = CheckAndFarmNearbyPokeStop(pokestop, objClient, fortInfo);

                        if (farmed)
                        {
                            pokestop.CooldownCompleteTimestampMs = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500;
                            MarkFirstExperiencie();
                        }

                        Setout.SetCheckTimeToRun();
                        RandomHelper.RandomSleep(500, 600); // Time between pokestops
                    }
                }
                CatchingLogic.Execute(mapObjectsResponse);

                if (runGymLogic)
                {
                    GymsLogic.Execute();
                    GymTutorial();
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private bool VerifyLocation()
        {
            #region Stay within defined radius

            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(BotSettings.DefaultLatitude, BotSettings.DefaultLongitude, objClient.CurrentLatitude, objClient.CurrentLongitude);

            //walk back to default location if outside of defined radius
            if ((BotSettings.MaxWalkingRadiusInMeters == 0 ||
                !(distanceFromStart > BotSettings.MaxWalkingRadiusInMeters)) &&
                !BotSettings.RelocateDefaultLocation)
            {
                return false;
            }

            var walkingspeed = BotSettings.WalkingSpeedInKilometerPerHour;

            if (BotSettings.RelocateDefaultLocation)
            {
                if (BotSettings.RelocateDefaultLocationTravelSpeed > 0)
                {
                    walkingspeed = BotSettings.RelocateDefaultLocationTravelSpeed;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Relocating to new Default Location! Travelling at " + walkingspeed + "km/h");

                BotSettings.RelocateDefaultLocation = false;
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "You're outside of the defined max. walking radius. Walking back!");
            }

            WalkWithRouting(BotSettings.DefaultLatitude, BotSettings.DefaultLongitude);

            return true;

            #endregion
        }

        #endregion

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

        #region Handle Tutorial Calls
        // This happens when starting a new player
        private void MakeTutorial(object sender, EventArgs eventArgs)
        {
            //LegalScreen = 0,
            //AvatarSelection = 1,
            //AccountCreation = 2,
            //PokemonCapture = 3,
            //NameSelection = 4,
            //PokemonBerry = 5,
            //UseItem = 6,
            //FirstTimeExperienceComplete = 7,
            //PokestopTutorial = 8,
            //GymTutorial = 9
            /*
             * On first connection before any move you must do 0,1,3,4 & 7
             *
             */

            if (!GlobalVars.CompleteTutorial)
                return;

            var state = objClient.Player.PlayerResponse.PlayerData.TutorialState;
            AvatarSettings.Load(); // we need it to AvatarSelection, NameSelection and PokemonCapture
            /* 0 */ if (!state.Contains(TutorialState.LegalScreen)) Tutorial.MarkTutorialAsDone(TutorialState.LegalScreen, objClient);
            /* 1 */ if (!state.Contains(TutorialState.AvatarSelection)) Tutorial.MarkTutorialAsDone(TutorialState.AvatarSelection, objClient);
            /* 3 */ if (!state.Contains(TutorialState.PokemonCapture)) Tutorial.MarkTutorialAsDone(TutorialState.PokemonCapture, objClient);
            /* 4 */ if (!state.Contains(TutorialState.NameSelection)) Tutorial.MarkTutorialAsDone(TutorialState.NameSelection, objClient);
            /* 5 */ //if (!state.Contains(TutorialState.PokemonBerry)) Tutorial.MarkTutorialAsDone(TutorialState.PokemonBerry, objClient);
            /* 6 */ //if (!state.Contains(TutorialState.UseItem)) Tutorial.MarkTutorialAsDone(TutorialState.UseItem, objClient);
            /* 7 */ if (!state.Contains(TutorialState.FirstTimeExperienceComplete)) Tutorial.MarkTutorialAsDone(TutorialState.FirstTimeExperienceComplete, objClient);
            RandomHelper.RandomDelay(2000).Wait();
        }

        // This happens after first pokestop
        private void MarkFirstExperiencie()
        {
            if (!GlobalVars.CompleteTutorial)
                return;

            var state = objClient.Player.PlayerResponse.PlayerData.TutorialState;

            /* 8 */ if (!state.Contains(TutorialState.PokestopTutorial)) Tutorial.MarkTutorialAsDone(TutorialState.PokestopTutorial, objClient);
        }

        // This happens after first gym
        private void GymTutorial()
        {
            if (!GlobalVars.CompleteTutorial)
                return;

            var state = objClient.Player.PlayerResponse.PlayerData.TutorialState;

            /* 9 */ if (!state.Contains(TutorialState.GymTutorial)) Tutorial.MarkTutorialAsDone(TutorialState.GymTutorial, objClient);
        }
        #endregion

        #endregion
    }
}