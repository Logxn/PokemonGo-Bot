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
            Globals.acc = comboBox1.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;
            if (comboBox1.SelectedIndex == 0)
            {
                label2.Text = "E-mail:";
            //    tbUsername.Hide();
            //    label2.Hide();
            //    tbPassword.Hide();
            //    label3.Hide();
            }
            else
            {
                label2.Text = "Username:";
            //    tbUsername.Show();
            //    label2.Show();
            //    tbPassword.Show();
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

            comboBox1.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };
            comboBox1.DataSource = types;

            //tbUsername.Hide();
            //label2.Hide();
            //tbPassword.Hide();
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
                    if (cbGermanNames.Checked)
                    {
                        checkedListBox1.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        checkedListBox2.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            checkedListBox3.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                            evolveIDS[pokemon.ToString()] = ev;
                            ev++;
                        }
                    }
                    else
                    {
                        checkedListBox1.Items.Add(pokemon.ToString());
                        checkedListBox2.Items.Add(pokemon.ToString());
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            checkedListBox3.Items.Add(pokemon.ToString());
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
                i = 0;
                int tb = 1;
                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 0:
                            if (line == "Google")
                                comboBox1.SelectedIndex = 0;
                            else
                                comboBox1.SelectedIndex = 1;
                            break;
						case 1:
							tbUsername.Text = line;
							break;
						case 2:
							tbPassword.Text = line;
							break;
						case 3:
							tbLatitude.Text = line;
							break;
						case 4:
							tbLongitude.Text = line;
							break;
						case 5:
							tbAltitude.Text = line;
							break;
						case 6:
							tbWalkSpeed.Text = line;
							break;
						case 7:
							tbWalkDistance.Text = line;
							break;
                        case 8:
                            cbWlkFromDefLoc.Checked = bool.Parse(line);
                            break;
                        case 9:
                            cbTransferDupes.Checked = bool.Parse(line);
                            break;
						case 10:
							tbKeepDupes.Text = line;
							break;
                        case 11:
                            cbEvolveIfCandy.Checked = bool.Parse(line);
                            break;
						case 12:
							tbTransThreshCP.Text = line;
							break;
                        case 13:
                            tbTelAPI.Text = line;
                            break;
                        case 14:
                            tbTelName.Text = line;
                            break;
                        case 15:
                            tbTelDelay.Text = line;
                            break;
                        case 16:
                            //if (line == "1")
                            //{
                            //    Globals.navigation_option = 1;
                            //    cbGermanNames.Checked = true;
                            //    cbUseLuckyEgg.Checked = false;
                            //} else
                            //{
                            //    Globals.navigation_option = 2;
                            //    cbUseLuckyEgg.Checked = true;
                            //    cbGermanNames.Checked = false;
                            //}
							// UNUSED?!
                            break;
                        case 17:
                            cbUseLuckyEgg.Checked = bool.Parse(line);
                            break;
                        case 18:
                            cbGermanNames.Checked = bool.Parse(line);
                            break;
                        case 19:
                            cbUseIncense.Checked = bool.Parse(line);
                            break;
						case 20:
							tbIVMaxPcntTransfer.Text = line;
							break;
                        case 21:
                            cbUsePokeListGUI.Checked = bool.Parse(line);
                            break;
                        case 22:
                            cbKeepEvolvingPokemons.Checked = bool.Parse(line);
                            break;
                        case 23:
                            cbUsePokeVision.Checked = bool.Parse(line);
                            break;
                        default:
                            //TextBox temp = (TextBox)this.Controls.Find("textBox" + tb, true).FirstOrDefault();
                            //temp.Text = line;
                            //tb++;
                            break;
                    }
                    i++;
                }
            }
            else
            {
                tbLatitude.Text = "40,764883";
                tbLongitude.Text = "-73,972967";
                tbAltitude.Text = "10";
                tbWalkSpeed.Text = "50";
                tbWalkDistance.Text = "5000";
                tbKeepDupes.Text = "3";
                tbTransThreshCP.Text = "999";
                tbTelDelay.Text = "5000";
            }

            if (File.Exists(Program.items))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.items);
                i = 0;
                foreach (string line in lines)
                {
                    switch(i) {
						case 0:
							tbKeepNBall.Text = line;
							break;
						case 1:
							tbKeepSBall.Text = line;
							break;
						case 2:
							tbKeepHBall.Text = line;
							break;
						case 3:
							tbKeepMBall.Text = line;
							break;
						case 4:
							tbKeepNRevive.Text = line;
							break;
						case 5:
							tbKeepTRevive.Text = line;
							break;
						case 6:
							tbKeepNPotion.Text = line;
							break;
						case 7:
							tbKeepSPotion.Text = line;
							break;
						case 8:
							tbKeepHPotion.Text = line;
							break;
						case 9:
							tbKeepTPotion.Text = line;
							break;
						case 10:
							tbKeepBerry.Text = line;
							break;
					}
                    i++;
                }
            }
            else
            {
                tbKeepNBall.Text = "20";
                tbKeepSBall.Text = "50";
                tbKeepHBall.Text = "100";
                tbKeepNRevive.Text = "20";
                tbKeepNPotion.Text = "0";
                tbKeepSPotion.Text = "0";
                tbKeepHPotion.Text = "50";
                tbKeepBerry.Text = "75";
                tbKeepMBall.Text = "200";
                tbKeepTPotion.Text = "100";
                tbKeepTRevive.Text = "20";
                tbIVMaxPcntTransfer.Text = "90";
            }

            if (File.Exists(Program.keep))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.keep);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (cbGermanNames.Checked)
                            checkedListBox1.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            checkedListBox1.SetItemChecked(pokeIDS[line] - 1, true);
                }
            }

            if (File.Exists(Program.ignore))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.ignore);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (cbGermanNames.Checked)
                            checkedListBox2.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            checkedListBox2.SetItemChecked(pokeIDS[line] - 1, true);
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
                        if (cbGermanNames.Checked)
                            checkedListBox3.SetItemChecked(evolveIDS[gerEng[line]] - 1, true);
                        else
                            checkedListBox3.SetItemChecked(evolveIDS[line] - 1, true);
                }
            }

        }

        private void tbLatitude_KeyPress(object sender, KeyPressEventArgs e)
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

        private void tbAltitude_KeyPress(object sender, KeyPressEventArgs e)
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

        private void tbWalkDistance_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tbUsername.Text == "")
            {
                tbUsername.BackColor = Color.Red;
                return;
            }
            else
                Globals.username = tbUsername.Text;
            if (tbPassword.Text == "")
            {
                tbPassword.BackColor = Color.Red;
                return;
            }
            else
                Globals.password = tbPassword.Text;

            if (tbLatitude.Text == "")
            {
                tbLatitude.BackColor = Color.Red;
                return;
            }
            else
                Globals.latitute = double.Parse(tbLatitude.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (tbLongitude.Text == "")
            {
                tbLongitude.BackColor = Color.Red;
                return;
            }
            else
                Globals.longitude = double.Parse(tbLongitude.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (tbAltitude.Text == "")
            {
                tbAltitude.BackColor = Color.Red;
                return;
            }
            else
                Globals.altitude = double.Parse(tbAltitude.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (tbWalkSpeed.Text == "")
            {
                tbWalkSpeed.BackColor = Color.Red;
                return;
            }
            else
                Globals.speed = double.Parse(tbWalkSpeed.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (tbWalkDistance.Text == "")
            {
                tbWalkDistance.BackColor = Color.Red;
                return;
            }
            else
                Globals.radius = int.Parse(tbWalkDistance.Text);

            if (tbKeepDupes.Text == "")
            {
                tbKeepDupes.BackColor = Color.Red;
                return;
            }
            else
                Globals.duplicate = int.Parse(tbKeepDupes.Text);

            if (tbTransThreshCP.Text == "")
            {
                tbTransThreshCP.BackColor = Color.Red;
                return;
            }
            else
                Globals.maxCp = int.Parse(tbTransThreshCP.Text);

            Globals.transfer = cbTransferDupes.Checked;
            Globals.defLoc = cbWlkFromDefLoc.Checked;
            Globals.evolve = cbEvolveIfCandy.Checked;

            if (tbKeepNBall.Text == "")
            {
                tbKeepNBall.BackColor = Color.Red;
                return;
            }
            else
                Globals.pokeball = int.Parse(tbKeepNBall.Text);

            if (tbKeepSBall.Text == "")
            {
                tbKeepSBall.BackColor = Color.Red;
                return;
            }
            else
                Globals.greatball = int.Parse(tbKeepSBall.Text);

            if (tbKeepHBall.Text == "")
            {
                tbKeepHBall.BackColor = Color.Red;
                return;
            }
            else
                Globals.ultraball = int.Parse(tbKeepHBall.Text);

            if (tbKeepNRevive.Text == "")
            {
                tbKeepNRevive.BackColor = Color.Red;
                return;
            }
            else
                Globals.revive = int.Parse(tbKeepNRevive.Text);

            if (tbKeepNPotion.Text == "")
            {
                tbKeepNPotion.BackColor = Color.Red;
                return;
            }
            else
                Globals.potion = int.Parse(tbKeepNPotion.Text);

            if (tbKeepSPotion.Text == "")
            {
                tbKeepSPotion.BackColor = Color.Red;
                return;
            }
            else
                Globals.superpotion = int.Parse(tbKeepSPotion.Text);

            if (tbKeepHPotion.Text == "")
            {
                tbKeepHPotion.BackColor = Color.Red;
                return;
            }
            else
                Globals.hyperpotion = int.Parse(tbKeepHPotion.Text);

            if (tbKeepBerry.Text == "")
            {
                tbKeepBerry.BackColor = Color.Red;
                return;
            }
            else
                Globals.berry = int.Parse(tbKeepBerry.Text);

            if (tbTelAPI.Text != "")
                Globals.telAPI = tbTelAPI.Text;

            if (tbTelName.Text != "")
                Globals.telName = tbTelName.Text;

            if (tbTelDelay.Text == "")
            {
                tbTelDelay.BackColor = Color.Red;
                return;
            }
            else
                Globals.telDelay = int.Parse(tbTelDelay.Text);

            if (tbKeepTPotion.Text == "")
            {
                tbKeepTPotion.BackColor = Color.Red;
            }
            else
            {
                Globals.toppotion = int.Parse(tbKeepTPotion.Text);
            }

            if (tbKeepMBall.Text == "")
            {
                tbKeepMBall.BackColor = Color.Red;
            }
            else
            {
                Globals.masterball = int.Parse(tbKeepMBall.Text);
            }

            if (tbKeepTRevive.Text == "")
            {
                tbKeepTRevive.BackColor = Color.Red;
            }
            else
            {
                Globals.toprevive = int.Parse(tbKeepTRevive.Text);
            }

            if (tbIVMaxPcntTransfer.Text == "")
            {
                tbIVMaxPcntTransfer.BackColor = Color.Red;
            } else
            {
                Globals.ivmaxpercent = int.Parse(tbIVMaxPcntTransfer.Text);
            }

            Globals.gerNames = cbGermanNames.Checked;
            Globals.useincense = cbUseIncense.Checked;
            Globals.pokeList = cbUsePokeListGUI.Checked;
            Globals.keepPokemonsThatCanEvolve = cbKeepEvolvingPokemons.Checked;
            Globals.pokevision = cbUsePokeVision.Checked;

            foreach (string pokemon in checkedListBox1.CheckedItems)
            {
                if (cbGermanNames.Checked)
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox2.CheckedItems)
            {
                if (cbGermanNames.Checked)
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox3.CheckedItems)
            {
                if (cbGermanNames.Checked)
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
                    Globals.pokevision.ToString()
            };
            System.IO.File.WriteAllLines(@Program.account, accFile);

            string[] itemsFile = {
                    Globals.pokeball.ToString(),
                    Globals.greatball.ToString(),
                    Globals.ultraball.ToString(),
                    Globals.masterball.ToString(),
                    Globals.revive.ToString(),
                    Globals.toprevive.ToString(),
                    Globals.potion.ToString(),
                    Globals.superpotion.ToString(),
                    Globals.hyperpotion.ToString(),
                    Globals.toppotion.ToString(),
                    Globals.berry.ToString()
            };
            System.IO.File.WriteAllLines(@Program.items, itemsFile);

            string[] temp = new string[200];
            int i = 0;
            foreach (PokemonId pokemon in Globals.noTransfer)
            {
                if (cbGermanNames.Checked)
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
                if (cbGermanNames.Checked)
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
                if (cbGermanNames.Checked)
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
            if (checkBox4.Checked)
            {
                int i = 0;
                while (i < checkedListBox1.Items.Count)
                {
                    checkedListBox1.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < checkedListBox1.Items.Count)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                int i = 0;
                while (i < checkedListBox2.Items.Count)
                {
                    checkedListBox2.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < checkedListBox2.Items.Count)
                {
                    checkedListBox2.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                int i = 0;
                while (i < checkedListBox3.Items.Count)
                {
                    checkedListBox3.SetItemChecked(i, true);
                    i++;
                }

            }
            else
            {
                int i = 0;
                while (i < checkedListBox3.Items.Count)
                {
                    checkedListBox3.SetItemChecked(i, false);
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

        private void cbUseLuckyEgg_CheckedChanged_1(object sender, EventArgs e)
        {
            if (cbUseLuckyEgg.Checked)
            {
                Globals.useluckyegg = true;
            }
            else
            {
                Globals.useluckyegg = false;
            }
        }

        private void cbGermanNames_CheckedChanged(object sender, EventArgs e)
        {
            while (checkedListBox1.Items.Count > 0)
            {
                checkedListBox1.Items.RemoveAt(0);
                checkedListBox2.Items.RemoveAt(0);
                if (checkedListBox3.Items.Count > 0)
                    checkedListBox3.Items.RemoveAt(0);
            }
            int i = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    if (cbGermanNames.Checked)
                    {
                        checkedListBox1.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        checkedListBox2.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            checkedListBox3.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        }
                    }
                    else
                    {
                        checkedListBox1.Items.Add(pokemon.ToString());
                        checkedListBox2.Items.Add(pokemon.ToString());
                        if (!(evolveBlacklist.Contains(i)))
                        {
                            checkedListBox3.Items.Add(pokemon.ToString());
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
            tbLatitude.Text = Globals.latitute.ToString();
            tbLongitude.Text = Globals.longitude.ToString();
            tbAltitude.Text = Globals.altitude.ToString();
        }

        private void cbKeepEvolvingPokemons_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKeepEvolvingPokemons.Checked)
            {
                Globals.keepPokemonsThatCanEvolve = true;
            }
            else
            {
                Globals.keepPokemonsThatCanEvolve = false;
            }
        }

        private void cbUsePokeVision_CheckedChanged(object sender, EventArgs e)
        {
            if (cbKeepEvolvingPokemons.Checked)
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

            if (tbKeepNBall.Text != "" && tbKeepSBall.Text != "" && tbKeepHBall.Text != "" && tbKeepNRevive.Text != "" && tbKeepNPotion.Text != "" && tbKeepSPotion.Text != "" && tbKeepHPotion.Text != "" && tbKeepBerry.Text != "" && tbKeepMBall.Text != "" && tbKeepTPotion.Text != "" && tbKeepTRevive.Text != "")
            {
                item_summe = Convert.ToInt16(tbKeepNBall.Text) +
                            Convert.ToInt16(tbKeepSBall.Text) +
                            Convert.ToInt16(tbKeepHBall.Text) +
                            Convert.ToInt16(tbKeepNRevive.Text) +
                            Convert.ToInt16(tbKeepNPotion.Text) +
                            Convert.ToInt16(tbKeepSPotion.Text) +
                            Convert.ToInt16(tbKeepHPotion.Text) +
                            Convert.ToInt16(tbKeepBerry.Text) +
                            Convert.ToInt16(tbKeepMBall.Text) +
                            Convert.ToInt16(tbKeepTRevive.Text) +
                            Convert.ToInt16(tbKeepTPotion.Text);
            }
            tbItemCount.Text = Convert.ToString(item_summe);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RUNUBQEANCAGQ");
        }
    }
}
