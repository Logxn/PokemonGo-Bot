﻿/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 1:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class LocationPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public GMap.NET.WindowsForms.GMapControl map;
        private System.Windows.Forms.CheckBox cbShowPokemon;
        private System.Windows.Forms.CheckBox cbShowPokeStops;
        private System.Windows.Forms.Button buttonRefreshForts;
        public System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnGetPoints;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudRadius;
        private System.Windows.Forms.Button btnPauseWalking;
        private System.Windows.Forms.Button buttonZoomOut;
        private System.Windows.Forms.Button buttonZoomIn;
        private System.Windows.Forms.Button buttonLoadPokestops;
        private System.Windows.Forms.Button buttonSavePokestops;
        
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
            this.map = new GMap.NET.WindowsForms.GMapControl();
            this.cbShowPokemon = new System.Windows.Forms.CheckBox();
            this.cbShowPokeStops = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nudRadius = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonRefreshForts = new System.Windows.Forms.Button();
            this.btnGetPoints = new System.Windows.Forms.Button();
            this.lblAddress = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.btnPauseWalking = new System.Windows.Forms.Button();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonLoadPokestops = new System.Windows.Forms.Button();
            this.buttonSavePokestops = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // map
            // 
            this.map.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.EmptyTileColor = System.Drawing.Color.Navy;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.LevelsKeepInMemmory = 5;
            this.map.Location = new System.Drawing.Point(5, 5);
            this.map.Margin = new System.Windows.Forms.Padding(5);
            this.map.MarkersEnabled = true;
            this.map.MaxZoom = 2;
            this.map.MinZoom = 2;
            this.map.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.map.Name = "map";
            this.map.NegativeMode = false;
            this.map.PolygonsEnabled = true;
            this.map.RetryLoadTile = 0;
            this.map.RoutesEnabled = true;
            this.map.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.map.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.map.ShowTileGridLines = false;
            this.map.Size = new System.Drawing.Size(873, 501);
            this.map.TabIndex = 1;
            this.map.Zoom = 0D;
            this.map.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.map_OnMarkerClick);
            this.map.OnMapDrag += new GMap.NET.MapDrag(this.map_OnMapDrag);
            this.map.Load += new System.EventHandler(this.map_Load);
            // 
            // cbShowPokemon
            // 
            this.cbShowPokemon.AutoSize = true;
            this.cbShowPokemon.Checked = true;
            this.cbShowPokemon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokemon.Location = new System.Drawing.Point(140, 16);
            this.cbShowPokemon.Margin = new System.Windows.Forms.Padding(4);
            this.cbShowPokemon.Name = "cbShowPokemon";
            this.cbShowPokemon.Size = new System.Drawing.Size(101, 17);
            this.cbShowPokemon.TabIndex = 4;
            this.cbShowPokemon.Text = "Show Pokemon";
            this.cbShowPokemon.UseVisualStyleBackColor = true;
            this.cbShowPokemon.Visible = false;
            this.cbShowPokemon.CheckStateChanged += new System.EventHandler(this.cbShowPokemon_CheckStateChanged);
            // 
            // cbShowPokeStops
            // 
            this.cbShowPokeStops.AutoSize = true;
            this.cbShowPokeStops.Checked = true;
            this.cbShowPokeStops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokeStops.Location = new System.Drawing.Point(272, 16);
            this.cbShowPokeStops.Margin = new System.Windows.Forms.Padding(5);
            this.cbShowPokeStops.Name = "cbShowPokeStops";
            this.cbShowPokeStops.Size = new System.Drawing.Size(103, 17);
            this.cbShowPokeStops.TabIndex = 3;
            this.cbShowPokeStops.Text = "Show PokeStop";
            this.cbShowPokeStops.UseVisualStyleBackColor = true;
            this.cbShowPokeStops.Visible = false;
            this.cbShowPokeStops.CheckedChanged += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(705, 62);
            this.button1.Margin = new System.Windows.Forms.Padding(5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(104, 25);
            this.button1.TabIndex = 12;
            this.button1.Text = "Set Location";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(475, 66);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Altitude:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(232, 66);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Longitude:";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 66);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Latitude:";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox3.Enabled = false;
            this.textBox3.Location = new System.Drawing.Point(539, 65);
            this.textBox3.Margin = new System.Windows.Forms.Padding(5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(155, 20);
            this.textBox3.TabIndex = 11;
            this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.Location = new System.Drawing.Point(311, 65);
            this.textBox2.Margin = new System.Windows.Forms.Padding(5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(155, 20);
            this.textBox2.TabIndex = 10;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(69, 65);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(155, 20);
            this.textBox1.TabIndex = 9;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.nudRadius);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.buttonRefreshForts);
            this.panel1.Controls.Add(this.btnGetPoints);
            this.panel1.Controls.Add(this.lblAddress);
            this.panel1.Controls.Add(this.tbAddress);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(2, 419);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(829, 92);
            this.panel1.TabIndex = 2;
            // 
            // nudRadius
            // 
            this.nudRadius.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nudRadius.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRadius.Location = new System.Drawing.Point(596, 38);
            this.nudRadius.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.Size = new System.Drawing.Size(97, 20);
            this.nudRadius.TabIndex = 27;
            this.nudRadius.ValueChanged += new System.EventHandler(this.nudRadius_ValueChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(539, 40);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 26;
            this.label4.Text = "Radius:";
            // 
            // buttonRefreshForts
            // 
            this.buttonRefreshForts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefreshForts.AutoSize = true;
            this.buttonRefreshForts.Location = new System.Drawing.Point(14, 4);
            this.buttonRefreshForts.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefreshForts.Name = "buttonRefreshForts";
            this.buttonRefreshForts.Size = new System.Drawing.Size(172, 25);
            this.buttonRefreshForts.TabIndex = 6;
            this.buttonRefreshForts.Text = "Refresh Pokestops";
            this.buttonRefreshForts.UseVisualStyleBackColor = true;
            this.buttonRefreshForts.Visible = false;
            this.buttonRefreshForts.Click += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // btnGetPoints
            // 
            this.btnGetPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetPoints.Location = new System.Drawing.Point(383, 36);
            this.btnGetPoints.Margin = new System.Windows.Forms.Padding(5);
            this.btnGetPoints.Name = "btnGetPoints";
            this.btnGetPoints.Size = new System.Drawing.Size(92, 25);
            this.btnGetPoints.TabIndex = 8;
            this.btnGetPoints.Text = "Get Point";
            this.btnGetPoints.UseVisualStyleBackColor = true;
            this.btnGetPoints.Click += new System.EventHandler(this.BtnGetPointsClick);
            // 
            // lblAddress
            // 
            this.lblAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(5, 40);
            this.lblAddress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(48, 13);
            this.lblAddress.TabIndex = 24;
            this.lblAddress.Text = "Address:";
            // 
            // tbAddress
            // 
            this.tbAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAddress.Location = new System.Drawing.Point(69, 36);
            this.tbAddress.Margin = new System.Windows.Forms.Padding(5);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(292, 20);
            this.tbAddress.TabIndex = 7;
            // 
            // btnPauseWalking
            // 
            this.btnPauseWalking.Location = new System.Drawing.Point(16, 12);
            this.btnPauseWalking.Margin = new System.Windows.Forms.Padding(2);
            this.btnPauseWalking.Name = "btnPauseWalking";
            this.btnPauseWalking.Size = new System.Drawing.Size(106, 23);
            this.btnPauseWalking.TabIndex = 44;
            this.btnPauseWalking.Text = "Pause Walking";
            this.btnPauseWalking.UseVisualStyleBackColor = true;
            this.btnPauseWalking.Click += new System.EventHandler(this.btnPauseWalking_Click);
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZoomOut.Location = new System.Drawing.Point(853, 453);
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.Size = new System.Drawing.Size(25, 23);
            this.buttonZoomOut.TabIndex = 45;
            this.buttonZoomOut.Text = "+";
            this.buttonZoomOut.UseVisualStyleBackColor = true;
            this.buttonZoomOut.Click += new System.EventHandler(this.buttonZoomOut_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonZoomIn.Location = new System.Drawing.Point(853, 475);
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.Size = new System.Drawing.Size(25, 23);
            this.buttonZoomIn.TabIndex = 46;
            this.buttonZoomIn.Text = "-";
            this.buttonZoomIn.UseVisualStyleBackColor = true;
            this.buttonZoomIn.Click += new System.EventHandler(this.buttonZoomIn_Click);
            // 
            // buttonLoadPokestops
            // 
            this.buttonLoadPokestops.Location = new System.Drawing.Point(484, 12);
            this.buttonLoadPokestops.Name = "buttonLoadPokestops";
            this.buttonLoadPokestops.Size = new System.Drawing.Size(94, 23);
            this.buttonLoadPokestops.TabIndex = 48;
            this.buttonLoadPokestops.Text = "Load Pokestops";
            this.buttonLoadPokestops.UseVisualStyleBackColor = true;
            this.buttonLoadPokestops.Click += new System.EventHandler(this.buttonLoadPokestops_Click);
            // 
            // buttonSavePokestops
            // 
            this.buttonSavePokestops.Location = new System.Drawing.Point(385, 12);
            this.buttonSavePokestops.Name = "buttonSavePokestops";
            this.buttonSavePokestops.Size = new System.Drawing.Size(94, 23);
            this.buttonSavePokestops.TabIndex = 47;
            this.buttonSavePokestops.Text = "Save Pokestops";
            this.buttonSavePokestops.UseVisualStyleBackColor = true;
            this.buttonSavePokestops.Click += new System.EventHandler(this.buttonSavePokestops_Click);
            // 
            // LocationPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.buttonLoadPokestops);
            this.Controls.Add(this.buttonSavePokestops);
            this.Controls.Add(this.buttonZoomIn);
            this.Controls.Add(this.buttonZoomOut);
            this.Controls.Add(this.cbShowPokemon);
            this.Controls.Add(this.btnPauseWalking);
            this.Controls.Add(this.cbShowPokeStops);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.map);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LocationPanel";
            this.Size = new System.Drawing.Size(888, 514);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
