/*
 * Created by SharpDevelop.
 * User: Xelwon
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
		    System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntegerInput));
		    this.confirmation = new System.Windows.Forms.Button();
		    this.amount = new System.Windows.Forms.NumericUpDown();
		    ((System.ComponentModel.ISupportInitialize)(this.amount)).BeginInit();
		    this.SuspendLayout();
		    // 
		    // confirmation
		    // 
		    resources.ApplyResources(this.confirmation, "confirmation");
		    this.confirmation.DialogResult = System.Windows.Forms.DialogResult.OK;
		    this.confirmation.Name = "confirmation";
		    this.confirmation.UseVisualStyleBackColor = true;
		    this.confirmation.Click += new System.EventHandler(this.ConfirmationClick);
		    // 
		    // amount
		    // 
		    resources.ApplyResources(this.amount, "amount");
		    this.amount.Name = "amount";
		    // 
		    // IntegerInput
		    // 
		    this.AcceptButton = this.confirmation;
		    resources.ApplyResources(this, "$this");
		    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		    this.Controls.Add(this.amount);
		    this.Controls.Add(this.confirmation);
		    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
		    this.Name = "IntegerInput";
		    ((System.ComponentModel.ISupportInitialize)(this.amount)).EndInit();
		    this.ResumeLayout(false);

		}
	}
}
