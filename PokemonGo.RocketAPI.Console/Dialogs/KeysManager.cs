/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 02/03/2017
 * Time: 23:58
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PokeMaster.Dialogs
{
    /// <summary>
    /// Description of KeysManager.
    /// </summary>
    public partial class KeysManager : Form
    {
        private static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        private static string filename = Path.Combine(path, "keys.json");
        public KeysManager()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            if (File.Exists(filename)){
                var strJSON = File.ReadAllText(filename);
                var keys1 = JsonConvert.DeserializeObject<ArrayList>(strJSON);
                listBox1.Items.Clear();
                foreach ( var element in keys1) {
                    listBox1.Items.Add(element);
                }
            }
        }
        void buttonAcept_Click(object sender, EventArgs e)
        {
          string strJSON = JsonConvert.SerializeObject(listBox1.Items,Formatting.Indented);
          File.WriteAllText(filename,strJSON);
          Close();
        }
        void buttonDelete_Click(object sender, EventArgs e)
        {
            for (var i = listBox1.SelectedItems.Count -1;i>=0;i--)
                listBox1.Items.Remove(listBox1.SelectedItems[i]);
        }
        void buttonAdd_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox1.Text);
            textBox1.Text ="";
        }
    }
}
