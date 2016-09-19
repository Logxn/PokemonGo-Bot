/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 19/09/2016
 * Time: 0:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class TeamSelect
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ImageList imageList;
		
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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Mystic", 1);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Valor", 2);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Instinct", 0);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamSelect));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.Location = new System.Drawing.Point(255, 105);
			this.buttonOk.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(55, 23);
			this.buttonOk.TabIndex = 86;
			this.buttonOk.Text = "Ok";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(314, 105);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(55, 23);
			this.buttonCancel.TabIndex = 85;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
			listViewItem1,
			listViewItem2,
			listViewItem3});
			this.listView.LargeImageList = this.imageList;
			this.listView.Location = new System.Drawing.Point(12, 12);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(356, 88);
			this.listView.TabIndex = 87;
			this.listView.UseCompatibleStateImageBehavior = false;
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "team-instinct");
			this.imageList.Images.SetKeyName(1, "team-mystic");
			this.imageList.Images.SetKeyName(2, "team-valor");
			// 
			// TeamSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(380, 139);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Name = "TeamSelect";
			this.Text = "TeamSelect";
			this.ResumeLayout(false);

		}
	}
}
