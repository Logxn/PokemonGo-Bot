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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokeMaster.Logic.Functions;

namespace PokeMaster.Dialogs
{
    public partial class Update : Form
    {
        /*Not change order of this strings*/
        public static string localFile = Application.ExecutablePath;
        public static string downloadedFile = Path.GetTempFileName();
        public static string baseDir = new FileInfo(localFile).DirectoryName;
        public static string exeName = new FileInfo(localFile).Name;
        public static string remoteFile = "http://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/Builds-Only/" + exeName;
        public static string updateFile = Path.GetTempFileName()+".bat";

        public Update()
        {
            InitializeComponent();
        }


        private void startDownload()
        {

            var webClient = new WebClient();
            webClient.DownloadFileCompleted += Completed;
            webClient.DownloadProgressChanged += ProgressChanged;
            try
            {
                webClient.DownloadFileAsync(new Uri(remoteFile), downloadedFile);
            }
            catch(Exception e)
            {
                MessageBox.Show($"Failed downloading the newest Update: {e.Message}", "Oh Snap!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

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

        private void Update_Load(object sender, EventArgs e)
        {
            checkversion();
        }

        void checkversion()
        {
            if (Setout.GetServerVersion() > new Version (Application.ProductVersion)) {
                startDownload();
            } else {
                MessageBox.Show("Your client is up-to-date.");
                Hide();
            }
        }

        private void CreateBat()
        {
            try {
                var w = new StreamWriter(updateFile);
                w.WriteLine("timeout 2 > NUL");
                w.WriteLine($"move /y \"{downloadedFile}\" \"{localFile}\"");
                w.WriteLine("echo Y");
                w.WriteLine($"start {localFile}");
                w.WriteLine($"del /f \"{updateFile}\"");
                w.Close();
                OpenBat();
            } catch (Exception e) {
                MessageBox.Show($"Error Creating Bat: {e.Message}", "Oh Snap!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private void OpenBat()
        {
            try {
                var psi = new ProcessStartInfo (updateFile);
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(psi);
                Environment.Exit(0);
            } catch (Exception e) {
                MessageBox.Show($"Error Opening Bat: {e.Message}", "Oh Snap!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
    }
}
