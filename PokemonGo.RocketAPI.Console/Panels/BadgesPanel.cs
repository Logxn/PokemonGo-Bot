/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 05/08/2017
 * Time: 1:07
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
    /// Description of BadgesPanel.
    /// </summary>
    public partial class BadgesPanel : UserControl
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        
        public BadgesPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            th.Translate(this);
        }
        void btnReaload_Click(object sender, EventArgs e)
        {
            listView.Items.Clear();
            Execute();
        }
        public void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    
                    var badges = client.Player.GetPlayerProfile();
                    listView.Items.Clear();
                   
                    ListViewItem listViewItem;
                    foreach (var item in badges.Badges) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = item.BadgeType.ToString().Replace("Badge","");
                        // TODO: put gold, silver an cupper icons.
                        //listViewItem.ImageKey
                       
                        listViewItem.SubItems.Add("" + item.Rank);
                        listViewItem.SubItems.Add("" + item.CurrentValue);
                        listViewItem.SubItems.Add("" + ((item.EndValue==-1)?"":item.EndValue.ToString()));
                        listView.Items.Add(listViewItem);
                    }
                    foreach (var item in badges.GymBadges.GymBadge) {
                        listViewItem = new ListViewItem();
                        listViewItem.Tag = item;
                        listViewItem.Text = "GYM: " + item.Name;
                        // TODO: put gold, silver an bronce icons.
                        //listViewItem.ImageKey
                        listViewItem.SubItems.Add("" + item.GymBadgeType.ToString().Replace("GymBadge",""));
                        listViewItem.SubItems.Add("" + item.Progress);
                        listViewItem.SubItems.Add("" );
                        listView.Items.Add(listViewItem);
                    }
                    //listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
                
            } catch (Exception e) {
                Logger.Error("[EggsList-Error] " + e.StackTrace);
                RandomHelper.RandomSleep(1000, 1100);
            }
        }
    }
}
