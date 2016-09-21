/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 21/09/2016
 * Time: 22:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class FormChanges
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private PokemonGo.RocketAPI.Console.ChangesPanel changesPanel1;
        
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
            this.changesPanel1 = new PokemonGo.RocketAPI.Console.ChangesPanel();
            this.SuspendLayout();
            // 
            // changesPanel1
            // 
            this.changesPanel1.Location = new System.Drawing.Point(0, 0);
            this.changesPanel1.Name = "changesPanel1";
            this.changesPanel1.Size = new System.Drawing.Size(690, 348);
            this.changesPanel1.TabIndex = 0;
            // 
            // FormChanges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 360);
            this.Controls.Add(this.changesPanel1);
            this.Name = "FormChanges";
            this.Text = "FormChanges";
            this.ResumeLayout(false);

        }
    }
}
