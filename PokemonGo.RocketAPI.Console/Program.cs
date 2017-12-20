using PokeMaster.Helper;
using PokeMaster.Logic.Functions;
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

        [STAThread]
        static void Main(string[] args)
        {
            var openGUI = true;
            Version version = new Version();
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
                        Profile selectedProfile = new Profile();
                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Argument: -nogui");
                        if (!arg.Contains(":")) // Load Default Profile
                        {
                            #region Read bot settings
                            if (File.Exists(GlobalVars.FileForProfiles))
                            {
                                var Profiles = Newtonsoft.Json.JsonConvert.DeserializeObject<Collection<Profile>>(File.ReadAllText(GlobalVars.FileForProfiles));
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
                                    selectedProfile.Settings = ProfileSettings.LoadFromFile(Path.Combine(GlobalVars.PathToConfigs, $"{selectedProfile.ProfileName}.json"));
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
                            if (File.Exists(Path.Combine(GlobalVars.PathToConfigs, givenProfile[1] + ".json")))
                            {
                                selectedProfile.ProfileName = givenProfile[1];
                                GlobalVars.ProfileName = selectedProfile.ProfileName;
                                selectedProfile.Settings = ProfileSettings.LoadFromFile(Path.Combine(GlobalVars.PathToConfigs, givenProfile[1] + ".json"));
                                selectedProfile.Settings.SaveToGlobals();
                            }
                            else
                            {
                                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Profile not found! You didn't setup the bot correctly by running it with -nogui.");
                                Environment.Exit(-1);
                            }
                        }

                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "Argument: profile. Using " + GlobalVars.ProfileName + ". Check logs in this profile log folder.");
                        CheckLogDirectories(Path.Combine(GlobalVars.PathToLogs, GlobalVars.ProfileName));

                        if (GlobalVars.UsePwdEncryption) GlobalVars.Password = Encryption.Decrypt(GlobalVars.Password);
                        #endregion
                    }
                    #endregion

                    #region Argument Coordinates
                    if (arg.Contains(","))
                    {
                        if (File.Exists(GlobalVars.FileForCoordinates))
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "Last coords file exists, trying to delete it");
                            File.Delete(GlobalVars.FileForCoordinates);
                        }
                        
                        string[] crdParts = arg.Split(',');
                        GlobalVars.latitude = double.Parse(crdParts[0].Replace(',', '.'), ConfigWindow.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                        GlobalVars.longitude = double.Parse(crdParts[1].Replace(',', '.'), ConfigWindow.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                        Logger.ColoredConsoleWrite(ConsoleColor.Cyan, $"Argument: coordinates. Starting at: {GlobalVars.latitude},{GlobalVars.longitude},{GlobalVars.altitude}");
                        //we assume -nogui
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
            var currentAPIVersion = new CurrentAPIVersion();
            Logger.ColoredConsoleWrite(ConsoleColor.DarkCyan, "_____________________________________________________________");
            Logger.ColoredConsoleWrite(ConsoleColor.White, "Attention: NO BOT IS SAFE - YOUR ACCOUNT MIGHT BE BANNED !!!");
            Logger.ColoredConsoleWrite(ConsoleColor.Gray, "----- You have been warned => Your decision, your risk -----");
            Logger.ColoredConsoleWrite(ConsoleColor.DarkCyan, "_____________________________________________________________");
            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"             Supported API version: {Resources.BotApiSupportedVersion}");
            Logger.ColoredConsoleWrite(ConsoleColor.Gray, $"              Current API version: {currentAPIVersion.GetNianticAPIVersion()}");
            Logger.ColoredConsoleWrite(ConsoleColor.DarkCyan, $"_____________________________________________________v{Resources.BotVersion}");

            Resources.SetApiVars(currentAPIVersion.GetNianticAPIVersion());
            // Check if a new version of BOT is available
            CheckForNewBotVersion();

            if (!GlobalVars.BypassCheckCompatibilityVersion)
            {
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
            CheckLogDirectories(Path.Combine(GlobalVars.PathToLogs, GlobalVars.ProfileName));

            GlobalVars.infoObservable.HandleNewHuntStats += SaveHuntStats;

            Task MainTask;

            MainTask = Task.Run(() =>
            {
               do
               {
                    try
                    {
                        DeviceSetup.SelectDevice(GlobalVars.DeviceTradeName, GlobalVars.DeviceID, GlobalVars.FileForDeviceData);
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

            MainTask.Wait();

            SleepHelper.AllowSleep();

        }

        private static void SaveHuntStats(string newHuntStat)
        {
            File.AppendAllText(GlobalVars.FileForHuntStats, newHuntStat);
        }

        public static void CheckForNewBotVersion()
        {
            try
            {
                var newest = Setout.GetServerVersion();

                if (newest <= Assembly.GetExecutingAssembly().GetName().Version)
                {
                    //ColoredConsoleWrite(ConsoleColor.Yellow, "Awesome! You have already got the newest version! " + Assembly.GetExecutingAssembly().GetName().Version);
                    return;
                }

                Logger.ColoredConsoleWrite(ConsoleColor.Red, $"Bot Version {newest} is available!");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "We recommend to use this new version.");
            }
            catch (Exception)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.White, "Unable to check for updates now...");
            }
        }



        private static void CheckLogDirectories(string logPath)
        {
            Logger.SwitchToProfileLog(logPath); // Here we change from General Log to profile log, stored inside Logs\<profile_name>\

            GlobalVars.PathToLogs = logPath;

            GlobalVars.FileForAppLog = Path.Combine(GlobalVars.PathToLogs, "log.txt");
            GlobalVars.FileForPokemonsCaught = Path.Combine(GlobalVars.PathToLogs, "PokeLog.txt");
            GlobalVars.FileForTransfers = Path.Combine(GlobalVars.PathToLogs, "TransferLog.txt");
            GlobalVars.FileForEvolve = Path.Combine(GlobalVars.PathToLogs, "EvolveLog.txt");
            GlobalVars.FileForEggs = Path.Combine(GlobalVars.PathToLogs, "EggLog.txt");
            GlobalVars.FileForSession = Path.Combine(GlobalVars.PathToConfigs, $"session_{GlobalVars.ProfileName}.json");
            GlobalVars.FileForCoordinates = Path.Combine(GlobalVars.PathToConfigs, $"LastCoords_{GlobalVars.ProfileName}.txt");


            if (!Directory.Exists(GlobalVars.PathToLogs)) Directory.CreateDirectory(GlobalVars.PathToLogs);
            if (!File.Exists(GlobalVars.FileForPokemonsCaught)) File.Create(GlobalVars.FileForPokemonsCaught).Close();
            if (!File.Exists(GlobalVars.FileForTransfers)) File.Create(GlobalVars.FileForTransfers).Close();
            if (!File.Exists(GlobalVars.FileForEvolve)) File.Create(GlobalVars.FileForEvolve).Close();
            if (!File.Exists(GlobalVars.FileForEggs)) File.Create(GlobalVars.FileForEggs).Close();

            Logger.Rename(GlobalVars.PathToLogs); // Rotate logs if checked
        }
    }
}
