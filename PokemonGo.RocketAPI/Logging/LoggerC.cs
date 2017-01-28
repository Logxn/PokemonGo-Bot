using PokemonGo.RocketAPI.Logging;
using System;
using System.IO;
using PokemonGo.RocketAPI;

namespace PokemonGo.RocketAPI.Logging
{
    /// <summary>
    /// Generic logger which can be used across the projects.
    /// Logger should be set to properly log.
    /// </summary>
    
    public static class LoggerC 
    {
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string log = Path.Combine(path, "log.txt");
        public static LogLevel SelectedLevel = LogLevel.Info;

        public static void Write(string message, LogLevel level = LogLevel.Info)
        {
            if (level <= SelectedLevel)
                Console.WriteLine($"[{level}][{DateTime.Now.ToString("HH:mm:ss")}] "+ message);
            if ( (level!=LogLevel.Debug) || (SelectedLevel ==LogLevel.Debug) )
                AddLog(message);
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text, LogLevel level = LogLevel.Info)
        {
            if (level <= SelectedLevel)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine($"[{level}][{DateTime.Now.ToString("HH:mm:ss")}] "+ text);
                Console.ForegroundColor = originalColor;
            }
            if ( (level!=LogLevel.Debug) || (SelectedLevel ==LogLevel.Debug) )
                AddLog(text);
        }

        public static void Info(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Green,text, LogLevel.Info);
        }

        public static void Warning(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Yellow,text, LogLevel.Warning);
        }

        public static void Error(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Red,text, LogLevel.Error);
        }

        public static void Debug(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Blue,text, LogLevel.Debug);
        }

        public static void AddLog(string text, LogLevel level =LogLevel.Info)
        { 
            if (!File.Exists(log))
            {
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.Create(log);
            } 
            try
            {
                // here you know that the file exists
                TextWriter tw = new StreamWriter(log, true); //  we need to add a new line (aka. i am the brain)
                tw.WriteLine($"[{level}][{DateTime.Now.ToString("HH:mm:ss")}] "+ text); 
                tw.Close();
            } catch (Exception)
            {
                // Probably used by other process error
            }
        }
    }
}
