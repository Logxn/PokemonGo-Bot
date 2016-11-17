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
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tpLocation = new System.Windows.Forms.TabPage();
            this.RepeatRoute = new System.Windows.Forms.CheckBox();
            this.CreateRoute = new System.Windows.Forms.Button();
            this.locationPanel1 = new PokemonGo.RocketAPI.Console.LocationPanel();
            this.tpPokemons = new System.Windows.Forms.TabPage();
            this.pokemonsPanel1 = new PokemonGo.RocketAPI.Console.PokemonsPanel();
            this.tpItems = new System.Windows.Forms.TabPage();
            this.itemsPanel1 = new PokemonGo.RocketAPI.Console.ItemsPanel();
            this.tpEggs = new System.Windows.Forms.TabPage();
            this.eggsPanel1 = new PokemonGo.RocketAPI.Console.EggsPanel();
            this.tpPlayerInfo = new System.Windows.Forms.TabPage();
            this.playerPanel1 = new PokemonGo.RocketAPI.Console.PlayerPanel();
            this.tpSnipers = new System.Windows.Forms.TabPage();
            this.sniperPanel1 = new PokemonGo.RocketAPI.Console.SniperPanel();
            this.tpOptions = new System.Windows.Forms.TabPage();
            this.changesPanel1 = new PokemonGo.RocketAPI.Console.ChangesPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tpLocation.SuspendLayout();
            this.tpPokemons.SuspendLayout();
            this.tpItems.SuspendLayout();
            this.tpEggs.SuspendLayout();
            this.tpPlayerInfo.SuspendLayout();
            this.tpSnipers.SuspendLayout();
            this.tpOptions.SuspendLayout();
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
            this.TabControl1.Controls.Add(this.tpSnipers);
            this.TabControl1.Controls.Add(this.tpOptions);
            this.TabControl1.Location = new System.Drawing.Point(4, 4);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(765, 543);
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
            this.tpLocation.Size = new System.Drawing.Size(757, 517);
            this.tpLocation.TabIndex = 3;
            this.tpLocation.Text = "Location";
            this.tpLocation.UseVisualStyleBackColor = true;
            // 
            // RepeatRoute
            // 
            this.RepeatRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RepeatRoute.AutoSize = true;
            this.RepeatRoute.Enabled = false;
            this.RepeatRoute.Location = new System.Drawing.Point(659, 469);
            this.RepeatRoute.Name = "RepeatRoute";
            this.RepeatRoute.Size = new System.Drawing.Size(93, 17);
            this.RepeatRoute.TabIndex = 46;
            this.RepeatRoute.Text = "Repeat Route";
            this.RepeatRoute.UseVisualStyleBackColor = true;
            // 
            // CreateRoute
            // 
            this.CreateRoute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateRoute.Location = new System.Drawing.Point(658, 491);
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
            this.locationPanel1.Size = new System.Drawing.Size(756, 516);
            this.locationPanel1.TabIndex = 0;
            // 
            // tpPokemons
            // 
            this.tpPokemons.AutoScroll = true;
            this.tpPokemons.Controls.Add(this.pokemonsPanel1);
            this.tpPokemons.Location = new System.Drawing.Point(4, 22);
            this.tpPokemons.Name = "tpPokemons";
            this.tpPokemons.Padding = new System.Windows.Forms.Padding(3);
            this.tpPokemons.Size = new System.Drawing.Size(757, 517);
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
            this.pokemonsPanel1.Size = new System.Drawing.Size(749, 506);
            this.pokemonsPanel1.TabIndex = 0;
            // 
            // tpItems
            // 
            this.tpItems.Controls.Add(this.itemsPanel1);
            this.tpItems.Location = new System.Drawing.Point(4, 22);
            this.tpItems.Margin = new System.Windows.Forms.Padding(2);
            this.tpItems.Name = "tpItems";
            this.tpItems.Padding = new System.Windows.Forms.Padding(2);
            this.tpItems.Size = new System.Drawing.Size(757, 517);
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
            this.itemsPanel1.Size = new System.Drawing.Size(746, 520);
            this.itemsPanel1.TabIndex = 0;
            // 
            // tpEggs
            // 
            this.tpEggs.Controls.Add(this.eggsPanel1);
            this.tpEggs.Location = new System.Drawing.Point(4, 22);
            this.tpEggs.Margin = new System.Windows.Forms.Padding(2);
            this.tpEggs.Name = "tpEggs";
            this.tpEggs.Padding = new System.Windows.Forms.Padding(2);
            this.tpEggs.Size = new System.Drawing.Size(757, 517);
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
            this.eggsPanel1.Size = new System.Drawing.Size(748, 508);
            this.eggsPanel1.TabIndex = 0;
            // 
            // tpPlayerInfo
            // 
            this.tpPlayerInfo.Controls.Add(this.playerPanel1);
            this.tpPlayerInfo.Location = new System.Drawing.Point(4, 22);
            this.tpPlayerInfo.Margin = new System.Windows.Forms.Padding(2);
            this.tpPlayerInfo.Name = "tpPlayerInfo";
            this.tpPlayerInfo.Padding = new System.Windows.Forms.Padding(2);
            this.tpPlayerInfo.Size = new System.Drawing.Size(757, 517);
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
            this.playerPanel1.Size = new System.Drawing.Size(759, 511);
            this.playerPanel1.TabIndex = 0;
            // 
            // tpSnipers
            // 
            this.tpSnipers.Controls.Add(this.sniperPanel1);
            this.tpSnipers.Location = new System.Drawing.Point(4, 22);
            this.tpSnipers.Margin = new System.Windows.Forms.Padding(2);
            this.tpSnipers.Name = "tpSnipers";
            this.tpSnipers.Padding = new System.Windows.Forms.Padding(2);
            this.tpSnipers.Size = new System.Drawing.Size(757, 517);
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
            this.sniperPanel1.Size = new System.Drawing.Size(747, 506);
            this.sniperPanel1.TabIndex = 0;
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.changesPanel1);
            this.tpOptions.Location = new System.Drawing.Point(4, 22);
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpOptions.Size = new System.Drawing.Size(757, 517);
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
            this.changesPanel1.Size = new System.Drawing.Size(762, 519);
            this.changesPanel1.TabIndex = 0;
            // 
            // Pokemons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(773, 550);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(734, 490);
            this.Name = "Pokemons";
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
            this.tpSnipers.ResumeLayout(false);
            this.tpOptions.ResumeLayout(false);
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
                
        private System.Windows.Forms.TextBox NumericUpDown;
        private PokemonGo.RocketAPI.Console.ItemsPanel itemsPanel1;
        private System.Windows.Forms.TabPage tpLocation;
        private PokemonGo.RocketAPI.Console.LocationPanel locationPanel1;
        private System.Windows.Forms.TabPage tpPlayerInfo;
        private PokemonGo.RocketAPI.Console.PlayerPanel playerPanel1;
        private System.Windows.Forms.TabPage tpEggs;
        private PokemonGo.RocketAPI.Console.EggsPanel eggsPanel1;
        private PokemonGo.RocketAPI.Console.ChangesPanel changesPanel1;
        private System.Windows.Forms.TabPage tpSnipers;
        private System.Windows.Forms.CheckBox RepeatRoute;
        private System.Windows.Forms.Button CreateRoute;
        private PokemonGo.RocketAPI.Console.SniperPanel sniperPanel1;
        private PokemonGo.RocketAPI.Console.PokemonsPanel pokemonsPanel1;
    }
}