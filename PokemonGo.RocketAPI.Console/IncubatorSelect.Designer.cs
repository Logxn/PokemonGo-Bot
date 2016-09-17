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
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(596, 282);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(55, 23);
			this.buttonCancel.TabIndex = 83;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// listView
			// 
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
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
			this.listView.Location = new System.Drawing.Point(12, 12);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(639, 265);
			this.listView.TabIndex = 82;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			// 
			// chID
			// 
			this.chID.Text = "ID";
			// 
			// chItemID
			// 
			this.chItemID.Text = "Item ID";
			this.chItemID.Width = 100;
			// 
			// chUsesRemaining
			// 
			this.chUsesRemaining.Text = "Uses Remaining";
			this.chUsesRemaining.Width = 100;
			// 
			// chIncubatorType
			// 
			this.chIncubatorType.Text = "Incubator Type";
			this.chIncubatorType.Width = 100;
			// 
			// chStartKmWalked
			// 
			this.chStartKmWalked.Text = "Start Km Walked";
			this.chStartKmWalked.Width = 100;
			// 
			// chTargetKmWalked
			// 
			this.chTargetKmWalked.Text = "Target Km Walked";
			this.chTargetKmWalked.Width = 100;
			// 
			// chPokemon
			// 
			this.chPokemon.Text = "Pokemon";
			this.chPokemon.Width = 100;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(537, 282);
			this.buttonOk.Margin = new System.Windows.Forms.Padding(2);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(55, 23);
			this.buttonOk.TabIndex = 84;
			this.buttonOk.Text = "Ok";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
			// 
			// IncubatorSelect
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(663, 316);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.listView);
			this.Name = "IncubatorSelect";
			this.Text = "Select Incubator";
			this.ResumeLayout(false);

		}
	}
}
