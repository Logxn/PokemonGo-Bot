using System.Drawing;
namespace PokemonGo.RocketAPI.Console
{
    partial class PokemonsPanel
    {
       
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokemonsPanel));
            this.PokemonListView = new System.Windows.Forms.ListView();
            this.btnreload = new System.Windows.Forms.Button();
            this.btnEvolve = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.btnTransfer = new System.Windows.Forms.Button();
            this.btnIVToNick = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.powerUpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.evolveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iVsToNicknameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeFavouritesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeBuddyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusTexbox = new System.Windows.Forms.TextBox();
            this.checkBoxreload = new System.Windows.Forms.CheckBox();
            this.reloadsecondstextbox = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.reloadtimer = new System.Windows.Forms.Timer(this.components);
            this.btnFullPowerUp = new System.Windows.Forms.Button();
            this.freezedenshit = new System.Windows.Forms.Timer(this.components);
            this.numPwrUpLimit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lang_tr_btn2 = new System.Windows.Forms.Button();
            this.lang_ptBR_btn2 = new System.Windows.Forms.Button();
            this.lang_spain_btn2 = new System.Windows.Forms.Button();
            this.lang_de_btn_2 = new System.Windows.Forms.Button();
            this.lang_en_btn2 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.reloadsecondstextbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPwrUpLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // PokemonListView
            // 
            this.PokemonListView.AllowColumnReorder = true;
            resources.ApplyResources(this.PokemonListView, "PokemonListView");
            this.PokemonListView.FullRowSelect = true;
            this.PokemonListView.GridLines = true;
            this.PokemonListView.Name = "PokemonListView";
            this.PokemonListView.UseCompatibleStateImageBehavior = false;
            this.PokemonListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // btnreload
            // 
            resources.ApplyResources(this.btnreload, "btnreload");
            this.btnreload.Name = "btnreload";
            this.btnreload.UseVisualStyleBackColor = true;
            this.btnreload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnEvolve
            // 
            resources.ApplyResources(this.btnEvolve, "btnEvolve");
            this.btnEvolve.Name = "btnEvolve";
            this.btnEvolve.UseVisualStyleBackColor = true;
            this.btnEvolve.Click += new System.EventHandler(this.btnEvolve_Click);
            // 
            // btnUpgrade
            // 
            resources.ApplyResources(this.btnUpgrade, "btnUpgrade");
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnTransfer
            // 
            resources.ApplyResources(this.btnTransfer, "btnTransfer");
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnIVToNick
            // 
            resources.ApplyResources(this.btnIVToNick, "btnIVToNick");
            this.btnIVToNick.Name = "btnIVToNick";
            this.btnIVToNick.UseVisualStyleBackColor = true;
            this.btnIVToNick.Click += new System.EventHandler(this.BtnIVToNickClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferToolStripMenuItem,
            this.powerUpToolStripMenuItem,
            this.evolveToolStripMenuItem,
            this.iVsToNicknameToolStripMenuItem,
            this.changeFavouritesToolStripMenuItem,
            this.changeBuddyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            resources.ApplyResources(this.transferToolStripMenuItem, "transferToolStripMenuItem");
            this.transferToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // powerUpToolStripMenuItem
            // 
            this.powerUpToolStripMenuItem.Name = "powerUpToolStripMenuItem";
            resources.ApplyResources(this.powerUpToolStripMenuItem, "powerUpToolStripMenuItem");
            this.powerUpToolStripMenuItem.Click += new System.EventHandler(this.powerUpToolStripMenuItem_Click);
            // 
            // evolveToolStripMenuItem
            // 
            this.evolveToolStripMenuItem.Name = "evolveToolStripMenuItem";
            resources.ApplyResources(this.evolveToolStripMenuItem, "evolveToolStripMenuItem");
            this.evolveToolStripMenuItem.Click += new System.EventHandler(this.evolveToolStripMenuItem_Click);
            // 
            // iVsToNicknameToolStripMenuItem
            // 
            this.iVsToNicknameToolStripMenuItem.Name = "iVsToNicknameToolStripMenuItem";
            resources.ApplyResources(this.iVsToNicknameToolStripMenuItem, "iVsToNicknameToolStripMenuItem");
            this.iVsToNicknameToolStripMenuItem.Click += new System.EventHandler(this.IVsToNicknameToolStripMenuItemClick);
            // 
            // changeFavouritesToolStripMenuItem
            // 
            this.changeFavouritesToolStripMenuItem.Name = "changeFavouritesToolStripMenuItem";
            resources.ApplyResources(this.changeFavouritesToolStripMenuItem, "changeFavouritesToolStripMenuItem");
            this.changeFavouritesToolStripMenuItem.Click += new System.EventHandler(this.changeFavouritesToolStripMenuItemClick);
            // 
            // changeBuddyToolStripMenuItem
            // 
            this.changeBuddyToolStripMenuItem.Name = "changeBuddyToolStripMenuItem";
            resources.ApplyResources(this.changeBuddyToolStripMenuItem, "changeBuddyToolStripMenuItem");
            this.changeBuddyToolStripMenuItem.Click += new System.EventHandler(this.changeBuddyToolStripMenuItem_Click);
            // 
            // statusTexbox
            // 
            resources.ApplyResources(this.statusTexbox, "statusTexbox");
            this.statusTexbox.Name = "statusTexbox";
            // 
            // checkBoxreload
            // 
            resources.ApplyResources(this.checkBoxreload, "checkBoxreload");
            this.checkBoxreload.Name = "checkBoxreload";
            this.checkBoxreload.UseVisualStyleBackColor = true;
            this.checkBoxreload.CheckedChanged += new System.EventHandler(this.checkboxReload_CheckedChanged);
            // 
            // reloadsecondstextbox
            // 
            resources.ApplyResources(this.reloadsecondstextbox, "reloadsecondstextbox");
            this.reloadsecondstextbox.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.reloadsecondstextbox.Minimum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.reloadsecondstextbox.Name = "reloadsecondstextbox";
            this.reloadsecondstextbox.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // reloadtimer
            // 
            this.reloadtimer.Interval = 1000;
            this.reloadtimer.Tick += new System.EventHandler(this.reloadtimer_Tick);
            // 
            // btnFullPowerUp
            // 
            resources.ApplyResources(this.btnFullPowerUp, "btnFullPowerUp");
            this.btnFullPowerUp.Name = "btnFullPowerUp";
            this.btnFullPowerUp.UseVisualStyleBackColor = true;
            this.btnFullPowerUp.Click += new System.EventHandler(this.btnFullPowerUp_Click);
            // 
            // freezedenshit
            // 
            this.freezedenshit.Interval = 5000;
            this.freezedenshit.Tick += new System.EventHandler(this.freezedenshit_Tick);
            // 
            // numPwrUpLimit
            // 
            resources.ApplyResources(this.numPwrUpLimit, "numPwrUpLimit");
            this.numPwrUpLimit.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numPwrUpLimit.Name = "numPwrUpLimit";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lang_tr_btn2
            // 
            resources.ApplyResources(this.lang_tr_btn2, "lang_tr_btn2");
            this.lang_tr_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lang_tr_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.tr1;
            this.lang_tr_btn2.Name = "lang_tr_btn2";
            this.lang_tr_btn2.UseVisualStyleBackColor = false;
            this.lang_tr_btn2.Click += new System.EventHandler(this.lang_tr_btn2_Click);
            // 
            // lang_ptBR_btn2
            // 
            resources.ApplyResources(this.lang_ptBR_btn2, "lang_ptBR_btn2");
            this.lang_ptBR_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lang_ptBR_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.ptBR;
            this.lang_ptBR_btn2.Name = "lang_ptBR_btn2";
            this.lang_ptBR_btn2.UseVisualStyleBackColor = false;
            this.lang_ptBR_btn2.Click += new System.EventHandler(this.lang_ptBR_btn2_Click);
            // 
            // lang_spain_btn2
            // 
            resources.ApplyResources(this.lang_spain_btn2, "lang_spain_btn2");
            this.lang_spain_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lang_spain_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.spain;
            this.lang_spain_btn2.Name = "lang_spain_btn2";
            this.lang_spain_btn2.UseVisualStyleBackColor = false;
            this.lang_spain_btn2.Click += new System.EventHandler(this.lang_spain_btn2_Click);
            // 
            // lang_de_btn_2
            // 
            resources.ApplyResources(this.lang_de_btn_2, "lang_de_btn_2");
            this.lang_de_btn_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lang_de_btn_2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.de;
            this.lang_de_btn_2.Name = "lang_de_btn_2";
            this.lang_de_btn_2.UseVisualStyleBackColor = false;
            this.lang_de_btn_2.Click += new System.EventHandler(this.lang_de_btn_2_Click);
            // 
            // lang_en_btn2
            // 
            resources.ApplyResources(this.lang_en_btn2, "lang_en_btn2");
            this.lang_en_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lang_en_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.en;
            this.lang_en_btn2.Name = "lang_en_btn2";
            this.lang_en_btn2.UseVisualStyleBackColor = false;
            this.lang_en_btn2.Click += new System.EventHandler(this.lang_en_btn2_Click);
            // 
            // PokemonsPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.btnreload);
            this.Controls.Add(this.reloadsecondstextbox);
            this.Controls.Add(this.checkBoxreload);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.PokemonListView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lang_tr_btn2);
            this.Controls.Add(this.lang_ptBR_btn2);
            this.Controls.Add(this.btnFullPowerUp);
            this.Controls.Add(this.lang_spain_btn2);
            this.Controls.Add(this.lang_de_btn_2);
            this.Controls.Add(this.numPwrUpLimit);
            this.Controls.Add(this.lang_en_btn2);
            this.Controls.Add(this.statusTexbox);
            this.Controls.Add(this.btnEvolve);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnIVToNick);
            resources.ApplyResources(this, "$this");
            this.Name = "PokemonsPanel";
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.reloadsecondstextbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPwrUpLimit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ListView PokemonListView;
        private System.Windows.Forms.Button btnreload;
        private System.Windows.Forms.Button btnEvolve;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.Button btnIVToNick;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem transferToolStripMenuItem;
        private System.Windows.Forms.TextBox statusTexbox;
        private System.Windows.Forms.ToolStripMenuItem powerUpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem evolveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iVsToNicknameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeFavouritesToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxreload;
        private System.Windows.Forms.NumericUpDown reloadsecondstextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer reloadtimer;
        private System.Windows.Forms.Button btnFullPowerUp;
        private System.Windows.Forms.Button lang_en_btn2;
        private System.Windows.Forms.Button lang_de_btn_2;
        private System.Windows.Forms.Button lang_spain_btn2;
        private System.Windows.Forms.Button lang_ptBR_btn2;
        private System.Windows.Forms.Timer freezedenshit;
        private System.Windows.Forms.Button lang_tr_btn2;
        private System.Windows.Forms.NumericUpDown numPwrUpLimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem changeBuddyToolStripMenuItem;
    }
}