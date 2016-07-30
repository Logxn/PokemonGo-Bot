namespace PokemonGo.RocketAPI.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using PokemonGo.RocketAPI.GeneratedCode;

    public class Inventory
    {
        public static GetInventoryResponse _cachedInventory;
        public static DateTime _lastRefresh;
        private readonly Client _client;

        public Inventory(Client client)
        {
            this._client = client;
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

        public async Task ExportPokemonToCSV(Profile player, string filename = "PokemonList.csv")
        {
            if (player == null)
                return;
            var stats = await this.GetPlayerStats();
            var stat = stats.FirstOrDefault();
            if (stat == null)
                return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");

            if (Directory.Exists(path))
            {
                try
                {
                    var pokelist_file = Path.Combine(path, $"Profile_{player.Username}_{filename}");
                    if (File.Exists(pokelist_file))
                        File.Delete(pokelist_file);
                    var ls = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    var header = "PokemonID,Name,NickName,CP / MaxCP,IV Perfection in %,Attack 1,Attack 2,HP,Attk,Def,Stamina,Familie Candies,IsInGym,IsFavorite,previewLink";
                    File.WriteAllText(pokelist_file, $"{header.Replace(",", $"{ls}")}");

                    var AllPokemon = await this.GetHighestsPerfect();
                    var myPokemonSettings = await this.GetPokemonSettings();
                    var pokemonSettings = myPokemonSettings.ToList();
                    var myPokemonFamilies = await this.GetPokemonFamilies();
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    var trainerLevel = stat.Level;
                    int[] exp_req =
                    {
                        0, 1000, 3000, 6000, 10000, 15000, 21000, 28000, 36000, 45000, 55000, 65000, 75000, 85000, 100000, 120000, 140000, 160000, 185000, 210000, 260000, 335000, 435000, 560000, 710000, 900000, 1100000, 1350000, 1650000, 2000000, 2500000, 3000000, 3750000, 4750000, 6000000, 7500000, 9500000, 12000000, 15000000, 20000000
                    };
                    var exp_req_at_level = exp_req[stat.Level - 1];

                    using (var w = File.AppendText(pokelist_file))
                    {
                        w.WriteLine(string.Empty);
                        foreach (var pokemon in AllPokemon)
                        {
                            var toEncode = $"{(int) pokemon.PokemonId}" + "," + trainerLevel + "," + PokemonInfo.GetLevel(pokemon) + "," + pokemon.Cp + "," + pokemon.Stamina;

                            // Generate base64 code to make it viewable here http://poke.isitin.org/#MTUwLDIzLDE3LDE5MDIsMTE4
                            var encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(toEncode));

                            var IsInGym = string.Empty;
                            var IsFavorite = string.Empty;

                            if (pokemon.Favorite != 0)
                                IsFavorite = "Yes";
                            else
                                IsFavorite = "No";

                            var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                            var familiecandies = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId).Candy;
                            var perfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            perfection = perfection.Replace(",", ls == "," ? "." : ",");
                            string content_part1 = $"{(int) pokemon.PokemonId},{pokemon.PokemonId},{pokemon.Nickname},{pokemon.Cp}/{PokemonInfo.CalculateMaxCP(pokemon)},";
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
                    Logger.Error("Export Player Infos and all Pokemons to CSV not possible. File seems be in use!" /*, LogLevel.Warning*/);
                }
            }
        }

        public async Task<IEnumerable<PokemonData>> GetDuplicatePokemonToTransfer(bool keepPokemonsThatCanEvolve = false)
        {
            var myPokemon = await this.GetPokemons();

            var pokemonList = myPokemon.Where(p => p.DeployedFortId == 0 && p.Favorite == 0).ToList();
            if (keepPokemonsThatCanEvolve)
            {
                var results = new List<PokemonData>();
                var pokemonsThatCanBeTransfered = pokemonList.GroupBy(p => p.PokemonId).ToList();

                var myPokemonSettings = await this.GetPokemonSettings();
                var pokemonSettings = myPokemonSettings as IList<PokemonSettings> ?? myPokemonSettings.ToList();

                var myPokemonFamilies = await this.GetPokemonFamilies();
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

                    if (this._client.getSettingHandle().HoldMaxDoublePokemons > amountToSkip)
                    {
                        amountToSkip = this._client.getSettingHandle().HoldMaxDoublePokemons;
                    }

                    results.AddRange(pokemonList.Where(x => x.PokemonId == pokemon.Key && PokemonInfo.CalculatePokemonPerfection(x) <= this._client.getSettingHandle().ivmaxpercent).OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax).Skip(amountToSkip).ToList());
                }

                return results;
            }

            return pokemonList.GroupBy(p => p.PokemonId).Where(x => x.Count() > 1).SelectMany(p => p.Where(x => x.Favorite == 0 && PokemonInfo.CalculatePokemonPerfection(x) <= this._client.getSettingHandle().ivmaxpercent).OrderByDescending(x => x.Cp).ThenBy(n => n.StaminaMax).Skip(this._client.getSettingHandle().HoldMaxDoublePokemons).ToList());
        }

        public async Task<int> GetHighestCPofType(PokemonData pokemon)
        {
            var myPokemon = await this.GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.Where(x => x.PokemonId == pokemon.PokemonId).OrderByDescending(x => x.Cp).First().Cp;
        }

        public async Task<IEnumerable<PokemonData>> GetHighestsPerfect(int limit = 1000)
        {
            var myPokemon = await this.GetPokemons();
            var pokemons = myPokemon.ToList();
            return pokemons.OrderByDescending(PokemonInfo.CalculatePokemonPerfection).Take(limit);
        }

        public async Task<int> getInventoryCount()
        {
            var i = 0;
            var p = await this.GetItems();
            i = p.Where(n => n != null).Sum(f => f.Count);
            return i;
        }

        public async Task<int> GetItemAmountByType(MiscEnums.Item type)
        {
            var pokeballs = await this.GetItems();
            return pokeballs.FirstOrDefault(i => (MiscEnums.Item) i.Item_ == type)?.Count ?? 0;
        }

        public async Task<IEnumerable<Item>> GetItems()
        {
            var inventory = await getCachedInventory(this._client);
            return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Item).Where(p => p != null);
        }

        public async Task<IEnumerable<Item>> GetItemsToRecycle(ISettings settings)
        {
            var myItems = await this.GetItems();

            return myItems.Where(x => settings.itemRecycleFilter.Any(f => f.Key == (ItemId) x.Item_ && x.Count > f.Value)).Select(x => new Item
                                                                                                                                       {
                                                                                                                                           Item_ = x.Item_,
                                                                                                                                           Count = x.Count - settings.itemRecycleFilter.Single(f => f.Key == (ItemId) x.Item_).Value,
                                                                                                                                           Unseen = x.Unseen
                                                                                                                                       });
        }

        public async Task<IEnumerable<PlayerStats>> GetPlayerStats()
        {
            var inventory = await getCachedInventory(this._client);
            return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PlayerStats).Where(p => p != null);
        }

        public async Task<int> getPokemonCount()
        {
            var i = 0;
            var p = await this.GetPokemons();
            i = p.Count();
            return i;
        }

        public async Task<IEnumerable<PokemonFamily>> GetPokemonFamilies()
        {
            var inventory = await getCachedInventory(this._client);
            return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemons()
        {
            var inventory = await getCachedInventory(this._client);
            return inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p != null && p?.PokemonId > 0);
        }

        public async Task<IEnumerable<PokemonSettings>> GetPokemonSettings()
        {
            var templates = await this._client.GetItemTemplates();
            return templates.ItemTemplates.Select(i => i.PokemonSettings).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
        }

        public async Task<IEnumerable<PokemonData>> GetPokemonToEvolve(IEnumerable<PokemonId> filter = null)
        {
            var myPokemons = await this.GetPokemons();
            myPokemons = myPokemons.Where(p => p.DeployedFortId == 0).OrderByDescending(p => p.Cp); // Don't evolve pokemon in gyms
            if (filter != null)
            {
                myPokemons = myPokemons.Where(p => filter.Contains(p.PokemonId));
            }

            var pokemons = myPokemons.ToList();

            var myPokemonSettings = await this.GetPokemonSettings();
            var pokemonSettings = myPokemonSettings.ToList();

            var myPokemonFamilies = await this.GetPokemonFamilies();
            var pokemonFamilies = myPokemonFamilies.ToArray();

            var pokemonToEvolve = new List<PokemonData>();
            foreach (var pokemon in pokemons)
            {
                var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                // Don't evolve if we can't evolve it
                if (settings.EvolutionIds.Count == 0)
                    continue;

                var pokemonCandyNeededAlready = pokemonToEvolve.Count(p => pokemonSettings.Single(x => x.PokemonId == p.PokemonId).FamilyId == settings.FamilyId) * settings.CandyToEvolve;
                if (familyCandy.Candy - pokemonCandyNeededAlready > settings.CandyToEvolve)
                    pokemonToEvolve.Add(pokemon);
            }

            return pokemonToEvolve;
        }
    }
}