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
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using POGOProtos.Enums;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Enums;
using PokeMaster.Logic.Shared;
using PokeMaster.Dialogs;
using PokeMaster.Helper;
using System.ComponentModel;
using System.Collections;

namespace PokeMaster
{
    public partial class ConfigWindow : System.Windows.Forms.Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        private const string NEW_YORK_COORS = "40.764883;-73.972967";

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public static Collection<Profile> Profiles = new Collection<Profile>();
        
        public static NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        public static int[] evolveBlacklist = {
            3, 6, 9, 12, 15, 18, 20, 22, 24, 26, 28, 31, 34, 36, 38, 40, 42, 45, 47, 49, 51, 53, 55, 57, 59, 62, 65, 68, 71, 73, 76, 78, 80, 82, 83, 85, 87, 89, 91, 94, 95, 97, 99, 101, 103, 105, 106, 107, 108, 110, 112, 113, 114, 115, 117, 119, 121, 122, 123, 124, 125, 126, 127, 128, 130, 131, 132, 134, 135, 136, 137, 139, 141, 142, 143, 144, 145, 146, 149, 150, 151
        };

        static Dictionary<string, int> pokeIDS = new Dictionary<string, int>();
        static Dictionary<string, int> evolveIDS = new Dictionary<string, int>();

        private Profile ActiveProfile;
        private List<PokemonId> toSnipe = new List<PokemonId>();
        
