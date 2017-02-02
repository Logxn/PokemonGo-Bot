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
using PokemonGo.RocketAPI.Logic.Shared;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of SniperPanel.
    /// </summary>
    public partial class SniperPanel : UserControl
    {
        private static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        public WebBrowser webBrowser = null;
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        private static Components.HRefLink[] links = {
         new Components.HRefLink("pokedexs.com","https://pokedexs.com"),
         new Components.HRefLink("pokegosnipers.com","http://pokegosnipers.com"),
         new Components.HRefLink("pokezz.com","http://pokezz.com"),
         new Components.HRefLink("pokewatchers.com","http://pokewatchers.com"),
         new Components.HRefLink("mypogosnipers.com","http://www.mypogosnipers.com"),
         new Components.HRefLink("snipe.necrobot2.com","http://snipe.necrobot2.com"),
         new Components.HRefLink("pokesniper.org","http://pokesniper.org/"),
         new Components.HRefLink("iv100.top","http://iv100.top/")
        };

        public SniperPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            IntializeComboLinks();
            th.Translate(this);
        }

        void IntializeComboLinks()
        {
            comboBoxLinks.DataSource = links;
            comboBoxLinks.DisplayMember= "Text";
            comboBoxLinks.SelectedIndex = 0;
        }

        public void AddButtonClick(System.EventHandler evh){
        	buttonGo.Click += evh;
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
            GlobalVars.NotToSnipe.Clear();
            foreach (string pokemon in checkedListBox_NotToSnipe.CheckedItems)
            {
                GlobalVars.NotToSnipe.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            MessageBox.Show("This setting will only affect current session unless you update configuration on the \"Change Options\" Tab");
        }
        void ForceAutoSnipe_Click(object sender, EventArgs e)
        {
          Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Initiated Automatic Snipe Routine! We'll stop farming and start sniping ASAP!");
          GlobalVars.ForceSnipe = true;
        }
        void SnipePokemonPokeCom_CheckedChanged(object sender, EventArgs e)
        {
          GlobalVars.SnipePokemon = SnipePokemonPokeCom.Checked;
        }
        void AvoidRegionLock_CheckedChanged(object sender, EventArgs e)
        {
          GlobalVars.AvoidRegionLock = AvoidRegionLock.Checked;
        }
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
          var pokemonImage = PokeImgManager.GetPokemonVeryLargeImage((PokemonId)comboBox1.SelectedValue);
           PokemonImage.Image = pokemonImage;
        }

        private GeoCoordinate splLatLngResult;
        void SnipeInfo_TextChanged(object sender, EventArgs e)
        {
            splLatLngResult = SplitLatLng(SnipeInfo.Text);
            SnipeMe.Enabled = (splLatLngResult != null);
        }

        GeoCoordinate SplitLatLng(string latlng)
        {
          if (latlng!="")
            try
            {
                var array = latlng.Split(',');
                var lat = double.Parse(array[0].Trim(), CultureInfo.InvariantCulture);
                var lng = double.Parse(array[1].Trim(), CultureInfo.InvariantCulture);
                return new GeoCoordinate(lat, lng);
            }
            catch (Exception ex1)
            {
                Logger.ExceptionInfo( ex1.ToString());
            }
          return null;
        }

        void SnipeMe_Click(object sender, EventArgs e)
        {
            SnipePoke((PokemonId)comboBox1.SelectedItem, (int) nudSecondsSnipe.Value,(int) nudTriesSnipe.Value ,checkBoxSnipeTransfer.Checked);
        }

        void SnipePoke(PokemonId id, int secondsToWait, int numberOfTries, bool transferIt)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Manual Snipe Triggered! We'll stop farming and go catch the pokemon ASAP");
            
            GlobalVars.SnipeOpts.ID = id;
            GlobalVars.SnipeOpts.Location = splLatLngResult;
            GlobalVars.SnipeOpts.WaitSecond = secondsToWait;
            GlobalVars.SnipeOpts.NumTries = numberOfTries;
            GlobalVars.SnipeOpts.TransferIt = transferIt;
            GlobalVars.SnipeOpts.Enabled = true;
            //GlobalVars.ForceSnipe = true;
            SnipeInfo.Text = "";
        }

        public void Execute()
        {
            SnipePokemonPokeCom.Checked = GlobalVars.SnipePokemon;
            AvoidRegionLock.Checked = GlobalVars.AvoidRegionLock;
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
            foreach (PokemonId Id in GlobalVars.NotToSnipe)
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
                    UnregisterUriScheme(URI_SCHEME);
                    UnregisterUriScheme(URI_SCHEME_MSNIPER);

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
                    RegisterUriScheme(Application.ExecutablePath,URI_SCHEME,URI_KEY);
                    RegisterUriScheme(Application.ExecutablePath,URI_SCHEME_MSNIPER,URI_KEY_MSNIPER);
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
                    if (txt.IndexOf("pokesniper2://")> -1){
                        txt = txt.Replace("pokesniper2://","");
                        var splt = txt.Split('/');
                        splLatLngResult = SplitLatLng(splt[1]);
                        int stw = 2;
                        int tries =3;
                        var transferIt = false;
                        try {
                            stw = (int) nudSecondsSnipe.Value;
                            tries = (int) nudTriesSnipe.Value;
                            transferIt = checkBoxSnipeTransfer.Checked;
                        } catch (Exception) {
                        }
                        var pokename = ToCapital(splt[0]).Replace('.','_').Replace('-','_');
                        SnipePoke( (PokemonId)Enum.Parse(typeof(PokemonId), pokename),stw,tries,transferIt);
                    }else if (txt.IndexOf("msniper://")> -1){
                        txt = txt.Replace("msniper://","");
                        var splt = txt.Split('/');
                        splLatLngResult = SplitLatLng(splt[3]);
                        int stw = 2;
                        int tries =3;
                        var transferIt = false;
                        try {
                            stw = (int) nudSecondsSnipe.Value;
                            tries = (int) nudTriesSnipe.Value;
                            transferIt = checkBoxSnipeTransfer.Checked;
                        } catch (Exception) {
                        }
                        var pokename = ToCapital(splt[0]).Replace('.','_').Replace('-','_');
                        SnipePoke( (PokemonId)Enum.Parse(typeof(PokemonId), pokename),stw,tries,transferIt);
                    }
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
        const string URI_SCHEME_MSNIPER = "msniper";
        const string URI_KEY_MSNIPER = "URL:msniper Protocol";

        static void RegisterUriScheme(string appPath, string uri_scheme, string uri_key) {
            // HKEY_CLASSES_ROOT\myscheme            
            using (RegistryKey hkcrClass = Registry.CurrentUser.CreateSubKey("Software\\Classes\\"+ uri_scheme)) {
                hkcrClass.SetValue(null, uri_key);
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
        static void UnregisterUriScheme(string uri_scheme) {
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\"+ uri_scheme);
        }
        void nudSecondsSnipe_ValueChanged(object sender, EventArgs e)
        {
          GlobalVars.SnipeOpts.WaitSecond = (int) nudSecondsSnipe.Value;
        }
        void nudTriesSnipe_ValueChanged(object sender, EventArgs e)
        {
          GlobalVars.SnipeOpts.NumTries = (int) nudTriesSnipe.Value;          
        }
        void PokesniperCom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        void buttonGo_Click(object sender, EventArgs e)
        {
            if (comboBoxLinks.SelectedItem !=null){
                var url = (comboBoxLinks.SelectedItem as Components.HRefLink).URL;
                if ( webBrowser!=null  &&  !checkBoxExternalWeb.Checked ){
                    webBrowser.Navigate(url);
                }else{
                    System.Diagnostics.Process.Start(url);
                }
            }

        }
    }
}
