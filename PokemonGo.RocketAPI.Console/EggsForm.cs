/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/09/2016
 * Time: 1:13
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
	/// Description of EggsForm.
	/// </summary>
	public partial class EggsForm : Form
	{
		public EggsForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		public void Show(IEnumerable pokemons ){			
			base.Show();
			eggsPanel1.pokemons = (IOrderedEnumerable<PokemonData>) pokemons;
			eggsPanel1.Execute();
		}
	}
}
