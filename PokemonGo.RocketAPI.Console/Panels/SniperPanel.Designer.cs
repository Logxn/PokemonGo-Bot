/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 3:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class SniperPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button ForceAutoSnipe;
        private System.Windows.Forms.CheckBox AvoidRegionLock;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button UpdateNotToSnipe;
        private System.Windows.Forms.CheckBox SelectallNottoSnipe;
        private System.Windows.Forms.CheckedListBox checkedListBox_NotToSnipe;
        private System.Windows.Forms.CheckBox SnipePokemonPokeCom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox PokemonImage;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button SnipeMe;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox SnipeInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel LinkPokesniperCom;
        private System.Windows.Forms.LinkLabel linkRarespawns;
        private System.Windows.Forms.LinkLabel linkPokezz;
        
        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ForceAutoSnipe = new System.Windows.Forms.Button();
            this.AvoidRegionLock = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.UpdateNotToSnipe = new System.Windows.Forms.Button();
            this.SelectallNottoSnipe = new System.Windows.Forms.CheckBox();
            this.checkedListBox_NotToSnipe = new System.Windows.Forms.CheckedListBox();
            this.SnipePokemonPokeCom = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PokemonImage = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SnipeMe = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.SnipeInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkPokezz = new System.Windows.Forms.LinkLabel();
            this.linkRarespawns = new System.Windows.Forms.LinkLabel();
            this.LinkPokesniperCom = new System.Windows.Forms.LinkLabel();
            this.btnInstall = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox23.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ForceAutoSnipe
            // 
            this.ForceAutoSnipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForceAutoSnipe.Location = new System.Drawing.Point(12, 334);
            this.ForceAutoSnipe.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ForceAutoSnipe.Name = "ForceAutoSnipe";
            this.ForceAutoSnipe.Size = new System.Drawing.Size(212, 28);
            this.ForceAutoSnipe.TabIndex = 77;
            this.ForceAutoSnipe.Text = "Start Auto Snipe";
            this.ForceAutoSnipe.UseVisualStyleBackColor = true;
            this.ForceAutoSnipe.Click += new System.EventHandler(this.ForceAutoSnipe_Click);
            // 
            // AvoidRegionLock
            // 
            this.AvoidRegionLock.AutoSize = true;
            this.AvoidRegionLock.Checked = true;
            this.AvoidRegionLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidRegionLock.Location = new System.Drawing.Point(4, 393);
            this.AvoidRegionLock.Margin = new System.Windows.Forms.Padding(4);
            this.AvoidRegionLock.Name = "AvoidRegionLock";
            this.AvoidRegionLock.Size = new System.Drawing.Size(177, 17);
            this.AvoidRegionLock.TabIndex = 78;
            this.AvoidRegionLock.Text = "Avoid Region Locked Pokemon";
            this.AvoidRegionLock.UseVisualStyleBackColor = true;
            this.AvoidRegionLock.CheckedChanged += new System.EventHandler(this.AvoidRegionLock_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(345, 369);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(174, 20);
            this.label4.TabIndex = 81;
            this.label4.Text = "30.123456, -97.123456";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(359, 398);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(143, 13);
            this.label5.TabIndex = 82;
            this.label5.Text = "please use decimals for now.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(292, 345);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(237, 13);
            this.label3.TabIndex = 83;
            this.label3.Text = "You must enter Snipe Info in the following format!";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.UpdateNotToSnipe);
            this.groupBox23.Controls.Add(this.SelectallNottoSnipe);
            this.groupBox23.Controls.Add(this.checkedListBox_NotToSnipe);
            this.groupBox23.Location = new System.Drawing.Point(4, 4);
            this.groupBox23.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox23.Size = new System.Drawing.Size(228, 324);
            this.groupBox23.TabIndex = 79;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Pokemon - Not to Snipe";
            // 
            // UpdateNotToSnipe
            // 
            this.UpdateNotToSnipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.UpdateNotToSnipe.Location = new System.Drawing.Point(108, 294);
            this.UpdateNotToSnipe.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.UpdateNotToSnipe.Name = "UpdateNotToSnipe";
            this.UpdateNotToSnipe.Size = new System.Drawing.Size(113, 22);
            this.UpdateNotToSnipe.TabIndex = 33;
            this.UpdateNotToSnipe.Text = "Update";
            this.UpdateNotToSnipe.UseVisualStyleBackColor = true;
            this.UpdateNotToSnipe.Click += new System.EventHandler(this.UpdateNotToSnipe_Click);
            // 
            // SelectallNottoSnipe
            // 
            this.SelectallNottoSnipe.AutoSize = true;
            this.SelectallNottoSnipe.Location = new System.Drawing.Point(8, 295);
            this.SelectallNottoSnipe.Margin = new System.Windows.Forms.Padding(4);
            this.SelectallNottoSnipe.Name = "SelectallNottoSnipe";
            this.SelectallNottoSnipe.Size = new System.Drawing.Size(69, 17);
            this.SelectallNottoSnipe.TabIndex = 32;
            this.SelectallNottoSnipe.Text = "Select all";
            this.SelectallNottoSnipe.UseVisualStyleBackColor = true;
            this.SelectallNottoSnipe.CheckedChanged += new System.EventHandler(this.SelectallNottoSnipe_CheckedChanged);
            // 
            // checkedListBox_NotToSnipe
            // 
            this.checkedListBox_NotToSnipe.CheckOnClick = true;
            this.checkedListBox_NotToSnipe.FormattingEnabled = true;
            this.checkedListBox_NotToSnipe.Location = new System.Drawing.Point(8, 25);
            this.checkedListBox_NotToSnipe.Margin = new System.Windows.Forms.Padding(4);
            this.checkedListBox_NotToSnipe.Name = "checkedListBox_NotToSnipe";
            this.checkedListBox_NotToSnipe.ScrollAlwaysVisible = true;
            this.checkedListBox_NotToSnipe.Size = new System.Drawing.Size(212, 229);
            this.checkedListBox_NotToSnipe.TabIndex = 0;
            // 
            // SnipePokemonPokeCom
            // 
            this.SnipePokemonPokeCom.AutoSize = true;
            this.SnipePokemonPokeCom.Location = new System.Drawing.Point(4, 369);
            this.SnipePokemonPokeCom.Margin = new System.Windows.Forms.Padding(4);
            this.SnipePokemonPokeCom.Name = "SnipePokemonPokeCom";
            this.SnipePokemonPokeCom.Size = new System.Drawing.Size(195, 17);
            this.SnipePokemonPokeCom.TabIndex = 80;
            this.SnipePokemonPokeCom.Text = "Enable Automatic Pokemon Sniping";
            this.SnipePokemonPokeCom.UseVisualStyleBackColor = true;
            this.SnipePokemonPokeCom.CheckedChanged += new System.EventHandler(this.SnipePokemonPokeCom_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PokemonImage);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.SnipeMe);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label64);
            this.groupBox1.Controls.Add(this.SnipeInfo);
            this.groupBox1.Location = new System.Drawing.Point(268, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(391, 340);
            this.groupBox1.TabIndex = 84;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual Sniping ";
            // 
            // PokemonImage
            // 
            this.PokemonImage.Location = new System.Drawing.Point(96, 25);
            this.PokemonImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PokemonImage.Name = "PokemonImage";
            this.PokemonImage.Size = new System.Drawing.Size(197, 172);
            this.PokemonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PokemonImage.TabIndex = 76;
            this.PokemonImage.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(95, 219);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(199, 21);
            this.comboBox1.TabIndex = 75;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // SnipeMe
            // 
            this.SnipeMe.BackColor = System.Drawing.Color.MediumAquamarine;
            this.SnipeMe.Enabled = false;
            this.SnipeMe.Location = new System.Drawing.Point(21, 298);
            this.SnipeMe.Margin = new System.Windows.Forms.Padding(4);
            this.SnipeMe.Name = "SnipeMe";
            this.SnipeMe.Size = new System.Drawing.Size(345, 27);
            this.SnipeMe.TabIndex = 74;
            this.SnipeMe.Text = "Snipe Me!";
            this.SnipeMe.UseVisualStyleBackColor = false;
            this.SnipeMe.Click += new System.EventHandler(this.SnipeMe_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(136, 199);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 72;
            this.label6.Text = "Pokemon Name";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(125, 247);
            this.label64.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(98, 13);
            this.label64.TabIndex = 72;
            this.label64.Text = "Latitude, Longitude";
            // 
            // SnipeInfo
            // 
            this.SnipeInfo.Location = new System.Drawing.Point(21, 268);
            this.SnipeInfo.Margin = new System.Windows.Forms.Padding(4);
            this.SnipeInfo.Name = "SnipeInfo";
            this.SnipeInfo.Size = new System.Drawing.Size(345, 20);
            this.SnipeInfo.TabIndex = 73;
            this.SnipeInfo.TextChanged += new System.EventHandler(this.SnipeInfo_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkPokezz);
            this.groupBox2.Controls.Add(this.linkRarespawns);
            this.groupBox2.Controls.Add(this.LinkPokesniperCom);
            this.groupBox2.Controls.Add(this.btnInstall);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(10, 420);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(600, 81);
            this.groupBox2.TabIndex = 85;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "URI Service";
            // 
            // linkPokezz
            // 
            this.linkPokezz.Location = new System.Drawing.Point(276, 58);
            this.linkPokezz.Name = "linkPokezz";
            this.linkPokezz.Size = new System.Drawing.Size(67, 15);
            this.linkPokezz.TabIndex = 91;
            this.linkPokezz.TabStop = true;
            this.linkPokezz.Text = "pokezz.com";
            this.linkPokezz.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // linkRarespawns
            // 
            this.linkRarespawns.Location = new System.Drawing.Point(160, 58);
            this.linkRarespawns.Name = "linkRarespawns";
            this.linkRarespawns.Size = new System.Drawing.Size(110, 15);
            this.linkRarespawns.TabIndex = 90;
            this.linkRarespawns.TabStop = true;
            this.linkRarespawns.Text = "www.rarespawns.be";
            this.linkRarespawns.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // LinkPokesniperCom
            // 
            this.LinkPokesniperCom.Location = new System.Drawing.Point(54, 58);
            this.LinkPokesniperCom.Name = "LinkPokesniperCom";
            this.LinkPokesniperCom.Size = new System.Drawing.Size(100, 15);
            this.LinkPokesniperCom.TabIndex = 89;
            this.LinkPokesniperCom.TabStop = true;
            this.LinkPokesniperCom.Text = "Pokesniper.com";
            this.LinkPokesniperCom.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(491, 53);
            this.btnInstall.Margin = new System.Windows.Forms.Padding(4);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(103, 22);
            this.btnInstall.TabIndex = 5;
            this.btnInstall.Text = "Install Service";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label7.Location = new System.Drawing.Point(8, 15);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 88;
            this.label7.Text = "NOTES:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(538, 13);
            this.label1.TabIndex = 86;
            this.label1.Text = "Handles \"pokesniper2://\" URI Protocol. So if you have another application to do i" +
    "t. Is  advisable uninstall before.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(403, 13);
            this.label2.TabIndex = 87;
            this.label2.Text = "With Service Installed you can snipe directly from pokesniper URIs like these pag" +
    "es:";
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SniperPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ForceAutoSnipe);
            this.Controls.Add(this.AvoidRegionLock);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox23);
            this.Controls.Add(this.SnipePokemonPokeCom);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SniperPanel";
            this.Size = new System.Drawing.Size(685, 505);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
