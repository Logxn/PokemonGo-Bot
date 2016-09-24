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
		private GMap.NET.WindowsForms.GMapControl map;
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
            this.btnGetPoints = new System.Windows.Forms.Button();
            this.lblAddress = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
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
            this.map.Size = new System.Drawing.Size(871, 501);
            this.map.TabIndex = 1;
            this.map.Zoom = 0D;
            this.map.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.map_OnMarkerClick);
            this.map.OnMapDrag += new GMap.NET.MapDrag(this.map_OnMapDrag);
            this.map.Load += new System.EventHandler(this.map_Load);
            // 
            // cbShowPokemon
            // 
            this.cbShowPokemon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowPokemon.AutoSize = true;
            this.cbShowPokemon.Checked = true;
            this.cbShowPokemon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokemon.Location = new System.Drawing.Point(141, 13);
            this.cbShowPokemon.Margin = new System.Windows.Forms.Padding(4);
            this.cbShowPokemon.Name = "cbShowPokemon";
            this.cbShowPokemon.Size = new System.Drawing.Size(127, 21);
            this.cbShowPokemon.TabIndex = 22;
            this.cbShowPokemon.Text = "Show Pokemon";
            this.cbShowPokemon.UseVisualStyleBackColor = true;
            this.cbShowPokemon.Visible = false;
            this.cbShowPokemon.CheckStateChanged += new System.EventHandler(this.cbShowPokemon_CheckedChanged);
            // 
            // buttonRefreshPokemon
            // 
            this.buttonRefreshPokemon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefreshPokemon.AutoSize = true;
            this.buttonRefreshPokemon.Location = new System.Drawing.Point(311, 2);
            this.buttonRefreshPokemon.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefreshPokemon.Name = "buttonRefreshPokemon";
            this.buttonRefreshPokemon.Size = new System.Drawing.Size(164, 34);
            this.buttonRefreshPokemon.TabIndex = 20;
            this.buttonRefreshPokemon.Text = "Refresh Pokemon";
            this.buttonRefreshPokemon.UseVisualStyleBackColor = true;
            this.buttonRefreshPokemon.Visible = false;
            this.buttonRefreshPokemon.Click += new System.EventHandler(this.cbShowPokemon_CheckedChanged);
            // 
            // cbShowPokeStops
            // 
            this.cbShowPokeStops.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowPokeStops.AutoSize = true;
            this.cbShowPokeStops.Checked = true;
            this.cbShowPokeStops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokeStops.Location = new System.Drawing.Point(5, 13);
            this.cbShowPokeStops.Margin = new System.Windows.Forms.Padding(5);
            this.cbShowPokeStops.Name = "cbShowPokeStops";
            this.cbShowPokeStops.Size = new System.Drawing.Size(129, 21);
            this.cbShowPokeStops.TabIndex = 19;
            this.cbShowPokeStops.Text = "Show PokeStop";
            this.cbShowPokeStops.UseVisualStyleBackColor = true;
            this.cbShowPokeStops.Visible = false;
            this.cbShowPokeStops.CheckedChanged += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // buttonRefreshForts
            // 
            this.buttonRefreshForts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefreshForts.AutoSize = true;
            this.buttonRefreshForts.Location = new System.Drawing.Point(480, 2);
            this.buttonRefreshForts.Margin = new System.Windows.Forms.Padding(4);
            this.buttonRefreshForts.Name = "buttonRefreshForts";
            this.buttonRefreshForts.Size = new System.Drawing.Size(172, 34);
            this.buttonRefreshForts.TabIndex = 21;
            this.buttonRefreshForts.Text = "Refresh Pokestops";
            this.buttonRefreshForts.UseVisualStyleBackColor = true;
            this.buttonRefreshForts.Visible = false;
            this.buttonRefreshForts.Click += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(705, 62);
            this.button1.Margin = new System.Windows.Forms.Padding(5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(166, 29);
            this.button1.TabIndex = 18;
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
            this.label3.Size = new System.Drawing.Size(59, 17);
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
            this.label2.Size = new System.Drawing.Size(75, 17);
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
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "Latitude:";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox3.Location = new System.Drawing.Point(539, 65);
            this.textBox3.Margin = new System.Windows.Forms.Padding(5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(155, 22);
            this.textBox3.TabIndex = 14;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.Location = new System.Drawing.Point(311, 65);
            this.textBox2.Margin = new System.Windows.Forms.Padding(5);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(155, 22);
            this.textBox2.TabIndex = 13;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(69, 65);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(155, 22);
            this.textBox1.TabIndex = 12;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.panel1.Location = new System.Drawing.Point(2, 419);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(878, 92);
            this.panel1.TabIndex = 23;
            // 
            // btnGetPoints
            // 
            this.btnGetPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGetPoints.Location = new System.Drawing.Point(464, 36);
            this.btnGetPoints.Margin = new System.Windows.Forms.Padding(5);
            this.btnGetPoints.Name = "btnGetPoints";
            this.btnGetPoints.Size = new System.Drawing.Size(135, 29);
            this.btnGetPoints.TabIndex = 25;
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
            this.lblAddress.Size = new System.Drawing.Size(64, 17);
            this.lblAddress.TabIndex = 24;
            this.lblAddress.Text = "Address:";
            // 
            // tbAddress
            // 
            this.tbAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbAddress.Location = new System.Drawing.Point(69, 36);
            this.tbAddress.Margin = new System.Windows.Forms.Padding(5);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(389, 22);
            this.tbAddress.TabIndex = 23;
            // 
            // LocationPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.map);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "LocationPanel";
            this.Size = new System.Drawing.Size(886, 514);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

		}
    }
}
