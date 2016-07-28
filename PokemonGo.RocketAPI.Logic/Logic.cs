﻿namespace PokemonGo.RocketAPI.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Threading.Tasks;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.Exceptions;
    using PokemonGo.RocketAPI.Extensions;
    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Helpers;
    using PokemonGo.RocketAPI.Logic.Utils;

    public class Logic
    {
        public const double SpeedDownTo = 10 / 3.6;
        public readonly Client _client;
        public readonly ISettings _clientSettings;
        public readonly Inventory _inventory;
        public BotStats _botStats;
        public TelegramUtil _telegram;
        private readonly Navigation _navigation;
        private int count;
        private DateTime lastegguse;
        private DateTime lastincenseuse;

        public Logic(ISettings clientSettings)
        {
            this._clientSettings = clientSettings;
            this._client = new Client(this._clientSettings);
            this._inventory = new Inventory(this._client);
            this._botStats = new BotStats();
            this._navigation = new Navigation(this._client);
        }

        public async Task Execute()
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Starting Execute on login server: {this._clientSettings.AuthType}", LogLevel.Info);

            while (true)
            {
                try
                {
                    if (this._clientSettings.AuthType == AuthType.Ptc)
                        await this._client.DoPtcLogin(this._clientSettings.PtcUsername, this._clientSettings.PtcPassword);
                    else if (this._clientSettings.AuthType == AuthType.Google)
                        await this._client.DoGoogleLogin();

                    if (this._clientSettings.TelegramAPIToken != "YourAccessToken" && this._clientSettings.TelegramName != "YourTelegramNickname")
                    {
                        try
                        {
                            this._telegram = new TelegramUtil(this._client, new Telegram.Bot.TelegramBotClient(this._clientSettings.TelegramAPIToken), this._clientSettings, this._inventory);

                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "To Activate Informations with Telegram, write the Bot a message for more Informations");
                            var me = await this._telegram.getClient().GetMeAsync();
                            this._telegram.getClient().OnCallbackQuery += this._telegram.BotOnCallbackQueryReceived;
                            this._telegram.getClient().OnMessage += this._telegram.BotOnMessageReceived;
                            this._telegram.getClient().OnMessageEdited += this._telegram.BotOnMessageReceived;
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, "Telegram Name: " + me.Username);
                            this._telegram.getClient().StartReceiving();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    await this.PostLoginExecute();
                }
                catch (PtcOfflineException)
                {
                    Logger.Error("PTC Server Offline. Trying to Restart in 20 Seconds...");
                    try
                    {
                        this._telegram.getClient().StopReceiving();
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
                        this._telegram.getClient().StopReceiving();
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
                        this._telegram.getClient().StopReceiving();
                    }
                    catch (Exception)
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
                    await this._client.SetServer();
                    await this.StatsLog(this._client);
                    if (this._clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await this.EvolveAllPokemonWithEnoughCandy();
                    }

                    await this.TransferDuplicatePokemon(true);
                    await this.RecycleItems();
                    await this.ExecuteFarmingPokestopsAndPokemons(this._client);
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
            for (var i = 0; i < repeat; i++)
                await action();
        }

        public async Task UseIncense()
        {
            if (this._clientSettings.UserIncense)
            {
                var inventory = await this._inventory.GetItems();
                var incsense = inventory.Where(p => (ItemId) p.Item_ == ItemId.ItemIncenseOrdinary).FirstOrDefault();

                if (this.lastincenseuse > DateTime.Now.AddSeconds(5))
                {
                    var duration = this.lastincenseuse - DateTime.Now;
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Incense still running: {duration.Minutes}m{duration.Seconds}s");
                    return;
                }

                if (incsense == null || incsense.Count <= 0)
                {
                    return;
                }

                await this._client.UseItemIncense(ItemId.ItemIncenseOrdinary);
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Incsense, remaining: {incsense.Count - 1}");
                this.lastincenseuse = DateTime.Now.AddMinutes(30);
                await Task.Delay(3000);
            }
        }

        public async Task UseLuckyEgg(Client client)
        {
            var inventory = await this._inventory.GetItems();
            var luckyEgg = inventory.Where(p => (ItemId) p.Item_ == ItemId.ItemLuckyEgg).FirstOrDefault();

            if (this.lastegguse > DateTime.Now.AddSeconds(5))
            {
                var duration = this.lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0)
            {
                return;
            }

            await this._client.UseItemXpBoost(ItemId.ItemLuckyEgg);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            this.lastegguse = DateTime.Now.AddMinutes(30);
            await Task.Delay(3000);
        }

        private double _distance(double Lat1, double Lng1, double Lat2, double Lng2)
        {
            double r_earth = 6378137;
            var d_lat = (Lat2 - Lat1) * Math.PI / 180;
            var d_lon = (Lng2 - Lng1) * Math.PI / 180;
            var alpha = Math.Sin(d_lat / 2) * Math.Sin(d_lat / 2) + Math.Cos(Lat1 * Math.PI / 180) * Math.Cos(Lat2 * Math.PI / 180) * Math.Sin(d_lon / 2) * Math.Sin(d_lon / 2);
            var d = 2 * r_earth * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            return d;
        }

        private async Task EvolveAllPokemonWithEnoughCandy(IEnumerable<PokemonId> filter = null)
        {
            var pokemonToEvolve = await this._inventory.GetPokemonToEvolve(filter);
            if (pokemonToEvolve.Count() != 0)
            {
                if (this._clientSettings.UseLuckyEgg)
                {
                    await this.UseLuckyEgg(this._client);
                }
            }

            foreach (var pokemon in pokemonToEvolve)
            {
                if (!this._clientSettings.pokemonsToEvolve.Contains(pokemon.PokemonId))
                {
                    continue;
                }

                this.count++;
                if (this.count == 6)
                {
                    this.count = 0;
                    await this.StatsLog(this._client);
                }

                var evolvePokemonOutProto = await this._client.EvolvePokemon(pokemon.Id);

                if (evolvePokemonOutProto.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved {StringUtils.getPokemonNameByLanguage(this._clientSettings, pokemon.PokemonId)} with {pokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(pokemon)} % perfect) successfully to {StringUtils.getPokemonNameByLanguage(this._clientSettings, evolvePokemonOutProto.EvolvedPokemon.PokemonType)} with {evolvePokemonOutProto.EvolvedPokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(evolvePokemonOutProto.EvolvedPokemon)} % perfect) for {evolvePokemonOutProto.ExpAwarded}xp", LogLevel.Info);
                    this._botStats.addExperience(evolvePokemonOutProto.ExpAwarded);
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemon.PokemonId}. EvolvePokemonOutProto.Result was {evolvePokemonOutProto.Result}, stopping evolving {pokemon.PokemonId}", LogLevel.Info);
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
        }

        private async Task ExecuteCatchAllNearbyPokemons()
        {
            var client = this._client;
            var mapObjects = await client.GetMapObjects();

            // var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons);
            var pokemons = mapObjects.MapCells.SelectMany(i => i.CatchablePokemons).OrderBy(i => LocationUtils.CalculateDistanceInMeters(this._client.CurrentLat, this._client.CurrentLng, i.Latitude, i.Longitude));

            if (pokemons != null && pokemons.Any())
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"Found {pokemons.Count()} catchable Pokemon(s).");

            foreach (var pokemon in pokemons)
            {
                this.count++;
                if (this.count >= 3)
                {
                    this.count = 0;
                    await this.StatsLog(client);
                    if (this._clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await this.EvolveAllPokemonWithEnoughCandy();
                    }

                    await this.TransferDuplicatePokemon(true);
                    await this.RecycleItems();
                }

                if (this._clientSettings.catchPokemonSkipList.Contains(pokemon.PokemonId))
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Skipped Pokemon: " + pokemon.PokemonId);
                    continue;
                }

                var distance = LocationUtils.CalculateDistanceInMeters(this._client.CurrentLat, this._client.CurrentLng, pokemon.Latitude, pokemon.Longitude);
                await Task.Delay(distance > 100 ? 1000 : 100);
                var encounterPokemonResponse = await this._client.EncounterPokemon(pokemon.EncounterId, pokemon.SpawnpointId);

                if (encounterPokemonResponse.Status == EncounterResponse.Types.Status.EncounterSuccess)
                {
                    var bestPokeball = await this.GetBestBall(encounterPokemonResponse?.WildPokemon);
                    if (bestPokeball == MiscEnums.Item.ITEM_UNKNOWN)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, $"We dont own Pokeballs! - We missed a {pokemon.PokemonId} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp}");
                        return;
                    }

                    CatchPokemonResponse caughtPokemonResponse;
                    do
                    {
                        var inventoryBerries = await this._inventory.GetItems();
                        var probability = encounterPokemonResponse?.CaptureProbability?.CaptureProbability_?.FirstOrDefault();
                        var bestBerry = await this.GetBestBerry(encounterPokemonResponse?.WildPokemon);
                        var berries = inventoryBerries.Where(p => (ItemId) p.Item_ == bestBerry).FirstOrDefault();
                        if (bestBerry != ItemId.ItemUnknown && probability.HasValue && probability.Value < 0.35)
                        {
                            // Throw berry is we can
                            var useRaspberry = await this._client.UseCaptureItem(pokemon.EncounterId, bestBerry, pokemon.SpawnpointId);
                            Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Used {bestBerry}. Remaining: {berries.Count}.", LogLevel.Info);
                            await RandomHelper.RandomDelay(50, 200);
                        }

                        caughtPokemonResponse = await this._client.CatchPokemon(pokemon.EncounterId, pokemon.SpawnpointId, pokemon.Latitude, pokemon.Longitude, bestPokeball);
                    }
                    while (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchMissed || caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchEscape);

                    if (caughtPokemonResponse.Status == CatchPokemonResponse.Types.CatchStatus.CatchSuccess)
                    {
                        foreach (var xp in caughtPokemonResponse.Scores.Xp)
                            this._botStats.addExperience(xp);

                        Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {StringUtils.getPokemonNameByLanguage(this._clientSettings, pokemon.PokemonId)} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} ({PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon.PokemonData)}% perfect) using a {bestPokeball}");

                        // try
                        // {
                        // var r = (HttpWebRequest)WebRequest.Create("http://pokemon.becher.xyz/index.php?pokeName=" + pokemon.PokemonId);
                        // var rp = (HttpWebResponse)r.GetResponse();
                        // var rps = new StreamReader(rp.GetResponseStream()).ReadToEnd();
                        // Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {pokemon.PokemonId} ({rps}) with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {bestPokeball}");
                        // } catch (Exception)
                        // {
                        // Logger.ColoredConsoleWrite(ConsoleColor.Magenta, $"We caught a {pokemon.PokemonId} (Language Server Offline) with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} using a {bestPokeball}");
                        // }
                        this._botStats.addPokemon(1);
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, $"{StringUtils.getPokemonNameByLanguage(this._clientSettings, pokemon.PokemonId)} with CP {encounterPokemonResponse?.WildPokemon?.PokemonData?.Cp} ({PokemonInfo.CalculatePokemonPerfection(encounterPokemonResponse?.WildPokemon.PokemonData)} % perfect) got away while using a {bestPokeball}..");
                    }
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Error Catching Pokemon: {encounterPokemonResponse?.Status}");
                }

                await RandomHelper.RandomDelay(50, 200);
            }
        }

        private async Task ExecuteFarmingPokestopsAndPokemons(Client client)
        {
            var distanceFromStart = LocationUtils.CalculateDistanceInMeters(this._clientSettings.DefaultLatitude, this._clientSettings.DefaultLongitude, this._client.CurrentLat, this._client.CurrentLng);

            if (this._clientSettings.MaxWalkingRadiusInMeters != 0 && distanceFromStart > this._clientSettings.MaxWalkingRadiusInMeters)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Youre outside of the defined Max Walking Radius. Walking back!");
                var update = await this._navigation.HumanLikeWalking(new GeoCoordinate(this._clientSettings.DefaultLatitude, this._clientSettings.DefaultLongitude), this._clientSettings.WalkingSpeedInKilometerPerHour, this.ExecuteCatchAllNearbyPokemons);
                var start = await this._navigation.HumanLikeWalking(new GeoCoordinate(this._clientSettings.DefaultLatitude, this._clientSettings.DefaultLongitude), this._clientSettings.WalkingSpeedInKilometerPerHour, this.ExecuteCatchAllNearbyPokemons);
            }

            Resources.OutPutWalking = true;
            var mapObjects = await client.GetMapObjects();

            // var pokeStops = mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime());
            var pokeStops = this._navigation.pathByNearestNeighbour(mapObjects.MapCells.SelectMany(i => i.Forts).Where(i => i.Type == FortType.Checkpoint && i.CooldownCompleteTimestampMs < DateTime.UtcNow.ToUnixTime()).OrderBy(i => LocationUtils.CalculateDistanceInMeters(this._client.CurrentLat, this._client.CurrentLng, i.Latitude, i.Longitude)).ToArray(), this._clientSettings.WalkingSpeedInKilometerPerHour);

            if (this._clientSettings.MaxWalkingRadiusInMeters != 0)
            {
                pokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(this._client.CurrentLat, this._client.CurrentLng, i.Latitude, i.Longitude) <= this._clientSettings.MaxWalkingRadiusInMeters).ToArray();
                if (pokeStops.Count() == 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "We cant find any PokeStops in a range of " + this._clientSettings.MaxWalkingRadiusInMeters + "m!");
                }
            }

            if (pokeStops.Count() == 0)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We cant find any PokeStops, which are unused! Probably Server unstable, or you visted them all. Retrying..");
                await this.ExecuteCatchAllNearbyPokemons();
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "We found " + pokeStops.Count() + " PokeStops near.");
            }

            foreach (var pokeStop in pokeStops)
            {
                // replace this true with settings variable!!
                if (true)
                {
                    await this.UseIncense();
                }

                await this.ExecuteCatchAllNearbyPokemons();
                this.count++;
                if (this.count >= 3)
                {
                    this.count = 0;
                    await this.StatsLog(client);
                    if (this._clientSettings.EvolvePokemonsIfEnoughCandy)
                    {
                        await this.EvolveAllPokemonWithEnoughCandy();
                    }

                    await this.TransferDuplicatePokemon(true);
                    await this.RecycleItems();
                }

                var distance = LocationUtils.CalculateDistanceInMeters(this._client.CurrentLat, this._client.CurrentLng, pokeStop.Latitude, pokeStop.Longitude);
                var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Next Pokestop: {fortInfo.Name} in {distance:0.##}m distance.");
                var update = await this._navigation.HumanLikeWalking(new GeoCoordinate(pokeStop.Latitude, pokeStop.Longitude), this._clientSettings.WalkingSpeedInKilometerPerHour, this.ExecuteCatchAllNearbyPokemons);

                ////var fortInfo = await client.GetFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);
                var fortSearch = await client.SearchFort(pokeStop.Id, pokeStop.Latitude, pokeStop.Longitude);

                if (fortSearch.ExperienceAwarded > 0)
                {
                    this._botStats.addExperience(fortSearch.ExperienceAwarded);
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Farmed XP: {fortSearch.ExperienceAwarded}, Gems: {fortSearch.GemsAwarded}, Eggs: {fortSearch.PokemonDataEgg} Items: {StringUtils.GetSummedFriendlyNameOfItemAwardList(fortSearch.ItemsAwarded)}", LogLevel.Info);
                }

                await RandomHelper.RandomDelay(50, 200);
            }

            if (this._clientSettings.WalkBackToDefaultLocation)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, "Walking back to Default Location.");
                await this._navigation.HumanLikeWalking(new GeoCoordinate(this._clientSettings.DefaultLatitude, this._clientSettings.DefaultLongitude), this._clientSettings.WalkingSpeedInKilometerPerHour, this.ExecuteCatchAllNearbyPokemons);
            }
        }

        private async Task<MiscEnums.Item> GetBestBall(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await this._inventory.GetItems();
            var balls = items.Where(i => ((MiscEnums.Item) i.Item_ == MiscEnums.Item.ITEM_POKE_BALL || (MiscEnums.Item) i.Item_ == MiscEnums.Item.ITEM_GREAT_BALL || (MiscEnums.Item) i.Item_ == MiscEnums.Item.ITEM_ULTRA_BALL || (MiscEnums.Item) i.Item_ == MiscEnums.Item.ITEM_MASTER_BALL) && i.Count > 0).GroupBy(i => (MiscEnums.Item) i.Item_).ToList();
            if (balls.Count == 0)
                return MiscEnums.Item.ITEM_UNKNOWN;

            var pokeBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_POKE_BALL);
            var greatBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_GREAT_BALL);
            var ultraBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_ULTRA_BALL);
            var masterBalls = balls.Any(g => g.Key == MiscEnums.Item.ITEM_MASTER_BALL);

            if (masterBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_MASTER_BALL;
            if (ultraBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            if (greatBalls && pokemonCp >= 2000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (ultraBalls && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_ULTRA_BALL;
            if (greatBalls && pokemonCp >= 1000)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            if (greatBalls && pokemonCp >= 500)
                return MiscEnums.Item.ITEM_GREAT_BALL;

            return balls.OrderBy(g => g.Key).First().Key;
        }

        private async Task<ItemId> GetBestBerry(WildPokemon pokemon)
        {
            var pokemonCp = pokemon?.PokemonData?.Cp;

            var items = await this._inventory.GetItems();
            var berries = items.Where(i => (ItemId) i.Item_ == ItemId.ItemRazzBerry || (ItemId) i.Item_ == ItemId.ItemBlukBerry || (ItemId) i.Item_ == ItemId.ItemNanabBerry || (ItemId) i.Item_ == ItemId.ItemWeparBerry || (ItemId) i.Item_ == ItemId.ItemPinapBerry).GroupBy(i => (ItemId) i.Item_).ToList();
            if (berries.Count == 0 || pokemonCp <= 350)
                return ItemId.ItemUnknown;

            var razzBerryCount = await this._inventory.GetItemAmountByType(MiscEnums.Item.ITEM_RAZZ_BERRY);
            var blukBerryCount = await this._inventory.GetItemAmountByType(MiscEnums.Item.ITEM_BLUK_BERRY);
            var nanabBerryCount = await this._inventory.GetItemAmountByType(MiscEnums.Item.ITEM_NANAB_BERRY);
            var weparBerryCount = await this._inventory.GetItemAmountByType(MiscEnums.Item.ITEM_WEPAR_BERRY);
            var pinapBerryCount = await this._inventory.GetItemAmountByType(MiscEnums.Item.ITEM_PINAP_BERRY);

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

        private async Task RecycleItems()
        {
            var items = await this._inventory.GetItemsToRecycle(this._clientSettings);

            foreach (var item in items)
            {
                var transfer = await this._client.RecycleItem((ItemId) item.Item_, item.Count);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(ItemId) item.Item_}", LogLevel.Info);
                await RandomHelper.RandomDelay(500, 700);
            }
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
                    var l = c.Level;

                    var expneeded = c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                    var curexp = c.Experience - c.PrevLevelXp - StringUtils.getExpDiff(c.Level);
                    var curexppercent = Convert.ToDouble(curexp) / Convert.ToDouble(expneeded) * 100;
                    var pokemonToEvolve = (await this._inventory.GetPokemonToEvolve(null)).Count();

                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Level: " + c.Level);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP Needed: " + expneeded);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Current EXP: {curexp} ({Math.Round(curexppercent)}%)");
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "EXP to Level up: " + (c.NextLevelXp - c.Experience));
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "KM Walked: " + c.KmWalked);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "PokeStops visited: " + c.PokeStopVisits);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Stardust: " + profil.Profile.Currency.ToArray()[1].Amount);
                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Pokemon to evolve: " + pokemonToEvolve);

                    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "_____________________________");

                    Console.Title = profil.Profile.Username + " Level " + c.Level + " - (" + (c.Experience - c.PrevLevelXp - StringUtils.getExpDiff(c.Level)) + " / " + (c.NextLevelXp - c.PrevLevelXp - StringUtils.getExpDiff(c.Level)) + " | " + Math.Round(curexppercent) + "%)   | Stardust: " + profil.Profile.Currency.ToArray()[1].Amount + " | " + this._botStats;
                }
            }
        }

        private async Task TransferDuplicatePokemon(bool keepPokemonsThatCanEvolve = false)
        {
            if (this._clientSettings.TransferDoublePokemons)
            {
                var duplicatePokemons = await this._inventory.GetDuplicatePokemonToTransfer();

                foreach (var duplicatePokemon in duplicatePokemons)
                {
                    if (!this._clientSettings.pokemonsToHold.Contains(duplicatePokemon.PokemonId))
                    {
                        if (duplicatePokemon.Cp > this._clientSettings.DontTransferWithCPOver)
                        {
                            continue;
                        }

                        var bestPokemonOfType = await this._inventory.GetHighestCPofType(duplicatePokemon);

                        var transfer = await this._client.TransferPokemon(duplicatePokemon.Id);
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer {StringUtils.getPokemonNameByLanguage(this._clientSettings, duplicatePokemon.PokemonId)} with {duplicatePokemon.Cp} CP ({PokemonInfo.CalculatePokemonPerfection(duplicatePokemon)} % perfect) (Best: {bestPokemonOfType} CP)", LogLevel.Info);
                        await RandomHelper.RandomDelay(500, 700);
                    }
                }
            }
        }
    }
}