using AllEnum;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public partial class GUI : Form
    {
        public static string path = Directory.GetCurrentDirectory();
        public static string account = path + "\\Config.txt";
        public static string items = path + "\\Items.txt";
        public static string keep = path + "\\noTransfer.txt";
        public static string ignore = path + "\\noCatch.txt";
        public static string evolve = path + "\\Evolve.txt";
        public NumberStyles cords = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;

        public GUI()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.acc = comboBox1.SelectedIndex == 0 ? Enums.AuthType.Google : Enums.AuthType.Ptc;
            if (comboBox1.SelectedIndex == 0)
            {
                textBox1.Hide();
                label2.Hide();
                textBox2.Hide();
                label3.Hide();
            }
            else
            {
                textBox1.Show();
                label2.Show();
                textBox2.Show();
                label3.Show();
            }
        }

        private void GUI_Load(object sender, EventArgs e)
        {
            comboBox1.DisplayMember = "Text";
            var types = new[] {
                new { Text = "Google"},
                new { Text = "Pokemon Trainer Club"},
            };
            comboBox1.DataSource = types;

            textBox1.Hide();
            label2.Hide();
            textBox2.Hide();
            label3.Hide();

            string[] pokemonID = new string[200];
            var pokeIDS = new Dictionary<string, int>();
            int i = 0;
            foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
            {
                if (pokemon.ToString() != "Missingno")
                {
                    pokeIDS[pokemon.ToString()] = i;
                    checkedListBox1.Items.Add(pokemon);
                    checkedListBox2.Items.Add(pokemon);
                    checkedListBox3.Items.Add(pokemon);
                    i++;
                }
            }

            if (File.Exists(account))
            {
                string[] lines = System.IO.File.ReadAllLines(@account);
                i = 1;
                int tb = 1;
                foreach (string line in lines)
                {
                    switch (i)
                    {
                        case 1:
                            if (line == "Google")
                                comboBox1.SelectedIndex = 0;
                            else
                                comboBox1.SelectedIndex = 1;
                            break;
                        case 9:
                            checkBox1.Checked = bool.Parse(line);
                            break;
                        case 10:
                            checkBox2.Checked = bool.Parse(line);
                            break;
                        case 12:
                            checkBox3.Checked = bool.Parse(line);
                            break;
                        default:
                            TextBox temp = (TextBox)this.Controls.Find("textBox" + tb, true).FirstOrDefault();
                            temp.Text = line;
                            tb++;
                            break;
                    }
                    i++;
                }
            }
            else {
                textBox3.Text = "40,764883";
                textBox4.Text = "-73,972967";
                textBox5.Text = "10";
                textBox6.Text = "50";
                textBox7.Text = "5000";
                textBox8.Text = "3";
                textBox9.Text = "999";
            }

            if (File.Exists(items)) {
                string[] lines = System.IO.File.ReadAllLines(@items);
                i = 10;
                foreach (string line in lines)
                {
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
            }

            if (File.Exists(keep))
            {
                string[] lines = System.IO.File.ReadAllLines(@keep);
                foreach (string line in lines)
                {
                    if(line != "")
                        checkedListBox1.SetItemChecked(pokeIDS[line], true);
                }
            }

            if (File.Exists(ignore))
            {
                string[] lines = System.IO.File.ReadAllLines(@ignore);
                foreach (string line in lines)
                {
                    if (line != "")
                        checkedListBox2.SetItemChecked(pokeIDS[line], true);
                }
            }

            if (File.Exists(evolve))
            {
                string[] lines = System.IO.File.ReadAllLines(@evolve);
                foreach (string line in lines)
                {
                    if (line != "")
                        checkedListBox3.SetItemChecked(pokeIDS[line], true);
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
            if (comboBox1.SelectedIndex == 1)
            {
                if(textBox1.Text == "")
                {
                    textBox1.BackColor = Color.Red;
                    return;
                }else
                    Globals.username = textBox1.Text;
                if (textBox2.Text == "")
                {
                    textBox2.BackColor = Color.Red;
                    return;
                }
                else
                    Globals.password = textBox2.Text;
            }

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
                Globals.hyperpoiton = int.Parse(textBox16.Text);

            if (textBox17.Text == "")
            {
                textBox17.BackColor = Color.Red;
                return;
            }
            else
                Globals.berry = int.Parse(textBox17.Text);

            foreach (PokemonId pokemon in checkedListBox1.CheckedItems)
                Globals.noTransfer.Add(pokemon);
            foreach (PokemonId pokemon in checkedListBox2.CheckedItems)
                Globals.noCatch.Add(pokemon);
            foreach (PokemonId pokemon in checkedListBox3.CheckedItems)
                Globals.doEvolve.Add(pokemon);

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
                    Globals.maxCp.ToString()
            };
            System.IO.File.WriteAllLines(@account, accFile);

            string[] itemsFile = {
                    Globals.pokeball.ToString(),
                    Globals.greatball.ToString(),
                    Globals.ultraball.ToString(),
                    Globals.revive.ToString(),
                    Globals.potion.ToString(),
                    Globals.superpotion.ToString(),
                    Globals.hyperpoiton.ToString(),
                    Globals.berry.ToString()
            };
            System.IO.File.WriteAllLines(@items, itemsFile);

            string[] noTransFile = new string[200];
            int i = 0;
            foreach(PokemonId pokemon in Globals.noTransfer)
            {
                noTransFile.SetValue(pokemon.ToString(), i);
                i++;
            }
            System.IO.File.WriteAllLines(@keep, noTransFile);

            string[] noCatchFile = new string[200];
            i = 0;
            foreach (PokemonId pokemon in Globals.noCatch)
            {
                noCatchFile.SetValue(pokemon.ToString(), i);
                i++;
            }
            System.IO.File.WriteAllLines(@ignore, noCatchFile);

            string[] EvolveFile = new string[200];
            i = 0;
            foreach (PokemonId pokemon in Globals.doEvolve)
            {
                EvolveFile.SetValue(pokemon.ToString(), i);
                i++;
            }
            System.IO.File.WriteAllLines(@evolve, EvolveFile);

            GUI.ActiveForm.Close();
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
    }
}
