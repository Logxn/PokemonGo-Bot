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
            this.locationPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.locationPanel1.Location = new System.Drawing.Point(0, 1);
            this.locationPanel1.Name = "locationPanel1";
            this.locationPanel1.Size = new System.Drawing.Size(787, 553);
            this.locationPanel1.TabIndex = 0;
            // 
            // btnPicMenu
            // 
            this.btnPicMenu.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPicMenu.Image = ((System.Drawing.Image)(resources.GetObject("btnPicMenu.Image")));
            this.btnPicMenu.Location = new System.Drawing.Point(384, 550);
            this.btnPicMenu.Name = "btnPicMenu";
            this.btnPicMenu.Size = new System.Drawing.Size(35, 35);
            this.btnPicMenu.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.btnPicMenu.TabIndex = 1;
            this.btnPicMenu.TabStop = false;
            this.btnPicMenu.Click += new System.EventHandler(this.btnPicMenu_Click);
            // 
            // btnPicPokes
            // 
            this.btnPicPokes.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPicPokes.BackColor = System.Drawing.Color.Transparent;
            this.btnPicPokes.Image = ((System.Drawing.Image)(resources.GetObject("btnPicPokes.Image")));
            this.btnPicPokes.Location = new System.Drawing.Point(331, 492);
            this.btnPicPokes.Name = "btnPicPokes";
            this.btnPicPokes.Size = new System.Drawing.Size(44, 48);
            this.btnPicPokes.TabIndex = 2;
            this.btnPicPokes.TabStop = false;
            this.btnPicPokes.Visible = false;
            this.btnPicPokes.Click += new System.EventHandler(this.btnPicPokes_Click);
            // 
            // btnPicEggs
            // 
            this.btnPicEggs.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPicEggs.BackColor = System.Drawing.Color.Transparent;
            this.btnPicEggs.Image = ((System.Drawing.Image)(resources.GetObject("btnPicEggs.Image")));
            this.btnPicEggs.Location = new System.Drawing.Point(377, 438);
            this.btnPicEggs.Name = "btnPicEggs";
            this.btnPicEggs.Size = new System.Drawing.Size(44, 48);
            this.btnPicEggs.TabIndex = 3;
            this.btnPicEggs.TabStop = false;
            this.btnPicEggs.Visible = false;
            this.btnPicEggs.Click += new System.EventHandler(this.btnPicEggs_Click);
            // 
            // btnPicItems
            // 
            this.btnPicItems.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPicItems.BackColor = System.Drawing.Color.Transparent;
            this.btnPicItems.Image = ((System.Drawing.Image)(resources.GetObject("btnPicItems.Image")));
            this.btnPicItems.Location = new System.Drawing.Point(427, 492);
            this.btnPicItems.Name = "btnPicItems";
            this.btnPicItems.Size = new System.Drawing.Size(44, 48);
            this.btnPicItems.TabIndex = 4;
            this.btnPicItems.TabStop = false;
            this.btnPicItems.Visible = false;
            this.btnPicItems.Click += new System.EventHandler(this.btnPicItems_Click);
            // 
            // btnPicConfig
            // 
            this.btnPicConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPicConfig.BackColor = System.Drawing.Color.Transparent;
            this.btnPicConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnPicConfig.Image")));
            this.btnPicConfig.Location = new System.Drawing.Point(721, 10);
            this.btnPicConfig.Name = "btnPicConfig";
            this.btnPicConfig.Size = new System.Drawing.Size(50, 50);
            this.btnPicConfig.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnPicConfig.TabIndex = 5;
            this.btnPicConfig.TabStop = false;
            this.btnPicConfig.Visible = false;
            this.btnPicConfig.Click += new System.EventHandler(this.btnPicConfig_Click);
            // 
            // btnPicClose
            // 
            this.btnPicClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPicClose.Image = ((System.Drawing.Image)(resources.GetObject("btnPicClose.Image")));
            this.btnPicClose.Location = new System.Drawing.Point(388, 549);
            this.btnPicClose.Name = "btnPicClose";
            this.btnPicClose.Size = new System.Drawing.Size(40, 36);
            this.btnPicClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnPicClose.TabIndex = 6;
            this.btnPicClose.TabStop = false;
            this.btnPicClose.Visible = false;
            this.btnPicClose.Click += new System.EventHandler(this.btnPicClose_Click);
            // 
            // btnPicSnipe
            // 
            this.btnPicSnipe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPicSnipe.BackColor = System.Drawing.Color.Transparent;
            this.btnPicSnipe.Image = ((System.Drawing.Image)(resources.GetObject("btnPicSnipe.Image")));
            this.btnPicSnipe.Location = new System.Drawing.Point(726, 66);
            this.btnPicSnipe.Name = "btnPicSnipe";
            this.btnPicSnipe.Size = new System.Drawing.Size(40, 40);
            this.btnPicSnipe.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnPicSnipe.TabIndex = 8;
            this.btnPicSnipe.TabStop = false;
            this.btnPicSnipe.Visible = false;
            this.btnPicSnipe.Click += new System.EventHandler(this.btnSnipe_Click);
            // 
            // btnPicProfile
            // 
            this.btnPicProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPicProfile.Image = ((System.Drawing.Image)(resources.GetObject("btnPicProfile.Image")));
            this.btnPicProfile.Location = new System.Drawing.Point(13, 545);
            this.btnPicProfile.Name = "btnPicProfile";
            this.btnPicProfile.Size = new System.Drawing.Size(49, 40);
            this.btnPicProfile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.btnPicProfile.TabIndex = 9;
            this.btnPicProfile.TabStop = false;
            this.btnPicProfile.Click += new System.EventHandler(this.btnPicProfile_Click);
            // 
            // GameAspectSimulator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 588);
            this.Controls.Add(this.btnPicProfile);
            this.Controls.Add(this.btnPicSnipe);
            this.Controls.Add(this.btnPicClose);
            this.Controls.Add(this.btnPicConfig);
            this.Controls.Add(this.btnPicItems);
            this.Controls.Add(this.btnPicEggs);
            this.Controls.Add(this.btnPicPokes);
            this.Controls.Add(this.btnPicMenu);
            this.Controls.Add(this.locationPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GameAspectSimulator";
            this.Text = "GameSimulator";
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
