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
            this.statusTexbox = new System.Windows.Forms.TextBox();
            this.checkBoxreload = new System.Windows.Forms.CheckBox();
            this.reloadsecondstextbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.reloadtimer = new System.Windows.Forms.Timer(this.components);
            this.btnFullPowerUp = new System.Windows.Forms.Button();
            this.lang_spain_btn2 = new System.Windows.Forms.Button();
            this.lang_de_btn_2 = new System.Windows.Forms.Button();
            this.lang_en_btn2 = new System.Windows.Forms.Button();
            this.btnShowMap = new System.Windows.Forms.Button();
            this.lang_ptBR_btn2 = new System.Windows.Forms.Button();
            this.freezedenshit = new System.Windows.Forms.Timer(this.components);
            this.lang_tr_btn2 = new System.Windows.Forms.Button();
            this.btnForceUnban = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
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
            this.PokemonListView.Location = new System.Drawing.Point(16, 32);
            this.PokemonListView.Margin = new System.Windows.Forms.Padding(4);
            this.PokemonListView.Name = "PokemonListView";
            this.PokemonListView.Size = new System.Drawing.Size(877, 416);
            this.PokemonListView.TabIndex = 0;
            this.PokemonListView.UseCompatibleStateImageBehavior = false;
            this.PokemonListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // btnreload
            // 
            this.btnreload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnreload.Location = new System.Drawing.Point(16, 457);
            this.btnreload.Margin = new System.Windows.Forms.Padding(2);
            this.btnreload.Name = "btnreload";
            this.btnreload.Size = new System.Drawing.Size(73, 28);
            this.btnreload.TabIndex = 1;
            this.btnreload.Text = "Reload";
            this.btnreload.UseVisualStyleBackColor = true;
            this.btnreload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnEvolve
            // 
            this.btnEvolve.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEvolve.Location = new System.Drawing.Point(108, 494);
            this.btnEvolve.Margin = new System.Windows.Forms.Padding(2);
            this.btnEvolve.Name = "btnEvolve";
            this.btnEvolve.Size = new System.Drawing.Size(104, 28);
            this.btnEvolve.TabIndex = 2;
            this.btnEvolve.Text = "Evolve";
            this.btnEvolve.UseVisualStyleBackColor = true;
            this.btnEvolve.Click += new System.EventHandler(this.btnEvolve_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpgrade.Location = new System.Drawing.Point(217, 494);
            this.btnUpgrade.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(98, 28);
            this.btnUpgrade.TabIndex = 3;
            this.btnUpgrade.Text = "PowerUp";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // btnTransfer
            // 
            this.btnTransfer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnTransfer.Location = new System.Drawing.Point(459, 494);
            this.btnTransfer.Margin = new System.Windows.Forms.Padding(2);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new System.Drawing.Size(115, 28);
            this.btnTransfer.TabIndex = 4;
            this.btnTransfer.Text = "Transfer";
            this.btnTransfer.UseVisualStyleBackColor = true;
            this.btnTransfer.Click += new System.EventHandler(this.btnTransfer_Click);
            // 
            // btnIVToNick
            // 
            this.btnIVToNick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIVToNick.Location = new System.Drawing.Point(17, 494);
            this.btnIVToNick.Margin = new System.Windows.Forms.Padding(2);
            this.btnIVToNick.Name = "btnIVToNick";
            this.btnIVToNick.Size = new System.Drawing.Size(84, 28);
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
            this.iVsToNicknameToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(186, 100);
            this.contextMenuStrip1.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextMenuStrip1_Closing);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            this.transferToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.transferToolStripMenuItem.Text = "Transfer";
            this.transferToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // powerUpToolStripMenuItem
            // 
            this.powerUpToolStripMenuItem.Name = "powerUpToolStripMenuItem";
            this.powerUpToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.powerUpToolStripMenuItem.Text = "PowerUp";
            this.powerUpToolStripMenuItem.Click += new System.EventHandler(this.powerUpToolStripMenuItem_Click);
            // 
            // evolveToolStripMenuItem
            // 
            this.evolveToolStripMenuItem.Name = "evolveToolStripMenuItem";
            this.evolveToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.evolveToolStripMenuItem.Text = "Evolve";
            this.evolveToolStripMenuItem.Visible = false;
            this.evolveToolStripMenuItem.Click += new System.EventHandler(this.evolveToolStripMenuItem_Click);
            // 
            // iVsToNicknameToolStripMenuItem
            // 
            this.iVsToNicknameToolStripMenuItem.Name = "iVsToNicknameToolStripMenuItem";
            this.iVsToNicknameToolStripMenuItem.Size = new System.Drawing.Size(185, 24);
            this.iVsToNicknameToolStripMenuItem.Text = "IVs to Nickname";
            this.iVsToNicknameToolStripMenuItem.Click += new System.EventHandler(this.IVsToNicknameToolStripMenuItemClick);
            // 
            // statusTexbox
            // 
            this.statusTexbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusTexbox.Enabled = false;
            this.statusTexbox.Location = new System.Drawing.Point(16, 528);
            this.statusTexbox.Margin = new System.Windows.Forms.Padding(4);
            this.statusTexbox.Name = "statusTexbox";
            this.statusTexbox.Size = new System.Drawing.Size(875, 22);
            this.statusTexbox.TabIndex = 5;
            // 
            // checkBoxreload
            // 
            this.checkBoxreload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxreload.AutoSize = true;
            this.checkBoxreload.Location = new System.Drawing.Point(98, 463);
            this.checkBoxreload.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxreload.Name = "checkBoxreload";
            this.checkBoxreload.Size = new System.Drawing.Size(114, 21);
            this.checkBoxreload.TabIndex = 6;
            this.checkBoxreload.Text = "Reload every";
            this.checkBoxreload.UseVisualStyleBackColor = true;
            this.checkBoxreload.CheckedChanged += new System.EventHandler(this.checkboxReload_CheckedChanged);
            // 
            // reloadsecondstextbox
            // 
            this.reloadsecondstextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reloadsecondstextbox.Location = new System.Drawing.Point(209, 463);
            this.reloadsecondstextbox.Margin = new System.Windows.Forms.Padding(4);
            this.reloadsecondstextbox.Name = "reloadsecondstextbox";
            this.reloadsecondstextbox.Size = new System.Drawing.Size(31, 22);
            this.reloadsecondstextbox.TabIndex = 7;
            this.reloadsecondstextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.reloadsecondstextbox_KeyPress);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(372, 467);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 17);
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
            this.btnFullPowerUp.Location = new System.Drawing.Point(321, 494);
            this.btnFullPowerUp.Margin = new System.Windows.Forms.Padding(2);
            this.btnFullPowerUp.Name = "btnFullPowerUp";
            this.btnFullPowerUp.Size = new System.Drawing.Size(134, 28);
            this.btnFullPowerUp.TabIndex = 11;
            this.btnFullPowerUp.Text = "FULL-PowerUp";
            this.btnFullPowerUp.UseVisualStyleBackColor = true;
            this.btnFullPowerUp.Click += new System.EventHandler(this.btnFullPowerUp_Click);
            // 
            // lang_spain_btn2
            // 
            this.lang_spain_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.spain;
            this.lang_spain_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_spain_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_spain_btn2.Location = new System.Drawing.Point(97, 6);
            this.lang_spain_btn2.Margin = new System.Windows.Forms.Padding(4);
            this.lang_spain_btn2.Name = "lang_spain_btn2";
            this.lang_spain_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_spain_btn2.TabIndex = 15;
            this.lang_spain_btn2.UseVisualStyleBackColor = true;
            this.lang_spain_btn2.Click += new System.EventHandler(this.lang_spain_btn2_Click);
            // 
            // lang_de_btn_2
            // 
            this.lang_de_btn_2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.de;
            this.lang_de_btn_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_de_btn_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_de_btn_2.Location = new System.Drawing.Point(57, 6);
            this.lang_de_btn_2.Margin = new System.Windows.Forms.Padding(4);
            this.lang_de_btn_2.Name = "lang_de_btn_2";
            this.lang_de_btn_2.Size = new System.Drawing.Size(32, 18);
            this.lang_de_btn_2.TabIndex = 14;
            this.lang_de_btn_2.UseVisualStyleBackColor = true;
            this.lang_de_btn_2.Click += new System.EventHandler(this.lang_de_btn_2_Click);
            // 
            // lang_en_btn2
            // 
            this.lang_en_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.en;
            this.lang_en_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_en_btn2.Enabled = false;
            this.lang_en_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_en_btn2.Location = new System.Drawing.Point(17, 6);
            this.lang_en_btn2.Margin = new System.Windows.Forms.Padding(4);
            this.lang_en_btn2.Name = "lang_en_btn2";
            this.lang_en_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_en_btn2.TabIndex = 13;
            this.lang_en_btn2.UseVisualStyleBackColor = true;
            this.lang_en_btn2.Click += new System.EventHandler(this.lang_en_btn2_Click);
            // 
            // btnShowMap
            // 
            this.btnShowMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShowMap.Image = global::PokemonGo.RocketAPI.Console.Properties.Resources.map;
            this.btnShowMap.Location = new System.Drawing.Point(817, 457);
            this.btnShowMap.Margin = new System.Windows.Forms.Padding(4);
            this.btnShowMap.Name = "btnShowMap";
            this.btnShowMap.Size = new System.Drawing.Size(77, 64);
            this.btnShowMap.TabIndex = 12;
            this.btnShowMap.UseVisualStyleBackColor = true;
            this.btnShowMap.Click += new System.EventHandler(this.btnShowMap_Click);
            // 
            // lang_ptBR_btn2
            // 
            this.lang_ptBR_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.ptBR;
            this.lang_ptBR_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_ptBR_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_ptBR_btn2.Location = new System.Drawing.Point(137, 6);
            this.lang_ptBR_btn2.Margin = new System.Windows.Forms.Padding(4);
            this.lang_ptBR_btn2.Name = "lang_ptBR_btn2";
            this.lang_ptBR_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_ptBR_btn2.TabIndex = 42;
            this.lang_ptBR_btn2.UseVisualStyleBackColor = true;
            this.lang_ptBR_btn2.Click += new System.EventHandler(this.lang_ptBR_btn2_Click);
            // 
            // freezedenshit
            // 
            this.freezedenshit.Interval = 5000;
            this.freezedenshit.Tick += new System.EventHandler(this.freezedenshit_Tick);
            // 
            // lang_tr_btn2
            // 
            this.lang_tr_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.tr1;
            this.lang_tr_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_tr_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_tr_btn2.Location = new System.Drawing.Point(177, 6);
            this.lang_tr_btn2.Margin = new System.Windows.Forms.Padding(4);
            this.lang_tr_btn2.Name = "lang_tr_btn2";
            this.lang_tr_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_tr_btn2.TabIndex = 42;
            this.lang_tr_btn2.UseVisualStyleBackColor = true;
            this.lang_tr_btn2.Click += new System.EventHandler(this.lang_tr_btn2_Click);
            // 
            // btnForceUnban
            // 
            this.btnForceUnban.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnForceUnban.Location = new System.Drawing.Point(648, 459);
            this.btnForceUnban.Margin = new System.Windows.Forms.Padding(2);
            this.btnForceUnban.Name = "btnForceUnban";
            this.btnForceUnban.Size = new System.Drawing.Size(167, 28);
            this.btnForceUnban.TabIndex = 43;
            this.btnForceUnban.Text = "Pause Walking";
            this.btnForceUnban.UseVisualStyleBackColor = true;
            this.btnForceUnban.Click += new System.EventHandler(this.btnForceUnban_Click);
            // 
            // text_EMail
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(369, 461);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "text_EMail";
            this.textBox1.Size = new System.Drawing.Size(36, 22);
            this.textBox1.TabIndex = 44;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(243, 464);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "s    Power Up Limit";
            // 
            // checkBox_Start_Walk_from_default_location
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(697, 495);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox_Start_Walk_from_default_location";
            this.checkBox1.Size = new System.Drawing.Size(118, 21);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Repeat Route";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(578, 493);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 28);
            this.button1.TabIndex = 43;
            this.button1.Text = "Use Incense";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_SetLocation
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(538, 459);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button_SetLocation";
            this.button2.Size = new System.Drawing.Size(106, 28);
            this.button2.TabIndex = 43;
            this.button2.Text = "Use Lure";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(413, 459);
            this.button3.Margin = new System.Windows.Forms.Padding(2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 28);
            this.button3.TabIndex = 43;
            this.button3.Text = "Use Lucky Egg";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Pokemons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(910, 567);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnForceUnban);
            this.Controls.Add(this.lang_tr_btn2);
            this.Controls.Add(this.lang_ptBR_btn2);
            this.Controls.Add(this.lang_spain_btn2);
            this.Controls.Add(this.lang_de_btn_2);
            this.Controls.Add(this.lang_en_btn2);
            this.Controls.Add(this.btnShowMap);
            this.Controls.Add(this.btnFullPowerUp);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.reloadsecondstextbox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBoxreload);
            this.Controls.Add(this.statusTexbox);
            this.Controls.Add(this.btnTransfer);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.btnEvolve);
            this.Controls.Add(this.btnIVToNick);
            this.Controls.Add(this.btnreload);
            this.Controls.Add(this.PokemonListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(886, 603);
            this.Name = "Pokemons";
            this.Text = "Pokemon List";
            this.Load += new System.EventHandler(this.Pokemons_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private System.Windows.Forms.CheckBox checkBoxreload;
        private System.Windows.Forms.TextBox reloadsecondstextbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer reloadtimer;
        private System.Windows.Forms.Button btnFullPowerUp;
        private System.Windows.Forms.Button btnShowMap;
        private System.Windows.Forms.Button lang_en_btn2;
        private System.Windows.Forms.Button lang_de_btn_2;
        private System.Windows.Forms.Button lang_spain_btn2;
        private System.Windows.Forms.Button lang_ptBR_btn2;
        private System.Windows.Forms.Timer freezedenshit;
        private System.Windows.Forms.Button lang_tr_btn2;
        private System.Windows.Forms.Button btnForceUnban;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}