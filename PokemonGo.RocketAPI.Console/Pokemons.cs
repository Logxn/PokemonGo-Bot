﻿namespace PokemonGo.RocketAPI.Console
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Helpers;

    public partial class Pokemons : Form
    {
        public static ISettings ClientSettings;
        private static Client client;
        private static GetInventoryResponse inventory;
        private static IOrderedEnumerable<PokemonData> pokemons;
        private static GetPlayerResponse profile;

        public Pokemons()
        {
            this.InitializeComponent();
            ClientSettings = new Settings();
        }

        public static float Perfect(PokemonData poke)
        {
            return (poke.IndividualAttack + poke.IndividualDefense + poke.IndividualStamina) / (3.0f * 15.0f) * 100.0f;
        }

        private static async Task<taskResponse> evolvePokemon(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.EvolvePokemon(pokemon.Id);

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
            catch (TaskCanceledException)
            {
                await evolvePokemon(pokemon);
            }
            catch (UriFormatException)
            {
                await evolvePokemon(pokemon);
            }
            catch (ArgumentOutOfRangeException)
            {
                await evolvePokemon(pokemon);
            }
            catch (ArgumentNullException)
            {
                await evolvePokemon(pokemon);
            }
            catch (NullReferenceException)
            {
                await evolvePokemon(pokemon);
            }
            catch (Exception)
            {
                await evolvePokemon(pokemon);
            }

            return resp;
        }

        private static Bitmap GetPokemonImage(int pokemonId)
        {
            var Sprites = AppDomain.CurrentDomain.BaseDirectory + "Sprites\\";
            var location = Sprites + pokemonId + ".png";
            if (!Directory.Exists(Sprites))
                Directory.CreateDirectory(Sprites);
            if (!File.Exists(location))
            {
                var wc = new WebClient();
                wc.DownloadFile("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png", location);
            }

            var picbox = new PictureBox();
            picbox.Image = Image.FromFile(location);
            var bitmapRemote = (Bitmap) picbox.Image;
            return bitmapRemote;
        }

        private static async Task<taskResponse> PowerUp(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
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
            catch (TaskCanceledException)
            {
                await PowerUp(pokemon);
            }
            catch (UriFormatException)
            {
                await PowerUp(pokemon);
            }
            catch (ArgumentOutOfRangeException)
            {
                await PowerUp(pokemon);
            }
            catch (ArgumentNullException)
            {
                await PowerUp(pokemon);
            }
            catch (NullReferenceException)
            {
                await PowerUp(pokemon);
            }
            catch (Exception ex)
            {
                await PowerUp(pokemon);
            }

            return resp;
        }

        private static async Task<taskResponse> transferPokemon(PokemonData pokemon)
        {
            var resp = new taskResponse(false, string.Empty);
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
            catch (TaskCanceledException)
            {
                await transferPokemon(pokemon);
            }
            catch (UriFormatException)
            {
                await transferPokemon(pokemon);
            }
            catch (ArgumentOutOfRangeException)
            {
                await transferPokemon(pokemon);
            }
            catch (ArgumentNullException)
            {
                await transferPokemon(pokemon);
            }
            catch (NullReferenceException)
            {
                await transferPokemon(pokemon);
            }
            catch (Exception)
            {
                await transferPokemon(pokemon);
            }

            return resp;
        }

        private async void btnUpgrade_Click(object sender, EventArgs e)
        {
            this.EnabledButton(false);
            var selectedItems = this.listView1.SelectedItems;
            var powerdup = 0;
            var total = selectedItems.Count;
            var failed = string.Empty;
            var resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await PowerUp((PokemonData) selectedItem.Tag);
                if (resp.Status)
                    powerdup++;
                else
                    failed += resp.Message + " ";
            }

            if (failed != string.Empty)
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully powered up " + powerdup + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.listView1.Clear();
            this.Execute();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listView1.Clear();
            this.Execute();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            this.EnabledButton(false);
            var selectedItems = this.listView1.SelectedItems;
            var evolved = 0;
            var total = selectedItems.Count;
            var failed = string.Empty;
            var resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await evolvePokemon((PokemonData) selectedItem.Tag);
                if (resp.Status)
                    evolved++;
                else
                    failed += resp.Message + " ";
            }

            if (failed != string.Empty)
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully evolved " + evolved + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.listView1.Clear();
            this.Execute();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            this.EnabledButton(false);
            var selectedItems = this.listView1.SelectedItems;
            var transfered = 0;
            var total = selectedItems.Count;
            var failed = string.Empty;
            var resp = new taskResponse(false, string.Empty);

            foreach (ListViewItem selectedItem in selectedItems)
            {
                resp = await transferPokemon((PokemonData) selectedItem.Tag);
                if (resp.Status)
                {
                    this.listView1.Items.Remove(selectedItem);
                    transfered++;
                }
                else
                    failed += resp.Message + " ";
            }

            if (failed != string.Empty)
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + this.listView1.Items.Count + "/" + profile.Profile.PokeStorage;
            this.EnabledButton(true);

            // listView1.Clear();
            // Execute();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked)
            {
                var def = 0;
                int interval;
                if (int.TryParse(this.textBox2.Text, out interval))
                {
                    def = interval;
                }

                if (def < 30 || def > 3600)
                {
                    MessageBox.Show("Interval has to be between 30 and 3600 seconds!");
                    this.textBox2.Text = "60";
                    this.checkBox1.Checked = false;
                }
                else
                {
                    this.timer1.Interval = def * 1000;
                    this.timer1.Start();
                }
            }
            else
            {
                this.timer1.Stop();
            }
        }

        private void contextMenuStrip1_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            this.contextMenuStrip1.Items[2].Visible = false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listView1.SelectedItems[0].Checked)
                this.contextMenuStrip1.Items[2].Visible = true;
        }

        private void EnabledButton(bool enabled)
        {
            this.button1.Enabled = enabled;
            this.button2.Enabled = enabled;
            this.button3.Enabled = enabled;
            this.btnUpgrade.Enabled = enabled;
            this.checkBox1.Enabled = enabled;
            this.textBox2.Enabled = enabled;
        }

        private async void evolveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData) this.listView1.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

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
                this.listView1.Clear();
                this.Execute();
            }
            else
                MessageBox.Show(resp.Message + " evolving failed!", "Evolve Status", MessageBoxButtons.OK);
        }

        private async void Execute()
        {
            this.EnabledButton(false);
            this.textBox1.Text = "Reloading Pokemon list.";

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
                pokemons = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.Pokemon).Where(p => p != null && p?.PokemonId > 0).OrderByDescending(key => key.Cp);
                var families = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily).Where(p => p != null && (int) p?.FamilyId > 0).OrderByDescending(p => (int) p.FamilyId);

                var imageSize = 50;

                var imageList = new ImageList
                                {
                                    ImageSize = new Size(imageSize, imageSize)
                                };
                this.listView1.ShowItemToolTips = true;

                var templates = await client.GetItemTemplates();
                var myPokemonSettings = templates.ItemTemplates.Select(i => i.PokemonSettings).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                var pokemonSettings = myPokemonSettings.ToList();

                var myPokemonFamilies = inventory.InventoryDelta.InventoryItems.Select(i => i.InventoryItemData?.PokemonFamily).Where(p => p != null && p?.FamilyId != PokemonFamilyId.FamilyUnset);
                var pokemonFamilies = myPokemonFamilies.ToArray();

                this.listView1.DoubleBuffered(true);
                foreach (var pokemon in pokemons)
                {
                    Bitmap pokemonImage = null;
                    await Task.Run(() =>
                    {
                        pokemonImage = GetPokemonImage((int) pokemon.PokemonId);
                    });
                    imageList.Images.Add(pokemon.PokemonId.ToString(), pokemonImage);

                    this.listView1.LargeImageList = imageList;
                    var listViewItem = new ListViewItem();
                    listViewItem.Tag = pokemon;

                    var currentCandy = families.Where(i => (int) i.FamilyId <= (int) pokemon.PokemonId).Select(f => f.Candy).First();
                    var currIv = Math.Round(Perfect(pokemon));

                    // listViewItem.SubItems.Add();
                    listViewItem.ImageKey = pokemon.PokemonId.ToString();

                    var pokemonId2 = pokemon.PokemonId;
                    var pokemonName = pokemon.Id;

                    listViewItem.Text = string.Format("{0}\n{1} CP", pokemon.PokemonId, pokemon.Cp);
                    listViewItem.ToolTipText = currentCandy + " Candy\n" + currIv + "% IV";

                    var settings = pokemonSettings.Single(x => x.PokemonId == pokemon.PokemonId);
                    var familyCandy = pokemonFamilies.Single(x => settings.FamilyId == x.FamilyId);

                    if (settings.EvolutionIds.Count > 0 && familyCandy.Candy > settings.CandyToEvolve)
                        listViewItem.Checked = true;

                    this.listView1.Items.Add(listViewItem);
                }

                this.Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + pokemons.Count() + "/" + profile.Profile.PokeStorage;
                this.EnabledButton(true);

                this.textBox1.Text = string.Empty;
            }
            catch (TaskCanceledException e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
            catch (UriFormatException e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
            catch (ArgumentOutOfRangeException e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
            catch (ArgumentNullException e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
            catch (NullReferenceException e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
            catch (Exception e)
            {
                this.textBox1.Text = e.Message;
                this.Execute();
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this.listView1.FocusedItem.Bounds.Contains(e.Location))
                {
                    if (this.listView1.SelectedItems.Count > 1)
                    {
                        MessageBox.Show("You can only select 1 item for quick action!", "Selection to large", MessageBoxButtons.OK);
                        return;
                    }

                    this.contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private void Pokemons_Load(object sender, EventArgs e)
        {
            this.textBox2.Text = "60";
            this.Execute();
        }

        private async void powerUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData) this.listView1.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

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
                this.listView1.Clear();
                this.Execute();
            }
            else
                MessageBox.Show(resp.Message + " powering up failed!", "PowerUp Status", MessageBoxButtons.OK);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Execute();
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData) this.listView1.SelectedItems[0].Tag;
            var resp = new taskResponse(false, string.Empty);

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
                this.listView1.Items.Remove(this.listView1.SelectedItems[0]);
                this.Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + this.listView1.Items.Count + "/" + profile.Profile.PokeStorage;
            }
            else
                MessageBox.Show(resp.Message + " transfer failed!", "Transfer Status", MessageBoxButtons.OK);
        }

        public class taskResponse
        {
            public taskResponse()
            {
            }

            public taskResponse(bool status, string message)
            {
                this.Status = status;
                this.Message = message;
            }

            public string Message
            {
                get;
                set;
            }

            public bool Status
            {
                get;
                set;
            }
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
}