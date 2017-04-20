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
using PokeMaster.Dialogs;
using PokeMaster.Logic.Utils;
using POGOProtos.Enums;
using POGOProtos.Data;
using POGOProtos.Inventory;
using System.Linq;
using System.Collections;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;

namespace PokeMaster
{
    /// <summary>
    /// Description of EggsPanel.
    /// </summary>
    public partial class EggsPanel : UserControl
    {
        public  IOrderedEnumerable<PokemonData> pokemons = null;
        private IncubatorSelect incubatorSelect = new IncubatorSelect();
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();

        public EggsPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            th.Translate(this);
        }
        void BtnRealoadItemsClick(object sender, EventArgs e)
        {
            listView.Items.Clear();
            Execute();
        }
        public void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    var items = client.Inventory.GetEggs();
                    var incubators = client.Inventory.GetEggIncubators(); 
                    var arrStats = client.Inventory.GetPlayerStats();
                    var stats = arrStats.First();
	              	               	               
                    listView.Items.Clear();
	               
                    ListViewItem listViewItem;
                    foreach (var item in items) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = "" + item.EggKmWalkedStart;
                        listViewItem.ImageKey = "" + (item.EggKmWalkedTarget - item.EggKmWalkedStart) + "km";
	               	
                        EggIncubator incubator = GetIncubator(incubators, item.EggIncubatorId);
                        if (incubator != null) {
                            if (incubator.ItemId == ItemId.ItemIncubatorBasic) {
                                listViewItem.ImageKey = "bincegg";
                            } else if (incubator.ItemId == ItemId.ItemIncubatorBasicUnlimited) {
                                listViewItem.ImageKey = "unincegg";
                            }
                            listViewItem.Text = "" + Math.Round(incubator.TargetKmWalked - stats.KmWalked, 2);
                        }
                        listViewItem.SubItems.Add("" + item.EggKmWalkedTarget);	                	
                        if (incubator != null) {
                            listViewItem.SubItems.Add("" + incubator.PokemonId.ToString("X"));
                        } else {
                            listViewItem.SubItems.Add(th.TS(item.PokemonId.ToString()));
                        }
                        listViewItem.SubItems.Add(string.Format("{0}% {1}-{2}-{3}", PokemonInfo.CalculatePokemonPerfection(item).ToString("0"), item.IndividualAttack, item.IndividualDefense, item.IndividualStamina));
                        listViewItem.SubItems.Add(GetCreationTime(item.CreationTimeMs));
                        listViewItem.SubItems.Add(string.Format("{0}", item.Move1));
                        listViewItem.SubItems.Add(string.Format("{0} ({1})", item.Move2, PokemonInfo.GetAttack(item.Move2)));
                        if (incubator != null)
                            listViewItem.SubItems.Add(string.Format("Uses:{0}", incubator.UsesRemaining));
                        listView.Items.Add(listViewItem);
                    }
                    listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    RefreshTitle();
                }
	            
            } catch (Exception e) {

                Logger.Error("[EggsList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000, 1100);
                //Execute();
            }
        }
        private EggIncubator GetIncubator(IEnumerable incubators, string id)
        {
            foreach (EggIncubator incubator in incubators) {
                if (incubator.Id == id)
                    return incubator;
            }
            return null;
        }
        public void RefreshTitle()
        {
            var txt = th.TS("Eggs");
            if (Parent != null) {
                txt += ": " + listView.Items.Count;
            }
            Parent.Text = txt;
        }		
        private string GetPokemonName(PokemonId pokemonID)
        {
            return  th.TS(pokemonID.ToString());
        }
		
        private string GetCreationTime(ulong ms)
        {
            return StringUtils.ConvertTimeMSinString(ms, "yyyy/MM/dd HH:mm:ss");
        }
		
        private void IncubateToolStripMenuItemClick(object sender, EventArgs e)
        {		
            if (incubatorSelect.ShowDialog() == DialogResult.OK) {
                var egg = (PokemonData)listView.SelectedItems[0].Tag;
					
                var incubator = incubatorSelect.selected;
				 								
                var resp = new taskResponse(false, string.Empty);

                //resp = await IncubateEgg(incubator, egg).ConfigureAwait(false);
                resp = IncubateEgg(incubator, egg).Result;
                if (resp.Status) {
                    if (incubator.ItemId == ItemId.ItemIncubatorBasic) {
                        listView.SelectedItems[0].ImageKey = "bincegg";
                    } else if (incubator.ItemId == ItemId.ItemIncubatorBasicUnlimited) {
                        listView.SelectedItems[0].ImageKey = "unincegg";
                    }
                	
                } else
                    MessageBox.Show(resp.Message + th.TS(" Incubate Egg failed!"), th.TS("Recycle Status"), MessageBoxButtons.OK);
				
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
            public taskResponse()
            {
            }
            public taskResponse(bool status, string message)
            {
                Status = status;
                Message = message;
            }
        }
        private static async Task<taskResponse> IncubateEgg(EggIncubator item, PokemonData egg)
        {
            var resp1 = new taskResponse(false, string.Empty);
            try {
                var client = Logic.Logic.objClient;
                var resp2 =  client.Inventory.UseItemEggIncubator(item.Id, egg.Id);

                if (resp2.Result == UseItemEggIncubatorResponse.Types.Result.Success) {
                    resp1.Status = true;
                } else {
                    resp1.Message = item.ItemId.ToString();
                }
            } catch (Exception e) {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error IncubateEgg: " + e.Message);
                await IncubateEgg(item, egg).ConfigureAwait(false);
            }
            return resp1;
        }
        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var order = (sender as ListView).Sorting;
            listView.ListViewItemSorter = new Components.ListViewItemComparer(e.Column, order);
            (sender as ListView).Sorting = order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }
    }

}
