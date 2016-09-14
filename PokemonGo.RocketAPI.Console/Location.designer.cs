namespace PokemonGo.RocketAPI.Console
{
    partial class LocationSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationSelect));
            this.map = new GMap.NET.WindowsForms.GMapControl();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.cbShowPokeStops = new System.Windows.Forms.CheckBox();
            this.buttonRefreshPokemon = new System.Windows.Forms.Button();
            this.cbShowPokemon = new System.Windows.Forms.CheckBox();
            this.buttonRefreshForts = new System.Windows.Forms.Button();
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
            this.map.Location = new System.Drawing.Point(16, 15);
            this.map.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
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
            this.map.Size = new System.Drawing.Size(931, 585);
            this.map.TabIndex = 0;
            this.map.Zoom = 0D;
            this.map.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.map_OnMarkerClick);
            this.map.OnMapDrag += new GMap.NET.MapDrag(this.map_OnMapDrag);
            this.map.Load += new System.EventHandler(this.map_Load);
            // 
            // text_EMail
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox1.Location = new System.Drawing.Point(88, 612);
            this.textBox1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBox1.Name = "text_EMail";
            this.textBox1.Size = new System.Drawing.Size(165, 22);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // text_Password
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox2.Location = new System.Drawing.Point(347, 612);
            this.textBox2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBox2.Name = "text_Password";
            this.textBox2.Size = new System.Drawing.Size(165, 22);
            this.textBox2.TabIndex = 2;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // text_Latidude
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox3.Location = new System.Drawing.Point(589, 612);
            this.textBox3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.textBox3.Name = "text_Latidude";
            this.textBox3.Size = new System.Drawing.Size(165, 22);
            this.textBox3.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 613);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Latitude:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 613);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Longitude:";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(521, 613);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Altitude:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(769, 607);
            this.button1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(177, 28);
            this.button1.TabIndex = 7;
            this.button1.Text = "Set Location";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbShowPokeStops
            // 
            this.cbShowPokeStops.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowPokeStops.AutoSize = true;
            this.cbShowPokeStops.Checked = true;
            this.cbShowPokeStops.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokeStops.Location = new System.Drawing.Point(19, 588);
            this.cbShowPokeStops.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.cbShowPokeStops.Name = "cbShowPokeStops";
            this.cbShowPokeStops.Size = new System.Drawing.Size(129, 21);
            this.cbShowPokeStops.TabIndex = 8;
            this.cbShowPokeStops.Text = "Show PokeStop";
            this.cbShowPokeStops.UseVisualStyleBackColor = true;
            this.cbShowPokeStops.Visible = false;
            this.cbShowPokeStops.CheckedChanged += new System.EventHandler(this.cbShowPokeStops_CheckedChanged);
            // 
            // buttonRefreshPokemon
            // 
            this.buttonRefreshPokemon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefreshPokemon.AutoSize = true;
            this.buttonRefreshPokemon.Location = new System.Drawing.Point(345, 583);
            this.buttonRefreshPokemon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonRefreshPokemon.Name = "buttonRefreshPokemon";
            this.buttonRefreshPokemon.Size = new System.Drawing.Size(144, 28);
            this.buttonRefreshPokemon.TabIndex = 10;
            this.buttonRefreshPokemon.Text = "Refresh Pokemon";
            this.buttonRefreshPokemon.UseVisualStyleBackColor = true;
            this.buttonRefreshPokemon.Visible = false;            
            this.buttonRefreshPokemon.Click += new System.EventHandler(this.buttonRefreshPokemon_Click_1);
            // 
            // cbShowPokemon
            // 
            this.cbShowPokemon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbShowPokemon.AutoSize = true;
            this.cbShowPokemon.Checked = true;
            this.cbShowPokemon.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowPokemon.Location = new System.Drawing.Point(164, 588);
            this.cbShowPokemon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbShowPokemon.Name = "cbShowPokemon";
            this.cbShowPokemon.Size = new System.Drawing.Size(127, 21);
            this.cbShowPokemon.TabIndex = 11;
            this.cbShowPokemon.Text = "Show Pokemon";
            this.cbShowPokemon.UseVisualStyleBackColor = true;
            this.cbShowPokemon.Visible = false;
            this.cbShowPokemon.CheckedChanged += new System.EventHandler(this.cbShowPokemon_CheckedChanged);
            // 
            // buttonRefreshForts
            // 
            this.buttonRefreshForts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonRefreshForts.AutoSize = true;
            this.buttonRefreshForts.Location = new System.Drawing.Point(508, 583);
            this.buttonRefreshForts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonRefreshForts.Name = "buttonRefreshForts";
            this.buttonRefreshForts.Size = new System.Drawing.Size(144, 28);
            this.buttonRefreshForts.TabIndex = 10;
            this.buttonRefreshForts.Text = "Refresh Pokestops";
            this.buttonRefreshForts.UseVisualStyleBackColor = true;
            this.buttonRefreshForts.Visible = false;            
            this.buttonRefreshForts.Click += new System.EventHandler(this.buttonRefreshForts_Click);
            // 
            // LocationSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 641);
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
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.MaximizeBox = false;
            this.Name = "LocationSelect";
            this.Text = "Location";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocationSelect_FormClosing);
            this.Load += new System.EventHandler(this.LocationSelect_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GMap.NET.WindowsForms.GMapControl map;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cbShowPokeStops;
        private System.Windows.Forms.Button buttonRefreshPokemon;
        private System.Windows.Forms.Button buttonRefreshForts;
        private System.Windows.Forms.CheckBox cbShowPokemon;
    }
}