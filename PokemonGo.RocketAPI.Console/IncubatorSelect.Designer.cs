/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/09/2016
 * Time: 16:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class IncubatorSelect
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader chStartKmWalked;
		private System.Windows.Forms.ColumnHeader chTargetKmWalked;
		private System.Windows.Forms.ColumnHeader chPokemon;
		private System.Windows.Forms.ColumnHeader chIncubatorType;
		private System.Windows.Forms.ColumnHeader chUsesRemaining;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.ColumnHeader chID;
		private System.Windows.Forms.ColumnHeader chItemID;
		
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
		    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IncubatorSelect));
		    this.buttonCancel = new System.Windows.Forms.Button();
		    this.listView = new System.Windows.Forms.ListView();
		    this.chID = new System.Windows.Forms.ColumnHeader();
		    this.chItemID = new System.Windows.Forms.ColumnHeader();
		    this.chUsesRemaining = new System.Windows.Forms.ColumnHeader();
		    this.chIncubatorType = new System.Windows.Forms.ColumnHeader();
		    this.chStartKmWalked = new System.Windows.Forms.ColumnHeader();
		    this.chTargetKmWalked = new System.Windows.Forms.ColumnHeader();
		    this.chPokemon = new System.Windows.Forms.ColumnHeader();
		    this.buttonOk = new System.Windows.Forms.Button();
		    this.SuspendLayout();
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
		    this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chID,
            this.chItemID,
            this.chUsesRemaining,
            this.chIncubatorType,
            this.chStartKmWalked,
            this.chTargetKmWalked,
            this.chPokemon});
		    this.listView.FullRowSelect = true;
		    this.listView.GridLines = true;
		    this.listView.Name = "listView";
		    this.listView.UseCompatibleStateImageBehavior = false;
		    this.listView.View = System.Windows.Forms.View.Details;
		    // 
		    // chID
		    // 
		    resources.ApplyResources(this.chID, "chID");
		    // 
		    // chItemID
		    // 
		    resources.ApplyResources(this.chItemID, "chItemID");
		    // 
		    // chUsesRemaining
		    // 
		    resources.ApplyResources(this.chUsesRemaining, "chUsesRemaining");
		    // 
		    // chIncubatorType
		    // 
		    resources.ApplyResources(this.chIncubatorType, "chIncubatorType");
		    // 
		    // chStartKmWalked
		    // 
		    resources.ApplyResources(this.chStartKmWalked, "chStartKmWalked");
		    // 
		    // chTargetKmWalked
		    // 
		    resources.ApplyResources(this.chTargetKmWalked, "chTargetKmWalked");
		    // 
		    // chPokemon
		    // 
		    resources.ApplyResources(this.chPokemon, "chPokemon");
		    // 
		    // buttonOk
		    // 
		    resources.ApplyResources(this.buttonOk, "buttonOk");
		    this.buttonOk.Name = "buttonOk";
		    this.buttonOk.UseVisualStyleBackColor = true;
		    this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
		    // 
		    // IncubatorSelect
		    // 
		    resources.ApplyResources(this, "$this");
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.CancelButton = this.buttonCancel;
		    this.Controls.Add(this.buttonOk);
		    this.Controls.Add(this.buttonCancel);
		    this.Controls.Add(this.listView);
		    this.Name = "IncubatorSelect";
		    this.ResumeLayout(false);

		}
	}
}
