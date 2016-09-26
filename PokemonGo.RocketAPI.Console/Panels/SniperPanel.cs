/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 3:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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
using Microsoft.Win32;
using System.Text;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of SniperPanel.
    /// </summary>
    public partial class SniperPanel : UserControl
    {
        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        
        public SniperPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();            
            LinkPokesniperCom.Links.Add(0,LinkPokesniperCom.Text.Length,"http://pokesnipers.com/");
            linkRarespawns.Links.Add(0,linkRarespawns.Text.Length,"http://www.rarespawns.be/");
            linkPokezz.Links.Add(0,linkPokezz.Text.Length,"http://pokezz.com/");            
        }
        void SelectallNottoSnipe_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_NotToSnipe.Items.Count)
            {
                checkedListBox_NotToSnipe.SetItemChecked(i, SelectallNottoSnipe.Checked);
                i++;
            }
        }
        void UpdateNotToSnipe_Click(object sender, EventArgs e)
        {
            Globals.NotToSnipe.Clear();
            foreach (string pokemon in checkedListBox_NotToSnipe.CheckedItems)
            {
                Globals.NotToSnipe.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            MessageBox.Show("This setting will only affect current session unless you update configuration on the \"Change Options\" Tab");
        }
        void ForceAutoSnipe_Click(object sender, EventArgs e)
        {
          Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Initiated Automatic Snipe Routine! We'll stop farming and start sniping ASAP!");
          Globals.ForceSnipe = true;
        }
        void SnipePokemonPokeCom_CheckedChanged(object sender, EventArgs e)
        {
          Globals.SnipePokemon = SnipePokemonPokeCom.Checked;
        }
        void AvoidRegionLock_CheckedChanged(object sender, EventArgs e)
        {
          Globals.AvoidRegionLock = AvoidRegionLock.Checked;
        }
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          var pokemonImage = PokeImgManager.GetPokemonVeryLargeImage((PokemonId)comboBox1.SelectedValue);
           PokemonImage.Image = pokemonImage;
        }
        
        void SnipeInfo_TextChanged(object sender, EventArgs e)
        {
            SplitLatLng(SnipeInfo.Text);
        }
        
        void SplitLatLng(string latlng)
        {
          try
            {
                var array = latlng.Split(',');
                var lat = double.Parse(array[0].Trim(), CultureInfo.InvariantCulture);
                var lng = double.Parse(array[1].Trim(), CultureInfo.InvariantCulture);
                ManualSnipePokemon.Location = new GeoCoordinate(lat, lng);
            }
            catch
            {
                //do nothing
            }
            if (ManualSnipePokemon.Location != null)
                SnipeMe.Enabled = true;
        }
        
        void SnipeMe_Click(object sender, EventArgs e)
        {
            SnipePoke((PokemonId)comboBox1.SelectedItem);
        }
        void SnipePoke(PokemonId id)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Manual Snipe Triggered! We'll stop farming and go catch the pokemon ASAP");
            ManualSnipePokemon.ID = id;
            SnipeInfo.Text = "";
            Globals.ForceSnipe = true;            
        }
        
        public void Execute()
        {
            SnipePokemonPokeCom.Checked = Globals.SnipePokemon;
            AvoidRegionLock.Checked = Globals.AvoidRegionLock;
            var pokemonControlSource = new System.Collections.Generic.List<PokemonId>();        
            var ie = 1; // seeing line 114 of GUI, must be same value
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = ie;                            
                    checkedListBox_NotToSnipe.Items.Add(pokemon.ToString());                            
                    ie++;
                    pokemonControlSource.Add(pokemon);
                }
            }
            comboBox1.DataSource = pokemonControlSource;
            foreach (PokemonId Id in Globals.NotToSnipe)
            {
                string _id = Id.ToString();
                try {
                    checkedListBox_NotToSnipe.SetItemChecked(pokeIDS[_id] - 1, true);
                } catch (Exception e) {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed,"[Ignoring Error] More information in log file");
                    Logger.AddLog(string.Format("Error loading checkedListBox_NotToSnipe id:{0}, pokeIDS[id]:{1}",_id,pokeIDS[_id]));
                    Logger.AddLog(e.ToString());
                }
                
            }                    
        
        }
        void btnInstall_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                try {
                    UnregisterUriScheme();
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Uninstalled");
                    timer1.Enabled = false;
                    btnInstall.Text ="Install Service";
                } catch (Exception) {
                    MessageBox.Show("Cannot uninstall service\n"+e.ToString());
                }                
            }
            else
            {
              try {
                    RegisterUriScheme(Application.ExecutablePath);
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Installed");
                    timer1.Enabled = true;
                    btnInstall.Text ="Uninstall Service";
                } catch (Exception) {
                    MessageBox.Show("Cannot install service.\n"+e.ToString());
                }
            }
            
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            try {                
                var filename = Path.GetTempPath()+"pokesniper";
                if (File.Exists(filename)){
                    var stream = new FileStream(filename,FileMode.Open);
                    var utf8 = new UTF8Encoding();
                    var reader = new BinaryReader(stream,utf8);
                    string txt =  reader.ReadString();
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Read URI: " +txt);
                    reader.Close();
                    stream.Close();
                    File.Delete(filename);
                    /* code to snipe*/
                    txt = txt.Replace("pokesniper2://","");
                    var splt = txt.Split('/');
                    SplitLatLng(splt[1]);
                    SnipePoke( (PokemonId)Enum.Parse(typeof(PokemonId), ToCapital(splt[0])));
                }
            } catch (Exception ex) {
                MessageBox.Show( ex.ToString());
                
            }
        }
        static string ToCapital(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            var txt =char.ToUpper(s[0]) + s.Substring(1).ToLower();
            
            // Replacing text for the exception pokemons that not are written in capital format
            txt = txt.Replace("mime","Mime");
            return txt;
        }

        const string URI_SCHEME = "pokesniper2";
        const string URI_KEY = "URL:pokesniper2 Protocol";

        static void RegisterUriScheme(string appPath) {
            // HKEY_CLASSES_ROOT\myscheme            
            using (RegistryKey hkcrClass = Registry.CurrentUser.CreateSubKey("Software\\Classes\\"+ URI_SCHEME)) {
                hkcrClass.SetValue(null, URI_KEY);
                hkcrClass.SetValue("URL Protocol", String.Empty, RegistryValueKind.String);

                // use the application's icon as the URI scheme icon
                using (RegistryKey defaultIcon = hkcrClass.CreateSubKey("DefaultIcon")) {
                    string iconValue = String.Format("\"{0}\",0", appPath);
                    defaultIcon.SetValue(null, iconValue);
                }

                // open the application and pass the URI to the command-line
                using (RegistryKey shell = hkcrClass.CreateSubKey("shell")) {
                    using (RegistryKey open = shell.CreateSubKey("open")) {
                        using (RegistryKey command = open.CreateSubKey("command")) {
                            string cmdValue = String.Format("\"{0}\" \"%1\"", appPath);
                            command.SetValue(null, cmdValue);
                        }
                    }
                }
            }
        }
        static void UnregisterUriScheme() {
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\"+ URI_SCHEME);
        }
        void PokesniperCom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }        
    }
}
