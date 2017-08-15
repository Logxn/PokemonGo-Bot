/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/08/2017
 * Time: 18:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;

namespace PokeMaster.Panels
{
    /// <summary>
    /// Description of PokedexPanel.
    /// </summary>
    public partial class PokedexPanel : UserControl
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public PokedexPanel()
        {

            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            th.Translate(this);
            var imageList = PokemonsPanel.createImageList();
            listView.SmallImageList = imageList;
            listView.LargeImageList = imageList;

            listView.Columns.Clear();
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Pokemon")));
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Times Caught")));
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Shinies Caught")));
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Times Encountered")));
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Shinies Encountered")));
            listView.Columns.Add(PokemonsPanel.CreateColumn(th.TS("Evolution Stones")));

        }
        public void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    
                    var entries = client.Inventory.GetPokedexEntries();
                    listView.Items.Clear();
                   
                    ListViewItem listViewItem;
                    foreach (var item in entries) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = item.PokemonId.ToString();
                        listViewItem.ImageKey = "Missingno";
                        if (item.TimesCaptured> 0)
                            listViewItem.ImageKey = item.PokemonId.ToString();
                        listViewItem.SubItems.Add("" + item.TimesCaptured);
                        //listViewItem.SubItems.Add("" + item.CapturedCostumes);
                        //listViewItem.SubItems.Add("" + item.CapturedForms);
                        //listViewItem.SubItems.Add("" + item.CapturedGenders);
                        listViewItem.SubItems.Add("" + item.CapturedShiny);
                        listViewItem.SubItems.Add("" + item.TimesEncountered);
                        //listViewItem.SubItems.Add("" + item.EncounteredCostumes);
                        //listViewItem.SubItems.Add("" + item.EncounteredForms);
                        //listViewItem.SubItems.Add("" + item.EncounteredGenders);
                        listViewItem.SubItems.Add("" + item.EncounteredShiny);
                        listViewItem.SubItems.Add("" + item.EvolutionStones);
                        listView.Items.Add(listViewItem);
                    }
                    listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
                
            } catch (Exception e) {
                Logger.Error("[EggsList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000, 1100);
            }
        }
        void btnReaload_Click(object sender, EventArgs e)
        {
          listView.Items.Clear();
          Execute();
        }
    }
}
