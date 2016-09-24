using System.Globalization;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Console.PokeData;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Threading;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Logic.Utils;
using System.Collections.Generic;
using static PokemonGo.RocketAPI.Console.GUI;
using POGOProtos.Inventory.Item;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Response;
using GMap.NET;
using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Device.Location;

namespace PokemonGo.RocketAPI.Console
{
    public partial class PokemonsPanel : UserControl
    {
        public static string languagestr2;
        private static Client client;
        private static GetPlayerResponse profile;
        private static POGOProtos.Data.Player.PlayerStats stats;
        private static GetInventoryResponse inventory;
        static Profile ActiveProfile = new Profile();
        private static IOrderedEnumerable<PokemonData> pokemons;
        private static List<AdditionalPokeData> additionalPokeData = new List<AdditionalPokeData>();
        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();

        private void loadAdditionalPokeData()
        {
            try
            {
                var path = "PokeData\\AdditionalPokeData.json";
                var jsonData = File.ReadAllText(path);
                additionalPokeData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdditionalPokeData>>(jsonData);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Could not load additional PokeData", LogLevel.Error);
            }
        }

        public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse() { }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
        public PokemonsPanel()
        {
            InitializeComponent();
            ClientSettings = new Settings();

            InitialzePokemonListView();
            loadAdditionalPokeData();
        }

        public static ISettings ClientSettings;



