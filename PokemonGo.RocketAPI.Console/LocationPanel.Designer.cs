/*
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
        private System.Windows.Forms.Button buttonRefreshPokemon;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationPanel));
            this.map = new GMap.NET.WindowsForms.GMapControl();
            this.cbShowPokemon = new System.Windows.Forms.CheckBox();
            this.buttonRefreshPokemon = new System.Windows.Forms.Button();
            this.cbShowPokeStops = new System.Windows.Forms.CheckBox();
            this.buttonRefreshForts = new System.Windows.Forms.Button();
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
            this.btnGetPoints = new System.Windows.Forms.Button();
            this.lblAddress = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.btnPauseWalking = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).BeginInit();
            this.SuspendLayout();
            // 
            // map
            // 
            resources.ApplyResources(this.map, "map");
            this.map.Bearing = 0F;
            this.map.CanDragMap = true;
            this.map.EmptyTileColor = System.Drawing.Color.Navy;
            this.map.GrayScaleMode = false;
            this.map.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.map.LevelsKeepInMemmory = 5;
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
            this.map.Zoom = 0D;
            this.map.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.map_OnMarkerClick);
            this.map.OnMapDrag += new GMap.NET.MapDrag(this.map_OnMapDrag);
            this.map.Load += new System.EventHandler(this.map_Load);
            // 
            // cbShowPokemon
            // 
            resources.ApplyResources(this.cbShowPokemon, "cbShowPokemon");
            this.cbShowPokemon.Checked = true;
            this.cbShowPokemon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokemon.Name = "cbShowPokemon";
            this.cbShowPokemon.UseVisualStyleBackColor = true;
            this.cbShowPokemon.CheckStateChanged += new System.EventHandler(this.cbShowPokemon_CheckedChanged);
            // 
            // buttonRefreshPokemon
            // 
            resources.ApplyResources(this.buttonRefreshPokemon, "buttonRefreshPokemon");
            this.buttonRefreshPokemon.Name = "buttonRefreshPokemon";
            this.buttonRefreshPokemon.UseVisualStyleBackColor = true;
            this.buttonRefreshPokemon.Click += new System.EventHandler(this.cbShowPokemon_CheckedChanged);
            // 
            // cbShowPokeStops
            // 
            resources.ApplyResources(this.cbShowPokeStops, "cbShowPokeStops");
            this.cbShowPokeStops.Checked = true;
            this.cbShowPokeStops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokeStops.Name = "cbShowPokeStops";
            this.cbShowPokeStops.UseVisualStyleBackColor = true;
            this.cbShowPokeStops.CheckedChanged += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // buttonRefreshForts
            // 
            resources.ApplyResources(this.buttonRefreshForts, "buttonRefreshForts");
            this.buttonRefreshForts.Name = "buttonRefreshForts";
            this.buttonRefreshForts.UseVisualStyleBackColor = true;
            this.buttonRefreshForts.Click += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // textBox3
            // 
            resources.ApplyResources(this.textBox3, "textBox3");
            this.textBox3.Name = "textBox3";
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.Name = "textBox2";
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.nudRadius);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.cbShowPokeStops);
            this.panel1.Controls.Add(this.cbShowPokemon);
            this.panel1.Controls.Add(this.buttonRefreshPokemon);
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
            this.panel1.Name = "panel1";
            // 
            // nudRadius
            // 
            resources.ApplyResources(this.nudRadius, "nudRadius");
            this.nudRadius.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudRadius.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudRadius.Name = "nudRadius";
            this.nudRadius.ValueChanged += new System.EventHandler(this.nudRadius_ValueChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnGetPoints
            // 
            resources.ApplyResources(this.btnGetPoints, "btnGetPoints");
            this.btnGetPoints.Name = "btnGetPoints";
            this.btnGetPoints.UseVisualStyleBackColor = true;
            this.btnGetPoints.Click += new System.EventHandler(this.BtnGetPointsClick);
            // 
            // lblAddress
            // 
            resources.ApplyResources(this.lblAddress, "lblAddress");
            this.lblAddress.Name = "lblAddress";
            // 
            // tbAddress
            // 
            resources.ApplyResources(this.tbAddress, "tbAddress");
            this.tbAddress.Name = "tbAddress";
            // 
            // btnPauseWalking
            // 
            resources.ApplyResources(this.btnPauseWalking, "btnPauseWalking");
            this.btnPauseWalking.Name = "btnPauseWalking";
            this.btnPauseWalking.UseVisualStyleBackColor = true;
            this.btnPauseWalking.Click += new System.EventHandler(this.btnPauseWalking_Click);
            // 
            // LocationPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.btnPauseWalking);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.map);
            resources.ApplyResources(this, "$this");
            this.Name = "LocationPanel";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRadius)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
