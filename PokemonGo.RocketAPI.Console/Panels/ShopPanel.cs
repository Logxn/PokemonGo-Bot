/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/02/2017
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Networking.Platform.Responses;
using PokemonGo.RocketAPI;

namespace PokeMaster.Panels
{
    /// <summary>
    /// Description of ShopPanel.
    /// </summary>
    public partial class ShopPanel : UserControl
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public ShopPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
        }
        public void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    Logger.Debug("Before of GetStoreItems");
                    var inventory = client.Store.GetStoreItems().Items;
                    Logger.Debug("After of GetStoreItems");
                    ListViewItem listViewItem;
                    listView.Items.Clear();
                    foreach (var item in inventory) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = "" + item.ItemId;
                        listViewItem.SubItems.Add("" + item.CurrencyToBuy);
                        listViewItem.SubItems.Add("" + item.IsIap);
                        listViewItem.SubItems.Add("" + item.Unknown7);
                        listViewItem.SubItems.Add("" + item.YieldsCurrency);
                        listViewItem.SubItems.Add("" + item.Tags);
                        listView.Items.Add(listViewItem);
                    }
                }
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
        }
        void btnBuy_Click(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0) {
                var item = (GetStoreItemsResponse.Types.StoreItem)listView.SelectedItems[0].Tag;
                if (MessageBox.Show(this, th.TS("Buying {0}.", item.ItemId) + th.TS("\nAre you sure you want?"), th.TS("Confirmation Message"), MessageBoxButtons.OKCancel) == DialogResult.OK) {
                    Logic.Logic.objClient.Store.BuyItemPokeCoins(item.ItemId);
                }
            }
        }
        void buyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnBuy_Click(sender, e);
        }
        void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Execute();
        }

    }
}
