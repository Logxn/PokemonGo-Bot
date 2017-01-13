/*
 * Created by SharpDevelop.
 * User: usuarioIEDC
 * Date: 13/01/2017
 * Time: 12:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Logic.Utils
{
	/// <summary>
	/// Description of CaptchaHelper.
	/// </summary>
	public partial class CaptchaHelper : Form
	{
		public string TOKEN ="";
		public CaptchaHelper()
		{
			InitializeComponent();
			
		}
		public CaptchaHelper(string url)
		{
			InitializeComponent();
			webBrowser1.Navigate(url);
		}
		public void addChallengeUrl(string url)
		{
			webBrowser1.Navigate(url);
		}
		void TimerCheckURLChangesTick(object sender, EventArgs e)
		{
			var url= webBrowser1.Url.AbsolutePath;
			if (url.IndexOf("unity:")>-1)
			{
				TOKEN = url.Substring(url.IndexOf("unity:") +6);
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
