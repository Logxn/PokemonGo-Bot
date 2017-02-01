using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Console.PokeData;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console
{
    public partial class PokemonsPanel : UserControl
    {
        public  IOrderedEnumerable<PokemonData> pokemons;
        public GetPlayerResponse profile;
        public PlayerPanel playerPanel1;
        private static GetInventoryResponse inventory;
        private static List<AdditionalPokeData> additionalPokeData = new List<AdditionalPokeData>();
        private static ISettings BotSettings;
        private static Client client;
        private ColumnHeader SortingColumn;
        private DownloadItemTemplatesResponse templates;
        
        public PokemonsPanel()
        {
            InitializeComponent();
            BotSettings = new Settings();
            InitialzePokemonListView();
        }

        #region initialize listview
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
            PokemonListView.Columns.Add(CreateColumn("In Gym"));
            PokemonListView.Columns.Add(CreateColumn("Capture Place"));

            PokemonListView.Columns["#"].DisplayIndex = 0;
            
            PokemonListView.ColumnClick += new ColumnClickEventHandler(PokemonListView_ColumnClick);
            PokemonListView.ShowItemToolTips = true;
            PokemonListView.DoubleBuffered(true);
            PokemonListView.View = View.Details;
            createImageList();
        }

        private void createImageList(){
            const int imageSize = 50;
            var imageList = new ImageList { ImageSize = new Size(imageSize, imageSize) };
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                Bitmap pokemonImage = PokeImgManager.GetPokemonLargeImage(pokemon);
                if (pokemonImage!= null)
                    imageList.Images.Add(pokemon.ToString(), pokemonImage);
            }
            PokemonListView.SmallImageList = imageList;
            PokemonListView.LargeImageList = imageList;
        }

        private ColumnHeader CreateColumn(string name)
        {
            var columnheader = new ColumnHeader();
            columnheader.Name = name;
            columnheader.Text = name;
            return columnheader;
        }

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
        private void loadAdditionalPokeData()
        {
            const string path = "PokeData\\AdditionalPokeData.json";
            try
            {
                var jsonData = File.ReadAllText(path);
                additionalPokeData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdditionalPokeData>>(jsonData);
            }
            catch (Exception e)
            {
                var strTrace = $"Could not load additional PokeData: {e.Message}{e.StackTrace}";
                Logger.ColoredConsoleWrite(ConsoleColor.Red, strTrace, LogLevel.Error);
            }
        }

        #endregion initialize listview

        public void Execute()
        {
            EnabledButton(false, "Reloading Pokemon list.");
            try
            {
                client = Logic.Logic.objClient;
                if (client.ReadyToUse != false)
                {
                    Helpers.RandomHelper.RandomSleep(1000, 1200);
                    refreshData().Wait();

                    pokemons =
                    inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonData)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp);


                    var families = inventory.InventoryDelta.InventoryItems
                        .Select(i => i.InventoryItemData?.Candy)
                        .Where(p => p != null && (int)p?.FamilyId > 0)
                        .OrderByDescending(p => (int)p.FamilyId);

                    var myPokemonSettings = templates.ItemTemplates.Select(i => i.PokemonSettings).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                    var pokemonSettings = myPokemonSettings.ToList();
                    
                    var myPokemonFamilies = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Candy).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                    var pokemonFamilies = myPokemonFamilies.ToArray();
                    try{
                        PokemonListView.BeginUpdate();
                    }catch(Exception ex1){
                        Logger.ExceptionInfo(ex1.ToString());
                    }
                    
                    PokemonListView.Items.Clear();

                    foreach (var pokemon in pokemons)
                    {
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
                        var specSymbol ="";
                        if  (pokemon.Favorite == 1) 
                            specSymbol = "★";
                        if ((profile!=null) && (profile.PlayerData.BuddyPokemon.Id == pokemon.Id))
                            specSymbol = "☉";
                        listViewItem.Text = specSymbol + Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId);

                        listViewItem.ToolTipText = Logic.Utils.StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs,"dd/MM/yyyy HH:mm:ss");
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
                        listViewItem.SubItems.Add(Logic.Utils.StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "yyyy/MM/dd HH:mm:ss"));
                        listViewItem.SubItems.Add(pokemon.Pokeball.ToString().Replace("Item", ""));
                        listViewItem.SubItems.Add("" + pokemon.NumUpgrades);
                        listViewItem.SubItems.Add("" + pokemon.BattlesAttacked);
                        listViewItem.SubItems.Add("" + pokemon.BattlesDefended);
                        listViewItem.SubItems.Add("" + pokemon.DeployedFortId);
                        var CapturedLatlng = S2Helper.GetLatLng(pokemon.CapturedCellId);
                        listViewItem.SubItems.Add(Logic.Utils.LocationUtils.FindAddress(CapturedLatlng[0],CapturedLatlng[1]));

                        PokemonListView.Items.Add(listViewItem);
                    }
                    try{
                        PokemonListView.EndUpdate();
                    }catch(Exception ex1){
                        Logger.ExceptionInfo(ex1.ToString());
                    }
                    PokemonListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    EnabledButton(true);
                    statusTexbox.Text = string.Empty;
                    RefreshTitle();
                    if (playerPanel1!=null) {
                        playerPanel1.SetPokemons(pokemons);
                    }
                }
            }
            catch (Exception ex1)
            {
            	Logger.ExceptionInfo(ex1.ToString());
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
            checkBoxreload.Enabled = enabled;
            reloadsecondstextbox.Enabled = enabled;
            PokemonListView.Enabled = enabled;
            btnIVToNick.Enabled = enabled;
        }

        public async Task refreshData(){
            inventory = await client.Inventory.GetInventory().ConfigureAwait(false);
            templates = await client.Download.GetItemTemplates().ConfigureAwait(false);
        }


        public void RefreshTitle()
        {
            var txt ="Pokemons";
            if (Parent !=null)
            {
                txt += ": " + PokemonListView.Items.Count;
                if (profile!=null)
                {
                    txt +="/" + profile.PlayerData.MaxPokemonStorage;
                }
            }
            Parent.Text = txt;
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

        private void transferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to transfer?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = transferPokemon(pokemon).Result;
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                PokemonListView.Items.Remove(PokemonListView.SelectedItems[0]);
                RefreshTitle();
            }
            else
                MessageBox.Show(resp.Message + " transfer failed!", "Transfer Status", MessageBoxButtons.OK);
        }
        private static async Task<taskResponse> transferPokemon(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
            try
            {
                var transferPokemonResponse = await client.Inventory.TransferPokemon(pokemon.Id).ConfigureAwait(false);

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
                await transferPokemon(pokemon).ConfigureAwait(false);
            }
            return resp;
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            Execute();
        }

        private void btnEvolve_Click(object sender, EventArgs e)
        {

            EnabledButton(false, "Evolving...");
            var selectedItems = PokemonListView.SelectedItems;
            int evolved = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            var date = DateTime.Now.ToString();
            string logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string evolvelog = System.IO.Path.Combine(logPath, "EvolveLog.txt");

            var resp = new taskResponse(false, string.Empty);

            if (GlobalVars.pauseAtEvolve2)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Taking a break to evolve some pokemons!");
                GlobalVars.PauseTheWalking = true;
            }


            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = evolvePokemon((PokemonData)selectedItem.Tag).Result;

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

                if (GlobalVars.UseAnimationTimes)
                {
                    Helpers.RandomHelper.RandomSleep(30000, 35000);
                }
            }


            if (failed != string.Empty)
            {
                if (BotSettings.LogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                }
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons. Failed: " + failed, "Evolve status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
            {
                if (BotSettings.LogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons." + Environment.NewLine);
                }
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons.", "Evolve status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (evolved > 0)
            {
                Execute();
            }
            else
                EnabledButton(true);

            if (GlobalVars.pauseAtEvolve)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Evolved everything. Time to continue our journey!");
                GlobalVars.PauseTheWalking = false;
            }
        }

        private static async Task<taskResponse> evolvePokemon(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.Inventory.EvolvePokemon((ulong)pokemon.Id).ConfigureAwait(false);

                if (evolvePokemonResponse.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                Helpers.RandomHelper.RandomSleep(1000, 2000);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error evolvePokemon: " + e.Message);
                await evolvePokemon(pokemon).ConfigureAwait(false);
            }
            return resp;
        }        

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            EnabledButton(false, "Transfering...");
            var selectedItems = PokemonListView.SelectedItems;
            int transfered = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;

            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string logs = Path.Combine(logPath, "TransferLog.txt");
            string date = DateTime.Now.ToString();

            if (GlobalVars.pauseAtEvolve2)  // stop walking
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Taking a short break to transfer some pokemons!");
                GlobalVars.PauseTheWalking = true;
            }
            DialogResult dialogResult = MessageBox.Show("You clicked transfer. This can not be undone.", "Are you Sure?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {

                ReleasePokemonResponse _response = new ReleasePokemonResponse();

                List<ulong> pokemonsToTransfer = new List<ulong>();

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    var pokemon = (PokemonData)selectedItem.Tag;
                    var strPokename = Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, pokemon.PokemonId);

                    if (pokemon.DeployedFortId == "" && pokemon.Favorite == 0 && pokemon.Id != profile.PlayerData.BuddyPokemon.Id)
                    {
                        pokemonsToTransfer.Add(pokemon.Id);

                        transfered++;

                        File.AppendAllText(logs, $"[{date}] - MANUAL - Enqueuing to BULK transfer pokemon {transfered}/{total}: {Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, pokemon.PokemonId)}" + Environment.NewLine);
                        var strPerfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                        var strTransfer = $"Enqueuing to BULK transfer pokemon {transfered}/{total}: {strPokename} CP {pokemon.Cp} IV {strPerfection}";
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, strTransfer, LogLevel.Info);

                        PokemonListView.Items.Remove(selectedItem);
                    }
                    else
                    {
                        if (pokemon.DeployedFortId != "") Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is deployed in a Gym.");
                        if (pokemon.Favorite == 1) Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is a favourite pokemon.");
                        if (pokemon.Id == profile.PlayerData.BuddyPokemon.Id) Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is your Buddy.");
                        total--;
                    }
                }

                _response = client.Inventory.TransferPokemon(pokemonsToTransfer).Result;
                
                if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
                { 
                    if (BotSettings.LogTransfer)
                    {
                        File.AppendAllText(logs, $"[{date}] - MANUAL - Sucessfully Bulk transfered {transfered}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer Successful of {transfered}/{total} pokemons => {_response.CandyAwarded.ToString()} candy/ies awarded.");
                    statusTexbox.Text = $"Succesfully Bulk transfered {total} Pokemons.";
                    Helpers.RandomHelper.RandomSleep(1000, 2000);
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Something happened while transferring pokemons.");
                }

                RefreshTitle();
                client.Inventory.GetInventory(true).Wait(); // force refresh inventory

                if (GlobalVars.pauseAtEvolve)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Transferred everything. Time to continue our journey!");
                    GlobalVars.PauseTheWalking = false;
                }                
            }
            EnabledButton(true);
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            var selectedItems = PokemonListView.SelectedItems;
            int powerdup = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            var resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = PowerUp((PokemonData)selectedItem.Tag).Result;
                if (resp.Status)
                    powerdup++;
                else
                    failed += resp.Message + " ";
                 Helpers.RandomHelper.RandomSleep(1000, 3000);
            }
            if (failed != string.Empty)
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (powerdup > 0)
            {
                Execute();
            }
            else
                EnabledButton(true);
        }
        private static async Task<taskResponse> PowerUp(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.Inventory.UpgradePokemon(pokemon.Id).ConfigureAwait(false);

                if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                Helpers.RandomHelper.RandomSleep(1000, 2000);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error Powering Up: " + e.Message);
                await PowerUp(pokemon).ConfigureAwait(false);
            }
            return resp;
        }
        private void btnFullPowerUp_Click(object sender, EventArgs e)
        {
            EnabledButton(false, "Powering up...");
            DialogResult result = MessageBox.Show("This process may take some time.", "FullPowerUp status", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                var selectedItems = PokemonListView.SelectedItems;
                int poweredup = 0;
                int total = selectedItems.Count;
                string failed = string.Empty;
                int i = 0;
                int powerUps = 0;
                var resp = new taskResponse(false, string.Empty);
                while (i == 0)
                {
                    var poweruplimit = (int)numPwrUpLimit.Value;
                    foreach (ListViewItem selectedItem in selectedItems)
                    {
                        if (poweruplimit > 0)
                        {
                            if (poweredup < poweruplimit)
                            {
                                resp = PowerUp((PokemonData)selectedItem.Tag).Result;
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
                            resp = PowerUp((PokemonData)selectedItem.Tag).Result;
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
                        Helpers.RandomHelper.RandomSleep(1200, 1500);
                    }
                }
                if (poweredup > 0 && i == 1)
                {
                    Execute();
                }
            }
            else
            {
                EnabledButton(true);
            }
        }
        private void BtnIVToNickClick(object sender, EventArgs e)
        {
            EnabledButton(false, "Renaming...");
            var selectedItems = PokemonListView.SelectedItems;
            int renamed = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;

            DialogResult dialogResult = MessageBox.Show("You clicked to change nickame using IVs.\nAre you Sure?", "Confirm Dialog", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var resp = false;

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    var pokemon = (PokemonData)selectedItem.Tag;
                    pokemon.Nickname = IVsToNickname(pokemon);
                    resp = changePokemonNickname(pokemon);
                    if (resp)
                    {
                        selectedItem.ToolTipText = Logic.Utils.StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                        selectedItem.ToolTipText += "\nNickname: " + pokemon.Nickname;
                        renamed++;
                        statusTexbox.Text = "Renamig..." + renamed;
                    }
                    else
                        failed += pokemon.Nickname + " ";
                     Helpers.RandomHelper.RandomSleep(5000, 6000);
                }

                if (failed != string.Empty)
                    MessageBox.Show("Succesfully renamed " + renamed + "/" + total + " Pokemons. Failed: " + failed, "Rename status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Succesfully renamed " + renamed + "/" + total + " Pokemons.", "Rename status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            EnabledButton(true);
        }
        private static string IVsToNickname(PokemonData pokemon)
        {
            string croppedName = Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId) + " ";
            string nickname;
            //<nickname = string.Format("{0}{1}{2}{3}", pokemon.IndividualAttack.ToString("X"), pokemon.IndividualDefense.ToString("X"), pokemon.IndividualStamina.ToString("X"),(45 - pokemon.IndividualAttack- pokemon.IndividualDefense- pokemon.IndividualStamina));
            nickname = string.Format("{0}.{1}.{2}.{3}", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina);
            int lenDiff = 12 - nickname.Length;
            if (croppedName.Length > lenDiff)
                croppedName = croppedName.Substring(0, lenDiff);
            return croppedName + nickname;
        }
        private static bool changePokemonNickname(PokemonData pokemon)
        {
            var ret = false;
            try
            {
                var result = client.Inventory.NicknamePokemon(pokemon.Id, pokemon.Nickname).Result;

                if ( result.Result == NicknamePokemonResponse.Types.Result.Success)
                {
                    ret = true;
                }
                else
                {
                    Logger.Error($"Failed renaming {pokemon.Nickname}: {result.Result.ToString()}");
                }
            }
            catch (Exception e)
            {
                Logger.Error( "Error changePokemonNickname: " + e.Message);
            }
            return ret;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if ((PokemonListView.SelectedItems.Count > 0) && (PokemonListView.SelectedItems[0].Checked))
                contextMenuStrip1.Items[2].Visible = true;
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            contextMenuStrip1.Items[2].Visible = false;
        }

        private void powerUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to power it up?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = PowerUp(pokemon).Result;
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " powering up failed!", "PowerUp Status", MessageBoxButtons.OK);
        }

        private void iVsToNicknameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var resp = false;

            string promptValue = Prompt.ShowDialog(IVsToNickname(pokemon), "Confirm Nickname");

            if (promptValue != "")
            {
                pokemon.Nickname = promptValue;
                resp = changePokemonNickname(pokemon);
            }
            else
            {
                return;
            }
            if (resp)
            {
                PokemonListView.SelectedItems[0].ToolTipText = Logic.Utils.StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                PokemonListView.SelectedItems[0].ToolTipText += "\nNickname: " + pokemon.Nickname;
            }
            else
                MessageBox.Show( pokemon.Nickname + " rename failed!", "Rename Status", MessageBoxButtons.OK);
        }

        private void changeFavouritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

            string poname = Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId);
            if (MessageBox.Show(this, poname + " will be " + ((pokemon.Favorite == 1) ? "deleted from" : "added to") + " your favourites." + "\nAre you sure you want?", "Confirmation Message", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                pokemon.Favorite = (pokemon.Favorite == 1) ? 0 : 1;
                resp = changeFavourites(pokemon).Result;
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                var specSymbol ="";
                if  (pokemon.Favorite == 1) 
                    specSymbol = "★";
                if ((profile!=null) && (profile.PlayerData.BuddyPokemon.Id == pokemon.Id))
                    specSymbol = "☉";
                PokemonListView.SelectedItems[0].Text = specSymbol + Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId);
            }
            else
                MessageBox.Show(resp.Message + " change favourites failed!", "Change favourites Status", MessageBoxButtons.OK);
        }
        
        private static async Task<taskResponse> changeFavourites(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
            try
            {
                var response = await client.Inventory.SetFavoritePokemon((long)pokemon.Id, (pokemon.Favorite == 1)).ConfigureAwait(false);

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
                await changeFavourites(pokemon).ConfigureAwait(false);
            }
            return resp;
        }

        private void evolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon)) + "% perfect", "Are you sure you want to evolve?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = evolvePokemon(pokemon).Result;
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " evolving failed!", "Evolve Status", MessageBoxButtons.OK);
        }

        private void changeBuddyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            var ret = false;

            string poname = Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId);
            if (MessageBox.Show(this, poname + " will be put as your buddy." + "\nAre you sure you want?", "Confirmation Message", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ret = changeBuddy(pokemon);
            }
            else
            {
                return;
            }
            if (ret)
            {
                var specSymbol ="";
                if  (pokemon.Favorite == 1)
                    specSymbol = "★";
                if ((profile!=null) &&(profile.PlayerData.BuddyPokemon.Id == pokemon.Id))
                    specSymbol = "☉";
                PokemonListView.SelectedItems[0].Text = specSymbol + Logic.Utils.StringUtils.getPokemonNameByLanguage(BotSettings, (PokemonId)pokemon.PokemonId);
            }
            else
                MessageBox.Show("Change buddy "+poname+" failed!", "Change Buddy Status", MessageBoxButtons.OK);
        }
        
        private static bool changeBuddy(PokemonData pokemon)
        {
            var ret = false;
            try
            {
                var response = client.Inventory.SetBuddyPokemon(pokemon.Id).Result;

                if (response.Result == SetBuddyPokemonResponse.Types.Result.Success)
                {
                    ret = true;
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error ChangeFavourites: " + e.Message);
            }
            return ret;
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
            Execute();
        }
        private void freezedenshit_Tick(object sender, EventArgs e)
        {
            freezedenshit.Stop();
        }

        private void btnUseLure_Click(object sender, EventArgs e)
        {
            GlobalVars.UseLureGUIClick = true;
        }

        private void btnUseLuckyEgg_Click(object sender, EventArgs e)
        {
            GlobalVars.UseLuckyEggGUIClick = true;
        }

        private void btnUseIncense_Click(object sender, EventArgs e)
        {
            GlobalVars.UseIncenseGUIClick = true;
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
        

    }
}