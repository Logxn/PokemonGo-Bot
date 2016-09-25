/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 23/09/2016
 * Time: 23:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of GameAspectSimulator.
    /// </summary>
    public partial class GameAspectSimulator : Form
    {
        UserControl panel  = null;
        private const AnchorStyles allAnchors = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
        public GameAspectSimulator()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            locationPanel1.Init(true, 0, 0, 0);
        }
        void btnPicClose_Click(object sender, EventArgs e)
        {
            btnPicClose.Visible =false;
            Controls.Remove(panel);
            panel = null;
            btnPicMenu.Visible =true;
            btnPicProfile.Visible =true;
            locationPanel1.Visible=true;
        }
        
        void btnPicMenu_Click(object sender, EventArgs e)
        {
            setVisiblePics(!btnPicPokes.Visible);
        }
        void setVisiblePics(bool value)
        {
            btnPicPokes.Visible= value;
            btnPicEggs.Visible= value;
            btnPicItems.Visible= value;
            btnPicConfig.Visible = value;
            btnPicSnipe.Visible = value;

        }
        void btnPicEggs_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;
            panel  = new EggsPanel();
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, this.Size.Height -55);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((EggsPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
        }
        void btnPicItems_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;
            panel  = new ItemsPanel();
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, this.Size.Height -55);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((ItemsPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
        }
        void btnPicConfig_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;
            panel  = new ChangesPanel();
            panel.Anchor = allAnchors;
            panel.Location= new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, this.Size.Height -55);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((ChangesPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
        }
        void btnPicPokes_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;
            panel  = new PokemonsPanel();
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, this.Size.Height -55);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((PokemonsPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
          
        }
        void btnSnipe_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;
            panel  = new SniperPanel();
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, this.Size.Height -55);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((SniperPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
          
        }
        private void This_Close(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
        void btnPicProfile_Click(object sender, EventArgs e)
        {
            setVisiblePics(false);
            btnPicMenu.Visible = false;
            btnPicProfile.Visible = false;            
            panel  = new PlayerPanel();
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, 200);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((PlayerPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
        }
        
    }
}
