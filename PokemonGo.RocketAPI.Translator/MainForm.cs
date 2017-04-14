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


namespace PokeMaster.Translator
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        string BaseFileName = "";
        string TranslationFileName = "";
        
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
                BaseFileName = openFileDialog1.FileName;
                var strJSON = System.IO.File.ReadAllText(BaseFileName);
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
                var key = item.Cells["ColumnKey"].Value ?? "";
                var value = item.Cells["ColumnTranslation"].Value ?? "";
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
                var value = item.Cells["ColumnValue"].Value ?? "";
                item.Cells["ColumnTranslation"].Value = value;
            }
          
        }
        void autoTranslateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach ( DataGridViewRow  item in dataGridView1.Rows)
            {
                var value = item.Cells["ColumnValue"].Value ?? "";
                if (item.Cells["ColumnTranslation"].Value == null || item.Cells["ColumnTranslation"].Value.ToString()=="")
                    if (value.ToString() != "")
                    {
                        var translated = TranslateReferenceCom.Translate(value.ToString(),"english",textBox1.Text);
                        item.Cells["ColumnTranslation"].Value = translated;
                    }
            }
        }
        void OpenTranslationToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (BaseFileName == "")
            {
                MessageBox.Show("Base not openened yet");
                return;
            }
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                TranslationFileName = openFileDialog1.FileName;
                var strJSON = System.IO.File.ReadAllText(TranslationFileName);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(strJSON);
                foreach (var item in dict)
                {
                    var row = FindRow(item.Key);
                    if (row != null )
                    {
                        row.Cells["ColumnTranslation"].Value = item.Value;
                    }
                }                
            }
        }
        private DataGridViewRow FindRow(string searchValue)
        {
            foreach(DataGridViewRow row in dataGridView1.Rows)
                if (row.Cells["ColumnKey"].Value!=null)
                    if(row.Cells["ColumnKey"].Value.ToString().Equals(searchValue))
                        return row;               
            return null;
        }
        private void PasteClipboardValue()
        {
            //Show Error if no cell is selected
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("Please select a cell", "Paste", 
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        
            //Get the starting Cell
            DataGridViewCell startCell = GetStartCell(dataGridView1);
            //Get the clipboard value in a dictionary
            String[] lines = Clipboard.GetText().Split('\n');
        
            int iRowIndex = startCell.RowIndex;
            foreach (var line in lines)
            {
                int iColIndex = startCell.ColumnIndex;
                //Check if the index is within the limit
                if (iColIndex <= dataGridView1.Columns.Count - 1
                && iRowIndex <= dataGridView1.Rows.Count - 1)
                {
                    DataGridViewCell cell = dataGridView1[iColIndex, iRowIndex];
                    cell.Value = line;
                    iRowIndex++;
                }
            }
        }
        
        private DataGridViewCell GetStartCell(DataGridView dgView)
        {
            //get the smallest row,column index
            if (dgView.SelectedCells.Count == 0)
                return null;
        
            int rowIndex = dgView.Rows.Count - 1;
            int colIndex = dgView.Columns.Count - 1;
        
            foreach (DataGridViewCell dgvCell in dgView.SelectedCells)
            {
                if (dgvCell.RowIndex < rowIndex)
                    rowIndex = dgvCell.RowIndex;
                if (dgvCell.ColumnIndex < colIndex)
                    colIndex = dgvCell.ColumnIndex;
            }
        
            return dgView[colIndex, rowIndex];
        }
        void DataGridView1KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Shift && e.KeyCode == Keys.Insert) || (e.Control && e.KeyCode == Keys.V))
               PasteClipboardValue();     
            if ((e.KeyCode == Keys.Back)  ||(e.KeyCode == Keys.Delete) )
                 foreach (DataGridViewCell dgvCell in dataGridView1.SelectedCells)
                    if (dgvCell.ColumnIndex == ColumnTranslation.Index)
                        dgvCell.Value ="";
        }       
        
    }
}
