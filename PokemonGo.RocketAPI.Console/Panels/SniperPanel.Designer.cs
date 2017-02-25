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
        private System.Windows.Forms.CheckBox AvoidRegionLock;
        private System.Windows.Forms.Label label4;
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
        private System.Windows.Forms.Timer timerSnipe;
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
        private System.Windows.Forms.GroupBox gbLocations;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader cuURI;
        private System.Windows.Forms.ColumnHeader chIV;
        private System.Windows.Forms.ColumnHeader chProbability;
        private System.Windows.Forms.ColumnHeader chDate;
        private System.Windows.Forms.ColumnHeader chLastUpdate;
        private System.Windows.Forms.ColumnHeader chTillHidden;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer timerLocations;
        private System.Windows.Forms.ColumnHeader chId;
        private System.Windows.Forms.ColumnHeader chName;
        
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
            this.AvoidRegionLock = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
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
            this.timerSnipe = new System.Windows.Forms.Timer(this.components);
            this.nudSecondsSnipe = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.nudTriesSnipe = new System.Windows.Forms.NumericUpDown();
            this.checkBoxSnipeTransfer = new System.Windows.Forms.CheckBox();
            this.gbLocations = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.cuURI = new System.Windows.Forms.ColumnHeader();
            this.chIV = new System.Windows.Forms.ColumnHeader();
            this.chProbability = new System.Windows.Forms.ColumnHeader();
            this.chDate = new System.Windows.Forms.ColumnHeader();
            this.chLastUpdate = new System.Windows.Forms.ColumnHeader();
            this.chTillHidden = new System.Windows.Forms.ColumnHeader();
            this.timerLocations = new System.Windows.Forms.Timer(this.components);
            this.chId = new System.Windows.Forms.ColumnHeader();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsSnipe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriesSnipe)).BeginInit();
            this.gbLocations.SuspendLayout();
            this.SuspendLayout();
            // 
            // AvoidRegionLock
            // 
            this.AvoidRegionLock.AutoSize = true;
            this.AvoidRegionLock.Checked = true;
            this.AvoidRegionLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidRegionLock.Location = new System.Drawing.Point(459, 395);
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
            // SnipePokemonPokeCom
            // 
            this.SnipePokemonPokeCom.AutoSize = true;
            this.SnipePokemonPokeCom.Enabled = false;
            this.SnipePokemonPokeCom.Location = new System.Drawing.Point(459, 371);
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
            this.groupBox1.Location = new System.Drawing.Point(11, 2);
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
            this.groupBox2.Size = new System.Drawing.Size(726, 81);
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
            this.buttonGo.Location = new System.Drawing.Point(485, 52);
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
            this.comboBoxLinks.Location = new System.Drawing.Point(289, 54);
            this.comboBoxLinks.Name = "comboBoxLinks";
            this.comboBoxLinks.Size = new System.Drawing.Size(190, 21);
            this.comboBoxLinks.TabIndex = 94;
            // 
            // btnInstall
            // 
            this.btnInstall.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnInstall.Location = new System.Drawing.Point(611, 52);
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
            // timerSnipe
            // 
            this.timerSnipe.Interval = 10000;
            this.timerSnipe.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // nudSecondsSnipe
            // 
            this.nudSecondsSnipe.Location = new System.Drawing.Point(357, 357);
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
            this.label8.Location = new System.Drawing.Point(233, 359);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(117, 20);
            this.label8.TabIndex = 87;
            this.label8.Text = "Seconds to wait there:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(11, 361);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(149, 17);
            this.label9.TabIndex = 89;
            this.label9.Text = "Num Tries Finding Encounter:";
            // 
            // nudTriesSnipe
            // 
            this.nudTriesSnipe.Location = new System.Drawing.Point(167, 359);
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
            this.checkBoxSnipeTransfer.Location = new System.Drawing.Point(14, 393);
            this.checkBoxSnipeTransfer.Name = "checkBoxSnipeTransfer";
            this.checkBoxSnipeTransfer.Size = new System.Drawing.Size(192, 17);
            this.checkBoxSnipeTransfer.TabIndex = 97;
            this.checkBoxSnipeTransfer.Text = "Transfer directly at snipe succesful.";
            this.checkBoxSnipeTransfer.UseVisualStyleBackColor = true;
            // 
            // gbLocations
            // 
            this.gbLocations.Controls.Add(this.label3);
            this.gbLocations.Controls.Add(this.listView);
            this.gbLocations.Location = new System.Drawing.Point(416, 3);
            this.gbLocations.Name = "gbLocations";
            this.gbLocations.Size = new System.Drawing.Size(312, 339);
            this.gbLocations.TabIndex = 98;
            this.gbLocations.TabStop = false;
            this.gbLocations.Text = "Locations";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(7, 317);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 90;
            this.label3.Text = " Double click to Snipe";
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cuURI,
            this.chIV,
            this.chProbability,
            this.chDate,
            this.chLastUpdate,
            this.chId,
            this.chName,
            this.chTillHidden});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(7, 20);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(298, 288);
            this.listView.TabIndex = 81;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // cuURI
            // 
            this.cuURI.Text = "URI";
            this.cuURI.Width = 140;
            // 
            // chIV
            // 
            this.chIV.Text = "IV";
            this.chIV.Width = 40;
            // 
            // chProbability
            // 
            this.chProbability.Text = "Probability";
            // 
            // chDate
            // 
            this.chDate.Text = "Creation Date";
            // 
            // chLastUpdate
            // 
            this.chLastUpdate.Text = "Last Update";
            // 
            // chTillHidden
            // 
            this.chTillHidden.Text = "Till Hidden";
            // 
            // timerLocations
            // 
            this.timerLocations.Enabled = true;
            this.timerLocations.Interval = 10000;
            this.timerLocations.Tick += new System.EventHandler(this.timerLocations_Tick);
            // 
            // chId
            // 
            this.chId.Text = "Id";
            // 
            // chName
            // 
            this.chName.Text = "Name";
            // 
            // SniperPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.gbLocations);
            this.Controls.Add(this.checkBoxSnipeTransfer);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.nudTriesSnipe);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nudSecondsSnipe);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.AvoidRegionLock);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SnipePokemonPokeCom);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SniperPanel";
            this.Size = new System.Drawing.Size(734, 514);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSecondsSnipe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTriesSnipe)).EndInit();
            this.gbLocations.ResumeLayout(false);
            this.gbLocations.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
