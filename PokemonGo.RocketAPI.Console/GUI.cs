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

        public GUI()
        {
            InitializeComponent();
        }       

        private void GUI_Load(object sender, EventArgs e)
        {
            _clientSettings = new Settings();

            var ret = MessageBox.Show("The Bot isn't done! Be aware that you can get banned!\n\nDon't login with the new App Version (0.3.7) (0.3.5 is ok!)\n\nOr you will probably get Banned if you use the bot again!\n\nAre you sure you want to continue?","Warning", MessageBoxButtons.YesNo) ;
            if (ret== DialogResult.No){
            	Application.Exit();
            }        	

            Directory.CreateDirectory(Program.path);
            Directory.CreateDirectory(Program.path_translation);
            Directory.CreateDirectory(Program.path_pokedata);
            Directory.CreateDirectory(devicePath);
            Directory.CreateDirectory(PokeDataPath);


            if(File.Exists($@"{baseDirectory}\update.bat"))
                File.Delete($@"{baseDirectory}\update.bat");

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

            foreach(var extract in pokeData)
                Extract("PokemonGo.RocketAPI.Console", Program.path_pokedata, "PokeData", extract);

            TranslationHandler.Init();

            #region Loading everything
            comboBox_AccountType.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };
            comboBox_AccountType.DataSource = types;


            var pokeIDS = new Dictionary<string, int>();
            var evolveIDS = new Dictionary<string, int>();
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
                    i++;
                }
            }

            try
            {
                if (File.Exists(Program.account))
                {
                    string[] lines = File.ReadAllLines(@Program.account);
                    i = 1;
                    foreach (string line in lines)
                    {
                        switch (i)
                        {
                            case 1:
                                if (line == "Google")
                                    comboBox_AccountType.SelectedIndex = 0;
                                else
                                    comboBox_AccountType.SelectedIndex = 1;
                                break;
                            case 2:
                                text_EMail.Text = line;
                                break;
                            case 3:
                                text_Password.Text = line;
                                break;
                            case 4:
                                text_Latidude.Text = line;
                                break;
                            case 5:
                                text_Longitude.Text = line;
                                break;
                            case 6:
                                text_Altidude.Text = line;
                                break;
                            case 7:
                                text_Speed.Text = line;
                                break;
                            case 8:
                                text_MoveRadius.Text = line;
                                break;
                            case 9:
                                checkBox_Start_Walk_from_default_location.Checked = bool.Parse(line);
                                break;
                            case 10:
                                checkBox_AutoTransferDoublePokemon.Checked = bool.Parse(line);
                                break;
                            case 11:
                                text_MaxDuplicatePokemon.Text = line;
                                break;
                            case 12:
                                checkBox_EvolvePokemonIfEnoughCandy.Checked = bool.Parse(line);
                                break;
                            case 13:
                                text_MaxCPToTransfer.Text = line;
                                break;
                            case 14:
                                text_Telegram_Token.Text = line;
                                break;
                            case 15:
                                text_Telegram_Name.Text = line;
                                break;
                            case 16:
                                text_Telegram_LiveStatsDelay.Text = line;
                                break;
                            case 17:
                                //if (line == "1")
                                //{
                                //    Globals.navigation_option = 1;
                                //    checkBox8.Checked = true;
                                //    checkBox_UseLuckyEggAtEvolve.Checked = false;
                                //} else
                                //{
                                //    Globals.navigation_option = 2;
                                //    checkBox_UseLuckyEggAtEvolve.Checked = true;
                                //    checkBox8.Checked = false;
                                //}
                                break;
                            case 18:
                                checkBox_UseLuckyEggAtEvolve.Checked = bool.Parse(line);
                                break;
                            case 19:
                                checkBox_UseIncenseEvery30min.Checked = bool.Parse(line);
                                break;
                            case 20:
                                text_MaxIVToTransfer.Text = line;
                                break;
                            case 21:
                                checkBox_EnablePokemonListGui.Checked = bool.Parse(line);
                                break;
                            case 22:
                                checkBox_KeepPokemonWhichCanBeEvolved.Checked = bool.Parse(line);
                                break;
                            case 23:
                                checkBox_UseLuckyEggIfNotRunning.Checked = bool.Parse(line);
                                break;
                            case 24:
                                checkBox_AutoIncubate.Checked = bool.Parse(line);
                                chkAutoIncubate_CheckedChanged(null, EventArgs.Empty);
                                break;
                            case 25:
                                checkBox_UseBasicIncubators.Checked = bool.Parse(line);
                                break;
                            case 26:
                                checkBox_TransferFirstLowIV.Checked = bool.Parse(line);
                                break;
                            case 27:
                                //langSelected = line;
                                break;
                            case 28:
                                checkBox_UseRazzberryIfChanceUnder.Checked = bool.Parse(line);
                                break;
                            case 29:
                                text_UseRazzberryChance.Text = line;
                                break;
                            case 30:
                                checkBox_RandomSleepAtCatching.Checked = bool.Parse(line);
                                break;
                            case 31:
                                checkBox_FarmPokestops.Checked = bool.Parse(line);
                                break;
                            case 32:
                                text_Telegram_Token.Text = line;
                                break;
                            case 34:
                                checkbox_PWDEncryption.Checked = bool.Parse(line);
                                break;
                            //case 35:
                            //    checkBox_EnableItemsListGui.Checked = bool.Parse(line);
                                //break;
                            //case 35:
                            //    checkBox_CatchLurePokemons.Checked = bool.Parse(line);
                            //    break;
                            default:
                                break;
                        }
                        i++;
                    }
		            if (checkbox_PWDEncryption.Checked)
		            	text_Password.Text = Encryption.Decrypt(text_Password.Text);
                }
                else
                {
                    text_Latidude.Text = "40,764883";
                    text_Longitude.Text = "-73,972967";
                    text_Altidude.Text = "10";
                    text_Speed.Text = "50";
                    text_MoveRadius.Text = "5000";
                    text_MaxDuplicatePokemon.Text = "3";
                    text_MaxCPToTransfer.Text = "999";
                    text_Telegram_LiveStatsDelay.Text = "5000";
                }
            } catch (Exception)
            {
                MessageBox.Show("Your Config is broken, check if every setting is right!");
            }

            if (File.Exists(Program.throws))
            {
                string[] lines = File.ReadAllLines(@Program.throws);
                i = 1;
                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            text_Pb_Excellent.Text = line;
                            break;
                        case 2:
                            text_Pb_Great.Text = line;
                            break;
                        case 3:
                            text_Pb_Nice.Text = line;
                            break;
                        case 4:
                            text_Pb_Ordinary.Text = line;
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
            else
            {
                text_Pb_Excellent.Text = "25";
                text_Pb_Great.Text = "25";
                text_Pb_Nice.Text = "25";
                text_Pb_Ordinary.Text = "25";
            }

            if (File.Exists(Program.items))
            {
                string[] lines = File.ReadAllLines(@Program.items);
                i = 1;
                foreach (string line in lines)
                {
                    switch(i)
                    {
                        case 1:
                            text_MaxPokeballs.Text = line;
                            break;
                        case 2:
                            text_MaxGreatBalls.Text = line;
                            break;
                        case 3:
                            text_MaxUltraBalls.Text = line;
                            break;
                        case 4:
                            text_MaxRevives.Text = line;
                            break;
                        case 5:
                            text_MaxPotions.Text = line;
                            break;
                        case 6:
                            text_MaxSuperPotions.Text = line;
                            break;
                        case 7:
                            text_MaxHyperPotions.Text = line;
                            break;
                        case 8:
                            text_MaxRazzBerrys.Text = line;
                            break;
                        case 9:
                            text_MaxMasterBalls.Text = line;
                            break;
                        case 10:
                            text_MaxTopPotions.Text = line;
                            break;
                        case 11:
                            text_MaxTopRevives.Text = line;
                            break;
                        default:
                            break;
                    }
                    i++;
                }
            }
            else
            {
                text_MaxPokeballs.Text = "100";
                text_MaxGreatBalls.Text = "100";
                text_MaxUltraBalls.Text = "100";
                text_MaxRevives.Text = "100";
                text_MaxPotions.Text = "100";
                text_MaxSuperPotions.Text = "100";
                text_MaxHyperPotions.Text = "100";
                text_MaxRazzBerrys.Text = "100";
                text_MaxMasterBalls.Text = "0";
                text_MaxTopPotions.Text = "100";
                text_MaxTopRevives.Text = "100";
            }

            if (File.Exists(Program.miscSettings))
            {
                string[] lines = File.ReadAllLines(Program.miscSettings);
                i = 1;

                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            logPokemon.Checked = bool.Parse(line);
                        break;
                        case 2:
                            logManuelTransfer.Checked = bool.Parse(line);
                        break;
                        case 3:
                            logEvolution.Checked = bool.Parse(line);
                        break;
                        case 4:
                            checkbox_LogEggs.Checked = bool.Parse(line);
                        break;
                    }
                    i++;
                }
            }

            if (File.Exists(Program.walkSetting))
            {

                string[] lines = File.ReadAllLines(@Program.walkSetting);
                i = 1;
                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            checkBox_FarmPokestops.Checked = bool.Parse(line);
                            break;
                        case 2:
                            checkBox_CatchPokemon.Checked = bool.Parse(line);
                            break;
                        case 3:
                            checkBox_BreakAtLure.Checked = bool.Parse(line);
                            break;
                        case 4:
                            checkBox_UseLureAtBreak.Checked = bool.Parse(line);
                            break;
                        case 5:
                            checkBox_RandomlyReduceSpeed.Checked = bool.Parse(line);
                            break;
                        case 6:
                            checkBox_UseGoogleMapsRouting.Checked = bool.Parse(line);
                            break;
                        case 7:
                            text_GoogleMapsAPIKey.Text = line;
                            break;
                        case 8:
                            text_PokemonCatchLimit.Text = line;
                            break;
                        case 9:
                            text_PokestopFarmLimit.Text = line;
                            break;
                        case 10:
                            text_XPFarmedLimit.Text = line;
                            break;
                        case 11:
                            text_BreakInterval.Text = line;
                            break;
                        case 12:
                            text_BreakLength.Text = line;
                            break;
                        case 13:
                            text_MinWalkSpeed.Text = line;
                            break;
                        case 14:
                            text_TimeToRun.Text = line;
                            break;
                        case 15:
                            checkBox_WalkInArchimedeanSpiral.Checked = bool.Parse(line);
                            break;
                        case 17:
                            checkBox1.Checked = bool.Parse(line);
                            break;
                    }
                    i++;
                }
            }

            if (File.Exists(Program.keep))
            {
                string[] lines = File.ReadAllLines(@Program.keep);
                foreach (string line in lines)
                {
                    if (line != string.Empty)
                        checkedListBox_PokemonNotToTransfer.SetItemChecked(pokeIDS[line] - 1, true);
                }
            }

            if (File.Exists(Program.ignore))
            {
                string[] lines = File.ReadAllLines(@Program.ignore);
                foreach (string line in lines)
                {
                    if (line != string.Empty)
                        checkedListBox_PokemonNotToCatch.SetItemChecked(pokeIDS[line] - 1, true);
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

            if(File.Exists(Program.updateSettings))
            {
                string[] lines = File.ReadAllLines(Program.updateSettings);
                i = 1;
                foreach(string line in lines)
                {
                    switch(i)
                    {
                        case 1:
                            checkbox_AutoUpdate.Checked = bool.Parse(line);
                        break;
                        case 2:
                            checkbox_checkWhileRunning.Checked = bool.Parse(line);
                        break;
                    }
                    i++;   
                }
            }

            /* VERSION INFORMATION */
            var currVersion = Assembly.GetExecutingAssembly().GetName().Version;
            var newestVersion = Program.getNewestVersion();

            groupBox9.Text = $"Your Version: {currVersion} | Newest: {newestVersion}";

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

            if (File.Exists(Program.evolve))
            {
                string[] lines = File.ReadAllLines(@Program.evolve);
                foreach (string line in lines)
                {
                    if (line != string.Empty)
                        checkedListBox_PokemonToEvolve.SetItemChecked(evolveIDS[line] - 1, true);
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

            #endregion
        }
        //Account Type Changed Event
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.acc = comboBox_AccountType.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;

            if (comboBox_AccountType.SelectedIndex == 0)
                label2.Text = "E-Mail:";
            else
                label2.Text = TranslationHandler.GetString("username", "Username :");
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

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
            ActiveForm.Dispose();
        }

        private void Save()
        {
            #region Setting aaaaaaaaaaaall the globals
            if (text_EMail.Text == string.Empty)
            {
                text_EMail.BackColor = Color.Red;
                return;
            }
            else
                Globals.username = text_EMail.Text;

            if (text_Password.Text == string.Empty)
            {
                text_Password.BackColor = Color.Red;
                return;
            }
            else
                Globals.password = text_Password.Text;

            if (text_Latidude.Text == string.Empty)
            {
                text_Latidude.BackColor = Color.Red;
                return;
            }
            else
                Globals.latitute = double.Parse(text_Latidude.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (text_Longitude.Text == string.Empty)
            {
                text_Longitude.BackColor = Color.Red;
                return;
            }
            else
                Globals.longitude = double.Parse(text_Longitude.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (text_Altidude.Text == string.Empty)
            {
                text_Altidude.BackColor = Color.Red;
                return;
            }
            else
                Globals.altitude = double.Parse(text_Altidude.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (text_Speed.Text == string.Empty)
            {
                text_Speed.BackColor = Color.Red;
                return;
            }
            else
                Globals.speed = double.Parse(text_Speed.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);
            if (Globals.speed > 15)
            {
                var speed = Globals.speed;
                DialogResult dialogResult = MessageBox.Show("The risk of being banned is significantly greater when using higher than human jogging speeds (e.g. > 15km/hr) Click 'No' to use ~10km/hr instead", $"Are you sure you wish to set your speed to {speed} ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //user acknowledges speed risk; do nothing.
                }
                else if (dialogResult == DialogResult.No)
                    Globals.speed = double.Parse("9.5", cords, NumberFormatInfo.InvariantInfo);
            }

            if (!Globals.UseAnimationTimes)
            {
                DialogResult dialogResult = MessageBox.Show("The risk of being banned is significantly greater when when API calls are not timed the same as the Pokemon Go App. Click no to use the application delay at evolve", "Are you sure you wish to disable Animation delay?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //user acknowledges speed risk; do nothing.
                }
                else if (dialogResult == DialogResult.No)
                    Globals.UseAnimationTimes = true;
            }

            if (text_MoveRadius.Text == string.Empty)
            {
                text_MoveRadius.BackColor = Color.Red;
                return;
            }
            else
                Globals.radius = int.Parse(text_MoveRadius.Text);

            if (text_MaxDuplicatePokemon.Text == string.Empty)
            {
                text_MaxDuplicatePokemon.BackColor = Color.Red;
                return;
            }
            else
                Globals.duplicate = int.Parse(text_MaxDuplicatePokemon.Text);

            if (text_MaxCPToTransfer.Text == string.Empty)
            {
                text_MaxCPToTransfer.BackColor = Color.Red;
                return;
            }
            else
                Globals.maxCp = int.Parse(text_MaxCPToTransfer.Text);

            Globals.transfer = checkBox_AutoTransferDoublePokemon.Checked;
            Globals.defLoc = checkBox_Start_Walk_from_default_location.Checked;
            Globals.evolve = checkBox_EvolvePokemonIfEnoughCandy.Checked;

            if (text_Pb_Excellent.Text == string.Empty)
            {
                text_Pb_Excellent.BackColor = Color.Red;
                return;
            }
            else
                Globals.excellentthrow = int.Parse(text_Pb_Excellent.Text);

            if (text_Pb_Great.Text == string.Empty)
            {
                text_Pb_Great.BackColor = Color.Red;
                return;
            }
            else
                Globals.greatthrow = int.Parse(text_Pb_Great.Text);

            if (text_Pb_Nice.Text == string.Empty)
            {
                text_Pb_Nice.BackColor = Color.Red;
                return;
            }
            else
                Globals.nicethrow = int.Parse(text_Pb_Nice.Text);

            if (text_Pb_Ordinary.Text == string.Empty)
            {
                text_Pb_Ordinary.BackColor = Color.Red;
                return;
            }
            else
                Globals.ordinarythrow = int.Parse(text_Pb_Ordinary.Text);

            if (text_MaxPokeballs.Text == string.Empty)
            {
                text_MaxPokeballs.BackColor = Color.Red;
                return;
            }
            else
                Globals.pokeball = int.Parse(text_MaxPokeballs.Text);

            if (text_MaxGreatBalls.Text == string.Empty)
            {
                text_MaxGreatBalls.BackColor = Color.Red;
                return;
            }
            else
                Globals.greatball = int.Parse(text_MaxGreatBalls.Text);

            if (text_MaxUltraBalls.Text == string.Empty)
            {
                text_MaxUltraBalls.BackColor = Color.Red;
                return;
            }
            else
                Globals.ultraball = int.Parse(text_MaxUltraBalls.Text);

            if (text_MaxRevives.Text == string.Empty)
            {
                text_MaxRevives.BackColor = Color.Red;
                return;
            }
            else
                Globals.revive = int.Parse(text_MaxRevives.Text);

            if (text_MaxPotions.Text == string.Empty)
            {
                text_MaxPotions.BackColor = Color.Red;
                return;
            }
            else
                Globals.potion = int.Parse(text_MaxPotions.Text);

            if (text_MaxSuperPotions.Text == string.Empty)
            {
                text_MaxSuperPotions.BackColor = Color.Red;
                return;
            }
            else
                Globals.superpotion = int.Parse(text_MaxSuperPotions.Text);

            if (text_MaxHyperPotions.Text == string.Empty)
            {
                text_MaxHyperPotions.BackColor = Color.Red;
                return;
            }
            else
                Globals.hyperpotion = int.Parse(text_MaxHyperPotions.Text);

            if (text_MaxRazzBerrys.Text == string.Empty)
            {
                text_MaxRazzBerrys.BackColor = Color.Red;
                return;
            }
            else
                Globals.berry = int.Parse(text_MaxRazzBerrys.Text);

            if (text_Telegram_Token.Text != string.Empty)
                Globals.telAPI = text_Telegram_Token.Text;

            if (text_Telegram_Name.Text != string.Empty)
                Globals.telName = text_Telegram_Name.Text;

            if (text_Telegram_LiveStatsDelay.Text == string.Empty)
            {
                text_Telegram_LiveStatsDelay.BackColor = Color.Red;
                return;
            }
            else
                Globals.telDelay = int.Parse(text_Telegram_LiveStatsDelay.Text);

            if (text_MaxTopPotions.Text == string.Empty)
            {
                text_MaxTopPotions.BackColor = Color.Red;
            }
            else
            {
                Globals.toppotion = int.Parse(text_MaxTopPotions.Text);
            }

            if (text_MaxMasterBalls.Text == string.Empty)
            {
                text_MaxMasterBalls.BackColor = Color.Red;
            }
            else
            {
                Globals.masterball = int.Parse(text_MaxMasterBalls.Text);
            }

            if (text_MaxTopRevives.Text == string.Empty)
            {
                text_MaxTopRevives.BackColor = Color.Red;
            }
            else
            {
                Globals.toprevive = int.Parse(text_MaxTopRevives.Text);
            }

            if (text_MaxIVToTransfer.Text == string.Empty)
            {
                text_MaxIVToTransfer.BackColor = Color.Red;
            }
            else
            {
                Globals.ivmaxpercent = int.Parse(text_MaxIVToTransfer.Text);
            }

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

            if (logPokemon.Checked)
            {
                Globals.logPokemons = true;
            }

            if (logManuelTransfer.Checked)
            {
                Globals.logManualTransfer = true;
            }

            if (logEvolution.Checked)
            {
                Globals.bLogEvolve = true;
            }

            if(checkBox_StopWalkingWhenEvolving.Checked)
            {
                Globals.pauseAtEvolve = true;
                Globals.pauseAtEvolve2 = true;
            }

            if(checkbox_AutoUpdate.Checked)
            {
                Globals.AutoUpdate = true;
            }

            if(checkbox_checkWhileRunning.Checked)
            {
                Globals.CheckWhileRunning = true;
            }

            if(checkbox_LogEggs.Checked)
            {
                Globals.LogEggs = true;
            }

            Globals.useincense = checkBox_UseIncenseEvery30min.Checked;
            Globals.pokeList = checkBox_EnablePokemonListGui.Checked;
            //Globals.itemsList = false;
            Globals.keepPokemonsThatCanEvolve = checkBox_KeepPokemonWhichCanBeEvolved.Checked;
            Globals.pokevision = checkBox1.Checked;
            Globals.useLuckyEggIfNotRunning = checkBox_UseLuckyEggIfNotRunning.Checked;
            Globals.userazzberry = checkBox_UseRazzberryIfChanceUnder.Checked;
            Globals.TransferFirstLowIV = checkBox_TransferFirstLowIV.Checked;
            Globals.settingsLanguage = langSelected;
            Globals.sleepatpokemons = checkBox_RandomSleepAtCatching.Checked;
            Globals.Espiral = checkBox_WalkInArchimedeanSpiral.Checked;

            if (text_TimeToRun.Text == String.Empty)
                Globals.TimeToRun = 0;
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

            #endregion

            #region CreatingSettings
            string[] accFile = {
                    Globals.acc.ToString(), // 1
                    Globals.username,
                    Globals.password,
                    Globals.latitute.ToString(),
                    Globals.longitude.ToString(),
                    Globals.altitude.ToString(),
                    Globals.speed.ToString(),
                    Globals.radius.ToString(),
                    Globals.defLoc.ToString(),
                    Globals.transfer.ToString(), // 10
                    Globals.duplicate.ToString(),
                    Globals.evolve.ToString(),
                    Globals.maxCp.ToString(),
                    Globals.telAPI,
                    Globals.telName,
                    Globals.telDelay.ToString(),
                    Globals.navigation_option.ToString(),
                    Globals.useluckyegg.ToString(),
                    Globals.useincense.ToString(),
                    Globals.ivmaxpercent.ToString(), // 20
                    Globals.pokeList.ToString(),
                    Globals.keepPokemonsThatCanEvolve.ToString(),
                    Globals.useLuckyEggIfNotRunning.ToString(),
                    Globals.autoIncubate.ToString(),
                    Globals.useBasicIncubators.ToString(),
                    Globals.TransferFirstLowIV.ToString(),
                    Globals.settingsLanguage,
                    Globals.userazzberry.ToString(),
                    Convert.ToInt16(Globals.razzberry_chance * 100).ToString(),
                    Globals.sleepatpokemons.ToString(),  //30
                    Globals.farmPokestops.ToString(),
                    Globals.telAPI.ToString(),
                    Globals.pauseAtEvolve.ToString(),
                    Globals.usePwdEncryption.ToString(),
                    //Globals.itemsList.ToString(),
                    //Globals.CatchLurePokemons.ToString()
            };
            if (Globals.usePwdEncryption)
            {
            	accFile[2] = Encryption.Encrypt(accFile[2]);
            }            
            File.WriteAllLines(@Program.account, accFile);

            string[] throwsFile = {
                Globals.excellentthrow.ToString(),
                Globals.greatthrow.ToString(),
                Globals.nicethrow.ToString(),
                Globals.ordinarythrow.ToString()
            };
            File.WriteAllLines(Program.throws, throwsFile);

            string[] itemsFile = {
                    Globals.pokeball.ToString(),
                    Globals.greatball.ToString(),
                    Globals.ultraball.ToString(),
                    Globals.revive.ToString(),
                    Globals.potion.ToString(),
                    Globals.superpotion.ToString(),
                    Globals.hyperpotion.ToString(),
                    Globals.berry.ToString(),
                    Globals.masterball.ToString(),
                    Globals.toppotion.ToString(),
                    Globals.toprevive.ToString()
            };
            File.WriteAllLines(@Program.items, itemsFile);

            string[] updateFile =
            {
                checkbox_AutoUpdate.Checked.ToString(),
                checkbox_checkWhileRunning.Checked.ToString(),
            };
            File.WriteAllLines(@Program.updateSettings, updateFile);

            string[] miscFile =
            {
                Globals.logPokemons.ToString(),
                Globals.logManualTransfer.ToString(),
                Globals.bLogEvolve.ToString(),
                Globals.LogEggs.ToString(),

            };
            File.WriteAllLines(@Program.miscSettings, miscFile);

            string[] walkSettingsFile =
            {
                Globals.farmPokestops.ToString(),
                Globals.CatchPokemon.ToString(),
                Globals.BreakAtLure.ToString(),
                Globals.UseLureAtBreak.ToString(),
                Globals.RandomReduceSpeed.ToString(),
                Globals.UseGoogleMapsAPI.ToString(),
                text_GoogleMapsAPIKey.Text,
                text_PokemonCatchLimit.Text,
                text_PokestopFarmLimit.Text,
                text_XPFarmedLimit.Text,
                text_BreakInterval.Text,
                text_BreakLength.Text,
                text_MinWalkSpeed.Text,
                text_TimeToRun.Text,
                Globals.Espiral.ToString(),
                Globals.UseBreakFields.ToString(),
                Globals.pokevision.ToString(),
            };
            File.WriteAllLines(@Program.walkSetting, walkSettingsFile);

            string[] temp = new string[200];
            int i = 0;
            foreach (PokemonId pokemon in Globals.noTransfer)
            {
                temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] noTransFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(Program.@keep, noTransFile);

            i = 0;
            Array.Clear(temp, 0, temp.Length);
            foreach (PokemonId pokemon in Globals.noCatch)
            {
                temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] noCatchFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(@Program.ignore, noCatchFile);

            Array.Clear(temp, 0, temp.Length);
            i = 0;
            foreach (PokemonId pokemon in Globals.doEvolve)
            {
                temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] EvolveFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(@Program.evolve, EvolveFile);
            #endregion
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

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            Globals.useluckyegg = checkBox_UseLuckyEggAtEvolve.Checked;
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            Globals.userazzberry = checkBox_UseRazzberryIfChanceUnder.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            while (checkedListBox_PokemonNotToTransfer.Items.Count > 0)
            {
                checkedListBox_PokemonNotToTransfer.Items.RemoveAt(0);
                checkedListBox_PokemonNotToCatch.Items.RemoveAt(0);
                if (checkedListBox_PokemonToEvolve.Items.Count > 0)
                {
                    checkedListBox_PokemonToEvolve.Items.RemoveAt(0);
                }
            }
            int i = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    checkedListBox_PokemonNotToTransfer.Items.Add(pokemon.ToString());
                    checkedListBox_PokemonNotToCatch.Items.Add(pokemon.ToString());
                    if (!(evolveBlacklist.Contains(i)))
                    {
                        checkedListBox_PokemonToEvolve.Items.Add(pokemon.ToString());
                    } 
                    i++;
                }
            }
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            Globals.TransferFirstLowIV = checkBox_TransferFirstLowIV.Checked;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            Globals.keepPokemonsThatCanEvolve = checkBox_KeepPokemonWhichCanBeEvolved.Checked;
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            Globals.useLuckyEggIfNotRunning = checkBox_UseLuckyEggIfNotRunning.Checked;
        }

        private void chkAutoIncubate_CheckedChanged(object sender, EventArgs e)
        {
            Globals.autoIncubate = checkBox_AutoIncubate.Checked;
            checkBox_UseBasicIncubators.Enabled = checkBox_AutoIncubate.Checked;
        }
        
        private void chkPWDEncryption_CheckedChanged(object sender, EventArgs e)
        {
            Globals.usePwdEncryption = checkbox_PWDEncryption.Checked;
            
        }

        private void chkUseBasicIncubators_CheckedChanged(object sender, EventArgs e)
        {
            Globals.useBasicIncubators = checkBox_UseBasicIncubators.Checked;
        }

        #endregion CheckedChanged Events

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
            LocationSelect locationSelector = new LocationSelect(false);
            locationSelector.ShowDialog();
            text_Latidude.Text = Globals.latitute.ToString(CultureInfo.InvariantCulture);
            text_Longitude.Text = Globals.longitude.ToString(CultureInfo.InvariantCulture);
            text_Altidude.Text = Globals.altitude.ToString(CultureInfo.InvariantCulture);
        }

        private void TextBoxes_Items_TextChanged(object sender, EventArgs e)
        {
            int itemSumme = 0;

            if (text_MaxPokeballs.Text != string.Empty && text_MaxGreatBalls.Text != string.Empty && text_MaxUltraBalls.Text != string.Empty && text_MaxRevives.Text != string.Empty && text_MaxPotions.Text != string.Empty && text_MaxSuperPotions.Text != string.Empty && text_MaxHyperPotions.Text != string.Empty && text_MaxRazzBerrys.Text != string.Empty && text_MaxMasterBalls.Text != string.Empty && text_MaxTopPotions.Text != string.Empty && text_MaxTopRevives.Text != string.Empty)
            {
                itemSumme = Convert.ToInt16(text_MaxPokeballs.Text) +
                            Convert.ToInt16(text_MaxGreatBalls.Text) +
                            Convert.ToInt16(text_MaxUltraBalls.Text) +
                            Convert.ToInt16(text_MaxRevives.Text) +
                            Convert.ToInt16(text_MaxPotions.Text) +
                            Convert.ToInt16(text_MaxSuperPotions.Text) +
                            Convert.ToInt16(text_MaxHyperPotions.Text) +
                            Convert.ToInt16(text_MaxRazzBerrys.Text) +
                            Convert.ToInt16(text_MaxMasterBalls.Text) +
                            Convert.ToInt16(text_MaxTopRevives.Text) +
                            Convert.ToInt16(text_MaxTopPotions.Text);
            }

            text_TotalItemCount.Text = Convert.ToString(itemSumme);
        }

        private void TextBoxes_Throws_TextChanged(object sender, EventArgs e)
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
            //label2.Text = TranslationHandler.getString("username", "Username:");
            label3.Text = TranslationHandler.GetString("password", "Password:");
            groupBox2.Text = TranslationHandler.GetString("locationSettings", "Location Settings");
            label7.Text = TranslationHandler.GetString("speed", "Speed:");
            label9.Text = TranslationHandler.GetString("moveRadius", "Move Radius:");
            label10.Text = TranslationHandler.GetString("meters", "meters");
            checkBox_Start_Walk_from_default_location.Text = TranslationHandler.GetString("startFromDefaultLocation", "Start from default location");
            groupBox3.Text = TranslationHandler.GetString("botSettings", "Bot Settings");
            checkBox_AutoTransferDoublePokemon.Text = TranslationHandler.GetString("autoTransferDoublePokemon", "Auto transfer double Pokemons");
            label11.Text = TranslationHandler.GetString("maxDupPokemon", "Max. duplicate Pokemons");
            label12.Text = TranslationHandler.GetString("maxCPtransfer", "Max. CP to transfer:");
            label28.Text = TranslationHandler.GetString("maxIVtransfer", "Max. IV to transfer:");
            groupBox8.Text = TranslationHandler.GetString("telegramSettings", "Telegram Settings");
            //label30.Text = TranslationHandler.GetString("infoline1", "This Bot is absolutely free and open source! Chargeback if you've paid for it!");
            //label32.Text = TranslationHandler.GetString("infoline2", "Whenever you encounter something related to 'Pokecrot', tell them the Bot is stolen!");
            label13.Text = TranslationHandler.GetString("maxPokeballs", "Max. Pokeballs:");
            label14.Text = TranslationHandler.GetString("maxGreatballs", "Max. GreatBalls:");
            label15.Text = TranslationHandler.GetString("maxUltraballs", "Max. UltraBalls:");
            label26.Text = TranslationHandler.GetString("maxMasterballs", "Max. MasterBalls:");
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
            Process.Start("https://twitter.com/LoganPunkt");
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_UseProxy.Checked)
            {
                button1.Enabled = false;
                prxyIP.Enabled = true;
                prxyPort.Enabled = true;
                UserSettings.Default.UseProxyVerified = true;
            }
            else
            {
                button1.Enabled = true;
                prxyIP.Enabled = false;
                prxyPort.Enabled = false;
                UserSettings.Default.UseProxyVerified = false;
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
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

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            Globals.sleepatpokemons = checkBox_RandomSleepAtCatching.Checked;
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            Globals.farmPokestops = checkBox_FarmPokestops.Checked;
        }

        private void textBox27_TextChanged_1(object sender, EventArgs e)
        {
            double.TryParse(text_TimeToRun.Text, out Globals.TimeToRun);
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            Globals.UseGoogleMapsAPI = checkBox_UseGoogleMapsRouting.Checked;
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo("https://developers.google.com/maps/documentation/directions/start#get-a-key/");
            Process.Start(sInfo);
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            Globals.UseLureAtBreak = checkBox_UseLureAtBreak.Checked;
        }

        private void label46_Click(object sender, EventArgs e)
        {

        }

        private void textBox30_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_PokemonCatchLimit.Text, out Globals.PokemonCatchLimit);
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_PokestopFarmLimit.Text, out Globals.PokestopFarmLimit);
        }

        private void textBox32_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_XPFarmedLimit.Text, out Globals.XPFarmedLimit);
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_BreakInterval.Text, out Globals.BreakInterval);
        }

        private void textBox29_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_BreakLength.Text, out Globals.BreakLength);
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(text_MinWalkSpeed.Text, out Globals.MinWalkSpeed);
        }

        private void checkBox18_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            Globals.CatchPokemon = checkBox_CatchPokemon.Checked;
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            Globals.BreakAtLure = checkBox_BreakAtLure.Checked;
        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {
            Globals.UseAnimationTimes = checkBox_SimulateAnimationTimeAtEvolve.Checked;
        }

        private void checkBox_RandomlyReduceSpeed_CheckedChanged(object sender, EventArgs e)
        {
            Globals.RandomReduceSpeed = checkBox_RandomlyReduceSpeed.Checked;
        }

        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            Globals.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;
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
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This will capture pokemons while walking spiral, and will use pokestops which are within 30 meters of the path projected.");
        }

        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Espiral = checkBox_WalkInArchimedeanSpiral.Checked;
        }

        private void checkBox26_CheckedChanged(object sender, EventArgs e)
        {
            Globals.UseBreakFields = checkBox_UseBreakIntervalAndLength.Checked;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Form update = new Update();
            this.Hide();
            update.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Globals.pokevision = checkBox1.Checked;
        }

        //private void checkBox_CatchLurePokemons_CheckedChanged(object sender, EventArgs e)
        //{
        //    Globals.CatchLurePokemons = checkBox_CatchLurePokemons.Checked;
        //}
    }
}
