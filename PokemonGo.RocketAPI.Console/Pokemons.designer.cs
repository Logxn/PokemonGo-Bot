namespace PokemonGo.RocketAPI.Console
{
    partial class Pokemons
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Pokemons));
            this.listView1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnUpgrade = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(12, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(759, 387);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 405);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(759, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Reload";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 434);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(759, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Evolve selected";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnUpgrade
            // 
            this.btnUpgrade.Location = new System.Drawing.Point(13, 463);
            this.btnUpgrade.Name = "btnUpgrade";
            this.btnUpgrade.Size = new System.Drawing.Size(759, 23);
            this.btnUpgrade.TabIndex = 3;
            this.btnUpgrade.Text = "PowerUp selected";
            this.btnUpgrade.UseVisualStyleBackColor = true;
            this.btnUpgrade.Click += new System.EventHandler(this.btnUpgrade_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 492);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(759, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Transfer selected";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transferToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(117, 26);
            // 
            // transferToolStripMenuItem
            // 
            this.transferToolStripMenuItem.Name = "transferToolStripMenuItem";
            this.transferToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.transferToolStripMenuItem.Text = "Transfer";
            this.transferToolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(13, 522);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(758, 20);
            this.textBox1.TabIndex = 5;
            // 
            // Pokemons
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 552);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnUpgrade);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Pokemons";
            this.Text = "Pokemon List";
            this.Load += new System.EventHandler(this.Pokemons_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnUpgrade;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem transferToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox1;
    }
}