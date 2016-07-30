namespace PokemonGo.RocketAPI.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.Exceptions;
    using PokemonGo.RocketAPI.GeneratedCode;
    using PokemonGo.RocketAPI.Logic.Utils;

    internal class Program
    {
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string account = Path.Combine(path, "Config.txt");
        public static string evolve = Path.Combine(path, "Evolve.txt");
        public static string ignore = Path.Combine(path, "noCatch.txt");
        public static string items = Path.Combine(path, "Items.txt");
        public static string keep = Path.Combine(path, "noTransfer.txt");

        public static void CheckVersion()
        {
            try
            {
                var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(DownloadServerVersion());

                if (!match.Success)
                    return;
                var gitVersion = new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));
                if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    // ColoredConsoleWrite(ConsoleColor.Yellow, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
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

        public static string DownloadServerVersion()
        {
            using (var wC = new WebClient())
                return wC.DownloadString("https://raw.githubusercontent.com/Ar1i/PokemonGo-Bot/master/PokemonGo.RocketAPI.Console/Properties/AssemblyInfo.cs");
        }

        public static Version getNewestVersion()
        {
            try
            {
                var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(DownloadServerVersion());

                if (!match.Success)
                    return Assembly.GetExecutingAssembly().GetName().Version;
                var gitVersion = new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));

                return gitVersion;
            }
            catch (Exception)
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            if (args != null && args.Length > 0 && args[0].Contains("-nogui"))
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "You added -nogui! If you didnt setup correctly with the GUI. It wont work.");
                foreach (PokemonId pokemon in Enum.GetValues(typeof (PokemonId)))
                {
                    if (pokemon.ToString() != "Missingno")
                    {
                        GUI.gerEng[StringUtils.getPokemonNameGer(pokemon)] = pokemon.ToString();
                    }
                }

                var i = 0;
                if (File.Exists(account))
                {
                    var lines = File.ReadAllLines(account);
                    foreach (var line in lines)
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

                            case 21:
                                Globals.ivmaxpercent = int.Parse(line);
                                break;

                            case 23:
                                Globals.keepPokemonsThatCanEvolve = bool.Parse(line);
                                break;
                        }

                        i++;
                    }
                }

                if (File.Exists(items))
                {
                    var lines = File.ReadAllLines(items);
                    i = 0;
                    foreach (var line in lines)
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
                    var lines = File.ReadAllLines(keep);
                    foreach (var line in lines)
                    {
                        if (line != string.Empty)
                            if (Globals.gerNames)
                                Globals.noTransfer.Add((PokemonId) Enum.Parse(typeof (PokemonId), GUI.gerEng[line]));
                            else
                                Globals.noTransfer.Add((PokemonId) Enum.Parse(typeof (PokemonId), line));
                    }
                }

                if (File.Exists(ignore))
                {
                    var lines = File.ReadAllLines(ignore);
                    foreach (var line in lines)
                    {
                        if (line != string.Empty)
                            if (Globals.gerNames)
                                Globals.noCatch.Add((PokemonId) Enum.Parse(typeof (PokemonId), GUI.gerEng[line]));
                            else
                                Globals.noCatch.Add((PokemonId) Enum.Parse(typeof (PokemonId), line));
                    }
                }

                if (File.Exists(evolve))
                {
                    var lines = File.ReadAllLines(evolve);
                    foreach (var line in lines)
                    {
                        if (line != string.Empty)
                            if (Globals.gerNames)
                                Globals.doEvolve.Add((PokemonId) Enum.Parse(typeof (PokemonId), GUI.gerEng[line]));
                            else
                                Globals.doEvolve.Add((PokemonId) Enum.Parse(typeof (PokemonId), line));
                    }
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GUI());

                if (Globals.pokeList)
                {
                    Task.Run(() =>
                    {
                        var pokemonList = new Pokemons();
                        pokemonList.ShowDialog();

                        // Application.Run(new Pokemons());
                    });
                }
            }

            // Application.Run(new Pokemons());
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
    }

    public static class Globals
    {
        public static AuthType acc = Enums.AuthType.Google;
        public static double altitude = 15.173855;
        public static int berry = 50;
        public static bool defLoc = true;
        public static List<PokemonId> doEvolve = new List<PokemonId>();
        public static int duplicate = 3;
        public static bool evolve = true;
        public static bool gerNames;
        public static int greatball = 50;
        public static int hyperpotion = 50;
        public static int ivmaxpercent;
        public static bool keepPokemonsThatCanEvolve = true;
        public static double latitute = 40.764883;
        public static double longitude = -73.972967;
        public static int masterball = 200;
        public static int maxCp = 999;

        public static int navigation_option = 1;
        public static List<PokemonId> noCatch = new List<PokemonId>();
        public static List<PokemonId> noTransfer = new List<PokemonId>();
        public static string password = "empty";
        public static int pokeball = 20;
        public static bool pokeList = true;
        public static int potion;
        public static int radius = 5000;
        public static int revive = 20;
        public static double speed = 50;
        public static int superpotion;
        public static string telAPI = string.Empty;
        public static int telDelay = 5000;
        public static string telName = string.Empty;
        public static int toppotion = 100;
        public static int toprevive = 50;
        public static bool transfer = true;
        public static int ultraball = 100;
        public static bool useincense = true;
        public static bool useluckyegg = true;
        public static string username = "empty";
    }
}