/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/01/2017
 * Time: 17:48
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
    /// Description of LabelText.
    /// </summary>
    public partial class LabelText : UserControl
    {
        public LabelText()
        {
            InitializeComponent();
        }
        public string Caption { get{return label1.Text;} set{label1.Text=value;} }
        override public string Text { get{return textBox1.Text;} set{textBox1.Text=value;} }
        public int SeparatorPoint { get{
                return textBox1.Location.X;
            } set{
                textBox1.Size = new Size( this.Size.Width - value -4 ,textBox1.Size.Height);
                textBox1.Location = new Point( value,textBox1.Location.Y);
                label1.Size= new Size(value-10,label1.Size.Height);
            } }
        
    }
}
