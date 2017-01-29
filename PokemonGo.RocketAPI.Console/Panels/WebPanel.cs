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
using System.Diagnostics;
using Microsoft.Win32;

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
        public void EnableIE11Emulation()
        {
            if (!existValue())
                addRegValue();
        }
                
        private const string keyName = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
        private void addRegValue()
        {
           var arguments = $"ADD \"HKCU\\{keyName}\" /v { AppDomain.CurrentDomain.FriendlyName} /t REG_DWORD /d 10001";
           var psInfo = new ProcessStartInfo("reg.exe",arguments);
           psInfo.WindowStyle = ProcessWindowStyle.Hidden;
           Process.Start(psInfo);
        }
        private bool existValue()
        {
            if (Registry.CurrentUser.GetValue(keyName+"\\"+AppDomain.CurrentDomain.FriendlyName, null) == null)
                return false;
            return true;
        }
    }
}
