/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2016
 * Time: 18:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class Items
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnreload0;
		private System.Windows.Forms.Button lang_tr_btn2;
		private System.Windows.Forms.Button lang_ptBR_btn2;
		private System.Windows.Forms.Button lang_spain_btn2;
		private System.Windows.Forms.Button lang_de_btn_2;
		private System.Windows.Forms.Button lang_en_btn2;
		private System.Windows.Forms.TextBox statusTexbox;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripMenuItem recycleToolStripMenuItem;
		private System.Windows.Forms.ColumnHeader chItem;
		private System.Windows.Forms.ColumnHeader chCount;
		private System.Windows.Forms.ColumnHeader chUnseen;
		private System.Windows.Forms.ListView ItemsListView;
		
		/// <summary>
		/// Disposes resources used by the form.
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Items));
            this.ItemsListView = new System.Windows.Forms.ListView();
            this.chItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chUnseen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.recycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnreload0 = new System.Windows.Forms.Button();
            this.lang_tr_btn2 = new System.Windows.Forms.Button();
            this.lang_ptBR_btn2 = new System.Windows.Forms.Button();
            this.lang_spain_btn2 = new System.Windows.Forms.Button();
            this.lang_de_btn_2 = new System.Windows.Forms.Button();
            this.lang_en_btn2 = new System.Windows.Forms.Button();
            this.statusTexbox = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
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
            this.ItemsListView.ContextMenuStrip = this.contextMenuStrip1;
            this.ItemsListView.FullRowSelect = true;
            this.ItemsListView.GridLines = true;
            this.ItemsListView.Location = new System.Drawing.Point(16, 33);
            this.ItemsListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ItemsListView.Name = "ItemsListView";
            this.ItemsListView.Size = new System.Drawing.Size(324, 372);
            this.ItemsListView.TabIndex = 0;
            this.ItemsListView.UseCompatibleStateImageBehavior = false;
            this.ItemsListView.View = System.Windows.Forms.View.Details;
            // 
            // chItem
            // 
            this.chItem.Text = "Item";
            this.chItem.Width = 120;
            // 
            // chCount
            // 
            this.chCount.Text = "Count";
            // 
            // chUnseen
            // 
            this.chUnseen.Text = "Unseen";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recycleToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(135, 30);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.ContextMenuStrip1Opening);
            // 
            // recycleToolStripMenuItem
            // 
            this.recycleToolStripMenuItem.Name = "recycleToolStripMenuItem";
            this.recycleToolStripMenuItem.Size = new System.Drawing.Size(134, 26);
            this.recycleToolStripMenuItem.Text = "Recycle";
            this.recycleToolStripMenuItem.Click += new System.EventHandler(this.RecycleToolStripMenuItemClick);
            // 
            // btnreload0
            // 
            this.btnreload0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnreload0.Location = new System.Drawing.Point(16, 412);
            this.btnreload0.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnreload0.Name = "btnreload0";
            this.btnreload0.Size = new System.Drawing.Size(73, 28);
            this.btnreload0.TabIndex = 2;
            this.btnreload0.Text = "Reload";
            this.btnreload0.UseVisualStyleBackColor = true;
            this.btnreload0.Click += new System.EventHandler(this.BtnreloadClick);
            // 
            // lang_tr_btn2
            // 
            this.lang_tr_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.tr1;
            this.lang_tr_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_tr_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_tr_btn2.Location = new System.Drawing.Point(176, 7);
            this.lang_tr_btn2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lang_tr_btn2.Name = "lang_tr_btn2";
            this.lang_tr_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_tr_btn2.TabIndex = 46;
            this.lang_tr_btn2.UseVisualStyleBackColor = true;
            // 
            // lang_ptBR_btn2
            // 
            this.lang_ptBR_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.ptBR;
            this.lang_ptBR_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_ptBR_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_ptBR_btn2.Location = new System.Drawing.Point(136, 7);
            this.lang_ptBR_btn2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lang_ptBR_btn2.Name = "lang_ptBR_btn2";
            this.lang_ptBR_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_ptBR_btn2.TabIndex = 47;
            this.lang_ptBR_btn2.UseVisualStyleBackColor = true;
            // 
            // lang_spain_btn2
            // 
            this.lang_spain_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.spain;
            this.lang_spain_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_spain_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_spain_btn2.Location = new System.Drawing.Point(96, 7);
            this.lang_spain_btn2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lang_spain_btn2.Name = "lang_spain_btn2";
            this.lang_spain_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_spain_btn2.TabIndex = 45;
            this.lang_spain_btn2.UseVisualStyleBackColor = true;
            // 
            // lang_de_btn_2
            // 
            this.lang_de_btn_2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.de;
            this.lang_de_btn_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_de_btn_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_de_btn_2.Location = new System.Drawing.Point(56, 7);
            this.lang_de_btn_2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lang_de_btn_2.Name = "lang_de_btn_2";
            this.lang_de_btn_2.Size = new System.Drawing.Size(32, 18);
            this.lang_de_btn_2.TabIndex = 44;
            this.lang_de_btn_2.UseVisualStyleBackColor = true;
            // 
            // lang_en_btn2
            // 
            this.lang_en_btn2.BackgroundImage = global::PokemonGo.RocketAPI.Console.Properties.Resources.en;
            this.lang_en_btn2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lang_en_btn2.Enabled = false;
            this.lang_en_btn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lang_en_btn2.Location = new System.Drawing.Point(16, 7);
            this.lang_en_btn2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lang_en_btn2.Name = "lang_en_btn2";
            this.lang_en_btn2.Size = new System.Drawing.Size(32, 18);
            this.lang_en_btn2.TabIndex = 43;
            this.lang_en_btn2.UseVisualStyleBackColor = true;
            // 
            // statusTexbox
            // 
            this.statusTexbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusTexbox.Enabled = false;
            this.statusTexbox.Location = new System.Drawing.Point(16, 447);
            this.statusTexbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.statusTexbox.Name = "statusTexbox";
            this.statusTexbox.Size = new System.Drawing.Size(324, 22);
            this.statusTexbox.TabIndex = 48;
            // 
            // Items
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 486);
            this.Controls.Add(this.statusTexbox);
            this.Controls.Add(this.lang_tr_btn2);
            this.Controls.Add(this.lang_ptBR_btn2);
            this.Controls.Add(this.lang_spain_btn2);
            this.Controls.Add(this.lang_de_btn_2);
            this.Controls.Add(this.lang_en_btn2);
            this.Controls.Add(this.btnreload0);
            this.Controls.Add(this.ItemsListView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Items";
            this.Text = "Items";
            this.Load += new System.EventHandler(this.Items_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
