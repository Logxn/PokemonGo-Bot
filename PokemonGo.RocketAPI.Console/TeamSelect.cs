/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 19/09/2016
 * Time: 0:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using POGOProtos.Enums;

namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of TeamSelect.
	/// </summary>
	public partial class TeamSelect : Form
	{
		public TeamColor selected;
		public TeamSelect()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

		}
		void ButtonOkClick(object sender, EventArgs e)
		{
			if (listView.SelectedItems.Count < 1)
			{
				MessageBox.Show("Please Select a Team.");
			}
			else
			{
				selected = TeamColor.Neutral;
				switch (listView.SelectedItems[0].Text) {
						case "Mystic": selected = TeamColor.Blue;
						break;
						case "Valor": selected = TeamColor.Red;
						break;
						case "Instinct": selected = TeamColor.Yellow;
						break;
				}
				
				DialogResult = DialogResult.OK;
				this.Close();
			}
		}
	}
}
