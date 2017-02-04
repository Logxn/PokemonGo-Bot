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
using System.Windows.Forms;

using PokemonGo.RocketAPI.Logic.Translation;
using POGOProtos.Enums;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using PokemonGo.RocketAPI.Logic.Shared;
using PokemonGo.RocketAPI.HttpClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PokemonGo.RocketAPI.Console
{
    public partial class GUI : System.Windows.Forms.Form
    {
        public static Collection<Profile> Profiles = new Collection<Profile>();
        
        public static NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        public static int[] evolveBlacklist = {
            3, 6, 9, 12, 15, 18, 20, 22, 24, 26, 28, 31, 34, 36, 38, 40, 42, 45, 47, 49, 51, 53, 55, 57, 59, 62, 65, 68, 71, 73, 76, 78, 80, 82, 83, 85, 87, 89, 91, 94, 95, 97, 99, 101, 103, 105, 106, 107, 108, 110, 112, 113, 114, 115, 117, 119, 121, 122, 123, 124, 125, 126, 127, 128, 130, 131, 132, 134, 135, 136, 137, 139, 141, 142, 143, 144, 145, 146, 149, 150, 151
        };

        /* PATHS */
        static string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static string devicePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        static string deviceinfo = Path.Combine(devicePath, "DeviceInfo.txt");
        static string PokeDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PokeData");
        static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        static string logs = Path.Combine(logPath, "PokeLog.txt");
        static string logmanualtransfer = Path.Combine(logPath, "TransferLog.txt");
        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        static Dictionary<string, int> evolveIDS = new Dictionary<string, int>();
        static string ConfigsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        private Profile ActiveProfile;
        
        public Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public GUI()
        {
            InitializeComponent();
        }
        
        public double StrCordToDouble(string str)
        {
            double ret = 0.0;
            double.TryParse(str.Replace(",","."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture,out ret);
            return ret;
        }

        private void GUI_Load(object sender, EventArgs e)
        {

            if (File.Exists(@"{baseDirectory}\update.bat"))
                File.Delete(@"{baseDirectory}\update.bat");

            comboBox_AccountType.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };

            comboBox_AccountType.DataSource = types;


            if (!Directory.Exists(Program.path_device))
                Directory.CreateDirectory(Program.path_device);
            
            if (File.Exists(deviceinfo))
            {
                string[] arrLine = File.ReadAllLines(deviceinfo);
                if (arrLine[0] != null)
                {
                    comboBox_Device.Text = arrLine[0];
                }
            }

            #region new translation
            if (!Directory.Exists(Program.path_translation))
                Directory.CreateDirectory(Program.path_translation);

            comboLanguage.SelectedIndex = 0;
            // Download json file of current Culture Info if exists
            Helper.TranslatorHelper.DownloadTranslationFile("PokemonGo.RocketAPI.Console/Lang", Program.path_translation, CultureInfo.CurrentCulture.Name);
            // Translate using Current Culture Info
            th.Translate(this);
            #endregion

            comboBoxLeaveInGyms.DataSource = new[]{
                th.TS("Random"),
                th.TS("Best CP"),
                th.TS("Worse CP"),
            };
            comboBoxLeaveInGyms.SelectedIndex = 0;

            var pokeData = new List<string>();
            pokeData.Add("AdditionalPokeData.json");

            if (!Directory.Exists(Program.path_pokedata))
                Directory.CreateDirectory(Program.path_pokedata);

            foreach (var extract in pokeData)
                Extract("PokemonGo.RocketAPI.Console", Program.path_pokedata, "PokeData", extract);

            int i = 1;
            int ev = 1;

            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon != PokemonId.Missingno)
                {
                    pokeIDS[pokemon.ToString()] = i;
                    checkedListBox_PokemonNotToTransfer.Items.Add(th.TS(pokemon.ToString()));
                    checkedListBox_AlwaysTransfer.Items.Add(th.TS(pokemon.ToString()));
                    checkedListBox_PokemonNotToCatch.Items.Add(th.TS(pokemon.ToString()));
                    if (!(evolveBlacklist.Contains(i)))
                    {
                        checkedListBox_PokemonToEvolve.Items.Add(th.TS(pokemon.ToString()));
                        evolveIDS[pokemon.ToString()] = ev;
                        ev++;
                    }
                    checkedListBox_NotToSnipe.Items.Add(th.TS(pokemon.ToString()));
                    i++;
                }
            }

            #region Loading Everything into GUI 

            Profiles = new Collection<Profile>();
            ActiveProfile = new Profile();
            ActiveProfile.ProfileName = "DefaultProfile";
            ActiveProfile.RunOrder = 1;
            ActiveProfile.Settings = new ProfileSettings();
            UpdateActiveProf(false);
            var foundDefaultProfile = false;
            var filenameProf ="";
            Profile selectedProfile = null;
            var result = "";
            if (File.Exists(Program.accountProfiles))
            {
                string JSONstring = File.ReadAllText(@Program.accountProfiles);
                Profiles = JsonConvert.DeserializeObject<Collection<Profile>>(JSONstring);
                foreach (Profile _profile in Profiles)
                {
                    if (_profile.IsDefault)
                        selectedProfile = _profile;
                    if (_profile.ProfileName == "DefaultProfile")
                         foundDefaultProfile = true;

                    filenameProf= Path.Combine(ConfigsPath, _profile.ProfileName +".json" );
                    if (File.Exists(filenameProf))
                    {
                        try
                        {
                            _profile.Settings = ProfileSettings.LoadFromFile( filenameProf);
                        }
                        catch (Exception)
                        {
                            result += filenameProf+"\n";
                        }
                    }
                    else
                    {
                        if (_profile.SettingsJSON!="")
                        {
                            try
                            {
                                _profile.Settings = ProfileSettings.LoadFromStringJSON( _profile.SettingsJSON);
                                _profile.SettingsJSON="";
                            }
                            catch (Exception)
                            {
                                if (_profile.ProfileName != "DefaultProfile")
                                    result += filenameProf+"\n";
                            }
                        
                        }
                    }
                    
                }
            }
            if (!foundDefaultProfile)
                Profiles.Add(ActiveProfile);

            ProfileSelect.DataSource = Profiles;
            ProfileSelect.DisplayMember = "ProfileName";
            if (selectedProfile != null)
            {
                  ActiveProfile.IsDefault = false;
                  LoadData(selectedProfile.Settings);
                  checkBoxDefaultProf.Checked = selectedProfile.IsDefault;
                  ProfileSelect.SelectedItem = selectedProfile;
                  ProfileName.Text = selectedProfile.ProfileName;
            }
            else
            {
                  ProfileSelect.SelectedItem = ActiveProfile;
                  ProfileName.Text = ActiveProfile.ProfileName;
            }

            if (result!="")
            {
                var message = th.TS("Loading Config failed\n{0} Check settings before running!",result);
                 MessageBox.Show(message);
            }

            ///* VERSION INFORMATION */
            var currVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var newestVersion = Program.getNewestVersion();

            currVer.Text = currVersion.ToString();
            newVer.Text = newestVersion.ToString();

            if (Program.getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
            {
                if (checkbox_AutoUpdate.Checked)
                {
                    System.Windows.Forms.Form update = new Update();
                    update.ShowDialog();
                }
                else
                {
                    var message = th.TS ("There is an Update on Github. do you want to open it ?");
                    var title = th.TS ("Newest Version: {0}",newestVersion);
                    DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        Process.Start("https://github.com/Logxn/PokemonGo-Bot");
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

        private void LoadData(ProfileSettings config)
        {   
            if (config == null)
                return;
            
            // tab 1 
            pFHashKey.Text = config.pFHashKey;
    
            comboBox_AccountType.SelectedIndex = 1;
            if (config.AuthType == Enums.AuthType.Google)
            {
                comboBox_AccountType.SelectedIndex = 0;
            }
            text_EMail.Text = config.Username;
            text_Password.Text = config.Password;
            checkbox_PWDEncryption.Checked = config.UsePwdEncryption;
            if (config.UsePwdEncryption )
            {
                text_Password.Text = Encryption.Decrypt(config.Password);
            }
    
            text_Latidude.Text = config.DefaultLatitude.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = config.DefaultLongitude.ToString(CultureInfo.InvariantCulture);
            text_Altitude.Text = config.DefaultAltitude.ToString(CultureInfo.InvariantCulture);
    
            checkBox_UseLuckyEggAtEvolve.Checked = config.UseLuckyEgg;
            checkBox_SimulateAnimationTimeAtEvolve.Checked = config.UseAnimationTimes;
            checkBox_EvolvePokemonIfEnoughCandy.Checked = config.EvolvePokemonsIfEnoughCandy;
            checkBox_UseIncenseEvery30min.Checked = config.UseIncense;
            checkBox_EnablePokemonListGui.Checked = config.EnablePokeList;
            checkBox_ConsoleInTab.Checked = config.EnableConsoleInTab;
            CB_SimulatePGO.Checked = config.simulatedPGO;
            checkBox_KeepPokemonWhichCanBeEvolved.Checked = config.keepPokemonsThatCanEvolve;
            checkBox_UseLuckyEggIfNotRunning.Checked = config.UseLuckyEggIfNotRunning;
            checkBox_FarmGyms.Checked = config.FarmGyms;
            checkBox_CollectDailyBonus.Checked = config.CollectDailyBonus;
    
            // tab 2 - Pokemons
            if (config.pokemonsToHold != null)
                foreach (PokemonId Id in config.pokemonsToHold)
                {
                    string _id = Id.ToString();
                    checkedListBox_PokemonNotToTransfer.SetItemChecked(pokeIDS[_id] - 1, true);
                }
            if (config.pokemonsToAlwaysTransfer != null)
                foreach (PokemonId Id in config.pokemonsToAlwaysTransfer)
                {
                    string _id = Id.ToString();
                    checkedListBox_AlwaysTransfer.SetItemChecked(pokeIDS[_id] - 1, true);
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
            checkBox_UseSpritesFolder.Checked = config.UseSpritesFolder;

            // tab 3 - throws
            checkBox2.Checked = config.LimitPokeballUse;
            checkBox3.Checked = config.LimitGreatballUse;
            checkBox7.Checked = config.LimitUltraballUse;
            numericUpDown1.Value = config.Max_Missed_throws;
            numericUpDown2.Value = config.InventoryBasePokeball;
            numericUpDown3.Value = config.InventoryBaseGreatball;
            numericUpDown4.Value = config.InventoryBaseUltraball;
    
            checkBox_UseRazzberryIfChanceUnder.Checked = config.UseRazzBerry;
            text_UseRazzberryChance.Text = (config.razzberry_chance * 100).ToString();
            NextBestBallOnEscape.Checked = config.NextBestBallOnEscape;

            // To avoid first calculation of 100 %
            text_Pb_Excellent.Value = 0;
            text_Pb_Great.Value = 0;
            text_Pb_Nice.Value = 0;
            text_Pb_Ordinary.Value = 0;
            text_Pb_Excellent.Value = config.excellentthrow;
            text_Pb_Great.Value = config.greatthrow;
            text_Pb_Nice.Value = config.nicethrow;
            text_Pb_Ordinary.Value = config.ordinarythrow;

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
            if (config.proxySettings !=null){
                checkBox_UseProxy.Checked = config.proxySettings.enabled;
                checkBox_UseProxyAuth.Checked = config.proxySettings.useAuth;
                prxyIP.Text = config.proxySettings.hostName;
                prxyPort.Text =""+ config.proxySettings.port;
                prxyUser.Text = config.proxySettings.username;
                prxyPass.Text = config.proxySettings.password;
            }

            // tab 6 walk
            text_Speed.Text = config.WalkingSpeedInKilometerPerHour.ToString();
            text_MinWalkSpeed.Text = config.MinWalkSpeed.ToString();
            text_MoveRadius.Text = config.MaxWalkingRadiusInMeters.ToString();
            text_TimeToRun.Text = config.TimeToRun.ToString();
            text_RestartAfterRun.Text = config.RestartAfterRun.ToString();
    
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
            checkBox_StartWalkingFromLastLocation.Checked = config.UseLastCords;
    
            // tab 7 - telegram and logs
            cbLogPokemon.Checked = config.LogPokemons;
            cbLogManuelTransfer.Checked = config.LogTransfer;
            cbLogEvolution.Checked = config.LogEvolve;
            cbLogEggs.Checked = config.LogEggs;
    
            text_Telegram_Token.Text = config.TelegramAPIToken;
            text_Telegram_Name.Text = config.TelegramName;
            text_Telegram_LiveStatsDelay.Text = config.TelegramLiveStatsDelay.ToString();
    
            SnipePokemonPokeCom.Checked = config.SnipePokemon;
            AvoidRegionLock.Checked = config.AvoidRegionLock;
    
            // tab 8 - update
            checkbox_AutoUpdate.Checked = config.AutoUpdate;
            checkbox_checkWhileRunning.Checked = config.CheckWhileRunning;
            ChangeSelectedLanguage(config.SelectedLanguage);
    
            // Dev Options
            checkbox_Verboselogging.Checked = config.EnableVerboseLogging;
            // Gyms
            comboBoxLeaveInGyms.SelectedIndex = config.LeaveInGyms;
        }
        
        private void ChangeSelectedLanguage(string lang)
        {
            var index = 0;
            if (lang == "Default")
                index =1;
            if (lang == "Deutsche")
                index =2;
            if (lang == "Español")
                index =3;
            if (lang == "Catalá")
                index =4;
            comboLanguage.SelectedIndex = index;
        }
        private void LoadLatestCoords()
        {
            var coordLoaded = false;
            if (File.Exists(Program.lastcords))
            {
                var latlngFromFile = File.ReadAllText(Program.lastcords);
                var latlng = latlngFromFile.Split(':');
                if (latlng.Length > 1)
                {
                    var lat = StrCordToDouble(latlng[0]);
                    var lng = StrCordToDouble(latlng[1]);
                    if (( lat!=0.0) && (lng != 0.0))
                    {
                        ActiveProfile.Settings.DefaultLatitude = lat;
                        ActiveProfile.Settings.DefaultLongitude = lng ;
                        coordLoaded = true;
                    }
                }
            }
            if (!coordLoaded){
                Logger.Warning(th.TS("Failed loading last coords!"));
                Logger.Warning(th.TS("Using default location"));
            }
        }
        //Account Type Changed Event
        private void comboAccType_SelectedIndexChanged(object sender, EventArgs e)
        {            
            if (comboBox_AccountType.SelectedIndex == 0)
                label2.Text = "E-Mail:";
            else
                label2.Text = th.TS("User Name:");
        }

        private void ProfileSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedProfile = (Profile) Profiles.FirstOrDefault(i => i == ProfileSelect.SelectedItem);
            LoadData(selectedProfile.Settings);
            ProfileName.Text = selectedProfile.ProfileName;
            checkBoxDefaultProf.Checked = selectedProfile.IsDefault;
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
        private void buttonSaveStart_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                if (ActiveProfile.Settings.UseLastCords)
                    LoadLatestCoords();

                var selectedCoords =ActiveProfile.Settings.DefaultLatitude.ToString("0.000000") +";"+ ActiveProfile.Settings.DefaultLongitude.ToString("0.000000");
                selectedCoords = selectedCoords.Replace(",",".");
                if (selectedCoords.Equals(NEW_YORK_COORS))
                {
                    var ret = MessageBox.Show(th.TS("Have you set correctly your location? (It seems like you are using default coords. This can lead to an auto-ban from niantic)"), th.TS("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (ret == DialogResult.No)
                    {
                        return;
                    }
                }
                
                // TODO: Make this decyption at end of comuncation
                if (ActiveProfile.Settings.UsePwdEncryption)
                    ActiveProfile.Settings.Password = Encryption.Decrypt(ActiveProfile.Settings.Password);

                GlobalVars.Assign(ActiveProfile.Settings);

                Dispose();
            }else
                    MessageBox.Show(th.TS("Please Review Red Boxes Before Start"));

        }

        private bool textBoxToActiveProf(TextBox textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty)
            {
                if (fieldName == string.Empty)
                {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                typeof(ProfileSettings).GetProperty(fieldName).SetValue(ActiveProfile.Settings, textBox.Text);
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }
        private bool textBoxToActiveProfDouble(TextBox textBox, string fieldName = "")
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
                var valueDBL = double.Parse(valueTXT, cords, CultureInfo.InvariantCulture);
                typeof(ProfileSettings).GetProperty(fieldName).SetValue(ActiveProfile.Settings, valueDBL);
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool textBoxToActiveProfInt(TextBox textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty)
            {
                if (fieldName == string.Empty)
                {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                var intVal = int.Parse(textBox.Text);
                var field = typeof(ProfileSettings).GetProperty(fieldName);
                field.SetValue(ActiveProfile.Settings, intVal);
            }
            else
            {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool UpdateActiveProf(bool makePrompts=true)
        {
            #region Setting all the globals

            ActiveProfile.IsDefault = checkBoxDefaultProf.Checked;
            // tab 1 - General
            
            ActiveProfile.Settings.AuthType = (comboBox_AccountType.SelectedIndex == 0) ? Enums.AuthType.Google : Enums.AuthType.Ptc;

            // Account Info
            bool ret = true;
            ret &= textBoxToActiveProf(pFHashKey, "pFHashKey");
            
            ret &= textBoxToActiveProf(text_EMail,"Username");
            ret &= textBoxToActiveProf(text_Password,"Password");
            ActiveProfile.Settings.UsePwdEncryption = checkbox_PWDEncryption.Checked;

            // Location
            ret &= textBoxToActiveProfDouble(text_Latidude, "DefaultLatitude");
            ret &= textBoxToActiveProfDouble(text_Longitude, "DefaultLongitude");
            ret &= textBoxToActiveProfDouble(text_Altitude, "DefaultAltitude");

            // Other
            ActiveProfile.Settings.UseLuckyEgg = checkBox_UseLuckyEggAtEvolve.Checked;
            ActiveProfile.Settings.UseAnimationTimes = checkBox_SimulateAnimationTimeAtEvolve.Checked;
            ActiveProfile.Settings.EvolvePokemonsIfEnoughCandy = checkBox_EvolvePokemonIfEnoughCandy.Checked;
            ActiveProfile.Settings.UseIncense = checkBox_UseIncenseEvery30min.Checked;
            ActiveProfile.Settings.EnablePokeList = checkBox_EnablePokemonListGui.Checked;
            ActiveProfile.Settings.EnableConsoleInTab = checkBox_ConsoleInTab.Checked;
            ActiveProfile.Settings.simulatedPGO = CB_SimulatePGO.Checked;
            ActiveProfile.Settings.keepPokemonsThatCanEvolve = checkBox_KeepPokemonWhichCanBeEvolved.Checked;
            ActiveProfile.Settings.UseLuckyEggIfNotRunning = checkBox_UseLuckyEggIfNotRunning.Checked;
            ActiveProfile.Settings.FarmGyms = checkBox_FarmGyms.Checked;
            ActiveProfile.Settings.CollectDailyBonus = checkBox_CollectDailyBonus.Checked;

            // tab 2 - pokemons
            ActiveProfile.Settings.pokemonsToHold.Clear();
            ActiveProfile.Settings.catchPokemonSkipList.Clear();
            ActiveProfile.Settings.pokemonsToEvolve.Clear();
            ActiveProfile.Settings.NotToSnipe.Clear();

            foreach (string pokemon in checkedListBox_PokemonNotToTransfer.CheckedItems)
            {
                ActiveProfile.Settings.pokemonsToHold.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            foreach (string pokemon in checkedListBox_AlwaysTransfer.CheckedItems)
            {
                ActiveProfile.Settings.pokemonsToAlwaysTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            
            foreach (string pokemon in checkedListBox_PokemonNotToCatch.CheckedItems)
            {
                ActiveProfile.Settings.catchPokemonSkipList.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            foreach (string pokemon in checkedListBox_PokemonToEvolve.CheckedItems)
            {
                ActiveProfile.Settings.pokemonsToEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            foreach (string pokemon in checkedListBox_NotToSnipe.CheckedItems)
            {
                ActiveProfile.Settings.NotToSnipe.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            ActiveProfile.Settings.UseSpritesFolder = checkBox_UseSpritesFolder.Checked;
            // bot settings
            ActiveProfile.Settings.TransferDoublePokemons = checkBox_AutoTransferDoublePokemon.Checked;
            ActiveProfile.Settings.TransferFirstLowIV = checkBox_TransferFirstLowIV.Checked;

            ret &= textBoxToActiveProfInt(text_MaxDuplicatePokemon, "HoldMaxDoublePokemons");

            ret &= textBoxToActiveProfInt(text_MaxIVToTransfer, "ivmaxpercent");
                        
            ret &= textBoxToActiveProfInt(text_MaxCPToTransfer, "DontTransferWithCPOver");

            // tab 3 - Throw
            ActiveProfile.Settings.LimitPokeballUse = checkBox2.Checked;
            ActiveProfile.Settings.LimitGreatballUse = checkBox3.Checked;
            ActiveProfile.Settings.LimitUltraballUse = checkBox7.Checked;
            ActiveProfile.Settings.Max_Missed_throws = (int)numericUpDown1.Value;
            ActiveProfile.Settings.InventoryBasePokeball = (int)numericUpDown2.Value;
            ActiveProfile.Settings.InventoryBaseGreatball = (int)numericUpDown3.Value;
            ActiveProfile.Settings.InventoryBaseUltraball = (int)numericUpDown4.Value;

            ActiveProfile.Settings.UseRazzBerry = checkBox_UseRazzberryIfChanceUnder.Checked;
            if (text_UseRazzberryChance.Text == string.Empty)
            {
                text_UseRazzberryChance.BackColor = Color.Red;
            }
            else
            {
                int x = int.Parse(text_UseRazzberryChance.Text);
                decimal c = ((decimal)x / 100);
                ActiveProfile.Settings.razzberry_chance = Convert.ToDouble(c);
            }

            
            ActiveProfile.Settings.excellentthrow = (int)text_Pb_Excellent.Value;
            ActiveProfile.Settings.greatthrow = (int)text_Pb_Great.Value;
            ActiveProfile.Settings.nicethrow = (int)text_Pb_Nice.Value;
            ActiveProfile.Settings.ordinarythrow = (int)text_Pb_Ordinary.Value;
            
            ret &= textBoxToActiveProfInt(GreatBallMinCP, "MinCPforGreatBall");
            ret &= textBoxToActiveProfInt(UltraBallMinCP, "MinCPforUltraBall");


            // tab 4 - Items
            ret &= textBoxToActiveProfInt(text_MaxPokeballs, "MaxPokeballs");
            ret &= textBoxToActiveProfInt(text_MaxGreatBalls, "MaxGreatballs");
            ret &= textBoxToActiveProfInt(text_MaxUltraBalls, "MaxUltraballs");
            ret &= textBoxToActiveProfInt(text_MaxRevives, "MaxRevives");
            ret &= textBoxToActiveProfInt(text_MaxTopRevives, "MaxTopRevives");
            ret &= textBoxToActiveProfInt(text_MaxPotions, "MaxPotions");
            ret &= textBoxToActiveProfInt(text_MaxSuperPotions, "MaxSuperPotions");
            ret &= textBoxToActiveProfInt(text_MaxHyperPotions, "MaxHyperPotions");
            ret &= textBoxToActiveProfInt(text_MaxTopPotions, "MaxTopPotions");
            ret &= textBoxToActiveProfInt(text_MaxRazzBerrys, "MaxBerries");
            ret &= textBoxToActiveProfInt(MinCPtoCatch, "MinCPtoCatch");
            ret &= textBoxToActiveProfInt(MinIVtoCatch, "MinIVtoCatch");

            // tab  - Eggs
            ActiveProfile.Settings.AutoIncubate = checkBox_AutoIncubate.Checked;
            ActiveProfile.Settings.UseBasicIncubators = checkBox_UseBasicIncubators.Checked;
            ActiveProfile.Settings.No2kmEggs = checkBox_2kmEggs.Checked;
            ActiveProfile.Settings.No5kmEggs = checkBox_5kmEggs.Checked;
            ActiveProfile.Settings.No10kmEggs = checkBox_10kmEggs.Checked;
            ActiveProfile.Settings.EggsAscendingSelection = rbSOEggsAscending.Checked;
            ActiveProfile.Settings.No2kmEggsBasicInc = checkBox_2kmEggsBasicInc.Checked;
            ActiveProfile.Settings.No5kmEggsBasicInc = checkBox_5kmEggsBasicInc.Checked;
            ActiveProfile.Settings.No10kmEggsBasicInc = checkBox_10kmEggsBasicInc.Checked;
            ActiveProfile.Settings.EggsAscendingSelectionBasicInc = rbSOEggsAscendingBasicInc.Checked;

            
            // tab  - Proxy
            ActiveProfile.Settings.proxySettings.enabled =checkBox_UseProxy.Checked;
            ActiveProfile.Settings.proxySettings.useAuth =checkBox_UseProxyAuth.Checked;
            ActiveProfile.Settings.proxySettings.hostName =prxyIP.Text;
            var intvalue = 0;
            int.TryParse(prxyPort.Text, out intvalue);
            ActiveProfile.Settings.proxySettings.port = intvalue;
            ActiveProfile.Settings.proxySettings.username =prxyUser.Text;
            ActiveProfile.Settings.proxySettings.password =prxyPass.Text;

            // tab 6 - Walk
            ret &= textBoxToActiveProfDouble(text_Speed,"WalkingSpeedInKilometerPerHour");
            
            if ((makePrompts) && (ActiveProfile.Settings.WalkingSpeedInKilometerPerHour > 15 ))
            {
                var speed = ActiveProfile.Settings.WalkingSpeedInKilometerPerHour;
                var message = th.TS("The risk of being banned is significantly greater when using higher than human jogging speeds (e.g. > 15km/hr) Click 'No' to use ~10km/hr instead");
                var title = th.TS("Are you sure you wish to set your speed to {0} ?",speed);
                var dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    ActiveProfile.Settings.WalkingSpeedInKilometerPerHour = double.Parse("9.5", cords, CultureInfo.InvariantCulture);
            }

            var value = text_MinWalkSpeed.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.MinWalkSpeed = int.Parse(value);

            ret &= textBoxToActiveProfInt(text_MoveRadius, "MaxWalkingRadiusInMeters");

            if (text_TimeToRun.Text == String.Empty)
                text_TimeToRun.Text = "0";
            ActiveProfile.Settings.TimeToRun = Double.Parse(text_TimeToRun.Text);

            if (text_RestartAfterRun.Text == String.Empty)
                text_RestartAfterRun.Text = "0";
            ActiveProfile.Settings.RestartAfterRun = int.Parse(text_RestartAfterRun.Text);


            value = text_PokemonCatchLimit.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.PokemonCatchLimit = int.Parse(value);

            value = text_PokestopFarmLimit.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.PokestopFarmLimit = int.Parse(value);

            value = text_XPFarmedLimit.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.XPFarmedLimit = int.Parse(value);

            value = text_BreakInterval.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.BreakInterval = int.Parse(value);

            value = text_BreakLength.Text;
            if (value != String.Empty)
                ActiveProfile.Settings.BreakLength = int.Parse(value);

            ActiveProfile.Settings.pauseAtEvolve = checkBox_StopWalkingWhenEvolving.Checked;
            ActiveProfile.Settings.pauseAtEvolve2 = checkBox_StopWalkingWhenEvolving.Checked;

            ActiveProfile.Settings.UseGoogleMapsAPI = checkBox_UseGoogleMapsRouting.Checked;
            ActiveProfile.Settings.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;

            ActiveProfile.Settings.sleepatpokemons = checkBox_RandomSleepAtCatching.Checked;
            ActiveProfile.Settings.FarmPokestops = checkBox_FarmPokestops.Checked;
            
            ActiveProfile.Settings.CatchPokemon = checkBox_CatchPokemon.Checked;
            ActiveProfile.Settings.BreakAtLure = checkBox_BreakAtLure.Checked;
            ActiveProfile.Settings.UseLureAtBreak = checkBox_UseLureAtBreak.Checked;
            ActiveProfile.Settings.RandomReduceSpeed = checkBox_RandomlyReduceSpeed.Checked;
            ActiveProfile.Settings.UseBreakFields = checkBox_UseBreakIntervalAndLength.Checked;

            ActiveProfile.Settings.Espiral = checkBox_WalkInArchimedeanSpiral.Checked;
            ActiveProfile.Settings.UseLastCords = checkBox_StartWalkingFromLastLocation.Checked;

            // tab 7 - Logs and Telegram            
            ActiveProfile.Settings.LogPokemons = cbLogPokemon.Checked;
            ActiveProfile.Settings.LogTransfer = cbLogManuelTransfer.Checked;
            ActiveProfile.Settings.LogEvolve = cbLogEvolution.Checked;
            ActiveProfile.Settings.LogEggs = cbLogEggs.Checked;

            ActiveProfile.Settings.TelegramAPIToken = text_Telegram_Token.Text;
            ActiveProfile.Settings.TelegramName = text_Telegram_Name.Text;
            ret &= textBoxToActiveProfInt(text_Telegram_LiveStatsDelay, "TelegramLiveStatsDelay");
            ActiveProfile.Settings.SnipePokemon = SnipePokemonPokeCom.Checked;
            if ((makePrompts) && (ActiveProfile.Settings.SnipePokemon ))
            {
                DialogResult result = MessageBox.Show(th.TS("Sniping has not been tested yet. It could get you banned. Do you want to continue?"), th.TS("Info"), MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                    ActiveProfile.Settings.SnipePokemon = true;
                else
                    ActiveProfile.Settings.SnipePokemon = false;
            }
            ActiveProfile.Settings.AvoidRegionLock = AvoidRegionLock.Checked;
            // tab 8 updates
            ActiveProfile.Settings.AutoUpdate = checkbox_AutoUpdate.Checked;
            ActiveProfile.Settings.CheckWhileRunning = checkbox_checkWhileRunning.Checked;
            ActiveProfile.Settings.NextBestBallOnEscape = NextBestBallOnEscape.Checked;
            ActiveProfile.Settings.SelectedLanguage = comboLanguage.Text;

            // dev options
            ActiveProfile.Settings.EnableVerboseLogging = checkbox_Verboselogging.Checked;

            // Gyms
            ActiveProfile.Settings.LeaveInGyms = comboBoxLeaveInGyms.SelectedIndex;

            #endregion
            return ret;
        }

        private bool Save()
        {
            if (  UpdateActiveProf(true)){
                if (ActiveProfile.Settings.UsePwdEncryption )
                {
                    ActiveProfile.Settings.Password = Encryption.Encrypt(ActiveProfile.Settings.Password);
                }
                var filenameProf= Path.Combine(ConfigsPath, ProfileName.Text +".json" );
                ActiveProfile.Settings.SaveToFile(filenameProf);
                var newProfiles = new Collection<Profile>();
                var foundActiveProf = false;
                foreach (Profile _profile in Profiles)
                {
                    var newProfile = new Profile();
                    newProfile.ProfileName = _profile.ProfileName;
                    newProfile.IsDefault = _profile.IsDefault;
                    if (ActiveProfile.IsDefault)
                        newProfile.IsDefault = false;
                    newProfile.RunOrder = _profile.RunOrder;
                    newProfile.SettingsJSON = _profile.SettingsJSON;
                    newProfile.Settings = null;
                    if (_profile.ProfileName == ProfileName.Text)
                    {
                        newProfile.IsDefault = ActiveProfile.IsDefault;
                        newProfile.RunOrder = ActiveProfile.RunOrder;
                        newProfile.SettingsJSON = "";
                        foundActiveProf = true;
                    }
                    newProfiles.Add(newProfile);
                }
                if (!foundActiveProf)
                {
                    var newProfile = new Profile();
                    newProfile.ProfileName = ProfileName.Text;
                    newProfile.IsDefault = ActiveProfile.IsDefault;
                    newProfile.RunOrder = ActiveProfile.RunOrder;
                    newProfile.SettingsJSON = "";
                    newProfile.Settings = null;
                    newProfiles.Add(newProfile);
                }
                var profileJSON = JsonConvert.SerializeObject(newProfiles,Formatting.Indented);
                File.WriteAllText(@Program.accountProfiles, profileJSON);
                if (!foundActiveProf)
                {
                    var newProfile = new Profile();
                    newProfile.ProfileName = ProfileName.Text;
                    newProfile.IsDefault = ActiveProfile.IsDefault;
                    newProfile.RunOrder = ActiveProfile.RunOrder;
                    newProfile.SettingsJSON = "";
                    newProfile.Settings = ProfileSettings.LoadFromFile(filenameProf);
                    Profiles.Add(newProfile);
                }
                else{
                    getProfileByName(ProfileName.Text).Settings = ProfileSettings.LoadFromFile(filenameProf);
                }

                var profName = ProfileName.Text;
                ProfileSelect.DataSource = new Profile[]{};
                ProfileSelect.DataSource = Profiles;
                var prof= getProfileByName(profName);
                ProfileSelect.SelectedItem =prof;

                return true;
            }
            return false;
           
        }
        Profile getProfileByName( string name )
        {
            foreach (var element in Profiles) {
                if (element.ProfileName == name)
                {
                    return element;
                }
            }
            return null;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Write(ex.Message);
            }
        }

        private void DisplayLocationSelector()
        {
            // We set current values
            GlobalVars.latitude = StrCordToDouble(text_Latidude.Text);
            GlobalVars.longitude = StrCordToDouble(text_Longitude.Text);
            GlobalVars.altitude = StrCordToDouble(text_Altitude.Text);
            int.TryParse(text_MoveRadius.Text, out GlobalVars.radius);

            LocationSelect locationSelector = new LocationSelect(false);           
            locationSelector.ShowDialog();
            
            // We set selected values
            text_Latidude.Text = GlobalVars.latitude.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = GlobalVars.longitude.ToString(CultureInfo.InvariantCulture);
            text_Altitude.Text = GlobalVars.altitude.ToString(CultureInfo.InvariantCulture);
            text_MoveRadius.Text = ""+GlobalVars.radius;
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
                decimal throwsChanceSum = 0;

                throwsChanceSum = text_Pb_Excellent.Value +
                                  text_Pb_Great.Value +
                                  text_Pb_Nice.Value +
                                  text_Pb_Ordinary.Value;
                if (throwsChanceSum > 100)
                {
                    MessageBox.Show(th.TS("You can not have a total throw chance greater than 100%.\nResetting throw chance to 0%!"));
                    (sender as NumericUpDown).Value =0;
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

        public static void Extract(string nameSpace, string outDir, string internalFilePath, string resourceName)
        {
            Assembly ass = Assembly.GetCallingAssembly();
            if (File.Exists(outDir + "\\" + resourceName))
            {
                File.Delete(outDir + "\\" + resourceName);
            }

            using (var s = ass.GetManifestResourceStream(nameSpace + "." + (internalFilePath == string.Empty ? string.Empty : internalFilePath + ".") + resourceName))
            {
                if (s != null)
                {
                    using (BinaryReader r = new BinaryReader(s))
                    using (FileStream fs = new FileStream(outDir + "\\" + resourceName, FileMode.OpenOrCreate))
                    using (BinaryWriter w = new BinaryWriter(fs))
                        w.Write(r.ReadBytes((int)s.Length));
                }
            }
            
        }
        // Code cleanup we can do later
        public class ExtendedWebClient : WebClient
        {

            public int Timeout {
                get;
                set;
            }
            
            public ExtendedWebClient()
            {
                this.Timeout = 2000;//In Milli seconds 
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.Timeout;
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
            catch (Exception)
            {
                File.WriteAllLines(deviceinfo, new string[] { comboBox_Device.SelectedItem.ToString(), " " });
            }
        }


        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://developers.google.com/maps/documentation/directions/start#get-a-key/");
            Process.Start(sInfo);
        }


        public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }



        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(th.TS("This will capture pokemons while walking spiral, and will use pokestops which are within 30 meters of the path projected."));
        }


        private void button2_Click_1(object sender, EventArgs e)
        {
            System.Windows.Forms.Form update = new Update();
            this.Hide();
            update.Show();
        }


        private void buttonSvProf_Click_2(object sender, EventArgs e)
        {
            if (Save())
                MessageBox.Show(th.TS("Current Configuration Saved as - ") + ProfileName.Text);
            else
                MessageBox.Show(th.TS("Please Review Red Boxes Before Save"));
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

        void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            ((TextBox) sender).BackColor = SystemColors.Window;
        }


        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://talk.pogodev.org/d/55-api-hashing-service-f-a-q/");
            Process.Start(sInfo);
        }
        void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://honorbuddy.myshopify.com/cart/29160991442:1?attributes[resellerId]=93");
        }
        void linkLabel16_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          Process.Start("https://github.com/Logxn/PokemonGo-Bot/");
        }

        private void button_Information_Click(object sender, EventArgs e)
        {
            var message = th.TS("Since the new API was cracked by the pogodev team, they have choosen to make the API pay2use We did not have any influence on this. We are very sorry this needed to happen!");
            var title = th.TS("Hashing Information");
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        void Button4Click(object sender, EventArgs e)
        {
            new GUIAvatar().ShowDialog();
        }
        void checkBox_UseProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxy.Checked){
                prxyIP.Enabled = true;
                prxyPort.Enabled = true;
            }else{
                prxyIP.Enabled = false;
                prxyPort.Enabled = false;
            }
        }
        void checkBox_UseProxyAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxyAuth.Checked){
                prxyUser.Enabled = true;
                prxyPass.Enabled = true;
            }else{
                prxyUser.Enabled = false;
                prxyPass.Enabled = false;
            }

        }
        void ComboLanguageSelectedIndexChanged(object sender, EventArgs e)
        {
            var lang = "";
            switch (comboLanguage.SelectedIndex) {
                case 0:
                    lang = CultureInfo.CurrentCulture.Name;
                    break;
                case 1:
                    lang = "default";
                    break;
                case 2:
                    lang = "de";
                    break;
                case 3:
                    lang = "es";
                    break;
                case 4:
                    lang = "ca-ES";
                    break;
            }

            if (lang !="")
            {
                Helper.TranslatorHelper.DownloadTranslationFile("PokemonGo.RocketAPI.Console/Lang", Program.path_translation, lang);
                th.SelectLanguage(lang);
                th.Translate(this);
            }
        }
        void checkBox_AlwaysTransfer_CheckedChanged(object sender, EventArgs e)
        {
            var i = 0;
            while (i < checkedListBox_AlwaysTransfer.Items.Count)
            {
                checkedListBox_AlwaysTransfer.SetItemChecked(i, (sender as CheckBox).Checked);
                i++;
            }
        }

    }
}
