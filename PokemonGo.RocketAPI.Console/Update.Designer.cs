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
            resources.ApplyResources(this.progress, "progress");
            this.progress.Name = "progress";
            this.progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // status
            // 
            resources.ApplyResources(this.status, "status");
            this.status.Name = "status";
            this.status.ReadOnly = true;
            // 
            // comp
            // 
            resources.ApplyResources(this.comp, "comp");
            this.comp.Name = "comp";
            // 
            // Update
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comp);
            this.Controls.Add(this.status);
            this.Controls.Add(this.progress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Update";
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
