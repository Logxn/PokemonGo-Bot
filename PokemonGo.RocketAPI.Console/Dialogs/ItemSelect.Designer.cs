/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 07/02/2017
 * Time: 23:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace PokeMaster.Dialogs
{
    partial class ItemSelect
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListView ItemsListView;
        private System.Windows.Forms.ColumnHeader chItem;
        private System.Windows.Forms.ColumnHeader chCount;
        private System.Windows.Forms.ColumnHeader chUnseen;
        private System.Windows.Forms.ColumnHeader chID;
        private System.Windows.Forms.ImageList imageListItems;
        
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemSelect));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.ItemsListView = new System.Windows.Forms.ListView();
            this.chItem = new System.Windows.Forms.ColumnHeader();
            this.chCount = new System.Windows.Forms.ColumnHeader();
            this.chUnseen = new System.Windows.Forms.ColumnHeader();
            this.chID = new System.Windows.Forms.ColumnHeader();
            this.imageListItems = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(218, 370);
            this.buttonOk.Margin = new System.Windows.Forms.Padding(2);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(55, 23);
            this.buttonOk.TabIndex = 86;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(277, 370);
            this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(55, 23);
            this.buttonCancel.TabIndex = 85;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // ItemsListView
            // 
            this.ItemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chItem,
            this.chCount,
            this.chUnseen,
            this.chID});
            this.ItemsListView.FullRowSelect = true;
            this.ItemsListView.GridLines = true;
            this.ItemsListView.Location = new System.Drawing.Point(13, 13);
            this.ItemsListView.Margin = new System.Windows.Forms.Padding(4);
            this.ItemsListView.Name = "ItemsListView";
            this.ItemsListView.Size = new System.Drawing.Size(319, 351);
            this.ItemsListView.TabIndex = 87;
            this.ItemsListView.UseCompatibleStateImageBehavior = false;
            this.ItemsListView.View = System.Windows.Forms.View.Details;
            // 
            // chItem
            // 
            this.chItem.Text = "Item";
            this.chItem.Width = 160;
            // 
            // chCount
            // 
            this.chCount.Text = "Count";
            // 
            // chUnseen
            // 
            this.chUnseen.Text = "Unseen";
            // 
            // chID
            // 
            this.chID.Text = "#";
            this.chID.Width = 40;
            // 
            // imageListItems
            // 
            this.imageListItems.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListItems.ImageStream")));
            this.imageListItems.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListItems.Images.SetKeyName(0, "incenseOrdinary");
            this.imageListItems.Images.SetKeyName(1, "hyperpotion");
            this.imageListItems.Images.SetKeyName(2, "pokeball");
            this.imageListItems.Images.SetKeyName(3, "potion");
            this.imageListItems.Images.SetKeyName(4, "revive");
            this.imageListItems.Images.SetKeyName(5, "greatball");
            this.imageListItems.Images.SetKeyName(6, "superpotion");
            this.imageListItems.Images.SetKeyName(7, "ultraball");
            this.imageListItems.Images.SetKeyName(8, "incubatorbasic");
            this.imageListItems.Images.SetKeyName(9, "incubatorbasicunlimited");
            this.imageListItems.Images.SetKeyName(10, "razzberry");
            this.imageListItems.Images.SetKeyName(11, "troydisk");
            this.imageListItems.Images.SetKeyName(12, "luckyegg");
            this.imageListItems.Images.SetKeyName(13, "maxrevive");
            this.imageListItems.Images.SetKeyName(14, "maxpotion");
            // 
            // ItemSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 404);
            this.Controls.Add(this.ItemsListView);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Name = "ItemSelect";
            this.Text = "ItemSelect";
            this.ResumeLayout(false);

        }
    }
}
