/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 25/01/2017
 * Time: 19:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;
using POGOProtos.Settings.Master;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Rpc;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;

namespace PokeMaster.Logic.Functions
{
    /// <summary>
    /// Description of Setout.
    /// </summary>

    public class IncubatorUsage : IEquatable<IncubatorUsage>
    {
        public string IncubatorId;
        public ulong PokemonId;

        public bool Equals(IncubatorUsage other)
        {
            return other != null && other.IncubatorId == IncubatorId && other.PokemonId == PokemonId;
        }
    }

    public static class Setout
    {
        public static int count;
        public static DateTime lastincenseuse;
        public static DateTime LastIncenselog;
        public static double startingXp = -10000;
        public static double currentxp = -10000;
        public static  int level = -1;
        public static double timetorunstamp = -10000;
        public static double pausetimestamp = -10000;
        public static double resumetimestamp = -10000;
        public static double lastlog = -10000;
        public static int pokemonCatchCount;
        public static int pokeStopFarmedCount;
        public static String timeLeftToNextLevel;
        public static DateTime sessionStart;
        private static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        private static string sessionFile = Path.Combine(configPath, "session.json");
        
        public static void SaveSession()
        {
            var strs = new Dictionary<string,string>();
            strs.Add("SessionStart", ""+ sessionStart.Ticks);
            strs.Add("PokemonsCaught", ""+pokemonCatchCount);
            strs.Add("PokestopsFarmed", ""+pokeStopFarmedCount);
            var Json = JsonConvert.SerializeObject(strs, Formatting.Indented);
            if(!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            File.WriteAllText(sessionFile, Json);
            
        }
        
        public static void LoadSession()
        {
            if(!File.Exists(sessionFile))
                return;
            var strJSON = File.ReadAllText(sessionFile);
            var strs = JsonConvert.DeserializeObject<Dictionary<string,string>>(strJSON);
            var ticks = 0L;
            long.TryParse(strs["SessionStart"],out ticks);
            sessionStart = new DateTime(ticks);
            int.TryParse(strs["PokemonsCaught"],out pokemonCatchCount);
            int.TryParse(strs["PokestopsFarmed"],out pokeStopFarmedCount);
            
        }

        public static void Execute()
        {
            // reset stat counter
            count = 0;
            CheckIfUseIncense();

            if (GlobalVars.UseLuckyEggIfNotRunning || GlobalVars.UseLuckyEggGUIClick)
            {
                GlobalVars.UseLuckyEggGUIClick = false;
                UseLuckyEgg(Logic.objClient);
            }

            if (GlobalVars.EvolvePokemonsIfEnoughCandy)
                EvolveAllPokemonWithEnoughCandy(GlobalVars.pokemonsToEvolve);

            if (GlobalVars.AutoIncubate)
                StartIncubation();

            TransferDuplicatePokemon(GlobalVars.keepPokemonsThatCanEvolve, GlobalVars.TransferFirstLowIV);

            CheckLevelUp(Logic.objClient);
            
            if (GlobalVars.ShowStats)
                StatsLog(Logic.objClient);

            if (GlobalVars.CheckWhileRunning)
                CheckWhileWalking();

            RefreshConsoleTitle(Logic.objClient);
            
            Logic.objClient.ReadyToUse = true;
            Logger.Debug("Client is ready to use");
            SetCheckTimeToRun();
        }


        public static void CheckIfUseIncense()
        {
            Logger.Debug("Checking use of incense");
            if (GlobalVars.RelocateDefaultLocation)
                return;
            if (GlobalVars.UseIncense || GlobalVars.UseIncenseGUIClick)
            {
                Logger.Debug("Use incense selected");
                GlobalVars.UseIncenseGUIClick = false;
                var inventory = Logic.objClient.Inventory.GetItems();
                var incsense = inventory.FirstOrDefault(p => p.ItemId == ItemId.ItemIncenseOrdinary);
                var loginterval = DateTime.Now - LastIncenselog;
                Logger.Debug("loginterval: "+ loginterval);
                Logger.Debug("last incense use: "+ lastincenseuse);
                if (lastincenseuse > DateTime.Now.AddSeconds(5))
                {
                    var duration = lastincenseuse - DateTime.Now;
                    Logger.Debug("duration: "+ duration);
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

                Logic.objClient.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                RandomHelper.RandomSleep(1100);
            }
        }

        private static void EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter )
        {
            int evolvecount = 0;
            int gotXP = 0;

            if ( GlobalVars.RelocateDefaultLocation)
                return;

            EvolvePokemonResponse evolvePokemonOutProto;

            var pokemonToEvolve = Logic.objClient.Inventory.GetPokemonToEvolve(true,filter);
            var toEvolveCount = pokemonToEvolve.Count();
            var startEvolving = (toEvolveCount==0 || toEvolveCount==GlobalVars.EvolveAt );

            if (startEvolving && GlobalVars.UseLuckyEgg)
                UseLuckyEgg(Logic.objClient);

            foreach (var pokemon in pokemonToEvolve)
            {

                var item = Inventory.GeteNeededItemToEvolve(pokemon.PokemonId);
                if (item != ItemId.ItemUnknown && Logic.objClient.Inventory.GetItemAmountByType(item) < 1){
                    if ( pokemon.PokemonId == PokemonId.Poliwhirl
                        || pokemon.PokemonId == PokemonId.Gloom
                        || pokemon.PokemonId == PokemonId.Slowpoke
                       )
                        item = ItemId.ItemUnknown; // try to evolve without items
                    else
                        continue; // go to next pokemon
                }
                evolvePokemonOutProto = Logic.objClient.Inventory.EvolvePokemon(pokemon.Id, item);

                if (evolvePokemonOutProto == null)
                    continue;

                var date = DateTime.Now.ToString();
                var evolvelog = Path.Combine(logPath, "EvolveLog.txt");

                

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)

                {
                    var getPokemonName = pokemon.PokemonId.ToString();
                    var cp = pokemon.Cp;
                    var calcPerf = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                    var getEvolvedName = StringUtils.getPokemonNameByLanguage(evolvePokemonOutProto.EvolvedPokemonData.PokemonId);
                    var getEvolvedCP = evolvePokemonOutProto.EvolvedPokemonData.Cp;
                    gotXP = gotXP + evolvePokemonOutProto.ExperienceAwarded;

                    if (evolvecount == 0)
                        if (GlobalVars.pauseAtEvolve2)
                    {
                        Logger.Info("Stopping to evolve some Pokemons.");
                        GlobalVars.PauseTheWalking = true;
                    }

                    var experienceAwarded =evolvePokemonOutProto.ExperienceAwarded.ToString("N0");
                    if (GlobalVars.LogEvolve)
                    {
                        File.AppendAllText(evolvelog, $"[{date}] - Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {experienceAwarded} XP" + Environment.NewLine);
                    }
                    Logger.Info( $"Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {experienceAwarded} XP");
                    Logger.Info($"Waiting a few seconds... dont worry!");
                    Logic.Instance.BotStats.AddExperience(evolvePokemonOutProto.ExperienceAwarded);

                    if (Logic.Instance.Telegram != null)
                        Logic.Instance.Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Evolve, StringUtils.getPokemonNameByLanguage( pokemon.PokemonId), pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"), StringUtils.getPokemonNameByLanguage( evolvePokemonOutProto.EvolvedPokemonData.PokemonId), evolvePokemonOutProto.EvolvedPokemonData.Cp, evolvePokemonOutProto.ExperienceAwarded.ToString("N0"));
                    evolvecount++;

                    if (GlobalVars.UseAnimationTimes)
                    {
                        RandomHelper.RandomSleep(30000, 35000);
                    }
                    else
                    {
                        RandomHelper.RandomSleep(500, 600);
                    }
                }
                else
                {
                    if (evolvePokemonOutProto.Result != EvolvePokemonResponse.Types.Result.Success)
                    {
                        if (GlobalVars.LogEvolve)
                        {
                            File.AppendAllText(evolvelog, $"[{date}] - Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}" + Environment.NewLine);
                        }
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}");
                        evolvecount++;
                    }
                }

            }
            if(evolvecount > 0)
            {
                var strGotXP = gotXP.ToString("N0");
                Logger.Info($"Evolved {evolvecount} Pokemons. We have got {strGotXP} XP.");

                if (GlobalVars.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons evolved. Time to continue our journey!");
                    GlobalVars.PauseTheWalking = false;
                }
            }

        }

