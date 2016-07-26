using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Exceptions;
using System.Reflection;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.Drawing;

namespace PokemonGo.RocketAPI.Console
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        { 
            if (args == null || args.Length == 0) { 
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GUI());
            } else if (args[0].Contains("-nogui"))
            {
                Logger.Write(Color.Red, "You added -nogui! If you didnt setup correctly with the GUI. It wont work.");
            } else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GUI());
            }

            Application.Run(new Display());
            //Logger.SetLogger(new Logging.ConsoleLogger(LogLevel.Info));

            Task.Run(() =>
            {

                try
                {
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
                catch (PtcOfflineException)
                {
                    Logger.Write(Color.Red, "PTC Servers are probably down OR you credentials are wrong.", LogLevel.Error);
                    Logger.Write(Color.Red, "Trying again in 20 seconds...");
                    Thread.Sleep(20000);
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
                catch (AccountNotVerifiedException)
                {
                    Logger.Write(Color.Red, "Your PTC Account is not activated. Exiting in 10 Seconds.");
                    Thread.Sleep(10000);
                    Environment.Exit(0);
                }
                
                catch (Exception ex)
                {
                    Logger.Write(Color.Red, $"Unhandled exception: {ex}", LogLevel.Error);
                    Logger.Error("Restarting in 20 Seconds.");
                    Thread.Sleep(200000);
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
            });
            System.Console.ReadLine();
        }
    }
    public static class Globals
    {
        public static Enums.AuthType acc = Enums.AuthType.Ptc;
        public static bool defLoc = true;
        public static string username = "empty";
        public static string password = "empty";
        public static double latitute;
        public static double longitude;
        public static double altitude;
        public static double speed;
        public static int radius;
        public static bool transfer;
        public static int duplicate;
        public static bool evolve;
        public static int maxCp;
        public static int pokeball;
        public static int greatball;
        public static int ultraball;
        public static int masterball;
        public static int revive;
        public static int potion;
        public static int superpotion;
        public static int hyperpotion;
        public static int toppotion;
        public static int toprevive;
        public static int berry;
        public static List<PokemonId> noTransfer = new List<PokemonId>();
        public static List<PokemonId> noCatch = new List<PokemonId>();
        public static List<PokemonId> doEvolve = new List<PokemonId>();
        public static string telAPI = "empty";
        public static string telName = "empty";
        public static int telDelay;

        public static int navigation_option = 1;
        public static bool useluckyegg = true;
        public static bool gerNames = false;
    }
}