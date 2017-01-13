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

		public static void Write(string message, LogLevel level = LogLevel.Info)
		{
            Console.WriteLine($"[{level}][{DateTime.Now.ToString("HH:mm:ss")}] "+ message);
            AddLog(message);
		}

        public static void ColoredConsoleWrite(ConsoleColor color, string text, LogLevel level = LogLevel.Info)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{level}][{DateTime.Now.ToString("HH:mm:ss")}] "+ text);
            Console.ForegroundColor = originalColor;
            AddLog(text);
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] " + text);
            Console.ForegroundColor = originalColor;
            AddLog(text);
        }

        public static void Error(string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] " + text);
            Console.ForegroundColor = originalColor;
            AddLog(text);
        }

        public static void AddLog(string line)
        { 
            if (!File.Exists(log))
            {
                File.Create(log);
            } 
            try
            {
                // here you know that the file exists
                TextWriter tw = new StreamWriter(log, true); //  we need to add a new line (aka. i am the brain)
                tw.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] " + line); 
                tw.Close();
            } catch (Exception)
            {
                // Probably used by other process error
            }
        }
    }

  

    /*public enum LogLevel
	{
		None = 0,
		Error = 1,
		Warning = 2,
		Info = 3,
		Debug = 4
	}*/
	
}
