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
			this.map.Location = new System.Drawing.Point(4, 4);
			this.map.Margin = new System.Windows.Forms.Padding(4);
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
			this.map.Size = new System.Drawing.Size(773, 403);
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
			this.cbShowPokemon.Location = new System.Drawing.Point(116, 363);
			this.cbShowPokemon.Name = "cbShowPokemon";
			this.cbShowPokemon.Size = new System.Drawing.Size(101, 17);
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
			this.buttonRefreshPokemon.Location = new System.Drawing.Point(252, 359);
			this.buttonRefreshPokemon.Name = "buttonRefreshPokemon";
			this.buttonRefreshPokemon.Size = new System.Drawing.Size(108, 23);
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
			this.cbShowPokeStops.Location = new System.Drawing.Point(7, 363);
			this.cbShowPokeStops.Margin = new System.Windows.Forms.Padding(4);
			this.cbShowPokeStops.Name = "cbShowPokeStops";
			this.cbShowPokeStops.Size = new System.Drawing.Size(103, 17);
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
			this.buttonRefreshForts.Location = new System.Drawing.Point(374, 359);
			this.buttonRefreshForts.Name = "buttonRefreshForts";
			this.buttonRefreshForts.Size = new System.Drawing.Size(108, 23);
			this.buttonRefreshForts.TabIndex = 21;
			this.buttonRefreshForts.Text = "Refresh Pokestops";
			this.buttonRefreshForts.UseVisualStyleBackColor = true;
			this.buttonRefreshForts.Visible = false;
			this.buttonRefreshForts.Click += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button1.Location = new System.Drawing.Point(568, 379);
			this.button1.Margin = new System.Windows.Forms.Padding(4);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(133, 23);
			this.button1.TabIndex = 18;
			this.button1.Text = "Set Location";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(384, 383);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 17;
			this.label3.Text = "Altitude:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(190, 383);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(57, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "Longitude:";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 383);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(48, 13);
			this.label1.TabIndex = 15;
			this.label1.Text = "Latitude:";
			// 
			// textBox3
			// 
			this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox3.Location = new System.Drawing.Point(435, 382);
			this.textBox3.Margin = new System.Windows.Forms.Padding(4);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(125, 20);
			this.textBox3.TabIndex = 14;
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox2.Location = new System.Drawing.Point(253, 382);
			this.textBox2.Margin = new System.Windows.Forms.Padding(4);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(125, 20);
			this.textBox2.TabIndex = 13;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox1.Location = new System.Drawing.Point(59, 382);
			this.textBox1.Margin = new System.Windows.Forms.Padding(4);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(125, 20);
			this.textBox1.TabIndex = 12;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// LocationPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbShowPokemon);
			this.Controls.Add(this.buttonRefreshPokemon);
			this.Controls.Add(this.cbShowPokeStops);
			this.Controls.Add(this.buttonRefreshForts);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.map);
			this.Name = "LocationPanel";
			this.Size = new System.Drawing.Size(709, 411);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
