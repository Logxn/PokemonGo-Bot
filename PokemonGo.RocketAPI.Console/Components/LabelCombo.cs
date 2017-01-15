/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/01/2017
 * Time: 16:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console.Components
{
    /// <summary>
    /// Description of LabelCombo.
    /// </summary>
    public partial class LabelCombo : UserControl
    {
        public string TextLabel { get{return label1.Text;} set{label1.Text=value;} }
        public int SelectedIndex { get{return comboBox1.SelectedIndex;} 
            set{
                if (comboBox1!=null){
                    try {
                        comboBox1.SelectedIndex=value;
                    } catch (Exception) {
                    }
                }
            } 
        }
        public ComboBox.ObjectCollection Items { get{return comboBox1.Items;} }
        public LabelCombo()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
    }
}
