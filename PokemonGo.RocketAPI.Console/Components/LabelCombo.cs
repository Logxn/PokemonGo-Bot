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

namespace PokeMaster.Components
{
    /// <summary>
    /// Description of LabelCombo.
    /// </summary>
    public partial class LabelCombo : UserControl
    {
        public LabelCombo()
        {
            InitializeComponent();
        }
        public int SeparatorPoint { get{
                return comboBox1.Location.X;
            } set{
                comboBox1.Location = new Point( value,comboBox1.Location.Y);
                comboBox1.Size = new Size( this.Size.Width - value -4 ,comboBox1.Size.Height);
                label1.Size= new Size(value-10,label1.Size.Height);
            } }
        public string Caption { get{return label1.Text;} set{label1.Text=value;} }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Localizable(true), MergableProperty(false) ]
        public ComboBox.ObjectCollection Items { get{return comboBox1.Items;} }
    }
}
