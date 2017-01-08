﻿/*
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
        ChangesPanel changesPanel = null;
        EggsPanel eggsPanel = null;
        ItemsPanel itemsPanel = null;
        PlayerPanel playerPanel = null;
        PokemonsPanel pokemonsPanel = null;
        SniperPanel sniperPanel = null;
        
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
            btnPicPokes.Parent= locationPanel1.map;
            btnPicEggs.Parent= locationPanel1.map;
            btnPicItems.Parent= locationPanel1.map;
            btnPicConfig.Parent = locationPanel1.map;
            btnPicSnipe.Parent = locationPanel1.map;
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
            if (eggsPanel == null)
                eggsPanel = new EggsPanel();
            panel  = eggsPanel;
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
            if (itemsPanel == null)
                itemsPanel = new ItemsPanel();
            panel  = itemsPanel;
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
            if (changesPanel == null)
                changesPanel = new ChangesPanel();
            panel  = changesPanel;
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
            if (pokemonsPanel == null)
                pokemonsPanel = new PokemonsPanel();
            panel  = pokemonsPanel;
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
            if (sniperPanel == null)
                sniperPanel = new SniperPanel();
            panel  = sniperPanel;
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
            if (playerPanel == null)
                playerPanel = new PlayerPanel();
            panel  = playerPanel;
            panel.Anchor = allAnchors;
            panel.Location = new Point (1,1);
            panel.Size = new Size(this.Size.Width -10, 280);
            panel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Controls.Add(panel);
            panel.Visible = true;
            ((PlayerPanel)panel).Execute();
            btnPicClose.Visible = true;
            locationPanel1.Visible=false;
        }
        
    }
}
