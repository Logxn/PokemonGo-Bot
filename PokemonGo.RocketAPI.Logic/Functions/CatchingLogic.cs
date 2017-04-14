/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/02/2017
 * Time: 22:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using POGOProtos.Enums;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Pokemon;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Shared;
using System.Linq;
using PokeMaster.Logic;
using PokeMaster.Logic.Utils;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Description of CatchingLogic.
    /// </summary>
    public static class CatchingLogic
    {
        private static Client client = null;
        private static LogicInfoObservable infoObservable =null;

        private static int zeroCachablePokemons  = 0;
        public static bool AllowCatchPokemon = true;
        public static List<ulong> SkippedPokemon = new List<ulong>();

        public static bool Execute( )
        {
            return Execute(null);
        }
        public static bool Execute(GetMapObjectsResponse mapObjectsResponse )
        {
            client = Logic.objClient;
            infoObservable= Logic.Instance.infoObservable;
            //bypass catching pokemon if disabled
            if (GlobalVars.CatchPokemon && AllowCatchPokemon )
            {
                if (mapObjectsResponse == null)
                {
                    mapObjectsResponse = client.Map.GetMapObjects().Result;
                }

                MapPokemon mapIncensePokemon = null;
                try {
                    var duration = Setout.lastincenseuse - DateTime.Now;
                    Logger.Debug("last incense use duration: "+ duration);
                    if (duration.TotalMilliseconds >0 ){
                        var incensePokemon= client.Map.GetIncensePokemons();
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
                    if (!GlobalVars.catchPokemonSkipList.Contains(mapIncensePokemon.PokemonId) )
                        CatchIncensedPokemon(mapIncensePokemon.EncounterId, mapIncensePokemon.SpawnPointId, mapIncensePokemon.PokemonId, mapIncensePokemon.Longitude, mapIncensePokemon.Latitude);

                var pokemons = mapObjectsResponse.MapCells.SelectMany(i => i.CatchablePokemons).OrderBy(i => LocationUtils.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude, i.Latitude, i.Longitude));
                Logger.Debug( $"Pokemons Catchable: {pokemons.Count()}");
                
                var nearbyPokemons = mapObjectsResponse.MapCells.SelectMany(i => i.NearbyPokemons);
                Logger.Debug( $"Pokemons Nearby: {nearbyPokemons.Count()}");
                var wildPokemons = mapObjectsResponse.MapCells.SelectMany(i => i.WildPokemons).OrderBy(i => LocationUtils.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude, i.Latitude, i.Longitude));
                Logger.Debug( $"Pokemons Wild: {wildPokemons.Count()}");
                if (pokemons.Any())
                {
                    var strNames = pokemons.Aggregate("", (current, pokemon) => current + ( pokemon.PokemonId + ", "));
                    strNames = strNames.Substring(0, strNames.Length - 2);

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s): " + strNames);
                    if (GlobalVars.ShowPokemons){
                        ShowNearbyPokemons(pokemons);
                    }
                }else{
                    zeroCachablePokemons++;
                    if (zeroCachablePokemons> 10){
                        zeroCachablePokemons =0;
                        client.Login.DoLogin().Wait();
                    }
                    return false;
                }


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

                    if (GlobalVars.catchPokemonSkipList.Contains(pokemon.PokemonId))
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Pokemon: " + pokemon.PokemonId);
                        continue;
                    }

                    #endregion

                    //get distance to pokemon
                    var distance = LocationUtils.CalculateDistanceInMeters(client.CurrentLatitude, client.CurrentLongitude, pokemon.Latitude, pokemon.Longitude);

                    RandomHelper.RandomSleep(distance > 100 ? 1000 : 100,distance > 100 ? 1100 : 110);

                    // Do Catch here
                    CatchPokemon(pokemon.EncounterId, pokemon.SpawnPointId, pokemon.PokemonId, pokemon.Longitude, pokemon.Latitude);

                    
                }
                client.Map.GetMapObjects(true).Wait(); //force Map Objects Update
                client.Inventory.GetInventory(true); //force Inventory Update
                return true;
            }
            return false;
        }
        public static ulong CatchIncensedPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong, double pokeLat)
        {
            return CatchPokemon(encounterId,spawnpointId, pokeid,pokeLong,pokeLat,false,-1,-1,2);
        }

        public static ulong CatchLuredPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong, double pokeLat)
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

        public static ulong CatchPokemon(ulong encounterId, string spawnpointId, PokemonId pokeid, double pokeLong = 0, double pokeLat = 0, bool goBack = false, double returnLatitude = -1, double returnLongitude = -1, int luredPoke = 0)
        {
            ulong ret = 0;
            EncounterResponse encounterPokemonResponse;

            //Offset Miss count here to account for user setting.
            var missCount = 0;

            if (GlobalVars.Max_Missed_throws <= 1)
            {
                missCount = 2;
            }

            if (GlobalVars.Max_Missed_throws == 2)
            {
                missCount = 1;
            }

            var forceHit = false;

            try
            {
                if (luredPoke == 0)
                    encounterPokemonResponse = client.Encounter.EncounterPokemon(encounterId, spawnpointId).Result;
                else if (luredPoke == 1){
                    var DiscEncounterPokemonResponse =  client.Encounter.EncounterLurePokemon(encounterId, spawnpointId);
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
                    var IncenseEncounterPokemonResponse =  client.Encounter.EncounterIncensePokemon(encounterId, spawnpointId);
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
                if (goBack)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"(SNIPING) Go to {returnLatitude} / {returnLongitude} before starting the capture.");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan,LocationUtils.FindAddress(returnLatitude, returnLongitude));
                    LocationUtils.updatePlayerLocation(client, returnLatitude, returnLongitude, GlobalVars.altitude);
                    var tmpMap = client.Map.GetMapObjects(true);
                }
                return ret;
            }

            if (goBack)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"(SNIPING) Go to {returnLatitude} / {returnLongitude} before starting the capture.");
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan,LocationUtils.FindAddress(returnLatitude, returnLongitude));
                LocationUtils.updatePlayerLocation(client, returnLatitude, returnLongitude, GlobalVars.altitude);
                var tmpMap = client.Map.GetMapObjects(true);
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

                    Logic.Instance.pokeballoutofstock = true;
                    AllowCatchPokemon = false;

                    return 0;
                }

                var inventoryBerries = client.Inventory.GetItems(true);
                var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();
                var probability100 =  Math.Round(probability.Value * 100);

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Encountered {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% Probability {probability100}%");
                if (encounterPokemonResponse.WildPokemon.PokemonData != null)
                    SaveLocations(encounterPokemonResponse.WildPokemon, iv, probability100);

                if (encounterPokemonResponse.WildPokemon.PokemonData != null &&
                    encounterPokemonResponse.WildPokemon.PokemonData.Cp >= GlobalVars.MinCPtoCatch &&
                    probability100 >= GlobalVars.MinProbToCatch &&
                    iv >= GlobalVars.MinIVtoCatch)
                {
                    var usedBerry = false;
                    var escaped = false;
                    CatchPokemonResponse caughtPokemonResponse;
                    var inventory = client.Inventory.GetItems();
                    var razz = inventory.FirstOrDefault(p => p.ItemId == ItemId.ItemRazzBerry);
                    var pinap= inventory.FirstOrDefault(p => p.ItemId == ItemId.ItemPinapBerry);
                    var nanab = inventory.FirstOrDefault(p => p.ItemId == ItemId.ItemNanabBerry);

                    do
                    {
                        // Check if the best ball is still valid
                        if (bestPokeball == ItemId.ItemUnknown)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"No Pokeballs! - missed {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}%");
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Detected all balls out of stock - disabling pokemon catch until restock of at least 1 ball type occurs");

                            Logic.Instance.pokeballoutofstock = true;
                            AllowCatchPokemon = false;

                            return 0;
                        }


                        if (GlobalVars.UseRazzBerry &&  !usedBerry && ( probability.Value < GlobalVars.razzberry_chance ))
                        {
                            if (razz != null && razz.Count > 0)
                            {
                                //Throw berry
                                var useRazzberry = client.Encounter.UseItemEncounter(encounterId, ItemId.ItemRazzBerry, spawnpointId);
                                if (useRazzberry.Status ==UseItemEncounterResponse.Types.Status.Success){
                                    razz.Count = razz.Count - 1;
                                    Logger.Info($"We used a Razz Berry. Remaining: {razz.Count}.");
                                    usedBerry = true;
                                }
                                else
                                    Logger.Info("RazzBerry Status: "+ useRazzberry.Status);

                                RandomHelper.RandomSleep(250);
                            }
                        }

                        if (GlobalVars.PokemonPinap.Contains(pokeid) && !usedBerry )
                        {
                            try {

                                if (pinap != null && pinap.Count > 0)
                                {
                                    // Use a pinap
                                    var res = client.Encounter.UseItemEncounter(encounterId, ItemId.ItemPinapBerry, spawnpointId);
                                    if (res.Status ==UseItemEncounterResponse.Types.Status.Success){
                                        pinap.Count = pinap.Count - 1;
                                        Logger.Info($"We used a Pinap Berry. Remaining: {pinap.Count}.");
                                        usedBerry = true;
                                    }
                                    else
                                        Logger.Info("PinapBerry Status: "+ res.Status);
                                    RandomHelper.RandomSleep(250);
                                }
                            } catch (Exception ex1) {
                                Logger.Debug (""+ex1);
                            }
                        }

                        var r = new Random();

                        if(GlobalVars.UseNanabBerry && !usedBerry )
                        {
                            try {
                                var reallyUseIt =  (r.Next(0,GlobalVars.NanabPercent)!=0);
                                if (GlobalVars.NanabPercent == 100 || reallyUseIt){

                                    if (nanab != null && nanab.Count > 0)
                                    {
                                        var res = client.Encounter.UseItemEncounter(encounterId, ItemId.ItemNanabBerry, spawnpointId);
                                        if (res.Status ==UseItemEncounterResponse.Types.Status.Success){
                                            nanab.Count = nanab.Count - 1;
                                            Logger.Info($"We used a Nabab Berry. Remaining: {nanab.Count}.");
                                            usedBerry = true;
                                        }
                                        else
                                            Logger.Info("Status: "+ res.Status);
                                        RandomHelper.RandomSleep(250);
                                    }
                                }
                            } catch (Exception ex1) {
                                Logger.Debug (""+ex1);
                            }
                        }
                        // limit number of balls wasted by misses and log for UX because fools be tripin
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
                                forceHit |= rInt == 1;
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
                        if (caughtPokemonResponse==null){
                            caughtPokemonResponse = new CatchPokemonResponse();
                        }

                        if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Missed { pokeid} while using {bestPokeball}");
                            missCount++;
                            RandomHelper.RandomSleep(1500, 3000);
                        }
                        else if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"{pokeid} escaped while using {bestPokeball}");
                            usedBerry = false;
                            escaped = true;
                            //reset forceHit in case we randomly triggered on last throw.
                            forceHit = false;
                            RandomHelper.RandomSleep(1500, 3000);
                        }
                        // Update the best ball to ensure we can still throw
                        bestPokeball = GetBestBall(encounterPokemonResponse?.WildPokemon, escaped);
                    } while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        ret = caughtPokemonResponse.CapturedPokemonId;

                        if (GlobalVars.ShowPokemons)
                            Logic.Instance.DeletePokemonFromMap(encounterPokemonResponse.WildPokemon.SpawnPointId);

                        var curDate = DateTime.Now;
                        Task.Factory.StartNew(() =>infoObservable.PushNewHuntStats($"{pokeLat}/{pokeLong};{pokeid};{curDate.Ticks};{curDate}" + Environment.NewLine));

                        var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                        var logs = Path.Combine(logPath, "PokeLog.txt");
                        var date = DateTime.Now;
                        if (caughtPokemonResponse.CaptureAward.Xp.Sum() >= 500)
                        {
                            if (GlobalVars.LogPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught new {pokeid} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {strIVPerfection}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Caught {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP | {caughtPokemonResponse.CaptureAward.Candy.Sum()} Candies | {caughtPokemonResponse.CaptureAward.Stardust.Sum()} Stardust");
                            Setout.pokemonCatchCount++;
                            Setout.SaveSession();
                        }
                        else
                        {
                            if (GlobalVars.LogPokemons)
                            {
                                File.AppendAllText(logs, $"[{date}] Caught {pokeid} (CP: {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} | IV: {strIVPerfection}% | Pokeball used: {bestPokeball} | XP: {caughtPokemonResponse.CaptureAward.Xp.Sum()}) " + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Caught {pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got {caughtPokemonResponse.CaptureAward.Xp.Sum()} XP | {caughtPokemonResponse.CaptureAward.Candy.Sum()} Candies | {caughtPokemonResponse.CaptureAward.Stardust.Sum()} Stardust");
                            Setout.pokemonCatchCount++;
                            Setout.SaveSession();

                            if (Logic.Instance.Telegram != null)
                                Logic.Instance.Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Catch, pokeid, encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp, strIVPerfection, bestPokeball, caughtPokemonResponse.CaptureAward.Xp.Sum());
                        }
                        Logic.Instance.BotStats.AddPokemon(1);
                        Logic.Instance.BotStats.AddExperience(caughtPokemonResponse.CaptureAward.Xp.Sum());
                        Logic.Instance.BotStats.AddStardust(caughtPokemonResponse.CaptureAward.Stardust.Sum());
                        Setout.RefreshConsoleTitle(client);
                        RandomHelper.RandomSleep(1500, 2000);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{pokeid} CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} IV {strIVPerfection}% got away while using {bestPokeball}..");
                        Logic.FailedSoftban++;
                        if (Logic.FailedSoftban > 10)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Soft Ban Detected - Stopping Bot to prevent perma-ban. Try again in 4-24 hours and be more careful next time!");
                            Setout.LimitReached("");
                        }
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pokemon CP or IV or Prob lower than Configured Min to Catch - Skipping Pokemon");
                    SkippedPokemon.Add(encounterPokemonResponse.WildPokemon.EncounterId);
                }
            }else if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.PokemonInventoryFull){
                Logger.Warning("You have no free space for new pokemons...transfer some as soon as possible.");
            }else{
                Logger.Debug(encounterPokemonResponse.Status.ToString());
            }
            return ret;
        }
        private static CatchPokemonResponse CatchPokemonWithRandomVariables(ulong encounterId, string spawnpointId, ItemId bestPokeball, bool forceHit)
        {
            #region Reset Function Variables

            var normalizedRecticleSize = 1.95;
            var hitTxt = "Default Perfect";
            var spinModifier = 1.0;
            var spinTxt = "Curve";
            var pbExcellent = GlobalVars.excellentthrow;
            var pbGreat = GlobalVars.greatthrow;
            var pbNice = GlobalVars.nicethrow;
            var pbOrdinary = GlobalVars.ordinarythrow;
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
            return client.Encounter.CatchPokemon(encounterId, spawnpointId, bestPokeball, forceHit, normalizedRecticleSize, spinModifier);
        }
        private static ItemId GetBestBall(WildPokemon pokemon, bool escaped)
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
                        pokeballqty = pokeballtype.Value;
                        break;
                    case "greatBalls":
                        greatballqty = pokeballtype.Value;
                        break;
                    case "ultraBalls":
                        ultraballqty = pokeballtype.Value;
                        break;
                }
            }
            if (pokeballCollection.ContainsKey("pokeBalls"))
            {
                pokeBalls = true;
                pokeBalls &= (pokeballqty > GlobalVars.InventoryBasePokeball && GlobalVars.InventoryBasePokeball != 0) || !GlobalVars.LimitPokeballUse;
            }
            if (pokeballCollection.ContainsKey("greatBalls"))
            {
                greatBalls = true;
                greatBalls &= (greatballqty > GlobalVars.InventoryBaseGreatball && GlobalVars.InventoryBaseGreatball != 0) || !GlobalVars.LimitGreatballUse;
            }

            if (pokeballCollection.ContainsKey("ultraBalls"))
            {
                ultraBalls = true;
                ultraBalls &= (ultraballqty > GlobalVars.InventoryBaseUltraball && GlobalVars.InventoryBaseUltraball != 0) || !GlobalVars.LimitUltraballUse;
            }

            masterBalls |= pokeballCollection.ContainsKey("masterBalls");

            #endregion

            #region Get Lowest Appropriate Ball by CP and escape status

            var lowestAppropriateBall = ItemId.ItemUnknown;

            var minCPforGreatBall = 500;
            var minCPforUltraBall = 1000;

            if (GlobalVars.MinCPforGreatBall > 0 && GlobalVars.MinCPforUltraBall > 0 && GlobalVars.MinCPforGreatBall < GlobalVars.MinCPforUltraBall)
            {
                minCPforGreatBall = GlobalVars.MinCPforGreatBall;
                minCPforUltraBall = GlobalVars.MinCPforUltraBall;
            }

            var getMyLowestAppropriateBall = new Dictionary<Func<int?, bool>, Action>
            {
                {x => x < minCPforGreatBall, () => lowestAppropriateBall = ItemId.ItemPokeBall}, {x => x < minCPforUltraBall, () => lowestAppropriateBall = ItemId.ItemGreatBall}, {x => x < 2000, () => lowestAppropriateBall = ItemId.ItemUltraBall}, {x => x >= 2000, () => lowestAppropriateBall = ItemId.ItemMasterBall}
            };
            getMyLowestAppropriateBall.First(sw => sw.Key(pokemonCp)).Value();
            //use next best ball if pokemon has escped before
            if (escaped && GlobalVars.NextBestBallOnEscape)
            {
                switch (lowestAppropriateBall)
                {
                    case ItemId.ItemGreatBall:
                        lowestAppropriateBall = ItemId.ItemUltraBall;
                        break;
                    case ItemId.ItemUltraBall:
                        lowestAppropriateBall = ItemId.ItemMasterBall;
                        break;
                    case ItemId.ItemMasterBall:
                        lowestAppropriateBall = ItemId.ItemMasterBall;
                        break;
                    default:
                        lowestAppropriateBall = ItemId.ItemGreatBall;
                        break;
                }
            }
            //handle appropriate ball out of stock
            switch (lowestAppropriateBall)
            {
                case ItemId.ItemGreatBall:
                    if (greatBalls) return ItemId.ItemGreatBall;
                    if (ultraBalls) return ItemId.ItemUltraBall;
                    if (masterBalls) return ItemId.ItemMasterBall;
                    return pokeBalls ? ItemId.ItemPokeBall : ItemId.ItemUnknown;
                case ItemId.ItemUltraBall:
                    if (ultraBalls) return ItemId.ItemUltraBall;
                    if (masterBalls) return ItemId.ItemMasterBall;
                    if (greatBalls) return ItemId.ItemGreatBall;
                    return pokeBalls ? ItemId.ItemPokeBall : ItemId.ItemUnknown;
                case ItemId.ItemMasterBall:
                    if (masterBalls) return ItemId.ItemMasterBall;
                    if (ultraBalls) return ItemId.ItemUltraBall;
                    if (greatBalls) return ItemId.ItemGreatBall;
                    return pokeBalls ? ItemId.ItemPokeBall : ItemId.ItemUnknown;
                default:
                    if (pokeBalls) return ItemId.ItemPokeBall;
                    if (greatBalls) return ItemId.ItemGreatBall;
                    if (ultraBalls) return ItemId.ItemUltraBall;
                    return pokeBalls ? ItemId.ItemMasterBall : ItemId.ItemUnknown;
            }

            #endregion
        }

        private static Dictionary<string, int> GetPokeballQty()
        {
            var pokeBallCollection = new Dictionary<string, int>();
            var items = client.Inventory.GetItems();
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

        public static void ShowNearbyPokemons(IEnumerable<MapPokemon> pokeData)
        {
            infoObservable.PushNewPokemonLocations(pokeData);
        }
        
        public static void SaveLocations(WildPokemon pokemon, double iv, double probability){
            if (!GlobalVars.SaveLocations && ! GlobalVars.SendToDiscord)
                return;
            if (GlobalVars.SaveLocations){
                if (iv < GlobalVars.MinIVSave)
                    return;
                var strIV = iv.ToString("0.00");
                var id = pokemon.EncounterId;
                var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                var LastModified = StringUtils.TimeMStoString(pokemon.LastModifiedTimestampMs, @"mm\:ss");
                var TillHidden = StringUtils.TimeMStoString(pokemon.TimeTillHiddenMs, @"mm\:ss");
                var name = GlobalVars.ProfileName;
                var Latitude = pokemon.Latitude.ToString(CultureInfo.InvariantCulture);
                var Longitude = pokemon.Longitude.ToString(CultureInfo.InvariantCulture);
                var line = $"{date}|{LastModified}|{id}|{name}|{TillHidden}|{probability}|{strIV}|pokesniper2://{pokemon.PokemonData.PokemonId}/{Latitude},{Longitude}";
                
                if (!ExistYet(id)){
                    try
                    {
                        File.AppendAllLines(GlobalVars.SaveLocationsFile, new string[] { line });
                    }
                    catch(Exception)
                    {
                        Logger.Info("Hey pssst. If you get this message follow these steps:");
                        Logger.Info("1. Open your Pokemonlist and go to the Tab 'Options'");
                        Logger.Info("2. Select Misc");
                        Logger.Info("3. Either you press the two dots and select a path...");
                        Logger.Info("4. Or disable the feature...");
                        Logger.Info("5. Dont forget to press Update Config.");
                    }
                }
                
            }
            if (GlobalVars.SendToDiscord){
                 if (iv < GlobalVars.MinIVSave)
                     return;
                 if (iv >= 90)
                    DiscordLogic.SendMessage(DiscordLogic.FormatMessage(pokemon,iv,probability));
            }
        }
        public static bool ExistYet(ulong EncounterId)
        {
            var tries = 5;
            do {
                try {
                    var lines = File.ReadAllLines(GlobalVars.SaveLocationsFile);
                    foreach (var element in lines) {
                        var id = 0UL;
                        var values =element.Split('|');
                        if (values.Length>2)
                            ulong.TryParse(values[2], out id);
                        if (id == EncounterId)
                            return true;
                    }
                    
                } catch (Exception ex1) {
                    Logger.Debug(ex1.ToString());
                }
                tries--;
                RandomHelper.RandomSleep(150);
            } while(tries < 0);
            return false;
        }

    }
}
