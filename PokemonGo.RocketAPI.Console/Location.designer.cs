namespace PokemonGo.RocketAPI.Console
{
    partial class LocationSelect
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public PokemonGo.RocketAPI.Console.LocationPanel locationPanel1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LocationSelect));
            this.locationPanel1 = new PokemonGo.RocketAPI.Console.LocationPanel();
            this.SuspendLayout();
            // 
            // locationPanel1
            // 
            resources.ApplyResources(this.locationPanel1, "locationPanel1");
            this.locationPanel1.Name = "locationPanel1";
            // 
            // LocationSelect
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.locationPanel1);
            this.MaximizeBox = false;
            this.Name = "LocationSelect";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LocationSelect_FormClosing);
            this.ResumeLayout(false);

        }
        
		
        #endregion

    }
}