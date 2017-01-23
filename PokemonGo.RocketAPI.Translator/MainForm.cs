/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 23/01/2017
 * Time: 20:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Globalization;


namespace PokemonGo.RocketAPI.Translator
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
            textBox1.Text = CultureInfo.CurrentCulture.EnglishName.Split(' ')[0].ToLower();
        }
        void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("not implemented yet.");
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                var strJSON = System.IO.File.ReadAllText(openFileDialog1.FileName);
                var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(strJSON);
                dataGridView1.Rows.Clear();
                var position = 1;
                foreach (var item in dict)
                {
                    dataGridView1.Rows.Add(position,item.Key, item.Value);
                    position ++;
                }                
            }
        }
        void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = textBox1.Text +".json";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveToFile(saveFileDialog1.FileName);
            }
        }
        void saveToFile(string filename)
        {
            var dict = new Dictionary<string, string>();
            foreach ( DataGridViewRow  item in dataGridView1.Rows)
            {
                var key = item.Cells["ColumnKey"].Value;
                if (key == null)
                    key = "";
                var value = item.Cells["ColumnTranslation"].Value;
                if (value == null)
                    value = "";
                if (!dict.ContainsKey(key.ToString()))
                    dict.Add(key.ToString(),value.ToString());
            }
            var strJSON = Newtonsoft.Json.JsonConvert.SerializeObject(dict,Formatting.Indented);
            System.IO.File.WriteAllText(filename,strJSON);
        }
        void copyDefaultValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach ( DataGridViewRow  item in dataGridView1.Rows)
            {
                var value = item.Cells["ColumnValue"].Value;
                if (value == null)
                    value = "";
                item.Cells["ColumnTranslation"].Value = value;
            }
          
        }
        void autoTranslateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach ( DataGridViewRow  item in dataGridView1.Rows)
            {
                var value = item.Cells["ColumnValue"].Value;
                if (value == null)
                    value = "";
                if ((item.Cells["ColumnTranslation"].Value == null) || item.Cells["ColumnTranslation"].Value=="")
                    if (value.ToString() != "")
                    {
                        var translated = TranslateReferenceCom.Translate(value.ToString(),"english",textBox1.Text);
                        item.Cells["ColumnTranslation"].Value = translated;
                    }
            }
        }
        
        
    }
}
