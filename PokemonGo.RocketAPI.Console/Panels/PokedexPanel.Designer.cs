/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/08/2017
 * Time: 18:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokeMaster.Panels
{
    partial class PokedexPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnReaload;
        private System.Windows.Forms.ListView listView;
        
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
            this.btnReaload = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // btnReaload
            // 
            this.btnReaload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReaload.Location = new System.Drawing.Point(14, 168);
            this.btnReaload.Margin = new System.Windows.Forms.Padding(2);
            this.btnReaload.Name = "btnReaload";
            this.btnReaload.Size = new System.Drawing.Size(81, 29);
            this.btnReaload.TabIndex = 85;
            this.btnReaload.Text = "Reload";
            this.btnReaload.UseVisualStyleBackColor = true;
            this.btnReaload.Click += new System.EventHandler(this.btnReaload_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(14, 14);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(618, 148);
            this.listView.TabIndex = 84;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // PokedexPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnReaload);
            this.Controls.Add(this.listView);
            this.Name = "PokedexPanel";
            this.Size = new System.Drawing.Size(648, 209);
            this.ResumeLayout(false);

        }
    }
}
