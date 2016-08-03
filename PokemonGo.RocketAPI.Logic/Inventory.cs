using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.GeneratedCode;
using System;
using System.Threading;
using System.IO;

namespace PokemonGo.RocketAPI.Logic
{
    public class Inventory
    {
        private readonly Client _client;
        public static DateTime LastRefresh;
        public static GetInventoryResponse CachedInventory;

        public Inventory(Client client)
        {
            _client = client;
        }

        public async Task<IEnumerable<PokemonData>> GetHighestCPofType2(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(PokemonInfo.CalculatePokemonPerfection)
                    .ToList();
        }

        public async Task<IEnumerable<PokemonData>> GetHighestIVofType(PokemonData pokemon)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(x => x.Cp)
                    .ToList();
        }

        public async Task<int> GetHighestCPofType(PokemonData pokemon)
        {
            try
            {
                var myPokemon = await GetPokemons();
                var pokemons = myPokemon.ToList();
                return pokemons.Where(x => x.PokemonId == pokemon.PokemonId)
                                .OrderByDescending(x => x.Cp)
                                .First().Cp;
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public async Task<int> GetEggsCount()
        {
            var p = await GetEggs();
            var i = p.Count();
            return i;
        }

        public async Task<int> GetPokemonCount()
        {
            var p = await GetPokemons();
            var i = p.Count();
            return i;
        }

        public async Task<int> GetInventoryCount()
        {
            var p = await GetItems();
            var i = p.Where(n => n != null).Sum(f => f.Count);
            return i;
        }

        public async Task<IEnumerable<PokemonData>> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public async Task<IEnumerable<PokemonData>> GetEggs()
        {
            var inventory = await GetCachedInventory(_client);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p.IsEgg);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await GetCachedInventory(_client);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p.PokemonId > 0);
        }
        public async Task<IEnumerable<PokemonFamily>> GetPokemonFamilies()
        {
            var inventory = await GetCachedInventory(_client);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await _client.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
        {
            var inventory = await GetCachedInventory(_client);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(IEnumerable<PokemonId> filter = null, bool orderByIv = false)
        {
            var myPokemons = await GetPokemons();
            myPokemons = myPokemons.Where(p => p.DeployedFortId == 0).OrderByDescending(p => p.Cp); //Don't evolve pokemon in gyms
            if (filter != null)
            {
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            }
            var pokemons = myPokemons.ToList();

            var myPokemonSettings = await GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies = await GetPokemonFamilies();
            var pokemonFamilies = myPokemonFamilies.ToArray();

            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                //Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(p => pokemonSettings.Single(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId) * settings.CandyToEvolve;
                if (familyCandy.Candy - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }
            if (orderByIv)
            {
                return pokemonToEvolve.OrderByDescending(PokemonInfo.CalculatePokemonPerfection);
            }
            else
            {
                return pokemonToEvolve.OrderByDescending(x => x.Cp);
            }
        }

        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(bool keepPokemonsThatCanEvolve = false, bool orderByIv = false)
        {
            var myPokemon = await GetPokemons();

            var pokemonList = myPokemon.Where(p => p.DeployedFortId == 0 && p.Favorite == 0).ToList();
            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId)
                    .ToList();

                var myPokemonSettings = await GetPokemonSettings();
                var pokemonSettings = myPokemonSettings as IList<PokemonSettings> ?? myPokemonSettings.ToList();

                var myPokemonFamilies = await GetPokemonFamilies();
                var pokemonFamilies = myPokemonFamilies as PokemonFamily[] ?? myPokemonFamilies.ToArray();

                foreach (var pokemon in pokemonsThatCanBeTransfered)
                {
                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.Key);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);
                    var amountToSkip = 0;

                    if (settings.CandyToEvolve != 0)
                    {
                        amountToSkip = familyCandy.Candy / settings.CandyToEvolve;
                    }

                    if (_client.getSettingHandle().HoldMaxDoublePokemons > amountToSkip)
                    {
                        amountToSkip = _client.getSettingHandle().HoldMaxDoublePokemons;
                    }
                    if (orderByIv)
                    {
                        results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
                            .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                            .ThenBy(n => n.StaminaMax)
                            .Skip(amountToSkip)
                            .ToList());

                    }
                    else
                    {
                        results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
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
                    .Where(x => x.Count() > 1)
                    .SelectMany(p => p.Where(x => x.Favorite == 0 && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
                    .OrderByDescending(PokemonInfo.CalculatePokemonPerfection)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(_client.getSettingHandle().HoldMaxDoublePokemons)
                    .ToList());

            }
            else
            {
                return pokemonList
                    .GroupBy(p => p.PokemonId)
                    .Where(x => x.Count() > 1)
                    .SelectMany(p => p.Where(x => x.Favorite == 0 && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
                    .OrderByDescending(x => x.Cp)
                    .ThenBy(n => n.StaminaMax)
                    .Skip(_client.getSettingHandle().HoldMaxDoublePokemons)
                    .ToList());
            }
        }

        public async Task ExportPokemonToCsv(Profile player, string filename = "PokemonList.csv")
        {
            if (player == null)
                return;
            var stats = await GetPlayerStats();
            var stat = stats.FirstOrDefault();
            if (stat == null)
                return;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

            if (Directory.Exists(path))
            {
                try
                {
                    string pokelistFile = Path.Combine(path, $"Profile_{player.Username}_{filename}");
                    if (File.Exists(pokelistFile))
                        File.Delete(pokelistFile);
                    string ls = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    string header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    File.WriteAllText(pokelistFile, $"{header.Replace(",", $"{ls}")}");

                    var allPokemon = await GetHighestsPerfect();
                    var myPokemonSettings = await GetPokemonSettings();
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies = await GetPokemonFamilies();
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    int trainerLevel = stat.Level;
                    int[] expReq = new[] { 0, 1000, 3000, 6000, 10000, 15000, 21000, 28000, 36000, 45000, 55000, 65000, 75000, 85000, 100000, 120000, 140000, 160000, 185000, 210000, 260000, 335000, 435000, 560000, 710000, 900000, 1100000, 1350000, 1650000, 2000000, 2500000, 3000000, 3750000, 4750000, 6000000, 7500000, 9500000, 12000000, 15000000, 20000000 };
                    int expReqAtLevel = expReq[stat.Level - 1];

                    using (var w = File.AppendText(pokelistFile))
                    {
                        w.WriteLine(string.Empty);
                        foreach (var pokemon in allPokemon)
                        {
                            string toEncode = $"{(int)pokemon.PokemonId}" + "," + trainerLevel + "," + PokemonInfo.GetLevel(pokemon) + "," + pokemon.Cp + "," + pokemon.Stamina;
                            //Generate base64 code to make it viewable here http://poke.isitin.org/#MTUwLDIzLDE3LDE5MDIsMTE4
                            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toEncode));

                            string isInGym = string.Empty;
                            var isFavorite = pokemon.Favorite != 0 ? "Yes" : "No";

                            var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId).Candy;
                            string perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string contentPart1 = $"{(int)pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
                            string contentPart2 = $",{pokemon.Move1},{pokemon.Move2},{pokemon.Stamina},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{familiecandies},{isInGym},{isFavorite},http://poke.isitin.org/#{encoded}";
                            string content = $"{contentPart1.Replace(",", $"{ls}")}\"{perfection}\"{contentPart2.Replace(",", $"{ls}")}";
                            w.WriteLine($"{content}");

                        }
                        w.Close();
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Export Player Infos and all Pokemon to \"\\Config\\{filename}\"", LogLevel.Info);
                }
                catch
                {
                    Logger.Error("Export Player Infos and all Pokemons to CSV not possible. File seems be in use!"/*, LogLevel.Warning*/);
                }
            }
        }


        public async Task<IEnumerable<Item>> GetItems(bool refresh = false)
        {
            var inventory = await GetCachedInventory(_client, refresh);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<Item>> GetItemsNonCache()
        {
            var inventory = await GetCachedInventory(_client, true);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<int> GetItemAmountByType(MiscEnums.Item type)
        {
            var pokeballs = await GetItems();
            return pokeballs.FirstOrDefault(i => (MiscEnums.Item)i.Item_ == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<Item>> GetItemsToRecycle(ISettings settings, bool refresh = false)
        {
            var myItems = await GetItems(refresh);

            return myItems
                .Where(x => settings.itemRecycleFilter.Any(f => f.Key == ((ItemId)x.Item_) && x.Count > f.Value))
                .Select(x => new Item { Item_ = x.Item_, Count = x.Count - settings.itemRecycleFilter.Single(f => f.Key == (ItemId)x.Item_).Value, Unseen = x.Unseen });
        }

        public static async Task<GetInventoryResponse> GetCachedInventory(Client client, bool request = false)
        {
            var now = DateTime.UtcNow;
            var ss = new SemaphoreSlim(10);

            if (LastRefresh.AddSeconds(30).Ticks > now.Ticks && request == false)
            {
                return CachedInventory;
            }
            await ss.WaitAsync();
            try
            {
                LastRefresh = now;
                try
                {
                    CachedInventory = await client.GetInventory();
                }
                catch
                {
                }

                return CachedInventory;
            }
            finally
            {
                ss.Release();
            }
        }

        DateTime _lastegguse;
        public async Task UseLuckyEgg(Client client)
        {
            var inventory = await GetItems();
            var luckyEgg = inventory.FirstOrDefault(p => (ItemId)p.Item_ == ItemId.ItemLuckyEgg);

            if (_lastegguse > DateTime.Now.AddSeconds(5))
            {
                TimeSpan duration = _lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0) { return; }

            await _client.UseItemXpBoost(ItemId.ItemLuckyEgg);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            _lastegguse = DateTime.Now.AddMinutes(30);
            await Task.Delay(3000);
        }

        public async Task<IEnumerable<EggIncubator>> GetEggIncubators(bool includeBasicIncubators)
        {
            var inventory = await GetCachedInventory(_client);
            var availableIncubators = inventory.InventoryDelta.InventoryItems.Where(x => x.InventoryItemData.EggIncubators != null)
                                                          .Select(i => i.InventoryItemData.EggIncubators.EggIncubator)
                                                          .Where(i => i != null);

            var incubators = !includeBasicIncubators ? availableIncubators.Where(x => x.ItemId == ItemId.ItemIncubatorBasicUnlimited.ToString()) :
                                                       availableIncubators.Where(x => x.UsesRemaining > 0 || x.ItemId == ItemId.ItemIncubatorBasicUnlimited.ToString());
            return incubators;
        }
    }
}
