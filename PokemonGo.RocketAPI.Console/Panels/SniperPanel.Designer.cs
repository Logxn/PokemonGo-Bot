/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 3:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokeMaster
{
    partial class SniperPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Timer timerSnipe;
        private System.Windows.Forms.ComboBox comboBoxLinks;
        private System.Windows.Forms.Button buttonGo;
        public System.Windows.Forms.CheckBox checkBoxExternalWeb;
        private System.Windows.Forms.GroupBox gbLocations;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader cuURI;
        private System.Windows.Forms.ColumnHeader chIV;
        private System.Windows.Forms.ColumnHeader chProbability;
        private System.Windows.Forms.ColumnHeader chDate;
        private System.Windows.Forms.ColumnHeader chLastUpdate;
        private System.Windows.Forms.ColumnHeader chTillHidden;
        private System.Windows.Forms.Timer timerLocations;
        private System.Windows.Forms.ColumnHeader chId;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.Timer timerAutosnipe;
        private System.Windows.Forms.ColumnHeader chUsed;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem markAsUsedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem snipeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.NumericUpDown numAutoImport;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Timer timerAutoImport;
        private System.Windows.Forms.TextBox textBoxPokemonsList;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label LabelLocation;
        private System.Windows.Forms.TextBox textBoxLocation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SniperPanel));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxExternalWeb = new System.Windows.Forms.CheckBox();
            this.buttonGo = new System.Windows.Forms.Button();
            this.comboBoxLinks = new System.Windows.Forms.ComboBox();
            this.timerSnipe = new System.Windows.Forms.Timer(this.components);
            this.gbLocations = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.textBoxPokemonsList = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.listView = new System.Windows.Forms.ListView();
            this.cuURI = new System.Windows.Forms.ColumnHeader();
            this.chIV = new System.Windows.Forms.ColumnHeader();
            this.chProbability = new System.Windows.Forms.ColumnHeader();
            this.chDate = new System.Windows.Forms.ColumnHeader();
            this.chLastUpdate = new System.Windows.Forms.ColumnHeader();
            this.chId = new System.Windows.Forms.ColumnHeader();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chTillHidden = new System.Windows.Forms.ColumnHeader();
            this.chUsed = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.snipeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markAsUsedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.numAutoImport = new System.Windows.Forms.NumericUpDown();
            this.timerLocations = new System.Windows.Forms.Timer(this.components);
            this.timerAutosnipe = new System.Windows.Forms.Timer(this.components);
            this.timerAutoImport = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LabelLocation = new System.Windows.Forms.Label();
            this.textBoxLocation = new System.Windows.Forms.TextBox();
            this.groupBox2.SuspendLayout();
            this.gbLocations.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoImport)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.checkBoxExternalWeb);
            this.groupBox2.Controls.Add(this.buttonGo);
            this.groupBox2.Controls.Add(this.comboBoxLinks);
            this.groupBox2.ForeColor = System.Drawing.Color.DarkRed;
            this.groupBox2.Location = new System.Drawing.Point(4, 449);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(745, 53);
            this.groupBox2.TabIndex = 85;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "External webs with pokemon locations";
            // 
            // checkBoxExternalWeb
            // 
            this.checkBoxExternalWeb.ForeColor = System.Drawing.SystemColors.ControlText;
            this.checkBoxExternalWeb.Location = new System.Drawing.Point(6, 19);
            this.checkBoxExternalWeb.Name = "checkBoxExternalWeb";
            this.checkBoxExternalWeb.Size = new System.Drawing.Size(169, 20);
            this.checkBoxExternalWeb.TabIndex = 96;
            this.checkBoxExternalWeb.Text = "Open In External Browser";
            this.checkBoxExternalWeb.UseVisualStyleBackColor = true;
            // 
            // buttonGo
            // 
            this.buttonGo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonGo.Location = new System.Drawing.Point(657, 16);
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
            this.comboBoxLinks.Location = new System.Drawing.Point(160, 18);
            this.comboBoxLinks.Name = "comboBoxLinks";
            this.comboBoxLinks.Size = new System.Drawing.Size(480, 21);
            this.comboBoxLinks.TabIndex = 94;
            // 
            // timerSnipe
            // 
            this.timerSnipe.Interval = 10000;
            this.timerSnipe.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // gbLocations
            // 
            this.gbLocations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbLocations.Controls.Add(this.checkBox2);
            this.gbLocations.Controls.Add(this.textBoxPokemonsList);
            this.gbLocations.Controls.Add(this.label7);
            this.gbLocations.Controls.Add(this.checkBox1);
            this.gbLocations.Controls.Add(this.listView);
            this.gbLocations.Controls.Add(this.numAutoImport);
            this.gbLocations.Location = new System.Drawing.Point(416, 3);
            this.gbLocations.Name = "gbLocations";
            this.gbLocations.Size = new System.Drawing.Size(333, 439);
            this.gbLocations.TabIndex = 98;
            this.gbLocations.TabStop = false;
            this.gbLocations.Text = "Locations";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(9, 72);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(158, 17);
            this.checkBox2.TabIndex = 106;
            this.checkBox2.Text = "Intercept Discord Messages";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // textBoxPokemonsList
            // 
            this.textBoxPokemonsList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPokemonsList.Enabled = false;
            this.textBoxPokemonsList.Location = new System.Drawing.Point(9, 46);
            this.textBoxPokemonsList.Name = "textBoxPokemonsList";
            this.textBoxPokemonsList.Size = new System.Drawing.Size(317, 20);
            this.textBoxPokemonsList.TabIndex = 105;
            this.textBoxPokemonsList.Text = "http://www.mypogosnipers.com/data/cache/free.txt";
            this.textBoxPokemonsList.Leave += new System.EventHandler(this.textBoxPokemonsList_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(272, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 13);
            this.label7.TabIndex = 104;
            this.label7.Text = "Minutes";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label7.DoubleClick += new System.EventHandler(this.label7_DoubleClick);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(9, 20);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(4);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(171, 17);
            this.checkBox1.TabIndex = 102;
            this.checkBox1.Text = "Enable Automatic Import Every";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
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
            this.chTillHidden,
            this.chUsed});
            this.listView.ContextMenuStrip = this.contextMenuStrip1;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(8, 93);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(318, 339);
            this.listView.TabIndex = 81;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
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
            // chId
            // 
            this.chId.Text = "Id";
            // 
            // chName
            // 
            this.chName.Text = "Name";
            // 
            // chTillHidden
            // 
            this.chTillHidden.Text = "Till Hidden";
            // 
            // chUsed
            // 
            this.chUsed.Text = "Used";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.snipeToolStripMenuItem,
            this.markAsUsedToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.deleteAllToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(191, 92);
            // 
            // snipeToolStripMenuItem
            // 
            this.snipeToolStripMenuItem.Name = "snipeToolStripMenuItem";
            this.snipeToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.snipeToolStripMenuItem.Text = "Snipe";
            this.snipeToolStripMenuItem.Click += new System.EventHandler(this.snipeToolStripMenuItem_Click);
            // 
            // markAsUsedToolStripMenuItem
            // 
            this.markAsUsedToolStripMenuItem.Name = "markAsUsedToolStripMenuItem";
            this.markAsUsedToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.markAsUsedToolStripMenuItem.Text = "Mark/Unmark as used";
            this.markAsUsedToolStripMenuItem.Click += new System.EventHandler(this.markAsUsedToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.deleteToolStripMenuItem.Text = "Delete Seleted";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // deleteAllToolStripMenuItem
            // 
            this.deleteAllToolStripMenuItem.Name = "deleteAllToolStripMenuItem";
            this.deleteAllToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.deleteAllToolStripMenuItem.Text = "Delete All";
            this.deleteAllToolStripMenuItem.Click += new System.EventHandler(this.deleteAllToolStripMenuItem_Click);
            // 
            // numAutoImport
            // 
            this.numAutoImport.Location = new System.Drawing.Point(220, 17);
            this.numAutoImport.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numAutoImport.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAutoImport.Name = "numAutoImport";
            this.numAutoImport.Size = new System.Drawing.Size(46, 20);
            this.numAutoImport.TabIndex = 103;
            this.numAutoImport.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numAutoImport.ValueChanged += new System.EventHandler(this.numAutoImport_ValueChanged);
            // 
            // timerLocations
            // 
            this.timerLocations.Enabled = true;
            this.timerLocations.Interval = 10000;
            this.timerLocations.Tick += new System.EventHandler(this.timerLocations_Tick);
            // 
            // timerAutosnipe
            // 
            this.timerAutosnipe.Interval = 180000;
            this.timerAutosnipe.Tick += new System.EventHandler(this.timerAutosnipe_Tick);
            // 
            // timerAutoImport
            // 
            this.timerAutoImport.Interval = 600000;
            this.timerAutoImport.Tick += new System.EventHandler(this.timerAutoImport_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labelTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LabelLocation);
            this.groupBox1.Controls.Add(this.textBoxLocation);
            this.groupBox1.Location = new System.Drawing.Point(10, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 276);
            this.groupBox1.TabIndex = 99;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Snipe";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(26, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(345, 96);
            this.label4.TabIndex = 6;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // labelTime
            // 
            this.labelTime.AutoSize = true;
            this.labelTime.Location = new System.Drawing.Point(178, 93);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(25, 13);
            this.labelTime.TabIndex = 5;
            this.labelTime.Text = "120";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Maroon;
            this.label3.Location = new System.Drawing.Point(108, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(181, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "(Latitude, Longitude)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(275, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Minutes";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Minimal Time to wait:";
            // 
            // LabelLocation
            // 
            this.LabelLocation.AutoSize = true;
            this.LabelLocation.Location = new System.Drawing.Point(17, 29);
            this.LabelLocation.Name = "LabelLocation";
            this.LabelLocation.Size = new System.Drawing.Size(51, 13);
            this.LabelLocation.TabIndex = 1;
            this.LabelLocation.Text = "Location:";
            // 
            // textBoxLocation
            // 
            this.textBoxLocation.Location = new System.Drawing.Point(17, 56);
            this.textBoxLocation.Name = "textBoxLocation";
            this.textBoxLocation.Size = new System.Drawing.Size(363, 20);
            this.textBoxLocation.TabIndex = 0;
            this.textBoxLocation.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // SniperPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbLocations);
            this.Controls.Add(this.groupBox2);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SniperPanel";
            this.Size = new System.Drawing.Size(759, 517);
            this.Load += new System.EventHandler(this.SniperPanel_Load);
            this.groupBox2.ResumeLayout(false);
            this.gbLocations.ResumeLayout(false);
            this.gbLocations.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numAutoImport)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
