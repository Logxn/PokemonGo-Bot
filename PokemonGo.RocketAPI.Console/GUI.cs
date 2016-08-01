using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public partial class GUI : Form
    {
        public static NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        public static int[] evolveBlacklist = {
            3, 6, 9, 12, 15, 18, 20, 22, 24, 26, 28, 31, 34, 36, 38, 40, 42, 45, 47, 49, 51, 53, 55, 57, 59, 62, 65, 68, 71, 73, 76, 78, 80, 82, 83, 85, 87, 89, 91, 94, 95, 97, 99, 101, 103, 105, 106, 107, 108, 110, 112, 113, 114, 115, 117, 119, 121, 122, 123, 124, 125, 126, 127, 128, 130, 131, 132, 134, 135, 136, 137, 139, 141, 142, 143, 144, 145, 146, 149, 150, 151
        };
        public static Dictionary<string, string> gerEng = new Dictionary<string, string>();

        public GUI()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.acc = accountTypeComboBox.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;
            if (accountTypeComboBox.SelectedIndex == 0)
            {
                label2.Text = "E-mail:";
            //    textBox1.Hide();
            //    label2.Hide();
            //    textBox2.Hide();
            //    label3.Hide();
            }
            else
            {
                label2.Text = "Username:";
            //    textBox1.Show();
            //    label2.Show();
            //    textBox2.Show();
            //    label3.Show();
            }
        }

        private void GUI_Load(object sender, EventArgs e)
        {

            // Create missing Files
            System.IO.Directory.CreateDirectory(Program.path); 

            // Version Infoooo
            groupBox9.Text = "Your Version: " + Assembly.GetExecutingAssembly().GetName().Version + " | Newest: " + Program.getNewestVersion();
            if (Program.getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
            { 
                DialogResult dialogResult = MessageBox.Show("There is an Update on Github. do you want to open it ?", "Newest Version: " + Program.getNewestVersion(), MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
                }
                else if (dialogResult == DialogResult.No)
                {
                    //nothing   
                } 
            }

            accountTypeComboBox.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };
            accountTypeComboBox.DataSource = types;

            //textBox1.Hide();
            //label2.Hide();
            //textBox2.Hide();
            //label3.Hide();

            var pokeIDS = new Dictionary<string, int>();
            var evolveIDS = new Dictionary<string, int>();
            int i = 1;
            int ev = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = i;
                    gerEng[StringUtils.getPokemonNameGer(pokemon)] = pokemon.ToString();
                    if (gerPkNamesCheckBox.Checked)
                    {
                        pkToNotTransferCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        pkToNotCatchCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            pkToEvolveCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                            evolveIDS[pokemon.ToString()] = ev;
                            ev++;
                        }
                    }
                    else
                    {
                        pkToNotTransferCListBox.Items.Add(pokemon.ToString());
                        pkToNotCatchCListBox.Items.Add(pokemon.ToString());
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            pkToEvolveCListBox.Items.Add(pokemon.ToString());
                            evolveIDS[pokemon.ToString()] = ev;
                            ev++;
                        }
                    }
                    i++;
                }
            }

            if (File.Exists(Program.account))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.account);
                i = 1;
                int tb = 0;

                TextBox[] tboxes = {
                    emailTBox,
                    pswdTBox,
                    latitudeTBox,
                    longitudeTBox,
                    altitudeTBox,
                    speedTBox,
                    radiusTBox,
                    maxDuplicatePkTBox,
                    maxCPToTransfTBox,
                    tgApiTokenTBox,
                    tgUsernameTBox,
                    tgLiveStatsDelayTBox,
                    maxIVToTransfTBox,
                    alwaysCatchOverCPTBox           
                };

                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            if (line == "Google")
                                accountTypeComboBox.SelectedIndex = 0;
                            else
                                accountTypeComboBox.SelectedIndex = 1;
                            break;
                        case 9:
                            startFromDefaultLocCheckBox.Checked = bool.Parse(line);
                            break;
                        case 10:
                            autoTransfDoublePkCheckBox.Checked = bool.Parse(line);
                            break;
                        case 12:
                            evolvePkIfCandyCheckBox.Checked = bool.Parse(line);
                            break;
                        case 17:
                            //if (line == "1")
                            //{
                            //    Globals.navigation_option = 1;
                            //    checkBox8.Checked = true;
                            //    checkBox7.Checked = false;
                            //} else
                            //{
                            //    Globals.navigation_option = 2;
                            //    checkBox7.Checked = true;
                            //    checkBox8.Checked = false;
                            //}
                            break;
                        case 18:
                            useLuckyEggEvolveCheckBox.Checked = bool.Parse(line);
                            break;
                        case 19:
                            gerPkNamesCheckBox.Checked = bool.Parse(line);
                            break;
                        case 20:
                            useIncense30MinsCheckBox.Checked = bool.Parse(line);
                            break;
                        case 22:
                            enablePkGUICheckBox.Checked = bool.Parse(line);
                            break;
                        case 23:
                            keepEvolvablePkCheckBox.Checked = bool.Parse(line);
                            break;
                        case 24: // reserved for Pokevision
                            //checkBox12.Checked = bool.Parse(line);
                            break;
                        default:
                            if (tb < tboxes.Count()) { 
                                tboxes[tb].Text = line;
                                tb++;
                            }
                            break;
                    }
                    i++;
                }
            }
            else
            {
                latitudeTBox.Text = "40,764883";
                longitudeTBox.Text = "-73,972967";
                altitudeTBox.Text = "10";
                speedTBox.Text = "50";
                radiusTBox.Text = "5000";
                maxDuplicatePkTBox.Text = "3";
                maxCPToTransfTBox.Text = "999";
                maxIVToTransfTBox.Text = "90";
                tgLiveStatsDelayTBox.Text = "5000";
                alwaysCatchOverCPTBox.Text = "1500";
            }

            if (File.Exists(Program.items))
            {
                TextBox[] tboxes = {
                    maxPokeBallsTBox,
                    maxGreatBallsTBox,
                    maxUltraBallsTBox,
                    maxReviveTBox,
                    maxPotTBox,
                    maxSuperPotTBox,
                    maxHyperPotTBox,
                    maxRazzBerrysTBox,
                    maxMasterBallsTBox,
                    maxTopPotTBox,
                    maxTopReviveTBox
                };

                string[] lines = System.IO.File.ReadAllLines(@Program.items);
                i = 0;
                foreach (string line in lines)
                {
                    if (i < tboxes.Count())
                    {
                        tboxes[i].Text = line;
                        i++;
                    }
                }
            }
            else
            {
                maxPokeBallsTBox.Text = "20";
                maxGreatBallsTBox.Text = "50";
                maxUltraBallsTBox.Text = "100";
                maxReviveTBox.Text = "20";
                maxPotTBox.Text = "0";
                maxSuperPotTBox.Text = "0";
                maxHyperPotTBox.Text = "50";
                maxRazzBerrysTBox.Text = "75";
                maxMasterBallsTBox.Text = "200";
                maxTopPotTBox.Text = "100";
                maxTopReviveTBox.Text = "20";
                maxIVToTransfTBox.Text = "90";
            }

            if (File.Exists(Program.keep))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.keep);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (gerPkNamesCheckBox.Checked)
                            pkToNotTransferCListBox.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            pkToNotTransferCListBox.SetItemChecked(pokeIDS[line] - 1, true);
                }
            }

            if (File.Exists(Program.ignore))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.ignore);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (gerPkNamesCheckBox.Checked)
                            pkToNotCatchCListBox.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            pkToNotCatchCListBox.SetItemChecked(pokeIDS[line] - 1, true);
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
                } catch
                {

                }
            }

            if (File.Exists(Program.evolve))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.evolve);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (gerPkNamesCheckBox.Checked)
                            pkToEvolveCListBox.SetItemChecked(evolveIDS[gerEng[line]] - 1, true);
                        else
                            pkToEvolveCListBox.SetItemChecked(evolveIDS[line] - 1, true);
                }
            }

        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '-') && ((sender as TextBox).Text.IndexOf('-') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBoxes_OnlyDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (emailTBox.Text == "")
            {
                emailTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.username = emailTBox.Text;
            if (pswdTBox.Text == "")
            {
                pswdTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.password = pswdTBox.Text;

            if (latitudeTBox.Text == "")
            {
                latitudeTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.latitute = double.Parse(latitudeTBox.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (longitudeTBox.Text == "")
            {
                longitudeTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.longitude = double.Parse(longitudeTBox.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (altitudeTBox.Text == "")
            {
                altitudeTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.altitude = double.Parse(altitudeTBox.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (speedTBox.Text == "")
            {
                speedTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.speed = double.Parse(speedTBox.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (radiusTBox.Text == "")
            {
                radiusTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.radius = int.Parse(radiusTBox.Text);

            if (maxDuplicatePkTBox.Text == "")
            {
                maxDuplicatePkTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.duplicate = int.Parse(maxDuplicatePkTBox.Text);

            if (maxCPToTransfTBox.Text == "")
            {
                maxCPToTransfTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.maxCp = int.Parse(maxCPToTransfTBox.Text);

            Globals.transfer = autoTransfDoublePkCheckBox.Checked;
            Globals.defLoc = startFromDefaultLocCheckBox.Checked;
            Globals.evolve = evolvePkIfCandyCheckBox.Checked;

            if (maxPokeBallsTBox.Text == "")
            {
                maxPokeBallsTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.pokeball = int.Parse(maxPokeBallsTBox.Text);

            if (maxGreatBallsTBox.Text == "")
            {
                maxGreatBallsTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.greatball = int.Parse(maxGreatBallsTBox.Text);

            if (maxUltraBallsTBox.Text == "")
            {
                maxUltraBallsTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.ultraball = int.Parse(maxUltraBallsTBox.Text);

            if (maxReviveTBox.Text == "")
            {
                maxReviveTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.revive = int.Parse(maxReviveTBox.Text);

            if (maxPotTBox.Text == "")
            {
                maxPotTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.potion = int.Parse(maxPotTBox.Text);

            if (maxSuperPotTBox.Text == "")
            {
                maxSuperPotTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.superpotion = int.Parse(maxSuperPotTBox.Text);

            if (maxHyperPotTBox.Text == "")
            {
                maxHyperPotTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.hyperpotion = int.Parse(maxHyperPotTBox.Text);

            if (maxRazzBerrysTBox.Text == "")
            {
                maxRazzBerrysTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.berry = int.Parse(maxRazzBerrysTBox.Text);

            if (tgApiTokenTBox.Text != "")
                Globals.telAPI = tgApiTokenTBox.Text;

            if (tgUsernameTBox.Text != "")
                Globals.telName = tgUsernameTBox.Text;

            if (tgLiveStatsDelayTBox.Text == "")
            {
                tgLiveStatsDelayTBox.BackColor = Color.Red;
                return;
            }
            else
                Globals.telDelay = int.Parse(tgLiveStatsDelayTBox.Text);

            if (maxTopPotTBox.Text == "")
            {
                maxTopPotTBox.BackColor = Color.Red;
                return;
            }
            else
            {
                Globals.toppotion = int.Parse(maxTopPotTBox.Text);
            }

            if (maxMasterBallsTBox.Text == "")
            {
                maxMasterBallsTBox.BackColor = Color.Red;
                return;
            }
            else
            {
                Globals.masterball = int.Parse(maxMasterBallsTBox.Text);
            }

            if (maxTopReviveTBox.Text == "")
            {
                maxTopReviveTBox.BackColor = Color.Red;
                return;
            }
            else
            {
                Globals.toprevive = int.Parse(maxTopReviveTBox.Text);
            }

            if (maxIVToTransfTBox.Text == "")
            {
                maxIVToTransfTBox.BackColor = Color.Red;
                return;
            }
            else
            {
                Globals.ivmaxpercent = int.Parse(maxIVToTransfTBox.Text);
            }

            if (alwaysCatchOverCPTBox.Text == "")
            {
                alwaysCatchOverCPTBox.BackColor = Color.Red;
                return;
            }
            else
            {
                Globals.alwaisCatchOverCP = int.Parse(alwaysCatchOverCPTBox.Text);
            }

            Globals.gerNames = gerPkNamesCheckBox.Checked;
            Globals.useincense = useIncense30MinsCheckBox.Checked;
            Globals.pokeList = enablePkGUICheckBox.Checked;
            Globals.keepPokemonsThatCanEvolve = keepEvolvablePkCheckBox.Checked;
            //Globals.pokevision = checkBox12.Checked;

            foreach (string pokemon in pkToNotTransferCListBox.CheckedItems)
            {
                if (gerPkNamesCheckBox.Checked)
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in pkToNotCatchCListBox.CheckedItems)
            {
                if (gerPkNamesCheckBox.Checked)
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in pkToEvolveCListBox.CheckedItems)
            {
                if (gerPkNamesCheckBox.Checked)
                    Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }

            string[] accFile = {
                    Globals.acc.ToString(),
                    Globals.username,
                    Globals.password,
                    Globals.latitute.ToString(),
                    Globals.longitude.ToString(),
                    Globals.altitude.ToString(),
                    Globals.speed.ToString(),
                    Globals.radius.ToString(),
                    Globals.defLoc.ToString(),
                    Globals.transfer.ToString(),
                    Globals.duplicate.ToString(),
                    Globals.evolve.ToString(),
                    Globals.maxCp.ToString(),
                    Globals.telAPI,
                    Globals.telName,
                    Globals.telDelay.ToString(),
                    Globals.navigation_option.ToString(),
                    Globals.useluckyegg.ToString(),
                    Globals.gerNames.ToString(),
                    Globals.useincense.ToString(),
                    Globals.ivmaxpercent.ToString(),
                    Globals.pokeList.ToString(),
                    Globals.keepPokemonsThatCanEvolve.ToString(),
                    Globals.pokevision.ToString(),
                    Globals.alwaisCatchOverCP.ToString()
            };
            System.IO.File.WriteAllLines(@Program.account, accFile);

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
            System.IO.File.WriteAllLines(@Program.items, itemsFile);

            string[] temp = new string[200];
            int i = 0;
            foreach (PokemonId pokemon in Globals.noTransfer)
            {
                if (gerPkNamesCheckBox.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] noTransFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            System.IO.File.WriteAllLines(Program.@keep, noTransFile);

            i = 0;
            Array.Clear(temp, 0, temp.Length);
            foreach (PokemonId pokemon in Globals.noCatch)
            {
                if (gerPkNamesCheckBox.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] noCatchFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            System.IO.File.WriteAllLines(@Program.ignore, noCatchFile);

            Array.Clear(temp, 0, temp.Length);
            i = 0;
            foreach (PokemonId pokemon in Globals.doEvolve)
            {
                if (gerPkNamesCheckBox.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);   
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }
            string[] EvolveFile = temp.Where(x => !String.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(@Program.evolve, EvolveFile);

            ActiveForm.Dispose();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllPkToNotTransferCheckBox.Checked)
            {
                int i = 0;
                while (i < pkToNotTransferCListBox.Items.Count)
                {
                    pkToNotTransferCListBox.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < pkToNotTransferCListBox.Items.Count)
                {
                    pkToNotTransferCListBox.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllPkToNotCatchCheckBox.Checked)
            {
                int i = 0;
                while (i < pkToNotCatchCListBox.Items.Count)
                {
                    pkToNotCatchCListBox.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < pkToNotCatchCListBox.Items.Count)
                {
                    pkToNotCatchCListBox.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (selectAllPkToEvolveCheckBox.Checked)
            {
                int i = 0;
                while (i < pkToEvolveCListBox.Items.Count)
                {
                    pkToEvolveCListBox.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < pkToEvolveCListBox.Items.Count)
                {
                    pkToEvolveCListBox.SetItemChecked(i, false);
                    i++;
                }
            }
        }

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

        private void checkBox7_CheckedChanged_1(object sender, EventArgs e)
        {
            if (useLuckyEggEvolveCheckBox.Checked)
            {
                Globals.useluckyegg = true;
            }
            else
            {
                Globals.useluckyegg = false;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            while (pkToNotTransferCListBox.Items.Count > 0)
            {
                pkToNotTransferCListBox.Items.RemoveAt(0);
                pkToNotCatchCListBox.Items.RemoveAt(0);
                if (pkToEvolveCListBox.Items.Count > 0)
                    pkToEvolveCListBox.Items.RemoveAt(0);
            }
            int i = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    if (gerPkNamesCheckBox.Checked)
                    {
                        pkToNotTransferCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        pkToNotCatchCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            pkToEvolveCListBox.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        }
                    }
                    else
                    {
                        pkToNotTransferCListBox.Items.Add(pokemon.ToString());
                        pkToNotCatchCListBox.Items.Add(pokemon.ToString());
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            pkToEvolveCListBox.Items.Add(pokemon.ToString());
                        }
                    }
                    i++;
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                await displayLocationSelector();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Write(ex.Message);
            }
        }

        private async Task displayLocationSelector()
        {
            LocationSelect locationSelector = new LocationSelect();
            locationSelector.ShowDialog();
            latitudeTBox.Text = Globals.latitute.ToString();
            longitudeTBox.Text = Globals.longitude.ToString();
            altitudeTBox.Text = Globals.altitude.ToString();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (keepEvolvablePkCheckBox.Checked)
            {
                Globals.keepPokemonsThatCanEvolve = true;
            }
            else
            {
                Globals.keepPokemonsThatCanEvolve = false;
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (keepEvolvablePkCheckBox.Checked)
            {
                Globals.pokevision = true;
            }
            else
            {
                Globals.pokevision = false;
            }
        }

        private void TextBoxes_Items_TextChanged(object sender, EventArgs e)
        {
            int item_summe = 0;

            if (maxPokeBallsTBox.Text != "" && maxGreatBallsTBox.Text != "" && maxUltraBallsTBox.Text != "" && maxReviveTBox.Text != "" && maxPotTBox.Text != "" && maxSuperPotTBox.Text != "" && maxHyperPotTBox.Text != "" && maxRazzBerrysTBox.Text != "" && maxMasterBallsTBox.Text != "" && maxTopPotTBox.Text != "" && maxTopReviveTBox.Text != "")
            {
                item_summe = Convert.ToInt16(maxPokeBallsTBox.Text) +
                            Convert.ToInt16(maxGreatBallsTBox.Text) +
                            Convert.ToInt16(maxUltraBallsTBox.Text) +
                            Convert.ToInt16(maxReviveTBox.Text) +
                            Convert.ToInt16(maxPotTBox.Text) +
                            Convert.ToInt16(maxSuperPotTBox.Text) +
                            Convert.ToInt16(maxHyperPotTBox.Text) +
                            Convert.ToInt16(maxRazzBerrysTBox.Text) +
                            Convert.ToInt16(maxMasterBallsTBox.Text) +
                            Convert.ToInt16(maxTopReviveTBox.Text) +
                            Convert.ToInt16(maxTopPotTBox.Text);
            }
            totalCountInventoryTBox.Text = Convert.ToString(item_summe);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RUNUBQEANCAGQ");
        }

        private void system_banner_Click(object sender, EventArgs e)
        {
            Process.Start("https://high-minded.net/threads/pokemon-go-c-bot-safer-better.50731/");
        }
        
    }
}
