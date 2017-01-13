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
		    this.useToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		    this.imageListItems = new System.Windows.Forms.ImageList(this.components);
		    this.label1 = new System.Windows.Forms.Label();
		    this.lblCount = new System.Windows.Forms.Label();
		    this.btnCopy = new System.Windows.Forms.Button();
		    this.btnDiscard = new System.Windows.Forms.Button();
		    this.buttonUSe = new System.Windows.Forms.Button();
		    this.groupBox4.SuspendLayout();
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
		    this.groupBox4.Controls.Add(this.pictureBox9);
		    this.groupBox4.Controls.Add(this.pictureBox10);
		    this.groupBox4.Controls.Add(this.pictureBox5);
		    this.groupBox4.Controls.Add(this.pictureBox6);
		    this.groupBox4.Controls.Add(this.pictureBox7);
		    this.groupBox4.Controls.Add(this.pictureBox8);
		    this.groupBox4.Controls.Add(this.pictureBox3);
		    this.groupBox4.Controls.Add(this.pictureBox4);
		    this.groupBox4.Controls.Add(this.pictureBox2);
		    this.groupBox4.Controls.Add(this.pictureBox1);
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
		    resources.ApplyResources(this.groupBox4, "groupBox4");
		    this.groupBox4.Name = "groupBox4";
		    this.groupBox4.TabStop = false;
		    // 
		    // pictureBox9
		    // 
		    resources.ApplyResources(this.pictureBox9, "pictureBox9");
		    this.pictureBox9.Name = "pictureBox9";
		    this.pictureBox9.TabStop = false;
		    // 
		    // pictureBox10
		    // 
		    resources.ApplyResources(this.pictureBox10, "pictureBox10");
		    this.pictureBox10.Name = "pictureBox10";
		    this.pictureBox10.TabStop = false;
		    // 
		    // pictureBox5
		    // 
		    resources.ApplyResources(this.pictureBox5, "pictureBox5");
		    this.pictureBox5.Name = "pictureBox5";
		    this.pictureBox5.TabStop = false;
		    // 
		    // pictureBox6
		    // 
		    resources.ApplyResources(this.pictureBox6, "pictureBox6");
		    this.pictureBox6.Name = "pictureBox6";
		    this.pictureBox6.TabStop = false;
		    // 
		    // pictureBox7
		    // 
		    resources.ApplyResources(this.pictureBox7, "pictureBox7");
		    this.pictureBox7.Name = "pictureBox7";
		    this.pictureBox7.TabStop = false;
		    // 
		    // pictureBox8
		    // 
		    resources.ApplyResources(this.pictureBox8, "pictureBox8");
		    this.pictureBox8.Name = "pictureBox8";
		    this.pictureBox8.TabStop = false;
		    // 
		    // pictureBox3
		    // 
		    resources.ApplyResources(this.pictureBox3, "pictureBox3");
		    this.pictureBox3.Name = "pictureBox3";
		    this.pictureBox3.TabStop = false;
		    // 
		    // pictureBox4
		    // 
		    resources.ApplyResources(this.pictureBox4, "pictureBox4");
		    this.pictureBox4.Name = "pictureBox4";
		    this.pictureBox4.TabStop = false;
		    // 
		    // pictureBox2
		    // 
		    resources.ApplyResources(this.pictureBox2, "pictureBox2");
		    this.pictureBox2.Name = "pictureBox2";
		    this.pictureBox2.TabStop = false;
		    // 
		    // pictureBox1
		    // 
		    resources.ApplyResources(this.pictureBox1, "pictureBox1");
		    this.pictureBox1.Name = "pictureBox1";
		    this.pictureBox1.TabStop = false;
		    // 
		    // text_TotalItemCount
		    // 
		    this.text_TotalItemCount.BorderStyle = System.Windows.Forms.BorderStyle.None;
		    resources.ApplyResources(this.text_TotalItemCount, "text_TotalItemCount");
		    this.text_TotalItemCount.Name = "text_TotalItemCount";
		    this.text_TotalItemCount.ReadOnly = true;
		    this.text_TotalItemCount.TabStop = false;
		    // 
		    // label31
		    // 
		    resources.ApplyResources(this.label31, "label31");
		    this.label31.Name = "label31";
		    // 
		    // label27
		    // 
		    resources.ApplyResources(this.label27, "label27");
		    this.label27.Name = "label27";
		    // 
		    // label25
		    // 
		    resources.ApplyResources(this.label25, "label25");
		    this.label25.Name = "label25";
		    // 
		    // num_MaxTopRevives
		    // 
		    resources.ApplyResources(this.num_MaxTopRevives, "num_MaxTopRevives");
		    this.num_MaxTopRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopRevives.Name = "num_MaxTopRevives";
		    this.num_MaxTopRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxTopPotions
		    // 
		    resources.ApplyResources(this.num_MaxTopPotions, "num_MaxTopPotions");
		    this.num_MaxTopPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxTopPotions.Name = "num_MaxTopPotions";
		    this.num_MaxTopPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label20
		    // 
		    resources.ApplyResources(this.label20, "label20");
		    this.label20.Name = "label20";
		    // 
		    // label19
		    // 
		    resources.ApplyResources(this.label19, "label19");
		    this.label19.Name = "label19";
		    // 
		    // label18
		    // 
		    resources.ApplyResources(this.label18, "label18");
		    this.label18.Name = "label18";
		    // 
		    // label17
		    // 
		    resources.ApplyResources(this.label17, "label17");
		    this.label17.Name = "label17";
		    // 
		    // label16
		    // 
		    resources.ApplyResources(this.label16, "label16");
		    this.label16.Name = "label16";
		    // 
		    // num_MaxRazzBerrys
		    // 
		    resources.ApplyResources(this.num_MaxRazzBerrys, "num_MaxRazzBerrys");
		    this.num_MaxRazzBerrys.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxRazzBerrys.Name = "num_MaxRazzBerrys";
		    this.num_MaxRazzBerrys.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxHyperPotions
		    // 
		    resources.ApplyResources(this.num_MaxHyperPotions, "num_MaxHyperPotions");
		    this.num_MaxHyperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxHyperPotions.Name = "num_MaxHyperPotions";
		    this.num_MaxHyperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxSuperPotions
		    // 
		    resources.ApplyResources(this.num_MaxSuperPotions, "num_MaxSuperPotions");
		    this.num_MaxSuperPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxSuperPotions.Name = "num_MaxSuperPotions";
		    this.num_MaxSuperPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxPotions
		    // 
		    resources.ApplyResources(this.num_MaxPotions, "num_MaxPotions");
		    this.num_MaxPotions.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPotions.Name = "num_MaxPotions";
		    this.num_MaxPotions.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxRevives
		    // 
		    resources.ApplyResources(this.num_MaxRevives, "num_MaxRevives");
		    this.num_MaxRevives.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxRevives.Name = "num_MaxRevives";
		    this.num_MaxRevives.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label15
		    // 
		    resources.ApplyResources(this.label15, "label15");
		    this.label15.Name = "label15";
		    // 
		    // num_MaxUltraBalls
		    // 
		    resources.ApplyResources(this.num_MaxUltraBalls, "num_MaxUltraBalls");
		    this.num_MaxUltraBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxUltraBalls.Name = "num_MaxUltraBalls";
		    this.num_MaxUltraBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // num_MaxGreatBalls
		    // 
		    resources.ApplyResources(this.num_MaxGreatBalls, "num_MaxGreatBalls");
		    this.num_MaxGreatBalls.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxGreatBalls.Name = "num_MaxGreatBalls";
		    this.num_MaxGreatBalls.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label14
		    // 
		    resources.ApplyResources(this.label14, "label14");
		    this.label14.Name = "label14";
		    // 
		    // num_MaxPokeballs
		    // 
		    resources.ApplyResources(this.num_MaxPokeballs, "num_MaxPokeballs");
		    this.num_MaxPokeballs.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
		    this.num_MaxPokeballs.Name = "num_MaxPokeballs";
		    this.num_MaxPokeballs.ValueChanged += new System.EventHandler(this.num_Max);
		    // 
		    // label13
		    // 
		    resources.ApplyResources(this.label13, "label13");
		    this.label13.Name = "label13";
		    // 
		    // btnRealoadItems
		    // 
		    resources.ApplyResources(this.btnRealoadItems, "btnRealoadItems");
		    this.btnRealoadItems.Name = "btnRealoadItems";
		    this.btnRealoadItems.UseVisualStyleBackColor = true;
		    this.btnRealoadItems.Click += new System.EventHandler(this.BtnRealoadItemsClick);
		    // 
		    // ItemsListView
		    // 
		    resources.ApplyResources(this.ItemsListView, "ItemsListView");
		    this.ItemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chItem,
            this.chCount,
            this.chUnseen});
		    this.ItemsListView.ContextMenuStrip = this.contextMenuStripItems;
		    this.ItemsListView.FullRowSelect = true;
		    this.ItemsListView.GridLines = true;
		    this.ItemsListView.LargeImageList = this.imageListItems;
		    this.ItemsListView.Name = "ItemsListView";
		    this.ItemsListView.SmallImageList = this.imageListItems;
		    this.ItemsListView.UseCompatibleStateImageBehavior = false;
		    this.ItemsListView.View = System.Windows.Forms.View.Details;
		    // 
		    // chItem
		    // 
		    resources.ApplyResources(this.chItem, "chItem");
		    // 
		    // chCount
		    // 
		    resources.ApplyResources(this.chCount, "chCount");
		    // 
		    // chUnseen
		    // 
		    resources.ApplyResources(this.chUnseen, "chUnseen");
		    // 
		    // contextMenuStripItems
		    // 
		    this.contextMenuStripItems.ImageScalingSize = new System.Drawing.Size(20, 20);
		    this.contextMenuStripItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recycleToolStripMenuItem,
            this.useToolStripMenuItem});
		    this.contextMenuStripItems.Name = "contextMenuStrip1";
		    resources.ApplyResources(this.contextMenuStripItems, "contextMenuStripItems");
		    // 
		    // recycleToolStripMenuItem
		    // 
		    this.recycleToolStripMenuItem.Name = "recycleToolStripMenuItem";
		    resources.ApplyResources(this.recycleToolStripMenuItem, "recycleToolStripMenuItem");
		    this.recycleToolStripMenuItem.Click += new System.EventHandler(this.RecycleToolStripMenuItemClick);
		    // 
		    // useToolStripMenuItem
		    // 
		    this.useToolStripMenuItem.Name = "useToolStripMenuItem";
		    resources.ApplyResources(this.useToolStripMenuItem, "useToolStripMenuItem");
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
		    // 
		    // label1
		    // 
		    resources.ApplyResources(this.label1, "label1");
		    this.label1.Name = "label1";
		    // 
		    // lblCount
		    // 
		    resources.ApplyResources(this.lblCount, "lblCount");
		    this.lblCount.Name = "lblCount";
		    // 
		    // btnCopy
		    // 
		    resources.ApplyResources(this.btnCopy, "btnCopy");
		    this.btnCopy.Name = "btnCopy";
		    this.btnCopy.UseVisualStyleBackColor = true;
		    this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
		    // 
		    // btnDiscard
		    // 
		    resources.ApplyResources(this.btnDiscard, "btnDiscard");
		    this.btnDiscard.Name = "btnDiscard";
		    this.btnDiscard.UseVisualStyleBackColor = true;
		    this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
		    // 
		    // buttonUSe
		    // 
		    resources.ApplyResources(this.buttonUSe, "buttonUSe");
		    this.buttonUSe.Name = "buttonUSe";
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
		    this.Controls.Add(this.groupBox4);
		    this.Controls.Add(this.btnRealoadItems);
		    this.Controls.Add(this.ItemsListView);
		    resources.ApplyResources(this, "$this");
		    this.Name = "ItemsPanel";
		    this.groupBox4.ResumeLayout(false);
		    this.groupBox4.PerformLayout();
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
