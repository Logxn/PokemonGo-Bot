using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public partial class Pokemons : Form
    {
        private static Client client;
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
            Execute();
        }

        private async void Execute()
        {
            EnabledButton(false);

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
                var profile = await client.GetProfile();
                var inventory = await client.GetInventory();
                var pokemons =
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
                    //listViewItem.SubItems.Add();
                    listViewItem.ImageKey = pokemon.PokemonId.ToString();

                    var pokemonId2 = pokemon.PokemonId;
                    var pokemonName = pokemon.Id;

                    listViewItem.Text = string.Format("{0}\n{1} CP", pokemon.PokemonId, pokemon.Cp);
                    listViewItem.ToolTipText = currentCandy + " Candy\n" + currIv + "% IV";


                    this.listView1.Items.Add(listViewItem);

                }
                this.Text = "Pokemon List | User: " + profile.Profile.Username + " | Pokemons: " + pokemons.Count() + "/" + profile.Profile.PokeStorage;
                EnabledButton(true);


            }
            catch (TaskCanceledException e) {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (UriFormatException e) {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (ArgumentOutOfRangeException e) {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (ArgumentNullException e) {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (NullReferenceException e) {
                textBox1.Text = e.Message;
                Execute();
            }
            catch (Exception e) {
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
                    contextMenuStrip1.Show(Cursor.Position);
                }
            }
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var pokemon = (PokemonData)listView1.SelectedItems[0].Tag;


            if (MessageBox.Show(this, pokemon.PokemonId + " with " + pokemon.Cp + " CP thats " + Math.Round(Perfect(pokemon)) + "% perfect", "Are you sure you want to transfer?", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                var transfer = await client.TransferPokemon(pokemon.Id);
            }
            listView1.Items.Remove(listView1.SelectedItems[0]);
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
            listView1.Clear();
            Execute();
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
                    transfered++;
                else
                    failed += resp.Message+" ";

            }

            if(failed != string.Empty)
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons. Failed: " + failed, "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Succesfully transfered " + transfered + "/" + total + " Pokemons.", "Transfer status", MessageBoxButtons.OK, MessageBoxIcon.Information);
            listView1.Clear();
            Execute();
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
            listView1.Clear();
            Execute();
        }

        private static async Task<taskResponse> evolvePokemon(PokemonData pokemon)
        {
            taskResponse resp = new taskResponse(false, string.Empty);
            try
            {
                var evolvePokemonResponse = await client.EvolvePokemon((ulong)pokemon.Id);

                if(evolvePokemonResponse.Result == EvolvePokemonOut.Types.EvolvePokemonStatus.PokemonEvolvedSuccess)
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
    }
}
