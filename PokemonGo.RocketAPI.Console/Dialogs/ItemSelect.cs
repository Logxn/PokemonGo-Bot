/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 07/02/2017
 * Time: 23:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory;
using POGOProtos.Inventory.Item;
using System.Linq;

namespace PokeMaster.Dialogs
{
    /// <summary>
    /// Description of ItemSelect.
    /// </summary>
    public partial class ItemSelect : Form
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public ItemData  selected = null;
        
        public ItemSelect()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            th.Translate(this);

            this.chID.Text = th.TS("#");
            this.chItem.Text = th.TS("Item");
            this.chCount.Text = th.TS("Count");
            this.chUnseen.Text = th.TS("Unseen");
        }
        public void Execute()
        {
            var client = Logic.Logic.objClient;
            if (client.ReadyToUse != false) {
                var items = client.Inventory.GetItems();

                ListViewItem listViewItem;
                ItemsListView.Items.Clear();
                var sum = 0;
                foreach (var item in items) {
                    if (item.Count <1 || !canUseHere(item.ItemId))
                        continue; // lest go to next item
                    listViewItem = new ListViewItem();
                    listViewItem.Tag = item;
                    listViewItem.Text = ItemsPanel.getItemName(item.ItemId);
                    listViewItem.ImageKey = item.ItemId.ToString().Replace("Item", "");
                    listViewItem.SubItems.Add("" + item.Count);
                    sum += item.Count;
                    listViewItem.SubItems.Add("" + item.Unseen);
                    listViewItem.SubItems.Add("" + (int)item.ItemId);
                    ItemsListView.Items.Add(listViewItem);
                }
            }            
        }
        void ButtonOkClick(object sender, EventArgs e)
        {            
            if (ItemsListView.SelectedItems.Count < 1) {
                MessageBox.Show(th.TS("Please Select an item."));
                return;
            }
            selected = (ItemData)ItemsListView.SelectedItems[0].Tag;
            if (!canUseHere(selected.ItemId)) {
                MessageBox.Show(th.TS("This item cannot be use here."));
                return;
            }
            DialogResult = DialogResult.OK;
            this.Close();

        }
        public DialogResult ShowDialog()
        {
            Execute();
            return base.ShowDialog();
        }
        public static bool canUseHere(ItemId itemID)
        {
            switch (itemID) {
                case ItemId.ItemPotion:
                case ItemId.ItemSuperPotion:
                case ItemId.ItemHyperPotion:
                case ItemId.ItemMaxPotion:
                case ItemId.ItemRevive:
                case ItemId.ItemMaxRevive:
                    return true;
            }
            return false;
        }
        
    }
}
