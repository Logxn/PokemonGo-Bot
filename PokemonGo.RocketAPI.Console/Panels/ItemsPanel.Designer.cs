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
		private System.Windows.Forms.GroupBox groupBox4;
		public System.Windows.Forms.TextBox text_TotalItemCount;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label25;
		public System.Windows.Forms.NumericUpDown num_MaxTopRevives;
		public System.Windows.Forms.NumericUpDown num_MaxTopPotions;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label16;
		public System.Windows.Forms.NumericUpDown num_MaxRazzBerrys;
		public System.Windows.Forms.NumericUpDown num_MaxHyperPotions;
		public System.Windows.Forms.NumericUpDown num_MaxSuperPotions;
		public System.Windows.Forms.NumericUpDown num_MaxPotions;
		public System.Windows.Forms.NumericUpDown num_MaxRevives;
		private System.Windows.Forms.Label label15;
		public System.Windows.Forms.NumericUpDown num_MaxUltraBalls;
		public System.Windows.Forms.NumericUpDown num_MaxGreatBalls;
		private System.Windows.Forms.Label label14;
		public System.Windows.Forms.NumericUpDown num_MaxPokeballs;
		private System.Windows.Forms.Label label13;
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
		    this.groupBox4 = new System.Windows.Forms.GroupBox();
		    this.text_TotalItemCount = new System.Windows.Forms.TextBox();
		    this.label31 = new System.Windows.Forms.Label();
		    this.label27 = new System.Windows.Forms.Label();
		    this.label25 = new System.Windows.Forms.Label();
		    this.num_MaxTopRevives = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxTopPotions = new System.Windows.Forms.NumericUpDown();
		    this.label20 = new System.Windows.Forms.Label();
		    this.label19 = new System.Windows.Forms.Label();
		    this.label18 = new System.Windows.Forms.Label();
		    this.label17 = new System.Windows.Forms.Label();
		    this.label16 = new System.Windows.Forms.Label();
		    this.num_MaxRazzBerrys = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxHyperPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxSuperPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxPotions = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxRevives = new System.Windows.Forms.NumericUpDown();
		    this.label15 = new System.Windows.Forms.Label();
		    this.num_MaxUltraBalls = new System.Windows.Forms.NumericUpDown();
		    this.num_MaxGreatBalls = new System.Windows.Forms.NumericUpDown();
		    this.label14 = new System.Windows.Forms.Label();
		    this.num_MaxPokeballs = new System.Windows.Forms.NumericUpDown();
		    this.label13 = new System.Windows.Forms.Label();
		    this.btnRealoadItems = new System.Windows.Forms.Button();
		    this.ItemsListView = new System.Windows.Forms.ListView();
		    this.chItem = new System.Windows.Forms.ColumnHeader();
		    this.chCount = new System.Windows.Forms.ColumnHeader();
		    this.chUnseen = new System.Windows.Forms.ColumnHeader();
		    this.contextMenuStripItems = new System.Windows.Forms.ContextMenuStrip(this.components);
		    this.recycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		    this.imageListItems = new System.Windows.Forms.ImageList(this.components);
		    this.label1 = new System.Windows.Forms.Label();
		    this.lblCount = new System.Windows.Forms.Label();
		    this.btnCopy = new System.Windows.Forms.Button();
		    this.btnDiscard = new System.Windows.Forms.Button();
		    this.groupBox4.SuspendLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopRevives)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopPotions)).BeginInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxRazzBerrys)).BeginInit();
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
		    // groupBox4
		    // 
		    this.groupBox4.Controls.Add(this.text_TotalItemCount);
		    this.groupBox4.Controls.Add(this.label31);
		    this.groupBox4.Controls.Add(this.label27);
		    this.groupBox4.Controls.Add(this.label25);
		    this.groupBox4.Controls.Add(this.num_MaxTopRevives);
		    this.groupBox4.Controls.Add(this.num_MaxTopPotions);
		    this.groupBox4.Controls.Add(this.label20);
		    this.groupBox4.Controls.Add(this.label19);
		    this.groupBox4.Controls.Add(this.label18);
		    this.groupBox4.Controls.Add(this.label17);
		    this.groupBox4.Controls.Add(this.label16);
		    this.groupBox4.Controls.Add(this.num_MaxRazzBerrys);
		    this.groupBox4.Controls.Add(this.num_MaxHyperPotions);
		    this.groupBox4.Controls.Add(this.num_MaxSuperPotions);
		    this.groupBox4.Controls.Add(this.num_MaxPotions);
		    this.groupBox4.Controls.Add(this.num_MaxRevives);
		    this.groupBox4.Controls.Add(this.label15);
		    this.groupBox4.Controls.Add(this.num_MaxUltraBalls);
		    this.groupBox4.Controls.Add(this.num_MaxGreatBalls);
		    this.groupBox4.Controls.Add(this.label14);
		    this.groupBox4.Controls.Add(this.num_MaxPokeballs);
		    this.groupBox4.Controls.Add(this.label13);
		    this.groupBox4.Location = new System.Drawing.Point(5, 5);
		    this.groupBox4.Margin = new System.Windows.Forms.Padding(5);
		    this.groupBox4.Name = "groupBox4";
		    this.groupBox4.Padding = new System.Windows.Forms.Padding(5);
		    this.groupBox4.Size = new System.Drawing.Size(211, 460);
		    this.groupBox4.TabIndex = 80;
		    this.groupBox4.TabStop = false;
		    this.groupBox4.Text = "Pokemon Items";
		    // 
		    // text_TotalItemCount
		    // 
		    this.text_TotalItemCount.Location = new System.Drawing.Point(137, 409);
		    this.text_TotalItemCount.Margin = new System.Windows.Forms.Padding(5);
		    this.text_TotalItemCount.Name = "text_TotalItemCount";
		    this.text_TotalItemCount.ReadOnly = true;
		    this.text_TotalItemCount.Size = new System.Drawing.Size(63, 20);
		    this.text_TotalItemCount.TabIndex = 23;
		    this.text_TotalItemCount.TabStop = false;
		    // 
		    // label31
		    // 
		    this.label31.AutoSize = true;
		    this.label31.Location = new System.Drawing.Point(42, 412);
		    this.label31.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label31.Name = "label31";
		    this.label31.Size = new System.Drawing.Size(65, 13);
		    this.label31.TabIndex = 22;
		    this.label31.Text = "Total Count:";
		    // 
		    // label27
		    // 
		    this.label27.AutoSize = true;
		    this.label27.Location = new System.Drawing.Point(12, 180);
		    this.label27.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label27.Name = "label27";
		    this.label27.Size = new System.Drawing.Size(94, 13);
		    this.label27.TabIndex = 21;
		    this.label27.Text = "Max. TopRevives:";
		    // 
		    // label25
		    // 
		    this.label25.AutoSize = true;
		    this.label25.Location = new System.Drawing.Point(12, 329);
		    this.label25.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label25.Name = "label25";
		    this.label25.Size = new System.Drawing.Size(90, 13);
		    this.label25.TabIndex = 17;
		    this.label25.Text = "Max. TopPotions:";
		    // 
		    // num_MaxTopRevives
		    // 
		    this.num_MaxTopRevives.Location = new System.Drawing.Point(138, 180);
		    this.num_MaxTopRevives.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxTopRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopRevives.Name = "num_MaxTopRevives";
		    this.num_MaxTopRevives.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxTopRevives.TabIndex = 20;
		    this.num_MaxTopRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxTopPotions
		    // 
		    this.num_MaxTopPotions.Location = new System.Drawing.Point(138, 329);
		    this.num_MaxTopPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxTopPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopPotions.Name = "num_MaxTopPotions";
		    this.num_MaxTopPotions.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxTopPotions.TabIndex = 24;
		    this.num_MaxTopPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label20
		    // 
		    this.label20.AutoSize = true;
		    this.label20.Location = new System.Drawing.Point(12, 366);
		    this.label20.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label20.Name = "label20";
		    this.label20.Size = new System.Drawing.Size(89, 13);
		    this.label20.TabIndex = 15;
		    this.label20.Text = "Max. RazzBerrys:";
		    // 
		    // label19
		    // 
		    this.label19.AutoSize = true;
		    this.label19.Location = new System.Drawing.Point(12, 292);
		    this.label19.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label19.Name = "label19";
		    this.label19.Size = new System.Drawing.Size(99, 13);
		    this.label19.TabIndex = 14;
		    this.label19.Text = "Max. HyperPotions:";
		    // 
		    // label18
		    // 
		    this.label18.AutoSize = true;
		    this.label18.Location = new System.Drawing.Point(12, 255);
		    this.label18.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label18.Name = "label18";
		    this.label18.Size = new System.Drawing.Size(99, 13);
		    this.label18.TabIndex = 13;
		    this.label18.Text = "Max. SuperPotions:";
		    // 
		    // label17
		    // 
		    this.label17.AutoSize = true;
		    this.label17.Location = new System.Drawing.Point(12, 218);
		    this.label17.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label17.Name = "label17";
		    this.label17.Size = new System.Drawing.Size(71, 13);
		    this.label17.TabIndex = 12;
		    this.label17.Text = "Max. Potions:";
		    // 
		    // label16
		    // 
		    this.label16.AutoSize = true;
		    this.label16.Location = new System.Drawing.Point(12, 142);
		    this.label16.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label16.Name = "label16";
		    this.label16.Size = new System.Drawing.Size(75, 13);
		    this.label16.TabIndex = 11;
		    this.label16.Text = "Max. Revives:";
		    // 
		    // num_MaxRazzBerrys
		    // 
		    this.num_MaxRazzBerrys.Location = new System.Drawing.Point(138, 366);
		    this.num_MaxRazzBerrys.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxRazzBerrys.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxRazzBerrys.Name = "num_MaxRazzBerrys";
		    this.num_MaxRazzBerrys.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxRazzBerrys.TabIndex = 25;
		    this.num_MaxRazzBerrys.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxHyperPotions
		    // 
		    this.num_MaxHyperPotions.Location = new System.Drawing.Point(138, 292);
		    this.num_MaxHyperPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxHyperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxHyperPotions.Name = "num_MaxHyperPotions";
		    this.num_MaxHyperPotions.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxHyperPotions.TabIndex = 23;
		    this.num_MaxHyperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxSuperPotions
		    // 
		    this.num_MaxSuperPotions.Location = new System.Drawing.Point(138, 255);
		    this.num_MaxSuperPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxSuperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxSuperPotions.Name = "num_MaxSuperPotions";
		    this.num_MaxSuperPotions.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxSuperPotions.TabIndex = 22;
		    this.num_MaxSuperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxPotions
		    // 
		    this.num_MaxPotions.Location = new System.Drawing.Point(138, 218);
		    this.num_MaxPotions.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPotions.Name = "num_MaxPotions";
		    this.num_MaxPotions.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxPotions.TabIndex = 21;
		    this.num_MaxPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxRevives
		    // 
		    this.num_MaxRevives.Location = new System.Drawing.Point(138, 142);
		    this.num_MaxRevives.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxRevives.Name = "num_MaxRevives";
		    this.num_MaxRevives.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxRevives.TabIndex = 19;
		    this.num_MaxRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label15
		    // 
		    this.label15.AutoSize = true;
		    this.label15.Location = new System.Drawing.Point(12, 105);
		    this.label15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label15.Name = "label15";
		    this.label15.Size = new System.Drawing.Size(80, 13);
		    this.label15.TabIndex = 5;
		    this.label15.Text = "Max. UltraBalls:";
		    // 
		    // num_MaxUltraBalls
		    // 
		    this.num_MaxUltraBalls.Location = new System.Drawing.Point(138, 105);
		    this.num_MaxUltraBalls.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxUltraBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxUltraBalls.Name = "num_MaxUltraBalls";
		    this.num_MaxUltraBalls.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxUltraBalls.TabIndex = 17;
		    this.num_MaxUltraBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxGreatBalls
		    // 
		    this.num_MaxGreatBalls.Location = new System.Drawing.Point(138, 68);
		    this.num_MaxGreatBalls.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxGreatBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxGreatBalls.Name = "num_MaxGreatBalls";
		    this.num_MaxGreatBalls.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxGreatBalls.TabIndex = 16;
		    this.num_MaxGreatBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label14
		    // 
		    this.label14.AutoSize = true;
		    this.label14.Location = new System.Drawing.Point(12, 68);
		    this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label14.Name = "label14";
		    this.label14.Size = new System.Drawing.Size(84, 13);
		    this.label14.TabIndex = 2;
		    this.label14.Text = "Max. GreatBalls:";
		    // 
		    // num_MaxPokeballs
		    // 
		    this.num_MaxPokeballs.Location = new System.Drawing.Point(138, 30);
		    this.num_MaxPokeballs.Margin = new System.Windows.Forms.Padding(5);
		    this.num_MaxPokeballs.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPokeballs.Name = "num_MaxPokeballs";
		    this.num_MaxPokeballs.Size = new System.Drawing.Size(64, 20);
		    this.num_MaxPokeballs.TabIndex = 15;
		    this.num_MaxPokeballs.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label13
		    // 
		    this.label13.AutoSize = true;
		    this.label13.Location = new System.Drawing.Point(12, 30);
		    this.label13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label13.Name = "label13";
		    this.label13.Size = new System.Drawing.Size(82, 13);
		    this.label13.TabIndex = 0;
		    this.label13.Text = "Max. Pokeballs:";
		    // 
		    // btnRealoadItems
		    // 
		    this.btnRealoadItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.btnRealoadItems.Location = new System.Drawing.Point(225, 469);
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
            this.chUnseen});
		    this.ItemsListView.ContextMenuStrip = this.contextMenuStripItems;
		    this.ItemsListView.FullRowSelect = true;
		    this.ItemsListView.GridLines = true;
		    this.ItemsListView.LargeImageList = this.imageListItems;
		    this.ItemsListView.Location = new System.Drawing.Point(225, 14);
		    this.ItemsListView.Margin = new System.Windows.Forms.Padding(4);
		    this.ItemsListView.Name = "ItemsListView";
		    this.ItemsListView.Size = new System.Drawing.Size(422, 449);
		    this.ItemsListView.SmallImageList = this.imageListItems;
		    this.ItemsListView.TabIndex = 78;
		    this.ItemsListView.UseCompatibleStateImageBehavior = false;
		    this.ItemsListView.View = System.Windows.Forms.View.Details;
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
		    // contextMenuStripItems
		    // 
		    this.contextMenuStripItems.ImageScalingSize = new System.Drawing.Size(20, 20);
		    this.contextMenuStripItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recycleToolStripMenuItem});
		    this.contextMenuStripItems.Name = "contextMenuStrip1";
		    this.contextMenuStripItems.Size = new System.Drawing.Size(115, 26);
		    // 
		    // recycleToolStripMenuItem
		    // 
		    this.recycleToolStripMenuItem.Name = "recycleToolStripMenuItem";
		    this.recycleToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
		    this.recycleToolStripMenuItem.Text = "Recycle";
		    this.recycleToolStripMenuItem.Click += new System.EventHandler(this.RecycleToolStripMenuItemClick);
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
		    // 
		    // label1
		    // 
		    this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.label1.AutoSize = true;
		    this.label1.Location = new System.Drawing.Point(301, 475);
		    this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.label1.Name = "label1";
		    this.label1.Size = new System.Drawing.Size(65, 13);
		    this.label1.TabIndex = 81;
		    this.label1.Text = "Total Count:";
		    // 
		    // lblCount
		    // 
		    this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
		    this.lblCount.AutoSize = true;
		    this.lblCount.Location = new System.Drawing.Point(381, 475);
		    this.lblCount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
		    this.lblCount.Name = "lblCount";
		    this.lblCount.Size = new System.Drawing.Size(13, 13);
		    this.lblCount.TabIndex = 82;
		    this.lblCount.Text = "0";
		    // 
		    // btnCopy
		    // 
		    this.btnCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
		    this.btnCopy.Location = new System.Drawing.Point(5, 470);
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
		    this.btnDiscard.Location = new System.Drawing.Point(127, 470);
		    this.btnDiscard.Margin = new System.Windows.Forms.Padding(2);
		    this.btnDiscard.Name = "btnDiscard";
		    this.btnDiscard.Size = new System.Drawing.Size(89, 29);
		    this.btnDiscard.TabIndex = 84;
		    this.btnDiscard.Text = "Discard Excess";
		    this.btnDiscard.UseVisualStyleBackColor = true;
		    // 
		    // ItemsPanel
		    // 
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		    this.Controls.Add(this.btnDiscard);
		    this.Controls.Add(this.btnCopy);
		    this.Controls.Add(this.lblCount);
		    this.Controls.Add(this.label1);
		    this.Controls.Add(this.groupBox4);
		    this.Controls.Add(this.btnRealoadItems);
		    this.Controls.Add(this.ItemsListView);
		    this.Margin = new System.Windows.Forms.Padding(4);
		    this.Name = "ItemsPanel";
		    this.Size = new System.Drawing.Size(662, 506);
		    this.groupBox4.ResumeLayout(false);
		    this.groupBox4.PerformLayout();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopRevives)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxTopPotions)).EndInit();
		    ((System.ComponentModel.ISupportInitialize)(this.num_MaxRazzBerrys)).EndInit();
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
