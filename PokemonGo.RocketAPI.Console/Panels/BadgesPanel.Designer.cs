/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 05/08/2017
 * Time: 1:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokeMaster.Panels
{
    partial class BadgesPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnReaload;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader chType;
        private System.Windows.Forms.ColumnHeader chRank;
        private System.Windows.Forms.ColumnHeader chValue;
        private System.Windows.Forms.ColumnHeader chNext;
        
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
            this.chType = new System.Windows.Forms.ColumnHeader();
            this.chRank = new System.Windows.Forms.ColumnHeader();
            this.chValue = new System.Windows.Forms.ColumnHeader();
            this.chNext = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // btnReaload
            // 
            this.btnReaload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReaload.Location = new System.Drawing.Point(4, 106);
            this.btnReaload.Margin = new System.Windows.Forms.Padding(2);
            this.btnReaload.Name = "btnReaload";
            this.btnReaload.Size = new System.Drawing.Size(81, 29);
            this.btnReaload.TabIndex = 83;
            this.btnReaload.Text = "Reload";
            this.btnReaload.UseVisualStyleBackColor = true;
            this.btnReaload.Click += new System.EventHandler(this.btnReaload_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chType,
            this.chRank,
            this.chValue,
            this.chNext});
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(4, 4);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(526, 96);
            this.listView.TabIndex = 82;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 200;
            // 
            // chRank
            // 
            this.chRank.Text = "Rank";
            this.chRank.Width = 100;
            // 
            // chValue
            // 
            this.chValue.Text = "Value";
            this.chValue.Width = 100;
            // 
            // chNext
            // 
            this.chNext.Text = "Next";
            this.chNext.Width = 100;
            // 
            // BadgesPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnReaload);
            this.Controls.Add(this.listView);
            this.Name = "BadgesPanel";
            this.Size = new System.Drawing.Size(534, 138);
            this.ResumeLayout(false);

        }
    }
}
