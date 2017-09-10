using PokeMaster.Helper;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokemonGo.RocketAPI.HttpClient;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokeMaster
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        public static extern Boolean FreeConsole();

        public static string path_device = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        public static string LastCoordsTXTFileName = Path.Combine(GlobalVars.ConfigsPath, "LastCoords.txt");
        public static string huntstats = Path.Combine(GlobalVars.ConfigsPath, "HuntStats.txt");
        public static string deviceSettings = Path.Combine(path_device, "DeviceInfo.txt");
        public static string deviceData = Path.Combine(path_device, "DeviceData.json");
        public static string cmdCoords = string.Empty;
        public static string accountProfiles = Path.Combine(GlobalVars.ConfigsPath, "Profiles.txt");
        public static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        public static string path_pokedata = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PokeData");

        static Version version;       

        [STAThread]
        static void Main(string[] args)
        {
            var openGUI = true;
            version = new Version();
            // Review & parse command line arguments

            if (args != null && args.Length > 0)
            {
                #region Parse Arguments
                foreach (string arg in args)
                {
                    #region Argument -nogui
                    if (arg.Contains("-nogui"))
                    {
                        openGUI = false;
                        Profile selectedProfile = null;
                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "You added -nogui!");
                        if (!arg.Contains(":")) // Load Default Profile
                        {
                            #region Read bot settings
                            if (File.Exists(Program.accountProfiles))
                            {
                                var Profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<Collection<Profile>>(File.ReadAllText(accountProfiles));
                                foreach (Profile _profile in Profiles)
                                {
                                    if (_profile.IsDefault)
                                    {
                                        selectedProfile = _profile;
                                        break;
                                    }
                                }
                                if (selectedProfile != null)
                                {
                                    GlobalVars.ProfileName = selectedProfile.ProfileName;
                                    selectedProfile.Settings = ProfileSettings.LoadFromFile(Path.Combine(GlobalVars.ConfigsPath, $"{selectedProfile.ProfileName}.json"));
                                    selectedProfile.Settings.SaveToGlobals();
                                }
                                else
                                {
                                    Logger.ColoredConsoleWrite(ConsoleColor.Red, "Default Profile not found! You didn't setup the bot correctly by running it with -nogui.");
                                    Environment.Exit(-1);
                                }
                            }
                            else
                            {
                                Logger.ColoredConsoleWrite(ConsoleColor.Red, "You have not setup the bot yet. Run it without -nogui to Configure.");
                                Environment.Exit(-1);
                            }
                        }
                        else // Load selected profile
                        {
                            var givenProfile = arg.Split(':');
                            if (File.Exists(Path.Combine(GlobalVars.ConfigsPath, givenProfile[1] + ".json")))
                            {
                                selectedProfile.ProfileName = givenProfile[1];
                                GlobalVars.ProfileName = selectedProfile.ProfileName;
                                selectedProfile.Settings = ProfileSettings.LoadFromFile(Path.Combine(GlobalVars.ConfigsPath, givenProfile[1] + ".json"));
                                selectedProfile.Settings.SaveToGlobals();
                            }
                            else
                            {
                                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Profile not found! You didn't setup the bot correctly by running it with -nogui.");
                                Environment.Exit(-1);
                            }
                        }

                        Logger.ColoredConsoleWrite(ConsoleColor.Red, "Using Profile: " + GlobalVars.ProfileName);
                        if (GlobalVars.UsePwdEncryption) GlobalVars.Password = Encryption.Decrypt(GlobalVars.Password);
                        #endregion
                    }
                    #endregion

                    #region Argument Coordinates
                    if (arg.Contains(","))
                    {
                        if (File.Exists(LastCoordsTXTFileName))
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Last coords file exists, trying to delete it");
                            File.Delete(LastCoordsTXTFileName);
                        }
                        
                        string[] crdParts = arg.Split(',');
                        GlobalVars.latitude = double.Parse(crdParts[0].Replace(',', '.'), ConfigWindow.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                        GlobalVars.longitude = double.Parse(crdParts[1].Replace(',', '.'), ConfigWindow.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                        Logger.ColoredConsoleWrite(ConsoleColor.Yellow, $"Found coordinates in command line. Starting at: {GlobalVars.latitude},{GlobalVars.longitude},{GlobalVars.altitude}");
                        //we assume -noguie
                        openGUI = false;
                    }
                    #endregion

                    #region Argument -bypassversioncheck
                    GlobalVars.BypassCheckCompatibilityVersion |= arg.ToLower().Contains("-bypassversioncheck");
                    #endregion

                    #region Argument -bypasskillswitch
                    GlobalVars.BypassKillSwitch |= arg.ToLower().Contains("-bypasskillswitch");
                    #endregion

                    #region Argument -help
                    if (arg.ToLower().Contains("-help"))
                    {
                        //Show Help
                        System.Console.WriteLine($"Pokemon BOT C# v{Resources.BotVersion.ToString()} help" + Environment.NewLine);
                        System.Console.WriteLine("Use:");
                        System.Console.WriteLine("  -nogui<:profile_name> <lat>,<long>         Console mode only, starting on the indicated Profile , Latitude & Longitude ");
                        System.Console.WriteLine("  -bypassversioncheck         Do NOT check BOT & API compatibility (be careful with that option!)");
                        System.Console.WriteLine("  -bypasskillswitch           Do NOT check the forced bot stop (this assures a more than probable ban of your account)");
                        System.Console.WriteLine("  -help                       This help" + Environment.NewLine);
                        Environment.Exit(0);
                    }
                    #endregion
                }
            }
            #endregion

            // Checking if current BOT API implementation supports NIANTIC current API (unless there's an override command line switch)
            if (!GlobalVars.BypassCheckCompatibilityVersion)
            {
                var currentAPIVersion =new CurrentAPIVersion();
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "------------------------------------------------------------");
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "-                                                          -");
                Logger.ColoredConsoleWrite(ConsoleColor.Red,    "!!!! BOTTING IS NOT SAFE - YOUR ACCOUNT WILL BE FLAGGED !!!!");
                Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "----- You have been warned => Your decision, your risk -----");
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "____________________________________________________________");
                Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Bot Current version: {Resources.BotVersion}");
                Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Bot Supported API version: {Resources.BotApiSupportedVersion}");
                Logger.ColoredConsoleWrite(ConsoleColor.DarkMagenta, $"Current API version: {currentAPIVersion.GetNianticAPIVersion()}");

                bool CurrentVersionsOK = currentAPIVersion.CheckAPIVersionCompatibility(GlobalVars.BotApiSupportedVersion);
                if (!CurrentVersionsOK)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Atention, current API version is new and still not supported by Bot.");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Bot will now exit to keep your account safe.");
                    Logger.ColoredConsoleWrite(ConsoleColor.Red, $"---------- PRESS ANY KEY TO CLOSE ----------");

                    Console.ReadKey();
                    Environment.Exit(-1);
                }
            }

            // Check if a new version of BOT is available
            CheckVersion();

             // Check if Bot is deactivated at server level
            if (!GlobalVars.BypassKillSwitch) StringUtils.CheckKillSwitch();

            if (openGUI)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ConfigWindow());
                
                Task.Run(() =>
                {
                    new Panels.SplashScreen().ShowDialog();
                });
                openGUI = GlobalVars.EnablePokeList;
                if (GlobalVars.EnableConsoleInTab) 
                    Logger.type = 1;
                // To open tabbed GUI to test programing 
                /*
                TabbedSystem.skipReadyToUse = true;
                Application.Run( new TabbedSystem()); 
                Environment.Exit(0);
                */
            }
            
            SleepHelper.PreventSleep();

            // Ensure all log paths exists
            CheckLogDirectories(logPath = Path.Combine(logPath, GlobalVars.ProfileName));

            GlobalVars.infoObservable.HandleNewHuntStats += SaveHuntStats;

            Task.Run(() =>
            {
               do
               {
                    try
                    {
                        DeviceSetup.SelectDevice(GlobalVars.DeviceTradeName, GlobalVars.DeviceID, deviceData);
                        new Logic.Logic(new Settings(), GlobalVars.infoObservable).Execute();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error( $"Unhandled exception: {ex}");
                    }
                    Logger.Error("Restarting in 20 Seconds.");
                    Thread.Sleep(20000);
               }while (Logic.Logic.restartLogic);
            });

            if (openGUI)
            {
                if (GlobalVars.simulatedPGO)
                {
                    Application.Run( new GameAspectSimulator());
                }
                else
                {
                    if (GlobalVars.EnableConsoleInTab)
                        FreeConsole();
                     Application.Run( new TabbedSystem());
                }
            }
            else
            {
                Console.WriteLine("Press any key to continue.");
                Console.ReadKey();
            }
            SleepHelper.AllowSleep();
        }

        private static void SaveHuntStats(string newHuntStat)
        {
            File.AppendAllText(huntstats, newHuntStat);
        }

        public static void CheckVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(string.Empty(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})string.Empty\)\]")
                        .Match(DownloadServerVersion());

                if (!match.Success) return;
                var gitVersion =
                    new Version(
                        $"{match.Groups[1]}.{match.Groups[2]}.{match.Groups[3]}.{match.Groups[4]}");
                if (gitVersion <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    //ColoredConsoleWrite(ConsoleColor.Yellow, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Bot Version {gitVersion} is available!");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We recommend to use this new version.");
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
                        "https://raw.githubusercontent.com/Logxn/PokemonGo-Bot/master/ver.md");
        }

        private static void CheckLogDirectories(string logPath)
        {
            Logger.SwitchToProfileLog(logPath); // Here we change from General Log to profile log, stored inside Logs\<profile_name>\

            string pokelog = Path.Combine(logPath, "PokeLog.txt");
            string manualTransferLog = Path.Combine(logPath, "TransferLog.txt");
            string EvolveLog = Path.Combine(logPath, "EvolveLog.txt");

            if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);
            if (!File.Exists(pokelog)) File.Create(pokelog).Close();
            if (!File.Exists(manualTransferLog)) File.Create(manualTransferLog).Close();
            if (!File.Exists(EvolveLog)) File.Create(EvolveLog).Close();

            Logger.Rename(logPath); // Rotate logs if checked
        }
    }
}
