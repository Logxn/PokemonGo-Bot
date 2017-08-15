namespace PokeMaster
{
    partial class TabbedSystem
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TabbedSystem));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evolveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iVsToNicknameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeFavouritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.reloadtimer = new System.Windows.Forms.Timer(this.components);
            this.freezedenshit = new System.Windows.Forms.Timer(this.components);
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tpLocation = new System.Windows.Forms.TabPage();
            this.RepeatRoute = new System.Windows.Forms.CheckBox();
            this.CreateRoute = new System.Windows.Forms.Button();
            this.locationPanel1 = new PokeMaster.LocationPanel();
            this.tpPokemons = new System.Windows.Forms.TabPage();
            this.pokemonsPanel1 = new PokeMaster.PokemonsPanel();
            this.tpItems = new System.Windows.Forms.TabPage();
            this.itemsPanel1 = new PokeMaster.ItemsPanel();
            this.tpEggs = new System.Windows.Forms.TabPage();
            this.eggsPanel1 = new PokeMaster.EggsPanel();
            this.tpPlayerInfo = new System.Windows.Forms.TabPage();
            this.playerPanel1 = new PokeMaster.PlayerPanel();
            this.tpBadges = new System.Windows.Forms.TabPage();
            this.badgesPanel1 = new PokeMaster.Panels.BadgesPanel();
            this.tpPokedex = new System.Windows.Forms.TabPage();
            this.pokedexPanel1 = new PokeMaster.Panels.PokedexPanel();
            this.tpSnipers = new System.Windows.Forms.TabPage();
            this.sniperPanel1 = new PokeMaster.SniperPanel();
            this.tpOptions = new System.Windows.Forms.TabPage();
            this.changesPanel1 = new PokeMaster.ChangesPanel();
            this.tpWeb = new System.Windows.Forms.TabPage();
            this.webPanel1 = new PokeMaster.Panels.WebPanel();
            this.loggerPanel1 = new PokemonGo.RocketAPI.Logging.LoggerPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.contextMenuStrip1.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tpLocation.SuspendLayout();
            this.tpPokemons.SuspendLayout();
            this.tpItems.SuspendLayout();
            this.tpEggs.SuspendLayout();
            this.tpPlayerInfo.SuspendLayout();
            this.tpPokedex.SuspendLayout();
            this.tpBadges.SuspendLayout();
            this.tpSnipers.SuspendLayout();
            this.tpOptions.SuspendLayout();
            this.tpWeb.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.label2.Location = new System.Drawing.Point(273, 465);
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
            // TabControl1
            // 
            this.TabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl1.Controls.Add(this.tpLocation);
            this.TabControl1.Controls.Add(this.tpPokemons);
            this.TabControl1.Controls.Add(this.tpItems);
            this.TabControl1.Controls.Add(this.tpEggs);
            this.TabControl1.Controls.Add(this.tpPlayerInfo);
            this.TabControl1.Controls.Add(this.tpPokedex);
            this.TabControl1.Controls.Add(this.tpBadges);
            this.TabControl1.Controls.Add(this.tpSnipers);
            this.TabControl1.Controls.Add(this.tpOptions);
            this.TabControl1.Controls.Add(this.tpWeb);
            this.TabControl1.Location = new System.Drawing.Point(3, 3);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(790, 444);
            this.TabControl1.TabIndex = 0;
            this.TabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tpLocation
            // 
            this.tpLocation.Controls.Add(this.RepeatRoute);
            this.tpLocation.Controls.Add(this.CreateRoute);
            this.tpLocation.Controls.Add(this.locationPanel1);
            this.tpLocation.Location = new System.Drawing.Point(4, 22);
            this.tpLocation.Name = "tpLocation";
            this.tpLocation.Padding = new System.Windows.Forms.Padding(3);
            this.tpLocation.Size = new System.Drawing.Size(782, 418);
            this.tpLocation.TabIndex = 3;
            this.tpLocation.Text = "Location";
            this.tpLocation.UseVisualStyleBackColor = true;
            // 
            // RepeatRoute
            // 
            this.RepeatRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RepeatRoute.AutoSize = true;
            this.RepeatRoute.Enabled = false;
            this.RepeatRoute.Location = new System.Drawing.Point(652, 361);
            this.RepeatRoute.Name = "RepeatRoute";
            this.RepeatRoute.Size = new System.Drawing.Size(93, 17);
            this.RepeatRoute.TabIndex = 46;
            this.RepeatRoute.Text = "Repeat Route";
            this.RepeatRoute.UseVisualStyleBackColor = true;
            // 
            // CreateRoute
            // 
            this.CreateRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateRoute.Location = new System.Drawing.Point(651, 383);
            this.CreateRoute.Margin = new System.Windows.Forms.Padding(2);
            this.CreateRoute.Name = "CreateRoute";
            this.CreateRoute.Size = new System.Drawing.Size(94, 23);
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
            this.locationPanel1.Size = new System.Drawing.Size(781, 417);
            this.locationPanel1.TabIndex = 0;
            // 
            // tpPokemons
            // 
            this.tpPokemons.AutoScroll = true;
            this.tpPokemons.Controls.Add(this.pokemonsPanel1);
            this.tpPokemons.Location = new System.Drawing.Point(4, 22);
            this.tpPokemons.Name = "tpPokemons";
            this.tpPokemons.Padding = new System.Windows.Forms.Padding(3);
            this.tpPokemons.Size = new System.Drawing.Size(782, 418);
            this.tpPokemons.TabIndex = 1;
            this.tpPokemons.Text = "Pokemons";
            this.tpPokemons.UseVisualStyleBackColor = true;
            // 
            // pokemonsPanel1
            // 
            this.pokemonsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pokemonsPanel1.Location = new System.Drawing.Point(4, 4);
            this.pokemonsPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.pokemonsPanel1.Name = "pokemonsPanel1";
            this.pokemonsPanel1.Size = new System.Drawing.Size(774, 407);
            this.pokemonsPanel1.TabIndex = 0;
            // 
            // tpItems
            // 
            this.tpItems.Controls.Add(this.itemsPanel1);
            this.tpItems.Location = new System.Drawing.Point(4, 22);
            this.tpItems.Margin = new System.Windows.Forms.Padding(2);
            this.tpItems.Name = "tpItems";
            this.tpItems.Padding = new System.Windows.Forms.Padding(2);
            this.tpItems.Size = new System.Drawing.Size(782, 418);
            this.tpItems.TabIndex = 2;
            this.tpItems.Text = "Items";
            this.tpItems.UseVisualStyleBackColor = true;
            // 
            // itemsPanel1
            // 
            this.itemsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsPanel1.Location = new System.Drawing.Point(5, 5);
            this.itemsPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.itemsPanel1.Name = "itemsPanel1";
            this.itemsPanel1.Size = new System.Drawing.Size(771, 421);
            this.itemsPanel1.TabIndex = 0;
            // 
            // tpEggs
            // 
            this.tpEggs.Controls.Add(this.eggsPanel1);
            this.tpEggs.Location = new System.Drawing.Point(4, 22);
            this.tpEggs.Margin = new System.Windows.Forms.Padding(2);
            this.tpEggs.Name = "tpEggs";
            this.tpEggs.Padding = new System.Windows.Forms.Padding(2);
            this.tpEggs.Size = new System.Drawing.Size(782, 418);
            this.tpEggs.TabIndex = 5;
            this.tpEggs.Text = "Eggs";
            this.tpEggs.UseVisualStyleBackColor = true;
            // 
            // eggsPanel1
            // 
            this.eggsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.eggsPanel1.Location = new System.Drawing.Point(5, 5);
            this.eggsPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.eggsPanel1.Name = "eggsPanel1";
            this.eggsPanel1.Size = new System.Drawing.Size(773, 409);
            this.eggsPanel1.TabIndex = 0;
            // 
            // tpPlayerInfo
            // 
            this.tpPlayerInfo.Controls.Add(this.playerPanel1);
            this.tpPlayerInfo.Location = new System.Drawing.Point(4, 22);
            this.tpPlayerInfo.Margin = new System.Windows.Forms.Padding(2);
            this.tpPlayerInfo.Name = "tpPlayerInfo";
            this.tpPlayerInfo.Padding = new System.Windows.Forms.Padding(2);
            this.tpPlayerInfo.Size = new System.Drawing.Size(782, 418);
            this.tpPlayerInfo.TabIndex = 4;
            this.tpPlayerInfo.Text = "Player Information";
            this.tpPlayerInfo.UseVisualStyleBackColor = true;
            // 
            // playerPanel1
            // 
            this.playerPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playerPanel1.Location = new System.Drawing.Point(0, 0);
            this.playerPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.playerPanel1.Name = "playerPanel1";
            this.playerPanel1.Size = new System.Drawing.Size(784, 412);
            this.playerPanel1.TabIndex = 0;
            // 
            // tpBadges
            // 
            this.tpBadges.Controls.Add(this.badgesPanel1);
            this.tpBadges.Location = new System.Drawing.Point(4, 22);
            this.tpBadges.Name = "tpBadges";
            this.tpBadges.Padding = new System.Windows.Forms.Padding(3);
            this.tpBadges.Size = new System.Drawing.Size(782, 418);
            this.tpBadges.TabIndex = 8;
            this.tpBadges.Text = "Badges";
            this.tpBadges.UseVisualStyleBackColor = true;
            // 
            // badgesPanel1
            // 
            this.badgesPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.badgesPanel1.Location = new System.Drawing.Point(3, 6);
            this.badgesPanel1.Name = "badgesPanel1";
            this.badgesPanel1.Size = new System.Drawing.Size(773, 406);
            this.badgesPanel1.TabIndex = 0;
            // 
            // tpSnipers
            // 
            this.tpSnipers.Controls.Add(this.sniperPanel1);
            this.tpSnipers.Location = new System.Drawing.Point(4, 22);
            this.tpSnipers.Margin = new System.Windows.Forms.Padding(2);
            this.tpSnipers.Name = "tpSnipers";
            this.tpSnipers.Padding = new System.Windows.Forms.Padding(2);
            this.tpSnipers.Size = new System.Drawing.Size(782, 418);
            this.tpSnipers.TabIndex = 6;
            this.tpSnipers.Text = "Snipers";
            this.tpSnipers.UseVisualStyleBackColor = true;
            // 
            // sniperPanel1
            // 
            this.sniperPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sniperPanel1.Location = new System.Drawing.Point(4, 4);
            this.sniperPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.sniperPanel1.Name = "sniperPanel1";
            this.sniperPanel1.Size = new System.Drawing.Size(772, 407);
            this.sniperPanel1.TabIndex = 0;
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.changesPanel1);
            this.tpOptions.Location = new System.Drawing.Point(4, 22);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpOptions.Size = new System.Drawing.Size(782, 418);
            this.tpOptions.TabIndex = 0;
            this.tpOptions.Text = "Options";
            this.tpOptions.UseVisualStyleBackColor = true;
            // 
            // changesPanel1
            // 
            this.changesPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changesPanel1.Location = new System.Drawing.Point(0, 0);
            this.changesPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.changesPanel1.Name = "changesPanel1";
            this.changesPanel1.Size = new System.Drawing.Size(787, 420);
            this.changesPanel1.TabIndex = 0;
            // 
            // tpWeb
            // 
            this.tpWeb.Controls.Add(this.webPanel1);
            this.tpWeb.Location = new System.Drawing.Point(4, 22);
            this.tpWeb.Name = "tpWeb";
            this.tpWeb.Padding = new System.Windows.Forms.Padding(3);
            this.tpWeb.Size = new System.Drawing.Size(782, 418);
            this.tpWeb.TabIndex = 7;
            this.tpWeb.Text = "Web Browser";
            this.tpWeb.UseVisualStyleBackColor = true;
            // 
            // webPanel1
            // 
            this.webPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webPanel1.Location = new System.Drawing.Point(0, 0);
            this.webPanel1.Name = "webPanel1";
            this.webPanel1.Size = new System.Drawing.Size(776, 415);
            this.webPanel1.TabIndex = 0;
            // 
            // tpPokedex
            // 
            this.tpPokedex.Controls.Add(this.pokedexPanel1);
            this.tpPokedex.Location = new System.Drawing.Point(4, 22);
            this.tpPokedex.Name = "tpPokedex";
            this.tpPokedex.Padding = new System.Windows.Forms.Padding(3);
            this.tpPokedex.Size = new System.Drawing.Size(782, 418);
            this.tpPokedex.TabIndex = 9;
            this.tpPokedex.Text = "Pokedex";
            this.tpPokedex.UseVisualStyleBackColor = true;
            // 
            // loggerPanel1
            // 
            this.loggerPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerPanel1.Location = new System.Drawing.Point(0, 3);
            this.loggerPanel1.Name = "loggerPanel1";
            this.loggerPanel1.Size = new System.Drawing.Size(793, 90);
            this.loggerPanel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 1);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.TabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.loggerPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(793, 547);
            this.splitContainer1.SplitterDistance = 450;
            this.splitContainer1.TabIndex = 10;
            // 
            // pokedexPanel1
            // 
            this.pokedexPanel1.Location = new System.Drawing.Point(1, 3);
            this.pokedexPanel1.Name = "pokedexPanel1";
            this.pokedexPanel1.Size = new System.Drawing.Size(778, 409);
            this.pokedexPanel1.TabIndex = 0;
            // 
            // TabbedSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(798, 550);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(734, 490);
            this.Name = "TabbedSystem";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pokemon List";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Pokemons_Close);
            this.Load += new System.EventHandler(this.Pokemons_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.tpLocation.ResumeLayout(false);
            this.tpLocation.PerformLayout();
            this.tpPokemons.ResumeLayout(false);
            this.tpItems.ResumeLayout(false);
            this.tpEggs.ResumeLayout(false);
            this.tpPlayerInfo.ResumeLayout(false);
            this.tpPokedex.ResumeLayout(false);
            this.tpBadges.ResumeLayout(false);
            this.tpSnipers.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
            this.tpWeb.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl TabControl1;
        private System.Windows.Forms.TabPage tpOptions;
        private System.Windows.Forms.TabPage tpPokemons;
        private System.Windows.Forms.TabPage tpItems;
                
        private PokeMaster.ItemsPanel itemsPanel1;
        private System.Windows.Forms.TabPage tpLocation;
        private PokeMaster.LocationPanel locationPanel1;
        private System.Windows.Forms.TabPage tpPlayerInfo;
        private PokeMaster.PlayerPanel playerPanel1;
        private System.Windows.Forms.TabPage tpEggs;
        private PokeMaster.EggsPanel eggsPanel1;
        private PokeMaster.ChangesPanel changesPanel1;
        private System.Windows.Forms.TabPage tpSnipers;
        private System.Windows.Forms.CheckBox RepeatRoute;
        private System.Windows.Forms.Button CreateRoute;
        private PokeMaster.SniperPanel sniperPanel1;
        private PokeMaster.PokemonsPanel pokemonsPanel1;
        private PokemonGo.RocketAPI.Logging.LoggerPanel loggerPanel1;
        private System.Windows.Forms.TabPage tpWeb;
        private PokeMaster.Panels.WebPanel webPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabPage tpBadges;
        private PokeMaster.Panels.BadgesPanel badgesPanel1;
        private System.Windows.Forms.TabPage tpPokedex;
        private PokeMaster.Panels.PokedexPanel pokedexPanel1;
    }
}