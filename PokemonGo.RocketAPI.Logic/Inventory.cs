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
        public static DateTime _lastRefresh;
        public static GetInventoryResponse _cachedInventory;

        public Inventory(Client client)
        {
            _client = client;
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
            } catch (Exception)
            {
                return 0;
            }

        }

        public async Task<int> getPokemonCount()
        {
            int i = 0;
            var p = await GetPokemons();
            i = p.Count();
            return i;
        }

        public async Task<int> getInventoryCount()
        {
            int i = 0;
            var p = await GetItems();
            i = p.Where(n => n != null).Sum(f => f.Count); 
            return i;
        }

        public async Task<IEnumerable<PokemonData>> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon = await GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await getCachedInventory(_client);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon)
                    .Where(p => p != null && p?.PokemonId > 0);
        } 
        public async Task<IEnumerable<PokemonFamily>> GetPokemonFamilies()
        {
            var inventory = await getCachedInventory(_client);
            return
                inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await _client.GetItemTemplates();
            return
                templates.ItemTemplates.Select(i => i.PokemonSettings)
                    .Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
        {
            var inventory = await getCachedInventory(_client);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.PlayerStats)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(IEnumerable<PokemonId> filter = null)
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

            return pokemonToEvolve;
        }



        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(bool keepPokemonsThatCanEvolve = false)
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

                    results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
                        .OrderByDescending(x => x.Cp)
                        .ThenBy(n => n.StaminaMax)
                        .Skip(amountToSkip)
                        .ToList());

                }

                return results;
            }
            
            return pokemonList
                .GroupBy(p => p.PokemonId)
                .Where(x => x.Count() > 1)
                .SelectMany(p => p.Where(x => x.Favorite == 0 && PokemonInfo.CalculatePokemonPerfection(x) <= _client.getSettingHandle().ivmaxpercent)
                .OrderByDescending(x => x.Cp)
                .ThenBy(n => n.StaminaMax)
                .Skip(_client.getSettingHandle().HoldMaxDoublePokemons)
                .ToList());
        }

        public async Task ExportPokemonToCSV(Profile player, string filename = "PokemonList.csv")
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
                    string pokelist_file = Path.Combine(path, $"Profile_{player.Username}_{filename}");
                    if (File.Exists(pokelist_file))
                        File.Delete(pokelist_file);
                    string ls = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    string header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    File.WriteAllText(pokelist_file, $"{header.Replace(",", $"{ls}")}");

                    var AllPokemon = await GetHighestsPerfect();
                    var myPokemonSettings = await GetPokemonSettings();
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies = await GetPokemonFamilies();
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
 
                            if (pokemon.Favorite != 0)
                                IsFavorite = "Yes";
                            else
                                IsFavorite = "No";

                            var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId).Candy;
                            string perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string content_part1 = $"{(int)pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
                            string content_part2 = $",{pokemon.Move1},{pokemon.Move2},{pokemon.Stamina},{pokemon.IndividualAttack},{pokemon.IndividualDefense},{pokemon.IndividualStamina},{familiecandies},{IsInGym},{IsFavorite},http://poke.isitin.org/#{encoded}";
                            string content = $"{content_part1.Replace(",", $"{ls}")}\"{perfection}\"{content_part2.Replace(",", $"{ls}")}";
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


        public async Task<IEnumerable<Item>> GetItems()
        {
            var inventory = await getCachedInventory(_client);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<IEnumerable<Item>> GetItemsNonCache()
        {
            var inventory = await getCachedInventory(_client, true);
            return inventory.InventoryDelta.InventoryItems
                .Select(i => i.InventoryItemData?.Item)
                .Where(p => p != null);
        }

        public async Task<int> GetItemAmountByType(MiscEnums.Item type)
        {
            var pokeballs = await GetItems();
            return pokeballs.FirstOrDefault(i => (MiscEnums.Item)i.Item_ == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<Item>> GetItemsToRecycle(ISettings settings)
        {
            var myItems = await GetItems();

            return myItems
                .Where(x => settings.itemRecycleFilter.Any(f => f.Key == ((ItemId)x.Item_) && x.Count > f.Value))
                .Select(x => new Item { Item_ = x.Item_, Count = x.Count - settings.itemRecycleFilter.Single(f => f.Key == (ItemId)x.Item_).Value, Unseen = x.Unseen });
        }

        public static async Task<GetInventoryResponse> getCachedInventory(Client _client, bool request = false)
        {
            var now = DateTime.UtcNow;
            var ss = new SemaphoreSlim(10);

            if (_lastRefresh.AddSeconds(30).Ticks > now.Ticks && request == false)
            {
                return _cachedInventory;
            }
            await ss.WaitAsync();
            try
            {
                _lastRefresh = now;  
                try
                {
                    _cachedInventory = await _client.GetInventory();
                }
                catch
                {
                }

                return _cachedInventory;
            }
            finally
            {
                ss.Release();
            }
        }

        DateTime lastegguse;
        public async Task UseLuckyEgg(Client client)
        {
            var inventory = await GetItems();
            var luckyEgg = inventory.Where(p => (ItemId)p.Item_ == ItemId.ItemLuckyEgg).FirstOrDefault();

            if (lastegguse > DateTime.Now.AddSeconds(5))
            {
                TimeSpan duration = lastegguse - DateTime.Now;
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Lucky Egg still running: {duration.Minutes}m{duration.Seconds}s");
                return;
            }

            if (luckyEgg == null || luckyEgg.Count <= 0) { return; }

            await _client.UseItemXpBoost(ItemId.ItemLuckyEgg);
            Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Used Lucky Egg, remaining: {luckyEgg.Count - 1}");
            lastegguse = DateTime.Now.AddMinutes(30);
            await Task.Delay(3000);
        }
    }
}
