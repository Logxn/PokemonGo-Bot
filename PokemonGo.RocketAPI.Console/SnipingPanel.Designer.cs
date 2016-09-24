namespace PokemonGo.RocketAPI.Console
{
    partial class SnipingPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SnipePokemonPokeCom = new System.Windows.Forms.CheckBox();
            this.groupBox23 = new System.Windows.Forms.GroupBox();
            this.UpdateNotToSnipe = new System.Windows.Forms.Button();
            this.SelectallNottoSnipe = new System.Windows.Forms.CheckBox();
            this.checkedListBox_NotToSnipe = new System.Windows.Forms.CheckedListBox();
            this.OnlySnipe = new System.Windows.Forms.CheckBox();
            this.AvoidRegionLock = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SnipeMe = new System.Windows.Forms.Button();
            this.label64 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SnipeInfo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3.SuspendLayout();
            this.groupBox23.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(3, 424);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(248, 32);
            this.button4.TabIndex = 78;
            this.button4.Text = "Re-Start Automatic Snipe";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ForceAutoSnipe_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SnipePokemonPokeCom);
            this.groupBox3.Controls.Add(this.groupBox23);
            this.groupBox3.Controls.Add(this.OnlySnipe);
            this.groupBox3.Controls.Add(this.AvoidRegionLock);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(248, 415);
            this.groupBox3.TabIndex = 80;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Automatic Sniping";
            // 
            // SnipePokemonPokeCom
            // 
            this.SnipePokemonPokeCom.AutoSize = true;
            this.SnipePokemonPokeCom.Location = new System.Drawing.Point(14, 26);
            this.SnipePokemonPokeCom.Margin = new System.Windows.Forms.Padding(4);
            this.SnipePokemonPokeCom.Name = "SnipePokemonPokeCom";
            this.SnipePokemonPokeCom.Size = new System.Drawing.Size(191, 21);
            this.SnipePokemonPokeCom.TabIndex = 71;
            this.SnipePokemonPokeCom.Text = "Enable Automatic Sniping";
            this.SnipePokemonPokeCom.UseVisualStyleBackColor = true;
            // 
            // groupBox23
            // 
            this.groupBox23.Controls.Add(this.UpdateNotToSnipe);
            this.groupBox23.Controls.Add(this.SelectallNottoSnipe);
            this.groupBox23.Controls.Add(this.checkedListBox_NotToSnipe);
            this.groupBox23.Location = new System.Drawing.Point(14, 82);
            this.groupBox23.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox23.Name = "groupBox23";
            this.groupBox23.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox23.Size = new System.Drawing.Size(220, 296);
            this.groupBox23.TabIndex = 71;
            this.groupBox23.TabStop = false;
            this.groupBox23.Text = "Pokemon - Not to Snipe";
            // 
            // UpdateNotToSnipe
            // 
            this.UpdateNotToSnipe.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.UpdateNotToSnipe.Location = new System.Drawing.Point(102, 260);
            this.UpdateNotToSnipe.Name = "UpdateNotToSnipe";
            this.UpdateNotToSnipe.Size = new System.Drawing.Size(105, 23);
            this.UpdateNotToSnipe.TabIndex = 33;
            this.UpdateNotToSnipe.Text = "Apply Changes";
            this.UpdateNotToSnipe.UseVisualStyleBackColor = true;
            // 
            // SelectallNottoSnipe
            // 
            this.SelectallNottoSnipe.AutoSize = true;
            this.SelectallNottoSnipe.Location = new System.Drawing.Point(8, 260);
            this.SelectallNottoSnipe.Margin = new System.Windows.Forms.Padding(4);
            this.SelectallNottoSnipe.Name = "SelectallNottoSnipe";
            this.SelectallNottoSnipe.Size = new System.Drawing.Size(87, 21);
            this.SelectallNottoSnipe.TabIndex = 32;
            this.SelectallNottoSnipe.Text = "Select all";
            this.SelectallNottoSnipe.UseVisualStyleBackColor = true;
            // 
            // checkedListBox_NotToSnipe
            // 
            this.checkedListBox_NotToSnipe.CheckOnClick = true;
            this.checkedListBox_NotToSnipe.FormattingEnabled = true;
            this.checkedListBox_NotToSnipe.Location = new System.Drawing.Point(8, 25);
            this.checkedListBox_NotToSnipe.Margin = new System.Windows.Forms.Padding(4);
            this.checkedListBox_NotToSnipe.Name = "checkedListBox_NotToSnipe";
            this.checkedListBox_NotToSnipe.ScrollAlwaysVisible = true;
            this.checkedListBox_NotToSnipe.Size = new System.Drawing.Size(199, 225);
            this.checkedListBox_NotToSnipe.TabIndex = 0;
            // 
            // OnlySnipe
            // 
            this.OnlySnipe.AutoSize = true;
            this.OnlySnipe.Location = new System.Drawing.Point(14, 53);
            this.OnlySnipe.Margin = new System.Windows.Forms.Padding(4);
            this.OnlySnipe.Name = "OnlySnipe";
            this.OnlySnipe.Size = new System.Drawing.Size(192, 21);
            this.OnlySnipe.TabIndex = 70;
            this.OnlySnipe.Text = "Only Run Snipe Pokemon";
            this.OnlySnipe.UseVisualStyleBackColor = true;
            // 
            // AvoidRegionLock
            // 
            this.AvoidRegionLock.AutoSize = true;
            this.AvoidRegionLock.Checked = true;
            this.AvoidRegionLock.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AvoidRegionLock.Location = new System.Drawing.Point(14, 385);
            this.AvoidRegionLock.Margin = new System.Windows.Forms.Padding(4);
            this.AvoidRegionLock.Name = "AvoidRegionLock";
            this.AvoidRegionLock.Size = new System.Drawing.Size(227, 21);
            this.AvoidRegionLock.TabIndex = 70;
            this.AvoidRegionLock.Text = "Avoid Region Locked Pokemon";
            this.AvoidRegionLock.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SnipeMe);
            this.groupBox1.Controls.Add(this.label64);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.SnipeInfo);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(270, 248);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(363, 208);
            this.groupBox1.TabIndex = 79;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Manual Sniping (Disabled While Under Construction!)";
            // 
            // SnipeMe
            // 
            this.SnipeMe.BackColor = System.Drawing.Color.MediumAquamarine;
            this.SnipeMe.Location = new System.Drawing.Point(21, 81);
            this.SnipeMe.Margin = new System.Windows.Forms.Padding(4);
            this.SnipeMe.Name = "SnipeMe";
            this.SnipeMe.Size = new System.Drawing.Size(324, 28);
            this.SnipeMe.TabIndex = 74;
            this.SnipeMe.Text = "Snipe Me!";
            this.SnipeMe.UseVisualStyleBackColor = false;
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(69, 30);
            this.label64.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(228, 17);
            this.label64.TabIndex = 72;
            this.label64.Text = "Snipe Info (pokemonName|lat|long)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(36, 143);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(300, 25);
            this.label4.TabIndex = 72;
            this.label4.Text = "Venusaur|30.123456|-97.123456";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 177);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(190, 17);
            this.label5.TabIndex = 72;
            this.label5.Text = "please use decimals for now.";
            // 
            // SnipeInfo
            // 
            this.SnipeInfo.Location = new System.Drawing.Point(21, 51);
            this.SnipeInfo.Margin = new System.Windows.Forms.Padding(4);
            this.SnipeInfo.Name = "SnipeInfo";
            this.SnipeInfo.Size = new System.Drawing.Size(324, 22);
            this.SnipeInfo.TabIndex = 73;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 117);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(315, 17);
            this.label3.TabIndex = 72;
            this.label3.Text = "You must enter Snipe Info in the following format!";
            // 
            // SnipingPanel
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "SnipingPanel";
            this.Size = new System.Drawing.Size(841, 461);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox23.ResumeLayout(false);
            this.groupBox23.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox SnipePokemonPokeCom;
        private System.Windows.Forms.GroupBox groupBox23;
        private System.Windows.Forms.Button UpdateNotToSnipe;
        private System.Windows.Forms.CheckBox SelectallNottoSnipe;
        private System.Windows.Forms.CheckedListBox checkedListBox_NotToSnipe;
        private System.Windows.Forms.CheckBox OnlySnipe;
        private System.Windows.Forms.CheckBox AvoidRegionLock;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SnipeMe;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox SnipeInfo;
        private System.Windows.Forms.Label label3;
    }
}
