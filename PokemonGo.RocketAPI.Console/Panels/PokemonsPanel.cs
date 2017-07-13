using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokeMaster.Components;
using PokeMaster.Dialogs;
using PokeMaster.Helper;
using PokeMaster.Logic.Functions;
using PokeMaster.PokeData;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using POGOProtos.Inventory.Item;
using POGOProtos.Map.Fort;
using PokemonGo.RocketAPI.Rpc;

namespace PokeMaster
{
    public partial class PokemonsPanel : UserControl
    {
        private static List<AdditionalPokeData> additionalPokeData = new List<AdditionalPokeData>();
        private static Client client;
        private ColumnHeader SortingColumn;

        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public PokemonsPanel()
        {
            InitializeComponent();
            th.Translate(this);
            InitialzePokemonListView();
            loadAdditionalPokeData();
        }

        #region initialize listview
        private void InitialzePokemonListView()
        {
            PokemonListView.Columns.Clear();
            ColumnHeader columnheader;

            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Name");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            
            PokemonListView.Columns.Add(CreateColumn(th.TS("Slash")));

            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("CP");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("IV A-D-S");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("LVL");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Evolvable?");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Height");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Weight");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("HP");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Attack");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("SpecialAttack (DPS)");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = "#";
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("% CP");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Type");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            columnheader = new ColumnHeader();
            columnheader.Name = th.TS("Type 2");
            columnheader.Text = columnheader.Name;
            PokemonListView.Columns.Add(columnheader);
            
            PokemonListView.Columns.Add(CreateColumn(th.TS("Catch Date")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Pokeball")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Num Upgrades")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Battles Attacked")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Battles Defended")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("In Gym")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Capture Place")));

