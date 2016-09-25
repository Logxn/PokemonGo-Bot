/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 16/09/2016
 * Time: 23:48
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
using PokemonGo.RocketAPI.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Data;
using POGOProtos.Inventory;
using System.Linq;
using System.Collections;

namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of EggsPanel.
	/// </summary>
	public partial class EggsPanel : UserControl
	{
		private  ISettings ClientSettings;
		public  IOrderedEnumerable<PokemonData> pokemons = null;
		private IncubatorSelect incubatorSelect = new IncubatorSelect();
		
		public EggsPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			ClientSettings = new Settings(); // this uses Globals variables.
		}
		void BtnRealoadItemsClick(object sender, EventArgs e)
		{
			listView.Items.Clear();
            Execute();
		}
		 public  async void Execute()
		{
			try
            {
                var client = Logic.Logic._client;
	            if (client.readyToUse != false)
	            {
	               var items = await client.Inventory.GetEggs();
	               var incubators = await client.Inventory.GetEggIncubators();
	               var arrStats = await client.Inventory.GetPlayerStats();
                   var stats = arrStats.First();

	              	               	               
	               listView.Items.Clear();
	               
	               ListViewItem listViewItem;	               
	               foreach (  var item in items) {
	                listViewItem = new ListViewItem();
	                listViewItem.Tag = item;
	                listViewItem.Text = ""+item.EggKmWalkedStart;
	               	listViewItem.ImageKey = "egg";
	               	
	               	//EggIncubator incubator = (incubators.Where(i => i.Id == item.EggIncubatorId));
	               	EggIncubator incubator = GetIncubator( incubators, item.EggIncubatorId);
	               	if (incubator !=null){
		               	if (incubator.ItemId==ItemId.ItemIncubatorBasic){
		                	listViewItem.ImageKey = "bincegg";
		                }else if (incubator.ItemId==ItemId.ItemIncubatorBasicUnlimited){
		                	listViewItem.ImageKey = "unincegg";
		                }
	               		listViewItem.Text = ""+Math.Round(incubator.TargetKmWalked - stats.KmWalked, 2);
	               	}
	                listViewItem.SubItems.Add(""+item.EggKmWalkedTarget);	                	
	                if (incubator !=null){
	                	if (pokemons !=null){
		                	var eggPoke = pokemons.FirstOrDefault(x => x.Id == incubator.PokemonId);
		                	if (eggPoke !=null){
		                		listViewItem.SubItems.Add(GetPokemonName(eggPoke.PokemonId));
		                	}else{
		                		listViewItem.SubItems.Add(""+incubator.PokemonId);
		                	}
	                	}else{
	                		listViewItem.SubItems.Add(""+incubator.PokemonId);
	                	}
	                }else{
	                	listViewItem.SubItems.Add(""+GetPokemonName(item.PokemonId));	                	
	                }
	                listViewItem.SubItems.Add(string.Format("{0}% {1}-{2}-{3}", PokemonInfo.CalculatePokemonPerfection(item).ToString("0"), item.IndividualAttack, item.IndividualDefense, item.IndividualStamina));
	                listViewItem.SubItems.Add(GetCreationTime(item.CreationTimeMs));
                    listViewItem.SubItems.Add(string.Format("{0}", item.Move1));
                    listViewItem.SubItems.Add(string.Format("{0} ({1})", item.Move2, PokemonInfo.GetAttack(item.Move2)));
					if (incubator !=null)
						listViewItem.SubItems.Add(string.Format("Uses:{0}",incubator.UsesRemaining));
	                listView.Items.Add(listViewItem);
	               }
	               listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
	            }
	            
            }
            catch (Exception e)
            {

                Logger.Error("[EggsList-Error] " + e.StackTrace);
                await Task.Delay(1000); // Lets the API make a little pause, so we dont get blocked
                Execute();
            }
		}
		
		private EggIncubator GetIncubator(IEnumerable incubators, string id){
		    foreach (EggIncubator incubator in incubators) {
				if (incubator.Id == id)
					return incubator;
		    }
			return null;
		}
		
		private string GetPokemonName(PokemonId pokemonID){
			return StringUtils.getPokemonNameByLanguage(ClientSettings, pokemonID);
		}
		
		private string GetCreationTime(ulong ms){
			return StringUtils.ConvertTimeMSinString(ms , "dd/MM/yyyy HH:mm:ss");
		}
		
		private async void IncubateToolStripMenuItemClick(object sender, EventArgs e)
		{		
			if (incubatorSelect.ShowDialog() == DialogResult.OK ){
				var egg = (PokemonData) listView.SelectedItems[0].Tag;
					
				var incubator = incubatorSelect.selected;
				 								
                var resp = new taskResponse(false, string.Empty);

                resp = await IncubateEgg(incubator, egg);
                if (resp.Status)
                {
	               	if (incubator.ItemId==ItemId.ItemIncubatorBasic){
	                	listView.SelectedItems[0].ImageKey = "bincegg";
	                }else if (incubator.ItemId==ItemId.ItemIncubatorBasicUnlimited){
	                	listView.SelectedItems[0].ImageKey = "unincegg";
	                }
                	
                }
                else
                    MessageBox.Show(resp.Message + " Incubate Egg failed!", "Recycle Status", MessageBoxButtons.OK);
				
			}
		}
		private void BtnRealoadClick(object sender, EventArgs e)
		{
			listView.Items.Clear();
            Execute();
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
		private static async Task<taskResponse> IncubateEgg(EggIncubator item, PokemonData egg)
        {
            taskResponse resp1 = new taskResponse(false, string.Empty);
            try
            {
            	var client = Logic.Logic._client;
            	var resp2 = await client.Inventory.UseItemEggIncubator( item.Id, egg.Id);

                if (resp2.Result == UseItemEggIncubatorResponse.Types.Result.Success)
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
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error IncubateEgg: " + e.Message);
                await IncubateEgg(item, egg);
            }
            return resp1;
        }				
	}
}
