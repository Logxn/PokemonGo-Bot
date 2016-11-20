using PokemonGo.RocketAPI.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

using PokemonGo.RocketAPI.Logic.Translation;
using POGOProtos.Enums;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace PokemonGo.RocketAPI.Console
{
    public partial class GUI : Form
    {
        public static NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        public static int[] evolveBlacklist = {
            3, 6, 9, 12, 15, 18, 20, 22, 24, 26, 28, 31, 34, 36, 38, 40, 42, 45, 47, 49, 51, 53, 55, 57, 59, 62, 65, 68, 71, 73, 76, 78, 80, 82, 83, 85, 87, 89, 91, 94, 95, 97, 99, 101, 103, 105, 106, 107, 108, 110, 112, 113, 114, 115, 117, 119, 121, 122, 123, 124, 125, 126, 127, 128, 130, 131, 132, 134, 135, 136, 137, 139, 141, 142, 143, 144, 145, 146, 149, 150, 151
        };
        public static ISettings _clientSettings;

        /* PATHS */
        static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static string devicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        static string deviceinfo = Path.Combine(devicePath, "DeviceInfo.txt");
        static string PokeDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PokeData");
        static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        static string logs = Path.Combine(logPath, "PokeLog.txt");
        static string logmanualtransfer = Path.Combine(logPath, "TransferLog.txt");
        static Profile ActiveProfile;
        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        static Dictionary<string, int> evolveIDS = new Dictionary<string, int>();

        public GUI()
        {
            InitializeComponent();
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            _clientSettings = new Settings();
            Globals.FirstLoad = false;
            Directory.CreateDirectory(Program.path);
            Directory.CreateDirectory(Program.path_translation);
            Directory.CreateDirectory(Program.path_pokedata);
            Directory.CreateDirectory(devicePath);
            Directory.CreateDirectory(PokeDataPath);

            if (File.Exists($@"{baseDirectory}\update.bat"))
                File.Delete($@"{baseDirectory}\update.bat");

            comboBox_AccountType.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };

            comboBox_AccountType.DataSource = types;
            if (!File.Exists(deviceinfo))
            {
                var f = File.Create(deviceinfo);
                f.Close();
                File.WriteAllLines(deviceinfo, new string[] { "galaxy6", " " });
            }
            else
            {
                string[] arrLine = File.ReadAllLines(deviceinfo);
                try
                {
                    if (arrLine[0] != null)
                    {
                        comboBox_Device.Text = arrLine[0];
                    }
                }
                catch (Exception)
                {
                }
            }

            /* TRANSLATION */
            List<string> b = new List<string>();
            b.Add("de.json");
            b.Add("france.json");
            b.Add("italian.json");
            b.Add("ptBR.json");
            b.Add("ru.json");
            b.Add("spain.json");
            b.Add("tr.json");
            b.Add("arabic.json");

            foreach (var l in b)
                Extract("PokemonGo.RocketAPI.Console", Program.path_translation, "Lang", l);

            List<string> pokeData = new List<string>();
            pokeData.Add("AdditionalPokeData.json");

            foreach (var extract in pokeData)
                Extract("PokemonGo.RocketAPI.Console", Program.path_pokedata, "PokeData", extract);

            TranslationHandler.Init();
            int i = 1;
            int ev = 1;

            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = i;
                    checkedListBox_PokemonNotToTransfer.Items.Add(pokemon.ToString());
                    checkedListBox_PokemonNotToCatch.Items.Add(pokemon.ToString());
                    if (!(evolveBlacklist.Contains(i)))
                    {
                        checkedListBox_PokemonToEvolve.Items.Add(pokemon.ToString());
                        evolveIDS[pokemon.ToString()] = ev;
                        ev++;
                    }
                    checkedListBox_NotToSnipe.Items.Add(pokemon.ToString());
                    i++;
                }
            }
            Globals.noTransfer = new List<PokemonId>();
            Globals.noCatch = new List<PokemonId>();
            Globals.doEvolve = new List<PokemonId>();
            Globals.NotToSnipe = new List<PokemonId>();

            #region Loading Everything into GUI 

            var loaded = false;
            Profile blankProfile = new Profile();
            blankProfile.ProfileName = "DefaultProfile";
            blankProfile.IsDefault = true;
            blankProfile.RunOrder = 1;
            blankProfile.SettingsJSON = "";
            Profileselect.DisplayMember = "ProfileName";
            Profile selectedProfile = blankProfile;
            if (File.Exists(Program.accountProfiles))
            {
                try
                {
                    string JSONstring = File.ReadAllText(@Program.accountProfiles);
                    Collection<Profile> profiles = JsonConvert.DeserializeObject<Collection<Profile>>(JSONstring);
                    if (profiles.Count == 1 && profiles.First().ProfileName != "DefaultProfile")
                    {
                        Globals.Profiles.Add(blankProfile);
                    }
                    foreach (Profile _profile in profiles)
                    {
                        Globals.Profiles.Add(_profile);
                        if (_profile.IsDefault)
                        {
                            ActiveProfile = _profile;
                            selectedProfile = _profile;
                            LoadData(_profile.SettingsJSON);
                            loaded = true;
                        }
                    }
                    if (!loaded)
                    {
                        ActiveProfile = blankProfile;
                        selectedProfile = blankProfile;
                    }
                }
                catch
                {
                    Globals.Profiles.Add(blankProfile);
                }
            }
            else
            {
                Globals.Profiles.Add(blankProfile);
                ActiveProfile = blankProfile;
            }
            Profileselect.DataSource = Globals.Profiles;
            Profileselect.SelectedItem = selectedProfile;
            ///* VERSION INFORMATION */
            var currVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var newestVersion = Program.getNewestVersion();

            currVer.Text = currVersion.ToString();
            ver.Text = $"Version: {currVersion}";
            newVer.Text = newestVersion.ToString();


            if (Program.getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
            {
                if (checkbox_AutoUpdate.Checked)
                {
                    Form update = new Update();
                    update.ShowDialog();
                }
                else
                {
                    DialogResult dialogResult = MessageBox.Show("There is an Update on Github. do you want to open it ?", $"Newest Version: {newestVersion}", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
                    else if (dialogResult == DialogResult.No)
                    {

                    }
                }

            }
            else
            {
                currVer.ForeColor = Color.Green;
                newVer.ForeColor = Color.Green;
            }
            #endregion
        }

        private void LoadData(string configString)
        //TODO - Accomplish this with Data-Binding from Globals to eliminate load and update lines
        {
            try
            {
                if (configString != "")
                {

                    var settings = new JsonSerializerSettings
                    {
                        Error = (sender, args) =>
                        {
                            if (System.Diagnostics.Debugger.IsAttached)
                            {
                                System.Diagnostics.Debugger.Break();
                            }
                        }
                    };
                    var config = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(configString, settings);
                    // tab 1 
                    ProfileName.Text = config.ProfileName;
                    checkBox1.Checked = config.IsDefault;

                    comboBox_AccountType.SelectedIndex = 1;
                    if (config.AuthType == Enums.AuthType.Google)
                    {
                        comboBox_AccountType.SelectedIndex = 0;
                    }
                    text_EMail.Text = config.GoogleUsername;
                    text_Password.Text = config.GooglePassword;
                    checkbox_PWDEncryption.Checked = config.UsePwdEncryption;
                    if (checkbox_PWDEncryption.Checked)
                    {
                        string encryptedPassword = config.GooglePassword;
                        if (config.AuthType != Enums.AuthType.Google)
                        {
                            encryptedPassword = config.PtcPassword;
                        }
                        Globals.password = Encryption.Decrypt(encryptedPassword);
                        text_Password.Text = Globals.password;
                    }

                    text_Latidude.Text = config.DefaultLatitude.ToString();
                    text_Longitude.Text = config.DefaultLongitude.ToString();
                    text_Altidude.Text = config.DefaultAltitude.ToString();

                    checkBox_UseLuckyEggAtEvolve.Checked = config.UseLuckyEgg;
                    checkBox_SimulateAnimationTimeAtEvolve.Checked = config.UseAnimationTimes;
                    checkBox_EvolvePokemonIfEnoughCandy.Checked = config.EvolvePokemonsIfEnoughCandy;
                    checkBox_UseIncenseEvery30min.Checked = config.UseIncense;
                    checkBox_EnablePokemonListGui.Checked = config.EnablePokeList;
                    CB_SimulatePGO.Checked = config.simulatedPGO;
                    checkBox_KeepPokemonWhichCanBeEvolved.Checked = config.keepPokemonsThatCanEvolve;
                    checkBox_UseLuckyEggIfNotRunning.Checked = config.UseLuckyEggIfNotRunning;

                    // tab 2 - Pokemons
                    if (config.pokemonsToHold != null)
                        foreach (PokemonId Id in config.pokemonsToHold)
                        {
                            string _id = Id.ToString();
                            checkedListBox_PokemonNotToTransfer.SetItemChecked(pokeIDS[_id] - 1, true);
                        }
                    if (config.catchPokemonSkipList != null)
                        foreach (PokemonId Id in config.catchPokemonSkipList)
                        {
                            string _id = Id.ToString();
                            checkedListBox_PokemonNotToCatch.SetItemChecked(pokeIDS[_id] - 1, true);
                        }
                    if (config.pokemonsToEvolve != null)
                        foreach (PokemonId Id in config.pokemonsToEvolve)
                        {
                            string _id = Id.ToString();
                            checkedListBox_PokemonToEvolve.SetItemChecked(evolveIDS[_id] - 1, true);
                        }

                    if (config.NotToSnipe != null)
                        foreach (PokemonId Id in config.NotToSnipe)
                        {
                            string _id = Id.ToString();
                            checkedListBox_NotToSnipe.SetItemChecked(pokeIDS[_id] - 1, true);
                        }

                    checkBox_AutoTransferDoublePokemon.Checked = config.TransferDoublePokemons;
                    checkBox_TransferFirstLowIV.Checked = config.TransferFirstLowIV;
                    text_MaxDuplicatePokemon.Text = config.HoldMaxDoublePokemons.ToString();
                    text_MaxIVToTransfer.Text = config.ivmaxpercent.ToString();
                    text_MaxCPToTransfer.Text = config.DontTransferWithCPOver.ToString();
                    MinCPtoCatch.Text = config.MinCPtoCatch.ToString();
                    MinIVtoCatch.Text = config.MinIVtoCatch.ToString();

                    // tab 3 - throws
                    checkBox2.Checked = Globals.LimitPokeballUse;
                    checkBox3.Checked = Globals.LimitGreatballUse;
                    checkBox7.Checked = Globals.LimitUltraballUse;
                    numericUpDown1.Value = Globals.Max_Missed_throws;
                    numericUpDown2.Value = Globals.InventoryBasePokeball;
                    numericUpDown3.Value = Globals.InventoryBaseGreatball;
                    numericUpDown4.Value = Globals.InventoryBaseUltraball;

                    checkBox_UseRazzberryIfChanceUnder.Checked = config.UseRazzBerry;
                    text_UseRazzberryChance.Text = (config.razzberry_chance * 100).ToString();
                    NextBestBallOnEscape.Checked = config.NextBestBallOnEscape;

                    text_Pb_Excellent.Text = config.Pb_Excellent.ToString();
                    text_Pb_Great.Text = config.Pb_Great.ToString();
                    text_Pb_Nice.Text = config.Pb_Nice.ToString();
                    text_Pb_Ordinary.Text = config.Pb_Ordinary.ToString();

                    GreatBallMinCP.Text = config.MinCPforGreatBall.ToString();
                    UltraBallMinCP.Text = config.MinCPforUltraBall.ToString();

                    // Tab 4 - Items
                    text_MaxPokeballs.Text = config.MaxPokeballs.ToString();
                    text_MaxGreatBalls.Text = config.MaxGreatballs.ToString();
                    text_MaxUltraBalls.Text = config.MaxUltraballs.ToString();
                    text_MaxRevives.Text = config.MaxRevives.ToString();
                    text_MaxTopRevives.Text = config.MaxTopRevives.ToString();
                    text_MaxPotions.Text = config.MaxPotions.ToString();
                    text_MaxSuperPotions.Text = config.MaxSuperPotions.ToString();
                    text_MaxHyperPotions.Text = config.MaxHyperPotions.ToString();
                    text_MaxTopPotions.Text = config.MaxTopPotions.ToString();
                    text_MaxRazzBerrys.Text = config.MaxBerries.ToString();

                    //tab eggs
                    checkBox_AutoIncubate.Checked = config.AutoIncubate;
                    checkBox_UseBasicIncubators.Checked = config.UseBasicIncubators;
                    checkBox_10kmEggs.Checked = config.No10kmEggs;
                    checkBox_2kmEggs.Checked = config.No2kmEggs;
                    checkBox_5kmEggs.Checked = config.No5kmEggs;
                    if (config.EggsAscendingSelection)
                        rbSOEggsAscending.Checked = true;
                    else
                        rbSOEggsDescending.Checked = true;
                    checkBox_10kmEggsBasicInc.Checked = config.No10kmEggsBasicInc;
                    checkBox_2kmEggsBasicInc.Checked = config.No2kmEggsBasicInc;
                    checkBox_5kmEggsBasicInc.Checked = config.No5kmEggsBasicInc;
                    if (config.EggsAscendingSelectionBasicInc)
                        rbSOEggsAscendingBasicInc.Checked = true;
                    else
                        rbSOEggsDescendingBasicInc.Checked = true;
                        

                    // tab 5 proxy


                    // tab 6 walk
                    text_Speed.Text = config.WalkingSpeedInKilometerPerHour.ToString();
                    text_MinWalkSpeed.Text = config.MinWalkSpeed.ToString();
                    text_MoveRadius.Text = config.MaxWalkingRadiusInMeters.ToString();
                    text_TimeToRun.Text = config.TimeToRun.ToString();

                    text_PokemonCatchLimit.Text = config.PokemonCatchLimit.ToString();
                    text_PokestopFarmLimit.Text = config.PokestopFarmLimit.ToString();
                    text_XPFarmedLimit.Text = config.XPFarmedLimit.ToString();
                    text_BreakInterval.Text = config.BreakInterval.ToString();
                    text_BreakLength.Text = config.BreakLength.ToString();

                    checkBox_StopWalkingWhenEvolving.Checked = config.pauseAtEvolve;

                    checkBox_UseGoogleMapsRouting.Checked = config.UseGoogleMapsAPI;
                    text_GoogleMapsAPIKey.Text = config.GoogleMapsAPIKey;

                    checkBox_RandomSleepAtCatching.Checked = config.sleepatpokemons;
                    checkBox_FarmPokestops.Checked = config.FarmPokestops;
                    checkBox_CatchPokemon.Checked = config.CatchPokemon;
                    checkBox_BreakAtLure.Checked = config.BreakAtLure;
                    checkBox_UseLureAtBreak.Checked = config.UseLureAtBreak;
                    checkBox_RandomlyReduceSpeed.Checked = config.RandomReduceSpeed;
                    checkBox_UseBreakIntervalAndLength.Checked = config.UseBreakFields;
                    checkBox_WalkInArchimedeanSpiral.Checked = config.Espiral;
                    checkBox_Start_Walk_from_default_location.Checked = config.WalkBackToDefaultLocation;

                    // tab 7 - telegram and logs
                    logPokemon.Checked = config.LogPokemon;
                    logManuelTransfer.Checked = config.LogTransfer;
                    logEvolution.Checked = config.LogEvolve;
                    checkbox_LogEggs.Checked = config.LogEggs;

                    text_Telegram_Token.Text = config.TelegramAPIToken;
                    text_Telegram_Name.Text = config.TelegramName;
                    text_Telegram_LiveStatsDelay.Text = config.TelegramLiveStatsDelay.ToString();

                    SnipePokemonPokeCom.Checked = config.SnipePokemon;
                    AvoidRegionLock.Checked = config.AvoidRegionLock;

                    // tab 8 - update                    
                    checkbox_AutoUpdate.Checked = config.AutoUpdate;
                    checkbox_checkWhileRunning.Checked = config.CheckWhileRunning;
                    langSelected = config.SelectedLanguage;

                    // Dev Options
                    checkbox_Verboselogging.Checked = config.EnableVerboseLogging;

                    var success = LoadGlobals(false);
                    if (!success)
                    {
                        MessageBox.Show("Loading Config failed - Check settings before running!");
                    }
                }
                else
                {
                    Exception e = new Exception("Loading Defaults");
                    throw e;
                }
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("Loading Defaults") && Globals.FirstLoad)
                    MessageBox.Show("Loading Config failed - Check settings before running!");

                text_Latidude.Text = "40,764883";
                text_Longitude.Text = "-73,972967";
                text_Altidude.Text = "10";
                text_Speed.Text = "50";
                text_MoveRadius.Text = "5000";
                text_MaxDuplicatePokemon.Text = "3";
                text_MaxCPToTransfer.Text = "999";
                text_Telegram_LiveStatsDelay.Text = "5000";
                text_Pb_Excellent.Text = "25";
                text_Pb_Great.Text = "25";
                text_Pb_Nice.Text = "25";
                text_Pb_Ordinary.Text = "25";
                text_MaxPokeballs.Text = "100";
                text_MaxGreatBalls.Text = "100";
                text_MaxUltraBalls.Text = "100";
                text_MaxRevives.Text = "100";
                text_MaxPotions.Text = "100";
                text_MaxSuperPotions.Text = "100";
                text_MaxHyperPotions.Text = "100";
                text_MaxRazzBerrys.Text = "100";
                text_MaxTopPotions.Text = "100";
                text_MaxTopRevives.Text = "100";
                var success = LoadGlobals(false);
                if (!success)
                {
                    MessageBox.Show("Loading Config failed - Check settings before running!");
                }
            }
            if (File.Exists(Program.lastcords))
            {
                try
                {
                    var latlngFromFile = File.ReadAllText(Program.lastcords);
                    var latlng = latlngFromFile.Split(':');
                    double latitude, longitude;
                    double.TryParse(latlng[0], out latitude);
                    double.TryParse(latlng[1], out longitude);
                    Globals.latitute = latitude;
                    Globals.longitude = longitude;
                }
                catch
                {

                }
            }
            // Load Proxy Settings
            if (_clientSettings.UseProxyHost != string.Empty)
                prxyIP.Text = _clientSettings.UseProxyHost;

            if (_clientSettings.UseProxyPort != 0)
                prxyPort.Text = "" + _clientSettings.UseProxyPort;

            if (_clientSettings.UseProxyUsername != string.Empty)
                prxyUser.Text = _clientSettings.UseProxyUsername;

            if (_clientSettings.UseProxyPassword != string.Empty)
                prxyPass.Text = "" + _clientSettings.UseProxyPassword;

            if (prxyIP.Text != "HTTPS Proxy IP")
                _clientSettings.UseProxyVerified = true;
            else
                _clientSettings.UseProxyVerified = false;

            if (prxyUser.Text != "Proxy Username")
                _clientSettings.UseProxyAuthentication = true;
            else
                _clientSettings.UseProxyAuthentication = false;

            // Placeholder event add
            prxyIP.GotFocus += new EventHandler(prxy_GotFocus);
            prxyPort.GotFocus += new EventHandler(prxy_GotFocus);
            prxyUser.GotFocus += new EventHandler(prxy_GotFocus);
            prxyPass.GotFocus += new EventHandler(prxy_GotFocus);
            Globals.FirstLoad = true;
        }
        //Account Type Changed Event
        private void comboAccType_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (comboBox_AccountType.SelectedIndex == 0)
                label2.Text = "E-Mail:";
            else
                label2.Text = TranslationHandler.GetString("username", "Username :");
        }
        private void Profileselectg_SelectedIndexChanged(object sender, EventArgs e)
        {
            Profile selectedProfile = (Profile)Globals.Profiles.Where(i => i == Profileselect.SelectedItem).FirstOrDefault();
            LoadData(selectedProfile.SettingsJSON);
        }
        //Password KeyPress Event
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
                e.Handled = true;
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                e.Handled = true;
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
                e.Handled = true;
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                e.Handled = true;
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                e.Handled = true;
        }

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private const string NEW_YORK_COORS = "40.764883;-73.972967";
        private void button1_Click(object sender, EventArgs e)
        {
            var selectedCoords =Globals.latitute.ToString("0.000000") +";"+Globals.longitude.ToString("0.000000");
            
            selectedCoords = selectedCoords.Replace(",",".");
            if (selectedCoords.Equals(NEW_YORK_COORS))
            {
                var ret = MessageBox.Show("Have you set correctly your location? (It seems like you are using default coords. This can lead to an auto-ban from niantic)", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (ret == DialogResult.No)
                {
                    return;
                }
            }
            if (Save())
            {
                Dispose();
            }else
                MessageBox.Show("Please Review Red Boxes Before Start");

        }

        private bool textBoxToGlobal(TextBox textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty)
            {
                if (fieldName == string.Empty)
                {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                typeof(Globals).GetField(fieldName).SetValue(null, textBox.Text);
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }
        private bool textBoxToGlobalDouble(TextBox textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty)
            {
                if (fieldName == string.Empty)
                {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                var valueTXT = textBox.Text.Replace(',', '.');
                var valueDBL = double.Parse(valueTXT, cords, NumberFormatInfo.InvariantInfo);
                typeof(Globals).GetField(fieldName).SetValue(null, valueDBL);
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool textBoxToGlobalInt(TextBox textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty)
            {
                if (fieldName == string.Empty)
                {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                typeof(Globals).GetField(fieldName).SetValue(null,
                     int.Parse(textBox.Text));
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool LoadGlobals(bool makePrompts=true)
        {
            #region Setting aaaaaaaaaaaall the globals

            // tab 1 - General     
            Globals.acc = (comboBox_AccountType.SelectedIndex == 0) ? Enums.AuthType.Google : Enums.AuthType.Ptc;

            // Account Info
            bool ret = true;
            ret &= textBoxToGlobal(text_EMail);
            ret &= textBoxToGlobal(text_Password);
            Globals.usePwdEncryption = checkbox_PWDEncryption.Checked;

            // Location
            ret &= textBoxToGlobalDouble(text_Latidude, "latitute");
            ret &= textBoxToGlobalDouble(text_Longitude, "longitude");
            ret &= textBoxToGlobalDouble(text_Altidude, "altitude");

            // Other
            Globals.useluckyegg = checkBox_UseLuckyEggAtEvolve.Checked;
            Globals.UseAnimationTimes = checkBox_SimulateAnimationTimeAtEvolve.Checked;
            Globals.evolve = checkBox_EvolvePokemonIfEnoughCandy.Checked;
            Globals.useincense = checkBox_UseIncenseEvery30min.Checked;
            Globals.pokeList = checkBox_EnablePokemonListGui.Checked;
            Globals.simulatedPGO = CB_SimulatePGO.Checked;
            Globals.keepPokemonsThatCanEvolve = checkBox_KeepPokemonWhichCanBeEvolved.Checked;
            Globals.useLuckyEggIfNotRunning = checkBox_UseLuckyEggIfNotRunning.Checked;

            // tab 2 - pokemons
            Globals.noTransfer.Clear();
            Globals.noCatch.Clear();
            Globals.doEvolve.Clear();
            Globals.NotToSnipe.Clear();

            foreach (string pokemon in checkedListBox_PokemonNotToTransfer.CheckedItems)
            {
                Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox_PokemonNotToCatch.CheckedItems)
            {
                Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox_PokemonToEvolve.CheckedItems)
            {
                Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox_NotToSnipe.CheckedItems)
            {
                Globals.NotToSnipe.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            // bot settings
            Globals.transfer = checkBox_AutoTransferDoublePokemon.Checked;
            Globals.TransferFirstLowIV = checkBox_TransferFirstLowIV.Checked;

            ret &= textBoxToGlobalInt(text_MaxDuplicatePokemon, "duplicate");
            ret &= textBoxToGlobalInt(text_MaxIVToTransfer, "ivmaxpercent");
            ret &= textBoxToGlobalInt(text_MaxCPToTransfer, "maxCp");

            // tab 3 - Throw
            Globals.LimitPokeballUse = checkBox2.Checked;
            Globals.LimitGreatballUse = checkBox3.Checked;
            Globals.LimitUltraballUse = checkBox7.Checked;
            Globals.Max_Missed_throws = (int)numericUpDown1.Value;
            Globals.InventoryBasePokeball = (int)numericUpDown2.Value;
            Globals.InventoryBaseGreatball = (int)numericUpDown3.Value;
            Globals.InventoryBaseUltraball = (int)numericUpDown4.Value;

            Globals.userazzberry = checkBox_UseRazzberryIfChanceUnder.Checked;
            if (text_UseRazzberryChance.Text == string.Empty)
            {
                text_UseRazzberryChance.BackColor = Color.Red;
            }
            else
            {
                int x = int.Parse(text_UseRazzberryChance.Text);
                decimal c = ((decimal)x / 100);
                Globals.razzberry_chance = Convert.ToDouble(c);
            }

            ret &= textBoxToGlobalInt(text_Pb_Excellent, "excellentthrow");
            ret &= textBoxToGlobalInt(text_Pb_Great, "greatthrow");
            ret &= textBoxToGlobalInt(text_Pb_Nice, "nicethrow");
            ret &= textBoxToGlobalInt(text_Pb_Ordinary, "ordinarythrow");
            ret &= textBoxToGlobalInt(GreatBallMinCP, "MinCPforGreatBall");
            ret &= textBoxToGlobalInt(UltraBallMinCP, "MinCPforUltraBall");

            // tab 4 - Items
            ret &= textBoxToGlobalInt(text_MaxPokeballs, "pokeball");
            ret &= textBoxToGlobalInt(text_MaxGreatBalls, "greatball");
            ret &= textBoxToGlobalInt(text_MaxUltraBalls, "ultraball");
            ret &= textBoxToGlobalInt(text_MaxRevives, "revive");
            ret &= textBoxToGlobalInt(text_MaxTopRevives, "toprevive");
            ret &= textBoxToGlobalInt(text_MaxPotions, "potion");
            ret &= textBoxToGlobalInt(text_MaxSuperPotions, "superpotion");
            ret &= textBoxToGlobalInt(text_MaxHyperPotions, "hyperpotion");
            ret &= textBoxToGlobalInt(text_MaxTopPotions, "toppotion");
            ret &= textBoxToGlobalInt(text_MaxRazzBerrys, "berry");
            ret &= textBoxToGlobalInt(MinCPtoCatch, "MinCPtoCatch");
            ret &= textBoxToGlobalInt(MinIVtoCatch, "MinIVtoCatch");

            // tab  - Eggs
            Globals.autoIncubate = checkBox_AutoIncubate.Checked;
            Globals.useBasicIncubators = checkBox_UseBasicIncubators.Checked;
            Globals.No2kmEggs = checkBox_2kmEggs.Checked;
            Globals.No5kmEggs = checkBox_5kmEggs.Checked;
            Globals.No10kmEggs = checkBox_10kmEggs.Checked;
            Globals.EggsAscendingSelection = rbSOEggsAscending.Checked;
            Globals.No2kmEggsBasicInc = checkBox_2kmEggsBasicInc.Checked;
            Globals.No5kmEggsBasicInc = checkBox_5kmEggsBasicInc.Checked;
            Globals.No10kmEggsBasicInc = checkBox_10kmEggsBasicInc.Checked;
            Globals.EggsAscendingSelectionBasicInc = rbSOEggsAscendingBasicInc.Checked;

            // tab  - Proxy
            /*
            UserSettings.Default.UseProxyVerified = checkBox_UseProxy.Checked;
            UserSettings.Default.UseProxyAuthentication = checkBox_UseProxyAuth.Checked;
            UserSettings.Default.UseProxyHost = prxyIP.Checked;
            UserSettings.Default.UseProxyPort = prxyPort.Checked;
            UserSettings.Default.UseProxyUsername = prxyUser.Checked;
            UserSettings.Default.UseProxyPassword = prxyPass.Checked;
            */

            // tab 6 - Walk
            ret &= textBoxToGlobalDouble(text_Speed);
            if ((makePrompts) && (Globals.speed > 15 && Globals.FirstLoad))
            {
                var speed = Globals.speed;
                var dialogResult = MessageBox.Show("The risk of being banned is significantly greater when using higher than human jogging speeds (e.g. > 15km/hr) Click 'No' to use ~10km/hr instead", $"Are you sure you wish to set your speed to {speed} ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    Globals.speed = double.Parse("9.5", cords, NumberFormatInfo.InvariantInfo);
            }

            var value = text_MinWalkSpeed.Text;
            if (value != String.Empty)
                Globals.MinWalkSpeed = int.Parse(value);

            ret &= textBoxToGlobalInt(text_MoveRadius, "radius");

            if (text_TimeToRun.Text == String.Empty)
                text_TimeToRun.Text = "0";
            Globals.TimeToRun = Double.Parse(text_TimeToRun.Text);

            value = text_PokemonCatchLimit.Text;
            if (value != String.Empty)
                Globals.PokemonCatchLimit = int.Parse(value);

            value = text_PokestopFarmLimit.Text;
            if (value != String.Empty)
                Globals.PokestopFarmLimit = int.Parse(value);

            value = text_XPFarmedLimit.Text;
            if (value != String.Empty)
                Globals.XPFarmedLimit = int.Parse(value);

            value = text_BreakInterval.Text;
            if (value != String.Empty)
                Globals.BreakInterval = int.Parse(value);

            value = text_BreakLength.Text;
            if (value != String.Empty)
                Globals.BreakLength = int.Parse(value);

            Globals.pauseAtEvolve = checkBox_StopWalkingWhenEvolving.Checked;
            Globals.pauseAtEvolve2 = checkBox_StopWalkingWhenEvolving.Checked;

            Globals.UseGoogleMapsAPI = checkBox_UseGoogleMapsRouting.Checked;
            Globals.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;

            Globals.sleepatpokemons = checkBox_RandomSleepAtCatching.Checked;
            Globals.farmPokestops = checkBox_FarmPokestops.Checked;
            Globals.CatchPokemon = checkBox_CatchPokemon.Checked;
            Globals.BreakAtLure = checkBox_BreakAtLure.Checked;
            Globals.UseLureAtBreak = checkBox_UseLureAtBreak.Checked;
            Globals.RandomReduceSpeed = checkBox_RandomlyReduceSpeed.Checked;
            Globals.UseBreakFields = checkBox_UseBreakIntervalAndLength.Checked;

            Globals.Espiral = checkBox_WalkInArchimedeanSpiral.Checked;
            Globals.defLoc = checkBox_Start_Walk_from_default_location.Checked;

            // tab 7 - Logs and Telegram            
            Globals.logPokemons = logPokemon.Checked;
            Globals.logManualTransfer = logManuelTransfer.Checked;
            Globals.bLogEvolve = logEvolution.Checked;
            Globals.LogEggs = checkbox_LogEggs.Checked;

            Globals.telAPI = text_Telegram_Token.Text;
            Globals.telName = text_Telegram_Name.Text;
            ret &= textBoxToGlobalInt(text_Telegram_LiveStatsDelay, "telDelay");
            Globals.SnipePokemon = SnipePokemonPokeCom.Checked;
            if ((makePrompts) && (Globals.SnipePokemon && !Globals.FirstLoad))
            {
                DialogResult result = MessageBox.Show("Sniping has not been tested yet. It could get you banned. Do you want to continue?", "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    Globals.SnipePokemon = true;
                else
                    Globals.SnipePokemon = false;
            }
            Globals.AvoidRegionLock = AvoidRegionLock.Checked;
            // tab 8 updates
            Globals.AutoUpdate = checkbox_AutoUpdate.Checked;
            Globals.CheckWhileRunning = checkbox_checkWhileRunning.Checked;
            Globals.NextBestBallOnEscape = NextBestBallOnEscape.Checked;
            Globals.settingsLanguage = langSelected;
            Globals.NextDestinationOverride.Clear();
            Globals.RouteToRepeat.Clear();

            // dev options

            Globals.EnableVerboseLogging = checkbox_Verboselogging.Checked;


            #endregion
            return ret;
        }

        private bool Save()
        {
            var success = LoadGlobals();

            if (success)
            {
                #region CreatingSettings
                var encryptedPassword = Encryption.Encrypt(Globals.password);
                var decryptedPassword = Encryption.Decrypt(encryptedPassword);

                if (Globals.usePwdEncryption)
                {
                    Globals.password = encryptedPassword;
                }

                Settings settings = new Settings();
                var configString = Newtonsoft.Json.JsonConvert.SerializeObject(settings);

                ActiveProfile.ProfileName = Globals.ProfileName;
                ActiveProfile.IsDefault = Globals.IsDefault;
                ActiveProfile.RunOrder = Globals.RunOrder;
                ActiveProfile.SettingsJSON = configString;
                Collection<Profile> newProfiles = new Collection<Profile>();
                string profileJSON = "";
                if (File.Exists(@Program.accountProfiles))
                {
                    var accountString = File.ReadAllText(@Program.accountProfiles);
                    Collection<Profile> _profiles = JsonConvert.DeserializeObject<Collection<Profile>>(accountString);

                    foreach (Profile _profile in _profiles)
                    {
                        if (_profile.ProfileName != ActiveProfile.ProfileName)
                            newProfiles.Add(_profile);
                    }
                }
                newProfiles.Add(ActiveProfile);
                profileJSON = Newtonsoft.Json.JsonConvert.SerializeObject(newProfiles);
                File.WriteAllText(@Program.accountProfiles, profileJSON);
                Globals.password = decryptedPassword;
                #endregion
                
                return true;
            }
            else
            {
                return false;
            }
           
        }

        #region CheckedChanged Events

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonNotToTransfer.Items.Count)
            {
                checkedListBox_PokemonNotToTransfer.SetItemChecked(i, checkBox4.Checked);
                i++;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonNotToCatch.Items.Count)
            {
                checkedListBox_PokemonNotToCatch.SetItemChecked(i, checkBox5.Checked);
                i++;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonToEvolve.Items.Count)
            {
                checkedListBox_PokemonToEvolve.SetItemChecked(i, checkBox6.Checked);
                i++;
            }
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

        #endregion

        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://high-minded.net/threads/pokemon-go-c-bot-safer-better.50731/");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://discord.gg/phu3GNN/");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DisplayLocationSelector();
                text_MoveRadius.Text = ""+Globals.radius;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Write(ex.Message);
            }
        }

        private void DisplayLocationSelector()
        {
            LocationSelect locationSelector = new LocationSelect(false);
            locationSelector.ShowDialog();
            text_Latidude.Text = Globals.latitute.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = Globals.longitude.ToString(CultureInfo.InvariantCulture);
            text_Altidude.Text = Globals.altitude.ToString(CultureInfo.InvariantCulture);
        }

        private void TextBoxes_Items_TextChanged(object sender, EventArgs e)
        {
            int itemSumme = 0;

            if (text_MaxPokeballs.Text != string.Empty && text_MaxGreatBalls.Text != string.Empty && text_MaxUltraBalls.Text != string.Empty && text_MaxRevives.Text != string.Empty && text_MaxPotions.Text != string.Empty && text_MaxSuperPotions.Text != string.Empty && text_MaxHyperPotions.Text != string.Empty && text_MaxRazzBerrys.Text != string.Empty && text_MaxTopPotions.Text != string.Empty && text_MaxTopRevives.Text != string.Empty)
            {
                itemSumme = Convert.ToInt16(text_MaxPokeballs.Text) +
                            Convert.ToInt16(text_MaxGreatBalls.Text) +
                            Convert.ToInt16(text_MaxUltraBalls.Text) +
                            Convert.ToInt16(text_MaxRevives.Text) +
                            Convert.ToInt16(text_MaxPotions.Text) +
                            Convert.ToInt16(text_MaxSuperPotions.Text) +
                            Convert.ToInt16(text_MaxHyperPotions.Text) +
                            Convert.ToInt16(text_MaxRazzBerrys.Text) +
                            Convert.ToInt16(text_MaxTopRevives.Text) +
                            Convert.ToInt16(text_MaxTopPotions.Text);
            }

            text_TotalItemCount.Text = Convert.ToString(itemSumme);
        }

        private void TextBoxes_Throws_TextChanged(object sender, EventArgs e)
        {
            if (!Globals.FirstLoad)
            {
                int throwsChanceSum = 0;

                if (text_Pb_Excellent.Text != string.Empty && text_Pb_Great.Text != string.Empty && text_Pb_Nice.Text != string.Empty && text_Pb_Ordinary.Text != string.Empty)
                {
                    throwsChanceSum = Convert.ToInt16(text_Pb_Excellent.Text) +
                                      Convert.ToInt16(text_Pb_Great.Text) +
                                      Convert.ToInt16(text_Pb_Nice.Text) +
                                      Convert.ToInt16(text_Pb_Ordinary.Text);
                }
                if (throwsChanceSum > 100)
                {
                    MessageBox.Show("You can not have a total throw chance greater than 100%.\nResetting all throw chances to 25%!");
                    text_Pb_Excellent.Text = "25";
                    text_Pb_Great.Text = "25";
                    text_Pb_Nice.Text = "25";
                    text_Pb_Ordinary.Text = "25";
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RUNUBQEANCAGQ");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://high-minded.net/threads/pokemon-go-c-bot-safer-better.50731/");
        }

        private string langSelected = "en";
        private void load_lang()
        {
            //TranslationHandler.getString("username", "Username :");

            Globals.acc = comboBox_AccountType.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;
            if (comboBox_AccountType.SelectedIndex == 0)
            {
                label2.Text = "E-Mail:";
            }
            else
            {
                label2.Text = TranslationHandler.GetString("username", "Username :");
            }

            label1.Text = TranslationHandler.GetString("accountType", "Account Type:");
            label3.Text = TranslationHandler.GetString("password", "Password:");
            groupBox2.Text = TranslationHandler.GetString("locationSettings", "Location Settings");
            label7.Text = TranslationHandler.GetString("speed", "Speed:");
            label9.Text = TranslationHandler.GetString("moveRadius", "Move Radius:");
            checkBox_Start_Walk_from_default_location.Text = TranslationHandler.GetString("startFromDefaultLocation", "Start from default location");
            groupBox3.Text = TranslationHandler.GetString("botSettings", "Bot Settings");
            checkBox_AutoTransferDoublePokemon.Text = TranslationHandler.GetString("autoTransferDoublePokemon", "Auto transfer double Pokemons");
            label11.Text = TranslationHandler.GetString("maxDupPokemon", "Max. duplicate Pokemons");
            label12.Text = TranslationHandler.GetString("maxCPtransfer", "Max. CP to transfer:");
            label28.Text = TranslationHandler.GetString("maxIVtransfer", "Max. IV to transfer:");
            label13.Text = TranslationHandler.GetString("maxPokeballs", "Max. Pokeballs:");
            label14.Text = TranslationHandler.GetString("maxGreatballs", "Max. GreatBalls:");
            label15.Text = TranslationHandler.GetString("maxUltraballs", "Max. UltraBalls:");
            label16.Text = TranslationHandler.GetString("maxRevives", "Max. Revives:");
            label27.Text = TranslationHandler.GetString("maxTopRevives", "Max. TopRevives:");
            label17.Text = TranslationHandler.GetString("maxPotions", "Max. Potions:");
            label18.Text = TranslationHandler.GetString("maxSuperpotions", "Max. SuperPotions:");
            label19.Text = TranslationHandler.GetString("maxHyperpotions", "Max. HyperPotions:");
            label25.Text = TranslationHandler.GetString("maxToppotions", "Max. TopPotions:");
            label20.Text = TranslationHandler.GetString("maxRazzberrys", "Max. RazzBerrys:");
            label31.Text = TranslationHandler.GetString("totalCount", "Total Count:");
            label48.Text = TranslationHandler.GetString("excellentChance", "Excellent chance:");
            label49.Text = TranslationHandler.GetString("greatChance", "Great chance:");
            label50.Text = TranslationHandler.GetString("niceChance", "Nice chance:");
            label51.Text = TranslationHandler.GetString("ordinaryChance", "Ordinary chance:");
            groupBox5.Text = TranslationHandler.GetString("pokemonNotToTransfer", "Pokemons - Not to transfer");
            checkBox4.Text = TranslationHandler.GetString("selectAll", "Select all");
            groupBox6.Text = TranslationHandler.GetString("pokemonNotToCatch", "Pokemons - Not to catch");
            checkBox5.Text = TranslationHandler.GetString("selectAll", "Select all");
            groupBox7.Text = TranslationHandler.GetString("pokemonNotToEvolve", "Pokemons - To envolve");
            checkBox6.Text = TranslationHandler.GetString("selectAll", "Select all");
            button1.Text = TranslationHandler.GetString("saveConfig", "Save Configuration / Start Bot");
            groupBox10.Text = TranslationHandler.GetString("otherSettings", "Other Settings");
            checkBox_UseLuckyEggAtEvolve.Text = TranslationHandler.GetString("useLuckyeggAtEvolve", "Use LuckyEgg at Evolve");
            checkBox_UseRazzberryIfChanceUnder.Text = TranslationHandler.GetString("useRazzBerry", "Use RazzBerry");
            checkBox_UseIncenseEvery30min.Text = TranslationHandler.GetString("useIncese", "Use Incense every 30min");
            checkBox_EvolvePokemonIfEnoughCandy.Text = TranslationHandler.GetString("evolvePokemonIfEnoughCandy", "Evolve Pokemons if enough candy");
            checkBox_EnablePokemonListGui.Text = TranslationHandler.GetString("enablePokemonListGUI", "Enable Pokemon List GUI");
            checkBox_KeepPokemonWhichCanBeEvolved.Text = TranslationHandler.GetString("keepPokemonWhichCanBeEvolved", "Keep Pokemons which can be evolved");
            checkBox_AutoIncubate.Text = TranslationHandler.GetString("autoIncubate", "Auto incubate");
            checkBox_2kmEggs.Text = TranslationHandler.GetString("2kmEggs", "2 Km");
            checkBox_5kmEggs.Text = TranslationHandler.GetString("5kmEggs", "5 Km");
            checkBox_10kmEggs.Text = TranslationHandler.GetString("10kmEggs", "10 Km");
            rbSOEggsAscending.Text = TranslationHandler.GetString("AscendingEggs", "Ascending (2 Km first)"); 
            rbSOEggsDescending.Text = TranslationHandler.GetString("DescendingEggs", "Descending (10 Km first)"); 
            checkBox_2kmEggsBasicInc.Text = TranslationHandler.GetString("2kmEggs", "2 Km");
            checkBox_5kmEggsBasicInc.Text = TranslationHandler.GetString("5kmEggs", "5 Km");
            checkBox_10kmEggsBasicInc.Text = TranslationHandler.GetString("10kmEggs", "10 Km");
            rbSOEggsAscendingBasicInc.Text = TranslationHandler.GetString("AscendingEggs", "Ascending (2 Km first)"); 
            rbSOEggsDescendingBasicInc.Text = TranslationHandler.GetString("DescendingEggs", "Descending (10 Km first)"); 
            checkBox_UseBasicIncubators.Text = TranslationHandler.GetString("useBasicIncubators", "Use basic incubators");
            checkbox_PWDEncryption.Text = TranslationHandler.GetString("pwdEncryption", "Encrypt password on config file");
        }

        private void languages_btn_Click(object sender, EventArgs e)
        {
            var clicked = (Button)sender;
            Button[] languageButtons = {
                lang_en_btn,
                lang_spain_btn,
                lang_de_btn,
                lang_ptBR_btn,
                lang_tr_btn,
                lang_ru_btn,
                lang_france_btn

                // add the new languages' buttons here
            };

            if (clicked != null)
            {
                // I have used the tag field of the button to save the language key
                langSelected = (string)clicked.Tag;
                if (!string.IsNullOrWhiteSpace(langSelected))
                {
                    if (langSelected == "en")
                        TranslationHandler.SelectLangauge(null);
                    else
                        TranslationHandler.SelectLangauge(langSelected);
                    load_lang();
                }
                else
                {
                    throw new MissingFieldException("Every language buttons needs to have as Tag field the language key");
                }
            }
        }

        public static void Extract(string nameSpace, string outDir, string internalFilePath, string resourceName)
        {
            Assembly ass = Assembly.GetCallingAssembly();

            if (File.Exists(outDir + "\\" + resourceName))
            {
                File.Delete(outDir + "\\" + resourceName);
            }
            //Logger.ColoredConsoleWrite(ConsoleColor.Red, ass.GetName().ToString());

            using (var s = ass.GetManifestResourceStream(nameSpace + "." + (internalFilePath == string.Empty ? string.Empty : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDir + "\\" + resourceName, FileMode.OpenOrCreate))
            using (BinaryWriter w = new BinaryWriter(fs))
                w.Write(r.ReadBytes((int)s.Length));
        }
        // Code cleanup we can do later
        public class ExtendedWebClient : WebClient
        {

            private int timeout;
            public int Timeout
            {
                get
                {
                    return timeout;
                }
                set
                {
                    timeout = value;
                }
            }
            public ExtendedWebClient()
            {
                this.timeout = 2000;//In Milli seconds 
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.timeout;
                return objWebRequest;
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitter.com/Ar1iDev");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitter.com/MattKnight4355");
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxy.Checked)
            {
                button1.Enabled = false;
                prxyIP.Enabled = true;
                prxyPort.Enabled = true;
                //UserSettings.Default.UseProxyVerified = true;
            }
            else
            {
                button1.Enabled = true;
                prxyIP.Enabled = false;
                prxyPort.Enabled = false;
                //UserSettings.Default.UseProxyVerified = false;
            }

        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxyAuth.Checked)
            {
                prxyUser.Enabled = true;
                prxyPass.Enabled = true;
            }
            else
            {
                prxyUser.Enabled = false;
                prxyPass.Enabled = false;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _clientSettings.UseProxyHost = string.Empty;
            _clientSettings.UseProxyPort = 0;
            _clientSettings.UseProxyUsername = string.Empty;
            _clientSettings.UseProxyVerified = false;
            _clientSettings.UseProxyAuthentication = false;
        }

        private void prxy_GotFocus(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "HTTPS Proxy IP")
            {
                tb.Text = "";
            }
            else if (tb.Text == "HTTPS Proxy Port")
            {
                tb.Text = "";
            }
            else if (tb.Text == "Proxy Username")
            {
                tb.Text = "";
            }
            else if (tb.Text == "Proxy Password")
            {
                tb.Text = "";
            }
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://proxylist.hidemyass.com/search-1297445#listable");
        }

        private void comboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] arrLine = File.ReadAllLines(deviceinfo);
                arrLine[0] = comboBox_Device.SelectedItem.ToString();
                File.WriteAllLines(deviceinfo, arrLine);
            }
            catch (IndexOutOfRangeException)
            {
                File.WriteAllLines(deviceinfo, new string[] { comboBox_Device.SelectedItem.ToString(), " " });
            }
        }


        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://developers.google.com/maps/documentation/directions/start#get-a-key/");
            Process.Start(sInfo);
        }


        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private void checkPrxy_Click(object sender, EventArgs e)
        {
            string proxyip = prxyIP.Text;
            int port = Convert.ToInt32(prxyPort.Text);

            _clientSettings.UseProxyHost = prxyIP.Text;
            _clientSettings.UseProxyPort = port;

            if (checkBox_UseProxyAuth.Checked)
            {
                _clientSettings.UseProxyUsername = prxyUser.Text;
                _clientSettings.UseProxyPassword = prxyPass.Text;
                _clientSettings.UseProxyAuthentication = true;
            }
            _clientSettings.UseProxyVerified = true;
            button1.Enabled = true;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Globals.evolve = checkBox3.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This will capture pokemons while walking spiral, and will use pokestops which are within 30 meters of the path projected.");
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            Form update = new Update();
            this.Hide();
            update.Show();
        }


        private void button2_Click_2(object sender, EventArgs e)
        {
            if (Save())
                MessageBox.Show("Current Configuration Saved as - " + ActiveProfile.ProfileName);
            else
                MessageBox.Show("Please Review Red Boxes Before Save");
        }

        private void ProfileName_TextChanged(object sender, EventArgs e)
        {
            Globals.ProfileName = ProfileName.Text;
            ActiveProfile.ProfileName = ProfileName.Text;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Globals.IsDefault = true;
            ActiveProfile.IsDefault = true;
            foreach (Profile x in Globals.Profiles)
            {
                if (x.ProfileName != Globals.ProfileName)
                {
                    x.IsDefault = false;
                }
                else
                {
                    x.IsDefault = true;
                }
            }
        }

        private void linkLabel2_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Ar1i/");
        }

        private void linkLabel9_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Logxn/");
        }

        private void linkLabel10_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/MTK4355/");
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {

        }
        void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            ((TextBox) sender).BackColor = SystemColors.Window;
        }

        private void tabGeneral_Click(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {

        }
    }
}
