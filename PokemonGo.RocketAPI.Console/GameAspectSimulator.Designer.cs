/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 23/09/2016
 * Time: 23:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class GameAspectSimulator
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private PokemonGo.RocketAPI.Console.LocationPanel locationPanel1;
        private System.Windows.Forms.PictureBox btnPicMenu;
        private System.Windows.Forms.PictureBox btnPicPokes;
        private System.Windows.Forms.PictureBox btnPicEggs;
        private System.Windows.Forms.PictureBox btnPicItems;
        private System.Windows.Forms.PictureBox btnPicConfig;
        private System.Windows.Forms.PictureBox btnPicClose;
        private System.Windows.Forms.PictureBox btnPicSnipe;
        private System.Windows.Forms.PictureBox btnPicProfile;
        
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameAspectSimulator));
            this.locationPanel1 = new PokemonGo.RocketAPI.Console.LocationPanel();
            this.btnPicMenu = new System.Windows.Forms.PictureBox();
            this.btnPicPokes = new System.Windows.Forms.PictureBox();
            this.btnPicEggs = new System.Windows.Forms.PictureBox();
            this.btnPicItems = new System.Windows.Forms.PictureBox();
            this.btnPicConfig = new System.Windows.Forms.PictureBox();
            this.btnPicClose = new System.Windows.Forms.PictureBox();
            this.btnPicSnipe = new System.Windows.Forms.PictureBox();
            this.btnPicProfile = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicPokes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicEggs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicClose)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicSnipe)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicProfile)).BeginInit();
            this.SuspendLayout();
            // 
            // locationPanel1
            // 
            resources.ApplyResources(this.locationPanel1, "locationPanel1");
            this.locationPanel1.Name = "locationPanel1";
            // 
            // btnPicMenu
            // 
            resources.ApplyResources(this.btnPicMenu, "btnPicMenu");
            this.btnPicMenu.Name = "btnPicMenu";
            this.btnPicMenu.TabStop = false;
            this.btnPicMenu.Click += new System.EventHandler(this.btnPicMenu_Click);
            // 
            // btnPicPokes
            // 
            resources.ApplyResources(this.btnPicPokes, "btnPicPokes");
            this.btnPicPokes.BackColor = System.Drawing.Color.Transparent;
            this.btnPicPokes.Name = "btnPicPokes";
            this.btnPicPokes.TabStop = false;
            this.btnPicPokes.Click += new System.EventHandler(this.btnPicPokes_Click);
            // 
            // btnPicEggs
            // 
            resources.ApplyResources(this.btnPicEggs, "btnPicEggs");
            this.btnPicEggs.BackColor = System.Drawing.Color.Transparent;
            this.btnPicEggs.Name = "btnPicEggs";
            this.btnPicEggs.TabStop = false;
            this.btnPicEggs.Click += new System.EventHandler(this.btnPicEggs_Click);
            // 
            // btnPicItems
            // 
            resources.ApplyResources(this.btnPicItems, "btnPicItems");
            this.btnPicItems.BackColor = System.Drawing.Color.Transparent;
            this.btnPicItems.Name = "btnPicItems";
            this.btnPicItems.TabStop = false;
            this.btnPicItems.Click += new System.EventHandler(this.btnPicItems_Click);
            // 
            // btnPicConfig
            // 
            resources.ApplyResources(this.btnPicConfig, "btnPicConfig");
            this.btnPicConfig.BackColor = System.Drawing.Color.Transparent;
            this.btnPicConfig.Name = "btnPicConfig";
            this.btnPicConfig.TabStop = false;
            this.btnPicConfig.Click += new System.EventHandler(this.btnPicConfig_Click);
            // 
            // btnPicClose
            // 
            resources.ApplyResources(this.btnPicClose, "btnPicClose");
            this.btnPicClose.Name = "btnPicClose";
            this.btnPicClose.TabStop = false;
            this.btnPicClose.Click += new System.EventHandler(this.btnPicClose_Click);
            // 
            // btnPicSnipe
            // 
            resources.ApplyResources(this.btnPicSnipe, "btnPicSnipe");
            this.btnPicSnipe.BackColor = System.Drawing.Color.Transparent;
            this.btnPicSnipe.Name = "btnPicSnipe";
            this.btnPicSnipe.TabStop = false;
            this.btnPicSnipe.Click += new System.EventHandler(this.btnSnipe_Click);
            // 
            // btnPicProfile
            // 
            resources.ApplyResources(this.btnPicProfile, "btnPicProfile");
            this.btnPicProfile.Name = "btnPicProfile";
            this.btnPicProfile.TabStop = false;
            this.btnPicProfile.Click += new System.EventHandler(this.btnPicProfile_Click);
            // 
            // GameAspectSimulator
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnPicConfig);
            this.Controls.Add(this.btnPicSnipe);
            this.Controls.Add(this.btnPicPokes);
            this.Controls.Add(this.btnPicEggs);
            this.Controls.Add(this.btnPicItems);
            this.Controls.Add(this.btnPicProfile);
            this.Controls.Add(this.btnPicClose);
            this.Controls.Add(this.btnPicMenu);
            this.Controls.Add(this.locationPanel1);
            this.Name = "GameAspectSimulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.This_Close);
            ((System.ComponentModel.ISupportInitialize)(this.btnPicMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicPokes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicEggs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicClose)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicSnipe)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPicProfile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
    }
    
}
