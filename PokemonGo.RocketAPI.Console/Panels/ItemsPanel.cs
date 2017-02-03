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
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.Logic.Shared;
	
namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of ItemsPanel.
    /// </summary>
    public partial class ItemsPanel : UserControl
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public ItemsPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            th.Translate(this);
        }
        void BtnRealoadItemsClick(object sender, EventArgs e)
        {
            ItemsListView.Items.Clear();
            Execute();
        }
        public void Execute()
        {
            try {
                num_MaxPokeballs.Value = GlobalVars.MaxPokeballs;
                num_MaxGreatBalls.Value = GlobalVars.MaxGreatballs;
                num_MaxUltraBalls.Value = GlobalVars.MaxUltraballs;
                num_MaxRevives.Value = GlobalVars.MaxRevives;
                num_MaxPotions.Value = GlobalVars.MaxPotions;
                num_MaxSuperPotions.Value = GlobalVars.MaxSuperPotions;
                num_MaxHyperPotions.Value = GlobalVars.MaxHyperPotions;
                num_MaxRazzBerrys.Value = GlobalVars.MaxBerries;
                num_MaxTopRevives.Value = GlobalVars.MaxTopRevives;
                num_MaxTopPotions.Value = GlobalVars.MaxTopPotions;
                int count = 0;
                count += GlobalVars.MaxPokeballs + GlobalVars.MaxGreatballs + GlobalVars.MaxUltraballs + GlobalVars.MaxRevives
                + GlobalVars.MaxPotions + GlobalVars.MaxSuperPotions + GlobalVars.MaxHyperPotions + GlobalVars.MaxBerries
                + GlobalVars.MaxTopRevives + GlobalVars.MaxTopPotions;
                text_TotalItemCount.Text = count.ToString();

                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    // eb - removing async
                    //var items = await client.Inventory.GetItems().ConfigureAwait(false);
                    var items = client.Inventory.GetItems().Result;

                    ItemId[] validsIDs = {
                        ItemId.ItemPokeBall,
                        ItemId.ItemGreatBall,
                        ItemId.ItemUltraBall
                    };
	               
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
                        ItemsListView.Items.Add(listViewItem);
                    }
                    lblCount.Text = "" + sum;
                }
            } catch (Exception e) {

                Logger.Error("[ItemsList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000, 1100);
                //Execute(); 
            }
        }
        private string getItemName(ItemId itemID)
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
                taskResponse resp = new taskResponse(false, string.Empty);

                //resp = await RecycleItems(item, amount).ConfigureAwait(false);
                resp = RecycleItems(item, amount).Result;
                if (resp.Status) {
                    item.Count -= amount;
                    ItemsListView.SelectedItems[0].SubItems[1].Text = "" + item.Count;
                } else
                    MessageBox.Show(resp.Message + th.TS(" recycle failed!"), th.TS("Recycle Status"), MessageBoxButtons.OK);

            }
        }
        public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse()
            {
            }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
        private static async Task<taskResponse> RecycleItems(ItemData item, int amount)
        {
            var resp1 = new taskResponse(false, string.Empty);
            try {
                var client = Logic.Logic.objClient;
                var resp2 = await client.Inventory.RecycleItem(item.ItemId, amount).ConfigureAwait(false);

                if (resp2.Result == RecycleInventoryItemResponse.Types.Result.Success) {
                    resp1.Status = true;
                } else {
                    resp1.Message = item.ItemId.ToString();
                }
            } catch (Exception e) {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error RecycleItem: " + e.Message);
                await RecycleItems(item, amount).ConfigureAwait(false);
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
                    case "num_MaxGreatBalls":
                        GlobalVars.MaxGreatballs = value;
                        break;
                    case "num_MaxUltraBalls":
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
                    case "num_MaxRazzBerrys":        		
                        GlobalVars.MaxBerries = value;
                        break;
        				
                }        
                int count = 0;
                count += GlobalVars.MaxPokeballs + GlobalVars.MaxGreatballs + GlobalVars.MaxUltraballs + GlobalVars.MaxRevives
                + GlobalVars.MaxPotions + GlobalVars.MaxSuperPotions + GlobalVars.MaxHyperPotions + GlobalVars.MaxBerries
                + GlobalVars.MaxTopRevives + GlobalVars.MaxTopPotions;
                text_TotalItemCount.Text = count.ToString();
            } catch (Exception e1) {
                Logger.ExceptionInfo(e1.ToString());
            }
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
        
        private async Task RecycleItems(bool forcerefresh = false)
        {            
            var client = Logic.Logic.objClient;
            var items = await client.Inventory.GetItemsToRecycle(GlobalVars.GetItemFilter()).ConfigureAwait(false);
            foreach (var item in items) {
                var transfer = await client.Inventory.RecycleItem((ItemId)item.ItemId, item.Count).ConfigureAwait(false);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, String.Format("Recycled {0}x {1}",item.Count,(ItemId)item.ItemId));
                await RandomHelper.RandomDelay(1000, 5000).ConfigureAwait(false);
            }
        }

        void btnDiscard_Click(object sender, EventArgs e)
        {
            RecycleItems().Wait();
            Execute();
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
    }
}
