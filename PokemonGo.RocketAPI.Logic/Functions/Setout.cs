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

        private static DateTime _lastegguse;
        public static void UseLuckyEgg(Client client)
        {
            var luckyEgg = Logic.objClient.Inventory.GetItemData( ItemId.ItemLuckyEgg);

            if (_lastegguse > DateTime.Now.AddSeconds(5))
            {
                TimeSpan duration = _lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0) { return; }

            client.Inventory.UseItemXpBoost();
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            _lastegguse = DateTime.Now.AddMinutes(30);
            RandomHelper.RandomSleep(3000, 3100);
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
            RecycleItems();

            CheckLevelUp(Logic.objClient);
            
            if (GlobalVars.ShowStats)
                StatsLog(Logic.objClient);

            if (GlobalVars.CheckWhileRunning)
                Update.CheckWhileWalking();

            RefreshConsoleTitle(Logic.objClient);
            
            Logic.ClientReadyToUse = true;
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
                
                var incsense = Logic.objClient.Inventory.GetItemData( ItemId.ItemIncenseOrdinary);
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

                Logic.objClient.Inventory.UseIncense(ItemId.ItemIncenseOrdinary);
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                RandomHelper.RandomSleep(1100);
            }
        }

        public static IEnumerable<PokemonSettings> GetPokemonSettings()
        {
            var templates =  Logic.objClient.Download.GetItemTemplates().Result;
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public static List<Candy> GetPokemonFamilies()
        {
            var inventory = Logic.objClient.Inventory.GetInventory().Result;

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

        public static ItemId GeteNeededItemToEvolve(PokemonId pokeId)
        {
                var item = ItemId.ItemUnknown;
                switch (pokeId) {
                    case PokemonId.Seadra:
                        item = ItemId.ItemDragonScale;
                        break;
                    case PokemonId.Poliwhirl:
                    case PokemonId.Slowpoke:
                        item = ItemId.ItemKingsRock;
                        break;
                    case PokemonId.Scyther:
                    case PokemonId.Onix:
                        item = ItemId.ItemMetalCoat;
                        break;
                    case PokemonId.Porygon:
                        item = ItemId.ItemUpGrade;
                        break;
                    case PokemonId.Gloom:
                    case PokemonId.Sunkern:
                        item = ItemId.ItemSunStone;
                        break;
                }
                return item;
        }

        private static void EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter )
        {
            int evolvecount = 0;
            int gotXP = 0;

            if ( GlobalVars.RelocateDefaultLocation)
            {
                return;
            }
            EvolvePokemonResponse evolvePokemonOutProto;

            var pokemonToEvolve = GetPokemonToEvolve(true,filter);
            var toEvolveCount = pokemonToEvolve.Count();
            var startEvolving = (toEvolveCount==0 || toEvolveCount==GlobalVars.EvolveAt );

            if (startEvolving && GlobalVars.UseLuckyEgg)
                    UseLuckyEgg(Logic.objClient);

            foreach (var pokemon in pokemonToEvolve)
            {

                var item = GeteNeededItemToEvolve(pokemon.PokemonId);
                var itemData = Logic.objClient.Inventory.GetItemData( item);
                
                if (item != ItemId.ItemUnknown && itemData.Count < 1){
                    if ( pokemon.PokemonId == PokemonId.Poliwhirl
                        || pokemon.PokemonId == PokemonId.Gloom
                        || pokemon.PokemonId == PokemonId.Slowpoke
                       )
                        item = ItemId.ItemUnknown; // try to evolve without items
                    else
                        continue; // go to next pokemon
                }
                evolvePokemonOutProto = Logic.objClient.Inventory.EvolvePokemon(pokemon.Id, item).Result;

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
                var incubators = Logic.objClient.Inventory.GetIncubators();
                var unusedEggs = Logic.objClient.Inventory.GetEggs().Where(x => string.IsNullOrEmpty(x.EggIncubatorId)).OrderBy(x => x.EggKmWalkedTarget - x.EggKmWalkedStart);
                var pokemons = Logic.objClient.Inventory.GetPokemons();

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
                ret = ret.Where(x => (int)x.EggKmWalkedTarget !=2).ToList();
            }
            if(GlobalVars.No5kmEggsBasicInc)
            {
                ret = ret.Where(x => (int)x.EggKmWalkedTarget !=5).ToList();
            }
            if(GlobalVars.No10kmEggsBasicInc)
            {
                ret = ret.Where(x => (int)x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }
        
        private static List<POGOProtos.Data.PokemonData> eggsHatchingAllowed(object eggs)
        {
            var ret = new List<POGOProtos.Data.PokemonData> ((List<POGOProtos.Data.PokemonData> ) eggs);
            if(GlobalVars.No2kmEggs)
            {
                ret = ret.Where(x => (int) x.EggKmWalkedTarget !=2).ToList();
            }
            if(GlobalVars.No5kmEggs)
            {
                ret = ret.Where(x => (int) x.EggKmWalkedTarget !=5).ToList();
            }
            if(GlobalVars.No10kmEggs)
            {
                ret = ret.Where(x => (int) x.EggKmWalkedTarget !=10).ToList();
            }
            return ret;
        }

        public static IEnumerable<ItemData> GetItemsToRecycle(ICollection<KeyValuePair<ItemId, int>> itemRecycleFilter)
        {
            var myItems = Logic.objClient.Inventory.GetItemsData();
            return myItems
                .Where(x => itemRecycleFilter.Any(f => f.Key == ((ItemId)x.ItemId) && x.Count > f.Value))
                .Select(x => new ItemData { ItemId = x.ItemId, Count = x.Count - itemRecycleFilter.Single(f => f.Key == (ItemId)x.ItemId).Value, Unseen = x.Unseen });
        }

        private static void RecycleItems(bool forcerefresh = false)
        {

            if (GlobalVars.RelocateDefaultLocation)
                return;
            var items = GetItemsToRecycle(GlobalVars.GetItemFilter());

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

            var inventory = client.Inventory.GetInventory();
            var playerStats = client.Inventory.GetPlayerStats();
            var stats = playerStats.First();
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            if ((int) startingXp == -10000) startingXp = stats.Experience;

            currentxp = stats.Experience;

            var numDifferentPokemons = Enum.GetNames(typeof(PokemonId)).Length -1;
            var pokemonToEvolve = (GetPokemonToEvolve()).Count();
            var pokedexpercentraw = Convert.ToDouble(stats.UniquePokedexEntries) / Convert.ToDouble(numDifferentPokemons) * 100;
            var pokedexpercent = Math.Floor(pokedexpercentraw);


            var items = Logic.objClient.Inventory.GetItemsData();
            var pokemonCount = client.Inventory.GetPokemons().Count();
            var eggCount = client.Inventory.GetEggs().Count();
            var maxPokemonStorage = client.Player.PlayerData.MaxPokemonStorage;
            var maxItemStorage = client.Player.PlayerData.MaxItemStorage;
            var stardust = client.Player.PlayerData.Currencies.ToArray()[1].Amount.ToString("N0");
            var currEXP = curexp.ToString("N0");
            var neededEXP = expneeded.ToString("N0");
            var expPercent = Math.Round(curexppercent, 2);

            Logic.ShowingStats = true;
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "-----------------------[PLAYER STATS]-----------------------");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Level/EXP: {stats.Level} | {currEXP}/{neededEXP} ({expPercent}%)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"EXP to Level up: {(stats.NextLevelXp - stats.Experience)}");
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

            Logic.ShowingStats = false;


        }

        private static void CheckLevelUp(Client client)
        {
            var stats = client.Inventory.GetPlayerStats().First();
            
            if (stats.Level == 1 && stats.NextLevelXp == 1000)
                client.Misc.MarkTutorialComplete(new Google.Protobuf.Collections.RepeatedField<TutorialState> {new TutorialState()});


            if (level == -1) {
                level = stats.Level;
            }
            else if (stats.Level > level) {
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
        }

        public static void RefreshConsoleTitle(Client client)
        {
            var playerStats = client.Inventory.GetPlayerStats();
            var stats = playerStats.First();
            var expneeded = stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexp = stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level);
            var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;

            string TitleText = client.Player.PlayerData.Username + @" Lvl " + stats.Level + @" (" +
            (stats.Experience - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"/" +
            (stats.NextLevelXp - stats.PrevLevelXp - StringUtils.getExpDiff(stats.Level)).ToString("N0") + @"|" +
            Math.Round(curexppercent, 2) + @"%) Stardust: " + client.Player.PlayerData.Currencies.ToArray()[1].Amount + @" " +
            Logic.Instance.BotStats;

            if (!GlobalVars.EnableConsoleInTab) System.Console.Title = TitleText;

            if (GlobalVars.EnablePokeList && Logic.ClientReadyToUse)
            {
                try {
                    Application.OpenForms[0].Invoke(new Action(() => Application.OpenForms[0].Text = TitleText));
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

                if (GlobalVars.TimeToRun > 0)
                {
                    if ((int) timetorunstamp == -10000)
                    {
                        timetorunstamp = GlobalVars.TimeToRun * 60 * 1000 + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                    }
                    else
                    {
                        var runTimeRemaining = timetorunstamp - (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
                        var remainingTime = Math.Round(runTimeRemaining / 1000 / 60, 2);
                        if (runTimeRemaining <= 0)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Time To Run Reached or Exceeded...Walking back to default location and stopping bot");

                            Logic.Instance.WalkWithRouting(GlobalVars.latitude, GlobalVars.longitude);

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

                if (GlobalVars.UseBreakFields)
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
                    else if ((int) resumetimestamp == -10000)
                    {
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

                if ((int) startingXp != -10000 && (int) currentxp != -10000 && (currentxp = -startingXp) >= GlobalVars.XPFarmedLimit)
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
            var pokemons = Logic.objClient.Inventory.GetPokemons();
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

            var _response = Logic.objClient.Inventory.TransferPokemons(idsToTransfer).Result;

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
                RandomHelper.RandomSleep(300, 400);

                if (GlobalVars.pauseAtEvolve2)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Stopping to transfer some Pokemons.");
                    GlobalVars.PauseTheWalking = true;
                }

                var buddyid = (Logic.objClient.Player.PlayerData.BuddyPokemon !=null)?Logic.objClient.Player.PlayerData.BuddyPokemon.Id:0;
                TransferUnwantedPokemon(buddyid);

                var duplicatePokemons = GetDuplicatePokemonToTransfer(GlobalVars.HoldMaxDoublePokemons, keepPokemonsThatCanEvolve, transferFirstLowIv);
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

                        if (Logic.objClient.Player.PlayerData.BuddyPokemon.Id == duplicatePokemon.Id)
                        {
                            Logger.Warning($"Pokemon {Pokename} with {IVPercent}% IV Is your buddy so can not be transfered");
                            continue;// go to next itearion from foreach
                        }

                        var bestPokemonOfType = GetHighestCPofType(duplicatePokemon);
                        var bestPokemonsCpOfType = GetHighestCPofType2(duplicatePokemon);
                        var bestPokemonsIvOfType = GetHighestIVofType(duplicatePokemon);

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
                    var _response = Logic.objClient.Inventory.TransferPokemons(pokemonsToTransfer).Result;

                    if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Transfer Successful of " + pokemonsToTransfer.Count + " pokemons => " + _response.CandyAwarded + ((_response.CandyAwarded == 1) ? " candy" : " candies") + " awarded.", Logger.LogLevel.Info);
                        PokemonGo.RocketAPI.Helpers.RandomHelper.RandomSleep(1000, 2000);
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
                Logic.ClientReadyToUse = false;
                Logic.Instance.Execute();
            }
        }
        public static IEnumerable<PokemonData> GetDuplicatePokemonToTransfer(int holdMaxDoublePokemons, bool keepPokemonsThatCanEvolve = false, bool orderByIv = false)
        {

            var pokemonList = Logic.objClient.Inventory.GetPokemons()
                .Where(p => p.DeployedFortId == string.Empty && p.Favorite == 0);

            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId);

                var myPokemonSettings =  GetPokemonSettings();
                var pokemonSettings = myPokemonSettings as IList<PokemonSettings> ?? myPokemonSettings;

                var myPokemonFamilies =  GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies;

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId);
                    var amountToSkip = 0;

                    if (settings.CandyToEvolve != 0)
                    {
                        amountToSkip = familyCandy.Candy_ / settings.CandyToEvolve;
                    }

                    if (holdMaxDoublePokemons > amountToSkip)
                    {
                        amountToSkip = holdMaxDoublePokemons;
                    }
                    if (orderByIv)
                    {
                        results.AddRange( (IEnumerable<PokemonData>) pokemonList.Where(x => x.PokemonId == pokemon.Key)
                                                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                                                    .ThenBy(n => n.StaminaMax)
                                                    .Skip(amountToSkip)
                                                    .ToList());
                    }
                    else
                    {
                        results.AddRange( (IEnumerable<PokemonData>) pokemonList.Where(x => x.PokemonId == pokemon.Key)
                            .OrderByDescending(x => x.Cp)
                            .ThenBy(n => n.StaminaMax)
                            .Skip(amountToSkip)
                            .ToList());
                    }

                }

                return results;
            }

            if (orderByIv)
            {
                return pokemonList
                    .GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 0)
                    .SelectMany(p => p.Where(x => x.Favorite == 0)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(holdMaxDoublePokemons)
                    .ToList());

            }
            else
            {
                return pokemonList
                    .GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 0)
                    .SelectMany(p => p.Where(x => x.Favorite == 0)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(holdMaxDoublePokemons)
                    .ToList());
            }
        }

        public static IEnumerable<PokemonData> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon = Logic.objClient.Inventory.GetPokemons();
            return myPokemon.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public static IEnumerable<PokemonData> GetHighestIVofType(PokemonData pokemon)
        {
            var myPokemon = Logic.objClient.Inventory.GetPokemons();
            return myPokemon.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(x => x.Cp);
        }

        public static IEnumerable<PokemonData> GetHighestCPofType2(PokemonData pokemon)
        {
            var myPokemon = Logic.objClient.Inventory.GetPokemons();
            return myPokemon.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(PokemonInfo.CalculatePokemonPerfection);
        }

        public static int GetHighestCPofType(PokemonData pokemon)
        {
            var myPokemon = Logic.objClient.Inventory.GetPokemons();
            return myPokemon.Where(x => x.PokemonId == pokemon.PokemonId)
                            .OrderByDescending(x => x.Cp)
                            .FirstOrDefault().Cp;
        }        
        public static void ExportPokemonToCSV(PlayerData player, string filename = "PokemonList.csv")
        {
            if (player == null)
                return;
            var stats =  Logic.objClient.Inventory.GetPlayerStats();
            var stat = stats.FirstOrDefault();
            if (stat == null)
                return;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

            if (Directory.Exists(path))
            {
                try
                {
                    const string header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    string pokelist_file = Path.Combine(path, $"Profile_{player.Username}_{filename}");
                    if (File.Exists(pokelist_file))
                        File.Delete(pokelist_file);
                    string ls = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    File.WriteAllText(pokelist_file, header.Replace(",", ls));

                    var AllPokemon =  GetHighestsPerfect();
                    var myPokemonSettings =  GetPokemonSettings();
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies =  GetPokemonFamilies();
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    int trainerLevel = stat.Level;
                    int[] exp_req = new[] { 0, 1000, 3000, 6000, 10000, 15000, 21000, 28000, 36000, 45000, 55000, 65000, 75000, 85000, 100000, 120000, 140000, 160000, 185000, 210000, 260000, 335000, 435000, 560000, 710000, 900000, 1100000, 1350000, 1650000, 2000000, 2500000, 3000000, 3750000, 4750000, 6000000, 7500000, 9500000, 12000000, 15000000, 20000000 };
                    int exp_req_at_level = exp_req[stat.Level - 1];

                    using (var w = File.AppendText(pokelist_file))
                    {
                        w.WriteLine("");
                        foreach (var pokemon in AllPokemon)
                        {
                            string toEncode = $"{(int)pokemon.PokemonId}" + "," + trainerLevel + "," + PokemonInfo.GetLevel(pokemon) + "," + pokemon.Cp + "," + pokemon.Stamina;
                            //Generate base64 code to make it viewable here http://poke.isitin.org/#MTUwLDIzLDE3LDE5MDIsMTE4
                            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toEncode));

                            string IsInGym = string.Empty;
                            string IsFavorite = string.Empty;

                            IsFavorite = pokemon.Favorite != 0 ? "Yes" : "No";

                            var settings = pokemonSettings.SingleOrDefault(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.SingleOrDefault(x => settings.FamilyId == x.FamilyId).Candy_;
                            string perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string content_part1 = $"{(int)pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
                            string content_part2 = $",{pokemon.Move1},{pokemon.Move2},{pokemon.Stamina},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{familiecandies},{IsInGym},{IsFavorite},http://poke.isitin.org/#{encoded}";
                            var str1 = content_part1.Replace(",", ls);
                            var str2= content_part2.Replace(",", ls);
                            string content = $"{str1}\"{perfection}\"{str2}";
                            w.WriteLine($"{content}");

                        }
                        w.Close();
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Export Player Infos and all Pokemon to \"\\Config\\{filename}\"");
                }
                catch
                {
                    Logger.Error("Export Player Infos and all Pokemons to CSV not possible. File seems be in use!"/*, LogLevel.Warning*/);
                }
            }
        }
    }
}
