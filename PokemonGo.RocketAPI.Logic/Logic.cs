using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllEnum;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Extensions;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketAPI.Exceptions;
using System.Net;
using System.IO;
using System.Device.Location;
using PokemonGo.RocketAPI.Helpers;

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


        public Logic(ISettings clientSettings)
        {
            _clientSettings = clientSettings;
            _client = new Client(_clientSettings);
            _inventory = new Inventory(_client);
            _botStats = new BotStats();
            _navigation = new Navigation(_client);
        }

        public async Task Execute()
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Starting Execute on login server: {_clientSettings.AuthType}", LogLevel.Info);

            while (true)
            {
                try
                {
                    if (_clientSettings.AuthType == AuthType.Ptc)
                        await _client.DoPtcLogin(_clientSettings.PtcUsername, _clientSettings.PtcPassword);
                    else if (_clientSettings.AuthType == AuthType.Google)
                        await _client.DoGoogleLogin();

                    if (_clientSettings.TelegramAPIToken != "YourAccessToken" || _clientSettings.TelegramName != "YourTelegramNickname")
                    {
                        _telegram = new TelegramUtil(_client, new Telegram.Bot.TelegramBotClient(_clientSettings.TelegramAPIToken), _clientSettings);

                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "To Activate Informations with Telegram, write the Bot a message for more Informations");
                        var me = await _telegram.getClient().GetMeAsync();
                        _telegram.getClient().OnCallbackQuery += _telegram.BotOnCallbackQueryReceived;
                        _telegram.getClient().OnMessage += _telegram.BotOnMessageReceived;
                        _telegram.getClient().OnMessageEdited += _telegram.BotOnMessageReceived;
                        Logger.ColoredConsoleWrite(ConsoleColor.Green, "Telegram Name: " + me.Username);
                        _telegram.getClient().StartReceiving();
                    }

                    await PostLoginExecute();
                }
                catch (AccessTokenExpiredException ex)
                {
                    Logger.Error($"Access token expired");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving();
                }
                catch (TaskCanceledException ex)
                {
                    Logger.Error("Task Canceled Exception - Restarting");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving();
                    await Execute();
                }
                catch (UriFormatException ex)
                {
                    Logger.Error("UriFormatException - Restarting");
                    Logger.Error($"{ex}");
                    await Execute();
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Logger.Error("ArgumentOutOfRangeException - Restarting");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving();

                    await Execute();
                }
                catch (ArgumentNullException ex)
                {
                    Logger.Error("ArgumentNullException - Restarting");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving(); 
                    await Execute();
                }
                catch (NullReferenceException ex)
                {
                    Logger.Error("NullReferenceException - Restarting");
                    Logger.Error($"{ex}"); 
                    _telegram.getClient().StopReceiving();
                    await Execute();
                }
                catch (InvalidResponseException ex)
                {
                    Logger.Error("InvalidResponseException - Restarting");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving();
                    await Execute();
                }
                catch (AggregateException ex)
                {
                    Logger.Error("AggregateException - Restarting");
                    Logger.Error($"{ex}");
                    _telegram.getClient().StopReceiving();
                    await Execute();
                }
            }
        }

        public async Task PostLoginExecute()
        {

            while (true)
            {
                try
                {

                    await _client.SetServer();
                    await StatsLog(_client);
                    if (_clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await EvolveAllPokemonWithEnoughCandy();
                    }
                    await TransferDuplicatePokemon(true);
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

        private async Task StatsLog(Client client)
        {
            var inventory = await client.GetInventory();
            var profil = await client.GetProfile();
            var stats = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData.PlayerStats).ToArray();
            foreach (var c in stats)
            {
                if (c != null)
                {
                    int l = c.Level;
                  
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Level: " + c.Level);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP Needed: " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level)));
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Current EXP: " + ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level)));
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP to Level up: " + ((c.NextLevelXp) - (c.Experience)));
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "KM Walked: " + c.KmWalked);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokeStops visited: " + c.PokeStopVisits);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Stardust: " + profil.Profile.Currency.ToArray()[1].Amount);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");


                    System.Console.Title = profil.Profile.Username + " Level " + c.Level + " - (" + ((c.Experience - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level)) + " / " + ((c.NextLevelXp - c.PrevLevelXp) - StringUtils.getExpDiff(c.Level)) + ") | Stardust: " + profil.Profile.Currency.ToArray()[1].Amount + " | " + _botStats.ToString();
                     
                }
            }

           


        }


        private int count = 0;

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {

            Resources.OutPutWalking = true;
            var mapObjects = await client.GetMapObjects();

            //var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());

            var pokeStops =
              Navigation.pathByNearestNeighbour(
              mapObjects.MapCells.SelectMany(i => i.Forts)
              .Where(
                  i =>
                  i.Type == FortType.Checkpoint &&
                  i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime())
                  .OrderBy(
                  i =>
                  LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, i.Latitude, i.Longitude)).ToArray());

            if (pokeStops.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We cant find any PokeStops, which are unused! Probably Server unstable, or you visted them all. Retrying..");
                await ExecuteCatchAllNearbyPokemons();

            } else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " PokeStops near.");
            }

            foreach (var pokeStop in pokeStops)
            {

                await ExecuteCatchAllNearbyPokemons();
                count++;
                if (count >= 3)
                {
                    count = 0;
                    await StatsLog(client);
                    await TransferDuplicatePokemon(true);
                    await RecycleItems();
                }

                var distance = LocationUtils.CalculateDistanceInMeters(_client.CurrentLat, _client.CurrentLng, pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");
                var update = await _navigation.HumanLikeWalking(new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude), _clientSettings.WalkingSpeedInKilometerPerHour, ExecuteCatchAllNearbyPokemons);

                ////var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                if (fortSearch.ExperienceAwarded > 0)
                {
                    _botStats.addExperience(fortSearch.ExperienceAwarded);
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: { fortSearch.GemsAwarded}, Eggs: {fortSearch.PokemonDataEgg} Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}", LogLevel.Info);
                }

                await RandomHelper.RandomDelay(50, 200);
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
                    await TransferDuplicatePokemon(true);
                    await RecycleItems();
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
                        if (bestBerry != AllEnum.ItemId.ItemUnknown && probability.HasValue && probability.Value < 0.35)
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

                        var r = (HttpWebRequest)WebRequest.Create("http://boosting-service.de/pokemon/index.php?pokeName=" + pokemon.PokemonId);
                        var rp = (HttpWebResponse)r.GetResponse();
                        var rps = new StreamReader(rp.GetResponseStream()).ReadToEnd();

                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {pokemon.PokemonId} ({rps}) with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {bestPokeball}");
                        _botStats.addPokemon(1);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} got away while using a {bestPokeball}..");
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
            if (pokemonToEvolve.Count() > 30)
            {
                // Use EGG - need to add this shit
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
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved {pokemon.PokemonId} successfully for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info);
                else
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", LogLevel.Info);

                await Task.Delay(1000);
            }
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        {
            if (_clientSettings.TransferDoublePokemons)
            {
                var duplicatePokemons = await _inventory.GetDuplicatePokemonToTransfer();

                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!_clientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {

                        if (duplicatePokemon.Cp > 999)
                        {
                            continue;
                        }

                        var bestPokemonOfType = await _inventory.GetHighestCPofType(duplicatePokemon);

                        var transfer = await _client.TransferPokemon(duplicatePokemon.Id);
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {duplicatePokemon.PokemonId} with {duplicatePokemon.Cp} CP (Best: {bestPokemonOfType} CP)", LogLevel.Info);
                        await Task.Delay(500);
                    }
                }
            }
        }

        private async Task RecycleItems()
        {
            var items = await _inventory.GetItemsToRecycle(_clientSettings);

            foreach (var item in items)
            {
                var transfer = await _client.RecycleItem((AllEnum.ItemId)item.Item_, item.Count);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(AllEnum.ItemId)item.Item_}", LogLevel.Info);
                await Task.Delay(500);
            }
        }

        private async Task<MiscEnums.Item> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await _inventory.GetItems();
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

        private async Task<AllEnum.ItemId> GetBestBerry(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await _inventory.GetItems();
            var berries = items.Where(i => (AllEnum.ItemId)i.Item_ == AllEnum.ItemId.ItemRazzBerry
                                        || (AllEnum.ItemId)i.Item_ == AllEnum.ItemId.ItemBlukBerry
                                        || (AllEnum.ItemId)i.Item_ == AllEnum.ItemId.ItemNanabBerry
                                        || (AllEnum.ItemId)i.Item_ == AllEnum.ItemId.ItemWeparBerry
                                        || (AllEnum.ItemId)i.Item_ == AllEnum.ItemId.ItemPinapBerry).GroupBy(i => ((AllEnum.ItemId)i.Item_)).ToList();
            if (berries.Count == 0 || pokemonCp <= 350) return AllEnum.ItemId.ItemUnknown;

            var razzBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_RAZZ_BERRY);
            var blukBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_BLUK_BERRY);
            var nanabBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_NANAB_BERRY);
            var weparBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_WEPAR_BERRY);
            var pinapBerryCount = await _inventory.GetItemAmountByType(MiscEnums.Item.ITEM_PINAP_BERRY);

            if (pinapBerryCount > 0 && pokemonCp >= 2000)
                return AllEnum.ItemId.ItemPinapBerry;
            else if (weparBerryCount > 0 && pokemonCp >= 2000)
                return AllEnum.ItemId.ItemWeparBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return AllEnum.ItemId.ItemNanabBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 2000)
                return AllEnum.ItemId.ItemBlukBerry;

            if (weparBerryCount > 0 && pokemonCp >= 1500)
                return AllEnum.ItemId.ItemWeparBerry;
            else if (nanabBerryCount > 0 && pokemonCp >= 1500)
                return AllEnum.ItemId.ItemNanabBerry;
            else if (blukBerryCount > 0 && pokemonCp >= 1500)
                return AllEnum.ItemId.ItemBlukBerry;

            if (nanabBerryCount > 0 && pokemonCp >= 1000)
                return AllEnum.ItemId.ItemNanabBerry;
            else if (blukBerryCount > 0 && pokemonCp >= 1000)
                return AllEnum.ItemId.ItemBlukBerry;

            if (blukBerryCount > 0 && pokemonCp >= 500)
                return AllEnum.ItemId.ItemBlukBerry;

            return berries.OrderBy(g => g.Key).First().Key;
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