        // To store incubators with eggs
        private static List<IncubatorUsage> rememberedIncubators = new List<IncubatorUsage>();
        private static void StartIncubation()
        {
            try
            {
                if ( GlobalVars.RelocateDefaultLocation)
                {
                    return;
                }
                var inventory = Logic.objClient.Inventory.GetInventory();
                var incubators = Logic.objClient.Inventory.GetEggIncubators().ToList();
                var unusedEggs = (Logic.objClient.Inventory.GetEggs()).Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart).ToList();
                var pokemons = (Logic.objClient.Inventory.GetPokemons()).ToList();

                var playerStats = Logic.objClient.Inventory.GetPlayerStats();
                var stats = playerStats.First();

                var kmWalked = stats.KmWalked;

                var logs = Path.Combine(logPath, "EggLog.txt");
                var date = DateTime.Now.ToString();

                var unusedEggsBasicInc = eggsHatchingAllowedBasicInc(unusedEggs);
                var unusedEggsUnlimitInc = eggsHatchingAllowed(unusedEggs);
                
                foreach (var incubator in rememberedIncubators)
                {
                    var hatched = pokemons.FirstOrDefault(x => !x.IsEgg && x.Id == incubator.PokemonId);
                    if (hatched == null) continue;

                    if (GlobalVars.LogEggs)
                    {
                        var MaxCP = PokemonInfo.CalculateMaxCP(hatched);
                        var Level = PokemonInfo.GetLevel(hatched);
                        var IVPercent = PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00");
                        File.AppendAllText(logs, $"[{date}] - Egg hatched and we got a {hatched.PokemonId} (CP: {hatched.Cp} | MaxCP: {MaxCP} | Level: {Level} | IV: {IVPercent}% )" + Environment.NewLine);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg hatched and we got a " + hatched.PokemonId + " CP: " + hatched.Cp + " MaxCP: " + PokemonInfo.CalculateMaxCP(hatched) + " Level: " + PokemonInfo.GetLevel(hatched) + " IV: " + PokemonInfo.CalculatePokemonPerfection(hatched).ToString("0.00") + "%");
                }

                if ((unusedEggsUnlimitInc.Count < 1) && (unusedEggsUnlimitInc.Count < 1))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "There are not Allowed Eggs to hatch");
                    return;
                }

