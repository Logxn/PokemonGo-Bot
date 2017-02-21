/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 18/01/2017
 * Time: 9:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console.Dialogs
   
{
    /// <summary>
    /// Description of GUIAvatar.
    /// </summary>
    public partial class AvatarSelect : Form
    {
        public AvatarSelect()
        {
            InitializeComponent();
            tutorialPanel1.LoadData();
            Helper.TranslatorHelper.getInstance().Translate(this);
        }
        void ButtonOkClick(object sender, EventArgs e)
        {
            tutorialPanel1.SaveData();
            Close();
        }
    }
}
