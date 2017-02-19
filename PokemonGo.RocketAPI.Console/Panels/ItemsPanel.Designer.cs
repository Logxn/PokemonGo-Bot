/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/09/2016
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class ItemsPanel
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.GroupBox groupBoxItems;
		public System.Windows.Forms.TextBox text_TotalItemCount;
		private System.Windows.Forms.Label label31;
		public System.Windows.Forms.NumericUpDown num_MaxTopRevives;
		public System.Windows.Forms.NumericUpDown num_MaxTopPotions;
		public System.Windows.Forms.NumericUpDown num_MaxBerries;
		public System.Windows.Forms.NumericUpDown num_MaxHyperPotions;
		public System.Windows.Forms.NumericUpDown num_MaxSuperPotions;
		public System.Windows.Forms.NumericUpDown num_MaxPotions;
		public System.Windows.Forms.NumericUpDown num_MaxRevives;
		public System.Windows.Forms.NumericUpDown num_MaxUltraBalls;
		public System.Windows.Forms.NumericUpDown num_MaxGreatBalls;
		public System.Windows.Forms.NumericUpDown num_MaxPokeballs;
		private System.Windows.Forms.Button btnRealoadItems;
		private System.Windows.Forms.ListView ItemsListView;
		private System.Windows.Forms.ColumnHeader chItem;
		private System.Windows.Forms.ColumnHeader chCount;
		private System.Windows.Forms.ColumnHeader chUnseen;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripItems;
		private System.Windows.Forms.ToolStripMenuItem recycleToolStripMenuItem;
		private System.Windows.Forms.ImageList imageListItems;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblCount;
		private System.Windows.Forms.Button btnCopy;
		private System.Windows.Forms.Button btnDiscard;
		private System.Windows.Forms.ToolStripMenuItem useToolStripMenuItem;
		private System.Windows.Forms.PictureBox pictureBox9;
		private System.Windows.Forms.PictureBox pictureBox10;
		private System.Windows.Forms.PictureBox pictureBox5;
		private System.Windows.Forms.PictureBox pictureBox6;
		private System.Windows.Forms.PictureBox pictureBox7;
		private System.Windows.Forms.PictureBox pictureBox8;
		private System.Windows.Forms.PictureBox pictureBox3;
		private System.Windows.Forms.PictureBox pictureBox4;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonUSe;
		private System.Windows.Forms.ColumnHeader chID;
		private System.Windows.Forms.PictureBox pictureBoxNana;
		public System.Windows.Forms.NumericUpDown num_MaxNanabBerries;
		private System.Windows.Forms.PictureBox pictureBoxPina;
		public System.Windows.Forms.NumericUpDown num_MaxPinapBerries;
		private System.Windows.Forms.PictureBox pictureBox15;
		public System.Windows.Forms.NumericUpDown num_MaxUpGrade;
		private System.Windows.Forms.PictureBox pictureBox11;
		private System.Windows.Forms.PictureBox pictureBox12;
		private System.Windows.Forms.PictureBox pictureBox13;
		private System.Windows.Forms.PictureBox pictureBox14;
		public System.Windows.Forms.NumericUpDown num_MaxMetalCoat;
		public System.Windows.Forms.NumericUpDown num_MaxKingsRock;
		public System.Windows.Forms.NumericUpDown num_MaxSunStone;
		public System.Windows.Forms.NumericUpDown num_MaxDragonScale;
		
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
		    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemsPanel));
		    this.groupBoxItems = new System.Windows.Forms.GroupBox();
		    this.pictureBox15 = new System.Windows.Forms.PictureBox();
		    this.num_MaxUpGrade = new System.Windows.Forms.NumericUpDown();
		    this.pictureBox11 = new System.Windows.Forms.PictureBox();
		    this.pictureBox12 = new System.Windows.Forms.PictureBox();
		    this.pictureBox13 = new System.Windows.Forms.PictureBox();
		    this.pictureBox14 = new System.Windows.Forms.PictureBox();
		    this.num_MaxMetalCoat = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxKingsRock = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxSunStone = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxDragonScale = new System.Windows.Forms.NumericUpDown();
		    this.pictureBoxNana = new System.Windows.Forms.PictureBox();
		    this.num_MaxNanabBerries = new System.Windows.Forms.NumericUpDown();
		    this.pictureBoxPina = new System.Windows.Forms.PictureBox();
		    this.num_MaxPinapBerries = new System.Windows.Forms.NumericUpDown();
		    this.pictureBox9 = new System.Windows.Forms.PictureBox();
		    this.pictureBox10 = new System.Windows.Forms.PictureBox();
		    this.pictureBox5 = new System.Windows.Forms.PictureBox();
		    this.pictureBox6 = new System.Windows.Forms.PictureBox();
		    this.pictureBox7 = new System.Windows.Forms.PictureBox();
		    this.pictureBox8 = new System.Windows.Forms.PictureBox();
		    this.pictureBox3 = new System.Windows.Forms.PictureBox();
		    this.pictureBox4 = new System.Windows.Forms.PictureBox();
		    this.pictureBox2 = new System.Windows.Forms.PictureBox();
		    this.pictureBox1 = new System.Windows.Forms.PictureBox();
		    this.text_TotalItemCount = new System.Windows.Forms.TextBox();
		    this.label31 = new System.Windows.Forms.Label();
		    this.num_MaxTopRevives = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxTopPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxBerries = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxHyperPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxSuperPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxRevives = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxUltraBalls = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxGreatBalls = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxPokeballs = new System.Windows.Forms.NumericUpDown();
		    this.btnRealoadItems = new System.Windows.Forms.Button();
		    this.ItemsListView = new System.Windows.Forms.ListView();
		    this.chItem = new System.Windows.Forms.ColumnHeader();
		    this.chCount = new System.Windows.Forms.ColumnHeader();
		    this.chUnseen = new System.Windows.Forms.ColumnHeader();
		    this.chID = new System.Windows.Forms.ColumnHeader();
		    this.contextMenuStripItems = new System.Windows.Forms.ContextMenuStrip(this.components);
		    this.recycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		    this.useToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		    this.imageListItems = new System.Windows.Forms.ImageList(this.components);
		    this.label1 = new System.Windows.Forms.Label();
		    this.lblCount = new System.Windows.Forms.Label();
		    this.btnCopy = new System.Windows.Forms.Button();
		    this.btnDiscard = new System.Windows.Forms.Button();
		    this.buttonUSe = new System.Windows.Forms.Button();
		    this.groupBoxItems.SuspendLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxUpGrade)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxMetalCoat)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxKingsRock)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxSunStone)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxDragonScale)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNana)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxNanabBerries)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPina)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPinapBerries)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopRevives)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopPotions)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxBerries)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxHyperPotions)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxSuperPotions)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPotions)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxRevives)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxUltraBalls)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxGreatBalls)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPokeballs)).BeginInit();
		    this.contextMenuStripItems.SuspendLayout();
		    this.SuspendLayout();
		    // 
		    // groupBoxItems
		    // 
		    this.groupBoxItems.Controls.Add(this.pictureBox15);
		    this.groupBoxItems.Controls.Add(this.num_MaxUpGrade);
		    this.groupBoxItems.Controls.Add(this.pictureBox11);
		    this.groupBoxItems.Controls.Add(this.pictureBox12);
		    this.groupBoxItems.Controls.Add(this.pictureBox13);
		    this.groupBoxItems.Controls.Add(this.pictureBox14);
		    this.groupBoxItems.Controls.Add(this.num_MaxMetalCoat);
		    this.groupBoxItems.Controls.Add(this.num_MaxKingsRock);
		    this.groupBoxItems.Controls.Add(this.num_MaxSunStone);
		    this.groupBoxItems.Controls.Add(this.num_MaxDragonScale);
		    this.groupBoxItems.Controls.Add(this.pictureBoxNana);
		    this.groupBoxItems.Controls.Add(this.num_MaxNanabBerries);
		    this.groupBoxItems.Controls.Add(this.pictureBoxPina);
		    this.groupBoxItems.Controls.Add(this.num_MaxPinapBerries);
		    this.groupBoxItems.Controls.Add(this.pictureBox9);
		    this.groupBoxItems.Controls.Add(this.pictureBox10);
		    this.groupBoxItems.Controls.Add(this.pictureBox5);
		    this.groupBoxItems.Controls.Add(this.pictureBox6);
		    this.groupBoxItems.Controls.Add(this.pictureBox7);
		    this.groupBoxItems.Controls.Add(this.pictureBox8);
		    this.groupBoxItems.Controls.Add(this.pictureBox3);
		    this.groupBoxItems.Controls.Add(this.pictureBox4);
		    this.groupBoxItems.Controls.Add(this.pictureBox2);
		    this.groupBoxItems.Controls.Add(this.pictureBox1);
		    this.groupBoxItems.Controls.Add(this.text_TotalItemCount);
		    this.groupBoxItems.Controls.Add(this.label31);
		    this.groupBoxItems.Controls.Add(this.num_MaxTopRevives);
		    this.groupBoxItems.Controls.Add(this.num_MaxTopPotions);
		    this.groupBoxItems.Controls.Add(this.num_MaxBerries);
		    this.groupBoxItems.Controls.Add(this.num_MaxHyperPotions);
		    this.groupBoxItems.Controls.Add(this.num_MaxSuperPotions);
		    this.groupBoxItems.Controls.Add(this.num_MaxPotions);
		    this.groupBoxItems.Controls.Add(this.num_MaxRevives);
		    this.groupBoxItems.Controls.Add(this.num_MaxUltraBalls);
		    this.groupBoxItems.Controls.Add(this.num_MaxGreatBalls);
		    this.groupBoxItems.Controls.Add(this.num_MaxPokeballs);
		    this.groupBoxItems.Location = new System.Drawing.Point(5, 5);
		    this.groupBoxItems.Margin = new System.Windows.Forms.Padding(5);
		    this.groupBoxItems.Name = "groupBoxItems";
		    this.groupBoxItems.Padding = new System.Windows.Forms.Padding(5);
		    this.groupBoxItems.Size = new System.Drawing.Size(233, 465);
		    this.groupBoxItems.TabIndex = 80;
		    this.groupBoxItems.TabStop = false;
		    this.groupBoxItems.Text = "Items - Maximun Values";
		    // 
		    // pictureBox15
		    // 
		    this.pictureBox15.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox15.Image")));
		    this.pictureBox15.Location = new System.Drawing.Point(17, 397);
		    this.pictureBox15.Name = "pictureBox15";
		    this.pictureBox15.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox15.TabIndex = 51;
		    this.pictureBox15.TabStop = false;
		    // 
		    // num_MaxUpGrade
		    // 
		    this.num_MaxUpGrade.Location = new System.Drawing.Point(58, 414);
		    this.num_MaxUpGrade.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxUpGrade.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxUpGrade.Name = "num_MaxUpGrade";
		    this.num_MaxUpGrade.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxUpGrade.TabIndex = 50;
		    this.num_MaxUpGrade.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // pictureBox11
		    // 
		    this.pictureBox11.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox11.Image")));
		    this.pictureBox11.Location = new System.Drawing.Point(123, 354);
		    this.pictureBox11.Name = "pictureBox11";
		    this.pictureBox11.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox11.TabIndex = 49;
		    this.pictureBox11.TabStop = false;
		    // 
		    // pictureBox12
		    // 
		    this.pictureBox12.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox12.Image")));
		    this.pictureBox12.Location = new System.Drawing.Point(17, 354);
		    this.pictureBox12.Name = "pictureBox12";
		    this.pictureBox12.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox12.TabIndex = 48;
		    this.pictureBox12.TabStop = false;
		    // 
		    // pictureBox13
		    // 
		    this.pictureBox13.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox13.Image")));
		    this.pictureBox13.Location = new System.Drawing.Point(123, 309);
		    this.pictureBox13.Name = "pictureBox13";
		    this.pictureBox13.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox13.TabIndex = 47;
		    this.pictureBox13.TabStop = false;
		    // 
		    // pictureBox14
		    // 
		    this.pictureBox14.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox14.Image")));
		    this.pictureBox14.Location = new System.Drawing.Point(17, 309);
		    this.pictureBox14.Name = "pictureBox14";
		    this.pictureBox14.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox14.TabIndex = 46;
		    this.pictureBox14.TabStop = false;
		    // 
		    // num_MaxMetalCoat
		    // 
		    this.num_MaxMetalCoat.Location = new System.Drawing.Point(165, 371);
		    this.num_MaxMetalCoat.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxMetalCoat.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxMetalCoat.Name = "num_MaxMetalCoat";
		    this.num_MaxMetalCoat.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxMetalCoat.TabIndex = 45;
		    this.num_MaxMetalCoat.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxKingsRock
		    // 
		    this.num_MaxKingsRock.Location = new System.Drawing.Point(58, 371);
		    this.num_MaxKingsRock.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxKingsRock.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxKingsRock.Name = "num_MaxKingsRock";
		    this.num_MaxKingsRock.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxKingsRock.TabIndex = 44;
		    this.num_MaxKingsRock.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxSunStone
		    // 
		    this.num_MaxSunStone.Location = new System.Drawing.Point(165, 326);
		    this.num_MaxSunStone.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxSunStone.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxSunStone.Name = "num_MaxSunStone";
		    this.num_MaxSunStone.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxSunStone.TabIndex = 43;
		    this.num_MaxSunStone.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxDragonScale
		    // 
		    this.num_MaxDragonScale.Location = new System.Drawing.Point(58, 326);
		    this.num_MaxDragonScale.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxDragonScale.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxDragonScale.Name = "num_MaxDragonScale";
		    this.num_MaxDragonScale.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxDragonScale.TabIndex = 42;
		    this.num_MaxDragonScale.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // pictureBoxNana
		    // 
		    this.pictureBoxNana.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxNana.Image")));
		    this.pictureBoxNana.Location = new System.Drawing.Point(123, 112);
		    this.pictureBoxNana.Name = "pictureBoxNana";
		    this.pictureBoxNana.Size = new System.Drawing.Size(41, 37);
		    this.pictureBoxNana.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBoxNana.TabIndex = 41;
		    this.pictureBoxNana.TabStop = false;
		    // 
		    // num_MaxNanabBerries
		    // 
		    this.num_MaxNanabBerries.Location = new System.Drawing.Point(165, 129);
		    this.num_MaxNanabBerries.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxNanabBerries.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxNanabBerries.Name = "num_MaxNanabBerries";
		    this.num_MaxNanabBerries.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxNanabBerries.TabIndex = 40;
		    this.num_MaxNanabBerries.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // pictureBoxPina
		    // 
		    this.pictureBoxPina.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxPina.Image")));
		    this.pictureBoxPina.Location = new System.Drawing.Point(17, 112);
		    this.pictureBoxPina.Name = "pictureBoxPina";
		    this.pictureBoxPina.Size = new System.Drawing.Size(41, 37);
		    this.pictureBoxPina.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBoxPina.TabIndex = 38;
		    this.pictureBoxPina.TabStop = false;
		    // 
		    // num_MaxPinapBerries
		    // 
		    this.num_MaxPinapBerries.Location = new System.Drawing.Point(58, 129);
		    this.num_MaxPinapBerries.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxPinapBerries.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPinapBerries.Name = "num_MaxPinapBerries";
		    this.num_MaxPinapBerries.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxPinapBerries.TabIndex = 37;
		    this.num_MaxPinapBerries.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // pictureBox9
		    // 
		    this.pictureBox9.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox9.Image")));
		    this.pictureBox9.Location = new System.Drawing.Point(123, 68);
		    this.pictureBox9.Name = "pictureBox9";
		    this.pictureBox9.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox9.TabIndex = 35;
		    this.pictureBox9.TabStop = false;
		    // 
		    // pictureBox10
		    // 
		    this.pictureBox10.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox10.Image")));
		    this.pictureBox10.Location = new System.Drawing.Point(123, 260);
		    this.pictureBox10.Name = "pictureBox10";
		    this.pictureBox10.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox10.TabIndex = 34;
		    this.pictureBox10.TabStop = false;
		    // 
		    // pictureBox5
		    // 
		    this.pictureBox5.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox5.Image")));
		    this.pictureBox5.Location = new System.Drawing.Point(17, 260);
		    this.pictureBox5.Name = "pictureBox5";
		    this.pictureBox5.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox5.TabIndex = 33;
		    this.pictureBox5.TabStop = false;
		    // 
		    // pictureBox6
		    // 
		    this.pictureBox6.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox6.Image")));
		    this.pictureBox6.Location = new System.Drawing.Point(123, 215);
		    this.pictureBox6.Name = "pictureBox6";
		    this.pictureBox6.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox6.TabIndex = 32;
		    this.pictureBox6.TabStop = false;
		    // 
		    // pictureBox7
		    // 
		    this.pictureBox7.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox7.Image")));
		    this.pictureBox7.Location = new System.Drawing.Point(17, 215);
		    this.pictureBox7.Name = "pictureBox7";
		    this.pictureBox7.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox7.TabIndex = 31;
		    this.pictureBox7.TabStop = false;
		    // 
		    // pictureBox8
		    // 
		    this.pictureBox8.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox8.Image")));
		    this.pictureBox8.Location = new System.Drawing.Point(123, 163);
		    this.pictureBox8.Name = "pictureBox8";
		    this.pictureBox8.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox8.TabIndex = 30;
		    this.pictureBox8.TabStop = false;
		    // 
		    // pictureBox3
		    // 
		    this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
		    this.pictureBox3.Location = new System.Drawing.Point(16, 163);
		    this.pictureBox3.Name = "pictureBox3";
		    this.pictureBox3.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox3.TabIndex = 29;
		    this.pictureBox3.TabStop = false;
		    // 
		    // pictureBox4
		    // 
		    this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
		    this.pictureBox4.Location = new System.Drawing.Point(16, 67);
		    this.pictureBox4.Name = "pictureBox4";
		    this.pictureBox4.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox4.TabIndex = 28;
		    this.pictureBox4.TabStop = false;
		    // 
		    // pictureBox2
		    // 
		    this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
		    this.pictureBox2.Location = new System.Drawing.Point(123, 23);
		    this.pictureBox2.Name = "pictureBox2";
		    this.pictureBox2.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		    this.pictureBox2.TabIndex = 27;
		    this.pictureBox2.TabStop = false;
		    // 
		    // pictureBox1
		    // 
		    this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
		    this.pictureBox1.Location = new System.Drawing.Point(16, 22);
		    this.pictureBox1.Name = "pictureBox1";
		    this.pictureBox1.Size = new System.Drawing.Size(41, 37);
		    this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		    this.pictureBox1.TabIndex = 26;
		    this.pictureBox1.TabStop = false;
		    // 
		    // text_TotalItemCount
		    // 
		    this.text_TotalItemCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
		    this.text_TotalItemCount.Location = new System.Drawing.Point(156, 442);
		    this.text_TotalItemCount.Margin = new System.Windows.Forms.Padding(5);
		    this.text_TotalItemCount.Name = "text_TotalItemCount";
		    this.text_TotalItemCount.ReadOnly = true;
		    this.text_TotalItemCount.Size = new System.Drawing.Size(63, 13);
		    this.text_TotalItemCount.TabIndex = 23;
		    this.text_TotalItemCount.TabStop = false;
		    // 
		    // label31
		    // 
		    this.label31.AutoSize = true;
		    this.label31.Location = new System.Drawing.Point(87, 442);
		    this.label31.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label31.Name = "label31";
		    this.label31.Size = new System.Drawing.Size(65, 13);
		    this.label31.TabIndex = 22;
		    this.label31.Text = "Total Count:";
		    this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		    // 
		    // num_MaxTopRevives
		    // 
		    this.num_MaxTopRevives.Location = new System.Drawing.Point(165, 180);
		    this.num_MaxTopRevives.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxTopRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopRevives.Name = "num_MaxTopRevives";
		    this.num_MaxTopRevives.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxTopRevives.TabIndex = 20;
		    this.num_MaxTopRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxTopPotions
		    // 
		    this.num_MaxTopPotions.Location = new System.Drawing.Point(165, 277);
		    this.num_MaxTopPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxTopPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopPotions.Name = "num_MaxTopPotions";
		    this.num_MaxTopPotions.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxTopPotions.TabIndex = 24;
		    this.num_MaxTopPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxBerries
		    // 
		    this.num_MaxBerries.Location = new System.Drawing.Point(164, 85);
		    this.num_MaxBerries.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxBerries.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxBerries.Name = "num_MaxBerries";
		    this.num_MaxBerries.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxBerries.TabIndex = 25;
		    this.num_MaxBerries.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxHyperPotions
		    // 
		    this.num_MaxHyperPotions.Location = new System.Drawing.Point(58, 277);
		    this.num_MaxHyperPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxHyperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxHyperPotions.Name = "num_MaxHyperPotions";
		    this.num_MaxHyperPotions.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxHyperPotions.TabIndex = 23;
		    this.num_MaxHyperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxSuperPotions
		    // 
		    this.num_MaxSuperPotions.Location = new System.Drawing.Point(165, 232);
		    this.num_MaxSuperPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxSuperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxSuperPotions.Name = "num_MaxSuperPotions";
		    this.num_MaxSuperPotions.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxSuperPotions.TabIndex = 22;
		    this.num_MaxSuperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxPotions
		    // 
		    this.num_MaxPotions.Location = new System.Drawing.Point(58, 232);
		    this.num_MaxPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPotions.Name = "num_MaxPotions";
		    this.num_MaxPotions.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxPotions.TabIndex = 21;
		    this.num_MaxPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxRevives
		    // 
		    this.num_MaxRevives.Location = new System.Drawing.Point(58, 180);
		    this.num_MaxRevives.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxRevives.Name = "num_MaxRevives";
		    this.num_MaxRevives.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxRevives.TabIndex = 19;
		    this.num_MaxRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxUltraBalls
		    // 
		    this.num_MaxUltraBalls.Location = new System.Drawing.Point(58, 84);
		    this.num_MaxUltraBalls.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxUltraBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxUltraBalls.Name = "num_MaxUltraBalls";
		    this.num_MaxUltraBalls.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxUltraBalls.TabIndex = 17;
		    this.num_MaxUltraBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxGreatBalls
		    // 
		    this.num_MaxGreatBalls.Location = new System.Drawing.Point(165, 40);
		    this.num_MaxGreatBalls.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxGreatBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxGreatBalls.Name = "num_MaxGreatBalls";
		    this.num_MaxGreatBalls.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxGreatBalls.TabIndex = 16;
		    this.num_MaxGreatBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxPokeballs
		    // 
		    this.num_MaxPokeballs.Location = new System.Drawing.Point(58, 39);
		    this.num_MaxPokeballs.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxPokeballs.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPokeballs.Name = "num_MaxPokeballs";
		    this.num_MaxPokeballs.Size = new System.Drawing.Size(52, 20);
		    this.num_MaxPokeballs.TabIndex = 15;
		    this.num_MaxPokeballs.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // btnRealoadItems
		    // 
		    this.btnRealoadItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.btnRealoadItems.Location = new System.Drawing.Point(583, 478);
		    this.btnRealoadItems.Margin = new System.Windows.Forms.Padding(2);
		    this.btnRealoadItems.Name = "btnRealoadItems";
		    this.btnRealoadItems.Size = new System.Drawing.Size(69, 29);
		    this.btnRealoadItems.TabIndex = 79;
		    this.btnRealoadItems.Text = "Reload";
		    this.btnRealoadItems.UseVisualStyleBackColor = true;
		    this.btnRealoadItems.Click += new System.EventHandler(this.BtnRealoadItemsClick);
		    // 
		    // ItemsListView
		    // 
		    this.ItemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
		    this.ItemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chItem,
            this.chCount,
            this.chUnseen,
            this.chID});
		    this.ItemsListView.ContextMenuStrip = this.contextMenuStripItems;
		    this.ItemsListView.FullRowSelect = true;
		    this.ItemsListView.GridLines = true;
		    this.ItemsListView.LargeImageList = this.imageListItems;
		    this.ItemsListView.Location = new System.Drawing.Point(245, 14);
		    this.ItemsListView.Margin = new System.Windows.Forms.Padding(4);
		    this.ItemsListView.Name = "ItemsListView";
		    this.ItemsListView.Size = new System.Drawing.Size(407, 446);
		    this.ItemsListView.SmallImageList = this.imageListItems;
		    this.ItemsListView.TabIndex = 78;
		    this.ItemsListView.UseCompatibleStateImageBehavior = false;
		    this.ItemsListView.View = System.Windows.Forms.View.Details;
		    this.ItemsListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ItemsListView_ColumnClick);
		    // 
		    // chItem
		    // 
		    this.chItem.Text = "Item";
		    this.chItem.Width = 160;
		    // 
		    // chCount
		    // 
		    this.chCount.Text = "Count";
		    // 
		    // chUnseen
		    // 
		    this.chUnseen.Text = "Unseen";
		    // 
		    // chID
		    // 
		    this.chID.Text = "#";
		    this.chID.Width = 40;
		    // 
		    // contextMenuStripItems
		    // 
		    this.contextMenuStripItems.ImageScalingSize = new System.Drawing.Size(20, 20);
		    this.contextMenuStripItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recycleToolStripMenuItem,
            this.useToolStripMenuItem});
		    this.contextMenuStripItems.Name = "contextMenuStrip1";
		    this.contextMenuStripItems.Size = new System.Drawing.Size(115, 48);
		    // 
		    // recycleToolStripMenuItem
		    // 
		    this.recycleToolStripMenuItem.Name = "recycleToolStripMenuItem";
		    this.recycleToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
		    this.recycleToolStripMenuItem.Text = "Recycle";
		    this.recycleToolStripMenuItem.Click += new System.EventHandler(this.RecycleToolStripMenuItemClick);
		    // 
		    // useToolStripMenuItem
		    // 
		    this.useToolStripMenuItem.Name = "useToolStripMenuItem";
		    this.useToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
		    this.useToolStripMenuItem.Text = "Use";
		    this.useToolStripMenuItem.Click += new System.EventHandler(this.useToolStripMenuItem_Click);
		    // 
		    // imageListItems
		    // 
		    this.imageListItems.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListItems.ImageStream")));
		    this.imageListItems.TransparentColor = System.Drawing.Color.Transparent;
		    this.imageListItems.Images.SetKeyName(0, "incenseOrdinary");
		    this.imageListItems.Images.SetKeyName(1, "hyperpotion");
		    this.imageListItems.Images.SetKeyName(2, "pokeball");
		    this.imageListItems.Images.SetKeyName(3, "potion");
		    this.imageListItems.Images.SetKeyName(4, "revive");
		    this.imageListItems.Images.SetKeyName(5, "greatball");
		    this.imageListItems.Images.SetKeyName(6, "superpotion");
		    this.imageListItems.Images.SetKeyName(7, "ultraball");
		    this.imageListItems.Images.SetKeyName(8, "incubatorbasic");
		    this.imageListItems.Images.SetKeyName(9, "incubatorbasicunlimited");
		    this.imageListItems.Images.SetKeyName(10, "razzberry");
		    this.imageListItems.Images.SetKeyName(11, "troydisk");
		    this.imageListItems.Images.SetKeyName(12, "luckyegg");
		    this.imageListItems.Images.SetKeyName(13, "maxrevive");
		    this.imageListItems.Images.SetKeyName(14, "maxpotion");
		    this.imageListItems.Images.SetKeyName(15, "pinapberry");
		    this.imageListItems.Images.SetKeyName(16, "nanabberry");
		    this.imageListItems.Images.SetKeyName(17, "dragonscale");
		    this.imageListItems.Images.SetKeyName(18, "kingsrock");
		    this.imageListItems.Images.SetKeyName(19, "metalcoat");
		    this.imageListItems.Images.SetKeyName(20, "sunstone");
		    this.imageListItems.Images.SetKeyName(21, "upgrade");
		    // 
		    // label1
		    // 
		    this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.label1.AutoSize = true;
		    this.label1.Location = new System.Drawing.Point(245, 464);
		    this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label1.Name = "label1";
		    this.label1.Size = new System.Drawing.Size(65, 13);
		    this.label1.TabIndex = 81;
		    this.label1.Text = "Total Count:";
		    this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		    // 
		    // lblCount
		    // 
		    this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.lblCount.AutoSize = true;
		    this.lblCount.Location = new System.Drawing.Point(309, 464);
		    this.lblCount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.lblCount.Name = "lblCount";
		    this.lblCount.Size = new System.Drawing.Size(13, 13);
		    this.lblCount.TabIndex = 82;
		    this.lblCount.Text = "0";
		    this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		    // 
		    // btnCopy
		    // 
		    this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		    this.btnCopy.Location = new System.Drawing.Point(6, 477);
		    this.btnCopy.Margin = new System.Windows.Forms.Padding(2);
		    this.btnCopy.Name = "btnCopy";
		    this.btnCopy.Size = new System.Drawing.Size(116, 29);
		    this.btnCopy.TabIndex = 83;
		    this.btnCopy.Text = "Adjust to Current Values";
		    this.btnCopy.UseVisualStyleBackColor = true;
		    this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
		    // 
		    // btnDiscard
		    // 
		    this.btnDiscard.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		    this.btnDiscard.Location = new System.Drawing.Point(128, 477);
		    this.btnDiscard.Margin = new System.Windows.Forms.Padding(2);
		    this.btnDiscard.Name = "btnDiscard";
		    this.btnDiscard.Size = new System.Drawing.Size(89, 29);
		    this.btnDiscard.TabIndex = 84;
		    this.btnDiscard.Text = "Discard Excess";
		    this.btnDiscard.UseVisualStyleBackColor = true;
		    this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
		    // 
		    // buttonUSe
		    // 
		    this.buttonUSe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.buttonUSe.Location = new System.Drawing.Point(510, 478);
		    this.buttonUSe.Margin = new System.Windows.Forms.Padding(2);
		    this.buttonUSe.Name = "buttonUSe";
		    this.buttonUSe.Size = new System.Drawing.Size(69, 29);
		    this.buttonUSe.TabIndex = 85;
		    this.buttonUSe.Text = "Use Item";
		    this.buttonUSe.UseVisualStyleBackColor = true;
		    this.buttonUSe.Click += new System.EventHandler(this.useToolStripMenuItem_Click);
		    // 
		    // ItemsPanel
		    // 
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		    this.Controls.Add(this.buttonUSe);
		    this.Controls.Add(this.btnDiscard);
		    this.Controls.Add(this.btnCopy);
		    this.Controls.Add(this.lblCount);
		    this.Controls.Add(this.label1);
		    this.Controls.Add(this.groupBoxItems);
		    this.Controls.Add(this.btnRealoadItems);
		    this.Controls.Add(this.ItemsListView);
		    this.Margin = new System.Windows.Forms.Padding(4);
		    this.Name = "ItemsPanel";
		    this.Size = new System.Drawing.Size(662, 514);
		    this.groupBoxItems.ResumeLayout(false);
		    this.groupBoxItems.PerformLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox15)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxUpGrade)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxMetalCoat)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxKingsRock)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxSunStone)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxDragonScale)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBoxNana)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxNanabBerries)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPina)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPinapBerries)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopRevives)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopPotions)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxBerries)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxHyperPotions)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxSuperPotions)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPotions)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxRevives)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxUltraBalls)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxGreatBalls)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxPokeballs)).EndInit();
		    this.contextMenuStripItems.ResumeLayout(false);
		    this.ResumeLayout(false);
		    this.PerformLayout();

		}
	}
}
