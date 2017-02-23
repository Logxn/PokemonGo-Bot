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
        private System.Windows.Forms.NumericUpDown nudSecondsSnipe;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudTriesSnipe;
        private System.Windows.Forms.ComboBox comboBoxLinks;
        private System.Windows.Forms.Button buttonGo;
        public System.Windows.Forms.CheckBox checkBoxExternalWeb;
        public System.Windows.Forms.CheckBox checkBoxSnipeTransfer;
        private System.Windows.Forms.CheckBox checkBoxSnipeGym;
        
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
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.UpdateNotToSnipe = new System.Windows.Forms.Button();
            this.SelectallNottoSnipe = new System.Windows.Forms.CheckBox();
            this.checkedListBox_NotToSnipe = new System.Windows.Forms.CheckedListBox();
            this.SnipePokemonPokeCom = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxSnipeGym = new System.Windows.Forms.CheckBox();
            this.PokemonImage = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SnipeMe = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.SnipeInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxExternalWeb = new System.Windows.Forms.CheckBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.comboBoxLinks = new System.Windows.Forms.ComboBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.nudSecondsSnipe = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.nudTriesSnipe = new System.Windows.Forms.NumericUpDown();
            this.checkBoxSnipeTransfer = new System.Windows.Forms.CheckBox();
            this.groupBox23.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsSnipe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriesSnipe)).BeginInit();
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
            this.label4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DarkRed;
            this.label4.Location = new System.Drawing.Point(127, 242);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(251, 21);
            this.label4.TabIndex = 81;
            this.label4.Text = "(Format: 30.123456, -97.123456 )";
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
            this.SnipePokemonPokeCom.Enabled = false;
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
            this.groupBox1.Controls.Add(this.checkBoxSnipeGym);
            this.groupBox1.Controls.Add(this.PokemonImage);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.SnipeMe);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label64);
            this.groupBox1.Controls.Add(this.SnipeInfo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(255, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(392, 340);
            this.groupBox1.TabIndex = 84;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual Sniping ";
            // 
            // checkBoxSnipeGym
            // 
            this.checkBoxSnipeGym.Location = new System.Drawing.Point(25, 165);
            this.checkBoxSnipeGym.Name = "checkBoxSnipeGym";
            this.checkBoxSnipeGym.Size = new System.Drawing.Size(104, 24);
            this.checkBoxSnipeGym.TabIndex = 82;
            this.checkBoxSnipeGym.Text = "Gym";
            this.checkBoxSnipeGym.UseVisualStyleBackColor = true;
            this.checkBoxSnipeGym.CheckedChanged += new System.EventHandler(this.checkBoxSnipeGym_CheckedChanged);
            // 
            // PokemonImage
            // 
            this.PokemonImage.Location = new System.Drawing.Point(135, 17);
            this.PokemonImage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.PokemonImage.Name = "PokemonImage";
            this.PokemonImage.Size = new System.Drawing.Size(197, 172);
            this.PokemonImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PokemonImage.TabIndex = 76;
            this.PokemonImage.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(135, 208);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(199, 21);
            this.comboBox1.TabIndex = 75;
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
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
            this.label6.Location = new System.Drawing.Point(21, 211);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 72;
            this.label6.Text = "Pokemon Name:";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(21, 249);
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
            this.groupBox2.Controls.Add(this.checkBoxExternalWeb);
            this.groupBox2.Controls.Add(this.buttonGo);
            this.groupBox2.Controls.Add(this.comboBoxLinks);
            this.groupBox2.Controls.Add(this.btnInstall);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.ForeColor = System.Drawing.Color.DarkRed;
            this.groupBox2.Location = new System.Drawing.Point(4, 418);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(650, 81);
            this.groupBox2.TabIndex = 85;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "URI Service";
            // 
            // checkBoxExternalWeb
            // 
            this.checkBoxExternalWeb.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxExternalWeb.Location = new System.Drawing.Point(11, 55);
            this.checkBoxExternalWeb.Name = "checkBoxExternalWeb";
            this.checkBoxExternalWeb.Size = new System.Drawing.Size(169, 20);
            this.checkBoxExternalWeb.TabIndex = 96;
            this.checkBoxExternalWeb.Text = "Open In External Browser";
            this.checkBoxExternalWeb.UseVisualStyleBackColor = true;
            // 
            // buttonGo
            // 
            this.buttonGo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonGo.Location = new System.Drawing.Point(413, 52);
            this.buttonGo.Name = "buttonGo";
            this.buttonGo.Size = new System.Drawing.Size(54, 23);
            this.buttonGo.TabIndex = 95;
            this.buttonGo.Text = "Go";
            this.buttonGo.UseVisualStyleBackColor = true;
            this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);
            // 
            // comboBoxLinks
            // 
            this.comboBoxLinks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLinks.FormattingEnabled = true;
            this.comboBoxLinks.Location = new System.Drawing.Point(217, 54);
            this.comboBoxLinks.Name = "comboBoxLinks";
            this.comboBoxLinks.Size = new System.Drawing.Size(190, 21);
            this.comboBoxLinks.TabIndex = 94;
            // 
            // btnInstall
            // 
            this.btnInstall.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnInstall.Location = new System.Drawing.Point(539, 52);
            this.btnInstall.Margin = new System.Windows.Forms.Padding(4);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(103, 22);
            this.btnInstall.TabIndex = 5;
            this.btnInstall.Text = "Install Service";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(7, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(616, 13);
            this.label1.TabIndex = 86;
            this.label1.Text = "Handles \"pokesniper2:// and msniper://\" URI Protocols. So if you have another app" +
    "lication to do it.  Is  advisable uninstall before";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(7, 35);
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
            // nudSecondsSnipe
            // 
            this.nudSecondsSnipe.Location = new System.Drawing.Point(603, 360);
            this.nudSecondsSnipe.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.nudSecondsSnipe.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudSecondsSnipe.Name = "nudSecondsSnipe";
            this.nudSecondsSnipe.Size = new System.Drawing.Size(46, 20);
            this.nudSecondsSnipe.TabIndex = 86;
            this.nudSecondsSnipe.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudSecondsSnipe.ValueChanged += new System.EventHandler(this.nudSecondsSnipe_ValueChanged);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(477, 361);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 20);
            this.label8.TabIndex = 87;
            this.label8.Text = "Seconds to wait there:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(255, 363);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(149, 17);
            this.label9.TabIndex = 89;
            this.label9.Text = "Num Tries Finding Encounter:";
            // 
            // nudTriesSnipe
            // 
            this.nudTriesSnipe.Location = new System.Drawing.Point(411, 361);
            this.nudTriesSnipe.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudTriesSnipe.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudTriesSnipe.Name = "nudTriesSnipe";
            this.nudTriesSnipe.Size = new System.Drawing.Size(46, 20);
            this.nudTriesSnipe.TabIndex = 88;
            this.nudTriesSnipe.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudTriesSnipe.ValueChanged += new System.EventHandler(this.nudTriesSnipe_ValueChanged);
            // 
            // checkBoxSnipeTransfer
            // 
            this.checkBoxSnipeTransfer.AutoSize = true;
            this.checkBoxSnipeTransfer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxSnipeTransfer.Location = new System.Drawing.Point(258, 395);
            this.checkBoxSnipeTransfer.Name = "checkBoxSnipeTransfer";
            this.checkBoxSnipeTransfer.Size = new System.Drawing.Size(192, 17);
            this.checkBoxSnipeTransfer.TabIndex = 97;
            this.checkBoxSnipeTransfer.Text = "Transfer directly at snipe succesful.";
            this.checkBoxSnipeTransfer.UseVisualStyleBackColor = true;
            // 
            // SniperPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.checkBoxSnipeTransfer);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.nudTriesSnipe);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudSecondsSnipe);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ForceAutoSnipe);
            this.Controls.Add(this.AvoidRegionLock);
            this.Controls.Add(this.groupBox23);
            this.Controls.Add(this.SnipePokemonPokeCom);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SniperPanel";
            this.Size = new System.Drawing.Size(685, 514);
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsSnipe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriesSnipe)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
