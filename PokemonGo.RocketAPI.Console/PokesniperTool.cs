/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 22/09/2016
 * Time: 22:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using Microsoft.Win32;
using System.Text;


namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class PokesniperTool : Form
    {
        const string URI_SCHEME = "pokesniper2";
        const string URI_KEY = "URL:pokesniper2 Protocol";

        static void RegisterUriScheme(string appPath) {
            // HKEY_CLASSES_ROOT\myscheme
            using (RegistryKey hkcrClass = Registry.ClassesRoot.CreateSubKey(URI_SCHEME)) {
                hkcrClass.SetValue(null, URI_KEY);
                hkcrClass.SetValue("URL Protocol", String.Empty, RegistryValueKind.String);

                // use the application's icon as the URI scheme icon
                using (RegistryKey defaultIcon = hkcrClass.CreateSubKey("DefaultIcon")) {
                    string iconValue = String.Format("\"{0}\",0", appPath);
                    defaultIcon.SetValue(null, iconValue);
                }

                // open the application and pass the URI to the command-line
                using (RegistryKey shell = hkcrClass.CreateSubKey("shell")) {
                    using (RegistryKey open = shell.CreateSubKey("open")) {
                        using (RegistryKey command = open.CreateSubKey("command")) {
                            string cmdValue = String.Format("\"{0}\" \"%1\"", appPath);
                            command.SetValue(null, cmdValue);
                        }
                    }
                }
            }
        }
        static void UnregisterUriScheme() {
            Registry.ClassesRoot.DeleteSubKeyTree(URI_SCHEME);
        }
        public PokesniperTool()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }
        void btnInstall_Click(object sender, EventArgs e)
        {
            try {
                RegisterUriScheme(Application.ExecutablePath);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Installed");
                timer1.Enabled = true;
                
            } catch (Exception) {
                MessageBox.Show("To execute this option the bot need executed with administrator privileges");
            }
        }
        void btnUninstall_Click(object sender, EventArgs e)
        {
            try {
                UnregisterUriScheme();
                Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Service Uninstalled");
                timer1.Enabled = false;
            } catch (Exception) {
                MessageBox.Show("To execute this option the bot need executed with administrator privileges");
            }
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            try {                
                var filename = Path.GetTempPath()+"pokesnipper";
                if (File.Exists(filename)){
                    var stream = new FileStream(filename,FileMode.Open);
                    var utf8 = new UTF8Encoding();
                    var reader = new BinaryReader(stream,utf8);
                    string txt =  reader.ReadString();
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Readed URI");
                    reader.Close();
                    stream.Close();
                    File.Delete(filename);
                    /*TODO: code to snipe*/
                }
            } catch (Exception ex) {
                MessageBox.Show( ex.ToString());
                
            }
        }
        void button2_Click(object sender, EventArgs e)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Timer to check URI Stopped");
            timer1.Enabled = false;
        }
        void button1_Click(object sender, EventArgs e)
        {
            Logger.ColoredConsoleWrite(ConsoleColor.DarkYellow, "Timer to check URI Started");
            timer1.Enabled = true;
          
        }
    }
    
}
