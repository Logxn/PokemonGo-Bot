using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Directions.Request;
using GoogleMapsApi.Entities.Directions.Response;
using POGOProtos.Data;
using POGOProtos.Data.Logs;
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
using System.IO;
using System.Text;
using POGOProtos.Map.Pokemon;
using PokemonGo.RocketAPI.Logic.Functions;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Logic.Shared;
using PokemonGo.RocketAPI.HttpClient;
using System.Net.Http.Headers;
using System.Net.Http;

namespace PokemonGo.RocketAPI.Logic
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
        private bool stopsloaded;
        public static Logic Instance;
        private readonly List<string> lureEncounters = new List<string>();
        public static int FailedSoftban;
        public List<ulong> SkippedPokemon = new List<ulong>();
        public double lastsearchtimestamp;
        public bool logicAllowCatchPokemon = true;
        
        public string Lure = "lureId";
        private bool addedlure = false;
        public Sniper sniperLogic;
        public static bool restartLogic =true;
        #endregion

        #region Constructor
        public Logic(ISettings botSettings, LogicInfoObservable infoObservable)
        {
            this.BotSettings = botSettings;
            var clientSettings = new PokemonGo.RocketAPI.Shared.ClientSettings(botSettings.pFHashKey, botSettings.DefaultLatitude , botSettings.DefaultLongitude, botSettings.DefaultAltitude,
                      botSettings.proxySettings.hostName, botSettings.proxySettings.port, botSettings.proxySettings.username, botSettings.proxySettings.password,
                      botSettings.AuthType, botSettings.Username, botSettings.Password, GlobalVars.BotApiSupportedVersion);
            objClient = new Client(clientSettings);
            objClient.setFailure(new ApiFailureStrat(objClient));
            BotStats = new BotStats();
            navigation = new Navigation(objClient,botSettings);
            pokevision = new PokeVisionUtil();
            this.infoObservable = infoObservable;
            Instance = this;
            sniperLogic = new  Sniper(objClient, botSettings);
            PokemonGo.RocketAPI.Shared.KeyCollection.Load();
        }
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

                    if (BotSettings.RelocateDefaultLocation) break;

                    ExecuteCatchAllNearbyPokemons();

                    var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;

                    if ((BotSettings.UseLureGUIClick && Setout.havelures) || (BotSettings.UseLureAtBreak && Setout.havelures && !pokestop.ActiveFortModifier.Any() && !addedlure))
                    {
                        BotSettings.UseLureGUIClick = false;

                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Adding lure and setting resume walking to 30 minutes");

                        objClient.Fort.AddFortModifier(fortInfo.FortId, ItemId.ItemTroyDisk).Wait();

                        Setout.resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 30000;
                        addedlure = true;
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
            } while (BotSettings.pauseAtPokeStop);
        }

        private int GetRandomWalkspeed()
        {
            var walkspeed = (int)BotSettings.WalkingSpeedInKilometerPerHour;
            if (!BotSettings.RandomReduceSpeed) return walkspeed;

            var randomWalkSpeed = new Random();
            if ((int)BotSettings.WalkingSpeedInKilometerPerHour - BotSettings.MinWalkSpeed > 1)
            {
                walkspeed = randomWalkSpeed.Next(BotSettings.MinWalkSpeed,
                    (int)BotSettings.WalkingSpeedInKilometerPerHour);
            }
            return walkspeed;
        }

        #region Execute Functions

        public void Execute()
        {
            Logger.SelectedLevel = LogLevel.Error;
            Logger.Warning( "Source code and binary files of this bot are absolutely free and open-source!");
            Logger.Warning( "If you've paid for it. Request a chargeback immediately!");
            Logger.Warning( "You only need pay for a key to access to Hash Service");

            if (GlobalVars.EnableVerboseLogging)
            {
                Logger.SelectedLevel = LogLevel.Debug;
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"LogLevel set to {Logger.SelectedLevel}. Many logs will be generated.");
            }

            #region Log Logger

            Logger.Info($"Starting Execute on login server: {BotSettings.AuthType}");

            if (BotSettings.LogPokemons)
            {
                Logger.Info( "You enabled Pokemonlogging. It will be saved to \"\\Logs\\PokeLog.txt\"");
            }

            if (BotSettings.LogTransfer)
            {
                Logger.Info( "You enabled manual transfer logging. It will be saved to \"\\Logs\\TransferLog.txt\"");
            }

            if (BotSettings.LogEvolve)
            {
                Logger.Info( "You enabled Evolution Logging. It will be saved to \"\\Logs\\EvolutionLog.txt\"");
            }

            #endregion

            #region Set Counters and Location

            Logger.Info( "Setting Pokemon Catch Count: to 0 for this session");

            Setout.pokemonCatchCount = 0;

            Logger.Info( "Setting Pokestop Farmed Count to 0 for this session");

            Setout.pokeStopFarmedCount = 0;

            objClient.CurrentAltitude = BotSettings.DefaultAltitude;
            objClient.CurrentLongitude = BotSettings.DefaultLongitude;
            objClient.CurrentLatitude = BotSettings.DefaultLatitude;

            #endregion

            #region Fix Altitude

            if (Math.Abs(objClient.CurrentAltitude) < double.Epsilon) {
                objClient.CurrentAltitude = LocationUtils.getAltitude(objClient.CurrentLatitude, objClient.CurrentLongitude);
                BotSettings.DefaultAltitude = objClient.CurrentAltitude;

                Logger.Warning($"Altitude was 0, resolved that. New Altitude is now: {objClient.CurrentAltitude}");
            }

            #endregion

            #region Use Proxy

            if (BotSettings.proxySettings.enabled)
            {
                Logger.Error("===============================================");
                Logger.Error("Proxy enabled.");
                Logger.Error($"ProxyIP: { BotSettings.proxySettings.username }:{BotSettings.proxySettings.password}");
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
                    
                    TelegramLogic.Instantiante();
                    
                    PostLoginExecute();

                    Logger.Info("All Pokestops in range was already visited.");
                }
                catch (PtcOfflineException)
                {
                    Logger.Error( "PTC Servers are probably down.");
                }
                catch (AggregateException ae) {
                   foreach (var e in ae.Flatten().InnerExceptions) {
                      if (e is LoginFailedException) {
                        Logger.Error( "Login Failed. Your credentials are wrong or PTC Account is banned.");
                        Logger.Error( "Exiting in 10 Seconds.");
                        RandomHelper.RandomSleep(10000,10001);
                        Environment.Exit(0);
                      } else if (e is GoogleException) {
                        Logger.Error( "Login Failed. Your credentials are wrong or Google Account is banned.");
                        Logger.Error( "Exiting in 10 Seconds.");
                        RandomHelper.RandomSleep(10000,10001);
                        Environment.Exit(0);
                      } else if (e is AccountNotVerifiedException) {
                        Logger.Error( "Your PTC Account is not activated.");
                        Logger.Error( "Exiting in 10 Seconds.");
                        RandomHelper.RandomSleep(10000,10001);
                        Environment.Exit(0);
                      }else {
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
                    Logger.ExceptionInfo(ex.Message+"/"+realerror+"/"+ex.GetType());
                    #endregion
                }

                TelegramLogic.Stop();
                var msToWait = 50000;
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Restarting in over {(msToWait+5000)/1000} Seconds.");
                RandomHelper.RandomSleep(msToWait,msToWait+10000);
            }
            #endregion
        }

        public void PostLoginExecute()
        {
            try
            {
                //update user location on map
                Task.Factory.StartNew(() => Logic.Instance.infoObservable.PushNewGeoLocations(new GeoCoordinate(GlobalVars.latitude, GlobalVars.longitude)));
                var profil = objClient.Player.GetPlayer().Result;
                objClient.Inventory.ExportPokemonToCSV(profil.PlayerData).Wait();
                Setout.Execute();
                ExecuteFarmingPokestopsAndPokemons(objClient);
            }
            catch (AccessTokenExpiredException)
            {
                throw new AccessTokenExpiredException();
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception: {ex}");

                if (BotSettings.RelocateDefaultLocation)
                {
                    Logger.Info( "Detected User Request to Relocate to a new farming spot!");
                }
            }
        }

        #endregion


        #region Catch, Farm and Walk Logic


        #region Archimedean Spiral

        private void Espiral(Client client, FortData[] pokeStops , int MaxWalkingRadiusInMeters)
        {
            //Intento de pajarera 1...
            ExecuteCatchAllNearbyPokemons();

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
                if ( BotSettings.RelocateDefaultLocation) break;

                var angle = 0.3 * i2;
                var xx = centerx + cantidadvar * angle * Math.Cos(angle);
                var yy = centery + cantidadvar * angle * Math.Sin(angle);
                var distancia = Navigation.DistanceBetween2Coordinates(centerx, centery, xx, yy);

                if (distancia > recorrido)
                {
                    salir = false;

                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Returning to the starting point...");

                    var update = navigation.HumanLikeWalking(new GeoCoordinate(BotSettings.DefaultLatitude, BotSettings.DefaultLongitude), BotSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);

                    break;
                }

                if (i2 % 10 == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Blue, "Distance from starting point: " + distancia + " metros...");
                }

                navigation.HumanLikeWalking(
                    new GeoCoordinate(xx, yy),
                    BotSettings.WalkingSpeedInKilometerPerHour,
                    ExecuteCatchAllNearbyPokemons);

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
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " usable PokeStops near your current location.");
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
                mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;

            //narrow map data to pokestops within walking distance
            
            var unixNow = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            
            var pokeStops = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < unixNow);

            var pokeGyms = mapObjectsResponse.MapCells.SelectMany(i => i.Forts)
                .Where(i => i.Type == FortType.Gym && i.CooldownCompleteTimestampMs < unixNow);

            if (!GlobalVars.FarmGyms)
                pokeGyms = new List<FortData>();

            var both = pokeStops.Concat(pokeGyms)
                .OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude));

            var forts = navigation.pathByNearestNeighbour(both.ToArray(), BotSettings.WalkingSpeedInKilometerPerHour);

            if (updateMap) {
                if (pokeStops.Any())
                    Task.Factory.StartNew(() => infoObservable.PushAvailablePokeStopLocations(pokeStops.ToArray()));
                if (pokeGyms.Any())
                    Task.Factory.StartNew(() => infoObservable.PushAvailablePokeGymsLocations(pokeGyms.ToArray()));
                stopsloaded = true;
            }

            return forts;
        }
        
        
        private void FncPokeStop(Client client, FortData[] pokeStopsIn, bool metros30)
        {
            var distanceFromStart = LocationUtils
                .CalculateDistanceInMeters(
                    BotSettings.DefaultLatitude,
                    BotSettings.DefaultLongitude,
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
                /*
                //check if map has pokestops loaded and load if not
                if (BotSettings.MapLoaded && !stopsloaded)
                {
                    Task.Factory.StartNew(() => infoObservable.PushAvailablePokeStopLocations(pokeStops));
                    stopsloaded = true;
                }
                */

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

                var fortInfo = objClient.Fort.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;

                //log error if pokestop not found
                if (fortInfo == null)
                {
                    Task.Factory.StartNew(() =>infoObservable.PushPokeStopInfoUpdate(pokeStop, "!!Can't Get PokeStop Information!!"));
                    continue;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");

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
                                if (BotSettings.MinWalkSpeed != 0)
                                {
                                    lowestspeed = BotSettings.MinWalkSpeed;
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


        private bool CheckAndFarmNearbyPokeStop(FortData pokeStop, Client client, FortDetailsResponse fortInfo)
        {
            if (Setout.count >= 9)
            {
                Setout.Execute();
            }

            if (pokeStop.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds && BotSettings.FarmPokestops)
            {
                var fortSearch = objClient.Fort.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude).Result;
                Logger.Debug("================[VERBOSE LOGGING - Pokestop Search]================");
                Logger.Debug($"Result: {fortSearch.Result}");
                Logger.Debug($"ChainHackSequenceNumber: {fortSearch.ChainHackSequenceNumber}");
                Logger.Debug($"Cooldown Complete (MS): {fortSearch.CooldownCompleteTimestampMs}");
                Logger.Debug($"EXP Award: {fortSearch.ExperienceAwarded}");
                Logger.Debug($"Gems Award: {fortSearch.GemsAwarded}");
                Logger.Debug($"Item Award: {fortSearch.ItemsAwarded}");
                Logger.Debug($"Egg Data: {fortSearch.PokemonDataEgg}");
                Logger.Debug("==================================================================");

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
                            }
                        }

                        if (logrestock && pokeballoutofstock)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");

                            logicAllowCatchPokemon = true;
                            pokeballoutofstock = false;
                        }

                        FailedSoftban = 0;
                        BotStats.AddExperience(fortSearch.ExperienceAwarded);
                        Setout.RefreshConsoleTitle(client);
                        Setout.pokeStopFarmedCount++;

                        Logger.Info($"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Egg: {egg}, Items: {items}");

                        var strItems = items.Replace(",", Environment.NewLine);
                        pokeStopInfo += $"{fortSearch.ExperienceAwarded} XP{Environment.NewLine}{fortSearch.GemsAwarded}{Environment.NewLine}{egg}{Environment.NewLine}{strItems}";

                        Logger.Debug("LureInfo: " +pokeStop.LureInfo);
                        if (pokeStop.LureInfo != null)
                        {
                            var pokedata = new MapPokemon ();
                            pokedata.EncounterId = pokeStop.LureInfo.EncounterId;
                            pokedata.PokemonId = pokeStop.LureInfo.ActivePokemonId;
                            pokedata.Latitude = pokeStop.Latitude;
                            pokedata.Longitude = pokeStop.Longitude;
                            pokedata.ExpirationTimestampMs = pokeStop.LureInfo.LureExpiresTimestampMs;
                            pokedata.SpawnPointId = pokeStop.LureInfo.FortId;
                            
                            infoObservable.PushNewPokemonLocation(pokedata);
                            Logger.Debug("Lured Pokemon: " +pokedata);

                            if (!BotSettings.catchPokemonSkipList.Contains(pokedata.PokemonId))
                            {
                                if (!lureEncounters.Contains(pokedata.EncounterId.ToString()))
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Catching Lured Pokemon");
                                    CatchLuredPokemon(pokedata.EncounterId,pokedata.SpawnPointId, pokedata.PokemonId, pokedata.Longitude, pokedata.Latitude);

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

                Task.Factory.StartNew(() =>infoObservable.PushPokeStopInfoUpdate(pokeStop, pokeStopInfo));

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

        private bool ExecuteCatchandFarm()
        {
            if ( BotSettings.RelocateDefaultLocation)
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
                        if (!GlobalVars.SpinGyms)
                            if (pokestop.Type != FortType.Checkpoint )
                                continue;

                        if (pokestop.Type == FortType.Gym )
                           Logger.Info("Spinning Gym");

                        var fortInfo = objClient.Fort.GetFort(pokestop.Id, pokestop.Latitude, pokestop.Longitude).Result;
                        var farmed = CheckAndFarmNearbyPokeStop(pokestop, objClient, fortInfo);
                        /* 
                        if (GlobalVars.TelegramName=="Magic Insert"){
                            var buddy = objClient.Player.GetPlayer().Result.PlayerData.BuddyPokemon.Id;
                            var pokemons = objClient.Inventory.GetPokemons().Result;
                            Functions.GymsLogic.putInPokestop(objClient,buddy,pokestop,pokemons);
                        }*/

                        if (farmed)
                        {
                            pokestop.CooldownCompleteTimestampMs = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + 300500;
                        }

                        Setout.SetCheckTimeToRun();
                        RandomHelper.RandomSleep(500, 600); // Time between pokestops
                    }
                }
                ExecuteCatchAllNearbyPokemons(mapObjectsResponse);
                
                GymsLogic.Execute();
                    
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ExecuteCatchAllNearbyPokemons()
        {
            return ExecuteCatchAllNearbyPokemons(null);
        }

        private bool ExecuteCatchAllNearbyPokemons(GetMapObjectsResponse mapObjectsResponse )
        {

            var client = objClient;
            
            //bypass catching pokemon if disabled
            if (BotSettings.CatchPokemon && logicAllowCatchPokemon )
            {
                if (mapObjectsResponse == null)
                {
                    mapObjectsResponse = objClient.Map.GetMapObjects().Result.Item1;
                }

                MapPokemon mapIncensePokemon = null;
                try {
                    var duration = Setout.lastincenseuse - DateTime.Now;
                    Logger.Debug("duration: "+ duration);
                    if (duration.TotalMilliseconds >0 ){
                        var incensePokemon= client.Map.GetIncensePokemons().Result;
                        Logger.Debug("incensePokemon: "+ incensePokemon);
                        if (incensePokemon.Result == GetIncensePokemonResponse.Types.Result.IncenseEncounterAvailable)
                        {
                            mapIncensePokemon = new MapPokemon();
                            mapIncensePokemon.EncounterId =incensePokemon.EncounterId;
                            mapIncensePokemon.Longitude = incensePokemon.Longitude;
                            mapIncensePokemon.Latitude = incensePokemon.Latitude;
                            mapIncensePokemon.PokemonDisplay = incensePokemon.PokemonDisplay;
                            mapIncensePokemon.PokemonId = incensePokemon.PokemonId;
                            mapIncensePokemon.SpawnPointId = incensePokemon.EncounterLocation;
                            mapIncensePokemon.ExpirationTimestampMs = incensePokemon.DisappearTimestampMs;
        
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found incensed Pokemon: {mapIncensePokemon.PokemonId}"  );
                            if (GlobalVars.ShowPokemons){
                                infoObservable.PushNewPokemonLocation(mapIncensePokemon);
                            }
                        }else
                            Logger.Debug("incensePokemon.Result: "+ incensePokemon.Result);
                    }

                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
                
                if (mapIncensePokemon!=null)
                    if (!BotSettings.catchPokemonSkipList.Contains(mapIncensePokemon.PokemonId))
                        CatchIncensedPokemon(mapIncensePokemon.EncounterId, mapIncensePokemon.SpawnPointId, mapIncensePokemon.PokemonId, mapIncensePokemon.Longitude, mapIncensePokemon.Latitude);                

                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons).OrderBy(i => LocationUtils.CalculateDistanceInMeters(objClient.CurrentLatitude, objClient.CurrentLongitude, i.Latitude, i.Longitude));

                Logger.Debug( $"Pokemons Catchable: {pokemons.Count()}");

                if (pokemons.Any())
                {
                    var strNames = pokemons.Aggregate("", (current, pokemon) => current + ( pokemon.PokemonId + ", "));
                    strNames = strNames.Substring(0, strNames.Length - 2);

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s): " + strNames);
                    if (GlobalVars.ShowPokemons){
                        ShowNearbyPokemons(pokemons);
                    }
                }else
                    return false;


                //catch them all!
                foreach (var pokemon in pokemons)
                {
                    #region Stats Log

                    //increment log stats counter and log stats
                    Setout.count++;

                    if (Setout.count >= 9 )
                    {
                        Setout.Execute();
                    }

                    #endregion


                    #region Skip pokemon if in list

                    if (BotSettings.catchPokemonSkipList.Contains(pokemon.PokemonId))
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
                client.Map.GetMapObjects(true).Wait(); //force Map Objects Update
                client.Inventory.GetInventory(true).Wait(); //force Inventory Update
                return true;
            }
            return false;
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

        public ulong CatchIncensedPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong, double pokeLat)
        {
            return CatchPokemon(encounterId,spawnpointId, pokeid,pokeLong,pokeLat,false,-1,-1,2);
        }

        public ulong CatchLuredPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong, double pokeLat)
        {
            return CatchPokemon(encounterId,spawnpointId, pokeid,pokeLong,pokeLat,false,-1,-1,1);
        }

        public static EncounterResponse.Types.Status DiskEncounterResultToEncounterStatus( DiskEncounterResponse.Types.Result diskEncounter)
        {
            switch (diskEncounter) {
                case DiskEncounterResponse.Types.Result.Unknown :
                    return EncounterResponse.Types.Status.EncounterError;
                case DiskEncounterResponse.Types.Result.Success :
                    return EncounterResponse.Types.Status.EncounterSuccess;
                case DiskEncounterResponse.Types.Result.NotAvailable  :
                    return EncounterResponse.Types.Status.EncounterNotFound ;
                case DiskEncounterResponse.Types.Result.NotInRange  :
                    return EncounterResponse.Types.Status.EncounterNotInRange ;
                case DiskEncounterResponse.Types.Result.EncounterAlreadyFinished  :
                    return EncounterResponse.Types.Status.EncounterAlreadyHappened ;
                case DiskEncounterResponse.Types.Result.PokemonInventoryFull:
                    return EncounterResponse.Types.Status.PokemonInventoryFull;
            }
            return EncounterResponse.Types.Status.EncounterError;
        }
        public static EncounterResponse.Types.Status IncenseEncounterResultToEncounterStatus( IncenseEncounterResponse.Types.Result incenseEncounter)
        {
            switch (incenseEncounter) {
                case IncenseEncounterResponse.Types.Result.IncenseEncounterUnknown :
                    return EncounterResponse.Types.Status.EncounterError;
                case IncenseEncounterResponse.Types.Result.IncenseEncounterSuccess :
                    return EncounterResponse.Types.Status.EncounterSuccess;
                case IncenseEncounterResponse.Types.Result.IncenseEncounterNotAvailable :
                    return EncounterResponse.Types.Status.EncounterNotFound ;
                case IncenseEncounterResponse.Types.Result.PokemonInventoryFull:
                    return EncounterResponse.Types.Status.PokemonInventoryFull;
            }
            return EncounterResponse.Types.Status.EncounterError;
        }

        public ulong CatchPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong = 0, double pokeLat = 0, bool goBack = false, double returnLatitude = -1, double returnLongitude = -1, int luredPoke = 0)
        {
            ulong ret = 0;
            EncounterResponse encounterPokemonResponse;

            //Offset Miss count here to account for user setting.
            var missCount = 0;

            if (BotSettings.Max_Missed_throws <= 1)
            {
                missCount = 2;
            }

            if (BotSettings.Max_Missed_throws == 2)
            {
                missCount = 1;
            }

            var forceHit = false;

            try
            {
                if (luredPoke == 0)
                    encounterPokemonResponse = objClient.Encounter.EncounterPokemon(encounterId, spawnpointId).Result;
                else if (luredPoke == 1){
                    var DiscEncounterPokemonResponse =  objClient.Encounter.EncounterLurePokemon(encounterId, spawnpointId).Result;
                    encounterPokemonResponse = new EncounterResponse();
                    encounterPokemonResponse.Status =DiskEncounterResultToEncounterStatus(DiscEncounterPokemonResponse.Result);
                    
                    if( DiscEncounterPokemonResponse.Result == DiskEncounterResponse.Types.Result.Success ){
                        encounterPokemonResponse.WildPokemon = new WildPokemon();
                        encounterPokemonResponse.WildPokemon.EncounterId = encounterId;
                        encounterPokemonResponse.WildPokemon.PokemonData = DiscEncounterPokemonResponse.PokemonData;
                        encounterPokemonResponse.CaptureProbability = new POGOProtos.Data.Capture.CaptureProbability();
                        encounterPokemonResponse.CaptureProbability.CaptureProbability_.Add(1.0F);
                    }
                    
                }else{
                    var IncenseEncounterPokemonResponse =  objClient.Encounter.EncounterIncensePokemon(encounterId, spawnpointId).Result;
                    encounterPokemonResponse = new EncounterResponse();
                    encounterPokemonResponse.Status =IncenseEncounterResultToEncounterStatus(IncenseEncounterPokemonResponse.Result);
                    
                    if( IncenseEncounterPokemonResponse.Result == IncenseEncounterResponse.Types.Result.IncenseEncounterSuccess ){
                        encounterPokemonResponse.WildPokemon = new WildPokemon();
                        encounterPokemonResponse.WildPokemon.EncounterId = encounterId;
                        encounterPokemonResponse.WildPokemon.PokemonData =IncenseEncounterPokemonResponse.PokemonData;
                        encounterPokemonResponse.CaptureProbability = IncenseEncounterPokemonResponse.CaptureProbability;
                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Error: Logic.cs - CatchPokemon - encounter: {ex.Message}");
                return ret;
            }
            finally
            {
                if (goBack)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"(SNIPING) Go to {returnLatitude} / {returnLongitude} before starting the capture.");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan,LocationUtils.FindAddress(returnLatitude, returnLongitude));
                    
                    var result = objClient.Player.UpdatePlayerLocation(
                        returnLatitude,
                        returnLongitude,
                        BotSettings.DefaultAltitude).Result;
                }
            }

            if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
            {
                if (SkippedPokemon.Contains(encounterPokemonResponse.WildPokemon.EncounterId))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Previously Skipped this Pokemon - Skipping Again!");
                    return 0;
                }

                var bestPokeball = GetBestBall(encounterPokemonResponse?.WildPokemon, false);

                var iv = PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse.WildPokemon.PokemonData);
                var strIVPerfection =iv.ToString("0.00");
                if (bestPokeball == ItemId.ItemUnknown)
                {
                    
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}%");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");

                    pokeballoutofstock = true;
                    logicAllowCatchPokemon = false;

                    return 0;
                }

                var inventoryBerries = objClient.Inventory.GetItems().Result;
                var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();

                var escaped = false;
                var berryOutOfStock = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Encountered {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% Probability {Math.Round(probability.Value * 100)}%");

                if (encounterPokemonResponse.WildPokemon.PokemonData != null &&
                    encounterPokemonResponse.WildPokemon.PokemonData.Cp > BotSettings.MinCPtoCatch &&
                    iv > BotSettings.MinIVtoCatch)
                {
                    var used = false;
                    CatchPokemonResponse caughtPokemonResponse;

                    do
                    {
                        // Check if the best ball is still valid
                        if (bestPokeball == ItemId.ItemUnknown)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}%");
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");

                            pokeballoutofstock = true;
                            logicAllowCatchPokemon = false;

                            return 0;
                        }

                        if (((probability.Value < BotSettings.razzberry_chance) || escaped) && BotSettings.UseRazzBerry && !used)
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
                                    used = true;

                                    Logger.Info( $"Thrown {bestBerry}. Remaining: {berries.Count}.");

                                    RandomHelper.RandomSleep(50, 200);
                                }
                                else
                                {
                                    escaped = true;
                                    used = true;
                                }
                            }
                            else
                            {
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
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Missed {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} while using {bestPokeball}");
                            missCount++;
                            RandomHelper.RandomSleep(1500, 6000);
                        }
                        else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"{StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} escaped while using {bestPokeball}");
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
                        ret = caughtPokemonResponse.CapturedPokemonId;

                        if (GlobalVars.ShowPokemons)
                            DeletePokemonFromMap(encounterPokemonResponse.WildPokemon.SpawnPointId);

                        var curDate = DateTime.Now;
                        Task.Factory.StartNew(() =>infoObservable.PushNewHuntStats($"{pokeLat}/{pokeLong};{pokeid};{curDate.Ticks};{curDate}" + Environment.NewLine));

                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        var logs = Path.Combine(logPath, "PokeLog.txt");
                        var date = DateTime.Now;
                        if (caughtPokemonResponse.CaptureAward.Xp.Sum() >= 500)
                        {
                            if (BotSettings.LogPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught new {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {strIVPerfection}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Caught {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP | {caughtPokemonResponse.CaptureAward.Candy.Sum()} Candies | {caughtPokemonResponse.CaptureAward.Stardust.Sum()} Stardust");
                            Setout.pokemonCatchCount++;
                        }
                        else
                        {
                            if (BotSettings.LogPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {strIVPerfection}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Caught {StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP | {caughtPokemonResponse.CaptureAward.Candy.Sum()} Candies | {caughtPokemonResponse.CaptureAward.Stardust.Sum()} Stardust");
                            Setout.pokemonCatchCount++;

                            if (Telegram != null)
                                Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Catch, StringUtils.getPokemonNameByLanguage(BotSettings, pokeid), encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, strIVPerfection, bestPokeball, caughtPokemonResponse.CaptureAward.Xp.Sum());
                        }
                        BotStats.AddPokemon(1);
                        BotStats.AddExperience(caughtPokemonResponse.CaptureAward.Xp.Sum());
                        BotStats.AddStardust(caughtPokemonResponse.CaptureAward.Stardust.Sum());
                        Setout.RefreshConsoleTitle(objClient);
                        RandomHelper.RandomSleep(1500, 2000);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{StringUtils.getPokemonNameByLanguage(BotSettings, pokeid)} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got away while using {bestPokeball}..");
                        FailedSoftban++;
                        if (FailedSoftban > 10)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Soft Ban Detected - Stopping Bot to prevent perma-ban. Try again in 4-24 hours and be more careful next time!");
                            Setout.LimitReached("");
                        }
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon CP or IV lower than Configured Min to Catch - Skipping Pokemon");
                    SkippedPokemon.Add(encounterPokemonResponse.WildPokemon.EncounterId);
                }
            }else if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.PokemonInventoryFull){
                Logger.Warning("You have no free space for new pokemons...transfer some as soon as possible.");
            }else{
                Logger.Debug(encounterPokemonResponse.Status.ToString());
            }
            return ret;
        }

        private CatchPokemonResponse CatchPokemonWithRandomVariables(ulong encounterId, string spawnpointId, ItemId bestPokeball, bool forceHit)
        {
            #region Reset Function Variables

            var normalizedRecticleSize = 1.95;
            var hitTxt = "Default Perfect";
            var spinModifier = 1.0;
            var spinTxt = "Curve";
            var pbExcellent = BotSettings.excellentthrow;
            var pbGreat = BotSettings.greatthrow;
            var pbNice = BotSettings.nicethrow;
            var pbOrdinary = BotSettings.ordinarythrow;
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
                    Logger.Warning("PokeBall Count is Zero");

            if (balls.Any(g => g.Key == ItemId.ItemGreatBall))
                if (balls.First(g => g.Key == ItemId.ItemGreatBall).First().Count > 0)
                    pokeBallCollection.Add("greatBalls", balls.First(g => g.Key == ItemId.ItemGreatBall).First().Count);
                else
                    Logger.Warning("GreatBall Count is Zero");

            if (balls.Any(g => g.Key == ItemId.ItemUltraBall))
                if (balls.First(g => g.Key == ItemId.ItemUltraBall).First().Count > 0)
                    pokeBallCollection.Add("ultraBalls", balls.First(g => g.Key == ItemId.ItemUltraBall).First().Count);
                else
                    Logger.Warning("UltraBall Count is Zero");

            if (balls.Any(g => g.Key == ItemId.ItemMasterBall))
                if (balls.First(g => g.Key == ItemId.ItemMasterBall).First().Count > 0)
                    pokeBallCollection.Add("masterBalls", balls.First(g => g.Key == ItemId.ItemMasterBall).First().Count);
                else
                    Logger.Warning("MasterBall Count is Zero");

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
                if ((pokeballqty <= BotSettings.InventoryBasePokeball || BotSettings.InventoryBasePokeball == 0) && BotSettings.LimitPokeballUse)
                {
                    pokeBalls = false;
                }
            }
            if (pokeballCollection.ContainsKey("greatBalls"))
            {
                greatBalls = true;
                if ((greatballqty <= BotSettings.InventoryBaseGreatball || BotSettings.InventoryBaseGreatball == 0) && BotSettings.LimitGreatballUse)
                {
                    greatBalls = false;
                }
            }

            if (pokeballCollection.ContainsKey("ultraBalls"))
            {
                ultraBalls = true;
                if ((ultraballqty <= BotSettings.InventoryBaseUltraball || BotSettings.InventoryBaseUltraball == 0) && BotSettings.LimitUltraballUse)
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

            if (BotSettings.MinCPforGreatBall > 0 && BotSettings.MinCPforUltraBall > 0 && BotSettings.MinCPforGreatBall < BotSettings.MinCPforUltraBall)
            {
                minCPforGreatBall = BotSettings.MinCPforGreatBall;
                minCPforUltraBall = BotSettings.MinCPforUltraBall;
            }

            var getMyLowestAppropriateBall = new Dictionary<Func<int?, bool>, Action>
            {
                {x => x < minCPforGreatBall, () => lowestAppropriateBall = ItemId.ItemPokeBall}, {x => x < minCPforUltraBall, () => lowestAppropriateBall = ItemId.ItemGreatBall}, {x => x < 2000, () => lowestAppropriateBall = ItemId.ItemUltraBall}, {x => x >= 2000, () => lowestAppropriateBall = ItemId.ItemMasterBall}
            };
            getMyLowestAppropriateBall.First(sw => sw.Key(pokemonCp)).Value();
            //use next best ball if pokemon has escped before
            if (escaped && BotSettings.NextBestBallOnEscape)
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
            if (!berries.Any()) {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Berrys to select! - Using next best ball instead"); 
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


        public void ShowNearbyPokemons(IEnumerable<MapPokemon> pokeData)
        {
            infoObservable.PushNewPokemonLocations(pokeData);
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
    }
}