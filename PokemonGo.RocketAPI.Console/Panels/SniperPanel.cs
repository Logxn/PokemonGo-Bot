/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 3:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Globalization;
using System.Net;
using POGOProtos.Enums;
using PokeMaster.Helper;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Device.Location;
using Microsoft.Win32;
using System.Text;
using PokeMaster.Logic.Functions;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;

namespace PokeMaster
{
    /// <summary>
    /// Description of SniperPanel.
    /// </summary>
    public partial class SniperPanel : UserControl
    {
        public WebBrowser webBrowser = null;
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        private const string linksFileName = "snipelinks.json";
        private static string linksFile = Path.Combine(GlobalVars.ConfigsPath, linksFileName);

        private static Components.HRefLink[] links = {
            new Components.HRefLink("pokedexs.com", "https://pokedexs.com"),
            new Components.HRefLink("pokezz.com", "http://pokezz.com"),
            new Components.HRefLink("pokewatchers.com", "http://pokewatchers.com"),
            new Components.HRefLink("mypogosnipers.com", "http://www.mypogosnipers.com"),
            new Components.HRefLink("pokesniper.org", "http://pokesniper.org/"),
            new Components.HRefLink("iv100.top", "http://iv100.top/"),
            new Components.HRefLink("pokedex100.com", "http://pokedex100.com/")
        };

        public SniperPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            IntializeCombos();
            th.Translate(this);
        }

