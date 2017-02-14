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

namespace PokemonGo.RocketAPI.Console.Panels
{
    /// <summary>
    /// Description of ShopPanel.
    /// </summary>
    public partial class ShopPanel : UserControl
    {
        public ShopPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        public void Execute()
        {
            try {
                var client = Logic.Logic.objClient;
                if (client.ReadyToUse != false) {
                    var inventory = client.Inventory.GetInventory();
                }
            }catch (Exception ex1){
                Logger.ExceptionInfo(ex1.ToString());
            }
        }
        
    }
}
