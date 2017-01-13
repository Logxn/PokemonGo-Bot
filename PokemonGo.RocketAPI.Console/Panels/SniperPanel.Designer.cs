/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 3:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class SniperPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button ForceAutoSnipe;
        private System.Windows.Forms.CheckBox AvoidRegionLock;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button UpdateNotToSnipe;
        private System.Windows.Forms.CheckBox SelectallNottoSnipe;
        private System.Windows.Forms.CheckedListBox checkedListBox_NotToSnipe;
        private System.Windows.Forms.CheckBox SnipePokemonPokeCom;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox PokemonImage;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button SnipeMe;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.TextBox SnipeInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.LinkLabel LinkPokesniperCom;
        private System.Windows.Forms.LinkLabel linkRarespawns;
        private System.Windows.Forms.LinkLabel linkPokezz;
        
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SniperPanel));
            this.ForceAutoSnipe = new System.Windows.Forms.Button();
            this.AvoidRegionLock = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.UpdateNotToSnipe = new System.Windows.Forms.Button();
            this.SelectallNottoSnipe = new System.Windows.Forms.CheckBox();
            this.checkedListBox_NotToSnipe = new System.Windows.Forms.CheckedListBox();
            this.SnipePokemonPokeCom = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PokemonImage = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SnipeMe = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.SnipeInfo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkPokezz = new System.Windows.Forms.LinkLabel();
            this.linkRarespawns = new System.Windows.Forms.LinkLabel();
            this.LinkPokesniperCom = new System.Windows.Forms.LinkLabel();
            this.btnInstall = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox23.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ForceAutoSnipe
            // 
            resources.ApplyResources(this.ForceAutoSnipe, "ForceAutoSnipe");
            this.ForceAutoSnipe.Name = "ForceAutoSnipe";
            this.ForceAutoSnipe.UseVisualStyleBackColor = true;
            this.ForceAutoSnipe.Click += new System.EventHandler(this.ForceAutoSnipe_Click);
            // 
            // AvoidRegionLock
            // 
            resources.ApplyResources(this.AvoidRegionLock, "AvoidRegionLock");
            this.AvoidRegionLock.Checked = true;
            this.AvoidRegionLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidRegionLock.Name = "AvoidRegionLock";
            this.AvoidRegionLock.UseVisualStyleBackColor = true;
            this.AvoidRegionLock.CheckedChanged += new System.EventHandler(this.AvoidRegionLock_CheckedChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.UpdateNotToSnipe);
            this.groupBox23.Controls.Add(this.SelectallNottoSnipe);
            this.groupBox23.Controls.Add(this.checkedListBox_NotToSnipe);
            resources.ApplyResources(this.groupBox23, "groupBox23");
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.TabStop = false;
            // 
            // UpdateNotToSnipe
            // 
            resources.ApplyResources(this.UpdateNotToSnipe, "UpdateNotToSnipe");
            this.UpdateNotToSnipe.Name = "UpdateNotToSnipe";
            this.UpdateNotToSnipe.UseVisualStyleBackColor = true;
            this.UpdateNotToSnipe.Click += new System.EventHandler(this.UpdateNotToSnipe_Click);
            // 
            // SelectallNottoSnipe
            // 
            resources.ApplyResources(this.SelectallNottoSnipe, "SelectallNottoSnipe");
            this.SelectallNottoSnipe.Name = "SelectallNottoSnipe";
            this.SelectallNottoSnipe.UseVisualStyleBackColor = true;
            this.SelectallNottoSnipe.CheckedChanged += new System.EventHandler(this.SelectallNottoSnipe_CheckedChanged);
            // 
            // checkedListBox_NotToSnipe
            // 
            this.checkedListBox_NotToSnipe.CheckOnClick = true;
            this.checkedListBox_NotToSnipe.FormattingEnabled = true;
            resources.ApplyResources(this.checkedListBox_NotToSnipe, "checkedListBox_NotToSnipe");
            this.checkedListBox_NotToSnipe.Name = "checkedListBox_NotToSnipe";
            // 
            // SnipePokemonPokeCom
            // 
            resources.ApplyResources(this.SnipePokemonPokeCom, "SnipePokemonPokeCom");
            this.SnipePokemonPokeCom.Name = "SnipePokemonPokeCom";
            this.SnipePokemonPokeCom.UseVisualStyleBackColor = true;
            this.SnipePokemonPokeCom.CheckedChanged += new System.EventHandler(this.SnipePokemonPokeCom_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.PokemonImage);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.SnipeMe);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label64);
            this.groupBox1.Controls.Add(this.SnipeInfo);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // PokemonImage
            // 
            resources.ApplyResources(this.PokemonImage, "PokemonImage");
            this.PokemonImage.Name = "PokemonImage";
            this.PokemonImage.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox1, "comboBox1");
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // SnipeMe
            // 
            this.SnipeMe.BackColor = System.Drawing.Color.MediumAquamarine;
            resources.ApplyResources(this.SnipeMe, "SnipeMe");
            this.SnipeMe.Name = "SnipeMe";
            this.SnipeMe.UseVisualStyleBackColor = false;
            this.SnipeMe.Click += new System.EventHandler(this.SnipeMe_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label64
            // 
            resources.ApplyResources(this.label64, "label64");
            this.label64.Name = "label64";
            // 
            // SnipeInfo
            // 
            resources.ApplyResources(this.SnipeInfo, "SnipeInfo");
            this.SnipeInfo.Name = "SnipeInfo";
            this.SnipeInfo.TextChanged += new System.EventHandler(this.SnipeInfo_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkPokezz);
            this.groupBox2.Controls.Add(this.linkRarespawns);
            this.groupBox2.Controls.Add(this.LinkPokesniperCom);
            this.groupBox2.Controls.Add(this.btnInstall);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // linkPokezz
            // 
            resources.ApplyResources(this.linkPokezz, "linkPokezz");
            this.linkPokezz.Name = "linkPokezz";
            this.linkPokezz.TabStop = true;
            this.linkPokezz.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // linkRarespawns
            // 
            resources.ApplyResources(this.linkRarespawns, "linkRarespawns");
            this.linkRarespawns.Name = "linkRarespawns";
            this.linkRarespawns.TabStop = true;
            this.linkRarespawns.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // LinkPokesniperCom
            // 
            resources.ApplyResources(this.LinkPokesniperCom, "LinkPokesniperCom");
            this.LinkPokesniperCom.Name = "LinkPokesniperCom";
            this.LinkPokesniperCom.TabStop = true;
            this.LinkPokesniperCom.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.PokesniperCom_LinkClicked);
            // 
            // btnInstall
            // 
            resources.ApplyResources(this.btnInstall, "btnInstall");
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label7.Name = "label7";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SniperPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.ForceAutoSnipe);
            this.Controls.Add(this.AvoidRegionLock);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox23);
            this.Controls.Add(this.SnipePokemonPokeCom);
            this.Controls.Add(this.groupBox1);
            resources.ApplyResources(this, "$this");
            this.Name = "SniperPanel";
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PokemonImage)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
