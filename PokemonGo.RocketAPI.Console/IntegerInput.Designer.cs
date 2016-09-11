﻿/*
 * Created by SharpDevelop.
 * User: DCRax
 * Date: 11/09/2016
 * Time: 3:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class IntegerInput
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button confirmation;
		private System.Windows.Forms.NumericUpDown amount;
		
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
			this.confirmation = new System.Windows.Forms.Button();
			this.amount = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.amount)).BeginInit();
			this.SuspendLayout();
			// 
			// confirmation
			// 
			this.confirmation.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.confirmation.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.confirmation.Location = new System.Drawing.Point(127, 15);
			this.confirmation.Name = "confirmation";
			this.confirmation.Size = new System.Drawing.Size(40, 22);
			this.confirmation.TabIndex = 0;
			this.confirmation.Text = "Ok";
			this.confirmation.UseVisualStyleBackColor = true;
			this.confirmation.Click += new System.EventHandler(this.ConfirmationClick);
			// 
			// amount
			// 
			this.amount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.amount.Location = new System.Drawing.Point(12, 16);
			this.amount.Name = "amount";
			this.amount.Size = new System.Drawing.Size(109, 20);
			this.amount.TabIndex = 1;
			// 
			// IntegerInput
			// 
			this.AcceptButton = this.confirmation;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(179, 52);
			this.Controls.Add(this.amount);
			this.Controls.Add(this.confirmation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "IntegerInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "IntegerInput";
			((System.ComponentModel.ISupportInitialize)(this.amount)).EndInit();
			this.ResumeLayout(false);

		}
	}
}
