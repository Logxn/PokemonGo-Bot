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
            this.tpConsole = new System.Windows.Forms.TabPage();
            this.loggerPanel1 = new PokemonGo.RocketAPI.Logging.LoggerPanel();
            this.contextMenuStrip1.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tpLocation.SuspendLayout();
            this.tpPokemons.SuspendLayout();
            this.tpItems.SuspendLayout();
            this.tpEggs.SuspendLayout();
            this.tpPlayerInfo.SuspendLayout();
            this.tpSnipers.SuspendLayout();
            this.tpOptions.SuspendLayout();
            this.tpConsole.SuspendLayout();
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
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            resources.ApplyResources(this.transferToolStripMenuItem, "transferToolStripMenuItem");
            // 
            // powerUpToolStripMenuItem
            // 
            this.powerUpToolStripMenuItem.Name = "powerUpToolStripMenuItem";
            resources.ApplyResources(this.powerUpToolStripMenuItem, "powerUpToolStripMenuItem");
            // 
            // evolveToolStripMenuItem
            // 
            this.evolveToolStripMenuItem.Name = "evolveToolStripMenuItem";
            resources.ApplyResources(this.evolveToolStripMenuItem, "evolveToolStripMenuItem");
            // 
            // iVsToNicknameToolStripMenuItem
            // 
            this.iVsToNicknameToolStripMenuItem.Name = "iVsToNicknameToolStripMenuItem";
            resources.ApplyResources(this.iVsToNicknameToolStripMenuItem, "iVsToNicknameToolStripMenuItem");
            // 
            // changeFavouritesToolStripMenuItem
            // 
            this.changeFavouritesToolStripMenuItem.Name = "changeFavouritesToolStripMenuItem";
            resources.ApplyResources(this.changeFavouritesToolStripMenuItem, "changeFavouritesToolStripMenuItem");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
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
            resources.ApplyResources(this.TabControl1, "TabControl1");
            this.TabControl1.Controls.Add(this.tpLocation);
            this.TabControl1.Controls.Add(this.tpPokemons);
            this.TabControl1.Controls.Add(this.tpItems);
            this.TabControl1.Controls.Add(this.tpEggs);
            this.TabControl1.Controls.Add(this.tpPlayerInfo);
            this.TabControl1.Controls.Add(this.tpSnipers);
            this.TabControl1.Controls.Add(this.tpOptions);
            //this.TabControl1.Controls.Add(this.tpConsole);
            this.TabControl1.Location = new System.Drawing.Point(4, 4);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tpLocation
            // 
            this.tpLocation.Controls.Add(this.RepeatRoute);
            this.tpLocation.Controls.Add(this.CreateRoute);
            this.tpLocation.Controls.Add(this.locationPanel1);
            resources.ApplyResources(this.tpLocation, "tpLocation");
            this.tpLocation.Name = "tpLocation";
            this.tpLocation.UseVisualStyleBackColor = true;
            // 
            // RepeatRoute
            // 
            resources.ApplyResources(this.RepeatRoute, "RepeatRoute");
            this.RepeatRoute.Name = "RepeatRoute";
            this.RepeatRoute.UseVisualStyleBackColor = true;
            // 
            // CreateRoute
            // 
            resources.ApplyResources(this.CreateRoute, "CreateRoute");
            this.CreateRoute.Name = "CreateRoute";
            this.CreateRoute.UseVisualStyleBackColor = true;
            this.CreateRoute.Click += new System.EventHandler(this.CreateRoute_Click);
            // 
            // locationPanel1
            // 
            resources.ApplyResources(this.locationPanel1, "locationPanel1");
            this.locationPanel1.Name = "locationPanel1";
            // 
            // tpPokemons
            // 
            resources.ApplyResources(this.tpPokemons, "tpPokemons");
            this.tpPokemons.Controls.Add(this.pokemonsPanel1);
            this.tpPokemons.Name = "tpPokemons";
            this.tpPokemons.UseVisualStyleBackColor = true;
            // 
            // pokemonsPanel1
            // 
            resources.ApplyResources(this.pokemonsPanel1, "pokemonsPanel1");
            this.pokemonsPanel1.Name = "pokemonsPanel1";
            // 
            // tpItems
            // 
            this.tpItems.Controls.Add(this.itemsPanel1);
            resources.ApplyResources(this.tpItems, "tpItems");
            this.tpItems.Name = "tpItems";
            this.tpItems.UseVisualStyleBackColor = true;
            // 
            // itemsPanel1
            // 
            resources.ApplyResources(this.itemsPanel1, "itemsPanel1");
            this.itemsPanel1.Name = "itemsPanel1";
            // 
            // tpEggs
            // 
            this.tpEggs.Controls.Add(this.eggsPanel1);
            resources.ApplyResources(this.tpEggs, "tpEggs");
            this.tpEggs.Name = "tpEggs";
            this.tpEggs.UseVisualStyleBackColor = true;
            // 
            // eggsPanel1
            // 
            resources.ApplyResources(this.eggsPanel1, "eggsPanel1");
            this.eggsPanel1.Name = "eggsPanel1";
            // 
            // tpPlayerInfo
            // 
            this.tpPlayerInfo.Controls.Add(this.playerPanel1);
            resources.ApplyResources(this.tpPlayerInfo, "tpPlayerInfo");
            this.tpPlayerInfo.Name = "tpPlayerInfo";
            this.tpPlayerInfo.UseVisualStyleBackColor = true;
            // 
            // playerPanel1
            // 
            resources.ApplyResources(this.playerPanel1, "playerPanel1");
            this.playerPanel1.Name = "playerPanel1";
            // 
            // tpSnipers
            // 
            this.tpSnipers.Controls.Add(this.sniperPanel1);
            resources.ApplyResources(this.tpSnipers, "tpSnipers");
            this.tpSnipers.Name = "tpSnipers";
            this.tpSnipers.UseVisualStyleBackColor = true;
            // 
            // sniperPanel1
            // 
            resources.ApplyResources(this.sniperPanel1, "sniperPanel1");
            this.sniperPanel1.Name = "sniperPanel1";
            // 
            // tpOptions
            // 
            this.tpOptions.Controls.Add(this.changesPanel1);
            resources.ApplyResources(this.tpOptions, "tpOptions");
            this.tpOptions.Name = "tpOptions";
            this.tpOptions.UseVisualStyleBackColor = true;
            // 
            // changesPanel1
            // 
            resources.ApplyResources(this.changesPanel1, "changesPanel1");
            this.changesPanel1.Name = "changesPanel1";
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.loggerPanel1);
            resources.ApplyResources(this.tpConsole, "tpConsole");
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // loggerPanel1
            // 
            resources.ApplyResources(this.loggerPanel1, "loggerPanel1");
            this.loggerPanel1.Name = "loggerPanel1";
            // 
            // tpConsole
            // 
            this.tpConsole.Controls.Add(this.loggerPanel1);
            this.tpConsole.Location = new System.Drawing.Point(4, 22);
            this.tpConsole.Name = "tpConsole";
            this.tpConsole.Padding = new System.Windows.Forms.Padding(3);
            this.tpConsole.Size = new System.Drawing.Size(757, 517);
            this.tpConsole.TabIndex = 7;
            this.tpConsole.Text = "Console";
            this.tpConsole.UseVisualStyleBackColor = true;
            // 
            // loggerPanel1
            // 
            this.loggerPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggerPanel1.Location = new System.Drawing.Point(6, 6);
            this.loggerPanel1.Name = "loggerPanel1";
            this.loggerPanel1.Size = new System.Drawing.Size(745, 505);
            this.loggerPanel1.TabIndex = 0;
            // 
            // Pokemons
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TabControl1);
            this.Name = "Pokemons";
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
            this.tpConsole.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage tpConsole;
        private PokemonGo.RocketAPI.Logging.LoggerPanel loggerPanel1;
    }
}