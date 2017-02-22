using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Logic
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
        }

        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Update");
        public static string file = path + @"\PokemonGo.RocketAPI.Console.exe";
        public static string basedir = AppDomain.CurrentDomain.BaseDirectory;


        private void startDownload()
        {

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri("http://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/Builds-Only/PokemonGo.RocketAPI.Console.exe"), file);

        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            progress.Value = int.Parse(Math.Truncate(percentage).ToString());
            status.Text = $"Downloaded { ConvertBytesToMegabytes(e.BytesReceived)}MB of { ConvertBytesToMegabytes(e.TotalBytesToReceive)} MB";
            comp.Text = $"Completed: {progress.Value}%";
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            CreateBat();
        }


        void checkversion()
        {
            if (GetNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
            {
                startDownload();
            }
            else
            {
                MessageBox.Show("Your client is up-to-date.");
                this.Hide();
            }
        }

        private void CreateBat()
        {
            try
            {
                StreamWriter w = new StreamWriter(basedir + @"\update.bat");
                //w.WriteLine($"taskkill /F /IM PokemonGo.RocketAPI.Console");
                w.WriteLine($"timeout 5 > NUL");
                w.WriteLine($"xcopy /s /y \"{file}\" \"{basedir}\"");
                w.WriteLine($"rmdir /s /q \"{path}\"");
                w.WriteLine($"echo Y");
                w.WriteLine($"start  \"{basedir}\" PokemonGo.RocketAPI.Console.exe");
                w.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            OpenBat();
        }

        private void OpenBat()
        {
            try
            {
                Process.Start($@"{basedir}\update.bat");
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }

        static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024f;
        }

        public static Version GetNewestVersion()
        {
            try
            {
                var match = DownloadServerVersion();

                var gitVersion = new Version(match);

                return gitVersion;

            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
            {
                return wC.DownloadString("https://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/ver.md");
            }
        }
        
        public static void CheckWhileWalking()
        {
            if (GetNewestVersion() > Assembly.GetEntryAssembly().GetName().Version)
            {
                if (Shared.GlobalVars.AutoUpdate)
                {
                    System.Windows.Forms.Form update = new Update();
                    update.ShowDialog();
                }
                else
                {
                    var dialogResult = MessageBox.Show(
                        @"There is an Update on Github. do you want to open it ?", $@"Newest Version: {GetNewestVersion()}, MessageBoxButtons.YesNo");

                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            Process.Start("https://github.com/Logxn/PokemonGo-Bot");
                            break;
                        case DialogResult.No:
                            //nothing   
                            break;
                        case DialogResult.None:
                            break;
                        case DialogResult.OK:
                            break;
                        case DialogResult.Cancel:
                            break;
                        case DialogResult.Abort:
                            break;
                        case DialogResult.Retry:
                            break;
                        case DialogResult.Ignore:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

    }
}
