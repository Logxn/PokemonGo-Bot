﻿using System;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Exceptions;
using System.Reflection;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using PokemonGo.RocketAPI.GeneratedCode;
using System.IO;
using PokemonGo.RocketAPI.Logic.Utils;

namespace PokemonGo.RocketAPI.Console
{
    class Program
    {
        public static string path = Directory.GetCurrentDirectory();
        public static string account = path + "\\Config.txt";
        public static string items = path + "\\Items.txt";
        public static string keep = path + "\\noTransfer.txt";
        public static string ignore = path + "\\noCatch.txt";
        public static string evolve = path + "\\Evolve.txt";

        [STAThread]
        static void Main(string[] args)
        {

            if (args != null && args.Length > 0 && args[0].Contains("-nogui"))
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "You added -nogui! If you didnt setup correctly with the GUI. It wont work.");
                foreach (PokemonId pokemon in Enum.GetValues(typeof(PokemonId)))
                {
                    if (pokemon.ToString() != "Missingno")
                    {
                        GUI.gerEng[StringUtils.getPokemonNameGer(pokemon)] = pokemon.ToString();
                    }
                }
                int i = 0;
                if (File.Exists(account))
                {
                    string[] lines = File.ReadAllLines(@account);
                    foreach (string line in lines)
                    {
                        switch (i)
                        {
                            case 0:
                                if (line == "Google")
                                    Globals.acc = Enums.AuthType.Google;
                                else
                                    Globals.acc = Enums.AuthType.Ptc;
                                break;
                            case 1:
                                Globals.username = line;
                                break;
                            case 2:
                                Globals.password = line;
                                break;
                            case 3:
                                Globals.latitute = double.Parse(line.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                                break;
                            case 4:
                                Globals.longitude = double.Parse(line.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                                break;
                            case 5:
                                Globals.altitude = double.Parse(line.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                                break;
                            case 6:
                                Globals.speed = double.Parse(line.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                                break;
                            case 7:
                                Globals.radius = int.Parse(line);
                                break;
                            case 8:
                                Globals.defLoc = bool.Parse(line);
                                break;
                            case 9:
                                Globals.transfer = bool.Parse(line);
                                break;
                            case 10:
                                Globals.duplicate = int.Parse(line);
                                break;
                            case 11:
                                Globals.evolve = bool.Parse(line);
                                break;
                            case 12:
                                Globals.maxCp = int.Parse(line);
                                break;
                            case 13:
                                Globals.telAPI = line;
                                break;
                            case 14:
                                Globals.telName = line;
                                break;
                            case 15:
                                Globals.telDelay = int.Parse(line);
                                break;
                            case 16:
                                Globals.telDelay = int.Parse(line);
                                break;
                            case 17:
                                Globals.useluckyegg = bool.Parse(line);
                                break;
                            case 18:
                                Globals.gerNames = bool.Parse(line);
                                break;
                            case 19:
                                Globals.useincense = bool.Parse(line);
                                break;
                        }
                        i++;
                    }
                }

                if (File.Exists(items))
                {
                    string[] lines = File.ReadAllLines(@items);
                    i = 0;
                    foreach (string line in lines)
                    {
                        switch (i)
                        {
                            case 0:
                                Globals.pokeball = int.Parse(line);
                                break;
                            case 1:
                                Globals.greatball = int.Parse(line);
                                break;
                            case 2:
                                Globals.ultraball = int.Parse(line);
                                break;
                            case 3:
                                Globals.revive = int.Parse(line);
                                break;
                            case 4:
                                Globals.potion = int.Parse(line);
                                break;
                            case 5:
                                Globals.superpotion = int.Parse(line);
                                break;
                            case 6:
                                Globals.hyperpotion = int.Parse(line);
                                break;
                            case 7:
                                Globals.berry = int.Parse(line);
                                break;
                            case 8:
                                Globals.masterball = int.Parse(line);
                                break;
                            case 9:
                                Globals.toppotion = int.Parse(line);
                                break;
                            case 10:
                                Globals.toprevive = int.Parse(line);
                                break;
                        }
                        i++;
                    }
                }

                if (File.Exists(keep))
                {
                    string[] lines = System.IO.File.ReadAllLines(@keep);
                    foreach (string line in lines)
                    {
                        if (line != "")
                            if (Globals.gerNames)
                                Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), GUI.gerEng[line]));
                            else
                                Globals.noTransfer.Add((PokemonId)Enum.Parse(typeof(PokemonId), line));
                    }
                }

                if (File.Exists(ignore))
                {
                    string[] lines = System.IO.File.ReadAllLines(@ignore);
                    foreach (string line in lines)
                    {
                        if (line != "")
                            if (Globals.gerNames)
                                Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), GUI.gerEng[line]));
                            else
                                Globals.noCatch.Add((PokemonId)Enum.Parse(typeof(PokemonId), line));
                    }
                }

                if (File.Exists(evolve))
                {
                    string[] lines = System.IO.File.ReadAllLines(@evolve);
                    foreach (string line in lines)
                    {
                        if (line != "")
                            if (Globals.gerNames)
                                Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), GUI.gerEng[line]));
                            else
                                Globals.doEvolve.Add((PokemonId)Enum.Parse(typeof(PokemonId), line));
                    }
                }

            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GUI());
            }
            

            Logger.SetLogger(new Logging.ConsoleLogger(LogLevel.Info));
            
            Task.Run(() =>
            {

                CheckVersion();

                try
                {
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
                catch (PtcOfflineException)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "PTC Servers are probably down OR you credentials are wrong.", LogLevel.Error);
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Trying again in 20 seconds...");
                    Thread.Sleep(20000);
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
                catch (AccountNotVerifiedException)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Your PTC Account is not activated. Exiting in 10 Seconds.");
                    Thread.Sleep(10000);
                    Environment.Exit(0);
                }
                
                catch (Exception ex)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Unhandled exception: {ex}", LogLevel.Error);
                    Logger.Error("Restarting in 20 Seconds.");
                    Thread.Sleep(200000);
                    new Logic.Logic(new Settings()).Execute().Wait();
                }
            });
            System.Console.ReadLine();
        }

        public static void CheckVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        string.Format(
                            "{0}.{1}.{2}.{3}",
                            match.Groups[1],
                            match.Groups[2],
                            match.Groups[3],
                            match.Groups[4]));
                if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    //ColoredConsoleWrite(ConsoleColor.Yellow, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Red, "There is a new Version available: " + gitVersion);
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Its recommended to use the newest Version.");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Starting in 10 Seconds.");
                Thread.Sleep(10000);
            }
            catch (Exception)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.White, "Unable to check for updates now...");
            }
        }

        public static Version getNewestVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return Assembly.GetExecutingAssembly().GetName().Version;
                var gitVersion =
                    new Version(
                        string.Format(
                            "{0}.{1}.{2}.{3}",
                            match.Groups[1],
                            match.Groups[2],
                            match.Groups[3],
                            match.Groups[4]));

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
                        "https://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/PokemonGo.RocketAPI.Console/Properties/AssemblyInfo.cs");
        }
    }
    public static class Globals
    {
        public static Enums.AuthType acc = Enums.AuthType.Google;
        public static bool defLoc = true;
        public static string username = "empty";
        public static string password = "empty";
        public static double latitute = 40.764883;
        public static double longitude = -73.972967;
        public static double altitude = 15.173855;
        public static double speed = 50;
        public static int radius = 5000;
        public static bool transfer = true;
        public static int duplicate = 3;
        public static bool evolve = true;
        public static int maxCp = 999;
        public static int pokeball = 20;
        public static int greatball = 50;
        public static int ultraball = 100;
        public static int masterball = 200;
        public static int revive = 20;
        public static int potion = 0;
        public static int superpotion = 0;
        public static int hyperpotion = 50;
        public static int toppotion = 100;
        public static int toprevive = 50;
        public static int berry = 50;
        public static List<PokemonId> noTransfer = new List<PokemonId>();
        public static List<PokemonId> noCatch = new List<PokemonId>();
        public static List<PokemonId> doEvolve = new List<PokemonId>();
        public static string telAPI = "empty";
        public static string telName = "empty";
        public static int telDelay = 5000;

        public static int navigation_option = 1;
        public static bool useluckyegg = true;
        public static bool useincense = true;
        public static bool gerNames = false;
    }
}