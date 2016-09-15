/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/09/2016
 * Time: 18:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using POGOProtos.Networking.Responses;
using POGOProtos.Inventory.Item;
using PokemonGo.RocketAPI.Rpc;

namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of Items.
	/// </summary>
	public partial class Items : Form
	{
		private static Client client;
		public Items()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
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
		
        private void Items_Load(object sender, EventArgs e)
        {
            Execute();
        }

        private void Form_StopClose(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
         
        private async void Execute()
        {
			await check();
			
			client = Logic.Logic._client;
            if (client.readyToUse != false)
            {
               var items = await client.Inventory.GetItems();
              
               ItemId[] validsIDs = {ItemId.ItemPokeBall,ItemId.ItemGreatBall,ItemId.ItemUltraBall};
               
               ListViewItem listViewItem;
               foreach (  var item in items) {
                listViewItem = new ListViewItem();
                listViewItem.Tag = item;
                listViewItem.Text = getItemName(item.ItemId);
                listViewItem.SubItems.Add(""+item.Count);
                listViewItem.SubItems.Add(""+item.Unseen);
                ItemsListView.Items.Add(listViewItem);
               }
            }
		}
		
		private string getItemName (ItemId itemID){
			switch (itemID) {
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
				case ItemId.ItemMasterBall:
					return "Master Ball";
				case ItemId.ItemRazzBerry:
					return "Razz Berry";
				case ItemId.ItemIncubatorBasic:
					string str1;					
					return "Egg Incubator";
				default:
					return itemID.ToString().Replace("Item","") ;
			}
		}
		async void RecycleToolStripMenuItemClick(object sender, EventArgs e)
		{
						
		    var item = (ItemData) ItemsListView.SelectedItems[0].Tag;
			int amount = IntegerInput.ShowDialog(1,"How many?",item.Count);
			if (amount > 0 ){
		        taskResponse resp = new taskResponse(false, string.Empty);
		
	            resp = await RecycleItems(item, amount);
		        if (resp.Status)
		        {
		        	item.Count -= amount;
		        	ItemsListView.SelectedItems[0].SubItems[1].Text = ""+item.Count;
		        }
		        else
		            MessageBox.Show(resp.Message + " recycle failed!", "Recycle Status", MessageBoxButtons.OK);
				
			}
		}
		private static async Task<taskResponse> RecycleItems(ItemData item, int amount)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
                var resp2 = await client.Inventory.RecycleItem( item.ItemId, amount);

                if (resp2.Result ==  RecycleInventoryItemResponse.Types.Result.Success)
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
                await RecycleItems(item , amount);
            }
            return resp1;
        }		
  		
		void BtnreloadClick(object sender, EventArgs e)
		{
			ItemsListView.Items.Clear();
            Execute();
		}
		
		public async Task check()
        {
            while (true)
            {
                try
                {
                    if (Logic.Logic._client != null && Logic.Logic._client.readyToUse != false)
                    {
                        break;
                    }
                }
                catch (Exception) { }
            }
        }
		void ContextMenuStrip1Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
	
		}
	}
	
}