        public async Task check()
        {
            while (true)
            {
                try
                {
                    if (Logic.Logic._client != null && Logic.Logic._client.readyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }

        public async void Execute()
        {
            EnabledButton(false, "Reloading Pokemon list.");
            await check();
            try
            {
                client = Logic.Logic._client;
                if (client.readyToUse != false)
                {
                    profile = await client.Player.GetPlayer();
                    await Task.Delay(1000); // Pause to simulate human speed. 
                    inventory = await client.Inventory.GetInventory();
                    pokemons =
                        inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.PokemonData)
                            .Where(p => p != null && p?.PokemonId > 0)
                            .OrderByDescending(key => key.Cp);
                    var families = inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Candy)
                        .Where(p => p != null && (int)p?.FamilyId > 0)
                        .OrderByDescending(p => (int)p.FamilyId);

                    var imageSize = 50;

                    var imageList = new ImageList { ImageSize = new Size(imageSize, imageSize) };
                    PokemonListView.SmallImageList = imageList;

                    var templates = await client.Download.GetItemTemplates();
                    var myPokemonSettings = templates.ItemTemplates.Select(i => i.PokemonSettings).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                    var pokemonSettings = myPokemonSettings.ToList();

                    var myPokemonFamilies = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Candy).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    int ie = 1;                    
                    foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
                    {
                        if (pokemon.ToString() != "Missingno")
                        {
                            pokeIDS[pokemon.ToString()] = ie;                            
                            ie++;
                        }
                    }
                    foreach (PokemonId Id in Globals.NotToSnipe)
                    {
                        string _id = Id.ToString();
                    }
                    PokemonListView.BeginUpdate();
                    foreach (var pokemon in pokemons)
                    {
                        Bitmap pokemonImage = null;
                        await Task.Run(() =>
                        {
                            pokemonImage = GetPokemonLargeImage(pokemon.PokemonId);
                        });
                        imageList.Images.Add(pokemon.PokemonId.ToString(), pokemonImage);

                        PokemonListView.LargeImageList = imageList;
                        var listViewItem = new ListViewItem();
                        listViewItem.Tag = pokemon;



                        var currentCandy = families
                            .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                            .Select(f => f.Candy_)
                            .First();
                        listViewItem.SubItems.Add(string.Format("{0}", pokemon.Cp));
                        //<listViewItem.SubItems.Add(string.Format("{0}% {1}{2}{3} ({4})", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack.ToString("X"), pokemon.IndividualDefense.ToString("X"), pokemon.IndividualStamina.ToString("X"), (45 - pokemon.IndividualAttack- pokemon.IndividualDefense- pokemon.IndividualStamina) ));
                        listViewItem.SubItems.Add(string.Format("{0}% {1}-{2}-{3}", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina));
                        listViewItem.SubItems.Add(string.Format("{0}", PokemonInfo.GetLevel(pokemon)));
                        listViewItem.ImageKey = pokemon.PokemonId.ToString();

                        listViewItem.Text = string.Format((pokemon.Favorite == 1) ? "{0} ★" : "{0}", StringUtils.getPokemonNameByLanguage(ClientSettings, (PokemonId)pokemon.PokemonId));

                        listViewItem.ToolTipText = StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs,"dd/MM/yyyy HH:mm:ss");
                        if (pokemon.Nickname != "")
                            listViewItem.ToolTipText += "\nNickname: " + pokemon.Nickname;

                        var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                        var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                        if (settings.EvolutionIds.Count > 0 && familyCandy.Candy_ >= settings.CandyToEvolve)
                        {
                            listViewItem.SubItems.Add("Y (" + familyCandy.Candy_ + "/" + settings.CandyToEvolve + ")");
                            listViewItem.Checked = true;
                        }
                        else
                        {
                            if (settings.EvolutionIds.Count > 0)
                                listViewItem.SubItems.Add("N (" + familyCandy.Candy_ + "/" + settings.CandyToEvolve + ")");
                            else
                                listViewItem.SubItems.Add("N (" + familyCandy.Candy_ + "/Max)");
                        }
                        listViewItem.SubItems.Add(string.Format("{0}", Math.Round(pokemon.HeightM, 2)));
                        listViewItem.SubItems.Add(string.Format("{0}", Math.Round(pokemon.WeightKg, 2)));
                        listViewItem.SubItems.Add(string.Format("{0}/{1}", pokemon.Stamina, pokemon.StaminaMax));
                        listViewItem.SubItems.Add(string.Format("{0}", pokemon.Move1));
                        listViewItem.SubItems.Add(string.Format("{0} ({1})", pokemon.Move2, PokemonInfo.GetAttack(pokemon.Move2)));
                        listViewItem.SubItems.Add(string.Format("{0}", (int)pokemon.PokemonId));
                        listViewItem.SubItems.Add(string.Format("{0}", PokemonInfo.CalculatePokemonPerfectionCP(pokemon).ToString("0.00")));

                        AdditionalPokeData addData = additionalPokeData.FirstOrDefault(x => x.PokedexNumber == (int)pokemon.PokemonId);
                        if (addData != null)
                        {
                            listViewItem.SubItems.Add(addData.Type1);
                            listViewItem.SubItems.Add(addData.Type2);
                        }
                        else
                        {
                            listViewItem.SubItems.Add("");
                            listViewItem.SubItems.Add("");
                        }
                        // NOTE: yyyy/MM/dd is inverted order to can sort correctly as text. 
                        listViewItem.SubItems.Add(StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "yyyy/MM/dd HH:mm:ss"));
                        listViewItem.SubItems.Add(pokemon.Pokeball.ToString().Replace("Item", ""));
                        listViewItem.SubItems.Add("" + pokemon.NumUpgrades);
                        listViewItem.SubItems.Add("" + pokemon.BattlesAttacked);
                        listViewItem.SubItems.Add("" + pokemon.BattlesDefended);

                        PokemonListView.Items.Add(listViewItem);
                    }
                    PokemonListView.EndUpdate();
                    PokemonListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    Text = "Pokemon List | User: " + profile.PlayerData.Username + " | Pokemons: " + pokemons.Count() + "/" + profile.PlayerData.MaxPokemonStorage;
                    EnabledButton(true);
                    button2.Enabled = false;
                    checkBox1.Enabled = false;
                    statusTexbox.Text = string.Empty;

                    var arrStats = await client.Inventory.GetPlayerStats();
                    stats = arrStats.First();

                }
            }
            catch (Exception e)
            {

                Logger.Error("[PokemonList-Error] " + e.StackTrace);
                await Task.Delay(1000); // Lets the API make a little pause, so we dont get blocked
                Execute();
            }
        }



        private void EnabledButton(bool enabled, string reason = "")
        {
            statusTexbox.Text = reason;
            btnreload.Enabled = enabled;
            btnEvolve.Enabled = enabled;
            btnTransfer.Enabled = enabled;
            btnUpgrade.Enabled = enabled;
            btnFullPowerUp.Enabled = enabled;
            btnShowMap.Enabled = enabled;
            checkBoxreload.Enabled = enabled;
            reloadsecondstextbox.Enabled = enabled;
            PokemonListView.Enabled = enabled;
            btnIVToNick.Enabled = enabled;
            button1.Enabled = enabled;
            button3.Enabled = enabled;
        }

        public static Bitmap GetPokemonSmallImage(PokemonId pokemon)
        {
            return getPokemonImagefromResource(pokemon, "20");
        }

        public static Bitmap GetPokemonMediumImage(PokemonId pokemon)
        {
            return getPokemonImagefromResource(pokemon, "35");
        }

        public static Bitmap GetPokemonLargeImage(PokemonId pokemon)
        {
            return getPokemonImagefromResource(pokemon, "50");
        }

        public static Bitmap GetPokemonVeryLargeImage(PokemonId pokemon)
        {
            return getPokemonImagefromResource(pokemon, "200");
        }

        /// <summary>
        /// Gets the pokemon imagefrom resource.
        /// </summary>
        /// <param name="pokemon">The pokemon.</param>
        /// <param name="size">The size.</param>
        /// <returns></returns>
        private static Bitmap getPokemonImagefromResource(PokemonId pokemon, string size)
        {
            var resource = PokemonGo.RocketAPI.Console.Properties.PokemonSprites.ResourceManager.GetObject("_" + (int)pokemon + "_" + size, CultureInfo.CurrentCulture);
            if (resource != null && resource is Bitmap)
            {
                return new Bitmap(resource as Bitmap);
            }
            else
                return null;
        }


        private void btnReload_Click(object sender, EventArgs e)
        {
            PokemonListView.Items.Clear();
            Execute();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (PokemonListView.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    if (PokemonListView.SelectedItems.Count > 1)
                    {
                        MessageBox.Show("You can only select 1 item for quick action!", "Selection to large", MessageBoxButtons.OK);
                        return;
                    }
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to transfer?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await transferPokemon(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.Items.Remove(PokemonListView.SelectedItems[0]);
                Text = "Pokemon List | User: " + profile.PlayerData.Username + " | Pokemons: " + PokemonListView.Items.Count + "/" + profile.PlayerData.MaxPokemonStorage;
            }
            else
                MessageBox.Show(resp.Message + " transfer failed!", "Transfer Status", MessageBoxButtons.OK);
        }

        private ColumnHeader SortingColumn = null;

        private void PokemonListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = PokemonListView.Columns[e.Column];
            System.Windows.Forms.SortOrder sort_order;
            if (SortingColumn == null)
            {
                sort_order = SortOrder.Ascending;
            }
            else
            {
                if (new_sorting_column == SortingColumn)
                {
                    if (SortingColumn.Text.StartsWith("> "))
                    {
                        sort_order = SortOrder.Descending;
                    }
                    else
                    {
                        sort_order = SortOrder.Ascending;
                    }
                }
                else
                {
                    sort_order = SortOrder.Ascending;
                }
                SortingColumn.Text = SortingColumn.Text.Substring(2);
            }

            // Display the new sort order.
            SortingColumn = new_sorting_column;
            if (sort_order == SortOrder.Ascending)
            {
                SortingColumn.Text = "> " + SortingColumn.Text;
            }
            else
            {
                SortingColumn.Text = "< " + SortingColumn.Text;
            }

            // Create a comparer.
            PokemonListView.ListViewItemSorter = new ListViewComparer(e.Column, sort_order);

            // Sort.
            PokemonListView.Sort();
        }

        private async void btnEvolve_Click(object sender, EventArgs e)
        {

            EnabledButton(false, "Evolving...");
            var selectedItems = PokemonListView.SelectedItems;
            int evolved = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            var date = DateTime.Now.ToString();
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string evolvelog = System.IO.Path.Combine(logPath, "EvolveLog.txt");

            taskResponse resp = new taskResponse(false, string.Empty);

            if (Globals.pauseAtEvolve2)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Taking a break to evolve some pokemons!");
                Globals.pauseAtWalking = true;
            }


            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await evolvePokemon((PokemonData)selectedItem.Tag);

                var pokemoninfo = (PokemonData)selectedItem.Tag;
                var name = pokemoninfo.PokemonId;

                File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Trying to evole Pokemon: {name}" + Environment.NewLine);
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Trying to Evolve {name}");

                if (resp.Status)
                {
                    evolved++;
                    statusTexbox.Text = "Evolving..." + evolved;
                }
                else
                    failed += resp.Message + " ";
                if (Globals.UseAnimationTimes)
                {
                    await RandomHelper.RandomDelay(30000, 35000);
                }
                else
                {
                    await RandomHelper.RandomDelay(500, 800);
                }
            }


            if (failed != string.Empty)
            {
                if (_clientSettings.bLogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                }
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons. Failed: " + failed, "Evolve status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
            {
                if (_clientSettings.bLogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons." + Environment.NewLine);
                }
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons.", "Evolve status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (evolved > 0)
            {
                PokemonListView.Items.Clear();
                Execute();
            }
            else
                EnabledButton(true);

            if (Globals.pauseAtEvolve)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved everything. Time to continue our journey!");
                Globals.pauseAtWalking = false;
            }
            //}
        }

        private async void btnTransfer_Click(object sender, EventArgs e)
        {
            EnabledButton(false, "Transfering...");
            var selectedItems = PokemonListView.SelectedItems;
            int transfered = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;

            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string logs = System.IO.Path.Combine(logPath, "TransferLog.txt");
            string date = DateTime.Now.ToString();
            PokemonData pokeData = new PokemonData();


            DialogResult dialogResult = MessageBox.Show("You clicked transfer. This can not be undone.", "Are you Sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                taskResponse resp = new taskResponse(false, string.Empty);

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    resp = await transferPokemon((PokemonData)selectedItem.Tag);
                    if (resp.Status)
                    {
                        var PokemonInfo = (PokemonData)selectedItem.Tag;
                        var name = PokemonInfo.PokemonId;

                        File.AppendAllText(logs, $"[{date}] - MANUAL - Trying to transfer pokemon: {name}" + Environment.NewLine);

                        PokemonListView.Items.Remove(selectedItem);
                        transfered++;
                        statusTexbox.Text = "Transfering..." + transfered;

                    }
                    else
                        failed += resp.Message + " ";
                    await RandomHelper.RandomDelay(5000, 6000);
                }



                if (failed != string.Empty)
                {
                    if (_clientSettings.logManualTransfer)
                    {
                        File.AppendAllText(logs, $"[{date}] - MANUAL - Sucessfully transfered {transfered}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                    }
                    MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (_clientSettings.logManualTransfer)
                    {
                        File.AppendAllText(logs, $"[{date}] - MANUAL - Sucessfully transfered {transfered}/{total} Pokemons." + Environment.NewLine);
                    }
                    MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                Text = "Pokemon List | User: " + profile.PlayerData.Username + " | Pokemons: " + PokemonListView.Items.Count + "/" + profile.PlayerData.MaxPokemonStorage;
            }
            EnabledButton(true);
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            var selectedItems = PokemonListView.SelectedItems;
            int powerdup = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            taskResponse resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await PowerUp((PokemonData)selectedItem.Tag);
                if (resp.Status)
                    powerdup++;
                else
                    failed += resp.Message + " ";
                await RandomHelper.RandomDelay(1000, 3000);
            }
            if (failed != string.Empty)
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (powerdup > 0)
            {
                PokemonListView.Items.Clear();
                Execute();
            }
            else
                EnabledButton(true);
        }

        private async void BtnIVToNickClick(object sender, EventArgs e)
        {
            EnabledButton(false, "Renaming...");
            var selectedItems = PokemonListView.SelectedItems;
            int renamed = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;

            DialogResult dialogResult = MessageBox.Show("You clicked to change nickame using IVs.\nAre you Sure?", "Confirm Dialog", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                taskResponse resp = new taskResponse(false, string.Empty);

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    PokemonData pokemon = (PokemonData)selectedItem.Tag;
                    pokemon.Nickname = IVsToNickname(pokemon);
                    resp = await changePokemonNickname(pokemon);
                    if (resp.Status)
                    {
                        selectedItem.ToolTipText = StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                        selectedItem.ToolTipText += "\nNickname: " + pokemon.Nickname;
                        renamed++;
                        statusTexbox.Text = "Renamig..." + renamed;
                    }
                    else
                        failed += resp.Message + " ";
                    await RandomHelper.RandomDelay(5000, 6000);
                }

                if (failed != string.Empty)
                    MessageBox.Show("Succesfully renamed " + renamed + "/" + total + " Pokemons. Failed: " + failed, "Rename status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Succesfully renamed " + renamed + "/" + total + " Pokemons.", "Rename status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            EnabledButton(true);
        }

        private static async Task<taskResponse> evolvePokemon(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.Inventory.EvolvePokemon((ulong)pokemon.Id);

                if (evolvePokemonResponse.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error evolvePokemon: " + e.Message);
                await evolvePokemon(pokemon);
            }
            return resp;
        }

        private static async Task<taskResponse> transferPokemon(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var transferPokemonResponse = await client.Inventory.TransferPokemon(pokemon.Id);

                if (transferPokemonResponse.Result == ReleasePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error transferPokemon: " + e.Message);
                await transferPokemon(pokemon);
            }
            return resp;
        }

        private static async Task<taskResponse> PowerUp(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.Inventory.UpgradePokemon(pokemon.Id);

                if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error Powering Up: " + e.Message);
                await PowerUp(pokemon);
            }
            return resp;
        }

        private static string IVsToNickname(PokemonData pokemon)
        {
            string croppedName = StringUtils.getPokemonNameByLanguage(ClientSettings, (PokemonId)pokemon.PokemonId) + " ";
            string nickname;
            //<nickname = string.Format("{0}{1}{2}{3}", pokemon.IndividualAttack.ToString("X"), pokemon.IndividualDefense.ToString("X"), pokemon.IndividualStamina.ToString("X"),(45 - pokemon.IndividualAttack- pokemon.IndividualDefense- pokemon.IndividualStamina));
            nickname = string.Format("{0}.{1}.{2}.{3}", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina);
            int lenDiff = 12 - nickname.Length;
            if (croppedName.Length > lenDiff)
                croppedName = croppedName.Substring(0, lenDiff);
            return croppedName + nickname;
        }

        private static async Task<taskResponse> changePokemonNickname(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var nicknamePokemonResponse1 = await client.Inventory.NicknamePokemon(pokemon.Id, pokemon.Nickname);

                if (nicknamePokemonResponse1.Result == NicknamePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error changePokemonNickname: " + e.Message);
                await changePokemonNickname(pokemon);
            }
            return resp;
        }

        private static async Task<taskResponse> changeFavourites(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var response = await client.Inventory.SetFavoritePokemon((long)pokemon.Id, (pokemon.Favorite == 1));

                if (response.Result == SetFavoritePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error ChangeFavourites: " + e.Message);
                await changeFavourites(pokemon);
            }
            return resp;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PokemonListView.SelectedItems.Count > 0 && PokemonListView.SelectedItems[0].Checked)
                contextMenuStrip1.Items[2].Visible = true;
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            contextMenuStrip1.Items[2].Visible = false;
        }

        private async void evolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to evolve?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await evolvePokemon(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.Items.Clear();
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " evolving failed!", "Evolve Status", MessageBoxButtons.OK);
        }

        public static double[] FindLocation(string address)
        {
            double[] ret = { 0.0, 0.0 };
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPoint(address, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null)
            {
                ret = new double[2];
                ret[0] = pos.Value.Lat;
                ret[1] = pos.Value.Lng;
            }
            return ret;
        }

        private async void powerUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to power it up?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await PowerUp(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.Items.Clear();
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " powering up failed!", "PowerUp Status", MessageBoxButtons.OK);
        }

        private async void IVsToNicknameToolStripMenuItemClick(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            string promptValue = Prompt.ShowDialog(IVsToNickname(pokemon), "Confirm Nickname");

            if (promptValue != "")
            {
                pokemon.Nickname = promptValue;
                resp = await changePokemonNickname(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.SelectedItems[0].ToolTipText = StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                PokemonListView.SelectedItems[0].ToolTipText += "\nNickname: " + pokemon.Nickname;
            }
            else
                MessageBox.Show(resp.Message + " rename failed!", "Rename Status", MessageBoxButtons.OK);
        }

        private async void changeFavouritesToolStripMenuItemClick(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            string poname = StringUtils.getPokemonNameByLanguage(ClientSettings, (PokemonId)pokemon.PokemonId);
            if (MessageBox.Show(this, poname + " will be " + ((pokemon.Favorite == 1) ? "deleted from" : "added to") + " your favourites." + "\nAre you sure you want?", "Confirmation Message", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                pokemon.Favorite = (pokemon.Favorite == 1) ? 0 : 1;
                resp = await changeFavourites(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.SelectedItems[0].Text = string.Format((pokemon.Favorite == 1) ? "{0} ★" : "{0}", StringUtils.getPokemonNameByLanguage(ClientSettings, (PokemonId)pokemon.PokemonId));
            }
            else
                MessageBox.Show(resp.Message + " rename failed!", "Rename Status", MessageBoxButtons.OK);
        }

        private void checkboxReload_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxreload.Checked)
            {
                int def = (int)reloadsecondstextbox.Value;
                reloadtimer.Interval = def * 1000;
                reloadtimer.Start();
            }
            else
            {
                reloadtimer.Stop();
            }

        }

        private void reloadtimer_Tick(object sender, EventArgs e)
        {
            PokemonListView.Items.Clear();
            Execute();
        }


        private async void btnFullPowerUp_Click(object sender, EventArgs e)
        {
            EnabledButton(false, "Powering up...");
            DialogResult result = MessageBox.Show("This process may take some time.", "FullPowerUp status", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                var selectedItems = PokemonListView.SelectedItems;
                int poweredup = 0;
                int total = selectedItems.Count;
                string failed = string.Empty;

                taskResponse resp = new taskResponse(false, string.Empty);
                int i = 0;
                int powerUps = 0;
                while (i == 0)
                {
                    var poweruplimit = (int)numPwrUpLimit.Value;
                    foreach (ListViewItem selectedItem in selectedItems)
                    {
                        if (poweruplimit > 0)
                        {
                            if (poweredup < poweruplimit)
                            {
                                resp = await PowerUp((PokemonData)selectedItem.Tag);
                                if (resp.Status)
                                {
                                    poweredup++;
                                }
                                else
                                    failed += resp.Message + " ";
                            }
                            else
                                failed += " Power Up Limit Reached ";
                        }
                        else
                        {
                            resp = await PowerUp((PokemonData)selectedItem.Tag);
                            if (resp.Status)
                            {
                                poweredup++;
                            }
                            else
                                failed += resp.Message + " ";
                        }
                    }
                    if (failed != string.Empty)
                    {
                        if (powerUps > 0)
                        {
                            MessageBox.Show("Pokemon succesfully powered " + powerUps + " times.", "FullPowerUp status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Pokemon not powered up. Not enough Stardust or Candy.", "FullPowerUp status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        i = 1;
                        EnabledButton(true);
                    }
                    else
                    {
                        powerUps++;
                        statusTexbox.Text = "Powering up..." + powerUps;
                        await RandomHelper.RandomDelay(1200, 1500);
                    }
                }
                if (poweredup > 0 && i == 1)
                {
                    PokemonListView.Items.Clear();
                    Execute();
                }
            }
            else
            {
                EnabledButton(true);
            }
            //}
        }

        private void lang_en_btn2_Click(object sender, EventArgs e)
        {
            lang_de_btn_2.Enabled = true;
            lang_spain_btn2.Enabled = true;
            lang_en_btn2.Enabled = false;
            lang_ptBR_btn2.Enabled = true;
            lang_tr_btn2.Enabled = true;
            languagestr2 = null;

            // Pokemon List GUI
            btnreload.Text = "Reload";
            btnEvolve.Text = "Evolve";
            checkBoxreload.Text = "Reload every";
            btnUpgrade.Text = "PowerUp";
            btnFullPowerUp.Text = "FULL-PowerUp";
            btnForceUnban.Text = "Force Unban";
            btnTransfer.Text = "Transfer";
        }

        private void lang_de_btn_2_Click(object sender, EventArgs e)
        {
            lang_en_btn2.Enabled = true;
            lang_spain_btn2.Enabled = true;
            lang_de_btn_2.Enabled = false;
            lang_ptBR_btn2.Enabled = true;
            lang_tr_btn2.Enabled = true;
            languagestr2 = "de";

            // Pokemon List GUI
            btnreload.Text = "Aktualisieren";
            btnEvolve.Text = "Entwickeln";
            checkBoxreload.Text = "Aktualisiere alle";
            btnUpgrade.Text = "PowerUp";
            btnFullPowerUp.Text = "FULL-PowerUp";
            btnForceUnban.Text = "Force Unban";
            btnTransfer.Text = "Versenden";
        }

        private void lang_spain_btn2_Click(object sender, EventArgs e)
        {
            lang_en_btn2.Enabled = true;
            lang_de_btn_2.Enabled = true;
            lang_spain_btn2.Enabled = false;
            lang_ptBR_btn2.Enabled = true;
            lang_tr_btn2.Enabled = true;
            languagestr2 = "spain";

            // Pokemon List GUI
            btnreload.Text = "Actualizar";
            btnEvolve.Text = "Evolucionar";
            checkBoxreload.Text = "Actualizar cada";
            btnUpgrade.Text = "Dar más poder";
            btnFullPowerUp.Text = "Dar más poder [TOTAL]";
            btnForceUnban.Text = "Force Unban";
            btnTransfer.Text = "Transferir";
        }

        private void lang_ptBR_btn2_Click(object sender, EventArgs e)
        {
            lang_en_btn2.Enabled = true;
            lang_de_btn_2.Enabled = true;
            lang_spain_btn2.Enabled = true;
            lang_ptBR_btn2.Enabled = false;
            lang_tr_btn2.Enabled = true;
            languagestr2 = "ptBR";

            // Pokemon List GUI
            btnreload.Text = "Recarregar";
            btnEvolve.Text = "Evoluir (selecionados)";
            checkBoxreload.Text = "Recarregar a cada";
            btnUpgrade.Text = "PowerUp (selecionados)";
            btnFullPowerUp.Text = "FULL-PowerUp (selecionados)";
            btnForceUnban.Text = "Force Unban";
            btnTransfer.Text = "Transferir (selecionados)";

        }

        private void lang_tr_btn2_Click(object sender, EventArgs e)
        {
            lang_de_btn_2.Enabled = true;
            lang_spain_btn2.Enabled = true;
            lang_en_btn2.Enabled = true;
            lang_ptBR_btn2.Enabled = true;
            lang_tr_btn2.Enabled = false;
            languagestr2 = "tr";

            // Pokemon List GUI
            btnreload.Text = "Yenile";
            btnEvolve.Text = "Geliştir";
            checkBoxreload.Text = "Yenile her";
            btnUpgrade.Text = "Güçlendir";
            btnFullPowerUp.Text = "TAM-Güçlendir";
            btnForceUnban.Text = "Banı Kaldırmaya Zorla";
            btnTransfer.Text = "Transfer";
        }

        private void btnForceUnban_Click(object sender, EventArgs e)
        {
            // **MTK4355 Repurposed force unban button since force-unban feature is no longer working**
            //Logic.Logic.failed_softban = 6;
            //btnForceUnban.Enabled = false;
            //freezedenshit.Start();
            if (btnForceUnban.Text.Equals("Pause Walking"))
            {
                Globals.pauseAtPokeStop = true;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pausing at next Pokestop. (will continue catching pokemon and farming pokestop when available)");
                if (Globals.RouteToRepeat.Count > 0)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Cleared!");
                    Globals.RouteToRepeat.Clear();
                }

                btnForceUnban.Text = "Resume Walking";
                button2.Enabled = true;
                checkBox1.Enabled = true;
            }
            else
            {
                Globals.pauseAtPokeStop = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Resume walking between Pokestops.");
                if (Globals.RouteToRepeat.Count > 0)
                {
                    foreach (var geocoord in Globals.RouteToRepeat)
                    {
                        Globals.NextDestinationOverride.AddLast(geocoord);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Captured! Beginning Route Momentarily.");
                }
                btnForceUnban.Text = "Pause Walking";
                button2.Enabled = false;
                checkBox1.Enabled = false;
            }

        }

        private void freezedenshit_Tick(object sender, EventArgs e)
        {
            btnForceUnban.Enabled = true;
            freezedenshit.Stop();
        }

        private void InitialzePokemonListView()
        {
            PokemonListView.Columns.Clear();
            ColumnHeader columnheader;
            columnheader = new ColumnHeader();
            columnheader.Name = "Name";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "CP";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "IV A-D-S";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "LVL";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Evolvable?";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Height";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Weight";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "HP";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Attack";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "SpecialAttack (DPS)";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "#";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "% CP";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Type";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "Type 2";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
	        
            PokemonListView.Columns.Add(CreateColumn("Catch Date"));
            PokemonListView.Columns.Add(CreateColumn("Pokeball"));
            PokemonListView.Columns.Add(CreateColumn("Num Upgrades"));
            PokemonListView.Columns.Add(CreateColumn("Battles Attacked"));
            PokemonListView.Columns.Add(CreateColumn("Battles Defended"));

            PokemonListView.Columns["#"].DisplayIndex = 0;
	        
            PokemonListView.ColumnClick += new ColumnClickEventHandler(PokemonListView_ColumnClick);
            PokemonListView.ShowItemToolTips = true;
            PokemonListView.DoubleBuffered(true);
            PokemonListView.View = View.Details;

        }

        private ColumnHeader CreateColumn(string name)
        {
            var columnheader = new ColumnHeader();
            columnheader.Name = name;
            columnheader.Text = name;
            return columnheader;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Globals.RepeatUserRoute = checkBox1.Checked;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Globals.UseLureGUIClick = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Globals.UseLuckyEggGUIClick = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.UseIncenseGUIClick = true;
        }

    }

}
