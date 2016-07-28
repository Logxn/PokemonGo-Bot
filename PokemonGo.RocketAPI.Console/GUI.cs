namespace PokemonGo.RocketAPI.Console
{
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

    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Logic.Utils;

    public partial class GUI : Form
    {
        public static NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        public static int[] evolveBlacklist =
        {
            3, 6, 9, 12, 15, 18, 20, 22, 24, 26, 28, 31, 34, 36, 38, 40, 42, 45, 47, 49, 51, 53, 55, 57, 59, 62, 65, 68, 71, 73, 76, 78, 80, 82, 83, 85, 87, 89, 91, 94, 95, 97, 99, 101, 103, 105, 106, 107, 108, 110, 112, 113, 114, 115, 117, 119, 121, 122, 123, 124, 125, 126, 127, 128, 130, 131, 132, 134, 135, 136, 137, 139, 141, 142, 143, 144, 145, 146, 149, 150, 151
        };

        public static Dictionary<string, string> gerEng = new Dictionary<string, string>();

        public GUI()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text == string.Empty)
            {
                this.textBox1.BackColor = Color.Red;
                return;
            }

            Globals.username = this.textBox1.Text;
            if (this.textBox2.Text == string.Empty)
            {
                this.textBox2.BackColor = Color.Red;
                return;
            }

            Globals.password = this.textBox2.Text;

            if (this.textBox3.Text == string.Empty)
            {
                this.textBox3.BackColor = Color.Red;
                return;
            }

            Globals.latitute = double.Parse(this.textBox3.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (this.textBox4.Text == string.Empty)
            {
                this.textBox4.BackColor = Color.Red;
                return;
            }

            Globals.longitude = double.Parse(this.textBox4.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (this.textBox5.Text == string.Empty)
            {
                this.textBox5.BackColor = Color.Red;
                return;
            }

            Globals.altitude = double.Parse(this.textBox5.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (this.textBox6.Text == string.Empty)
            {
                this.textBox6.BackColor = Color.Red;
                return;
            }

            Globals.speed = double.Parse(this.textBox6.Text.Replace(',', '.'), cords, NumberFormatInfo.InvariantInfo);

            if (this.textBox7.Text == string.Empty)
            {
                this.textBox7.BackColor = Color.Red;
                return;
            }

            Globals.radius = int.Parse(this.textBox7.Text);

            if (this.textBox8.Text == string.Empty)
            {
                this.textBox8.BackColor = Color.Red;
                return;
            }

            Globals.duplicate = int.Parse(this.textBox8.Text);

            if (this.textBox9.Text == string.Empty)
            {
                this.textBox9.BackColor = Color.Red;
                return;
            }

            Globals.maxCp = int.Parse(this.textBox9.Text);

            Globals.transfer = this.checkBox2.Checked;
            Globals.defLoc = this.checkBox1.Checked;
            Globals.evolve = this.checkBox3.Checked;

            if (this.textBox10.Text == string.Empty)
            {
                this.textBox10.BackColor = Color.Red;
                return;
            }

            Globals.pokeball = int.Parse(this.textBox10.Text);

            if (this.textBox11.Text == string.Empty)
            {
                this.textBox11.BackColor = Color.Red;
                return;
            }

            Globals.greatball = int.Parse(this.textBox11.Text);

            if (this.textBox12.Text == string.Empty)
            {
                this.textBox12.BackColor = Color.Red;
                return;
            }

            Globals.ultraball = int.Parse(this.textBox12.Text);

            if (this.textBox13.Text == string.Empty)
            {
                this.textBox13.BackColor = Color.Red;
                return;
            }

            Globals.revive = int.Parse(this.textBox13.Text);

            if (this.textBox14.Text == string.Empty)
            {
                this.textBox14.BackColor = Color.Red;
                return;
            }

            Globals.potion = int.Parse(this.textBox14.Text);

            if (this.textBox15.Text == string.Empty)
            {
                this.textBox15.BackColor = Color.Red;
                return;
            }

            Globals.superpotion = int.Parse(this.textBox15.Text);

            if (this.textBox16.Text == string.Empty)
            {
                this.textBox16.BackColor = Color.Red;
                return;
            }

            Globals.hyperpotion = int.Parse(this.textBox16.Text);

            if (this.textBox17.Text == string.Empty)
            {
                this.textBox17.BackColor = Color.Red;
                return;
            }

            Globals.berry = int.Parse(this.textBox17.Text);

            if (this.textBox18.Text != string.Empty)
                Globals.telAPI = this.textBox18.Text;

            if (this.textBox19.Text != string.Empty)
                Globals.telName = this.textBox19.Text;

            if (this.textBox20.Text == string.Empty)
            {
                this.textBox20.BackColor = Color.Red;
                return;
            }

            Globals.telDelay = int.Parse(this.textBox20.Text);

            if (this.textBox21.Text == string.Empty)
            {
                this.textBox21.BackColor = Color.Red;
            }
            else
            {
                Globals.toppotion = int.Parse(this.textBox21.Text);
            }

            if (this.textBox22.Text == string.Empty)
            {
                this.textBox22.BackColor = Color.Red;
            }
            else
            {
                Globals.masterball = int.Parse(this.textBox22.Text);
            }

            if (this.textBox23.Text == string.Empty)
            {
                this.textBox23.BackColor = Color.Red;
            }
            else
            {
                Globals.toprevive = int.Parse(this.textBox23.Text);
            }

            Globals.gerNames = this.checkBox8.Checked;
            Globals.useincense = this.checkBox9.Checked;

            foreach (string pokemon in this.checkedListBox1.CheckedItems)
            {
                if (this.checkBox8.Checked)
                    Globals.noTransfer.Add((PokemonId) Enum.Parse(typeof (PokemonId), gerEng[pokemon]));
                else
                    Globals.noTransfer.Add((PokemonId) Enum.Parse(typeof (PokemonId), pokemon));
            }

            foreach (string pokemon in this.checkedListBox2.CheckedItems)
            {
                if (this.checkBox8.Checked)
                    Globals.noCatch.Add((PokemonId) Enum.Parse(typeof (PokemonId), gerEng[pokemon]));
                else
                    Globals.noCatch.Add((PokemonId) Enum.Parse(typeof (PokemonId), pokemon));
            }

            foreach (string pokemon in this.checkedListBox3.CheckedItems)
            {
                if (this.checkBox8.Checked)
                    Globals.doEvolve.Add((PokemonId) Enum.Parse(typeof (PokemonId), gerEng[pokemon]));
                else
                    Globals.doEvolve.Add((PokemonId) Enum.Parse(typeof (PokemonId), pokemon));
            }

            string[] accFile =
            {
                Globals.acc.ToString(), Globals.username, Globals.password, Globals.latitute.ToString(), Globals.longitude.ToString(), Globals.altitude.ToString(), Globals.speed.ToString(), Globals.radius.ToString(), Globals.defLoc.ToString(), Globals.transfer.ToString(), Globals.duplicate.ToString(), Globals.evolve.ToString(), Globals.maxCp.ToString(), Globals.telAPI, Globals.telName, Globals.telDelay.ToString(), Globals.navigation_option.ToString(), Globals.useluckyegg.ToString(), Globals.gerNames.ToString(), Globals.useincense.ToString()
            };
            File.WriteAllLines(Program.account, accFile);

            string[] itemsFile =
            {
                Globals.pokeball.ToString(), Globals.greatball.ToString(), Globals.ultraball.ToString(), Globals.revive.ToString(), Globals.potion.ToString(), Globals.superpotion.ToString(), Globals.hyperpotion.ToString(), Globals.berry.ToString(), Globals.masterball.ToString(), Globals.toppotion.ToString(), Globals.toprevive.ToString()
            };
            File.WriteAllLines(Program.items, itemsFile);

            var temp = new string[200];
            var i = 0;
            foreach (var pokemon in Globals.noTransfer)
            {
                if (this.checkBox8.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }

            var noTransFile = temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(Program.keep, noTransFile);

            i = 0;
            Array.Clear(temp, 0, temp.Length);
            foreach (var pokemon in Globals.noCatch)
            {
                if (this.checkBox8.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }

            var noCatchFile = temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(Program.ignore, noCatchFile);

            Array.Clear(temp, 0, temp.Length);
            i = 0;
            foreach (var pokemon in Globals.doEvolve)
            {
                if (this.checkBox8.Checked)
                    temp.SetValue(StringUtils.getPokemonNameGer(pokemon), i);
                else
                    temp.SetValue(pokemon.ToString(), i);
                i++;
            }

            var EvolveFile = temp.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            File.WriteAllLines(Program.evolve, EvolveFile);

            ActiveForm.Dispose();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                await this.displayLocationSelector();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Write(ex.Message);
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox4.Checked)
            {
                var i = 0;
                while (i < this.checkedListBox1.Items.Count)
                {
                    this.checkedListBox1.SetItemChecked(i, true);
                    i++;
                }
            }
            else
            {
                var i = 0;
                while (i < this.checkedListBox1.Items.Count)
                {
                    this.checkedListBox1.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox5.Checked)
            {
                var i = 0;
                while (i < this.checkedListBox2.Items.Count)
                {
                    this.checkedListBox2.SetItemChecked(i, true);
                    i++;
                }
            }
            else
            {
                var i = 0;
                while (i < this.checkedListBox2.Items.Count)
                {
                    this.checkedListBox2.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox6.Checked)
            {
                var i = 0;
                while (i < this.checkedListBox3.Items.Count)
                {
                    this.checkedListBox3.SetItemChecked(i, true);
                    i++;
                }
            }
            else
            {
                var i = 0;
                while (i < this.checkedListBox3.Items.Count)
                {
                    this.checkedListBox3.SetItemChecked(i, false);
                    i++;
                }
            }
        }

        private void checkBox7_CheckedChanged_1(object sender, EventArgs e)
        {
            if (this.checkBox7.Checked)
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
            while (this.checkedListBox1.Items.Count > 0)
            {
                this.checkedListBox1.Items.RemoveAt(0);
                this.checkedListBox2.Items.RemoveAt(0);
                if (this.checkedListBox3.Items.Count > 0)
                    this.checkedListBox3.Items.RemoveAt(0);
            }

            var i = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof (PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    if (this.checkBox8.Checked)
                    {
                        this.checkedListBox1.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        this.checkedListBox2.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!evolveBlacklist.Contains(i))
                        {
                            this.checkedListBox3.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        }
                    }
                    else
                    {
                        this.checkedListBox1.Items.Add(pokemon.ToString());
                        this.checkedListBox2.Items.Add(pokemon.ToString());
                        if (!evolveBlacklist.Contains(i))
                        {
                            this.checkedListBox3.Items.Add(pokemon.ToString());
                        }
                    }

                    i++;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.acc = this.comboBox1.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;

            // if (comboBox1.SelectedIndex == 0)
            // {
            // textBox1.Hide();
            // label2.Hide();
            // textBox2.Hide();
            // label3.Hide();
            // }
            // else
            // {
            // textBox1.Show();
            // label2.Show();
            // textBox2.Show();
            // label3.Show();
            // }
        }

        private async Task displayLocationSelector()
        {
            var locationSelector = new LocationSelect();
            locationSelector.ShowDialog();
            this.textBox3.Text = Globals.latitute.ToString();
            this.textBox4.Text = Globals.longitude.ToString();
            this.textBox5.Text = Globals.altitude.ToString();
        }

        private void GUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            // Version Infoooo
            this.groupBox9.Text = "Your Version: " + Assembly.GetExecutingAssembly().GetName().Version + " | Newest: " + Program.getNewestVersion();
            if (Program.getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
            {
                MessageBox.Show("There is an Update on Github. Bottom left is a Github Link Label.");
            }

            this.comboBox1.DisplayMember = "Text";
            var types = new[]
                        {
                            new
                            {
                                Text = "Google"
                            },
                            new
                            {
                                Text = "Pokemon Trainer Club"
                            }
                        };
            this.comboBox1.DataSource = types;

            // textBox1.Hide();
            // label2.Hide();
            // textBox2.Hide();
            // label3.Hide();
            var pokeIDS = new Dictionary<string, int>();
            var evolveIDS = new Dictionary<string, int>();
            var i = 1;
            var ev = 1;
            foreach (PokemonId pokemon in Enum.GetValues(typeof (PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = i;
                    gerEng[StringUtils.getPokemonNameGer(pokemon)] = pokemon.ToString();
                    if (this.checkBox8.Checked)
                    {
                        this.checkedListBox1.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        this.checkedListBox2.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                        if (!evolveBlacklist.Contains(i))
                        {
                            this.checkedListBox3.Items.Add(StringUtils.getPokemonNameGer(pokemon));
                            evolveIDS[pokemon.ToString()] = ev;
                            ev++;
                        }
                    }
                    else
                    {
                        this.checkedListBox1.Items.Add(pokemon.ToString());
                        this.checkedListBox2.Items.Add(pokemon.ToString());
                        if (!evolveBlacklist.Contains(i))
                        {
                            this.checkedListBox3.Items.Add(pokemon.ToString());
                            evolveIDS[pokemon.ToString()] = ev;
                            ev++;
                        }
                    }

                    i++;
                }
            }

            if (File.Exists(Program.account))
            {
                var lines = File.ReadAllLines(Program.account);
                i = 1;
                var tb = 1;
                foreach (var line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            if (line == "Google")
                                this.comboBox1.SelectedIndex = 0;
                            else
                                this.comboBox1.SelectedIndex = 1;
                            break;

                        case 9:
                            this.checkBox1.Checked = bool.Parse(line);
                            break;

                        case 10:
                            this.checkBox2.Checked = bool.Parse(line);
                            break;

                        case 12:
                            this.checkBox3.Checked = bool.Parse(line);
                            break;

                        case 14:
                            this.textBox18.Text = line;
                            break;

                        case 15:
                            this.textBox19.Text = line;
                            break;

                        case 16:
                            this.textBox20.Text = line;
                            break;

                        case 17:

                            // if (line == "1")
                            // {
                            // Globals.navigation_option = 1;
                            // checkBox8.Checked = true;
                            // checkBox7.Checked = false;
                            // } else
                            // {
                            // Globals.navigation_option = 2;
                            // checkBox7.Checked = true;
                            // checkBox8.Checked = false;
                            // }
                            break;

                        case 18:
                            this.checkBox7.Checked = bool.Parse(line);
                            break;

                        case 19:
                            this.checkBox8.Checked = bool.Parse(line);
                            break;

                        case 20:
                            this.checkBox9.Checked = bool.Parse(line);
                            break;

                        default:
                            var temp = (TextBox) this.Controls.Find("textBox" + tb, true).FirstOrDefault();
                            temp.Text = line;
                            tb++;
                            break;
                    }

                    i++;
                }
            }
            else
            {
                this.textBox3.Text = "40,764883";
                this.textBox4.Text = "-73,972967";
                this.textBox5.Text = "10";
                this.textBox6.Text = "50";
                this.textBox7.Text = "5000";
                this.textBox8.Text = "3";
                this.textBox9.Text = "999";
                this.textBox20.Text = "5000";
            }

            if (File.Exists(Program.items))
            {
                var lines = File.ReadAllLines(Program.items);
                i = 10;
                foreach (var line in lines)
                {
                    if (i == 18)
                    {
                        i = 22;
                    }
                    else if (i == 23)
                    {
                        i = 21;
                    }
                    else if (i == 22)
                    {
                        i = 23;
                    }

                    var temp = (TextBox) this.Controls.Find("textBox" + i, true).FirstOrDefault();
                    temp.Text = line;
                    i++;
                }
            }
            else
            {
                this.textBox10.Text = "20";
                this.textBox11.Text = "50";
                this.textBox12.Text = "100";
                this.textBox13.Text = "20";
                this.textBox14.Text = "0";
                this.textBox15.Text = "0";
                this.textBox16.Text = "50";
                this.textBox17.Text = "75";
                this.textBox22.Text = "200";
                this.textBox21.Text = "100";
                this.textBox23.Text = "20";
            }

            if (File.Exists(Program.keep))
            {
                var lines = File.ReadAllLines(Program.keep);
                foreach (var line in lines)
                {
                    if (line != string.Empty)
                        if (this.checkBox8.Checked)
                            this.checkedListBox1.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            this.checkedListBox1.SetItemChecked(pokeIDS[line] - 1, true);
                }
            }

            if (File.Exists(Program.ignore))
            {
                var lines = File.ReadAllLines(Program.ignore);
                foreach (var line in lines)
                {
                    if (line != string.Empty)
                        if (this.checkBox8.Checked)
                            this.checkedListBox2.SetItemChecked(pokeIDS[gerEng[line]] - 1, true);
                        else
                            this.checkedListBox2.SetItemChecked(pokeIDS[line] - 1, true);
                }
            }

            if (File.Exists(Program.evolve))
            {
                var lines = File.ReadAllLines(Program.evolve);
                foreach (var line in lines)
                {
                    if (line != string.Empty)
                        if (this.checkBox8.Checked)
                            this.checkedListBox3.SetItemChecked(evolveIDS[gerEng[line]] - 1, true);
                        else
                            this.checkedListBox3.SetItemChecked(evolveIDS[line] - 1, true);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Ar1i/PokemonGo-Bot");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://high-minded.net/threads/pokemon-go-c-bot-safer-better.50731/");
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

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}