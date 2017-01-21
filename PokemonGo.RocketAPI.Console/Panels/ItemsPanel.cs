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
		public ItemsPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		void BtnRealoadItemsClick(object sender, EventArgs e)
		{
			ItemsListView.Items.Clear();
            Execute();
		}
		public void Execute()
        {
            try
            {
                num_MaxPokeballs.Value = GlobalSettings.pokeball;
                num_MaxGreatBalls.Value = GlobalSettings.greatball;
                num_MaxUltraBalls.Value = GlobalSettings.ultraball;
                num_MaxRevives.Value = GlobalSettings.revive;
                num_MaxPotions.Value = GlobalSettings.potion;
                num_MaxSuperPotions.Value = GlobalSettings.superpotion;
                num_MaxHyperPotions.Value = GlobalSettings.hyperpotion;
                num_MaxRazzBerrys.Value = GlobalSettings.berry;
                num_MaxTopRevives.Value = GlobalSettings.toprevive;
                num_MaxTopPotions.Value = GlobalSettings.toppotion;
                int count = 0;
                count += GlobalSettings.pokeball + GlobalSettings.greatball + GlobalSettings.ultraball + GlobalSettings.revive
                    + GlobalSettings.potion + GlobalSettings.superpotion + GlobalSettings.hyperpotion + GlobalSettings.berry
                    + GlobalSettings.toprevive + GlobalSettings.toppotion;
                text_TotalItemCount.Text = count.ToString();

                var client = Logic.Logic.objClient;
	            if (client.readyToUse != false)
	            {
                    // eb - removing async
                    //var items = await client.Inventory.GetItems().ConfigureAwait(false);
                    var items = client.Inventory.GetItems().Result;

                    ItemId[] validsIDs = {ItemId.ItemPokeBall,ItemId.ItemGreatBall,ItemId.ItemUltraBall};
	               
	               ListViewItem listViewItem;
	               ItemsListView.Items.Clear();
	               var sum = 0;
	               foreach (  var item in items) {
	                listViewItem = new ListViewItem();
	                listViewItem.Tag = item;
	                listViewItem.Text = getItemName(item.ItemId);
	                listViewItem.ImageKey = item.ItemId.ToString().Replace("Item","");
	                listViewItem.SubItems.Add(""+item.Count);
	                sum +=item.Count;
	                listViewItem.SubItems.Add(""+item.Unseen);
	                ItemsListView.Items.Add(listViewItem);
	               }
	               lblCount.Text=""+ sum;
	            }
            }
            catch (Exception e)
            {

                Logger.Error("[ItemsList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000,1100);
                //Execute(); 
            }
        }
		private string getItemName(ItemId itemID)
        {
            switch (itemID)
            {
                case ItemId.ItemPotion:
                    return "Potion";
                case ItemId.ItemSuperPotion:
                    return "Super Potion";
                case ItemId.ItemHyperPotion:
                    return "Hyper Potion";
                case ItemId.ItemMaxPotion:
                    return "Max Potion";
                case ItemId.ItemRevive:
                    return "Revive";
                case ItemId.ItemIncenseOrdinary:
                    return "Incense";
                case ItemId.ItemPokeBall:
                    return "Poke Ball";
                case ItemId.ItemGreatBall:
                    return "Great Ball";
                case ItemId.ItemUltraBall:
                    return "Ultra Ball";
                case ItemId.ItemRazzBerry:
                    return "Razz Berry";
                case ItemId.ItemIncubatorBasic:
                    return "Egg Incubator";
                case ItemId.ItemIncubatorBasicUnlimited:
                    return "Unlimited Egg Incubator";
                default:
                    return itemID.ToString().Replace("Item", "");
            }
        }
		void RecycleToolStripMenuItemClick(object sender, EventArgs e)
        {

            var item = (ItemData)ItemsListView.SelectedItems[0].Tag;
            int amount = IntegerInput.ShowDialog(1, "How many?", item.Count);
            if (amount > 0)
            {
                taskResponse resp = new taskResponse(false, string.Empty);

                //resp = await RecycleItems(item, amount).ConfigureAwait(false);
                resp = RecycleItems(item, amount).Result;
                if (resp.Status)
                {
                    item.Count -= amount;
                    ItemsListView.SelectedItems[0].SubItems[1].Text = "" + item.Count;
                }
                else
                    MessageBox.Show(resp.Message + " recycle failed!", "Recycle Status", MessageBoxButtons.OK);

            }
        }
		public class taskResponse
        {
            public bool Status { get; set; }
            public string Message { get; set; }
            public taskResponse() { }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
		private static async Task<taskResponse> RecycleItems(ItemData item, int amount)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic.objClient;
                var resp2 = await client.Inventory.RecycleItem(item.ItemId, amount).ConfigureAwait(false);

                if (resp2.Result == RecycleInventoryItemResponse.Types.Result.Success)
                {
                    resp1.Status = true;
                }
                else
                {
                    resp1.Message = item.ItemId.ToString();
                }
            }
            catch (Exception e)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error RecycleItem: " + e.Message);
                await RecycleItems(item, amount).ConfigureAwait(false);
            }
            return resp1;
        }		
        private void num_Max(object sender, EventArgs e)
        {
        	try{
        		var numB = (NumericUpDown) sender;
        		var value = (int) numB.Value;
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "==========Begin Recycle Filter Debug Logging=============");
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "Value Setter Triggered for: " + numB.Name + " New Value: " + numB.Value);
                //Logger.ColoredConsoleWrite(ConsoleColor.DarkGray, "==========End Recycle Filter Debug Logging=============");
                switch (numB.Name) {
        			case "num_MaxPokeballs":
        				GlobalSettings.pokeball = value;
        			break;
        			case "num_MaxGreatBalls":
        				GlobalSettings.greatball = value;
        			break;
        			case "num_MaxUltraBalls":
        				GlobalSettings.ultraball = value;
        			break;
        			case "num_MaxRevives":
        				GlobalSettings.revive = value;
        			break;
        			case "num_MaxPotions":
        				GlobalSettings.potion = value;
        			break;
        			case "num_MaxSuperPotions":
        				GlobalSettings.superpotion = value;
        			break;
        			case "num_MaxHyperPotions":
        				GlobalSettings.hyperpotion = value;
        			break;
        			case "num_MaxTopRevives":
        				GlobalSettings.toprevive = value;
        			break;
        			case "num_MaxTopPotions":
        				GlobalSettings.toppotion = value;
        			break;
        			case "num_MaxRazzBerrys":        		
        				GlobalSettings.berry = value;
        			break;
        				
        		}        
        		 int count = 0;
		            count += GlobalSettings.pokeball + GlobalSettings.greatball + GlobalSettings.ultraball + GlobalSettings.revive
		                + GlobalSettings.potion + GlobalSettings.superpotion + GlobalSettings.hyperpotion + GlobalSettings.berry 
		                + GlobalSettings.toprevive + GlobalSettings.toppotion;
		         text_TotalItemCount.Text = count.ToString();
        	}catch (Exception e1){
        		Logger.ExceptionInfo(e1.ToString());
        	}
        }
        void btnCopy_Click(object sender, EventArgs e)
        {
          foreach ( ListViewItem lvitem in ItemsListView.Items) {
                var item = (ItemData) (lvitem.Tag);
                var controlName = item.ItemId.ToString();
                controlName = controlName.Replace("Item","num_Max") + "s";
                var controls = Controls.Find(controlName,true);
                if (controls.Length > 0 )
                {
                    ((NumericUpDown)controls[0]).Value = item.Count;
                }
          }
        }
        
        private async Task RecycleItems(bool forcerefresh = false)
        {            
            var client = Logic.Logic.objClient;
            var items = await client.Inventory.GetItemsToRecycle(Logic.Logic.Instance.BotSettings.itemRecycleFilter).ConfigureAwait(false);
            foreach (var item in items)
            {
                var transfer = await client.Inventory.RecycleItem((ItemId)item.ItemId, item.Count).ConfigureAwait(false);
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Recycled {item.Count}x {(ItemId)item.ItemId}", LogLevel.Info);
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
            if ((item.ItemId != ItemId.ItemLuckyEgg) && (item.ItemId != ItemId.ItemIncenseOrdinary)&& (item.ItemId != ItemId.ItemTroyDisk))
            {
                MessageBox.Show(getItemName( item.ItemId) + " cannot be used here.");
                return;
            }
            DialogResult dialogResult = MessageBox.Show("Are you sure you want use " + getItemName( item.ItemId) +"?", "Use Warning", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (item.ItemId == ItemId.ItemIncenseOrdinary)
                {
                    GlobalSettings.UseIncenseGUIClick = true;
                }
                if (item.ItemId == ItemId.ItemLuckyEgg)
                {
                    GlobalSettings.UseLuckyEggGUIClick = true;
                }
                if (item.ItemId == ItemId.ItemTroyDisk)
                {
                    GlobalSettings.UseLureGUIClick = true;
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Lure will be used on next pokestop", LogLevel.Info);
                }
            
            }

        }		
	}
}
