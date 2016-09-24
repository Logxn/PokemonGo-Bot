/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/09/2016
 * Time: 16:57
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
	/// Description of IncubatorSelect.
	/// </summary>
	public partial class IncubatorSelect : Form
	{
		public EggIncubator selected=null;
		public IncubatorSelect()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();			
		}
		public async void Execute(){
			var client = Logic.Logic._client;
			if (client.readyToUse != false)
			{
				var incubators = await client.Inventory.GetEggIncubators();
  				listView.Items.Clear();	              
	            ListViewItem listViewItem;					
				foreach (  var item in incubators) {
					listViewItem = new ListViewItem();
					listViewItem.Tag = item;
					listViewItem.Text = ""+item.Id;
					listViewItem.SubItems.Add(""+item.ItemId);
					listViewItem.SubItems.Add(""+item.UsesRemaining);	                					               	
					listViewItem.SubItems.Add(""+item.IncubatorType);
					listViewItem.SubItems.Add(""+item.StartKmWalked);
					listViewItem.SubItems.Add(""+item.TargetKmWalked);
					listViewItem.SubItems.Add(""+item.PokemonId);					
					listView.Items.Add(listViewItem);			   	
				}
	             listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}			
		}
		void ButtonOkClick(object sender, EventArgs e)
		{			
			if (listView.SelectedItems.Count < 1)
			{
				MessageBox.Show("Please Select an incubator.");
			}
			else
			{
				selected = (EggIncubator)listView.SelectedItems[0].Tag;
				DialogResult = DialogResult.OK;
				this.Close();
			}
			
		}
		public DialogResult ShowDialog(){
			Execute();
			return base.ShowDialog();
		}
	}
}
