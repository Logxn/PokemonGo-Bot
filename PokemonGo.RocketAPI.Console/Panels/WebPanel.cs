/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/01/2017
 * Time: 0:56
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
    /// Description of WebPanel.
    /// </summary>
    public partial class WebPanel : UserControl
    {
        public WebPanel()
        {
            InitializeComponent();
        }
        public void ChangeURL(string weburl){
            textBox1.Text = weburl;
        	webBrowser1.Navigate(weburl);
        }
        public void AddButtonClick(System.EventHandler evh){
        	this.button1.Click += evh;
        }
		
    }
}
