/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 10/01/2017
 * Time: 23:56
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
/* All function used in this project
        public static void Error(string str){}
        public static void ColoredConsoleWrite(ConsoleColor color, string text, LogLevel level = LogLevel.Info){}
        public static void Write( string text, LogLevel level = LogLevel.Info){}
        public static void AddLog(string str){}*/ 
using System;
using System.IO;
using PokemonGo.RocketAPI.Logging;

namespace PokemonGo.RocketAPI
{
    public enum LogLevel
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Debug = 4
    }
    public static class Logger
    {
        public static int type = 0;
        public static string message;
        public static ConsoleColor color;
        
        public static LogLevel SelectedLevel {
            get {
                if (type == 0)
                    return LoggerC.SelectedLevel;
                if (type == 1)
                    return LoggerPanel.SelectedLevel;
                return LogLevel.None;
            }
            set {
                if (type == 0)
                    LoggerC.SelectedLevel = value;
                if (type == 1)
                    LoggerPanel.SelectedLevel = value; 
            }
        }
            
        public static void ExceptionInfo(string line)
        {
            if (type == 0) {
                LoggerC.ColoredConsoleWrite(ConsoleColor.Red, "Ignore this: sending exception information to log file.");
                LoggerC.AddLog(line);
            }
            if (type == 1) {
                LoggerPanel.ColoredConsoleWrite(ConsoleColor.Red, "Ignore this: sending exception information to log file.");
                LoggerPanel.AddLog(line);
            }
                
        }
        public static void Info(string str)
        {
            if (type == 0)
                LoggerC.Info(str);
            if (type == 1)
                LoggerPanel.Info(str);
        }
        public static void Warning(string str)
        {
            if (type == 0)
                LoggerC.Warning(str);
            if (type == 1)
                LoggerPanel.Warning(str);
        }
        public static void Error(string str)
        {
            if (type == 0)
                LoggerC.Error(str);
            if (type == 1)
                LoggerPanel.Error(str);
        }
        public static void Debug(string str)
        {
            if (type == 0)
                LoggerC.Debug(str);
            if (type == 1)
                LoggerPanel.Debug(str);
        }
        public static void ColoredConsoleWrite(ConsoleColor color, string line, LogLevel level = LogLevel.Info)
        {
            if (type == 0)
                LoggerC.ColoredConsoleWrite(color, line, level);
            if (type == 1)
                LoggerPanel.ColoredConsoleWrite(color, line, level);
        }
        public static void Write(string line, LogLevel level = LogLevel.Info)
        {
            if (type == 0)
                LoggerC.Write(line, level);
            if (type == 1)
                LoggerPanel.Write(line, level);
        }
        
        public static void AddLog(string line)
        {
            if (type == 0)
                LoggerC.AddLog(line);
            if (type == 1)
                LoggerPanel.AddLog(line);
        }

        public static void Rename()
        {
             string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
             string log = Path.Combine(path, "log.txt");
             string newlog = Path.Combine(path, "log0.txt");
             int i = 0;
             while (File.Exists(newlog)) {
                 i++;
                 newlog = Path.Combine(path, $"log{i}.txt");
             }
             File.Move(log, newlog);
        }
    }
}
