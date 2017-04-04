/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 14/09/2016
 * Time: 22:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Inventory.Item;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;
using PokeMaster.Dialogs;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Shared;
	
namespace PokeMaster
{
    /// <summary>
    /// Description of ItemsPanel.
    /// </summary>
    public partial class ItemsPanel : UserControl
    {
        private static Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public GetPlayerResponse profile;
        
        public ItemsPanel()
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
        void BtnRealoadItemsClick(object sender, EventArgs e)
        {
            ItemsListView.Items.Clear();
            Execute(profile);
        }
        public void Execute(GetPlayerResponse profileIn)
        {
            profile = profileIn;
            try {
                foreach (Control element in this.groupBoxItems.Controls) {
                    if (element.Name.IndexOf("num_") == 0){
                        var name = element.Name.Replace("num_","");
                        var property = typeof(GlobalVars).GetField(name);
                        if (property!=null)
                            (element as NumericUpDown).Value = (int) property.GetValue(null);
                    }
                }
                UpdateItemTotalCount();

                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    var items = client.Inventory.GetItems();
                    ListViewItem listViewItem;
                    ItemsListView.Items.Clear();
                    var sum = 0;
                    foreach (var item in items) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = getItemName(item.ItemId);
                        listViewItem.ImageKey = item.ItemId.ToString().Replace("Item", "");
                        listViewItem.SubItems.Add("" + item.Count);
                        sum += item.Count;
                        listViewItem.SubItems.Add("" + item.Unseen);
                        listViewItem.SubItems.Add( ""+(int) item.ItemId);
                        ItemsListView.Items.Add(listViewItem);
                    }
                    lblCount.Text = "" + sum;
                    RefreshTitle();
                }
            } catch (Exception e) {
                Logger.ExceptionInfo("[ItemsList-Error] " + e.StackTrace);
            }
        }

        public void RefreshTitle()
        {
            var txt = th.TS("Items");
            if (Parent != null) {
                txt += ": " + lblCount.Text;
                if (profile !=null)
                    txt += "/" + profile.PlayerData.MaxItemStorage;
            }
            Parent.Text = txt;
        }

        public static string getItemName(ItemId itemID)
        {
            switch (itemID) {
                case ItemId.ItemPotion:
                    return th.TS("Potion");
                case ItemId.ItemSuperPotion:
                    return th.TS("Super Potion");
                case ItemId.ItemHyperPotion:
                    return th.TS("Hyper Potion");
                case ItemId.ItemMaxPotion:
                    return th.TS("Max Potion");
                case ItemId.ItemRevive:
                    return th.TS("Revive");
                case ItemId.ItemMaxRevive:
                    return th.TS("Max Revive");
                case ItemId.ItemIncenseOrdinary:
                    return th.TS("Incense");
                case ItemId.ItemPokeBall:
                    return th.TS("Poke Ball");
                case ItemId.ItemGreatBall:
                    return th.TS("Great Ball");
                case ItemId.ItemUltraBall:
                    return th.TS("Ultra Ball");
                case ItemId.ItemRazzBerry:
                    return th.TS("Razz Berry");
                case ItemId.ItemNanabBerry:
                    return th.TS("Nanab Berry");
                case ItemId.ItemPinapBerry:
                    return th.TS("Pinap Berry");
                case ItemId.ItemIncubatorBasic:
                    return th.TS("Egg Incubator");
                case ItemId.ItemIncubatorBasicUnlimited:
                    return th.TS("Unlimited Egg Incubator");
                default:
                    return itemID.ToString().Replace("Item", "");
            }
        }

        void RecycleToolStripMenuItemClick(object sender, EventArgs e)
        {
            var item = (ItemData)ItemsListView.SelectedItems[0].Tag;
            int amount = IntegerInput.ShowDialog(1, "How many?", item.Count);
            if (amount > 0) {
                var resp = RecycleItems(item, amount);
                if (resp) {
                    item.Count -= amount;
                    ItemsListView.SelectedItems[0].SubItems[1].Text = "" + item.Count;
                } else
                    MessageBox.Show(th.TS(" recycle failed!"), th.TS("Recycle Status"), MessageBoxButtons.OK);
            }
        }

        private static bool RecycleItems(ItemData item, int amount)
        {
            var resp1 = false;
            try {
                var client = Logic.Logic.objClient;
                var resp2 = client.Inventory.RecycleItem(item.ItemId, amount).Result;

                if (resp2.Result == RecycleInventoryItemResponse.Types.Result.Success) {
                    resp1 = true;
                }else
                    Logger.Error("RecycleItems:" +resp2.Result);
            } catch (Exception e) {
                Logger.ExceptionInfo("RecycleItems: "+e);
            }
            return resp1;
        }

        private void num_Max(object sender, EventArgs e)
        {
            try {
                var numB = (NumericUpDown)sender;
                var value = (int)numB.Value;
                switch (numB.Name) {
                    case "num_MaxPokeballs":
                        GlobalVars.MaxPokeballs = value;
                        break;
                    case "num_MaxGreatballs":
                        GlobalVars.MaxGreatballs = value;
                        break;
                    case "num_MaxUltraballs":
                        GlobalVars.MaxUltraballs = value;
                        break;
                    case "num_MaxRevives":
                        GlobalVars.MaxRevives = value;
                        break;
                    case "num_MaxPotions":
                        GlobalVars.MaxPotions = value;
                        break;
                    case "num_MaxSuperPotions":
                        GlobalVars.MaxSuperPotions = value;
                        break;
                    case "num_MaxHyperPotions":
                        GlobalVars.MaxHyperPotions = value;
                        break;
                    case "num_MaxTopRevives":
                        GlobalVars.MaxTopRevives = value;
                        break;
                    case "num_MaxTopPotions":
                        GlobalVars.MaxTopPotions = value;
                        break;
                    case "num_MaxBerries":
                        GlobalVars.MaxBerries = value;
                        break;
                    case "num_MaxPinapBerries":
                        GlobalVars.MaxPinapBerries = value;
                        break;
                    case "num_MaxNanabBerries":
                        GlobalVars.MaxNanabBerries = value;
                        break;
                    case "num_MaxDragonScale":
                        GlobalVars.MaxDragonScale = value;
                        break;
                    case "num_MaxSunStone":
                        GlobalVars.MaxSunStone = value;
                        break;
                    case "num_MaxKingsRock":
                        GlobalVars.MaxKingsRock = value;
                        break;
                    case "num_MaxMetalCoat":
                        GlobalVars.MaxMetalCoat = value;
                        break;
                    case "num_MaxUpGrade":
                        GlobalVars.MaxUpGrade = value;
                        break;
                }
                UpdateItemTotalCount();

            } catch (Exception e1) {
                Logger.ExceptionInfo(e1.ToString());
            }
        }

        private void UpdateItemTotalCount(){
            int totalCount = 0;
            foreach (Control element in this.groupBoxItems.Controls) 
                if (element.Name.IndexOf("num_") == 0)
                    totalCount += (int)(element as NumericUpDown).Value;
            text_TotalItemCount.Text = ""+ totalCount;
        }

        void btnCopy_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem lvitem in ItemsListView.Items) {
                var item = (ItemData)(lvitem.Tag);
                var controlName = item.ItemId.ToString();
                controlName = controlName.Replace("Item", "num_Max") + "s";
                var controls = Controls.Find(controlName, true);
                if (controls.Length > 0) {
                    ((NumericUpDown)controls[0]).Value = item.Count;
                }
            }
        }

        private void RecycleItems()
        {            
            var client = Logic.Logic.objClient;
            var items = client.Inventory.GetItemsToRecycle(GlobalVars.GetItemFilter());
            foreach (var item in items) {
                var transfer = client.Inventory.RecycleItem((ItemId)item.ItemId, item.Count).Result;
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, String.Format("Recycled {0}x {1}",item.Count,(ItemId)item.ItemId));
                RandomHelper.RandomSleep(1000, 5000);
            }
        }

        void btnDiscard_Click(object sender, EventArgs e)
        {
            RecycleItems();
            Execute(profile);
        }

        void useToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = (ItemData)ItemsListView.SelectedItems[0].Tag;
            if ((item.ItemId != ItemId.ItemLuckyEgg) && (item.ItemId != ItemId.ItemIncenseOrdinary) && (item.ItemId != ItemId.ItemTroyDisk)) {
                MessageBox.Show(th.TS("{0} cannot be used here.",getItemName(item.ItemId)));
                return;
            }
            DialogResult dialogResult = MessageBox.Show(th.TS("Are you sure you want use {0}?", getItemName(item.ItemId)), th.TS("Use Warning"), MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes) {
                if (item.ItemId == ItemId.ItemIncenseOrdinary) {
                    GlobalVars.UseIncenseGUIClick = true;
                    RandomHelper.RandomSleep(200, 300);
                    BtnRealoadItemsClick(sender, e);
                }
                if (item.ItemId == ItemId.ItemLuckyEgg) {
                    GlobalVars.UseLuckyEggGUIClick = true;
                    RandomHelper.RandomSleep(200, 300);
                    BtnRealoadItemsClick(sender, e);
                }
                if (item.ItemId == ItemId.ItemTroyDisk) {
                    GlobalVars.UseLureGUIClick = true;
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Lure will be used on next pokestop"); 
                }
            
            }
        }

        void ItemsListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var order = (sender as ListView).Sorting;
            ItemsListView.ListViewItemSorter = new Components.ListViewItemComparer(e.Column, order);
            (sender as ListView).Sorting = order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }
}
