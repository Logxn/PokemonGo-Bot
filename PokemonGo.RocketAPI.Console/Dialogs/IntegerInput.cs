/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/09/2016
 * Time: 3:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of IntegerInput.
	/// </summary>
	public partial class IntegerInput : Form
	{
		public IntegerInput()
		{
			InitializeComponent();
			
		}
		public static int ShowDialog(int text, string caption, int maximum)
        {
            var prompt = new IntegerInput()
            {
            	Text = caption             	
            };
            prompt.amount.Value = text;
            prompt.amount.Maximum = maximum;

            return prompt.ShowDialog() == DialogResult.OK ?  (int) prompt.amount.Value : 0;
        }
		void ConfirmationClick(object sender, EventArgs e)
		{
			Close();
		}		
	}
}
