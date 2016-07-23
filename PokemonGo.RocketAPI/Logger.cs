using PokemonGo.RocketAPI.Logging;
using System;

namespace PokemonGo.RocketAPI
{
	/// <summary>
	/// Generic logger which can be used across the projects.
	/// Logger should be set to properly log.
	/// </summary>
	public static class Logger
	{
		private static ILogger logger;

		/// <summary>
		/// Set the logger. All future requests to <see cref="Write(string, LogLevel)"/> will use that logger, any old will be unset.
		/// </summary>
		/// <param name="logger"></param>
		public static void SetLogger(ILogger logger)
		{
			Logger.logger = logger;
		}

		/// <summary>
		/// Log a specific message to the logger setup by <see cref="SetLogger(ILogger)"/> .
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">Optional level to log. Default <see cref="LogLevel.Info"/>.</param>
		public static void Write(string message, LogLevel level = LogLevel.Info)
		{
			if (logger == null)
				return;
			logger.Write(message, level);
		}

        public static void ColoredConsoleWrite(ConsoleColor color, string text, LogLevel level = LogLevel.Info)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] "+ text);
            System.Console.ForegroundColor = originalColor;
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            System.Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] " + text);
            System.Console.ForegroundColor = originalColor;
        }

        public static void Error(string text)
        {
            ConsoleColor originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] " + text);
            System.Console.ForegroundColor = originalColor;
        }


    }

    public enum LogLevel
	{
		None = 0,
		Error = 1,
		Warning = 2,
		Info = 3,
		Debug = 4
	}
}