        public Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public ConfigWindow()
        {
            InitializeComponent();
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

            if (!Directory.Exists(GlobalVars.PathToDevices))
                Directory.CreateDirectory(GlobalVars.PathToDevices);
            
            // some information about ios devices
            // http://stackoverflow.com/questions/448162/determine-device-iphone-ipod-touch-with-iphone-sdk/3950748#3950748
            // https://www.theiphonewiki.com/wiki/Models
            if (!File.Exists(GlobalVars.FileForDeviceData))
                DownloadHelper.DownloadFile("PokemonGo.RocketAPI.Console/Resources", GlobalVars.PathToDevices, "DeviceData.json");

            var devData = new DeviceSetup(GlobalVars.FileForDeviceData);
            comboBox_Device.DisplayMember = "Tradename";
            comboBox_Device.DataSource = devData.data;
            comboBox_Device.Text = "iPhone 7";
            ButtonGenerateID_Click(sender, new EventArgs());
            
                
            #region new translation
            if (!Directory.Exists(GlobalVars.PathToTranslations))
                Directory.CreateDirectory(GlobalVars.PathToTranslations);

            comboLanguage.SelectedIndex = 0;
            // Download json file of current Culture Info if exists
            TranslatorHelper.DownloadTranslationFile("PokemonGo.RocketAPI.Console/Lang", GlobalVars.PathToTranslations, CultureInfo.CurrentCulture.Name);
            // Translate using Current Culture Info
            th.Translate(this);
            tabControl1.SizeMode = TabSizeMode.Normal;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            #endregion

            #region Populate Defaults & Lists
            comboLocale.DataSource = new LocaleHelper().getLocales();
            comboLocale.Text = "en-US";

            comboTimeZone.DataSource = new LocaleHelper().getTimezones();
            comboTimeZone.Text = "America/Los_Angeles";

            comboBoxLeaveInGyms.DataSource = new[] {
                th.TS("Random"),
                th.TS("Best CP"),
                th.TS("Worse CP"),
                th.TS("Favourite")
            };
            comboBoxLeaveInGyms.SelectedIndex = 0;

            comboBoxAttackers.DataSource = new[] {
                th.TS("Random"),
                th.TS("Best CP"),
                th.TS("Favourites"),
                th.TS("Lower than defenders CP"),
                th.TS("Best HP")
            };
            comboBoxAttackers.SelectedIndex = 0;

            int i = 1;
            int ev = 1;

            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId))) {
                if (pokemon != PokemonId.Missingno) {
                    pokeIDS[pokemon.ToString()] = i;
                    checkedListBox_PokemonNotToTransfer.Items.Add(th.TS(pokemon.ToString()));
                    checkedListBox_AlwaysTransfer.Items.Add(th.TS(pokemon.ToString()));
                    checkedListBox_PokemonNotToCatch.Items.Add(th.TS(pokemon.ToString()));
                    checkedListBox_Pinap.Items.Add(th.TS(pokemon.ToString()));
                    if (!(evolveBlacklist.Contains(i))) {
                        checkedListBox_PokemonToEvolve.Items.Add(th.TS(pokemon.ToString()));
                        evolveIDS[pokemon.ToString()] = ev;
                        ev++;
                    }
                    i++;
                }
            }
            #endregion

            #region Load Profile & Config

            Profiles = new Collection<Profile>();
            ActiveProfile = new Profile();
            ActiveProfile.ProfileName = "DefaultProfile";
            ActiveProfile.RunOrder = 1;
            ActiveProfile.Settings = new ProfileSettings();
            UpdateActiveProf(false);
            var foundDefaultProfile = false;
            var filenameProf = "";
            Profile selectedProfile = null;
            var result = "";

            if (File.Exists(GlobalVars.FileForProfiles)) {

                Profiles = JsonConvert.DeserializeObject<Collection<Profile>>(File.ReadAllText(@GlobalVars.FileForProfiles));

                foreach (Profile _profile in Profiles) {
                    if (_profile.IsDefault)
                        selectedProfile = _profile;

                    foundDefaultProfile |= _profile.ProfileName == "DefaultProfile";
                    filenameProf = Path.Combine(GlobalVars.PathToConfigs, _profile.ProfileName + ".json");
                    if (File.Exists(filenameProf)) {
                        try {
                            _profile.Settings = ProfileSettings.LoadFromFile(filenameProf);
                        } catch (Exception) {
                            result += filenameProf + "\n";
                        }
                    } else {
                        if (_profile.SettingsJSON != "") {
                            try {
                                _profile.Settings = ProfileSettings.LoadFromStringJSON(_profile.SettingsJSON);
                                _profile.SettingsJSON = "";
                            } catch (Exception) {
                                if (_profile.ProfileName != "DefaultProfile")
                                    result += filenameProf + "\n";
                            }
                            
                        }
                    }
                    
                }
            }

            if (!foundDefaultProfile)
                Profiles.Add(ActiveProfile);

            ProfileSelect.DataSource = Profiles;
            ProfileSelect.DisplayMember = "ProfileName";
            if (selectedProfile != null) {
                ActiveProfile.IsDefault = false;
                LoadData(selectedProfile.Settings);
                checkBoxDefaultProf.Checked = selectedProfile.IsDefault;
                ProfileSelect.SelectedItem = selectedProfile;
                ProfileName.Text = selectedProfile.ProfileName;
            } else {
                ProfileSelect.SelectedItem = ActiveProfile;
                ProfileName.Text = ActiveProfile.ProfileName;
            }

            if (result != "") {
                var message = th.TS("Loading Config failed\n{0} Check settings before running!", result);
                MessageBox.Show(message);
            }
            #endregion

            #region Version Check
            var currVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var newestVersion = Program.getNewestVersion();

            currVer.Text = currVersion.ToString();
            newVer.Text = newestVersion.ToString();

            if (Program.getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version) {
                if (checkbox_AutoUpdate.Checked) {
                    System.Windows.Forms.Form update = new Update();
                    update.ShowDialog();
                } else {
                    var message = th.TS("There is an Update on Github. do you want to open it ?");
                    var title = th.TS("Newest Version: {0}", newestVersion);
                    DialogResult dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                        Process.Start("https://github.com/Logxn/PokemonGo-Bot");
                    else if (dialogResult == DialogResult.No) {

                    }
                }

            } else {
                currVer.ForeColor = Color.Green;
                newVer.ForeColor = Color.Green;
            }
            #endregion
        }

        private void LoadData(ProfileSettings config)
        {
            if (config == null)
                return;

            #region Tab 1 - General
            pFHashKey.Text = config.pFHashKey;
            
            comboBox_AccountType.SelectedIndex = 1;
            if (config.AuthType == AuthType.Google) {
                comboBox_AccountType.SelectedIndex = 0;
            }

            comboBox_Device.Text = config.DeviceTradeName;
            textBoxDeviceID.Text = config.DeviceID;

            text_EMail.Text = config.Username;
            text_Password.Text = config.Password;
            checkbox_PWDEncryption.Checked = config.UsePwdEncryption;
            if (config.UsePwdEncryption) {
                text_Password.Text = Encryption.Decrypt(config.Password);
                if (text_Password.Text == "")
                    MessageBox.Show(th.TS("Password cannot be decrypted.\nPlease, enter it again."));
            }
            
            text_Latidude.Text = config.DefaultLatitude.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = config.DefaultLongitude.ToString(CultureInfo.InvariantCulture);
            text_Altitude.Text = config.DefaultAltitude.ToString(CultureInfo.InvariantCulture);
            
            comboLocale.Text = config.LocaleCountry +"-"+ config.LocaleLanguage;
            comboTimeZone.Text = config.LocaleTimeZone;
            
            
            checkBox_UseLuckyEggAtEvolve.Checked = config.UseLuckyEgg;
            checkBox_SimulateAnimationTimeAtEvolve.Checked = config.UseAnimationTimes;
            checkBox_EvolvePokemonIfEnoughCandy.Checked = config.EvolvePokemonsIfEnoughCandy;
            checkBox_UseIncenseEvery30min.Checked = config.UseIncense;
            checkBox_EnablePokemonListGui.Checked = config.EnablePokeList;
            checkBox_ConsoleInTab.Checked = config.EnableConsoleInTab;
            CB_SimulatePGO.Checked = config.simulatedPGO;
            checkBox_KeepPokemonWhichCanBeEvolved.Checked = config.keepPokemonsThatCanEvolve;
            checkBox_UseLuckyEggIfNotRunning.Checked = config.UseLuckyEggIfNotRunning;
            checkBox_CollectDailyBonus.Checked = config.CollectDailyBonus;
            checkBox_ShowStats.Checked = config.ShowStats;
            checkBox_UseNanabBerry.Checked = config.UseNanabBerry;
            checkBox_UseItemAtEvolve.Checked = config.UseItemAtEvolve;
            #endregion

            #region Tab 2 - Pokemons
            if (config.pokemonsToHold != null)
                foreach (PokemonId Id in config.pokemonsToHold) {
                string _id = Id.ToString();
                checkedListBox_PokemonNotToTransfer.SetItemChecked(pokeIDS[_id] - 1, true);
            }
            if (config.pokemonsToAlwaysTransfer != null)
                foreach (PokemonId Id in config.pokemonsToAlwaysTransfer) {
                string _id = Id.ToString();
                checkedListBox_AlwaysTransfer.SetItemChecked(pokeIDS[_id] - 1, true);
            }

            
            if (config.catchPokemonSkipList != null)
                foreach (PokemonId Id in config.catchPokemonSkipList) {
                string _id = Id.ToString();
                checkedListBox_PokemonNotToCatch.SetItemChecked(pokeIDS[_id] - 1, true);
            }
            if (config.pokemonsToEvolve != null)
                foreach (PokemonId Id in config.pokemonsToEvolve) {
                string _id = Id.ToString();
                checkedListBox_PokemonToEvolve.SetItemChecked(evolveIDS[_id] - 1, true);
            }
            
            checkBox_AutoTransferDoublePokemon.Checked = config.TransferDoublePokemons;
            checkBox_TransferFirstLowIV.Checked = config.TransferFirstLowIV;
            text_MaxDuplicatePokemon.Text = config.HoldMaxDoublePokemons.ToString();
            text_MaxIVToTransfer.Text = config.ivmaxpercent.ToString();
            text_MaxCPToTransfer.Text = config.DontTransferWithCPOver.ToString();
            MinCPtoCatch.Text = config.MinCPtoCatch.ToString();
            MinIVtoCatch.Text = config.MinIVtoCatch.ToString();
            MinProbToCatch.Text = config.MinProbToCatch.ToString();
            checkBox_UseSpritesFolder.Checked = config.UseSpritesFolder;
            checkBox_ShowPokemons.Checked = config.ShowPokemons;
            nud_EvolveAt.Value = config.EvolveAt;
            checkBox_TransferSlashPokemons.Checked = config.TransferSlashPokemons;
            #endregion

            #region Tab 3 - Throws
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
            num_NanabPercent.Value = config.NanabPercent;

            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId))) {
                if (pokemon == PokemonId.Missingno)
                    continue;
                var intID = (int)pokemon;
                var isChecked = false;
                if (config.PokemonPinap != null) {
                    isChecked = config.PokemonPinap.Contains(pokemon);
                }
                checkedListBox_Pinap.SetItemChecked(intID - 1, isChecked);
            }

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
            #endregion

            #region Tab 4 - Items
            foreach (Control element in this.groupBoxItems.Controls) {
                if (element.Name.IndexOf("num_") == 0) {
                    var name = element.Name.Replace("num_", "");
                    var property = config.GetType().GetProperty(name);
                    if (property != null)
                        (element as NumericUpDown).Value = (int)property.GetValue(config);

                }
            }
            
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
            #endregion

            #region Tab 5 - Proxy
            if (config.proxySettings == null)
                config.proxySettings = new ProxySettings();
            checkBox_UseProxy.Checked = config.proxySettings.enabled;
            checkBox_UseProxyAuth.Checked = config.proxySettings.useAuth;
            prxyIP.Text = config.proxySettings.hostName;
            prxyPort.Text = "" + config.proxySettings.port;
            prxyUser.Text = config.proxySettings.username;
            prxyPass.Text = config.proxySettings.password;
            #endregion

            #region Tab 6 - Walk
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
            checkBox_Paused.Checked = config.pauseAtPokeStop;
            
            checkBox_UseGoogleMapsRouting.Checked = config.UseGoogleMapsAPI;
            text_GoogleMapsAPIKey.Text = config.GoogleMapsAPIKey;
            
            checkBox_RandomSleepAtCatching.Checked = config.sleepatpokemons;
            checkBox_FarmPokestops.Checked = config.FarmPokestops;
            checkBox_CatchPokemon.Checked = config.CatchPokemon;
            checkBox_BreakAtLure.Checked = config.BreakAtLure;
            checkBox_UseLureAtBreak.Checked = config.UseLureAtBreak;
            checkBox_SkipRadius.Checked = config.SkipRadius;
            checkBox_RandomlyReduceSpeed.Checked = config.RandomReduceSpeed;
            checkBox_UseBreakIntervalAndLength.Checked = config.UseBreakFields;
            checkBox_WalkInArchimedeanSpiral.Checked = config.Espiral;
            checkBox_WalkInLoop.Checked = config.WalkInLoop;
            checkBox_WalkRandomly.Checked = config.WalkRandomly;
            checkBox_StartWalkingFromLastLocation.Checked = config.UseLastCords;
            checkBox_BlockAltitude.Checked = config.BlockAltitude;

            AdvancedBreaks.Checked = config.AdvancedBreaks;

            if (AdvancedBreaks.Checked)
            {
                BreakGridView.Visible = true;
                BreakGridView.Enabled = true;

                BindingSource BreakSettingsBindingSource = new BindingSource();

                BreakSettingsBindingSource.DataSource = config.Breaks;
                BreakGridView.DataSource = BreakSettingsBindingSource;
                BreakSettingsBindingSource.Sort = "BreakSequenceId ASC";
                BreakGridView.Columns["BreakSequenceId"].HeaderText = "ID";
                BreakGridView.Columns["BreakWalkTime"].HeaderText = "Walking Time";
                BreakGridView.Columns["BreakIdleTime"].HeaderText = "Idle Time";
                BreakGridView.Columns["BreakEnabled"].HeaderText = "Enabled";
                BreakGridView.Columns["BreakSettingsCatchPokemon"].HeaderText = "Catch";
                BreakGridView.Columns["BreakSettingsMaxSpeed"].HeaderText = "Max";
                BreakGridView.Columns["BreakSettingsMinSpeed"].HeaderText = "Min";
            }

            if (config.Breaks == null) config.Breaks = new List<BreakSettings>();
            #endregion

            #region Tab 7 - Telegram and Logs
            cbLogPokemon.Checked = config.LogPokemons;
            cbLogManuelTransfer.Checked = config.LogTransfer;
            cbLogEvolution.Checked = config.LogEvolve;
            cbLogEggs.Checked = config.LogEggs;
            
            text_Telegram_Token.Text = config.TelegramAPIToken;
            text_Telegram_Name.Text = config.TelegramName;
            text_Telegram_LiveStatsDelay.Text = config.TelegramLiveStatsDelay.ToString();
            
            toSnipe = config.ToSnipe;
            
            checkBoxSendToDiscord.Checked = config.SendToDiscord;
            textBoxDiscordUser.Text = config.DiscordUser;
            textBoxDiscordPassword.Text = config.DiscordPassword;
            textBoxDiscordServerID.Text = ""+config.DiscordServerID;
            if (config.DiscordServerID == 0UL)
                textBoxDiscordServerID.Text = "223025934435876865";
            #endregion

            #region Tab 8 - Update
            checkbox_AutoUpdate.Checked = config.AutoUpdate;
            checkbox_checkWhileRunning.Checked = config.CheckWhileRunning;
            ChangeSelectedLanguage(config.SelectedLanguage);
            #endregion

            #region Dev Options
            if (config.Debug == null)
                config.Debug = new DebugSettings();
            checkbox_Verboselogging.Checked = config.Debug.VerboseMode;
            checkBoxExtractText.Checked = config.Debug.ExtractFormTexts;
            checkBoxNewLog.Checked = config.Debug.NewLog;
            checkBoxStoreUntranslated.Checked = config.Debug.StoreUntranslatedText;
            TranslatorHelper.ActiveExtractTexts = checkBoxExtractText.Checked;
            TranslatorHelper.StoreUntranslated = checkBoxStoreUntranslated.Checked;
            
            checkBoxCompleteTutorial.Checked = config.CompleteTutorial;
            #endregion

            #region Gym Options
            if (config.Gyms == null)
                config.Gyms = new GymSettings();
            checkBox_FarmGyms.Checked = config.Gyms.Farm;
            checkBoxSaveFortsInfo.Checked = config.SaveForts;
            textBoxFortsFile.Text = config.FortsFile;
            checkBoxAttackGyms.Checked = config.Gyms.Attack;
            nudNumDefenders.Value = (config.Gyms.NumDefenders <= 0) ? 1 : config.Gyms.NumDefenders;
            numericUpDownMaxAttacks.Value = (config.Gyms.MaxAttacks <= 0) ? 1 : config.Gyms.MaxAttacks;
            comboBoxLeaveInGyms.SelectedIndex = config.Gyms.DeployPokemons;
            comboBoxAttackers.SelectedIndex = config.Gyms.Attackers;
            checkBoxSpinGyms.Checked = config.Gyms.Spin;
            #endregion
            
            // Save Location
            checkBoxSaveLocations.Checked = config.SaveLocations;
            numMinIVSave.Value = config.MinIVSave;
            textBoxSaveLocationsFile.Text = config.SaveLocationsFile;

        }
        
        private void ChangeSelectedLanguage(string lang)
        {
            var index = 0;
            if (lang == "Default")
                index = 1;
            if (lang == "Deutsch")
                index = 2;
            if (lang == "Español")
                index = 3;
            if (lang == "Catalá")
                index = 4;
            comboLanguage.SelectedIndex = index;
        }

        /// <summary>
        /// Get Coordinates from file
        /// </summary>
        private const double ePSILON = 0.000001;
        private void LoadLatestCoords()
        {
            bool CoordsAreLoaded = false;
            GlobalVars.FileForCoordinates = Path.Combine(GlobalVars.PathToConfigs, $"LastCoords_{ProfileName.Text}.txt");
            if (File.Exists(GlobalVars.FileForCoordinates)) {
                string[] CoordsFromFileTXT = File.ReadAllText(GlobalVars.FileForCoordinates).Split(':');

                if (CoordsFromFileTXT.Length > 1) {
                    double lat = StrCordToDouble(CoordsFromFileTXT[0]);
                    double lng = StrCordToDouble(CoordsFromFileTXT[1]);
                    double alt = (CoordsFromFileTXT.Length == 3) ? StrCordToDouble(CoordsFromFileTXT[2]) : 0;
                    if ((Math.Abs(lat) > ePSILON) && (Math.Abs(lng) > ePSILON)) {
                        ActiveProfile.Settings.DefaultLatitude = lat;
                        ActiveProfile.Settings.DefaultLongitude = lng;
                        ActiveProfile.Settings.DefaultAltitude = alt;
                        CoordsAreLoaded = true;
                    }
                }
            }
            if (!CoordsAreLoaded) {
                Logger.Warning(th.TS("Failed loading last coords!"));
                Logger.Warning(th.TS("Using default location"));
            }
        }

        // Account Type Changed Event
        private void comboAccType_SelectedIndexChanged(object sender, EventArgs e)
        {
            label2.Text = comboBox_AccountType.SelectedIndex == 0 ? "E-Mail:" : th.TS("User Name:");
        }

        // Profile Selection Changed
        private void ProfileSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedProfile = (Profile)Profiles.FirstOrDefault(i => i == ProfileSelect.SelectedItem);
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

        private void buttonSaveStart_Click(object sender, EventArgs e)
        {
            if (Save()) {
                if (checkBoxSaveLocations.Checked && string.IsNullOrEmpty(textBoxSaveLocationsFile.Text)) {
                    MessageBox.Show(th.TS("Attention: You did not select a path for 'SavePokemonsLocation'"), th.TS("Oh snap!"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    textBoxSaveLocationsFile.BackColor = System.Drawing.Color.Red;
                    return;
                }
                if (ActiveProfile.Settings.UseLastCords)
                    LoadLatestCoords();

                var selectedCoords = ActiveProfile.Settings.DefaultLatitude.ToString("0.000000") + ";" + ActiveProfile.Settings.DefaultLongitude.ToString("0.000000");
                selectedCoords = selectedCoords.Replace(",", ".");
                if (selectedCoords.Equals(NEW_YORK_COORS)) {
                    var ret = MessageBox.Show(th.TS("Have you set correctly your location? (It seems like you are using default coords. This can lead to an auto-ban)"), th.TS("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (ret == DialogResult.No) {
                        return;
                    }
                }
                if (!isIOS()) {
                    var ret = MessageBox.Show(th.TS("Selected device is not an iOS device.\nCurrent Hash Service only simulates iOS hash\nAre you sure to continue with this values?"), th.TS("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (ret == DialogResult.No) {
                        return;
                    }
                }
                
                // TODO: Make this decyption at end of comuncation
                if (ActiveProfile.Settings.UsePwdEncryption)
                    ActiveProfile.Settings.Password = Encryption.Decrypt(ActiveProfile.Settings.Password);

                GlobalVars.Assign(ActiveProfile.Settings);
                GlobalVars.ProfileName = ProfileName.Text;
                Logger.Info("GlobalVars.ProfileName: " + GlobalVars.ProfileName);

                GlobalVars.ContinueLatestSession &= !checkBoxContinue.Checked || MessageBox.Show(th.TS("Are you sure you want continue last session?\n You will continue with last values of farmed pokestops and caught Pokemons"), th.TS("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.No;

                Dispose();
            } else
                MessageBox.Show(th.TS("Please Review Red Boxes Before Start"));

        }

        private bool textBoxToActiveProf(Control textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty) {
                if (fieldName == string.Empty) {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                typeof(ProfileSettings).GetProperty(fieldName).SetValue(ActiveProfile.Settings, textBox.Text);
            } else {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool textBoxToActiveProfDouble(Control textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty) {
                if (fieldName == string.Empty) {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                var valueTXT = textBox.Text.Replace(',', '.');
                var valueDBL = double.Parse(valueTXT, cords, CultureInfo.InvariantCulture);
                typeof(ProfileSettings).GetProperty(fieldName).SetValue(ActiveProfile.Settings, valueDBL);
            } else {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool textBoxToActiveProfInt(Control textBox, string fieldName = "")
        {
            textBox.BackColor = SystemColors.Window;
            var ret = true;
            if (textBox.Text != string.Empty) {
                if (fieldName == string.Empty) {
                    fieldName = textBox.Name.ToLower().Replace("text_", "");
                }
                var intVal = int.Parse(textBox.Text);
                var field = typeof(ProfileSettings).GetProperty(fieldName);
                field.SetValue(ActiveProfile.Settings, intVal);
            } else {
                textBox.BackColor = Color.Red;
                this.ActiveControl = textBox;
                ret = false;
            }
            return ret;
        }

        private bool UpdateActiveProf(bool makePrompts = true)
        {
            #region Setting all the globals

            ActiveProfile.IsDefault = checkBoxDefaultProf.Checked;
            // tab 1 - General
            
            ActiveProfile.Settings.AuthType = (comboBox_AccountType.SelectedIndex == 0) ? AuthType.Google : AuthType.Ptc;

            // Account Info
            bool ret = true;
            ret &= textBoxToActiveProf(pFHashKey, "pFHashKey");

            ActiveProfile.Settings.DeviceTradeName = comboBox_Device.Text;
            
            ActiveProfile.Settings.DeviceID = textBoxDeviceID.Text;

            if (makePrompts && textBoxDeviceID.Text == "") {
                MessageBox.Show(th.TS("Please. Create a Device ID"));
                ret = false;
            }

            ret &= textBoxToActiveProf(text_EMail, "Username");
            ret &= textBoxToActiveProf(text_Password, "Password");
            ActiveProfile.Settings.UsePwdEncryption = checkbox_PWDEncryption.Checked;

            ActiveProfile.Settings.LastTimePlayedTs = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;

            // Location
            ret &= textBoxToActiveProfDouble(text_Latidude, "DefaultLatitude");
            ret &= textBoxToActiveProfDouble(text_Longitude, "DefaultLongitude");
            ret &= textBoxToActiveProfDouble(text_Altitude, "DefaultAltitude");
            
            var country = "US";
            var language = "en";
            var splitted = comboLocale.Text.Split('-');
            if (splitted.Length > 1){
                language = splitted[0];
                country = splitted[1];
            }
            ActiveProfile.Settings.LocaleCountry = country;
            ActiveProfile.Settings.LocaleLanguage = language;
            ActiveProfile.Settings.LocaleTimeZone = comboTimeZone.Text;

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
            ActiveProfile.Settings.CollectDailyBonus = checkBox_CollectDailyBonus.Checked;
            ActiveProfile.Settings.ShowStats = checkBox_ShowStats.Checked;
            ActiveProfile.Settings.UseItemAtEvolve= checkBox_UseItemAtEvolve.Checked;

            // tab 2 - pokemons
            ActiveProfile.Settings.pokemonsToHold.Clear();
            ActiveProfile.Settings.catchPokemonSkipList.Clear();
            ActiveProfile.Settings.pokemonsToEvolve.Clear();

            foreach (string pokemon in checkedListBox_PokemonNotToTransfer.CheckedItems) {
                ActiveProfile.Settings.pokemonsToHold.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            foreach (string pokemon in checkedListBox_AlwaysTransfer.CheckedItems) {
                ActiveProfile.Settings.pokemonsToAlwaysTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            
            foreach (string pokemon in checkedListBox_PokemonNotToCatch.CheckedItems) {
                ActiveProfile.Settings.catchPokemonSkipList.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            foreach (string pokemon in checkedListBox_PokemonToEvolve.CheckedItems) {
                ActiveProfile.Settings.pokemonsToEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }
            ActiveProfile.Settings.UseSpritesFolder = checkBox_UseSpritesFolder.Checked;
            ActiveProfile.Settings.ShowPokemons = checkBox_ShowPokemons.Checked;
            ActiveProfile.Settings.EvolveAt = (int)nud_EvolveAt.Value;
            // bot settings
            ActiveProfile.Settings.TransferDoublePokemons = checkBox_AutoTransferDoublePokemon.Checked;
            ActiveProfile.Settings.TransferFirstLowIV = checkBox_TransferFirstLowIV.Checked;
            ActiveProfile.Settings.TransferSlashPokemons = checkBox_TransferSlashPokemons.Checked;

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
            if (text_UseRazzberryChance.Text == string.Empty) {
                text_UseRazzberryChance.BackColor = Color.Red;
            } else {
                int x = int.Parse(text_UseRazzberryChance.Text);
                decimal c = ((decimal)x / 100);
                ActiveProfile.Settings.razzberry_chance = Convert.ToDouble(c);
            }

            ActiveProfile.Settings.NanabPercent = (int)num_NanabPercent.Value;

            ActiveProfile.Settings.excellentthrow = (int)text_Pb_Excellent.Value;
            ActiveProfile.Settings.greatthrow = (int)text_Pb_Great.Value;
            ActiveProfile.Settings.nicethrow = (int)text_Pb_Nice.Value;
            ActiveProfile.Settings.ordinarythrow = (int)text_Pb_Ordinary.Value;
            
            ret &= textBoxToActiveProfInt(GreatBallMinCP, "MinCPforGreatBall");
            ret &= textBoxToActiveProfInt(UltraBallMinCP, "MinCPforUltraBall");

            if (ActiveProfile.Settings.PokemonPinap == null)
                ActiveProfile.Settings.PokemonPinap = new List<PokemonId>();
            else
                ActiveProfile.Settings.PokemonPinap.Clear();
            foreach (string pokemon in checkedListBox_Pinap.CheckedItems) {
                ActiveProfile.Settings.PokemonPinap.Add((PokemonId)Enum.Parse(typeof(PokemonId), th.RS(pokemon)));
            }

            // tab 4 - Items
            foreach (Control element in this.groupBoxItems.Controls) {
                if (element.Name.IndexOf("num_") == 0) {
                    var name = element.Name.Replace("num_", "");
                    var property = ActiveProfile.Settings.GetType().GetProperty(name);
                    if (property != null)
                        property.SetValue(ActiveProfile.Settings, (int)(element as NumericUpDown).Value);
                }
            }

            ret &= textBoxToActiveProfInt(MinCPtoCatch, "MinCPtoCatch");
            ret &= textBoxToActiveProfInt(MinIVtoCatch, "MinIVtoCatch");
            ret &= textBoxToActiveProfInt(MinProbToCatch, "MinProbToCatch");

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
            if (ActiveProfile.Settings.proxySettings == null)
                ActiveProfile.Settings.proxySettings = new ProxySettings();
            
            ActiveProfile.Settings.proxySettings.enabled = checkBox_UseProxy.Checked;
            ActiveProfile.Settings.proxySettings.useAuth = checkBox_UseProxyAuth.Checked;
            ActiveProfile.Settings.proxySettings.hostName = prxyIP.Text;
            var intvalue = 0;
            int.TryParse(prxyPort.Text, out intvalue);
            ActiveProfile.Settings.proxySettings.port = intvalue;
            ActiveProfile.Settings.proxySettings.username = prxyUser.Text;
            ActiveProfile.Settings.proxySettings.password = prxyPass.Text;

            // tab 6 - Walk
            ret &= textBoxToActiveProfDouble(text_Speed, "WalkingSpeedInKilometerPerHour");
            if (text_Speed.Text != String.Empty)
            {
                toolTip1.SetToolTip(text_Speed, Convert.ToDouble(text_Speed.Text) * 1000 / 3600 + " m/s");
                ActiveProfile.Settings.WalkingSpeedInKilometerPerHour = double.Parse(text_Speed.Text);
            }

            if ((makePrompts) && (ActiveProfile.Settings.WalkingSpeedInKilometerPerHour > 15)) {
                var speed = ActiveProfile.Settings.WalkingSpeedInKilometerPerHour;
                var message = th.TS("The risk of being banned is significantly greater when using higher than human jogging speeds (e.g. > 15km/hr) Click 'No' to use ~10km/hr instead");
                var title = th.TS("Are you sure you wish to set your speed to {0} ?", speed);
                var dialogResult = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.No)
                    ActiveProfile.Settings.WalkingSpeedInKilometerPerHour = double.Parse("9.5", cords, CultureInfo.InvariantCulture);
            }

            if (text_MinWalkSpeed.Text != String.Empty)
            {
                toolTip1.SetToolTip(text_MinWalkSpeed, Convert.ToDouble(text_MinWalkSpeed.Text) * 1000 / 3600 + " m/s");
                ActiveProfile.Settings.MinWalkSpeed = double.Parse(text_MinWalkSpeed.Text);
            }

            ret &= textBoxToActiveProfInt(text_MoveRadius, "MaxWalkingRadiusInMeters");

            if (text_TimeToRun.Text == String.Empty)
                text_TimeToRun.Text = "0";
            ActiveProfile.Settings.TimeToRun = Double.Parse(text_TimeToRun.Text);

            if (text_RestartAfterRun.Text == String.Empty)
                text_RestartAfterRun.Text = "0";
            ActiveProfile.Settings.RestartAfterRun = int.Parse(text_RestartAfterRun.Text);


            if (text_PokemonCatchLimit.Text != String.Empty)
                ActiveProfile.Settings.PokemonCatchLimit = int.Parse(text_PokemonCatchLimit.Text);

            if (text_PokestopFarmLimit.Text != String.Empty)
                ActiveProfile.Settings.PokestopFarmLimit = int.Parse(text_PokestopFarmLimit.Text);

            if (text_XPFarmedLimit.Text != String.Empty)
                ActiveProfile.Settings.XPFarmedLimit = int.Parse(text_XPFarmedLimit.Text);

            ActiveProfile.Settings.UseBreakFields = checkBox_UseBreakIntervalAndLength.Checked;
            
            if (text_BreakInterval.Text != String.Empty)
                ActiveProfile.Settings.BreakInterval = int.Parse(text_BreakInterval.Text);

            if (text_BreakLength.Text != String.Empty)
                ActiveProfile.Settings.BreakLength = int.Parse(text_BreakLength.Text);
            
            if (ActiveProfile.Settings.UseBreakFields){
                if (ActiveProfile.Settings.BreakInterval <= 0){
                    text_BreakInterval.BackColor = Color.Red;
                    ret = false;
                }
                if (ActiveProfile.Settings.BreakLength <= 0){
                    text_BreakLength.BackColor = Color.Red;
                    ret = false;
                }
            }

            ActiveProfile.Settings.pauseAtEvolve = checkBox_StopWalkingWhenEvolving.Checked;
            ActiveProfile.Settings.pauseAtEvolve2 = checkBox_StopWalkingWhenEvolving.Checked;
            ActiveProfile.Settings.pauseAtPokeStop = checkBox_Paused.Checked;

            ActiveProfile.Settings.UseGoogleMapsAPI = checkBox_UseGoogleMapsRouting.Checked;
            ActiveProfile.Settings.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;

            ActiveProfile.Settings.sleepatpokemons = checkBox_RandomSleepAtCatching.Checked;
            ActiveProfile.Settings.FarmPokestops = checkBox_FarmPokestops.Checked;
            
            ActiveProfile.Settings.CatchPokemon = checkBox_CatchPokemon.Checked;
            ActiveProfile.Settings.BreakAtLure = checkBox_BreakAtLure.Checked;
            ActiveProfile.Settings.UseLureAtBreak = checkBox_UseLureAtBreak.Checked;
            ActiveProfile.Settings.RandomReduceSpeed = checkBox_RandomlyReduceSpeed.Checked;
            ActiveProfile.Settings.SkipRadius = checkBox_SkipRadius.Checked;

            ActiveProfile.Settings.Espiral = checkBox_WalkInArchimedeanSpiral.Checked;
            ActiveProfile.Settings.WalkInLoop = checkBox_WalkInLoop.Checked;
            ActiveProfile.Settings.WalkRandomly = checkBox_WalkRandomly.Checked;
            
            ActiveProfile.Settings.UseLastCords = checkBox_StartWalkingFromLastLocation.Checked;
            ActiveProfile.Settings.BlockAltitude = checkBox_BlockAltitude.Checked;

            // Save BreakSettings
            ActiveProfile.Settings.AdvancedBreaks = AdvancedBreaks.Checked;
            if (BreakGridView.DataSource != null)
            {
                ActiveProfile.Settings.Breaks = ((System.Windows.Forms.BindingSource)BreakGridView.DataSource).List.Cast<BreakSettings>().ToList();
            }

            // tab 7 - Logs and Telegram
            ActiveProfile.Settings.LogPokemons = cbLogPokemon.Checked;
            ActiveProfile.Settings.LogTransfer = cbLogManuelTransfer.Checked;
            ActiveProfile.Settings.LogEvolve = cbLogEvolution.Checked;
            ActiveProfile.Settings.LogEggs = cbLogEggs.Checked;

            ActiveProfile.Settings.TelegramAPIToken = text_Telegram_Token.Text;
            ActiveProfile.Settings.TelegramName = text_Telegram_Name.Text;
            ret &= textBoxToActiveProfInt(text_Telegram_LiveStatsDelay, "TelegramLiveStatsDelay");
            ActiveProfile.Settings.SnipePokemon = false;
            ActiveProfile.Settings.AvoidRegionLock = true;
            if ((makePrompts) && (ActiveProfile.Settings.SnipePokemon)) {
                DialogResult result = MessageBox.Show(th.TS("Sniping has not been tested yet. It could get you banned. Do you want to continue?"), th.TS("Info"), MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                ActiveProfile.Settings.SnipePokemon = result == DialogResult.OK ? true : false;
            }
            ActiveProfile.Settings.ToSnipe = toSnipe;

            ActiveProfile.Settings.SendToDiscord = checkBoxSendToDiscord.Checked;
            ActiveProfile.Settings.DiscordUser = textBoxDiscordUser.Text;
            ActiveProfile.Settings.DiscordPassword = textBoxDiscordPassword.Text;

            var server = 223025934435876865UL;
            ulong.TryParse(textBoxDiscordServerID.Text,out server);
            ActiveProfile.Settings.DiscordServerID = server;
            
            // tab 8 updates
            ActiveProfile.Settings.AutoUpdate = checkbox_AutoUpdate.Checked;
            ActiveProfile.Settings.CheckWhileRunning = checkbox_checkWhileRunning.Checked;
            ActiveProfile.Settings.NextBestBallOnEscape = NextBestBallOnEscape.Checked;
            ActiveProfile.Settings.SelectedLanguage = comboLanguage.Text;

            // dev options
            if (ActiveProfile.Settings.Debug == null)
                ActiveProfile.Settings.Debug = new DebugSettings();
            ActiveProfile.Settings.Debug.VerboseMode = checkbox_Verboselogging.Checked;
            ActiveProfile.Settings.Debug.ExtractFormTexts = checkBoxExtractText.Checked;
            ActiveProfile.Settings.Debug.NewLog = checkBoxNewLog.Checked;
            ActiveProfile.Settings.Debug.StoreUntranslatedText = checkBoxStoreUntranslated.Checked;
            ActiveProfile.Settings.CompleteTutorial = checkBoxCompleteTutorial.Checked;

            if (comboBox_Device.SelectedIndex < 0) {
                ret = false;
                if (makePrompts)
                    MessageBox.Show(th.TS("Please select a Device"));
            }

            // Gyms
            ActiveProfile.Settings.FortsFile = textBoxFortsFile.Text;
            ActiveProfile.Settings.SaveForts = checkBoxSaveFortsInfo.Checked;
            
            if (ActiveProfile.Settings.Gyms == null)
                ActiveProfile.Settings.Gyms = new GymSettings();
            ActiveProfile.Settings.Gyms.DeployPokemons = comboBoxLeaveInGyms.SelectedIndex;
            ActiveProfile.Settings.Gyms.Attackers = comboBoxAttackers.SelectedIndex;
            ActiveProfile.Settings.Gyms.Spin = checkBoxSpinGyms.Checked;
            ActiveProfile.Settings.Gyms.Farm = checkBox_FarmGyms.Checked;
            ActiveProfile.Settings.Gyms.Attack = checkBoxAttackGyms.Checked;
            ActiveProfile.Settings.Gyms.NumDefenders = (int)nudNumDefenders.Value;
            ActiveProfile.Settings.Gyms.MaxAttacks = (int)numericUpDownMaxAttacks.Value;

            ActiveProfile.Settings.UseNanabBerry = checkBox_UseNanabBerry.Checked;
            // Save Locations
            ActiveProfile.Settings.SaveLocations = checkBoxSaveLocations.Checked;
            ActiveProfile.Settings.MinIVSave = (int)numMinIVSave.Value;
            ActiveProfile.Settings.SaveLocationsFile = textBoxSaveLocationsFile.Text;
            

            #endregion
            return ret;
        }

        private bool Save()
        {
            if (UpdateActiveProf(true)) {
                if (ActiveProfile.Settings.UsePwdEncryption) {
                    ActiveProfile.Settings.Password = Encryption.Encrypt(ActiveProfile.Settings.Password);
                }
                if (!Directory.Exists(GlobalVars.PathToConfigs))
                    Directory.CreateDirectory(GlobalVars.PathToConfigs);
                var filenameProf = Path.Combine(GlobalVars.PathToConfigs, ProfileName.Text + ".json");
                ActiveProfile.Settings.SaveToFile(filenameProf);
                var newProfiles = new Collection<Profile>();
                var foundActiveProf = false;
                foreach (Profile _profile in Profiles) {
                    var newProfile = new Profile();
                    newProfile.ProfileName = _profile.ProfileName;
                    newProfile.IsDefault = _profile.IsDefault;

                    newProfile.IsDefault &= !ActiveProfile.IsDefault;

                    newProfile.RunOrder = _profile.RunOrder;
                    newProfile.SettingsJSON = _profile.SettingsJSON;
                    newProfile.Settings = null;
                    if (_profile.ProfileName == ProfileName.Text) {
                        newProfile.IsDefault = ActiveProfile.IsDefault;
                        newProfile.RunOrder = ActiveProfile.RunOrder;
                        newProfile.SettingsJSON = "";
                        foundActiveProf = true;
                    }
                    newProfiles.Add(newProfile);
                }
                if (!foundActiveProf) {
                    var newProfile = new Profile();
                    newProfile.ProfileName = ProfileName.Text;
                    newProfile.IsDefault = ActiveProfile.IsDefault;
                    newProfile.RunOrder = ActiveProfile.RunOrder;
                    newProfile.SettingsJSON = "";
                    newProfile.Settings = null;
                    newProfiles.Add(newProfile);
                }
                var profileJSON = JsonConvert.SerializeObject(newProfiles, Formatting.Indented);
                File.WriteAllText(@GlobalVars.FileForProfiles, profileJSON);
                if (!foundActiveProf) {
                    var newProfile = new Profile();
                    newProfile.ProfileName = ProfileName.Text;
                    newProfile.IsDefault = ActiveProfile.IsDefault;
                    newProfile.RunOrder = ActiveProfile.RunOrder;
                    newProfile.SettingsJSON = "";
                    newProfile.Settings = ProfileSettings.LoadFromFile(filenameProf);
                    Profiles.Add(newProfile);
                } else {
                    getProfileByName(ProfileName.Text).Settings = ProfileSettings.LoadFromFile(filenameProf);
                }

                var profName = ProfileName.Text;
                ProfileSelect.DataSource = new Profile[]{ };
                ProfileSelect.DataSource = Profiles;
                var prof = getProfileByName(profName);
                ProfileSelect.SelectedItem = prof;

                return true;
            }
            return false;
            
        }

        Profile getProfileByName(string name)
        {
            foreach (var element in Profiles) {
                if (element.ProfileName == name) {
                    return element;
                }
            }
            return null;
        }

        #region CheckedChanged Events

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonNotToTransfer.Items.Count) {
                checkedListBox_PokemonNotToTransfer.SetItemChecked(i, checkBox4.Checked);
                i++;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonNotToCatch.Items.Count) {
                checkedListBox_PokemonNotToCatch.SetItemChecked(i, checkBox5.Checked);
                i++;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            int i = 0;
            while (i < checkedListBox_PokemonToEvolve.Items.Count) {
                checkedListBox_PokemonToEvolve.SetItemChecked(i, checkBox6.Checked);
                i++;
            }
        }


        #endregion

        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        #region External Links 

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Logxn/PokemonGo-Bot");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://slaughter-gaming.de/");
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://discord.gg/phu3GNN/");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RUNUBQEANCAGQ");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://pokemaster.me");
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
            Process.Start("http://proxylist.hidemyass.com");
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://developers.google.com/maps/documentation/directions/start#get-a-key/");
            Process.Start(sInfo);
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

        private void linkLabel14_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sInfo = new ProcessStartInfo("https://talk.pogodev.org/d/55-api-hashing-service-f-a-q/");
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

        void linkLabelPokemaster_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://pokemaster.me");
        }

        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            try {
                DisplayLocationSelector();
            } catch (Exception ex) {
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
            GlobalVars.FortsFile = textBoxFortsFile.Text;
            GlobalVars.SaveForts = checkBoxSaveFortsInfo.Checked;

            new LocationSelect(false).ShowDialog();

            // We set selected values
            text_Latidude.Text = GlobalVars.latitude.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = GlobalVars.longitude.ToString(CultureInfo.InvariantCulture);
            text_Altitude.Text = GlobalVars.altitude.ToString(CultureInfo.InvariantCulture);
            text_MoveRadius.Text = "" + GlobalVars.radius;
        }

        private void ItemsMaxValues_TextChanged(object sender, EventArgs e)
        {
            int totalCount = 0;

            foreach (Control element in this.groupBoxItems.Controls)
                if (element.Name.IndexOf("num_") == 0)
                    totalCount += (int)(element as NumericUpDown).Value;

            text_TotalItemCount.Text = "" + totalCount;
        }

        private void TextBoxes_Throws_TextChanged(object sender, EventArgs e)
        {
            decimal throwsChanceSum = 0;

            throwsChanceSum = text_Pb_Excellent.Value +
                text_Pb_Great.Value +
                text_Pb_Nice.Value +
                text_Pb_Ordinary.Value;
            if (throwsChanceSum > 100) {
                MessageBox.Show(th.TS("You can not have a total throw chance greater than 100%.\nResetting throw chance to 0%!"));
                (sender as NumericUpDown).Value = 0;
            }
        }

        //public class ExtendedWebClient : WebClient
        //{

        //    public int Timeout {
        //        get;
        //        set;
        //    }
            
        //    public ExtendedWebClient()
        //    {
        //        this.Timeout = 2000;//In Milli seconds
        //    }
        //    protected override WebRequest GetWebRequest(Uri address)
        //    {
        //        var objWebRequest = base.GetWebRequest(address);
        //        objWebRequest.Timeout = this.Timeout;
        //        return objWebRequest;
        //    }
        //}

        //public bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        //{
        //    return true;
        //}

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(th.TS("This will capture pokemons while walking spiral, and will use pokestops which are within 30 meters of the path projected."));
        }

        //private void button2_Click_1(object sender, EventArgs e)
        //{
        //    System.Windows.Forms.Form update = new Update();
        //    this.Hide();
        //    update.Show();
        //}

        private void buttonSvProf_Click_2(object sender, EventArgs e)
        {
            if (Save())
                MessageBox.Show(th.TS("Current Configuration Saved as - ") + ProfileName.Text);
            else
                MessageBox.Show(th.TS("Please Review Red Boxes Before Save"));
        }


        void TextBoxes_TextChanged(object sender, EventArgs e)
        {
            ((TextBox)sender).BackColor = SystemColors.Window;
        }


        private void button_Information_Click(object sender, EventArgs e)
        {
            var message = th.TS("Since the new API was cracked by the pogodev team, they have choosen to make the API pay2use We did not have any influence on this. We are very sorry this needed to happen!");
            var title = th.TS("Hashing Information");
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void Button4Click(object sender, EventArgs e)
        {
            new AvatarSelect().ShowDialog();
        }

        void checkBox_UseProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxy.Checked) {
                prxyIP.Enabled = true;
                prxyPort.Enabled = true;
            } else {
                prxyIP.Enabled = false;
                prxyPort.Enabled = false;
            }
        }
        void checkBox_UseProxyAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxyAuth.Checked) {
                prxyUser.Enabled = true;
                prxyPass.Enabled = true;
            } else {
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
                case 5:
                    lang = "zh";
                    break;
                case 6:
                    lang = "it";
                    break;
            }

            if (lang != "") {
                TranslatorHelper.DownloadTranslationFile("PokemonGo.RocketAPI.Console/Lang", GlobalVars.PathToTranslations, lang);
                th.SelectLanguage(lang);
                th.Translate(this);
                tabControl1.SizeMode = TabSizeMode.Normal;
                tabControl1.SizeMode = TabSizeMode.Fixed;
                
            }
        }

        void checkBox_AlwaysTransfer_CheckedChanged(object sender, EventArgs e)
        {
            var i = 0;
            while (i < checkedListBox_AlwaysTransfer.Items.Count) {
                checkedListBox_AlwaysTransfer.SetItemChecked(i, (sender as CheckBox).Checked);
                i++;
            }
        }
        void checkBoxStoreUntranslated_CheckedChanged(object sender, EventArgs e)
        {
            TranslatorHelper.StoreUntranslated = (sender as CheckBox).Checked;
        }
        void checkBoxExtractText_CheckedChanged(object sender, EventArgs e)
        {
            TranslatorHelper.ActiveExtractTexts = (sender as CheckBox).Checked;
        }
        void buttonSelectFile_Click(object sender, EventArgs e)
        {
            if (textBoxFortsFile.Text == "")
                textBoxFortsFile.Text = GlobalVars.PathToConfigs + "\\forts.json";
            saveFileDialog1.FileName = textBoxFortsFile.Text;
            saveFileDialog1.FilterIndex = 1;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                textBoxFortsFile.Text = saveFileDialog1.FileName;
            }
        }
        void ButtonGenerateID_Click(object sender, EventArgs e)
        {
            textBoxDeviceID.Text = DeviceSetup.RandomDeviceId(isIOS()?32:16);
        }

        bool isIOS()
        {
            var devicename = comboBox_Device.Text.ToLower();
            return (devicename.Contains("iphone") || devicename.Contains("ipad")|| devicename.Contains("ipod"));
        }

        void checkBox_UseBreakIntervalAndLength_CheckedChanged(object sender, EventArgs e)
        {
            text_BreakInterval.Enabled = (sender as CheckBox).Checked;
            text_BreakLength.Enabled = (sender as CheckBox).Checked;
        }
        void checkBox_RandomlyReduceSpeed_CheckedChanged(object sender, EventArgs e)
        {
            text_MinWalkSpeed.Enabled = (sender as CheckBox).Checked;
        }
        void checkBox_UseGoogleMapsRouting_CheckedChanged(object sender, EventArgs e)
        {
            text_GoogleMapsAPIKey.Enabled = (sender as CheckBox).Checked;
        }
        void checkBoxSaveFortsInfo_CheckedChanged(object sender, EventArgs e)
        {
            textBoxFortsFile.Enabled = (sender as CheckBox).Checked;
            buttonSelectFile.Enabled = (sender as CheckBox).Checked;
        }
        void checkBoxSaveLocations_CheckedChanged(object sender, EventArgs e)
        {
            numMinIVSave.Enabled = (sender as CheckBox).Checked;
            textBoxSaveLocationsFile.Enabled = (sender as CheckBox).Checked;
            buttonSelectLocationFile.Enabled = (sender as CheckBox).Checked;
        }
        void buttonSelectLocationFile_Click(object sender, EventArgs e)
        {
            if (textBoxSaveLocationsFile.Text == "")
                textBoxSaveLocationsFile.Text = GlobalVars.PathToConfigs + "\\pokemons.txt";
            saveFileDialog1.FileName = textBoxSaveLocationsFile.Text;
            saveFileDialog1.FilterIndex = 2;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                textBoxSaveLocationsFile.Text = saveFileDialog1.FileName;
            }
        }

        void buttonTest_Click(object sender, EventArgs e)
        {
            var port = 80;
            int.TryParse(prxyPort.Text, out port);
            MessageBox.Show(th.TS(testProxy(prxyIP.Text, port, prxyUser.Text, prxyPass.Text)));
        }

        private string testProxy(string proxyHost, int proxyPort, string proxyUsername, string proxyPassword)
        {
            if ((proxyHost == "") || (proxyPort == 0))
                return "Proxy Host or Port is wrong";
            try {
                var proxyUri = $"http://{proxyHost}:{proxyPort}";
                Logger.Info("proxyUri: " + proxyUri);
                Logger.Info("proxyUsername: " + proxyUsername);
                Logger.Info("proxyPassword: " + (proxyPassword != ""));
                var p = new WebProxy(new System.Uri(proxyUri), false, null);

                if (proxyUsername != "")
                    p.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
                var wc = new WebClient();
                wc.Proxy = p;
                wc.DownloadData("https://pokemaster.me");
                return "Proxy is OK";
            } catch (Exception ex) {
                return ex.Message;
            }
        }
        void checkBoxContinue_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVars.ContinueLatestSession = (sender as CheckBox).Checked;
        }
        void label66_DoubleClick(object sender, EventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift) {
                new KeysManager().ShowDialog();
            }
        }
        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var isChecked = (sender as CheckBox).Checked;
            for (var i = 0; i < checkedListBox_Pinap.Items.Count; i++) {
                checkedListBox_Pinap.SetItemChecked(i, isChecked);
            }
        }
        void button1_Click(object sender, EventArgs e)
        {
            new KeysManager().ShowDialog();
            if (!string.IsNullOrEmpty(GlobalVars.pFHashKey))
                pFHashKey.Text = GlobalVars.pFHashKey;
        }
        void label15_DoubleClick(object sender, EventArgs e)
        {
            textBoxDiscordServerID.Enabled |= Control.ModifierKeys == Keys.Shift;

        }

        private void AdvancedBreaks_CheckedChanged(object sender, EventArgs e)
        {
            if (AdvancedBreaks.Checked)
            {
                // Disable Basic Breaks
                checkBox_UseBreakIntervalAndLength.Checked = false;
                checkBox_UseBreakIntervalAndLength.Enabled = false;
                text_BreakInterval.Enabled = false;
                text_BreakLength.Enabled = false;

                // Enable BreakGrid
                BreakGridView.Visible = true;
                BreakGridView.Enabled = true;
            }
            else
            {
                // Disable Basic Breaks
                checkBox_UseBreakIntervalAndLength.Enabled = true;
                text_BreakInterval.Enabled = true;
                text_BreakLength.Enabled = true;

                // Disable BreakGrid
                BreakGridView.Visible = false;
                BreakGridView.Enabled = false;
            }
        }

        private void ConfigWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void text_Speed_TextChanged(object sender, EventArgs e)
        {
            if (text_Speed.Text.Length >= 1 && text_Speed.Text.All(char.IsDigit))
                toolTip1.SetToolTip(text_Speed, Convert.ToDouble(text_Speed.Text) * 1000 / 3600 + " m/s");
        }

        private void text_MinWalkSpeed_TextChanged(object sender, EventArgs e)
        {
            if (text_MinWalkSpeed.Text.Length >= 1 && text_MinWalkSpeed.Text.All(char.IsDigit))
                toolTip1.SetToolTip(text_MinWalkSpeed, Convert.ToDouble(text_MinWalkSpeed.Text) * 1000 / 3600 + " m/s");
        }

        private double StrCordToDouble(string str)
        {
            double ret = 0.0;
            double.TryParse(str.Replace(",", "."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out ret);
            return ret;
        }
    }
}
