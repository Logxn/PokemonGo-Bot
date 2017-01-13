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
		    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeamSelect));
		    this.buttonOk = new System.Windows.Forms.Button();
		    this.buttonCancel = new System.Windows.Forms.Button();
		    this.listView = new System.Windows.Forms.ListView();
		    this.imageList = new System.Windows.Forms.ImageList(this.components);
		    this.SuspendLayout();
		    // 
		    // buttonOk
		    // 
		    resources.ApplyResources(this.buttonOk, "buttonOk");
		    this.buttonOk.Name = "buttonOk";
		    this.buttonOk.UseVisualStyleBackColor = true;
		    this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
		    // 
		    // buttonCancel
		    // 
		    resources.ApplyResources(this.buttonCancel, "buttonCancel");
		    this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		    this.buttonCancel.Name = "buttonCancel";
		    this.buttonCancel.UseVisualStyleBackColor = true;
		    // 
		    // listView
		    // 
		    resources.ApplyResources(this.listView, "listView");
		    this.listView.FullRowSelect = true;
		    this.listView.GridLines = true;
		    this.listView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listView.Items"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listView.Items1"))),
            ((System.Windows.Forms.ListViewItem)(resources.GetObject("listView.Items2")))});
		    this.listView.LargeImageList = this.imageList;
		    this.listView.MultiSelect = false;
		    this.listView.Name = "listView";
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
		    resources.ApplyResources(this, "$this");
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.CancelButton = this.buttonCancel;
		    this.Controls.Add(this.listView);
		    this.Controls.Add(this.buttonOk);
		    this.Controls.Add(this.buttonCancel);
		    this.Name = "TeamSelect";
		    this.ResumeLayout(false);

		}
	}
}
