/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 17:18
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokemonGo.RocketAPI.Console
{
    partial class PlayerPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBoxTeam;
        private System.Windows.Forms.Panel panelLeftArea;
        private System.Windows.Forms.PictureBox pictureBoxBuddyPokemon;
        private System.Windows.Forms.PictureBox pictureBoxPlayerAvatar;
        private System.Windows.Forms.Button btnTeam;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader Title;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader ID;
		
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
            this.pictureBoxTeam = new System.Windows.Forms.PictureBox();
            this.panelLeftArea = new System.Windows.Forms.Panel();
            this.pictureBoxBuddyPokemon = new System.Windows.Forms.PictureBox();
            this.pictureBoxPlayerAvatar = new System.Windows.Forms.PictureBox();
            this.btnTeam = new System.Windows.Forms.Button();
            this.btnCollect = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.ID = new System.Windows.Forms.ColumnHeader();
            this.Title = new System.Windows.Forms.ColumnHeader();
            this.Value = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTeam)).BeginInit();
            this.panelLeftArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBuddyPokemon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayerAvatar)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxTeam
            // 
            this.pictureBoxTeam.Image = global::PokemonGo.RocketAPI.Console.Properties.Resources.team_valor;
            this.pictureBoxTeam.Location = new System.Drawing.Point(0, 6);
            this.pictureBoxTeam.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxTeam.Name = "pictureBoxTeam";
            this.pictureBoxTeam.Size = new System.Drawing.Size(299, 238);
            this.pictureBoxTeam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTeam.TabIndex = 64;
            this.pictureBoxTeam.TabStop = false;
            // 
            // panelLeftArea
            // 
            this.panelLeftArea.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.panelLeftArea.Controls.Add(this.pictureBoxBuddyPokemon);
            this.panelLeftArea.Controls.Add(this.pictureBoxPlayerAvatar);
            this.panelLeftArea.Controls.Add(this.pictureBoxTeam);
            this.panelLeftArea.Location = new System.Drawing.Point(2, 101);
            this.panelLeftArea.Margin = new System.Windows.Forms.Padding(2);
            this.panelLeftArea.Name = "panelLeftArea";
            this.panelLeftArea.Size = new System.Drawing.Size(300, 259);
            this.panelLeftArea.TabIndex = 89;
            // 
            // pictureBoxBuddyPokemon
            // 
            this.pictureBoxBuddyPokemon.Location = new System.Drawing.Point(2, 126);
            this.pictureBoxBuddyPokemon.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxBuddyPokemon.Name = "pictureBoxBuddyPokemon";
            this.pictureBoxBuddyPokemon.Size = new System.Drawing.Size(112, 112);
            this.pictureBoxBuddyPokemon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxBuddyPokemon.TabIndex = 66;
            this.pictureBoxBuddyPokemon.TabStop = false;
            // 
            // pictureBoxPlayerAvatar
            // 
            this.pictureBoxPlayerAvatar.Image = global::PokemonGo.RocketAPI.Console.Properties.Resources.Trainer_M;
            this.pictureBoxPlayerAvatar.Location = new System.Drawing.Point(2, 41);
            this.pictureBoxPlayerAvatar.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBoxPlayerAvatar.Name = "pictureBoxPlayerAvatar";
            this.pictureBoxPlayerAvatar.Size = new System.Drawing.Size(145, 201);
            this.pictureBoxPlayerAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPlayerAvatar.TabIndex = 65;
            this.pictureBoxPlayerAvatar.TabStop = false;
            // 
            // btnTeam
            // 
            this.btnTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTeam.Location = new System.Drawing.Point(712, 426);
            this.btnTeam.Margin = new System.Windows.Forms.Padding(4);
            this.btnTeam.Name = "btnTeam";
            this.btnTeam.Size = new System.Drawing.Size(94, 29);
            this.btnTeam.TabIndex = 102;
            this.btnTeam.Text = "Select Team";
            this.btnTeam.UseVisualStyleBackColor = true;
            this.btnTeam.Click += new System.EventHandler(this.BtnTeamClick);
            // 
            // btnCollect
            // 
            this.btnCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollect.Location = new System.Drawing.Point(598, 426);
            this.btnCollect.Margin = new System.Windows.Forms.Padding(4);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(106, 29);
            this.btnCollect.TabIndex = 105;
            this.btnCollect.Text = "Collect Coins";
            this.btnCollect.UseVisualStyleBackColor = true;
            this.btnCollect.Click += new System.EventHandler(this.btnColect_Click);
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ID,
            this.Title,
            this.Value});
            this.listView.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(305, 3);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(493, 416);
            this.listView.TabIndex = 106;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            // 
            // ID
            // 
            this.ID.Text = "#";
            this.ID.Width = 30;
            // 
            // Title
            // 
            this.Title.Text = "Title";
            this.Title.Width = 200;
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 200;
            // 
            // PlayerPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.btnCollect);
            this.Controls.Add(this.btnTeam);
            this.Controls.Add(this.panelLeftArea);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "PlayerPanel";
            this.Size = new System.Drawing.Size(810, 464);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTeam)).EndInit();
            this.panelLeftArea.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBuddyPokemon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlayerAvatar)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
