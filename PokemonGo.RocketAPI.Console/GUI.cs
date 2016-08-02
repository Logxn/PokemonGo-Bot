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
        public static string languagestr;
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
                label2.Text = "E-Mail:";
            //    textBox1.Hide();
            //    label2.Hide();
            //    textBox2.Hide();
            //    label3.Hide();
            }
            else
            {
                if (languagestr == null)
                {
                    label2.Text = "Username:";
                }
                else if (languagestr == "de")
                {
                    label2.Text = "Benutzername:";
                }
            //    textBox1.Show();
            //    label2.Show();
            //    textBox2.Show();
            //    label3.Show();
            }
        }

        private void GUI_Load(object sender, EventArgs e)
        {
			Program.ParseConfig();
			
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
                    if (checkBox8.Checked)
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

			if(Globals.acc == Enums.AuthType.Google)
				comboBox1.SelectedIndex = 0;
			else
				comboBox1.SelectedIndex = 1;
			textBox1.Text = Globals.username;
			textBox2.Text = Globals.password;
			checkBox1.Checked = Globals.defLoc;
			checkBox2.Checked = Globals.transfer;
			checkBox3.Checked = Globals.evolve;
			textBox18.Text = Globals.telAPI;
			textBox19.Text = Globals.telName;
			textBox20.Text = Globals.telDelay.ToString();
			checkBox7.Checked = Globals.useluckyegg;
			checkBox8.Checked = Globals.gerNames;
			checkBox9.Checked = Globals.useincense;
			textBox24.Text = Globals.ivmaxpercent.ToString();
			checkBox10.Checked = Globals.pokeList;
			checkBox11.Checked = Globals.keepPokemonsThatCanEvolve;
			textBox3.Text = Globals.latitute.ToString();
			textBox4.Text = Globals.longitude.ToString();
			textBox5.Text = Globals.altitude.ToString();
			textBox6.Text = Globals.speed.ToString();
			textBox7.Text = Globals.radius.ToString();
			textBox8.Text = Globals.duplicate.ToString();
			textBox9.Text = Globals.maxCp.ToString();
			
            if (File.Exists(Program.items))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.items);
                i = 10;
                foreach (string line in lines)
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
                    TextBox temp = (TextBox)this.Controls.Find("textBox" + i, true).FirstOrDefault();
                    temp.Text = line;
                    i++;
                }
            }
            else
            {
                textBox10.Text = "20";
                textBox11.Text = "50";
                textBox12.Text = "100";
                textBox13.Text = "20";
                textBox14.Text = "0";
                textBox15.Text = "0";
                textBox16.Text = "50";
                textBox17.Text = "75";
                textBox22.Text = "200";
                textBox21.Text = "100";
                textBox23.Text = "20";
                textBox24.Text = "90";
            }

            if (File.Exists(Program.keep))
            {
                string[] lines = System.IO.File.ReadAllLines(@Program.keep);
                foreach (string line in lines)
                {
                    if (line != "")
                        if (checkBox8.Checked)
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
                        if (checkBox8.Checked)
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
                        if (checkBox8.Checked)
                            checkedListBox3.SetItemChecked(evolveIDS[gerEng[line]] - 1, true);
                        else
                            checkedListBox3.SetItemChecked(evolveIDS[line] - 1, true);
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

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.BackColor = Color.Red;
                return;
            }
            else
                Globals.username = textBox1.Text;
            if (textBox2.Text == "")
            {
                textBox2.BackColor = Color.Red;
                return;
            }
            else
                Globals.password = textBox2.Text;

            if (textBox3.Text == "")
            {
                textBox3.BackColor = Color.Red;
                return;
            }
            else
                Globals.latitute = double.Parse(textBox3.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (textBox4.Text == "")
            {
                textBox4.BackColor = Color.Red;
                return;
            }
            else
                Globals.longitude = double.Parse(textBox4.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (textBox5.Text == "")
            {
                textBox5.BackColor = Color.Red;
                return;
            }
            else
                Globals.altitude = double.Parse(textBox5.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (textBox6.Text == "")
            {
                textBox6.BackColor = Color.Red;
                return;
            }
            else
                Globals.speed = double.Parse(textBox6.Text.Replace(',', '.'), cords, System.Globalization.NumberFormatInfo.InvariantInfo);

            if (textBox7.Text == "")
            {
                textBox7.BackColor = Color.Red;
                return;
            }
            else
                Globals.radius = int.Parse(textBox7.Text);

            if (textBox8.Text == "")
            {
                textBox8.BackColor = Color.Red;
                return;
            }
            else
                Globals.duplicate = int.Parse(textBox8.Text);

            if (textBox9.Text == "")
            {
                textBox9.BackColor = Color.Red;
                return;
            }
            else
                Globals.maxCp = int.Parse(textBox9.Text);

            Globals.transfer = checkBox2.Checked;
            Globals.defLoc = checkBox1.Checked;
            Globals.evolve = checkBox3.Checked;

            if (textBox10.Text == "")
            {
                textBox10.BackColor = Color.Red;
                return;
            }
            else
                Globals.pokeball = int.Parse(textBox10.Text);

            if (textBox11.Text == "")
            {
                textBox11.BackColor = Color.Red;
                return;
            }
            else
                Globals.greatball = int.Parse(textBox11.Text);

            if (textBox12.Text == "")
            {
                textBox12.BackColor = Color.Red;
                return;
            }
            else
                Globals.ultraball = int.Parse(textBox12.Text);

            if (textBox13.Text == "")
            {
                textBox13.BackColor = Color.Red;
                return;
            }
            else
                Globals.revive = int.Parse(textBox13.Text);

            if (textBox14.Text == "")
            {
                textBox14.BackColor = Color.Red;
                return;
            }
            else
                Globals.potion = int.Parse(textBox14.Text);

            if (textBox15.Text == "")
            {
                textBox15.BackColor = Color.Red;
                return;
            }
            else
                Globals.superpotion = int.Parse(textBox15.Text);

            if (textBox16.Text == "")
            {
                textBox16.BackColor = Color.Red;
                return;
            }
            else
                Globals.hyperpotion = int.Parse(textBox16.Text);

            if (textBox17.Text == "")
            {
                textBox17.BackColor = Color.Red;
                return;
            }
            else
                Globals.berry = int.Parse(textBox17.Text);

            if (textBox18.Text != "")
                Globals.telAPI = textBox18.Text;

            if (textBox19.Text != "")
                Globals.telName = textBox19.Text;

            if (textBox20.Text == "")
            {
                textBox20.BackColor = Color.Red;
                return;
            }
            else
                Globals.telDelay = int.Parse(textBox20.Text);

            if (textBox21.Text == "")
            {
                textBox21.BackColor = Color.Red;
            }
            else
            {
                Globals.toppotion = int.Parse(textBox21.Text);
            }

            if (textBox22.Text == "")
            {
                textBox22.BackColor = Color.Red;
            }
            else
            {
                Globals.masterball = int.Parse(textBox22.Text);
            }

            if (textBox23.Text == "")
            {
                textBox23.BackColor = Color.Red;
            }
            else
            {
                Globals.toprevive = int.Parse(textBox23.Text);
            }

            if (textBox24.Text == "")
            {
                textBox24.BackColor = Color.Red;
            } else
            {
                Globals.ivmaxpercent = int.Parse(textBox24.Text);
            }

            Globals.gerNames = checkBox8.Checked;
            Globals.useincense = checkBox9.Checked;
            Globals.pokeList = checkBox10.Checked;
            Globals.keepPokemonsThatCanEvolve = checkBox11.Checked;
            //Globals.pokevision = checkBox12.Checked;

            foreach (string pokemon in checkedListBox1.CheckedItems)
            {
                if (checkBox8.Checked)
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox2.CheckedItems)
            {
                if (checkBox8.Checked)
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }
            foreach (string pokemon in checkedListBox3.CheckedItems)
            {
                if (checkBox8.Checked)
                    Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), gerEng[pokemon]));
                else
                    Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), pokemon));
            }

            string[] accFile = {
                    "accType="+Globals.acc.ToString(),
                    "username="+Globals.username,
                    "password="+Globals.password,
                    "coords="+Globals.latitute.ToString()+","+Globals.longitude.ToString()+","+Globals.altitude.ToString(),
                    "speed="+Globals.speed.ToString(),
                    "radius="+Globals.radius.ToString(),
                    "defLoc="+Globals.defLoc.ToString(),
                    "transfer="+Globals.transfer.ToString(),
                    "duplicate="+Globals.duplicate.ToString(),
                    "evolve="+Globals.evolve.ToString(),
                    "maxCp="+Globals.maxCp.ToString(),
                    "telAPI="+Globals.telAPI,
                    "telName="+Globals.telName,
                    "telDelay="+Globals.telDelay.ToString(),
                    "navigation_option="+Globals.navigation_option.ToString(),
                    "useluckyegg="+Globals.useluckyegg.ToString(),
                    "gerNames="+Globals.gerNames.ToString(),
                    "useincense="+Globals.useincense.ToString(),
                    "ivmaxpercent="+Globals.ivmaxpercent.ToString(),
                    "pokeList="+Globals.pokeList.ToString(),
                    "keepPokemonsThatCanEvolve="+Globals.keepPokemonsThatCanEvolve.ToString(),
                    "pokevision="+Globals.pokevision.ToString()
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
                if (checkBox8.Checked)
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
                if (checkBox8.Checked)
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
                if (checkBox8.Checked)
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

        private void checkBox7_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
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
                    if (checkBox8.Checked)
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
            LocationSelect locationSelector = new LocationSelect(false);
            locationSelector.ShowDialog();
            textBox3.Text = Globals.latitute.ToString();
            textBox4.Text = Globals.longitude.ToString();
            textBox5.Text = Globals.altitude.ToString();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
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
            if (checkBox11.Checked)
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

            if (textBox10.Text != "" && textBox11.Text != "" && textBox12.Text != "" && textBox13.Text != "" && textBox14.Text != "" && textBox15.Text != "" && textBox16.Text != "" && textBox17.Text != "" && textBox22.Text != "" && textBox21.Text != "" && textBox23.Text != "")
            {
                item_summe = Convert.ToInt16(textBox10.Text) +
                            Convert.ToInt16(textBox11.Text) +
                            Convert.ToInt16(textBox12.Text) +
                            Convert.ToInt16(textBox13.Text) +
                            Convert.ToInt16(textBox14.Text) +
                            Convert.ToInt16(textBox15.Text) +
                            Convert.ToInt16(textBox16.Text) +
                            Convert.ToInt16(textBox17.Text) +
                            Convert.ToInt16(textBox22.Text) +
                            Convert.ToInt16(textBox23.Text) +
                            Convert.ToInt16(textBox21.Text);
            }
            textBox25.Text = Convert.ToString(item_summe);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RUNUBQEANCAGQ");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://high-minded.net/threads/pokemon-go-c-bot-safer-better.50731/");
        }

        private void lang_en_btn_Click(object sender, EventArgs e)
        {
            lang_de_btn.Enabled = true;
            lang_en_btn.Enabled = false;
            languagestr = null;

            // Main GUI
            label1.Text = "Account Type:";
            label2.Text = "Username:";
            label3.Text = "Password:";
            groupBox2.Text = "Location Settings";
            label7.Text = "Speed:";
            label9.Text = "Move Radius:";
            label10.Text = "meters";
            checkBox1.Text = "Start from default location";
            groupBox3.Text = "Bot Settings";
            checkBox2.Text = "Auto transfer double Pokemons";
            label11.Text = "Max. duplicate Pokemons";
            label12.Text = "Max. CP to transfer:";
            label28.Text = "Max. IV to transfer:";
            groupBox8.Text = "Telegram Settings";
            label30.Text = "This Bot is absolutely free and open source! Chargeback if you've paid for it!";
            label32.Text = "Whenever you encounter something related to 'Pokecrot', tell them the Bot is stolen!";
            label13.Text = "Max. Pokeballs:";
            label14.Text = "Max. GreatBalls:";
            label15.Text = "Max. UltraBalls:";
            label26.Text = "Max. MasterBalls:";
            label16.Text = "Max. Revives:";
            label27.Text = "Max. TopRevives:";
            label17.Text = "Max. Potions:";
            label18.Text = "Max. SuperPotions:";
            label19.Text = "Max. HyperPotions:";
            label25.Text = "Max. TopPotions:";
            label20.Text = "Max. RazzBerrys:";
            label31.Text = "Total Count:";
            groupBox5.Text = "Pokemons - Not to transfer";
            checkBox4.Text = "Select all";
            groupBox6.Text = "Pokemons - Not to catch";
            checkBox5.Text = "Select all";
            groupBox7.Text = "Pokemons - To envolve";
            checkBox6.Text = "Select all";
            button1.Text = "Save Configuration / Start Bot";
            groupBox10.Text = "Other Settings";
            checkBox7.Text = "Use LuckyEgg at Evolve";
            checkBox8.Text = "German Pokemon names";
            checkBox9.Text = "Use Incense every 30min";
            checkBox3.Text = "Evolve Pokemons if enough candy";
            checkBox10.Text = "Enable Pokemon List GUI";
            checkBox11.Text = "Keep Pokemons which can be evolved";
        }

        private void lang_de_btn_Click(object sender, EventArgs e)
        {
            lang_en_btn.Enabled = true;
            lang_de_btn.Enabled = false;
            languagestr = "de";
            
            // Main GUI
            label1.Text = "Account Typ:";
            label2.Text = "Benutzername:";
            label3.Text = "Passwort:";
            groupBox2.Text = "Standort Einstellungen";
            label7.Text = "Speed:";
            label9.Text = "Radius:";
            label10.Text = "Meter";
            checkBox1.Text = "Starte von Standardposition";
            groupBox3.Text = "Bot Einstellungen";
            checkBox2.Text = "Doppelte Pokemon automatisch wegschicken";
            label11.Text = "Max. doppelte Pokemon";
            label12.Text = "Max. CP zu senden:";
            label28.Text = "Max. IV zu senden:";
            groupBox8.Text = "Telegram Einstellungen";
            label30.Text = "Dieser Bot ist kostenlos und Open-Source! Fordere ggf. Geld zurück!";
            label32.Text = "Wenn du den Namen 'Pokecrot' siehst, teile allen mit, dass dieser gestohlen ist.";
            label13.Text = "Max. Pokebälle:";
            label14.Text = "Max. Superbälle:";
            label15.Text = "Max. Ultrabälle:";
            label26.Text = "Max. Meisterbälles:";
            label16.Text = "Max. Beleber:";
            label27.Text = "Max. Top-Beleber:";
            label17.Text = "Max. Tränke:";
            label18.Text = "Max. Supertränke:";
            label19.Text = "Max. Hypertränke:";
            label25.Text = "Max. Top-Tränke:";
            label20.Text = "Max. Beeren:";
            label31.Text = "Gesamtzahl:";
            groupBox5.Text = "Pokemon - Nicht zu Versenden";
            checkBox4.Text = "Alle auswählen";
            groupBox6.Text = "Pokemons - Nicht zu Fangen";
            checkBox5.Text = "Alle auswählen";
            groupBox7.Text = "Pokemons - Zu entwickeln";
            checkBox6.Text = "Alle auswählen";
            button1.Text = "Konfiguration Speichern / Bot starten";
            groupBox10.Text = "Sonstige Einstellungen";
            checkBox7.Text = "LuckyEgg bei Entwicklung";
            checkBox8.Text = "Deutsche Pokemon Namen";
            checkBox9.Text = "Verwende Rauch alle 30min";
            checkBox3.Text = "Pokemon entwickeln wenn möglich";
            checkBox10.Text = "Pokemon List GUI anzeigen";
            checkBox11.Text = "Behalte entwickelbare Pokemon";
        }
    }
}