                var newRememberedIncubators = new List<IncubatorUsage>();

                foreach (EggIncubator incubator in incubators)
                {
                    if (incubator.PokemonId == 0)
                    {

                        // If is basic incubator and user don't want use it, we go to the next incubator
                        if (    (incubator.ItemId == ItemId.ItemIncubatorBasic)
                            && ( ! GlobalVars.UseBasicIncubators) )
                            continue;

                        POGOProtos.Data.PokemonData egg;
                        if (incubator.ItemId == ItemId.ItemIncubatorBasic)
                            egg = GlobalVars.EggsAscendingSelectionBasicInc ? unusedEggsBasicInc.FirstOrDefault() : unusedEggsBasicInc.LastOrDefault();
                        else
                            egg = GlobalVars.EggsAscendingSelection ? unusedEggsUnlimitInc.FirstOrDefault() : unusedEggsUnlimitInc.LastOrDefault();

                        // If there is not eggs then we finish this function
                        if (egg == null)
                            return;

                        var response = Logic.objClient.Inventory.UseItemEggIncubator(incubator.Id, egg.Id);
                        try
                        {
                            unusedEggsUnlimitInc.Remove(egg);
                            unusedEggsBasicInc.Remove(egg);
                        }
                        catch (Exception ex){
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: Logic.cs - StartIncubation()");
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
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
                    rememberedIncubators = newRememberedIncubators;

            }
            catch (Exception ex)
            {
                // Leave this here: Logger.Error(e.StackTrace);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Egg: We dont have any eggs we could incubate.");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
            }
        }

        private static List<POGOProtos.Data.PokemonData> eggsHatchingAllowedBasicInc( object eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> ((List<POGOProtos.Data.PokemonData> ) eggs);
            if(GlobalVars.No2kmEggsBasicInc)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 2) > 0.001).ToList();
            }
            if(GlobalVars.No5kmEggsBasicInc)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 5) > 0.001).ToList();
            }
            if(GlobalVars.No10kmEggsBasicInc)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 10) > 0.001).ToList();
            }
            return ret;
        }
        
        private static List<POGOProtos.Data.PokemonData> eggsHatchingAllowed(object eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> ((List<POGOProtos.Data.PokemonData> ) eggs);
            if(GlobalVars.No2kmEggs)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 2) > 0.001).ToList();
            }
            if(GlobalVars.No5kmEggs)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 5) > 0.001).ToList();
            }
            if(GlobalVars.No10kmEggs)
            {
                ret = ret.Where(x => Math.Abs(x.EggKmWalkedTarget - 10) > 0.001).ToList();
            }
            return ret;
        }

        public static void RecycleItems(bool forcerefresh = false)
        {

            if (GlobalVars.RelocateDefaultLocation)
                return;
            var items = Logic.objClient.Inventory.GetItemsToRecycle(GlobalVars.GetItemFilter());

            foreach (var item in items)
            {
                if ((item.ItemId == ItemId.ItemPokeBall || item.ItemId == ItemId.ItemGreatBall || item.ItemId == ItemId.ItemUltraBall || item.ItemId == ItemId.ItemMasterBall) && Logic.Instance.pokeballoutofstock)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");
                    CatchingLogic.AllowCatchPokemon = true;
                    Logic.Instance.pokeballoutofstock = false;
                }
                var transfer = Logic.objClient.Inventory.RecycleItem(item.ItemId, item.Count).Result;
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {item.ItemId}");
                RandomHelper.RandomSleep(1000, 5000);
            }
        }


        private static void StatsLog(Client client)
        {

            var profile = client.Player.GetPlayer();
            var inventory = client.Inventory.GetInventory();
            var playerStats = client.Inventory.GetPlayerStats();
            var stats = playerStats.First();
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            if (Math.Abs(startingXp - -10000) < 0.001)
                startingXp = stats.Experience;

            currentxp = stats.Experience;

            var numDifferentPokemons = Enum.GetNames(typeof(PokemonId)).Length -1;
            var pokemonToEvolve = (client.Inventory.GetPokemonToEvolve()).Count();
            var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(numDifferentPokemons) * 100;
            var pokedexpercent = Math.Floor(pokedexpercentraw);


            var items = client.Inventory.GetItems();
            var pokemonCount = client.Inventory.GetPokemons().Count();
            var eggCount = client.Inventory.GetEggsCount();
            var maxPokemonStorage = profile.PlayerData.MaxPokemonStorage;
            var maxItemStorage = profile.PlayerData.MaxItemStorage;
            var stardust = profile.PlayerData.Currencies.ToArray()[1].Amount.ToString("N0");
            var currEXP = curexp.ToString("N0");
            var neededEXP = expneeded.ToString("N0");
            var expPercent = Math.Round(curexppercent, 2);
            var expLeft = stats.NextLevelXp - stats.Experience;
            timeLeftToNextLevel = Logic.Instance.BotStats.GettimeLeft(expLeft).ToString(@"dd\.hh\:mm");

            client.ShowingStats = true;
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "-----------------------[PLAYER STATS]-----------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Level/EXP: {stats.Level} | {currEXP}/{neededEXP} ({expPercent}%)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"EXP to Level up: {expLeft} (Time Left: {timeLeftToNextLevel})");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"PokeStops visited: {stats.PokeStopVisits}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"KM Walked: {Math.Round(stats.KmWalked, 2)}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokemon: {pokemonCount}/{maxPokemonStorage} ({pokemonToEvolve} Evolvable)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Eggs: {eggCount}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokedex Completion: {stats.UniquePokedexEntries}/{numDifferentPokemons} [{pokedexpercent}%]");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Stardust: {stardust}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Pokemon Catch Count this session: {pokemonCatchCount}");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"PokeStop Farmed Count this session: {pokeStopFarmedCount}");

            var totalitems = 0;
            foreach (var item in items){
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"{item.ItemId} Qty: {item.Count}");
                totalitems += item.Count;
            }
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Items: {totalitems}/{maxItemStorage} ");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "------------------------------------------------------------");

            client.ShowingStats = false;


        }

        private static void CheckLevelUp(Client client)
        {
            var stats = client.Inventory.GetPlayerStats().FirstOrDefault();
            if (stats == null)
                return;
            
            //if (stats.Level == 1 && stats.NextLevelXp == 1000)
            //    client.Misc.MarkTutorialComplete();


            if (level == -1) {
                level = stats.Level;
            }
            else if (stats.Level > level) {
                level = stats.Level;

                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Got the level up reward from your level up.");

                var lvlup = client.Player.GetLevelUpRewards(stats.Level);
                var alreadygot = new List<ItemId>();

                foreach (var i in lvlup.ItemsAwarded)
                {
                    if (alreadygot.Contains(i.ItemId)) continue;

                    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Got Item: {i.ItemId} ({i.ItemCount}x)");
                    alreadygot.Add(i.ItemId);
                }
                alreadygot.Clear();
            }
        }

        public static void RefreshConsoleTitle(Client client)
        {
            var profile = client.Player.GetPlayer();
            var inventory = client.Inventory.GetInventory();
            var playerStats = client.Inventory.GetPlayerStats();
            var stats = playerStats.FirstOrDefault();
            if (stats == null)
                return;
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;
            var expleft = expneeded -currentxp;

            string TitleText = profile.PlayerData.Username + @" Lvl " + stats.Level + @" (" +
                (curexp).ToString("N0") + @"/" +
                (expneeded).ToString("N0") + @"|" +
                Math.Round(curexppercent, 2) + @"%) Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount + @" " +
                Logic.Instance.BotStats.ToString(expleft);

            if (!GlobalVars.EnableConsoleInTab) System.Console.Title = TitleText;

            if (GlobalVars.EnablePokeList && client.ReadyToUse)
            {
                /*try {
                    Application.OpenForms[0].Invoke(new Action(() => Application.OpenForms[0].Text = TitleText));
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }*/

                //Xelwon fix this
            }
        }

        public static void SetCheckTimeToRun()
        {
            
            // Prevent Spamming Logs

            if ((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds > lastlog + 60000)
            {
                lastlog = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                Logger.Debug($"Last Log {lastlog}");

                #region Time to Run

                if (GlobalVars.TimeToRun > 0)
                {
                    if (Math.Abs((timetorunstamp + 10000)) < 0.001) {
                        timetorunstamp = GlobalVars.TimeToRun * 60 * 1000 + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    } else {
                        var runTimeRemaining = timetorunstamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        var remainingTime = Math.Round(runTimeRemaining / 1000 / 60, 2);
                        if (runTimeRemaining <= 0) {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Time To Run Reached or Exceeded...Walking back to default location and stopping bot");

                            Logic.Instance.WalkWithRouting(GlobalVars.latitude, GlobalVars.longitude);

                            LimitReached("Time to Run");
                        } else {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time to Run: {remainingTime} minutes");
                        }
                    }
                }

                #endregion

                #region Breaks

                if (GlobalVars.UseBreakFields && GlobalVars. BreakLength >0 && GlobalVars. BreakInterval > 0)
                {
                    if (pausetimestamp > -10000)
                    {
                        var walkTimeRemaining = pausetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        if (walkTimeRemaining <= 0)
                        {
                            pausetimestamp = -10000;
                            GlobalVars.pauseAtPokeStop = true;
                            resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds +GlobalVars.BreakLength * 60 * 1000;

                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Break Time! Pause walking for {GlobalVars.BreakLength} minutes");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {Math.Round(walkTimeRemaining / 1000 / 60, 2)} minutes");
                        }
                    }
                    else if (Math.Abs(resumetimestamp + 10000) < 0.001) {
                        pausetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + GlobalVars.BreakInterval * 60 * 1000;

                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {GlobalVars.BreakInterval} minutes");
                    }
                }

                if (resumetimestamp > -10000)
                {
                    var breakTimeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                    if (breakTimeRemaining <= 0)
                    {
                        resumetimestamp = -10000;
                        GlobalVars.pauseAtPokeStop = false;

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
                if (!GlobalVars.CatchPokemon)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Catching Pokemon Disabled in Client Settings - Skipping all pokemon");
                }

                #endregion

                #region Check Run Count Limits

                #region Catch Pokemon Count Check

                if (pokemonCatchCount >= GlobalVars.PokemonCatchLimit)
                {
                    if (GlobalVars.FarmPokestops)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached - Bot will only farm pokestops");

                        CatchingLogic.AllowCatchPokemon = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached and not farming pokestops - Bot will return to default location and stop");

                        Logic.Instance.WalkWithRouting(GlobalVars.latitude, GlobalVars.longitude);

                        LimitReached("Catched Pokemon");
                    }
                }

                #endregion

                #region Farm Pokestops Check

                if (pokeStopFarmedCount >= GlobalVars.PokestopFarmLimit)
                {
                    if (GlobalVars.CatchPokemon && CatchingLogic.AllowCatchPokemon)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached - Bot will only catch pokemon");

                        GlobalVars.FarmPokestops = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached and not catching pokemon - Bot will return to default location and stop");

                        Logic.Instance.WalkWithRouting(GlobalVars.latitude, GlobalVars.longitude);

                        LimitReached("Farmed Pokestops");
                    }
                }

                #endregion

                #region XP Check

                if (Math.Abs(startingXp + 10000) > 0.001 && Math.Abs(currentxp + 10000) > 0.001 && (currentxp = -startingXp) >= GlobalVars.XPFarmedLimit)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "XP Farmed Limit Reached - Bot will return to default location and stop");

                    if (GlobalVars.UseGoogleMapsAPI)
                    {
                        Logic.Instance.WalkWithRouting(GlobalVars.latitude, GlobalVars.longitude);
                    }
                    else
                    {
                        Logic.Instance.navigation.HumanLikeWalking(
                            new GeoCoordinate(
                                GlobalVars.latitude,
                                GlobalVars.longitude),
                            GlobalVars.WalkingSpeedInKilometerPerHour,
                            CatchingLogic.Execute);
                    }
                    LimitReached("Farmed XP");
                }

                #endregion

                #endregion
            }
        }

        private static  void TransferUnwantedPokemon(ulong buddyid)
        {
            if (!GlobalVars.pokemonsToAlwaysTransfer.Any())
                return;
            var pokemons = Logic.objClient.Inventory.GetPokemons(true);
            var toTransfer = pokemons.Where(x => x.DeployedFortId == string.Empty && x.Favorite == 0 && !x.IsEgg && x.Id != buddyid);
            var idsToTransfer = new List<ulong>();
            var logs = Path.Combine(logPath, "TransferLog.txt");
            foreach (var pokemon in toTransfer){
                if (GlobalVars.pokemonsToAlwaysTransfer.Contains(pokemon.PokemonId)){
                    idsToTransfer.Add(pokemon.Id);
                    var Pokename = pokemon.PokemonId.ToString();
                    if (GlobalVars.LogTransfer)
                    {
                        var date = DateTime.Now.ToString();
                        File.AppendAllText(logs, $"[{date}] - Transfer unwanted {Pokename} CP {pokemon.Cp}" + Environment.NewLine);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Enqueuing to BULK Transfer unwanted {Pokename} CP {pokemon.Cp} ");
                    RandomHelper.RandomSleep(400, 600);
                }
            }

            if (!idsToTransfer.Any())
                return;

            var _response = Logic.objClient.Inventory.TransferPokemons(idsToTransfer);

            if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer Successful of {idsToTransfer.Count} pokemons => {_response.CandyAwarded.ToString()} candy/ies awarded.");
                RandomHelper.RandomSleep(1000, 2000);
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Something happened while transferring pokemons.");
            }

        }

        private static void TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false, bool transferFirstLowIv = false)
        {
            if (GlobalVars.RelocateDefaultLocation)
            {
                return;
            }
            if (GlobalVars.TransferDoublePokemons)
            {
                var profil = Logic.objClient.Player.GetPlayer();
                RandomHelper.RandomSleep(300, 400);

                if (GlobalVars.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stopping to transfer some Pokemons.");
                    GlobalVars.PauseTheWalking = true;
                }
                var buddyId = (profil.PlayerData.BuddyPokemon!=null) ? profil.PlayerData.BuddyPokemon.Id : 0;

                TransferUnwantedPokemon(buddyId);

                var duplicatePokemons = Logic.objClient.Inventory.GetDuplicatePokemonToTransfer(GlobalVars.HoldMaxDoublePokemons, keepPokemonsThatCanEvolve, transferFirstLowIv);
                var pokemonsToTransfer = new List<ulong>();
                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    var Pokename = duplicatePokemon.PokemonId.ToString();
                    var IVPercent = PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00");

                    if (!GlobalVars.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp >= GlobalVars.DontTransferWithCPOver || PokemonInfo.CalculatePokemonPerfection(duplicatePokemon) >= GlobalVars.ivmaxpercent)
                        {
                            continue; // go to next itearion from foreach
                        }

                        if (buddyId == duplicatePokemon.Id)
                        {
                            Logger.Warning($"Pokemon {Pokename} with {IVPercent}% IV Is your buddy so can not be transfered");
                            continue;// go to next itearion from foreach
                        }

                        var bestPokemonOfType = Logic.objClient.Inventory.GetHighestCPofType(duplicatePokemon);
                        var bestPokemonsCpOfType = Logic.objClient.Inventory.GetHighestCPofType2(duplicatePokemon);
                        var bestPokemonsIvOfType = Logic.objClient.Inventory.GetHighestIVofType(duplicatePokemon);

                        pokemonsToTransfer.Add(duplicatePokemon.Id);

                        var logs = Path.Combine(logPath, "TransferLog.txt");
                        var date = DateTime.Now.ToString();

                        if (transferFirstLowIv)
                        {
                            var BestIV = PokemonInfo.CalculatePokemonPerfection(bestPokemonsIvOfType.First()).ToString("0.00");
                            if (GlobalVars.LogTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Your best is: {BestIV}% IV)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Enqueuing to BULK Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Your best is: {BestIV}% IV)");
                        }
                        else
                        {
                            if (GlobalVars.LogTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Best: {bestPokemonsCpOfType.First().Cp} CP)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Enqueuing to BULK Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Best: {bestPokemonsCpOfType.First().Cp} CP)");
                        }

                        if (Logic.Instance.Telegram != null)
                            Logic.Instance.Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Transfer, StringUtils.getPokemonNameByLanguage(duplicatePokemon.PokemonId), duplicatePokemon.Cp, PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00"), bestPokemonOfType);

                        RandomHelper.RandomSleep(400, 600);
                    }
                }
                if (pokemonsToTransfer.Any())
                {
                    var _response = Logic.objClient.Inventory.TransferPokemons(pokemonsToTransfer);

                    if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Transfer Successful of " + pokemonsToTransfer.Count + " pokemons => " + _response.CandyAwarded + ((_response.CandyAwarded == 1) ? " candy" : " candies") + " awarded.", LogLevel.Info);
                        RandomHelper.RandomSleep(1000, 2000);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Something happened while transferring pokemons.");
                    }

                    if (GlobalVars.pauseAtEvolve2)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons transfered. Time to continue our journey!");
                        GlobalVars.PauseTheWalking = false;
                    }
                }
            }
        }

        public static void LimitReached(string limit)
        {
            if (limit != "")
                Logger.Info($"You have reached {limit} limit");
            if ((GlobalVars.RestartAfterRun < 1) || (limit == "")){
                Logger.Info("We are closing the Bot for you! Wait 10 seconds");
                RandomHelper.RandomSleep(10000,10001);
                Environment.Exit(-1);
            }else{
                Logger.Info($"Waiting {GlobalVars.RestartAfterRun} minutes");
                for (var i= GlobalVars.RestartAfterRun; i>0; i--)
                {
                    Logger.Info($"{i} minutes left");
                    RandomHelper.RandomSleep(60000,61000);
                }
                lastlog = -10000;
                timetorunstamp = -10000;
                Logic.objClient.ReadyToUse = false;
                Logic.Instance.Execute();
            }
        }
        private static DateTime _lastegguse;

        public static void UseLuckyEgg(Client client)
        {
            var luckyEgg = client.Inventory.GetItemData( ItemId.ItemLuckyEgg);

            if (_lastegguse > DateTime.Now.AddSeconds(5))
            {
                TimeSpan duration = _lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0) { return; }

            client.Inventory.UseItemXpBoost(ItemId.ItemLuckyEgg);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            _lastegguse = DateTime.Now.AddMinutes(30);
            RandomHelper.RandomSleep(3000, 3100);
        }

        public static IEnumerable<PokemonSettings> GetPokemonSettings()
        {
            var templates =   Logic.objClient.Download.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public static List<Candy> GetPokemonFamilies()
        {
            var inventory =  Logic.objClient.Inventory.GetInventory();

            var families = from item in inventory.InventoryDelta.InventoryItems
                where item.InventoryItemData?.Candy != null
                where item.InventoryItemData?.Candy.FamilyId != PokemonFamilyId.FamilyUnset
                group item by item.InventoryItemData?.Candy.FamilyId into family
                select new Candy
            {
                FamilyId = family.FirstOrDefault().InventoryItemData.Candy.FamilyId,
                Candy_ = family.FirstOrDefault().InventoryItemData.Candy.Candy_
            };

            return families.ToList();
        }
        
        public static IEnumerable<PokemonData> GetPokemonToEvolve(bool forceRequest = false, IEnumerable<PokemonId> filter = null)
        {
            var myPokemons =  Logic.objClient.Inventory.GetPokemons();

            myPokemons = myPokemons.Where(p => p.DeployedFortId == string.Empty).OrderByDescending(p => p.Cp); //Don't evolve pokemon in gyms

            if (filter != null)
            {
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            }
            var pokemons = myPokemons.ToList();

            var myPokemonSettings =  GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies =  GetPokemonFamilies();
            var pokemonFamilies = myPokemonFamilies.ToArray();

            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId);

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                if (settings == null || familyCandy == null)
                {
                    continue;
                }

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(
                    p => pokemonSettings.SingleOrDefault(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId)
                    * settings.CandyToEvolve;

                if (familyCandy.Candy_ - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }
            return pokemonToEvolve;
        }

        public static Version GetServerVersion()
        {
            try {
                using (var wC = new WebClient())
                {
                    var strVersion = wC.DownloadString("https://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/ver.md");
                    return new Version( strVersion);
                }
            } catch (Exception) {
                return Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        public static void CheckWhileWalking()
        {
            if (GetServerVersion() > Assembly.GetEntryAssembly().GetName().Version)
            {
                Logger.Warning("There is an Update on Github");
                var dialogResult = MessageBox.Show(
                    "There is an Update on Github. do you want to open it ?", $"Newest Version: {GetServerVersion()}, MessageBoxButtons.YesNo");
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        Process.Start("https://github.com/Logxn/PokemonGo-Bot");
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
}