            PokemonListView.Columns.Add(CreateColumn(th.TS("Gender")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Costume")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Form")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Shiny")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Buddy Candy Awarded")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("Buddy Total Km Walked")));
            PokemonListView.Columns.Add(CreateColumn(th.TS("ID")));

            PokemonListView.Columns["#"].DisplayIndex = 0;
            PokemonListView.ColumnClick += PokemonListView_ColumnClick;

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
            imageList.Images.Add(PokemonId.Magikarp+"s",PokeImgManager.GetPokemonLargeImage(PokemonId.Magikarp,"s"));
            imageList.Images.Add(PokemonId.Gyarados+"s",PokeImgManager.GetPokemonLargeImage(PokemonId.Gyarados,"s"));
            foreach (var element in "abcdefghijklmnopqrstuvwxyz") {
                imageList.Images.Add(PokemonId.Unown.ToString()+element,PokeImgManager.GetPokemonLargeImage(PokemonId.Unown,""+element));
            }
            imageList.Images.Add(PokemonId.Pikachu+ Costume.Anniversary.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Pikachu,Costume.Anniversary.ToString().ToLower()));
            imageList.Images.Add(PokemonId.Pikachu+ Costume.Holiday2016.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Pikachu,Costume.Holiday2016.ToString().ToLower()));
            imageList.Images.Add(PokemonId.Pikachu+ Costume.OneYearAnniversary.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Pikachu,Costume.OneYearAnniversary.ToString().ToLower()));
            imageList.Images.Add(PokemonId.Raichu+ Costume.Anniversary.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Raichu,Costume.Anniversary.ToString().ToLower()));
            imageList.Images.Add(PokemonId.Raichu+ Costume.Holiday2016.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Raichu,Costume.Holiday2016.ToString().ToLower()));
            imageList.Images.Add(PokemonId.Raichu+ Costume.OneYearAnniversary.ToString().ToLower(),PokeImgManager.GetPokemonLargeImage(PokemonId.Raichu,Costume.OneYearAnniversary.ToString().ToLower()));
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
                    sort_order = SortingColumn.Text.StartsWith("> ", StringComparison.Ordinal) ? SortOrder.Descending : SortOrder.Ascending;
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
            Logger.Debug("Column Index:" + e.Column);
            Logger.Debug("Sort order:" + sort_order);
            PokemonListView.ListViewItemSorter = new ListViewComparer(e.Column, sort_order);

            // Sort
            //PokemonListView.Sort();
        }

        private void loadAdditionalPokeData()
        {
            const string remotepath = "PokemonGo.RocketAPI.Console/PokeData";
            const string localpath = "Configs";
            const string filename = "AdditionalPokeData.json";
            try {
                DownloadHelper.DownloadFile(remotepath,localpath,filename);
                var localpath_with_filename = Path.Combine(localpath,filename);
                if (File.Exists(localpath_with_filename)){
                    var jsonData = File.ReadAllText(localpath_with_filename);
                    additionalPokeData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AdditionalPokeData>>(jsonData);
                }
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
        }

        #endregion initialize listview

        public void Execute()
        {
            EnabledButton(false, th.TS("Reloading Pokemon list."));
            try
            {
                client = Logic.Logic.objClient;
                if (client.ReadyToUse)
                {
                    RandomHelper.RandomSleep(1000, 1200);

                    var pokemons = client.Inventory.GetPokemons();
                    var pokemonSettings = Setout.GetPokemonSettings();
                    var pokemonFamilies = Setout.GetPokemonFamilies();
                    var profile = client.Player;

                    try{
                        PokemonListView.BeginUpdate();
                    }catch(Exception ex1){
                        Logger.ExceptionInfo(ex1.ToString());
                    }
                    
                    PokemonListView.Items.Clear();

                    foreach (var pokemon in pokemons)
                    {
                        var listViewItem = new ListViewItem();
                        listViewItem.UseItemStyleForSubItems = false;

                        listViewItem.Tag = pokemon;
                        var currentCandy = pokemonFamilies
                            .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                            .Select(f => f.Candy_)
                            .First();

                        var specSymbol ="";
                        if  (pokemon.Favorite == 1)
                            specSymbol = "★";
                        
                        if ((profile.PlayerResponse.PlayerData.BuddyPokemon!=null) && (profile.PlayerResponse.PlayerData.BuddyPokemon.Id == pokemon.Id))
                            specSymbol = "☉";
                        listViewItem.Text = specSymbol + th.TS( pokemon.PokemonId.ToString());


                        listViewItem.ToolTipText = StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs,"dd/MM/yyyy HH:mm:ss");
                        if (pokemon.Nickname != "")
                            listViewItem.ToolTipText += th.TS("\n+Nickname: {0}",pokemon.Nickname);

                        var isbad =(pokemon.IsBad)?"Yes":"No";
                        listViewItem.SubItems.Add("" + isbad);

                        listViewItem.SubItems.Add(string.Format("{0}", pokemon.Cp));

                        if (checkBox_ShortName.Checked)
                            listViewItem.SubItems.Add(string.Format("{0}% {1}{2}{3} ({4})", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack.ToString("X"), pokemon.IndividualDefense.ToString("X"), pokemon.IndividualStamina.ToString("X"), (45 - pokemon.IndividualAttack- pokemon.IndividualDefense- pokemon.IndividualStamina) ));
                        else
                            listViewItem.SubItems.Add(string.Format("{0}% {1}-{2}-{3}", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina));
                        listViewItem.SubItems.Add(string.Format("{0}", PokemonInfo.GetLevel(pokemon)));

                        # region Evolve Column
                        var settings = pokemonSettings.FirstOrDefault(x => x.PokemonId == pokemon.PokemonId);
                        var familyCandy = pokemonFamilies.FirstOrDefault(x => x.FamilyId == settings.FamilyId);
                        listViewItem.SubItems.Add("");
                        var numOfEvolves = 0;
                        String strEvolves = EvolvesToString(pokemon, settings, familyCandy, out numOfEvolves);
                        // Colour Management
                        listViewItem.SubItems[listViewItem.SubItems.Count - 1].ForeColor = Color.DarkRed;
                        if (numOfEvolves == 1)
                            listViewItem.SubItems[listViewItem.SubItems.Count - 1].ForeColor = Color.ForestGreen;
                        else if (numOfEvolves > 1)
                            listViewItem.SubItems[listViewItem.SubItems.Count - 1].ForeColor = Color.LightPink;
                        listViewItem.SubItems[listViewItem.SubItems.Count - 1].Text = $"{strEvolves} | C:{familyCandy.Candy_}";
                        # endregion Evolve Column

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
                        listViewItem.SubItems.Add(th.TS(pokemon.Pokeball.ToString().Replace("Item", "")));
                        listViewItem.SubItems.Add("" + pokemon.NumUpgrades);
                        listViewItem.SubItems.Add("" + pokemon.BattlesAttacked);
                        listViewItem.SubItems.Add("" + pokemon.BattlesDefended);
                        listViewItem.SubItems.Add("" + pokemon.DeployedFortId);
                        if (pokemon.DeployedFortId != "")
                            listViewItem.SubItems[0].BackColor = Color.Bisque;

                        var CapturedLatlng = S2Helper.GetLatLng(pokemon.CapturedCellId);
                        listViewItem.SubItems.Add(LocationUtils.FindAddress(CapturedLatlng[0],CapturedLatlng[1]));
                        listViewItem.SubItems.Add("" + pokemon.PokemonDisplay.Gender);
                        var str ="";
                        if (pokemon.PokemonDisplay.Costume != Costume.Unset)
                            str =pokemon.PokemonDisplay.Costume.ToString();
                        listViewItem.SubItems.Add("" + str);
                        var form ="";
                        if (pokemon.PokemonDisplay.Form != POGOProtos.Enums.Form.Unset)
                            form = pokemon.PokemonDisplay.Form.ToString().Replace("Unown","").Replace("ExclamationPoint","!").Replace("QuestionMark","?");
                        listViewItem.SubItems.Add("" + form);
                        
                        var shiny =(pokemon.PokemonDisplay.Shiny)?"Yes":"No";
                        listViewItem.SubItems.Add("" + shiny);
                        
                        listViewItem.SubItems.Add("" + pokemon.BuddyCandyAwarded);
                        listViewItem.SubItems.Add("" + pokemon.BuddyTotalKmWalked);
                        listViewItem.SubItems.Add("" + pokemon.Id.ToString("X"));
                        
                        var special ="";
                        if (GlobalVars.UseSpritesFolder){
                            if (pokemon.PokemonId == PokemonId.Unown)
                                special = form.ToLower();
                            else if (shiny=="Yes") 
                                special = "s";
                            else if (pokemon.PokemonDisplay.Costume != Costume.Unset){
                                special = pokemon.PokemonDisplay.Costume.ToString().ToLower();
                            }
                        }
                        
                        listViewItem.ImageKey = pokemon.PokemonId + special;

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
            btnFullPowerUp.Enabled = enabled;
            checkBoxreload.Enabled = enabled;
            reloadsecondstextbox.Enabled = enabled;
            PokemonListView.Enabled = enabled;
            btnIVToNick.Enabled = enabled;
        }


        public void RefreshTitle()
        {
            var txt =th.TS("Pokemons");
            if (Parent != null) {
                txt += ": " + PokemonListView.Items.Count;
                var profile = client.Player;
                if (profile != null) {
                    var eggs = client.Inventory.GetEggs();
                    txt += "/" + (profile.PlayerResponse.PlayerData.MaxPokemonStorage - eggs.Count());
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
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void transferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            transferSelectedPokemons();
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
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            string evolvelog = Path.Combine(logPath, "EvolveLog.txt");
            int gotXP = 0;
            
            var resp = new EvolvePokemonResponse();

            if (GlobalVars.pauseAtEvolve2)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Taking a break to evolve some pokemons!");
                GlobalVars.PauseTheWalking = true;
            }
            
            var evolveDialog = new EvolvingDialog();
            foreach (ListViewItem selectedItem in selectedItems)
            {
                var pokemoninfo = (PokemonData)selectedItem.Tag;
                var item = Inventory.GeteNeededItemToEvolve(pokemoninfo.PokemonId);

                if (item != ItemId.ItemUnknown  && client.Inventory.GetItemAmountByType(item) < 1){
                    if ( pokemoninfo.PokemonId == PokemonId.Poliwhirl
                        || pokemoninfo.PokemonId == PokemonId.Gloom
                        || pokemoninfo.PokemonId == PokemonId.Slowpoke
                       )
                        item = ItemId.ItemUnknown; // try to evolve without items
                    else
                        continue; // go to next pokemon
                }
                resp = client.Inventory.EvolvePokemon(pokemoninfo.Id, item);
                if (resp.Result == EvolvePokemonResponse.Types.Result.Success)
                {
                    evolved++;
                    statusTexbox.Text = "Evolving..." + evolved;
                    if (GlobalVars.UseAnimationTimes)
                    {
                        evolveDialog.Show();
                        evolveDialog.RunAnimation(pokemoninfo.PokemonId,resp.EvolvedPokemonData.PokemonId);
                        evolveDialog.Hide();
                    }

                    var name = pokemoninfo.PokemonId;
                    var getPokemonName = StringUtils.getPokemonNameByLanguage(pokemoninfo.PokemonId);
                    var cp = pokemoninfo.Cp;
                    var calcPerf = PokemonInfo.CalculatePokemonPerfection(pokemoninfo).ToString("0.00");
                    var getEvolvedName = th.TS((resp.EvolvedPokemonData.PokemonId).ToString());
                    var getEvolvedCP = resp.EvolvedPokemonData.Cp;
                    gotXP = gotXP + resp.ExperienceAwarded;
                    var xpreward = resp.ExperienceAwarded.ToString("N0");
                    Logger.Info($"Evolved Pokemon: {getPokemonName} | CP {cp} | Perfection {calcPerf}% | => to {getEvolvedName} | CP: {getEvolvedCP} | XP Reward: {xpreward} XP");
                    Logger.Info($"Waiting a few seconds... dont worry!");
                }
                else
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Failed to evolve {pokemoninfo.PokemonId}. EvolvePokemonOutProto.Result was {resp.Result}");
                    failed += " {pokemoninfo.PokemonId} ";
                }

            }

            PokemonListView.Refresh();

            if (!string.IsNullOrEmpty(failed) )
            {
                if (GlobalVars.LogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                }
                MessageBox.Show(th.TS("Succesfully evolved {0}/{1} Pokemons. Failed: {2}",evolved,total,failed), th.TS("Evolve status"), MessageBoxButtons.OK, MessageBoxIcon.Information,MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
            else
            {
                if (GlobalVars.LogEvolve)
                {
                    File.AppendAllText(evolvelog, $"[{date}] - MANUAL - Sucessfully evolved {evolved}/{total} Pokemons." + Environment.NewLine);
                }
                MessageBox.Show(th.TS("Succesfully evolved {0}/{1} Pokemons.",evolved,total), th.TS("Evolve status"), MessageBoxButtons.OK, MessageBoxIcon.Information,MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000);
            }
            var gotxpno=gotXP.ToString("N0");
            Logger.Info($"Evolved {evolved} Pokemons. We have got {gotxpno} XP.");

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

        private void transferSelectedPokemons()
        {
            try {
                EnabledButton(false, th.TS("Transfering..."));
                var selectedItems = PokemonListView.SelectedItems;
                int transfered = 0;
                int total = selectedItems.Count;
                string failed = string.Empty;
                
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                string logs = Path.Combine(logPath, "TransferLog.txt");
                string date = DateTime.Now.ToString();
                
                if (GlobalVars.pauseAtEvolve2)  // stop walking
                {
                    Logger.Info("Taking a short break to transfer some pokemons!");
                    GlobalVars.PauseTheWalking = true;
                }

                DialogResult dialogResult = MessageBox.Show(th.TS("You're going to transfer pokemons. This can not be reversed."), th.TS("Are you Sure?"), MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    
                    var pokemonsToTransfer = new List<ulong>();
                    
                    var profile = client.Player;
                    
                    foreach (ListViewItem selectedItem in selectedItems)
                    {
                        var pokemon = (PokemonData)selectedItem.Tag;
                        var strPokename = th.TS( pokemon.PokemonId.ToString());
                        
                        if (pokemon.DeployedFortId == "" && pokemon.Favorite == 0  )
                        {

                            if ( profile.PlayerResponse.PlayerData.BuddyPokemon != null && pokemon.Id == profile.PlayerResponse.PlayerData.BuddyPokemon.Id)
                                continue;

                            pokemonsToTransfer.Add(pokemon.Id);

                            transfered++;

                            File.AppendAllText(logs, $"[{date}] - MANUAL - Enqueuing to BULK transfer pokemon {transfered}/{total}: { pokemon.PokemonId}" + Environment.NewLine);
                            var strPerfection = PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0.00");
                            var strTransfer = $"Enqueuing to BULK transfer pokemon {transfered}/{total}: {strPokename} CP {pokemon.Cp} IV {strPerfection}";
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, strTransfer, LogLevel.Info);
                            
                            PokemonListView.Items.Remove(selectedItem);
                        }
                        else
                        {
                            if (pokemon.DeployedFortId != "") Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is deployed in a Gym.");
                            if (pokemon.Favorite == 1) Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is a favourite pokemon.");
                            if (pokemon.Id == profile.PlayerResponse.PlayerData.BuddyPokemon.Id) Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"Impossible to transfer {strPokename} because it is your Buddy.");
                            total--;
                        }
                    }
                    if (pokemonsToTransfer.Any()){
                        var _response = client.Inventory.TransferPokemons(pokemonsToTransfer);
                        
                        if (_response.Result == ReleasePokemonResponse.Types.Result.Success)
                        {
                            if (GlobalVars.LogTransfer)
                            {
                                File.AppendAllText(logs, $"[{date}] - MANUAL - Sucessfully Bulk transfered {transfered}/{total} Pokemons. Failed: {failed}" + Environment.NewLine);
                            }
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Transfer Successful of {transfered}/{total} pokemons => {_response.CandyAwarded.ToString()} candy/ies awarded.");
                            statusTexbox.Text = $"Succesfully Bulk transfered {total} Pokemons.";
                            RandomHelper.RandomSleep(1000, 2000);
                        }
                        else
                        {
                            Logger.Error("Something happened while transferring pokemons.");
                        }
                        
                        Execute();
                    }
                    
                    if (GlobalVars.pauseAtEvolve)
                    {
                        Logger.Info("Transferred everything. Time to continue our journey!");
                        GlobalVars.PauseTheWalking = false;
                    }
                }
                EnabledButton(true);
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }

        }
        private void btnTransfer_Click(object sender, EventArgs e)
        {
            transferSelectedPokemons();
        }

        private static bool PowerUp(PokemonData pokemon)
        {
            var ret = false;
            try
            {
                var evolvePokemonResponse = client.Inventory.UpgradePokemon(pokemon.Id);

                if (evolvePokemonResponse.Result == UpgradePokemonResponse.Types.Result.Success)
                {
                    ret  = true;
                } else {
                    Logger.Warning(evolvePokemonResponse.Result.ToString());
                }
                RandomHelper.RandomSleep(1000, 2000);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error Powering Up: " + e.Message);
            }
            return ret;
        }

        private void btnFullPowerUp_Click(object sender, EventArgs e)
        {
            EnabledButton(false, "Powering up...");
            DialogResult result = MessageBox.Show(th.TS("This process may take some time."), th.TS("PowerUp status"), MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                string failed = string.Empty;
                var poweruplimit = (int)numPwrUpLimit.Value;
                var atLeast1PowerUp = false;
                foreach (ListViewItem selectedItem in PokemonListView.SelectedItems)
                    for (var i = 1; i<=poweruplimit;i++)
                    {
                        if (!PowerUp( (PokemonData) selectedItem.Tag) )
                            break; // goes to next selected pokemon
                        atLeast1PowerUp = true;
                    }
                if (atLeast1PowerUp)
                    Execute();
            }
            EnabledButton(true);
        }
        private void BtnIVToNickClick(object sender, EventArgs e)
        {
            EnabledButton(false, th.TS("Renaming..."));
            var selectedItems = PokemonListView.SelectedItems;
            int renamed = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;

            DialogResult dialogResult = MessageBox.Show(th.TS("You clicked to change nickame using IVs.\nAre you Sure?"), th.TS("Confirm Dialog"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var resp = false;

                foreach (ListViewItem selectedItem in selectedItems)
                {
                    var pokemon = (PokemonData)selectedItem.Tag;
                    pokemon.Nickname = IVsToNickname(pokemon, checkBox_ShortName.Checked);
                    resp = changePokemonNickname(pokemon);
                    if (resp)
                    {
                        selectedItem.ToolTipText = Logic.Utils.StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                        selectedItem.ToolTipText += th.TS("\nNickname: {0}", pokemon.Nickname);
                        renamed++;
                        statusTexbox.Text = th.TS("Renamig...") + renamed;
                    }
                    else
                        failed += pokemon.Nickname + " ";
                    RandomHelper.RandomSleep(3000);
                }

                if (failed != string.Empty)
                    MessageBox.Show(th.TS("Succesfully renamed {0}/{1} Pokemons. Failed: {2}", renamed,total,failed), th.TS("Rename status"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(th.TS("Succesfully renamed {0}/{1} Pokemons.",renamed,total), th.TS("Rename status"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            EnabledButton(true);
        }
        private static string IVsToNickname(PokemonData pokemon, bool useShortFormat)
        {
            string croppedName = pokemon.PokemonId + " ";
            string nickname;
            nickname = useShortFormat ?
                string.Format("{0}{1}{2}{3}", pokemon.IndividualAttack.ToString("X"), pokemon.IndividualDefense.ToString("X"), pokemon.IndividualStamina.ToString("X"), (45 - pokemon.IndividualAttack - pokemon.IndividualDefense - pokemon.IndividualStamina))
                : string.Format("{0}.{1}.{2}.{3}", PokemonInfo.CalculatePokemonPerfection(pokemon).ToString("0"), pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina);
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
                var result = client.Inventory.NicknamePokemon(pokemon.Id, pokemon.Nickname);

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
            contextMenuStrip1.Items[2].Visible |= (PokemonListView.SelectedItems.Count > 0
                                                   && PokemonListView.SelectedItems[0].Checked);
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            contextMenuStrip1.Items[2].Visible = false;
        }

        private void powerUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;
            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;
            if (MessageBox.Show(this, th.TS( " {0} with {1} CP thats {2} % perfect",pokemon.PokemonId,pokemon.Cp,Math.Round(PokemonInfo.CalculatePokemonPerfection(pokemon))), th.TS("Are you sure you want to power it up?"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                if ( PowerUp(pokemon))
                    Execute();
        }

        private void iVsToNicknameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;

            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;

            string promptValue = Prompt.ShowDialog(IVsToNickname(pokemon ,checkBox_ShortName.Checked), th.TS("Confirm Nickname"));

            if (promptValue != "")
            {
                pokemon.Nickname = promptValue;
                if (changePokemonNickname(pokemon))
                {
                    PokemonListView.SelectedItems[0].ToolTipText = StringUtils.ConvertTimeMSinString(pokemon.CreationTimeMs, "dd/MM/yyyy HH:mm:ss");
                    PokemonListView.SelectedItems[0].ToolTipText += th.TS("\nNickname: {0}",pokemon.Nickname);
                }
                else
                    MessageBox.Show( th.TS("{0} rename failed!",pokemon.Nickname), th.TS("Rename Status"), MessageBoxButtons.OK);
            }
        }

        private void changeFavouritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;
            var profile = client.Player;
            foreach (ListViewItem element in PokemonListView.SelectedItems) {
                var pokemon = element.Tag as PokemonData;
                string poname = th.TS(pokemon.PokemonId.ToString());
                if (MessageBox.Show(this, th.TS("{0} will be ",poname) + ((pokemon.Favorite == 1) ? th.TS("deleted from") : th.TS("added to")) + th.TS(" your favourites.\nAre you sure you want?"), th.TS("Confirmation Message"), MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    pokemon.Favorite = (pokemon.Favorite == 1) ? 0 : 1;
                    if ( changeFavourites(pokemon) ){
                        var specSymbol ="";
                        if  (pokemon.Favorite == 1)
                            specSymbol = "★";
                        if ( profile.PlayerResponse.PlayerData.BuddyPokemon!=null && profile.PlayerResponse.PlayerData.BuddyPokemon.Id == pokemon.Id)
                            specSymbol = "☉";
                        PokemonListView.SelectedItems[0].Text = specSymbol +  pokemon.PokemonId;
                    }else
                        MessageBox.Show(th.TS("{0} change favourites failed!",poname), th.TS("Change favourites Status"), MessageBoxButtons.OK);
                }
            }
        }
        
        private bool changeFavourites(PokemonData pokemon)
        {
            var resp = false;
            try
            {
                var response =  client.Inventory.SetFavoritePokemon( (long) pokemon.Id, (pokemon.Favorite == 1));
                resp = (response.Result == SetFavoritePokemonResponse.Types.Result.Success);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error ChangeFavourites: " + e.Message);
            }
            return resp;
        }

        private void evolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnEvolve_Click(sender, e);
        }

        private void changeBuddyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;

            var pokemon = (PokemonData)PokemonListView.SelectedItems[0].Tag;

            string poname = th.TS(pokemon.PokemonId.ToString());
            if (MessageBox.Show(this, th.TS("{0} will be put as your buddy.",poname) + th.TS("\nAre you sure you want?"), th.TS("Confirmation Message"), MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (changeBuddy(pokemon))
                {
                    client.Player.PlayerResponse.PlayerData.BuddyPokemon.Id = pokemon.Id;
                    PokemonListView.SelectedItems[0].Text = "☉" + pokemon.PokemonId;
                }
                else
                    MessageBox.Show(th.TS("Change buddy {0} failed!",poname), th.TS("Change Buddy Status"), MessageBoxButtons.OK);
            }
        }
        
        private static bool changeBuddy(PokemonData pokemon)
        {
            var ret = false;
            try
            {
                var response = client.Inventory.SetBuddyPokemon(pokemon.Id);

                ret = (response.Result == SetBuddyPokemonResponse.Types.Result.Success);
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error SetBuddyPokemon: " + e.Message);
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

        void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnReload_Click(sender,e);
        }

        void cureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;
            var dialog = new ItemSelect();
            if (dialog.ShowDialog() == DialogResult.OK) {
                var selectedItem = dialog.selected;
                var itemsCount = client.Inventory.GetItemData(selectedItem.ItemId).Count;
                var index = 0;
                foreach (ListViewItem element in PokemonListView.SelectedItems) {
                    var selectedPokemon = (PokemonData) element.Tag;
                    if (selectedItem.ItemId == ItemId.ItemRevive || selectedItem.ItemId == ItemId.ItemMaxRevive)
                    {
                        if (selectedPokemon.Stamina > 0)
                            continue;
                        var res = client.Inventory.UseItemRevive(selectedItem.ItemId,selectedPokemon.Id);
                        if (res.Result == UseItemReviveResponse.Types.Result.Success)
                            MessageBox.Show(th.TS("{0} Revived sucefully",selectedPokemon.PokemonId.ToString()));
                        else
                            Logger.Error("Error: "+ res);
                    }
                    else{
                        if (selectedPokemon.Stamina >= selectedPokemon.StaminaMax)
                            continue;
                        var res = client.Inventory.UseItemPotion(selectedItem.ItemId,selectedPokemon.Id);
                        if (res.Result == UseItemPotionResponse.Types.Result.Success)
                            MessageBox.Show(th.TS("{0} Cured sucefully",selectedPokemon.PokemonId.ToString()));
                        else
                            Logger.Error("Error: "+ res);
                    }
                    index ++;
                    if ( itemsCount <  index ){
                        MessageBox.Show(th.TS("You haven`t got enough Items for all selected pokemons\nCured {0} Pokemons.",index));
                        return;
                    }
                }
            }
        }

        void gymInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PokemonListView.SelectedItems.Count < 1)
                return;
            var selectedPokemon = (PokemonData) PokemonListView.SelectedItems[0].Tag;
            if (selectedPokemon.DeployedFortId=="")
                return;

            var forts = client.Map.GetMapObjects().Result;
            var pokeGym = forts.MapCells.SelectMany(i => i.Forts)
                .FirstOrDefault(i => i.Id == selectedPokemon.DeployedFortId );

            string message ="";
            if (pokeGym == null){
                message = th.TS("Gym is not in range.\nID: ") + selectedPokemon.DeployedFortId;
            }else{
                var gymDetails = client.Fort.GetGymDetails(pokeGym.Id, pokeGym.Latitude, pokeGym.Longitude);
                message = string.Format("{0}\n{1}, {2}\n{3}\nID: {4}", LocationUtils.FindAddress(pokeGym.Latitude, pokeGym.Longitude), pokeGym.Latitude, pokeGym.Longitude, gymDetails.Name ,  pokeGym.Id);
                Logic.Logic.Instance.infoObservable.PushUpdatePokeGym(pokeGym);
            }
            MessageBox.Show(message);
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
        

        string EvolvesToString(PokemonData pokemon, POGOProtos.Settings.Master.PokemonSettings settings, POGOProtos.Inventory.Candy familyCandy, out int numOfEvolves)
        {
            numOfEvolves = 0;
            if (settings == null || settings.EvolutionBranch.Count<1)
                return "N";

            var strEvolves = "";
            var separator = "";
            var item = Inventory.GeteNeededItemToEvolve(pokemon.PokemonId);
            var amountItems =  -1 ;
            if (item != ItemId.ItemUnknown ){
                var itemData = client.Inventory.GetItemData(item);
                if (itemData!=null)
                    amountItems = itemData.Count;
            }
            var i = 0;
            foreach (var element in settings.EvolutionBranch) {
                var canEvolve = "N";
                var stone ="";
                if ( familyCandy!=null && familyCandy.Candy_ >= element.CandyCost){
                    if (amountItems != 0){
                        canEvolve = "Y";
                        numOfEvolves ++;
                    }
                }
                if (i > 0){
                    separator = " | ";
                }
                if (item != ItemId.ItemUnknown ){
                    stone = ","+ settings.EvolutionPips;
                }
                strEvolves = strEvolves+ $"{separator}{canEvolve} ({element.CandyCost}{stone})";
                i++;
            }
            return strEvolves;
        }


    }
}