/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/09/2016
 * Time: 1:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
	partial class EggsForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private PokemonGo.RocketAPI.Console.EggsPanel eggsPanel1;
		
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
			this.eggsPanel1 = new PokemonGo.RocketAPI.Console.EggsPanel();
			this.SuspendLayout();
			// 
			// eggsPanel1
			// 
			this.eggsPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.eggsPanel1.Location = new System.Drawing.Point(2, 3);
			this.eggsPanel1.Name = "eggsPanel1";
			this.eggsPanel1.Size = new System.Drawing.Size(573, 470);
			this.eggsPanel1.TabIndex = 0;
			// 
			// EggsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(578, 477);
			this.Controls.Add(this.eggsPanel1);
			this.Name = "EggsForm";
			this.Text = "EggsForm";
			this.ResumeLayout(false);

		}
	}
}
