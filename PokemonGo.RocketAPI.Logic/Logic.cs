using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using System.Net;
using System.IO;
using System.Device.Location;
using PokemonGo.RocketAPI.Helpers;
using System.Web.Script.Serialization;

namespace PokemonGo.RocketAPI.Logic
{

    public class Logic
    {
        public readonly Client _client;
        public readonly ISettings _clientSettings;
        public readonly Inventory _inventory;
        public TelegramUtil _telegram;
        public BotStats _botStats;
        private readonly Navigation _navigation;
        public const double SpeedDownTo = 10 / 3.6;
        private readonly PokeVisionUtil _pokevision;

        public Logic(ISettings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
            _inventory = new Inventory(_client);
            _botStats = new BotStats();
            _navigation = new Navigation(_client);
            _pokevision = new PokeVisionUtil();
        }

        public async Task Execute()
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "This bot is absolutely free and open-source!");
            Logger.ColoredConsoleWrite(ConsoleColor.Red, "If you've paid for it. Request a chargeback immediately!");
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Starting Execute on login server: {_clientSettings.AuthType}", LogLevel.Info);

            while (true)
            {
                try
                {
                    if (_clientSettings.AuthType == AuthType.Ptc)
                        await _client.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                    else if (_clientSettings.AuthType == AuthType.Google)
                        await _client.DoGoogleLogin();

                    if (!string.IsNullOrEmpty(_clientSettings.TelegramAPIToken) && !string.IsNullOrEmpty(_clientSettings.TelegramName))
                    {
                        try
                        {
                            _telegram = new TelegramUtil(_client, new Telegram.Bot.TelegramBotClient(_clientSettings.TelegramAPIToken), _clientSettings, _inventory);

                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "To Activate Informations with Telegram, write the Bot a message for more Informations");
                            var me = await _telegram.getClient().GetMeAsync();
                            _telegram.getClient().OnCallbackQuery += _telegram.BotOnCallbackQueryReceived;
                            _telegram.getClient().OnMessage += _telegram.BotOnMessageReceived;
                            _telegram.getClient().OnMessageEdited += _telegram.BotOnMessageReceived;
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Telegram Name: " + me.Username);
                            _telegram.getClient().StartReceiving();
                        } catch (Exception)
                        {

                        }
                    }

                    await PostLoginExecute();
                }
                catch (PtcOfflineException)
                {
                    Logger.Error("PTC Server Offline. Trying to Restart in 20 Seconds...");
                    try
                    {
                        _telegram.getClient().StopReceiving();
                    }
                    catch (Exception)
                    {

                    }
                    await Task.Delay(10000);
                }
                catch (AccessTokenExpiredException)
                {
                    Logger.Error("Server Offline, or Access Token expired. Restarting in 20 Seconds.");
                    try
                    {
                        _telegram.getClient().StopReceiving();
                    }
                    catch (Exception)
                    {

                    }
                    await Task.Delay(10000);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error: " + ex.Source);
                    Logger.Error($"{ex}");
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Trying to Restart."); 
                    try
                    {
                        _telegram.getClient().StopReceiving();
                    } catch(Exception)
                    {

                    }
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Restarting in 10 Seconds.");
                await Task.Delay(10000);
            }
        }

        public async Task PostLoginExecute()
        {

            while (true)
            {
                try
                {

                    await _client.SetServer();
                    var profil = await _client.GetProfile();
                    await _inventory.ExportPokemonToCSV(profil.Profile);
                    await StatsLog(_client);
                    if (_clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await EvolveAllPokemonWithEnoughCandy();
                    }
                    await TransferDuplicatePokemon(_clientSettings.keepPokemonsThatCanEvolve);
                    await RecycleItems();
                    await ExecuteFarmingPokestopsAndPokemons(_client);

                }
                catch (AccessTokenExpiredException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.Write($"Exception: {ex}", LogLevel.Error);
                }
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Starting again.. But waiting 10 Seconds..");
                await Task.Delay(10000);
            }
        }

        public async Task RepeatAction(int repeat, Func<Task> action)
        {
            for (int i = 0; i < repeat; i++)
                await action();
        }

        //class PokeService
        //{
        //    public bool go_online; //Online oder nicht?
        //    public double go_response; //Wie lange der Server zum Responden braucht
        //    public double go_idle; //Wie lange die go server online sind (in minuten)
        //    public double go_uptime_hour; //Uptime in Prozent die letzte Stunde
        //    public double go_uptime_day; //Uptime in Prozent die letzten 24h

        //    public bool ptc_online; //Online oder nicht?
        //    public double ptc_response; //Wie lange PTC zum responden braucht
        //    public double ptc_idle; //Wie lange ptc schon läuft
        //    public double ptc_uptime_hour; //Prozent von PTC uptime letzte stunde
        //    public double ptc_uptime_day; //Prozent von PTC uptime letzten tag
        //}

        int dontspam = 3;
        private async Task StatsLog(Client client)
        {
            dontspam++;
            var profil = await _client.GetCachedProfile();
            var stats = await _inventory.GetPlayerStats();
            var c = stats.FirstOrDefault();

            int l = c.Level;

            var expneeded = ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
            var curexp = ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level));
            var curexppercent = (Convert.ToDouble(curexp) / Convert.ToDouble(expneeded)) * 100;
            var pokemonToEvolve = (await _inventory.GetPokemonToEvolve(null)).Count();

            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Level: " + c.Level);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP Needed: " + expneeded);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Current EXP: {curexp} ({Math.Round(curexppercent)}%)");
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP to Level up: " + ((c.NextLevelXp) - (c.Experience)));
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "KM Walked: " + c.KmWalked);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokeStops visited: " + c.PokeStopVisits);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Stardust: " + profil.Profile.Currency.ToArray()[1].Amount);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemon to evolve: " + pokemonToEvolve);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemons: " + await _inventory.getPokemonCount() + "/" + profil.Profile.PokeStorage);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Items: " + await _inventory.getInventoryCount() + "/" + profil.Profile.ItemStorage); 
            //if (dontspam >= 3)
            //{
            //    dontspam = 0;
            //    PokeService data = null;
            //    try
            //    {
            //        var clientx = new WebClient();
            //        clientx.Headers.Add("user-agent", "PokegoBot-Ar1i-Github");
            //        var jsonString = clientx.DownloadString("https://go.jooas.com/status");
            //        data = new JavaScriptSerializer().Deserialize<PokeService>(jsonString);
            //    }
            //    catch (Exception)
            //    {

            //    }

            //    if (data != null)
            //    {
            //        Logger.ColoredConsoleWrite(ConsoleColor.White, "");
            //        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokemonGO Server Status:");
            //        if (data.go_online)
            //        {
            //            if (data.go_idle > 60)
            //            {
            //                int gohour = Convert.ToInt32(data.go_idle / 60);

            //                if (gohour > 24)
            //                {
            //                    int goday = gohour / 24;

            //                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + goday + " day(s).");
            //                }
            //                else
            //                {
            //                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + gohour + "h.");
            //                }
            //            }
            //            else
            //            {
            //                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + data.go_idle + " min.");
            //            }

            //            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Server anwsers in ~" + data.go_response + " seconds.");
            //        }
            //        else
            //        {
            //            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Pokemon GO Servers: Offline.");
            //        }
            //        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemon Trainer Club Server Status:");
            //        if (data.ptc_online)
            //        {
            //            if (data.ptc_idle > 60)
            //            {
            //                int ptchour = Convert.ToInt32(data.ptc_idle / 60);

            //                if (ptchour > 24)
            //                {
            //                    int ptcday = ptchour / 24;
            //                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + ptcday + " day(s).");

            //                }
            //                else
            //                {
            //                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + ptchour + "h.");
            //                }
            //            }
            //            else
            //            {
            //                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Online since ~" + data.ptc_idle + " min.");
            //            }

            //            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Server anwsers in ~" + data.ptc_response + " seconds.");
            //        }
            //        else
            //        {
            //            Logger.ColoredConsoleWrite(ConsoleColor.Red, "Pokemon Trainer Club: Offline.");
            //        }
            //    } 
            //    else
            //    {
            //        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Cant get Server Status from https://go.jooas.com/status");
            //    }
            //}
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");
            

            System.Console.Title = profil.Profile.Username + " lvl" + c.Level + "-(" + ((c.Experience - c.PrevLevelXp) - 
                StringUtils.getExpDiff(c.Level)) + "/" + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level)) + "|" + Math.Round(curexppercent) + "%)| Stardust: " + profil.Profile.Currency.ToArray()[1].Amount + "| " + _botStats.ToString();

        }


        private int count = 0;

        private int failed_softban = 0;

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {

            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude, _client.CurrentLat, _client.CurrentLng);

            if (_clientSettings.MaxWalkingRadiusInMeters != 0 && distanceFromStart > _clientSettings.MaxWalkingRadiusInMeters)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Youre outside of the defined Max Walking Radius. Walking back!");
                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
                var start = await _navigation.HumanLikeWalking(new GeoCoordinate(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
            }

            Resources.OutPutWalking = true;
            var mapObjects = await _client.GetMapObjects();
            
            //var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            var pokeStops =
            _navigation.pathByNearestNeighbour(
            mapObjects.MapCells.SelectMany(i => i.Forts)
            .Where(
                i =>
                i.Type == FortType.Checkpoint &&
                i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                .OrderBy(
                i =>
                LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, i.Latitude, i.Longitude)).ToArray(), _clientSettings.WalkingSpeedInKilometerPerHour);


            if (_clientSettings.MaxWalkingRadiusInMeters != 0)
            {
                pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, i.Latitude, i.Longitude) <= _clientSettings.MaxWalkingRadiusInMeters).ToArray();
                if (pokeStops.Count() == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "We cant find any PokeStops in a range of " + _clientSettings.MaxWalkingRadiusInMeters + "m!");
                    await ExecuteCatchAllNearbyPokemons();
                }
            }

           
            if (pokeStops.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We cant find any PokeStops, which are unused! Probably Server unstable, or you visted them all. Retrying..");
                await ExecuteCatchAllNearbyPokemons();

            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " PokeStops near.");
            }

            foreach (var pokeStop in pokeStops)
            {
                // replace this true with settings variable!!
                await UseIncense();

                await ExecuteCatchAllNearbyPokemons();

                
                if (count >= 3)
                {
                    count = 0;
                    await StatsLog(client);
                    if (_clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await EvolveAllPokemonWithEnoughCandy();
                    }
                    await TransferDuplicatePokemon(_clientSettings.keepPokemonsThatCanEvolve);
                    await RecycleItems();
                }
                //if (_clientSettings.pokevision)
                //{
                //    foreach (spottedPoke p in await _pokevision.GetNearPokemons(_client.CurrentLat, _client.CurrentLng))
                //    {
                //        var dist = LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, p._lat, p._lng);
                //        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"PokeVision: A {StringUtils.getPokemonNameByLanguage(_clientSettings, p._pokeId)} in {dist:0.##}m distance. Trying to catch.");
                //        var upd = await _navigation.HumanLikeWalking(new GeoCoordinate(p._lat, p._lng), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
                //    }
                //}

                var distance = LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await _client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                if (fortInfo == null)
                {
                    continue;
                }
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");
                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);

                ////var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await _client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                count++;

                if (fortSearch.ExperienceAwarded > 0)
                {
                    failed_softban = 0;
                    _botStats.addExperience(fortSearch.ExperienceAwarded);
                    var egg = "";
                    if (fortSearch.PokemonDataEgg != null)
                    {
                        egg = "Egg " + fortSearch.PokemonDataEgg.EggKmWalkedTarget;
                    } else
                    {
                        egg = "/";
                    }

                    var i = "";
                    if (StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded) != "")
                    {
                        i = StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded);
                    } else
                    {
                        i = "/";
                    }

                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: { fortSearch.GemsAwarded}, Eggs: {egg} Items: {i}", LogLevel.Info);
                } else {
                    failed_softban++; 
                    if (failed_softban >= 6)
                    {
                        Logger.Error("Detected a Softban. Trying to use our Special 1337 Unban Methode.");
                        for (int i = 0; i < 60; i++)
                        {
                            var unban = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                            if (unban.ExperienceAwarded > 0)
                            {
                                break;
                            }
                        }
                        failed_softban = 0;
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Probably unbanned you.");
                    }
                }

                await RandomHelper.RandomDelay(50, 200);
            }
            if (_clientSettings.WalkBackToDefaultLocation)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Walking back to Default Location.");
                await _navigation.HumanLikeWalking(new GeoCoordinate(_clientSettings.DefaultLatitude, _clientSettings.DefaultLongitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);
            }
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var client = _client;
            var mapObjects = await client.GetMapObjects();

            //var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);
            var pokemons =
               mapObjects.MapCells.SelectMany(i => i.CatchablePokemons)
               .OrderBy(
                   i =>
                   LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, i.Latitude, i.Longitude));

            if (pokemons != null && pokemons.Any())
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s).");

            foreach (var pokemon in pokemons)
            {
                count++;
                if (count >= 3)
                {
                    count = 0;
                    await StatsLog(client);
                    if (_clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await EvolveAllPokemonWithEnoughCandy();
                    }
                    await TransferDuplicatePokemon(_clientSettings.keepPokemonsThatCanEvolve);
                    await RecycleItems();
                }

                if (_clientSettings.catchPokemonSkipList.Contains(pokemon.PokemonId))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Pokemon: " + pokemon.PokemonId);
                    continue;
                }

                var distance = LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, pokemon.Latitude, pokemon.Longitude);
                await Task.Delay(distance > 100 ? 1000 : 100);
                var encounterPokemonResponse = await _client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);
                
                if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
                {
                    var bestPokeball = await GetBestBall(encounterPokemonResponse?.WildPokemon);
                    if (bestPokeball == MiscEnums.Item.ITEM_UNKNOWN)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"We dont own Pokeballs! - We missed a {pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp}");
                        return;
                    }
                    CatchPokemonResponse caughtPokemonResponse;
                    do
                    {
                        
                        var inventoryBerries = await _inventory.GetItems();
                        var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();
                        var bestBerry = await GetBestBerry(encounterPokemonResponse?.WildPokemon);
                        var berries = inventoryBerries.Where(p => (ItemId)p.Item_ == bestBerry).FirstOrDefault();
                        if (bestBerry != ItemId.ItemUnknown && probability.HasValue && probability.Value < 0.35)
                        {
                            //Throw berry is we can
                            var useRaspberry = await _client.UseCaptureItem(pokemon.EncounterId, bestBerry, pokemon.SpawnpointId);
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Used {bestBerry}. Remaining: {berries.Count}.", LogLevel.Info);
                            await RandomHelper.RandomDelay(50, 200);
                        }

                        caughtPokemonResponse = await _client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, bestPokeball);
                    }
                    while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        foreach (int xp in caughtPokemonResponse.Scores.Xp)
                            _botStats.addExperience(xp);

                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} ({PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon.PokemonData)}% perfect) using a {bestPokeball} and we got {caughtPokemonResponse.Scores.Xp.Sum()} XP.");

                        //try
                        //{
                        //    var r = (HttpWebRequest)WebRequest.Create("http://pokemon.becher.xyz/index.php?pokeName=" + pokemon.PokemonId);
                        //    var rp = (HttpWebResponse)r.GetResponse();
                        //    var rps = new StreamReader(rp.GetResponseStream()).ReadToEnd();
                        //    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {pokemon.PokemonId} ({rps}) with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {bestPokeball}");
                        //} catch (Exception)
                        //{
                        //    Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {pokemon.PokemonId} (Language Server Offline) with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {bestPokeball}");
                        //}
                       
                        _botStats.addPokemon(1);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} ({PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon.PokemonData)} % perfect) got away while using a {bestPokeball}..");
                        failed_softban++; 
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Error Catching Pokemon: {encounterPokemonResponse?.Status}");
                }
                await RandomHelper.RandomDelay(50, 200);
            }
        }

        private async Task EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter = null)
        {
            var pokemonToEvolve = await _inventory.GetPokemonToEvolve(filter);
            if (pokemonToEvolve.Count() != 0)
            {
                if(_clientSettings.UseLuckyEgg)
                {
                    await _inventory.UseLuckyEgg(_client);
                }
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
                

                var evolvePokemonOutProto = await _client.EvolvePokemon((ulong)pokemon.Id);

                if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved {StringUtils.getPokemonNameByLanguage(_clientSettings, pokemon.PokemonId)} with {pokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(pokemon)} % perfect) successfully to {StringUtils.getPokemonNameByLanguage(_clientSettings, evolvePokemonOutProto.EvolvedPokemon.PokemonType)} with {evolvePokemonOutProto.EvolvedPokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(evolvePokemonOutProto.EvolvedPokemon)} % perfect) for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info);
                    _botStats.addExperience(evolvePokemonOutProto.ExpAwarded);
                }
                else
                {
                    if (evolvePokemonOutProto.Result != EvolvePokemonOut.Types.EvolvePokemonStatus.FailedPokemonMissing)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", LogLevel.Info);
                    }
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        {
            if (_clientSettings.TransferDoublePokemons)
            {
                var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer(keepPokemonsThatCanEvolve);

                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!_clientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {

                        if (duplicatePokemon.Cp > _clientSettings.DontTransferWithCPOver)
                        {
                            continue;
                        }

                        var bestPokemonOfType = await _inventory.GetHighestCPofType(duplicatePokemon);

                        var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(_clientSettings, duplicatePokemon.PokemonId)} with {duplicatePokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(duplicatePokemon)} % perfect) (Best: {bestPokemonOfType} CP)", LogLevel.Info);
                        await RandomHelper.RandomDelay(500, 700);
                    }
                }
            }
        }

        private async Task RecycleItems()
        {
            var items = await _inventory.GetItemsToRecycle(_clientSettings);

            foreach (var item in items)
            {
                var transfer = await _client.RecycleItem((ItemId)item.Item_, item.Count);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(ItemId)item.Item_}", LogLevel.Info);
                await RandomHelper.RandomDelay(500, 700);
            }
        }

        private async Task<MiscEnums.Item> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await _inventory.GetItemsNonCache();
            var balls = items.Where(i => ((MiscEnums.Item)i.Item_ == MiscEnums.Item.ITEM_POKE_BALL
                                      || (MiscEnums.Item)i.Item_ == MiscEnums.Item.ITEM_GREAT_BALL
                                      || (MiscEnums.Item)i.Item_ == MiscEnums.Item.ITEM_ULTRA_BALL
                                      || (MiscEnums.Item)i.Item_ == MiscEnums.Item.ITEM_MASTER_BALL) && i.Count > 0).GroupBy(i => ((MiscEnums.Item)i.Item_)).ToList();
            if (balls.Count == 0) return MiscEnums.Item.ITEM_UNKNOWN;

            var pokeBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_POKE_BALL);
            var greatBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_GREAT_BALL);
            var ultraBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_ULTRA_BALL);
            var masterBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_MASTER_BALL);

            if (masterBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_MASTER_BALL;
            else if (ultraBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (ultraBalls && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            else if (greatBalls && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (greatBalls && pokemonCp >= 500)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            return balls.OrderBy(g => g.Key).First().Key;
        }

        private async Task<ItemId> GetBestBerry(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await _inventory.GetItems();
            var berries = items.Where(i => (ItemId)i.Item_ == ItemId.ItemRazzBerry
                                        || (ItemId)i.Item_ == ItemId.ItemBlukBerry
                                        || (ItemId)i.Item_ == ItemId.ItemNanabBerry
                                        || (ItemId)i.Item_ == ItemId.ItemWeparBerry
                                        || (ItemId)i.Item_ == ItemId.ItemPinapBerry).GroupBy(i => ((ItemId)i.Item_)).ToList();
            if (berries.Count == 0 || pokemonCp <= 350) return ItemId.ItemUnknown;

            var razzBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_RAZZ_BERRY);
            var blukBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_BLUK_BERRY);
            var nanabBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_NANAB_BERRY);
            var weparBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_WEPAR_BERRY);
            var pinapBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_PINAP_BERRY);

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
        
        DateTime lastincenseuse;
        public async Task UseIncense()
        {
            if (_clientSettings.UserIncense)
            {
                var inventory = await _inventory.GetItems();
                var incsense = inventory.Where(p => (ItemId)p.Item_ == ItemId.ItemIncenseOrdinary).FirstOrDefault();

                if (lastincenseuse > DateTime.Now.AddSeconds(5))
                {
                    TimeSpan duration = lastincenseuse - DateTime.Now;
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Incense still running: {duration.Minutes}m{duration.Seconds}s");
                    return;
                }
                if (incsense == null || incsense.Count <= 0) { return; }

                await _client.UseItemIncense(ItemId.ItemIncenseOrdinary);
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                lastincenseuse = DateTime.Now.AddMinutes(30);
                await Task.Delay(3000);
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
    }
}
