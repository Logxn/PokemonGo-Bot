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

        private void Update_Load(object sender, EventArgs e)
        {

        }

        private void startDownload()
        {

            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri("http://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/Builds-Only/PokemonGo.RocketAPI.Console.exe"), file);

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
            if (getNewestVersion() > Assembly.GetExecutingAssembly().GetName().Version)
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
                w.WriteLine($"xcopy /s /y \"{file}\" \"{basedir}\\PokemonGo.RocketAPI.Console.exe\"");
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

        public static Version getNewestVersion()
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
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/ver.md");
        }


    }
}
