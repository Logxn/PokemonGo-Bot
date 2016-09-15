using System;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public partial class LocationSelect : Form
    {
    	public bool close = true;
        public LocationSelect(bool asViewOnly, int team = 0, int level = 0, long exp = 0)
        {
            InitializeComponent();
            this.locationPanel1.Init(asViewOnly,team,level,exp);
            this.locationPanel1.button1.Click += new System.EventHandler(this.button1_Click);
        }
        private void LocationSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (locationPanel1.close)
            {
                var result = MessageBox.Show("You didn't set start location! Are you sure you want to exit this window?", "Location selector", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No || result == DialogResult.Abort)
                {
                    e.Cancel = true;
                    return;
                }
            }        	
        }
        private void button1_Click(object sender, EventArgs e)
        {
        	Close();
        }

    }

}