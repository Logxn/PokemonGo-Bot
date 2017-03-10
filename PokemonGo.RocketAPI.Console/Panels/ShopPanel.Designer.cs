/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/02/2017
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokeMaster.Panels
{
    partial class ShopPanel
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.ColumnHeader chItem;
        private System.Windows.Forms.ColumnHeader chCurrency;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem buyToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeaderIsIap;
        private System.Windows.Forms.ColumnHeader columnHeaderUnknown7;
        private System.Windows.Forms.ColumnHeader columnHeaderYieldsCurrency;
        private System.Windows.Forms.ColumnHeader columnHeaderTags;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        
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
            this.listView = new System.Windows.Forms.ListView();
            this.chItem = new System.Windows.Forms.ColumnHeader();
            this.chCurrency = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderIsIap = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderUnknown7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderYieldsCurrency = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderTags = new System.Windows.Forms.ColumnHeader();
            this.btnBuy = new System.Windows.Forms.Button();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chItem,
            this.chCurrency,
            this.columnHeaderIsIap,
            this.columnHeaderUnknown7,
            this.columnHeaderYieldsCurrency,
            this.columnHeaderTags});
            this.listView.ContextMenuStrip = this.contextMenuStrip;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.Location = new System.Drawing.Point(20, 16);
            this.listView.Margin = new System.Windows.Forms.Padding(4);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(704, 364);
            this.listView.TabIndex = 81;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // chItem
            // 
            this.chItem.Text = "Item";
            this.chItem.Width = 200;
            // 
            // chCurrency
            // 
            this.chCurrency.Text = "Currency";
            // 
            // columnHeaderIsIap
            // 
            this.columnHeaderIsIap.Text = "IsIap";
            // 
            // columnHeaderUnknown7
            // 
            this.columnHeaderUnknown7.Text = "Unknown7";
            // 
            // columnHeaderYieldsCurrency
            // 
            this.columnHeaderYieldsCurrency.Text = "YieldsCurrency";
            // 
            // columnHeaderTags
            // 
            this.columnHeaderTags.Text = "Tags";
            this.columnHeaderTags.Width = 100;
            // 
            // btnBuy
            // 
            this.btnBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBuy.ContextMenuStrip = this.contextMenuStrip;
            this.btnBuy.Location = new System.Drawing.Point(20, 389);
            this.btnBuy.Margin = new System.Windows.Forms.Padding(2);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(81, 29);
            this.btnBuy.TabIndex = 82;
            this.btnBuy.Text = "Buy";
            this.btnBuy.UseVisualStyleBackColor = true;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buyToolStripMenuItem,
            this.refreshToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(114, 48);
            // 
            // buyToolStripMenuItem
            // 
            this.buyToolStripMenuItem.Name = "buyToolStripMenuItem";
            this.buyToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.buyToolStripMenuItem.Text = "Buy";
            this.buyToolStripMenuItem.Click += new System.EventHandler(this.buyToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // ShopPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.listView);
            this.Name = "ShopPanel";
            this.Size = new System.Drawing.Size(744, 435);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