        void IntializeCombos()
        {
            LoadLinks();

            comboBoxLinks.DataSource = links;
            comboBoxLinks.DisplayMember = "Text";
            comboBoxLinks.SelectedIndex = 0;
            
            var pokemonControlSource = new List<PokemonId>();
            //checkedListBox_ToSnipe.Items.Clear();
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId))) {
                if (pokemon == PokemonId.Missingno)
                    continue;
                pokemonControlSource.Add(pokemon);
                //checkedListBox_ToSnipe.Items.Add(th.TS(pokemon.ToString()));
            }            
            //comboBox1.DataSource = pokemonControlSource;

        }

        public void AddButtonClick(System.EventHandler evh)
        {
            buttonGo.Click += evh;
        }
        
        void ForceAutoSnipe_Click(object sender, EventArgs e)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Initiated Automatic Snipe Routine! We'll stop farming and start sniping ASAP!");
            GlobalVars.ForceSnipe = true;
        }
        void SnipePokemonPokeCom_CheckedChanged(object sender, EventArgs e)
        {
            timerAutosnipe.Enabled = (sender as CheckBox).Checked;
        }

        private GeoCoordinate splLatLngResult;

        GeoCoordinate SplitLatLng(string latlng)
        {
            if (latlng != "")
                try {
                    var array = latlng.Split(',');
                    var lat = double.Parse(array[0].Trim(), CultureInfo.InvariantCulture);
                    var lng = double.Parse(array[1].Trim(), CultureInfo.InvariantCulture);
                    return new GeoCoordinate(lat, lng);
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
            return null;
        }


        void SnipePoke(PokemonId id, int secondsToWait, int numberOfTries, bool transferIt, bool usePinap)
        {
            if (GlobalVars.SnipeOpts.Enabled){
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "There is a Snipe in process.");
                return;
            }
                
            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Manual Snipe Triggered! We'll stop farming and go catch the pokemon ASAP");
            
            GlobalVars.SnipeOpts.ID = id;
            GlobalVars.SnipeOpts.Location = splLatLngResult;
            GlobalVars.SnipeOpts.WaitSecond = secondsToWait;
            GlobalVars.SnipeOpts.NumTries = numberOfTries;
            GlobalVars.SnipeOpts.TransferIt = transferIt;
            GlobalVars.SnipeOpts.UsePinap = usePinap;
            GlobalVars.SnipeOpts.Enabled = true;
            
        }

        public void Execute()
        {
        }

        void btnInstall_Click(object sender, EventArgs e)
        {
            if (timerSnipe.Enabled) {
                try {
                    UnregisterUriScheme(URI_SCHEME);
                    UnregisterUriScheme(URI_SCHEME_MSNIPER);

                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Uninstalled");
                    timerSnipe.Enabled = false;
                } catch (Exception) {
                    MessageBox.Show(th.TS("Cannot uninstall service.") + "\n" + e.ToString());
                }                
            } else {
                try {
                    RegisterUriScheme(Application.ExecutablePath, URI_SCHEME, URI_KEY);
                    RegisterUriScheme(Application.ExecutablePath, URI_SCHEME_MSNIPER, URI_KEY_MSNIPER);
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Installed");
                    timerSnipe.Enabled = true;
                } catch (Exception) {
                    MessageBox.Show(th.TS("Cannot install service.") + "\n" + e.ToString());
                }
            }
            
        }

        void SnipeURI(string txt)
        {
            if (txt.IndexOf("pokesniper2://", StringComparison.Ordinal) > -1) {
                txt = txt.Replace("pokesniper2://", "");
                var splt = txt.Split('/');
                splLatLngResult = SplitLatLng(splt[1]);
                int stw = 2;
                int tries = 3;
                var transferIt = false;
                var usePinap = false;
                try {
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
                var pokeID = ToPokemonID(splt[0]);
                SnipePoke(pokeID, stw, tries, transferIt, usePinap);
            } else if (txt.IndexOf("msniper://", StringComparison.Ordinal) > -1) {
                txt = txt.Replace("msniper://", "");
                var splt = txt.Split('/');
                splLatLngResult = SplitLatLng(splt[3]);
                int stw = 2;
                int tries = 3;
                var transferIt = false;
                var usePinap = false;
                try {
                } catch (Exception ex1) {
                    Logger.ExceptionInfo(ex1.ToString());
                }
                var pokeID = ToPokemonID(splt[0]);
                SnipePoke(pokeID, stw, tries, transferIt, usePinap);
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            try {
                var filename = Path.GetTempPath() + "pokesniper";
                if (File.Exists(filename)) {
                    var stream = new FileStream(filename, FileMode.Open);
                    var utf8 = new UTF8Encoding();
                    var reader = new BinaryReader(stream, utf8);
                    string txt = reader.ReadString();
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Read URI: " + txt);
                    reader.Close();
                    stream.Close();
                    File.Delete(filename);
                    /* code to snipe*/
                    SnipeURI(txt);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());
            }
        }

        void loadLocationsList()
        {
            if (!File.Exists(GlobalVars.SaveLocationsFile))
                return;
            var lines = File.ReadAllLines(GlobalVars.SaveLocationsFile);
            Logger.Debug("Lines: " + lines);
            if (lines.Length > 0 && listView.Items.Count == lines.Length)
                return;
            foreach (var element in lines) {
                var columns = element.Split('|');
                var columnsCount = columns.Length;
                if (isInList(columns[2]))
                    continue;
                var listViewItem = new ListViewItem();
                if (columnsCount > 7)
                    listViewItem.Text = columns[7];
                if (columnsCount > 6)
                    listViewItem.SubItems.Add(columns[6]);
                if (columnsCount > 5)
                    listViewItem.SubItems.Add(columns[5]);
                for (var i = 0; i < 5; i++)
                    if (columnsCount > i)
                        listViewItem.SubItems.Add(columns[i]);
                
                if (listViewItem.SubItems[6].Text == GlobalVars.ProfileName)
                    listViewItem.SubItems.Add("true");
                else
                    listViewItem.SubItems.Add("false");
                
                listView.Items.Add(listViewItem);
            }
        }

        bool isInList(string id)
        {
            foreach (ListViewItem element in listView.Items) {
                if (element.SubItems[chId.Index].Text == id)
                    return true;
            }
            return false;
        }

        ListViewItem GetNextUnused()
        {
            foreach (ListViewItem element in listView.Items) {
                if ((element.SubItems[chName.Index].Text != GlobalVars.ProfileName)
                    && (element.SubItems[chUsed.Index].Text == "false")){
                    var txt = element.Text;
                    txt = txt.Replace("pokesniper2://", "");
                    var splt = txt.Split('/');
                    var pokeID = ToPokemonID(splt[0]);
                    if ( GlobalVars.ToSnipe.Contains(pokeID)){
                        return element;
                    }
                    Logger.Debug(pokeID +" not is in to snipe list");
                    element.SubItems[8].Text = "true";
                }
            }
            return null;
        }

        PokemonId ToPokemonID(string pokename)
        {
            var pokeStr = pokename.Replace('.', '_').Replace('-', '_');
            var pokeID = PokemonId.Missingno;
            Enum.TryParse<PokemonId>(pokeStr, true, out pokeID);
            return pokeID;
        }

        const string URI_SCHEME = "pokesniper2";
        const string URI_KEY = "URL:pokesniper2 Protocol";
        const string URI_SCHEME_MSNIPER = "msniper";
        const string URI_KEY_MSNIPER = "URL:msniper Protocol";

        static void RegisterUriScheme(string appPath, string uri_scheme, string uri_key)
        {
            // HKEY_CLASSES_ROOT\myscheme            
            using (RegistryKey hkcrClass = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + uri_scheme)) {
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
        static void UnregisterUriScheme(string uri_scheme)
        {
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\" + uri_scheme);
        }
        void PokesniperCom_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
        void buttonGo_Click(object sender, EventArgs e)
        {
            if (comboBoxLinks.SelectedItem != null) {
                var url = (comboBoxLinks.SelectedItem as Components.HRefLink).URL;
                if (webBrowser != null && !checkBoxExternalWeb.Checked) {
                    webBrowser.Navigate(url);
                } else {
                    System.Diagnostics.Process.Start(url);
                }
            }

        }
        private static void SaveLinks()
        {
            string ProfilesString = JsonConvert.SerializeObject(links, Formatting.Indented);
            File.WriteAllText(linksFile, ProfilesString);
        }

        private static bool LoadLinks()
        {
            if (!File.Exists(linksFile)) {
                DownloadHelper.DownloadFile("PokemonGo.RocketAPI.Console/Resources", GlobalVars.ConfigsPath, linksFileName);
            }
            if (File.Exists(linksFile)) {
                links = JsonConvert.DeserializeObject<Components.HRefLink[]>(File.ReadAllText(linksFile));
                return true;
            }
            return false;
        }

        void listView_DoubleClick(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count < 1)
                return;
            SnipeURI(listView.SelectedItems[0].Text);
            listView.SelectedItems[0].SubItems[chUsed.Index].ForeColor = Color.LightGreen;
            listView.SelectedItems[0].SubItems[chUsed.Index].Text = "true";
        }

        public static void SharePokesniperURI(string uri)
        {
            try {
                var filename = Path.GetTempPath() + "pokesniper";
                if (File.Exists(filename)) {
                    MessageBox.Show("There is a pending pokemon.\nTry latter");
                }
                var stream = new FileStream(filename, FileMode.OpenOrCreate);
                var writer = new BinaryWriter(stream, new UTF8Encoding());
                writer.Write(uri);
                stream.Close();
            } catch (Exception e) {
                MessageBox.Show(e.ToString());
            }
        }

        void timerLocations_Tick(object sender, EventArgs e)
        {
            try {
                loadLocationsList();
            } catch (Exception ex1) {
                Logger.Debug(ex1.ToString());
            }
        }

        void timerAutosnipe_Tick(object sender, EventArgs e)
        {
            if (!GlobalVars.CatchPokemon)
                return;
            var next = GetNextUnused();
            if (next != null) {
                SnipeURI(next.Text);
                next.SubItems[chUsed.Index].ForeColor = Color.LightSalmon;
                next.SubItems[chUsed.Index].Text = "true";
            }
            
        }

        void numSnipeSeconds_ValueChanged(object sender, EventArgs e)
        {
            var status = timerAutosnipe.Enabled;
            timerAutosnipe.Enabled = false;
            timerAutosnipe.Interval = (int)(sender as NumericUpDown).Value * 1000;
            timerAutosnipe.Enabled = status;
        }

        void markAsUsedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count <1)
                return;
            foreach (ListViewItem element in listView.SelectedItems) {
                element.SubItems[chUsed.Index].ForeColor = element.SubItems[chUsed.Index].Text == "true" ?  Color.LightSalmon  :  Color.White;
                element.SubItems[chUsed.Index].Text = element.SubItems[chUsed.Index].Text == "true" ? "false" : "true";
            }
        }
        void snipeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView_DoubleClick(sender,e);
        }
        void checkedListBox_ToSnipe_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var index = e.Index;
            var pokeID = (PokemonId) (index+1);
            CheckState isChecked = e.NewValue;
            if (GlobalVars.ToSnipe.Contains(pokeID))
                GlobalVars.ToSnipe.Remove(pokeID);
            if (isChecked == CheckState.Checked)
                GlobalVars.ToSnipe.Add(pokeID);
        }
        void label3_DoubleClick(object sender, EventArgs e)
        {
            var stre ="";
            foreach (var element in GlobalVars.ToSnipe) {
                stre += "" +element+",";
            }
            MessageBox.Show(stre);
        }
        //"http://www.mypogosnipers.com/data/cache/free.txt"
        void InportRemoteList(string remoteURL)
        {
            var webClient = new WebClient();
            var downloadedFile = Path.GetTempFileName();
            webClient.DownloadFile(remoteURL, downloadedFile);
            var lines = File.ReadAllLines(downloadedFile);
            // line example:-38.1197678226872,144.331814009056,177,Natu -IV: 98% - PeckFast/NightShade (mypogosnipers.com)
            if (lines.Length>0){
                foreach (var element in lines) {
                    var columns = element.Split(',');
                    var Latitude = columns[0].Trim();
                    var Longitude = columns[1].Trim();
                    var alt = columns[2].Trim();
                    var id = Latitude +Longitude+alt;
                    if (isInList(id))
                        continue;

                    var columns2 = columns[3].Trim().Split('-');
                    var pokeID = columns2[0].Trim();
                    var iv100 = columns2[1].Trim().Replace("IV:","").Replace("%","");
                    var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                    var listViewItem = new ListViewItem();
                    listViewItem.Text= $"pokesniper2://{pokeID}/{Latitude},{Longitude}";
                    listViewItem.SubItems.Add(iv100);
                    listViewItem.SubItems.Add("");
                    listViewItem.SubItems.Add(date);
                    listViewItem.SubItems.Add("");
                    listViewItem.SubItems.Add(id);
                    listViewItem.SubItems.Add("mypogosnipers");
                    listViewItem.SubItems.Add("");
                    listViewItem.SubItems.Add("false");
                    
                    listView.Items.Add(listViewItem);
                }
            }

        }
        void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (var i= listView.SelectedItems.Count -1;i>=0;i--) {
                listView.Items.Remove(listView.SelectedItems[i]);
            }
        }
        void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
        }
        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var order = (sender as ListView).Sorting;
            listView.ListViewItemSorter = new Components.ListViewItemComparer(e.Column, order);
            (sender as ListView).Sorting = order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timerAutoImport.Enabled = (sender as CheckBox).Checked;
            if (timerAutoImport.Enabled)
                timerAutoImport_Tick(sender, e);
        }

        void numAutoImport_ValueChanged(object sender, EventArgs e)
        {
            var status = timerAutoImport.Enabled;
            timerAutoImport.Enabled = false;
            timerAutoImport.Interval = (int)(sender as NumericUpDown).Value * 60000;
            timerAutoImport.Enabled = status;
        }
        void timerAutoImport_Tick(object sender, EventArgs e)
        {
            InportRemoteList(textBoxPokemonsList.Text);
        }
        void label7_DoubleClick(object sender, EventArgs e)
        {
          textBoxPokemonsList.Enabled |= Control.ModifierKeys == Keys.Shift;
        }
        void textBoxPokemonsList_Leave(object sender, EventArgs e)
        {
            textBoxPokemonsList.Enabled  = false;
        }
        void SniperPanel_Load(object sender, EventArgs e)
        {
          
        }
        void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            try {
            if (checkBox2.Checked)
                DiscordLogic.MessageReceived += InterceptedDiscortMessage;
            else
                DiscordLogic.MessageReceived -= InterceptedDiscortMessage;
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
        }
        private void InterceptedDiscortMessage(object s, DiscordLogic.DiscordReceivedDataEventArgs args)
        {
            try {
                var message = args.Message;
                var split1 = message.Split('=');
                var EncounterId = split1[0];
                Logger.Debug("EncounterId: "+ EncounterId);
                if (isInList(EncounterId)){
                    Logger.Info("Already in List");
                    return;
                }
                message = split1[1].Replace(" :100: ","").Replace(" 💯 ","")
                    .Replace(" :ok_hand: ","").Replace(" 👌 ","")
                    .Replace("♀","").Replace("♂","").Trim();
                Logger.Debug("message: "+ message);
                var split2 = message.Split(' ');
                var pokeID = split2[0];
                Logger.Debug("pokeID: "+ pokeID);
                var Latitude = split2[1].Replace(",","");
                Logger.Debug("Latitude: "+ Latitude);
                var Longitude = split2[2];
                Logger.Debug("Longitude: "+ Longitude);
                var split3 = message.Split(':');
                var iv100 = split3[1].Trim().Split('%')[0];
                Logger.Debug("iv100: "+ iv100);
                var prob = split3[5].Trim().Split('%')[0];
                Logger.Debug("prob: "+ prob);
                var date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                var listViewItem = new ListViewItem();
                listViewItem.Text= $"pokesniper2://{pokeID}/{Latitude},{Longitude}";
                listViewItem.SubItems.Add(iv100);
                listViewItem.SubItems.Add(prob);
                listViewItem.SubItems.Add(date);
                listViewItem.SubItems.Add("");
                listViewItem.SubItems.Add(EncounterId);
                listViewItem.SubItems.Add(args.Username);
                listViewItem.SubItems.Add("");
                listViewItem.SubItems.Add("false");
                
                listView.Items.Add(listViewItem);

            } catch (Exception ex1) {
                Logger.Error("Message Interception Failed");
                Logger.ExceptionInfo(ex1.ToString());
            }
        }
        static readonly double[,] TimeValues = {{ 1, 1 },
            { 2, 1 },
            { 3, 2 },
            { 4.6, 2 },
            { 5, 2 },
            { 7, 5 },
            { 9, 7 },
            { 10, 7 },
            { 12, 8 },
            { 18, 10 },
            { 26, 15 },
            { 42, 19 },
            { 65, 22 },
            { 76, 25 },
            { 81, 25 },
            { 100, 35 },
            { 20, 40 },
            { 250, 45 },
            { 350, 51 },
            { 460, 58 },
            { 500, 60 },
            { 565, 67 },
            { 700, 75 },
            { 716, 78 },
            { 830, 86 },
            { 1000, 90 },
            { 1500, 120 }
        };
        public static double GetMinimalTimeToWait(double km)
        {
            for (var i = TimeValues.Length/2 - 1; i > 0; i--){
                var val0 =TimeValues[i,0];
                var val1 =TimeValues[i,1];
                if (km >= val0)
                    return val1;
            }
            return TimeValues[0,1];
        }
        void textBox1_TextChanged(object sender, EventArgs e)
        {
            try {
                var splitted = textBoxLocation.Text.Split(',');
                var distance = LocationUtils.CalculateDistanceInMeters(Logic.Logic.objClient.CurrentLatitude, Logic.Logic.objClient.CurrentLongitude, double.Parse(splitted[0].Trim(),CultureInfo.InvariantCulture), double.Parse(splitted[1].Trim(),CultureInfo.InvariantCulture));
                labelTime.Text = GetMinimalTimeToWait(distance /1000).ToString();
            } catch (Exception ex) {
                labelTime.Text = ex.Message;
            }
          
        }

    }
}
