/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 16/09/2016
 * Time: 23:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class EggsPanel
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnReaload;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader chKmWalkedStart;
		private System.Windows.Forms.ColumnHeader chKmWalkedTarget;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem recycleToolStripMenuItem;
		private System.Windows.Forms.ImageList imageList;
		private System.Windows.Forms.ColumnHeader chPokemon;
		private System.Windows.Forms.ColumnHeader chIVs;
		private System.Windows.Forms.ColumnHeader chCreationTime;
		private System.Windows.Forms.ColumnHeader chMove1;
		private System.Windows.Forms.ColumnHeader chMove2;
		private System.Windows.Forms.ColumnHeader chIncubator;
		
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EggsPanel));
			this.btnReaload = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.chKmWalkedStart = new System.Windows.Forms.ColumnHeader();
			this.chKmWalkedTarget = new System.Windows.Forms.ColumnHeader();
			this.chPokemon = new System.Windows.Forms.ColumnHeader();
			this.chIVs = new System.Windows.Forms.ColumnHeader();
			this.chCreationTime = new System.Windows.Forms.ColumnHeader();
			this.chMove1 = new System.Windows.Forms.ColumnHeader();
			this.chMove2 = new System.Windows.Forms.ColumnHeader();
			this.chIncubator = new System.Windows.Forms.ColumnHeader();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.recycleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.contextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnReaload
			// 
			this.btnReaload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnReaload.Location = new System.Drawing.Point(3, 78);
			this.btnReaload.Margin = new System.Windows.Forms.Padding(2);
			this.btnReaload.Name = "btnReaload";
			this.btnReaload.Size = new System.Drawing.Size(55, 23);
			this.btnReaload.TabIndex = 81;
			this.btnReaload.Text = "Reload";
			this.btnReaload.UseVisualStyleBackColor = true;
			this.btnReaload.Click += new System.EventHandler(this.BtnRealoadClick);
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.chKmWalkedStart,
			this.chKmWalkedTarget,
			this.chPokemon,
			this.chIVs,
			this.chCreationTime,
			this.chMove1,
			this.chMove2,
			this.chIncubator});
			this.listView.ContextMenuStrip = this.contextMenuStrip;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.LargeImageList = this.imageList;
			this.listView.Location = new System.Drawing.Point(3, 3);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(618, 71);
			this.listView.SmallImageList = this.imageList;
			this.listView.TabIndex = 80;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// chKmWalkedStart
			// 
			this.chKmWalkedStart.Text = "Kms Left";
			// 
			// chKmWalkedTarget
			// 
			this.chKmWalkedTarget.Text = "Kms";
			// 
			// chPokemon
			// 
			this.chPokemon.Text = "Pokemon";
			this.chPokemon.Width = 100;
			// 
			// chIVs
			// 
			this.chIVs.Text = "IVs";
			this.chIVs.Width = 100;
			// 
			// chCreationTime
			// 
			this.chCreationTime.Text = "Creation Time";
			this.chCreationTime.Width = 100;
			// 
			// chMove1
			// 
			this.chMove1.Text = "Move 1";
			// 
			// chMove2
			// 
			this.chMove2.Text = "Move 2";
			// 
			// chIncubator
			// 
			this.chIncubator.Text = "Incubator";
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.recycleToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip1";
			this.contextMenuStrip.Size = new System.Drawing.Size(121, 26);
			// 
			// recycleToolStripMenuItem
			// 
			this.recycleToolStripMenuItem.Name = "recycleToolStripMenuItem";
			this.recycleToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
			this.recycleToolStripMenuItem.Text = "Incubate";
			this.recycleToolStripMenuItem.Click += new System.EventHandler(this.IncubateToolStripMenuItemClick);
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "bincegg");
			this.imageList.Images.SetKeyName(1, "unincegg");
			this.imageList.Images.SetKeyName(2, "egg");
			// 
			// EggsPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.btnReaload);
			this.Controls.Add(this.listView);
			this.Name = "EggsPanel";
			this.Size = new System.Drawing.Size(624, 104);
			this.contextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
