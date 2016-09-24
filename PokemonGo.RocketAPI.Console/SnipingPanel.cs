using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POGOProtos.Enums;
using System.Device.Location;
using PokemonGo.RocketAPI.Logic.Utils;

namespace PokemonGo.RocketAPI.Console
{    
    public partial class SnipingPanel : UserControl
    {
        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        public SnipingPanel()
        {
            InitializeComponent();
            SnipePokemonPokeCom.Checked = Globals.SnipePokemon;
            AvoidRegionLock.Checked = Globals.AvoidRegionLock;
            int ie = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = ie;
                    checkedListBox_NotToSnipe.Items.Add(pokemon.ToString());
                    ie++;
                }
            }
            //foreach (PokemonId Id in Globals.NotToSnipe)
            //{
            //    string _id = Id.ToString();
            //    checkedListBox_NotToSnipe.SetItemChecked(pokeIDS[_id] - 1, true);
            //}
        }
        private async void SnipeMe_Click(object sender, EventArgs e)
        {
            var array = SnipeInfo.Text.Split('|');
            PokemonId idPoke = PokemonParser.ParsePokemon(array[0]);
            GeoCoordinate geocoord = new GeoCoordinate(double.Parse(array[1]), double.Parse(array[2]));
            var success = await Logic.Logic._instance.Snipe(idPoke, geocoord);
            SnipeInfo.Text = "";
        }

        private void SnipePokemonPokeCom_CheckedChanged(object sender, EventArgs e)
        {
            Globals.SnipePokemon = SnipePokemonPokeCom.Checked;
        }

        private void AvoidRegionLock_CheckedChanged(object sender, EventArgs e)
        {
            Globals.AvoidRegionLock = AvoidRegionLock.Checked;
        }

        private void SelectallNottoSnipe_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_NotToSnipe.Items.Count)
            {
                checkedListBox_NotToSnipe.SetItemChecked(i, SelectallNottoSnipe.Checked);
                i++;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Globals.NotToSnipe.Clear();
            foreach (string pokemon in checkedListBox_NotToSnipe.CheckedItems)
            {
                Globals.NotToSnipe.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
        }

        private void ForceAutoSnipe_Click(object sender, EventArgs e)
        {
            Globals.ForceAutoSnipe = true;
        }
    }
}

