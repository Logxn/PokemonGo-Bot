/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 13/01/2017
 * Time: 12:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PokeMaster.Logic.Utils
{
    /// <summary>
    /// Description of CaptchaHelper.
    /// </summary>
    public partial class CaptchaHelper : Form
    {
        public string TOKEN ="";
        public bool finished = false;
        public bool IsOK = false;
        public CaptchaHelper()
        {
            finished = false;
            InitializeComponent();
            EnableIE11Emulation();
        }
        public CaptchaHelper(string url)
        {
            finished = false;
            InitializeComponent();
            EnableIE11Emulation();
            webBrowser1.Navigate(url);
        }
        public void addChallengeUrl(string url)
        {
            webBrowser1.Navigate(url);
        }
        void TimerCheckURLChangesTick(object sender, EventArgs e)
        {
            try {
                var url= webBrowser1.Url.AbsolutePath;
                if (url.IndexOf("unity:")>-1)
                {
                    TOKEN = url.Substring(url.IndexOf("unity:") +6);
                    DialogResult = DialogResult.OK;
                    IsOK = true;
                    finished = true;
                    Close();
                }
                
            } catch (Exception) {
                
                throw;
            }
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
