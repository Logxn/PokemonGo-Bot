/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 25/01/2017
 * Time: 19:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using POGOProtos.Enums;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Shared;
using PokemonGo.RocketAPI.Logic.Utils;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Logic.Functions
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
        public static  bool havelures;
        public static  int level = -1;
        public static double timetorunstamp = -10000;
        public static double pausetimestamp = -10000;
        public static double resumetimestamp = -10000;
        public static double lastlog = -10000;
        public static int pokemonCatchCount;
        public static int pokeStopFarmedCount;
        private static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");



        public static void Execute()
        {
            // reset stat counter
            count = 0;
            CheckIfUseIncense();

            if (Shared.GlobalVars.UseLuckyEggIfNotRunning || Shared.GlobalVars.UseLuckyEggGUIClick)
            {
                Shared.GlobalVars.UseLuckyEggGUIClick = false;
                Logic.objClient.Inventory.UseLuckyEgg(Logic.objClient).Wait();
            }

            if (Shared.GlobalVars.EvolvePokemonsIfEnoughCandy)
                EvolveAllPokemonWithEnoughCandy(Shared.GlobalVars.pokemonsToEvolve);

            if (Shared.GlobalVars.AutoIncubate)
                StartIncubation();

            TransferDuplicatePokemon(Shared.GlobalVars.keepPokemonsThatCanEvolve, Shared.GlobalVars.TransferFirstLowIV);
            RecycleItems();
            StatsLog(Logic.objClient);
            Logic.objClient.ReadyToUse = true;
            Logger.Debug("Client is ready to use");
            SetCheckTimeToRun();
        }


        public static void CheckIfUseIncense()
        {
            Logger.Debug("Checking use of incense");
            if (Shared.GlobalVars.RelocateDefaultLocation)
                return;
            if (Shared.GlobalVars.UseIncense || Shared.GlobalVars.UseIncenseGUIClick)
            {
                Logger.Debug("Use incense selected");
                Shared.GlobalVars.UseIncenseGUIClick = false;
                var inventory = Logic.objClient.Inventory.GetItems().Result;
                var incsense = inventory.FirstOrDefault(p => p.ItemId == ItemId.ItemIncenseOrdinary);
                var loginterval = DateTime.Now - LastIncenselog;
                Logger.Debug("loginterval: "+ loginterval);
                Logger.Debug("lastincenseuse: "+ lastincenseuse);
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

                Logic.objClient.Inventory.UseIncense(ItemId.ItemIncenseOrdinary).Wait();
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                RandomHelper.RandomSleep(1100);
            }
        }

        private static void EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter )
        {
            int evolvecount = 0;
            int gotXP = 0;

            if ( Shared.GlobalVars.RelocateDefaultLocation)
            {
                return;
            }
            var pokemonToEvolve = Logic.objClient.Inventory.GetPokemonToEvolve(filter).Result;
            var toEvolveCount = pokemonToEvolve.Count();
            var startEvolving = (toEvolveCount==0 || toEvolveCount==Shared.GlobalVars.EvolveAt );

            if (startEvolving && Shared.GlobalVars.UseLuckyEgg)
                    Logic.objClient.Inventory.UseLuckyEgg(Logic.objClient).Wait();

            foreach (var pokemon in pokemonToEvolve)
            {
                var evolvePokemonOutProto = Logic.objClient.Inventory.EvolvePokemon(pokemon.Id).Result;
                var date = DateTime.Now.ToString();
                var evolvelog = Path.Combine(logPath, "EvolveLog.txt");

                var getPokemonName = pokemon.PokemonId.ToString();
                var cp = pokemon.Cp;
                var calcPerf = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                var getEvolvedName = StringUtils.getPokemonNameByLanguage( evolvePokemonOutProto.EvolvedPokemonData.PokemonId);
                var getEvolvedCP = evolvePokemonOutProto.EvolvedPokemonData.Cp;
                gotXP = gotXP + evolvePokemonOutProto.ExperienceAwarded;

                if (evolvePokemonOutProto.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    if(evolvecount == 0)
                        if (Shared.GlobalVars.pauseAtEvolve2)
                        {
                            Logger.Info("Stopping to evolve some Pokemons.");
                            Shared.GlobalVars.PauseTheWalking = true;
                        }

                    var experienceAwarded =evolvePokemonOutProto.ExperienceAwarded.ToString("N0");
                    if (Shared.GlobalVars.LogEvolve)
                    {
                        File.AppendAllText(evolvelog, $"[{date}] - Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {experienceAwarded} XP" + Environment.NewLine);
                    }
                    Logger.Info( $"Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {experienceAwarded} XP");
                    Logic.Instance.BotStats.AddExperience(evolvePokemonOutProto.ExperienceAwarded);

                    if (Logic.Instance.Telegram != null)
                        Logic.Instance.Telegram.sendInformationText(TelegramUtil.TelegramUtilInformationTopics.Evolve, StringUtils.getPokemonNameByLanguage( pokemon.PokemonId), pokemon.Cp, PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00"), StringUtils.getPokemonNameByLanguage( evolvePokemonOutProto.EvolvedPokemonData.PokemonId), evolvePokemonOutProto.EvolvedPokemonData.Cp, evolvePokemonOutProto.ExperienceAwarded.ToString("N0"));
                    evolvecount++;
                }
                else
                {
                    if (evolvePokemonOutProto.Result != EvolvePokemonResponse.Types.Result.Success)
                    {
                        if (Shared.GlobalVars.LogEvolve)
                        {
                            File.AppendAllText(evolvelog, $"[{date}] - Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}" + Environment.NewLine);
                        }
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}");
                        evolvecount++;
                    }
                }

                if (Shared.GlobalVars.UseAnimationTimes)
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
                var strGotXP = gotXP.ToString("N0");
                Logger.Info($"Evolved {evolvecount} Pokemons. We have got {strGotXP} XP.");

                if (Shared.GlobalVars.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons evolved. Time to continue our journey!");
                    Shared.GlobalVars.PauseTheWalking = false;
                }
            }

        }

        // To store incubators with eggs
        private static List<IncubatorUsage> rememberedIncubators = new List<IncubatorUsage>();
        private static void StartIncubation()
        {
            try
            {
                if ( Shared.GlobalVars.RelocateDefaultLocation)
                {
                    return;
                }
                var inventory = Logic.objClient.Inventory.GetInventory().Result;
                var incubators = Logic.objClient.Inventory.GetEggIncubators(inventory).ToList();
                var unusedEggs = (Logic.objClient.Inventory.GetEggs(inventory)).Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart).ToList();
                var pokemons = (Logic.objClient.Inventory.GetPokemons().Result).ToList();

                var playerStats = Logic.objClient.Inventory.GetPlayerStats(inventory);
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

                    if (Shared.GlobalVars.LogEggs)
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
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "There is not Allowed Eggs to hatch");
                    return;
                }

                var newRememberedIncubators = new List<IncubatorUsage>();

                foreach (EggIncubator incubator in incubators)
                {
                    if (incubator.PokemonId == 0)
                    {

                        // If is basic incubator and user don't want use it, we go to the next incubator
                        if (    (incubator.ItemId == ItemId.ItemIncubatorBasic) 
                             && ( ! Shared.GlobalVars.UseBasicIncubators) )
                            continue;

                        POGOProtos.Data.PokemonData egg;
                        if (incubator.ItemId == ItemId.ItemIncubatorBasic) 
                            egg = Shared.GlobalVars.EggsAscendingSelectionBasicInc ? unusedEggsBasicInc.FirstOrDefault() : unusedEggsBasicInc.LastOrDefault();
                        else 
                            egg = Shared.GlobalVars.EggsAscendingSelection ? unusedEggsUnlimitInc.FirstOrDefault() : unusedEggsUnlimitInc.LastOrDefault();

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
            if(Shared.GlobalVars.No2kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=2).ToList();
            }
            if(Shared.GlobalVars.No5kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=5).ToList();
            }
            if(Shared.GlobalVars.No10kmEggsBasicInc)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }
        
        private static List<POGOProtos.Data.PokemonData> eggsHatchingAllowed(object eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> ((List<POGOProtos.Data.PokemonData> ) eggs);
            if(Shared.GlobalVars.No2kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=2).ToList();
            }
            if(Shared.GlobalVars.No5kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=5).ToList();
            }
            if(Shared.GlobalVars.No10kmEggs)
            {
                ret = ret.Where(x => x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }

        private static void RecycleItems(bool forcerefresh = false)
        {

            if (Shared.GlobalVars.RelocateDefaultLocation)
                return;
            var items = Logic.objClient.Inventory.GetItemsToRecycle(Shared.GlobalVars.GetItemFilter()).Result;

            foreach (var item in items)
            {
                if ((item.ItemId == ItemId.ItemPokeBall || item.ItemId == ItemId.ItemGreatBall || item.ItemId == ItemId.ItemUltraBall || item.ItemId == ItemId.ItemMasterBall) && Logic.Instance.pokeballoutofstock)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Detected Pokeball Restock - Enabling Catch Pokemon");
                    Logic.Instance.logicAllowCatchPokemon = true;
                    Logic.Instance.pokeballoutofstock = false;
                }
                var transfer = Logic.objClient.Inventory.RecycleItem(item.ItemId, item.Count).Result;
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {item.ItemId}");
                RandomHelper.RandomSleep(1000, 5000);
            }
        }

        private static void StatsLog(Client client)
        {

            #region Set Stat Variables

            var profile = client.Player.GetPlayer().Result;
            var inventory = client.Inventory.GetInventory().Result;
            var playerStats = client.Inventory.GetPlayerStats(inventory);
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

            var items = client.Inventory.GetItems(inventory); 
            var pokemonCount = client.Inventory.GetPokemons().Result.Count();
            var eggCount = client.Inventory.GetEggsCount(inventory);
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
            RefreshConsoleTitle(client);
            #endregion

            #region Check for Update

            if (Shared.GlobalVars.CheckWhileRunning)
            {
                Update.CheckWhileWalking();
            }

            #endregion
            client.ShowingStats = false;
        }

        public static void RefreshConsoleTitle(Client client)
        {
            var profile = client.Player.GetPlayer().Result;
            var inventory = client.Inventory.GetInventory().Result;
            var playerStats = client.Inventory.GetPlayerStats(inventory);
            var stats = playerStats.First();
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            string TitleText = profile.PlayerData.Username + @" Lvl " + stats.Level + @" (" +
            (stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"/" +
            (stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"|" +
            Math.Round(curexppercent, 2) + @"%) Stardust: " + profile.PlayerData.Currencies.ToArray()[1].Amount + @" " +
            Logic.Instance.BotStats;

            if (!GlobalVars.EnableConsoleInTab) System.Console.Title = TitleText;

            if (GlobalVars.EnablePokeList && client.ReadyToUse)
            {
                try {
                    Application.OpenForms["Pokemons"].Invoke(new Action(() => Application.OpenForms["Pokemons"].Text = TitleText));
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
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

                if (Shared.GlobalVars.TimeToRun > 0)
                {
                    if (timetorunstamp == -10000)
                    {
                        timetorunstamp = Shared.GlobalVars.TimeToRun * 60 * 1000 + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    }
                    else
                    {
                        var runTimeRemaining = timetorunstamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        var remainingTime = Math.Round(runTimeRemaining / 1000 / 60, 2);
                        if (runTimeRemaining <= 0)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Time To Run Reached or Exceeded...Walking back to default location and stopping bot");

                            Logic.Instance.WalkWithRouting(Shared.GlobalVars.latitude, Shared.GlobalVars.longitude);

                            LimitReached("Time to Run");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time to Run: {remainingTime} minutes");
                        }
                    }
                }

                #endregion

                #region Breaks

                if (Shared.GlobalVars.UseBreakFields)
                {
                    if (pausetimestamp > -10000)
                    {
                        var walkTimeRemaining = pausetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        if (walkTimeRemaining <= 0)
                        {
                            pausetimestamp = -10000;
                            Shared.GlobalVars.pauseAtPokeStop = true;
                            resumetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds +Shared.GlobalVars.BreakLength * 60 * 1000;

                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Break Time! Pause walking for {Shared.GlobalVars.BreakLength} minutes");
                        }
                        else
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {Math.Round(walkTimeRemaining / 1000 / 60, 2)} minutes");
                        }
                    }
                    else if (resumetimestamp == -10000)
                    {
                        pausetimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds + Shared.GlobalVars.BreakInterval * 60 * 1000;

                        Logger.ColoredConsoleWrite(ConsoleColor.Blue, $"Remaining Time until break: {Shared.GlobalVars.BreakInterval} minutes");
                    }
                }

                if (resumetimestamp > -10000)
                {
                    var breakTimeRemaining = resumetimestamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;

                    if (breakTimeRemaining <= 0)
                    {
                        resumetimestamp = -10000;
                        Shared.GlobalVars.pauseAtPokeStop = false;

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
                if (!Shared.GlobalVars.CatchPokemon)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Catching Pokemon Disabled in Client Settings - Skipping all pokemon");
                }

                #endregion

                #region Check Run Count Limits

                #region Catch Pokemon Count Check

                if (pokemonCatchCount >= Shared.GlobalVars.PokemonCatchLimit)
                {
                    if (Shared.GlobalVars.FarmPokestops)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached - Bot will only farm pokestops");

                        Logic.Instance.logicAllowCatchPokemon = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemon Catch Limit Reached and not farming pokestops - Bot will return to default location and stop");

                        Logic.Instance.WalkWithRouting(Shared.GlobalVars.latitude, Shared.GlobalVars.longitude);

                        LimitReached("Catched Pokemon");
                    }
                }

                #endregion

                #region Farm Pokestops Check

                if (pokeStopFarmedCount >= Shared.GlobalVars.PokestopFarmLimit)
                {
                    if (Shared.GlobalVars.CatchPokemon && Logic.Instance.logicAllowCatchPokemon)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached - Bot will only catch pokemon");

                        Shared.GlobalVars.FarmPokestops = false;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokestop Farmed Limit Reached and not catching pokemon - Bot will return to default location and stop");

                        Logic.Instance.WalkWithRouting(Shared.GlobalVars.latitude, Shared.GlobalVars.longitude);

                        LimitReached("Farmed Pokestops");
                    }
                }

                #endregion

                #region XP Check

                if (startingXp != -10000 && currentxp != -10000 && (currentxp = -startingXp) >= Shared.GlobalVars.XPFarmedLimit)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "XP Farmed Limit Reached - Bot will return to default location and stop");

                    if (Shared.GlobalVars.UseGoogleMapsAPI)
                    {
                       Logic.Instance.WalkWithRouting(Shared.GlobalVars.latitude, Shared.GlobalVars.longitude);
                    }
                    else
                    {
                        var walkHome = Logic.Instance.navigation.HumanLikeWalking(
                            new GeoCoordinate(
                                Shared.GlobalVars.latitude,
                                Shared.GlobalVars.longitude),
                            Shared.GlobalVars.WalkingSpeedInKilometerPerHour,
                            Logic.Instance.ExecuteCatchAllNearbyPokemons);
                    }
                    LimitReached("Farmed XP");
                }

                #endregion

                #endregion
            }
        }

        private static  void TransferUnwantedPokemon(ulong buddyid)
        {
            if (!Shared.GlobalVars.pokemonsToAlwaysTransfer.Any())
                return;
            var pokemons = Logic.objClient.Inventory.GetPokemons(true).Result;
            var toTransfer = pokemons.Where(x => x.DeployedFortId == string.Empty && x.Favorite == 0 && !x.IsEgg && x.Id != buddyid);
            var idsToTransfer = new List<ulong>();
            var logs = Path.Combine(logPath, "TransferLog.txt");
            foreach (var pokemon in toTransfer){
                if (Shared.GlobalVars.pokemonsToAlwaysTransfer.Contains(pokemon.PokemonId)){
                    idsToTransfer.Add(pokemon.Id);
                    var Pokename = pokemon.PokemonId.ToString();
                    if (Shared.GlobalVars.LogTransfer)
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

            var _response = Logic.objClient.Inventory.TransferPokemon(idsToTransfer).Result;

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
            if (Shared.GlobalVars.RelocateDefaultLocation)
            {
                return;
            }
            if (Shared.GlobalVars.TransferDoublePokemons)
            {
                var profil = Logic.objClient.Player.GetPlayer().Result;
                RandomHelper.RandomSleep(300, 400);

                if (Shared.GlobalVars.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stopping to transfer some Pokemons.");
                    Shared.GlobalVars.PauseTheWalking = true;
                }

                TransferUnwantedPokemon(profil.PlayerData.BuddyPokemon.Id);

                var duplicatePokemons = Logic.objClient.Inventory.GetDuplicatePokemonToTransfer(Shared.GlobalVars.HoldMaxDoublePokemons, keepPokemonsThatCanEvolve, transferFirstLowIv).Result;
                var pokemonsToTransfer = new List<ulong>();
                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    var Pokename = duplicatePokemon.PokemonId.ToString();
                    var IVPercent = PokemonInfo.CalculatePokemonPerfection(duplicatePokemon).ToString("0.00");

                    if (!Shared.GlobalVars.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp >= Shared.GlobalVars.DontTransferWithCPOver || PokemonInfo.CalculatePokemonPerfection(duplicatePokemon) >= Shared.GlobalVars.ivmaxpercent)
                        {
                            continue; // go to next itearion from foreach
                        }

                        if (profil.PlayerData.BuddyPokemon.Id == duplicatePokemon.Id)
                        {
                            Logger.Warning($"Pokemon {Pokename} with {IVPercent}% IV Is your buddy so can not be transfered");
                            continue;// go to next itearion from foreach
                        }

                        var bestPokemonOfType = Logic.objClient.Inventory.GetHighestCPofType(duplicatePokemon).Result;
                        var bestPokemonsCpOfType = Logic.objClient.Inventory.GetHighestCPofType2(duplicatePokemon).Result;
                        var bestPokemonsIvOfType = Logic.objClient.Inventory.GetHighestIVofType(duplicatePokemon).Result;

                        pokemonsToTransfer.Add(duplicatePokemon.Id);

                        var logs = Path.Combine(logPath, "TransferLog.txt");
                        var date = DateTime.Now.ToString();

                        if (transferFirstLowIv)
                        {
                            var BestIV = PokemonInfo.CalculatePokemonPerfection(bestPokemonsIvOfType.First()).ToString("0.00");
                            if (Shared.GlobalVars.LogTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Your best is: {BestIV}% IV)" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Enqueuing to BULK Transfer {Pokename} CP {duplicatePokemon.Cp} IV {IVPercent} % (Your best is: {BestIV}% IV)");
                        }
                        else
                        {
                            if (Shared.GlobalVars.LogTransfer)
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
                    var _response = Logic.objClient.Inventory.TransferPokemon(pokemonsToTransfer).Result;

                    if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Transfer Successful of " + pokemonsToTransfer.Count + " pokemons => " + _response.CandyAwarded + ((_response.CandyAwarded == 1) ? " candy" : " candies") + " awarded.", LogLevel.Info);
                        Helpers.RandomHelper.RandomSleep(1000, 2000);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Something happened while transferring pokemons.");
                    }

                    if (Shared.GlobalVars.pauseAtEvolve2)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Pokemons transfered. Time to continue our journey!");
                        Shared.GlobalVars.PauseTheWalking = false;
                    }
                }
            }
        }

        public static void LimitReached(string limit)
        {
            if (limit != "")
                Logger.Info($"You have reached {limit} limit");
            if ((Shared.GlobalVars.RestartAfterRun < 1) || (limit == "")){
                Logger.Info("We are closing the Bot for you! Wait 10 seconds");
                RandomHelper.RandomSleep(10000,10001);
                Environment.Exit(-1);
            }else{
                Logger.Info($"Waiting {Shared.GlobalVars.RestartAfterRun} minutes");
                for (var i= Shared.GlobalVars.RestartAfterRun; i>0; i--)
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

    }
}
