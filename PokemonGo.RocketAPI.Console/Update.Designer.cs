namespace PokemonGo.RocketAPI.Console
{
    partial class Update
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Update));
            this.progress = new System.Windows.Forms.ProgressBar();
            this.status = new System.Windows.Forms.TextBox();
            this.comp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progress
            // 
            this.progress.Location = new System.Drawing.Point(12, 12);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(516, 23);
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progress.TabIndex = 0;
            // 
            // status
            // 
            this.status.Enabled = false;
            this.status.Location = new System.Drawing.Point(12, 42);
            this.status.Name = "status";
            this.status.ReadOnly = true;
            this.status.Size = new System.Drawing.Size(516, 20);
            this.status.TabIndex = 1;
            this.status.Text = "Status...";
            // 
            // comp
            // 
            this.comp.AutoSize = true;
            this.comp.Location = new System.Drawing.Point(9, 70);
            this.comp.Name = "comp";
            this.comp.Size = new System.Drawing.Size(63, 13);
            this.comp.TabIndex = 2;
            this.comp.Text = "Completed: ";
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 92);
            this.Controls.Add(this.comp);
            this.Controls.Add(this.status);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Update";
            this.Text = "Update";
            this.Load += new System.EventHandler(this.Update_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.TextBox status;
        private System.Windows.Forms.Label comp;
    }
}
