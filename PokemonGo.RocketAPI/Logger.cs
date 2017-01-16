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
using PokemonGo.RocketAPI.Logging;

namespace PokemonGo.RocketAPI
{
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4
    }
    public class Logger
    {
        public static Logger logger;
        public static int type = 0;
        public static string message;
        public static ConsoleColor color;
        public Logger()
        {
            
        }
        private static LoggerPanel panel = null;
        public static void setPanel(LoggerPanel panel1){
            panel = panel1;
            //type = 1;
        }
        public static void ExceptionInfo(string line){
            if (type == 0)
            {
                LoggerC.ColoredConsoleWrite(ConsoleColor.Red, "Ignore this: sending exception information to log file.");
                LoggerC.AddLog(line);
            }
            else
                try {
                    if (panel != null){
                            panel.ColoredConsoleWrite(ConsoleColor.Red, "Ignore this: sending exception information to log file.");
                            panel.AddLog(line);
                    }
                    
                } catch (Exception ex1) {
                    AddLog(line+"\n"+ex1.ToString());
                }
        }
        public static void Error(string str){
            if (type == 0)
                LoggerC.Error(str);
            else
                try {
                    if (panel != null)
                        panel.Error(str);
                    
                } catch (Exception ex1) {
                    AddLog(str+ex1.ToString());
                }
        }
        public static void ColoredConsoleWrite(ConsoleColor color, string line, LogLevel level = LogLevel.Info){
            if (type == 0)
                LoggerC.ColoredConsoleWrite(color,line,level);
            else
                try {
                if (panel != null)
                    panel.ColoredConsoleWrite(color,line,level);
                } catch (Exception ex1) {
                    AddLog(line+ex1.ToString());
                }
        }
        public static void Write( string line, LogLevel level = LogLevel.Info){
            if (type == 0)
                LoggerC.Write(line,level);
            else
                try {
                if (panel != null)
                    panel.Write(line,level);
                } catch (Exception ex1) {
                    AddLog(line+ ex1.ToString());
                }
        }
        
        public static void AddLog(string line){
            if (type == 0)
                LoggerC.AddLog(line);
            else
                if (panel != null)
                    panel.AddLog(line);
        }
    }
}
