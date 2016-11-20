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
            this.btnPauseWalking = new System.Windows.Forms.Button();
            this.numPwrUpLimit = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUseIncense = new System.Windows.Forms.Button();
            this.btnUseLure = new System.Windows.Forms.Button();
            this.btnUseLuckyEgg = new System.Windows.Forms.Button();
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
            this.PokemonListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PokemonListView.FullRowSelect = true;
            this.PokemonListView.GridLines = true;
            this.PokemonListView.Location = new System.Drawing.Point(4, 4);
            this.PokemonListView.Margin = new System.Windows.Forms.Padding(4);
            this.PokemonListView.Name = "PokemonListView";
            this.PokemonListView.Size = new System.Drawing.Size(667, 233);
            this.PokemonListView.TabIndex = 0;
            this.PokemonListView.UseCompatibleStateImageBehavior = false;
            this.PokemonListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // btnreload
            // 
            this.btnreload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnreload.Location = new System.Drawing.Point(533, 271);
            this.btnreload.Margin = new System.Windows.Forms.Padding(2);
            this.btnreload.Name = "btnreload";
            this.btnreload.Size = new System.Drawing.Size(134, 23);
            this.btnreload.TabIndex = 1;
            this.btnreload.Text = "Reload";
            this.btnreload.UseVisualStyleBackColor = true;
            this.btnreload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnEvolve
            // 
            this.btnEvolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEvolve.Location = new System.Drawing.Point(221, 243);
            this.btnEvolve.Margin = new System.Windows.Forms.Padding(2);
            this.btnEvolve.Name = "btnEvolve";
            this.btnEvolve.Size = new System.Drawing.Size(86, 23);
            this.btnEvolve.TabIndex = 2;
            this.btnEvolve.Text = "Evolve";
            this.btnEvolve.UseVisualStyleBackColor = true;
            this.btnEvolve.Click += new System.EventHandler(this.btnEvolve_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpgrade.Location = new System.Drawing.Point(142, 272);
            this.btnUpgrade.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(74, 23);
            this.btnUpgrade.TabIndex = 3;
            this.btnUpgrade.Text = "PowerUp";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTransfer.Location = new System.Drawing.Point(221, 272);
            this.btnTransfer.Margin = new System.Windows.Forms.Padding(2);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(86, 23);
            this.btnTransfer.TabIndex = 4;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnIVToNick
            // 
            this.btnIVToNick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIVToNick.Location = new System.Drawing.Point(142, 243);
            this.btnIVToNick.Margin = new System.Windows.Forms.Padding(2);
            this.btnIVToNick.Name = "btnIVToNick";
            this.btnIVToNick.Size = new System.Drawing.Size(74, 23);
            this.btnIVToNick.TabIndex = 45;
            this.btnIVToNick.Text = "Nickname";
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(173, 158);
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            this.transferToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.transferToolStripMenuItem.Text = "Transfer";
            this.transferToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // powerUpToolStripMenuItem
            // 
            this.powerUpToolStripMenuItem.Name = "powerUpToolStripMenuItem";
            this.powerUpToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.powerUpToolStripMenuItem.Text = "PowerUp";
            this.powerUpToolStripMenuItem.Click += new System.EventHandler(this.powerUpToolStripMenuItem_Click);
            // 
            // evolveToolStripMenuItem
            // 
            this.evolveToolStripMenuItem.Name = "evolveToolStripMenuItem";
            this.evolveToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.evolveToolStripMenuItem.Text = "Evolve";
            this.evolveToolStripMenuItem.Visible = false;
            this.evolveToolStripMenuItem.Click += new System.EventHandler(this.evolveToolStripMenuItem_Click);
            // 
            // iVsToNicknameToolStripMenuItem
            // 
            this.iVsToNicknameToolStripMenuItem.Name = "iVsToNicknameToolStripMenuItem";
            this.iVsToNicknameToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.iVsToNicknameToolStripMenuItem.Text = "IVs to Nickname";
            this.iVsToNicknameToolStripMenuItem.Click += new System.EventHandler(this.IVsToNicknameToolStripMenuItemClick);
            // 
            // changeFavouritesToolStripMenuItem
            // 
            this.changeFavouritesToolStripMenuItem.Name = "changeFavouritesToolStripMenuItem";
            this.changeFavouritesToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.changeFavouritesToolStripMenuItem.Text = "Change Favourites";
            this.changeFavouritesToolStripMenuItem.Click += new System.EventHandler(this.changeFavouritesToolStripMenuItemClick);
            // 
            // changeBuddyToolStripMenuItem
            // 
            this.changeBuddyToolStripMenuItem.Name = "changeBuddyToolStripMenuItem";
            this.changeBuddyToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.changeBuddyToolStripMenuItem.Text = "Change Buddy";
            this.changeBuddyToolStripMenuItem.Click += new System.EventHandler(this.changeBuddyToolStripMenuItem_Click);
            // 
            // statusTexbox
            // 
            this.statusTexbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusTexbox.Enabled = false;
            this.statusTexbox.Location = new System.Drawing.Point(4, 303);
            this.statusTexbox.Name = "statusTexbox";
            this.statusTexbox.Size = new System.Drawing.Size(497, 20);
            this.statusTexbox.TabIndex = 5;
            // 
            // checkBoxreload
            // 
            this.checkBoxreload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxreload.AutoSize = true;
            this.checkBoxreload.Location = new System.Drawing.Point(533, 250);
            this.checkBoxreload.Name = "checkBoxreload";
            this.checkBoxreload.Size = new System.Drawing.Size(89, 17);
            this.checkBoxreload.TabIndex = 6;
            this.checkBoxreload.Text = "Reload every";
            this.checkBoxreload.UseVisualStyleBackColor = true;
            this.checkBoxreload.CheckedChanged += new System.EventHandler(this.checkboxReload_CheckedChanged);
            // 
            // reloadsecondstextbox
            // 
            this.reloadsecondstextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reloadsecondstextbox.Location = new System.Drawing.Point(627, 248);
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
            this.reloadsecondstextbox.Size = new System.Drawing.Size(40, 20);
            this.reloadsecondstextbox.TabIndex = 7;
            this.reloadsecondstextbox.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(273, 249);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 9;
            // 
            // reloadtimer
            // 
            this.reloadtimer.Interval = 1000;
            this.reloadtimer.Tick += new System.EventHandler(this.reloadtimer_Tick);
            // 
            // btnFullPowerUp
            // 
            this.btnFullPowerUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnFullPowerUp.Location = new System.Drawing.Point(23, 272);
            this.btnFullPowerUp.Margin = new System.Windows.Forms.Padding(2);
            this.btnFullPowerUp.Name = "btnFullPowerUp";
            this.btnFullPowerUp.Size = new System.Drawing.Size(112, 23);
            this.btnFullPowerUp.TabIndex = 11;
            this.btnFullPowerUp.Text = "FULL-PowerUp";
            this.btnFullPowerUp.UseVisualStyleBackColor = true;
            this.btnFullPowerUp.Click += new System.EventHandler(this.btnFullPowerUp_Click);
            // 
            // freezedenshit
            // 
            this.freezedenshit.Interval = 5000;
            this.freezedenshit.Tick += new System.EventHandler(this.freezedenshit_Tick);
            // 
            // btnPauseWalking
            // 
            this.btnPauseWalking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPauseWalking.Location = new System.Drawing.Point(421, 243);
            this.btnPauseWalking.Margin = new System.Windows.Forms.Padding(2);
            this.btnPauseWalking.Name = "btnPauseWalking";
            this.btnPauseWalking.Size = new System.Drawing.Size(106, 23);
            this.btnPauseWalking.TabIndex = 43;
            this.btnPauseWalking.Text = "Pause Walking";
            this.btnPauseWalking.UseVisualStyleBackColor = true;
            this.btnPauseWalking.Click += new System.EventHandler(this.btnForceUnban_Click);
            // 
            // numPwrUpLimit
            // 
            this.numPwrUpLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numPwrUpLimit.Location = new System.Drawing.Point(95, 246);
            this.numPwrUpLimit.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.numPwrUpLimit.Name = "numPwrUpLimit";
            this.numPwrUpLimit.Size = new System.Drawing.Size(39, 20);
            this.numPwrUpLimit.TabIndex = 44;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Power Up Qty";
            // 
            // btnUseIncense
            // 
            this.btnUseIncense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUseIncense.Location = new System.Drawing.Point(421, 271);
            this.btnUseIncense.Margin = new System.Windows.Forms.Padding(2);
            this.btnUseIncense.Name = "btnUseIncense";
            this.btnUseIncense.Size = new System.Drawing.Size(106, 23);
            this.btnUseIncense.TabIndex = 43;
            this.btnUseIncense.Text = "Use Incense";
            this.btnUseIncense.UseVisualStyleBackColor = true;
            this.btnUseIncense.Click += new System.EventHandler(this.btnUseIncense_Click);
            // 
            // btnUseLure
            // 
            this.btnUseLure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUseLure.Location = new System.Drawing.Point(311, 243);
            this.btnUseLure.Margin = new System.Windows.Forms.Padding(2);
            this.btnUseLure.Name = "btnUseLure";
            this.btnUseLure.Size = new System.Drawing.Size(106, 23);
            this.btnUseLure.TabIndex = 43;
            this.btnUseLure.Text = "Use Lure";
            this.btnUseLure.UseVisualStyleBackColor = true;
            this.btnUseLure.Click += new System.EventHandler(this.btnUseLure_Click);
            // 
            // btnUseLuckyEgg
            // 
            this.btnUseLuckyEgg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUseLuckyEgg.Location = new System.Drawing.Point(311, 271);
            this.btnUseLuckyEgg.Margin = new System.Windows.Forms.Padding(2);
            this.btnUseLuckyEgg.Name = "btnUseLuckyEgg";
            this.btnUseLuckyEgg.Size = new System.Drawing.Size(106, 23);
            this.btnUseLuckyEgg.TabIndex = 43;
            this.btnUseLuckyEgg.Text = "Use Lucky Egg";
            this.btnUseLuckyEgg.UseVisualStyleBackColor = true;
            this.btnUseLuckyEgg.Click += new System.EventHandler(this.btnUseLuckyEgg_Click);
            // 
            // lang_tr_btn2
            // 
            this.lang_tr_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lang_tr_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.tr1;
            this.lang_tr_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_tr_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_tr_btn2.Location = new System.Drawing.Point(627, 305);
            this.lang_tr_btn2.Name = "lang_tr_btn2";
            this.lang_tr_btn2.Size = new System.Drawing.Size(24, 15);
            this.lang_tr_btn2.TabIndex = 42;
            this.lang_tr_btn2.UseVisualStyleBackColor = false;
            this.lang_tr_btn2.Click += new System.EventHandler(this.lang_tr_btn2_Click);
            // 
            // lang_ptBR_btn2
            // 
            this.lang_ptBR_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lang_ptBR_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.ptBR;
            this.lang_ptBR_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_ptBR_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_ptBR_btn2.Location = new System.Drawing.Point(597, 305);
            this.lang_ptBR_btn2.Name = "lang_ptBR_btn2";
            this.lang_ptBR_btn2.Size = new System.Drawing.Size(24, 15);
            this.lang_ptBR_btn2.TabIndex = 42;
            this.lang_ptBR_btn2.UseVisualStyleBackColor = false;
            this.lang_ptBR_btn2.Click += new System.EventHandler(this.lang_ptBR_btn2_Click);
            // 
            // lang_spain_btn2
            // 
            this.lang_spain_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lang_spain_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.spain;
            this.lang_spain_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_spain_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_spain_btn2.Location = new System.Drawing.Point(567, 305);
            this.lang_spain_btn2.Name = "lang_spain_btn2";
            this.lang_spain_btn2.Size = new System.Drawing.Size(24, 15);
            this.lang_spain_btn2.TabIndex = 15;
            this.lang_spain_btn2.UseVisualStyleBackColor = false;
            this.lang_spain_btn2.Click += new System.EventHandler(this.lang_spain_btn2_Click);
            // 
            // lang_de_btn_2
            // 
            this.lang_de_btn_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lang_de_btn_2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.de;
            this.lang_de_btn_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_de_btn_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_de_btn_2.Location = new System.Drawing.Point(537, 305);
            this.lang_de_btn_2.Name = "lang_de_btn_2";
            this.lang_de_btn_2.Size = new System.Drawing.Size(24, 15);
            this.lang_de_btn_2.TabIndex = 14;
            this.lang_de_btn_2.UseVisualStyleBackColor = false;
            this.lang_de_btn_2.Click += new System.EventHandler(this.lang_de_btn_2_Click);
            // 
            // lang_en_btn2
            // 
            this.lang_en_btn2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lang_en_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.en;
            this.lang_en_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_en_btn2.Enabled = false;
            this.lang_en_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_en_btn2.Location = new System.Drawing.Point(507, 305);
            this.lang_en_btn2.Name = "lang_en_btn2";
            this.lang_en_btn2.Size = new System.Drawing.Size(24, 15);
            this.lang_en_btn2.TabIndex = 13;
            this.lang_en_btn2.UseVisualStyleBackColor = false;
            this.lang_en_btn2.Click += new System.EventHandler(this.lang_en_btn2_Click);
            // 
            // PokemonsPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.btnreload);
            this.Controls.Add(this.reloadsecondstextbox);
            this.Controls.Add(this.checkBoxreload);
            this.Controls.Add(this.btnUseLure);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnPauseWalking);
            this.Controls.Add(this.PokemonListView);
            this.Controls.Add(this.btnUseIncense);
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
            this.Controls.Add(this.btnUseLuckyEgg);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PokemonsPanel";
            this.Size = new System.Drawing.Size(675, 328);
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
        private System.Windows.Forms.Button btnPauseWalking;
        private System.Windows.Forms.NumericUpDown numPwrUpLimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUseIncense;
        private System.Windows.Forms.Button btnUseLure;
        private System.Windows.Forms.Button btnUseLuckyEgg;
        private System.Windows.Forms.ToolStripMenuItem changeBuddyToolStripMenuItem;
    }
}