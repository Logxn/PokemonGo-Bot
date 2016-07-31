using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public partial class Pokemons : Form
    {
        private static Client client;
        private static GetPlayerResponse profile;
        private static GetInventoryResponse inventory;
        private static IOrderedEnumerable<PokemonData> pokemons;
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
        public Pokemons()
        {
            InitializeComponent();
            ClientSettings = new Settings();
        }

        public static ISettings ClientSettings;

        private void Pokemons_Load(object sender, EventArgs e)
        {
            textBox2.Text = "60";
            Execute();
            this.listView1.ColumnClick += new ColumnClickEventHandler(listView1_ColumnClick);
        }

        private async void Execute()
        {
            EnabledButton(false);
            textBox1.Text = "Reloading Pokemon list.";

            client = new Client(ClientSettings);

            try
            {
                switch (ClientSettings.AuthType)
                {
                    case AuthType.Ptc:
                        await client.DoPtcLogin(ClientSettings.PtcUsername, ClientSettings.PtcPassword);
                        break;
                    case AuthType.Google:
                        await client.DoGoogleLogin();
                        break;
                }

                await client.SetServer();
                profile = await client.GetProfile();
                inventory = await client.GetInventory();
                pokemons =
                    inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.Pokemon)
                        .Where(p => p != null && p?.PokemonId > 0)
                        .OrderByDescending(key => key.Cp);
                var families = inventory.InventoryDelta.InventoryItems
                    .Select(i => i.InventoryItemData?.PokemonFamily)
                    .Where(p => p != null && (int)p?.FamilyId > 0)
                    .OrderByDescending(p => (int)p.FamilyId);

                var imageSize = 50;

                var imageList = new ImageList { ImageSize = new Size(imageSize, imageSize) };
                listView1.ShowItemToolTips = true;
                listView1.SmallImageList = imageList;

                var templates = await client.GetItemTemplates();
                var myPokemonSettings = templates.ItemTemplates.Select(i => i.PokemonSettings).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                var pokemonFamilies = myPokemonFamilies.ToArray();

                listView1.DoubleBuffered(true);
                listView1.View = View.Details;

                ColumnHeader columnheader;
                columnheader = new ColumnHeader();
                columnheader.Text = "Name";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "CP";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "IV A-D-S";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "LVL";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "Evolvable?";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "Height";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "Weight";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "Attack";
                listView1.Columns.Add(columnheader);
                columnheader = new ColumnHeader();
                columnheader.Text = "SpecialAttack";
                listView1.Columns.Add(columnheader);

                foreach (var pokemon in pokemons)
                {
                    Bitmap pokemonImage = null;
                    await Task.Run(() =>
                    {
                        pokemonImage = GetPokemonImage((int)pokemon.PokemonId);
                    });
                    imageList.Images.Add(pokemon.PokemonId.ToString(), pokemonImage);

                    listView1.LargeImageList = imageList;
                    var listViewItem = new ListViewItem();
                    listViewItem.Tag = pokemon;



                    var currentCandy = families
                        .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                        .Select(f => f.Candy)
                        .First();
                    var currIv = Math.Round(Perfect(pokemon));
                    listViewItem.SubItems.Add(string.Format("{0}", pokemon.Cp));
                    listViewItem.SubItems.Add(string.Format("{0}% {1}-{2}-{3}", currIv, pokemon.IndividualAttack, pokemon.IndividualDefense, pokemon.IndividualStamina));
                    listViewItem.SubItems.Add(string.Format("{0}", PokemonInfo.GetLevel(pokemon)));
                    listViewItem.ImageKey = pokemon.PokemonId.ToString();

                    var pokemonId2 = pokemon.PokemonId;
                    var pokemonName = pokemon.Id;

                    listViewItem.Text = string.Format("{0}", pokemon.PokemonId);
                    listViewItem.ToolTipText = "Favorite: " + pokemon.Favorite + "\nNickname: " + pokemon.Nickname;

                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                    if (settings.EvolutionIds.Count > 0 && familyCandy.Candy >= settings.CandyToEvolve)
                    {
                        listViewItem.SubItems.Add("Y (" + settings.CandyToEvolve + ")");
                        listViewItem.Checked = true;
                    }
                    else
                    {
                        if (settings.EvolutionIds.Count > 0)
                            listViewItem.SubItems.Add("N (" + familyCandy.Candy + "/" + settings.CandyToEvolve + ")");
                        else
                            listViewItem.SubItems.Add("N (" + familyCandy.Candy + "/Max)");
                    }
                    listViewItem.SubItems.Add(string.Format("{0}", Math.Round(pokemon.HeightM, 2)));
                    listViewItem.SubItems.Add(string.Format("{0}", Math.Round(pokemon.WeightKg, 2)));
                    listViewItem.SubItems.Add(string.Format("{0}", pokemon.Move1));
                    listViewItem.SubItems.Add(string.Format("{0}", pokemon.Move2));

                    listView1.Items.Add(listViewItem);
                }
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + pokemons.Count() + "/" + profile.Profile.PokeStorage;
                EnabledButton(true);

                textBox1.Text = string.Empty;
            }
            catch (TaskCanceledException e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (UriFormatException e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (ArgumentOutOfRangeException e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (ArgumentNullException e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (NullReferenceException e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (Exception e)
            {
                textBox1.Text = e.Message;
                Execute();
            }
        }

        private void EnabledButton(bool enabled)
        {
            button1.Enabled = enabled;
            button2.Enabled = enabled;
            button3.Enabled = enabled;
            btnUpgrade.Enabled = enabled;
            btnFullPowerUp.Enabled = enabled;
            checkBox1.Enabled = enabled;
            textBox2.Enabled = enabled;
            listView1.Enabled = enabled;
        }

        private static Bitmap GetPokemonImage(int pokemonId)
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";
            string location = Sprites + pokemonId + ".png";
            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);
            if (!File.Exists(location))
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png", @location);
            }
            PictureBox picbox = new PictureBox();
            picbox.Image = Image.FromFile(location);
            Bitmap bitmapRemote = (Bitmap)picbox.Image;
            return bitmapRemote;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            Execute();
        }

        public static float Perfect(PokemonData poke)
        {
            return ((float)(poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f)) * 100.0f;
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    if (listView1.SelectedItems.Count > 1)
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
            var pokemon = (PokemonData)listView1.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(Perfect(pokemon)) + "% perfect", "Are you sure you want to transfer?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await transferPokemon(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                listView1.Items.Remove(listView1.SelectedItems[0]);
                Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + listView1.Items.Count + "/" + profile.Profile.PokeStorage;
            }
            else
                MessageBox.Show(resp.Message + " transfer failed!", "Transfer Status", MessageBoxButtons.OK);
        }

        private ColumnHeader SortingColumn = null;

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ColumnHeader new_sorting_column = listView1.Columns[e.Column];
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
            listView1.ListViewItemSorter =
                new ListViewComparer(e.Column, sort_order);

            // Sort.
            listView1.Sort();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            var selectedItems = listView1.SelectedItems;
            int evolved = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            taskResponse resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await evolvePokemon((PokemonData)selectedItem.Tag);
                if (resp.Status)
                    evolved++;
                else
                    failed += resp.Message + " ";
            }

            if (failed != string.Empty)
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (evolved > 0)
            {
                listView1.Clear();
                Execute();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            var selectedItems = listView1.SelectedItems;
            int transfered = 0;
            int total = selectedItems.Count;
            string failed = string.Empty;
            taskResponse resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await transferPokemon((PokemonData)selectedItem.Tag);
                if (resp.Status)
                {
                    listView1.Items.Remove(selectedItem);
                    transfered++;
                }
                else
                    failed += resp.Message + " ";

            }

            if (failed != string.Empty)
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + listView1.Items.Count + "/" + profile.Profile.PokeStorage;
            EnabledButton(true);
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            var selectedItems = listView1.SelectedItems;
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
            }
            if (failed != string.Empty)
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (powerdup > 0)
            {
                listView1.Clear();
                Execute();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (!item.Checked)
                        item.Remove();
                }
            }
            else
            {
                checkBox2.Checked = false;
                listView1.Clear();
                Execute();
            }

        }

        private static async Task<taskResponse> evolvePokemon(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.EvolvePokemon((ulong)pokemon.Id);

                if (evolvePokemonResponse.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
            catch (TaskCanceledException) { await evolvePokemon(pokemon); }
            catch (UriFormatException) { await evolvePokemon(pokemon); }
            catch (ArgumentOutOfRangeException) { await evolvePokemon(pokemon); }
            catch (ArgumentNullException) { await evolvePokemon(pokemon); }
            catch (NullReferenceException) { await evolvePokemon(pokemon); }
            catch (Exception) { await evolvePokemon(pokemon); }
            return resp;
        }

        private static async Task<taskResponse> transferPokemon(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var transferPokemonResponse = await client.TransferPokemon(pokemon.Id);

                if (transferPokemonResponse.Status == 1)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }
            }
            catch (TaskCanceledException) { await transferPokemon(pokemon); }
            catch (UriFormatException) { await transferPokemon(pokemon); }
            catch (ArgumentOutOfRangeException) { await transferPokemon(pokemon); }
            catch (ArgumentNullException) { await transferPokemon(pokemon); }
            catch (NullReferenceException) { await transferPokemon(pokemon); }
            catch (Exception) { await transferPokemon(pokemon); }
            return resp;
        }

        private static async Task<taskResponse> PowerUp(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.PowerUp(pokemon.Id);

                if (evolvePokemonResponse.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
                {
                    resp.Status = true;
                }
                else
                {
                    resp.Message = pokemon.PokemonId.ToString();
                }

                await RandomHelper.RandomDelay(1000, 2000);
            }
            catch (TaskCanceledException) { await PowerUp(pokemon); }
            catch (UriFormatException) { await PowerUp(pokemon); }
            catch (ArgumentOutOfRangeException) { await PowerUp(pokemon); }
            catch (ArgumentNullException) { await PowerUp(pokemon); }
            catch (NullReferenceException) { await PowerUp(pokemon); }
            catch (Exception ex) { await PowerUp(pokemon); }
            return resp;
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView1.SelectedItems[0].Checked)
                contextMenuStrip1.Items[2].Visible = true;
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            contextMenuStrip1.Items[2].Visible = false;
        }

        private async void evolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)listView1.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(Perfect(pokemon)) + "% perfect", "Are you sure you want to evolve?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await evolvePokemon(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                listView1.Clear();
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " evolving failed!", "Evolve Status", MessageBoxButtons.OK);
        }

        private async void powerUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)listView1.SelectedItems[0].Tag;
            taskResponse resp = new taskResponse(false, string.Empty);

            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(Perfect(pokemon)) + "% perfect", "Are you sure you want to power it up?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                resp = await PowerUp(pokemon);
            }
            else
            {
                return;
            }
            if (resp.Status)
            {
                listView1.Clear();
                Execute();
            }
            else
                MessageBox.Show(resp.Message + " powering up failed!", "PowerUp Status", MessageBoxButtons.OK);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                int def = 0;
                int interval;
                if (int.TryParse(textBox2.Text, out interval))
                {
                    def = interval;
                }
                if (def < 30 || def > 3600)
                {
                    MessageBox.Show("Interval has to be between 30 and 3600 seconds!");
                    textBox2.Text = "60";
                    checkBox1.Checked = false;
                }
                else
                {
                    timer1.Interval = def * 1000;
                    timer1.Start();
                }
            }
            else
            {
                timer1.Stop();
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listView1.Clear();
            Execute();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private async void btnFullPowerUp_Click(object sender, EventArgs e)
        {
            EnabledButton(false);
            DialogResult result = MessageBox.Show("This process may take some time.", "Transfer status", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                var selectedItems = listView1.SelectedItems;
                int powerdup = 0;
                int total = selectedItems.Count;
                string failed = string.Empty;

                taskResponse resp = new taskResponse(false, string.Empty);
                int i = 0;
                int powerUps = 0;
                while (i == 0)
                {
                    foreach (ListViewItem selectedItem in selectedItems)
                    {
                        resp = await PowerUp((PokemonData)selectedItem.Tag);
                        if (resp.Status)
                            powerdup++;
                        else
                            failed += resp.Message + " ";
                    }
                    if (failed != string.Empty)
                    {
                        if (powerUps > 0)
                        {
                            MessageBox.Show("Pokemon succesfully powered " + powerUps + " times up.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Pokemon not powered up. Not enough Stardust or Candy.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        i = 1;
                        EnabledButton(true);
                    }
                    else
                    {
                        powerUps++;
                    }
                }
                if (powerdup > 0 && i == 1)
                {
                    listView1.Clear();
                    Execute();
                }
            }
            else
            {
                EnabledButton(true);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
    // Compares two ListView items based on a selected column.
    public class ListViewComparer : System.Collections.IComparer
    {
        private int ColumnNumber;
        private SortOrder SortOrder;

        public ListViewComparer(int column_number,
            SortOrder sort_order)
        {
            ColumnNumber = column_number;
            SortOrder = sort_order;
        }

        // Compare two ListViewItems.
        public int Compare(object object_x, object object_y)
        {
            // Get the objects as ListViewItems.
            ListViewItem item_x = object_x as ListViewItem;
            ListViewItem item_y = object_y as ListViewItem;

            // Get the corresponding sub-item values.
            string string_x;
            if (item_x.SubItems.Count <= ColumnNumber)
            {
                string_x = "";
            }
            else
            {
                string_x = item_x.SubItems[ColumnNumber].Text;
            }

            string string_y;
            if (item_y.SubItems.Count <= ColumnNumber)
            {
                string_y = "";
            }
            else
            {
                string_y = item_y.SubItems[ColumnNumber].Text;
            }

            // Compare them.
            int result;
            double double_x, double_y;
            if (double.TryParse(string_x, out double_x) &&
                double.TryParse(string_y, out double_y))
            {
                // Treat as a number.
                result = double_x.CompareTo(double_y);
            }
            else
            {
                DateTime date_x, date_y;
                if (DateTime.TryParse(string_x, out date_x) &&
                    DateTime.TryParse(string_y, out date_y))
                {
                    // Treat as a date.
                    result = date_x.CompareTo(date_y);
                }
                else
                {
                    // Treat as a string.
                    result = string_x.CompareTo(string_y);
                }
            }

            // Return the correct result depending on whether
            // we're sorting ascending or descending.
            if (SortOrder == SortOrder.Ascending)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }
}