namespace PokemonGo.RocketAPI.Console
{
    partial class Pokemons
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pokemons));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evolveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iVsToNicknameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeFavouritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.reloadtimer = new System.Windows.Forms.Timer(this.components);
            this.freezedenshit = new System.Windows.Forms.Timer(this.components);
            this.Options = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.pokemonsPanel1 = new PokemonGo.RocketAPI.Console.PokemonsPanel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.changesPanel1 = new PokemonGo.RocketAPI.Console.ChangesPanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.itemsPanel1 = new PokemonGo.RocketAPI.Console.ItemsPanel();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.RepeatRoute = new System.Windows.Forms.CheckBox();
            this.CreateRoute = new System.Windows.Forms.Button();
            this.locationPanel1 = new PokemonGo.RocketAPI.Console.LocationPanel();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.playerPanel1 = new PokemonGo.RocketAPI.Console.PlayerPanel();
            this.tabPageEggs = new System.Windows.Forms.TabPage();
            this.eggsPanel1 = new PokemonGo.RocketAPI.Console.EggsPanel();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.sniperPanel1 = new PokemonGo.RocketAPI.Console.SniperPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.Options.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPageEggs.SuspendLayout();
            this.tabPage6.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferToolStripMenuItem,
            this.powerUpToolStripMenuItem,
            this.evolveToolStripMenuItem,
            this.iVsToNicknameToolStripMenuItem,
            this.changeFavouritesToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 114);
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            this.transferToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.transferToolStripMenuItem.Text = "Transfer";
            // 
            // powerUpToolStripMenuItem
            // 
            this.powerUpToolStripMenuItem.Name = "powerUpToolStripMenuItem";
            this.powerUpToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.powerUpToolStripMenuItem.Text = "PowerUp";
            // 
            // evolveToolStripMenuItem
            // 
            this.evolveToolStripMenuItem.Name = "evolveToolStripMenuItem";
            this.evolveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.evolveToolStripMenuItem.Text = "Evolve";
            this.evolveToolStripMenuItem.Visible = false;
            // 
            // iVsToNicknameToolStripMenuItem
            // 
            this.iVsToNicknameToolStripMenuItem.Name = "iVsToNicknameToolStripMenuItem";
            this.iVsToNicknameToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.iVsToNicknameToolStripMenuItem.Text = "IVs to Nickname";
            // 
            // changeFavouritesToolStripMenuItem
            // 
            this.changeFavouritesToolStripMenuItem.Name = "changeFavouritesToolStripMenuItem";
            this.changeFavouritesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.changeFavouritesToolStripMenuItem.Text = "Change Favourites";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(273, 432);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 9;
            // 
            // reloadtimer
            // 
            this.reloadtimer.Interval = 1000;
            // 
            // freezedenshit
            // 
            this.freezedenshit.Interval = 5000;
            // 
            // Options
            // 
            this.Options.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Options.Controls.Add(this.tabPage2);
            this.Options.Controls.Add(this.tabPage1);
            this.Options.Controls.Add(this.tabPage3);
            this.Options.Controls.Add(this.tabPage4);
            this.Options.Controls.Add(this.tabPage5);
            this.Options.Controls.Add(this.tabPageEggs);
            this.Options.Controls.Add(this.tabPage6);
            this.Options.Location = new System.Drawing.Point(4, 4);
            this.Options.Name = "Options";
            this.Options.SelectedIndex = 0;
            this.Options.Size = new System.Drawing.Size(757, 510);
            this.Options.TabIndex = 46;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.pokemonsPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(749, 484);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Pokemon List";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // pokemonsPanel1
            // 
            this.pokemonsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pokemonsPanel1.Location = new System.Drawing.Point(4, 4);
            this.pokemonsPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.pokemonsPanel1.Name = "pokemonsPanel1";
            this.pokemonsPanel1.Size = new System.Drawing.Size(741, 473);
            this.pokemonsPanel1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.changesPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(732, 448);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Change Options";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // changesPanel1
            // 
            this.changesPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changesPanel1.Location = new System.Drawing.Point(0, 0);
            this.changesPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.changesPanel1.Name = "changesPanel1";
            this.changesPanel1.Size = new System.Drawing.Size(737, 450);
            this.changesPanel1.TabIndex = 0;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.itemsPanel1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage3.Size = new System.Drawing.Size(732, 448);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Items";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // itemsPanel1
            // 
            this.itemsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsPanel1.Location = new System.Drawing.Point(5, 5);
            this.itemsPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.itemsPanel1.Name = "itemsPanel1";
            this.itemsPanel1.Size = new System.Drawing.Size(721, 451);
            this.itemsPanel1.TabIndex = 0;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.RepeatRoute);
            this.tabPage4.Controls.Add(this.CreateRoute);
            this.tabPage4.Controls.Add(this.locationPanel1);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(732, 448);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Location";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // RepeatRoute
            // 
            this.RepeatRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RepeatRoute.AutoSize = true;
            this.RepeatRoute.Enabled = false;
            this.RepeatRoute.Location = new System.Drawing.Point(634, 400);
            this.RepeatRoute.Name = "RepeatRoute";
            this.RepeatRoute.Size = new System.Drawing.Size(93, 17);
            this.RepeatRoute.TabIndex = 46;
            this.RepeatRoute.Text = "Repeat Route";
            this.RepeatRoute.UseVisualStyleBackColor = true;
            // 
            // CreateRoute
            // 
            this.CreateRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateRoute.Location = new System.Drawing.Point(633, 422);
            this.CreateRoute.Margin = new System.Windows.Forms.Padding(2);
            this.CreateRoute.Name = "CreateRoute";
            this.CreateRoute.Size = new System.Drawing.Size(89, 23);
            this.CreateRoute.TabIndex = 47;
            this.CreateRoute.Text = "Define Route";
            this.CreateRoute.UseVisualStyleBackColor = true;
            this.CreateRoute.Click += new System.EventHandler(this.CreateRoute_Click);
            // 
            // locationPanel1
            // 
            this.locationPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.locationPanel1.Location = new System.Drawing.Point(3, 3);
            this.locationPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.locationPanel1.Name = "locationPanel1";
            this.locationPanel1.Size = new System.Drawing.Size(731, 447);
            this.locationPanel1.TabIndex = 0;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.playerPanel1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage5.Size = new System.Drawing.Size(732, 448);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Player Information";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // playerPanel1
            // 
            this.playerPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playerPanel1.BuddyInfoEnabled = false;
            this.playerPanel1.Location = new System.Drawing.Point(0, 0);
            this.playerPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.playerPanel1.Name = "playerPanel1";
            this.playerPanel1.Size = new System.Drawing.Size(734, 442);
            this.playerPanel1.TabIndex = 0;
            // 
            // tabPageEggs
            // 
            this.tabPageEggs.Controls.Add(this.eggsPanel1);
            this.tabPageEggs.Location = new System.Drawing.Point(4, 22);
            this.tabPageEggs.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageEggs.Name = "tabPageEggs";
            this.tabPageEggs.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageEggs.Size = new System.Drawing.Size(732, 448);
            this.tabPageEggs.TabIndex = 5;
            this.tabPageEggs.Text = "Eggs";
            this.tabPageEggs.UseVisualStyleBackColor = true;
            // 
            // eggsPanel1
            // 
            this.eggsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eggsPanel1.Location = new System.Drawing.Point(5, 5);
            this.eggsPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.eggsPanel1.Name = "eggsPanel1";
            this.eggsPanel1.Size = new System.Drawing.Size(723, 439);
            this.eggsPanel1.TabIndex = 0;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.sniperPanel1);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage6.Size = new System.Drawing.Size(732, 448);
            this.tabPage6.TabIndex = 6;
            this.tabPage6.Text = "Sniper Tools";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // sniperPanel1
            // 
            this.sniperPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sniperPanel1.Location = new System.Drawing.Point(4, 4);
            this.sniperPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.sniperPanel1.Name = "sniperPanel1";
            this.sniperPanel1.Size = new System.Drawing.Size(722, 437);
            this.sniperPanel1.TabIndex = 0;
            // 
            // Pokemons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(765, 517);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Options);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(734, 490);
            this.Name = "Pokemons";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pokemon List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Pokemons_Close);
            this.Load += new System.EventHandler(this.Pokemons_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.Options.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage5.ResumeLayout(false);
            this.tabPageEggs.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem transferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem powerUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evolveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iVsToNicknameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeFavouritesToolStripMenuItem;        
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer reloadtimer;
        private System.Windows.Forms.Timer freezedenshit;
        private System.Windows.Forms.TabControl Options;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
                
        private System.Windows.Forms.TextBox NumericUpDown;
        private PokemonGo.RocketAPI.Console.ItemsPanel itemsPanel1;
        private System.Windows.Forms.TabPage tabPage4;
        private PokemonGo.RocketAPI.Console.LocationPanel locationPanel1;
        private System.Windows.Forms.TabPage tabPage5;
        private PokemonGo.RocketAPI.Console.PlayerPanel playerPanel1;
        private System.Windows.Forms.TabPage tabPageEggs;
        private PokemonGo.RocketAPI.Console.EggsPanel eggsPanel1;
        private PokemonGo.RocketAPI.Console.ChangesPanel changesPanel1;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.CheckBox RepeatRoute;
        private System.Windows.Forms.Button CreateRoute;
        private PokemonGo.RocketAPI.Console.SniperPanel sniperPanel1;
        private PokemonGo.RocketAPI.Console.PokemonsPanel pokemonsPanel1;
    }
}