/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 11/01/2017
 * Time: 0:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using PokeMaster.Logic.Shared;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Logging;


namespace PokeMaster.Logic.Utils
{
    /// <summary>
    /// Description of LoggerPanel.
    /// </summary>
    public partial class Logger : UserControl, ILogger
    {
        public enum LogLevel
        {
            None = 0,
            Info = 1,
            Warning = 2,
            Error = 3,
            Debug = 4
        }
        public static LogLevel SelectedLevel = LogLevel.Info;
        public class Message
        {
            public Message(ConsoleColor c, string t)
            {
                color = c;
                text = t;
            }
            public string text;
            public ConsoleColor color;
        }
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string log = Path.Combine(path, "log.txt");
        public static Queue messages = new Queue();
        
        public Logger()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            APIConfiguration.Logger = this;
            
        }
        delegate void addNewLineCallback(ConsoleColor color, string text);
        public void addNewLine(ConsoleColor color, string text)
        {
            if (this.rtbLog.InvokeRequired) {
                try {
                    this.Invoke(new Action<ConsoleColor, string>(addNewLine), new object[] {
                                    color,
                                    text
                                });
                } catch (Exception ex1) {
                    AddLog(ex1.ToString());
                }
            } else {
                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.SelectionColor = ConsoleColorToColor(color);
                rtbLog.AppendText(text + "\n");
                if (rtbLog.Focused) {
                    rtbLog.SelectionStart = rtbLog.TextLength + 1;
                    rtbLog.ScrollToCaret();
                }
            }
        }
        
        Color ConsoleColorToColor(ConsoleColor ccolor)
        {
            switch (ccolor) {
                case ConsoleColor.Black:
                    return Color.Black;
                case ConsoleColor.Blue:
                    return Color.Blue;
                case ConsoleColor.Cyan:
                    return Color.Cyan;
                case ConsoleColor.DarkBlue:
                    return ColorTranslator.FromHtml("#000080");
                case ConsoleColor.DarkGray:
                    return ColorTranslator.FromHtml("#808080");
                case ConsoleColor.DarkGreen:
                    return ColorTranslator.FromHtml("#008000");
                case ConsoleColor.DarkMagenta:
                    return ColorTranslator.FromHtml("#800080");
                case ConsoleColor.DarkRed:
                    return ColorTranslator.FromHtml("#800000");
                case ConsoleColor.DarkYellow:
                    return ColorTranslator.FromHtml("#808000");
                case ConsoleColor.Gray:
                    return ColorTranslator.FromHtml("#C0C0C0");
                case ConsoleColor.Green:
                    return ColorTranslator.FromHtml("#00FF00");
                case ConsoleColor.Magenta:
                    return Color.Magenta;
                case ConsoleColor.Red:
                    return Color.Red;
                case ConsoleColor.White:
                    return Color.White;
            }
            return Color.Yellow;
        }
        public static void Info(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Green, text, LogLevel.Info);
        }

        public static void Warning(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Yellow, text, LogLevel.Warning);
        }

        public static void Error(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Red, text, LogLevel.Error);
        }

        public static void Debug(string text)
        {
            ColoredConsoleWrite(ConsoleColor.Blue, text, LogLevel.Debug);
        }

        public static void ColoredConsoleWrite(ConsoleColor color, string line, LogLevel level = LogLevel.Info)
        {
            var strtime = DateTime.Now.ToString("HH:mm:ss");
            var lineWithTime = $"[{strtime}] {line}";
            if (level <= SelectedLevel){
                if (GlobalVars.EnableConsoleInTab)
                    messages.Enqueue(new Message(color, lineWithTime));
                else{
                    ConsoleColor originalColor = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.WriteLine($"[{level}] {lineWithTime}");
                    Console.ForegroundColor = originalColor;
                }
            }
            if ((level != LogLevel.Debug) || (SelectedLevel == LogLevel.Debug))
                AddLog(line);
        }
        public static void Write(string line, LogLevel level = LogLevel.Info)
        {
            var strtime = DateTime.Now.ToString("HH:mm:ss");
            var lineWithTime = $"[{strtime}] {line}";
            if (level <= SelectedLevel)
                messages.Enqueue(new Message(ConsoleColor.White, lineWithTime));
            if ((level != LogLevel.Debug) || (SelectedLevel == LogLevel.Debug))
                AddLog(line);
        }

        public static void AddLog(string line)
        {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(log)) {
                File.Create(log);
            }
            try {
                TextWriter tw = new StreamWriter(log, true); //  we need to add a new line (aka. i am the brain)
                var strtime = DateTime.Now.ToString("HH:mm:ss");
                tw.WriteLine($"[{strtime}] {line}");
                tw.Close();
            } catch (Exception) {
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            try {
                while (messages.Count > 0) {
                    var msg = (messages.Dequeue() as Message);
                    addNewLine(msg.color, msg.text);
                }
            } catch (Exception ex1) {
                AddLog(ex1.ToString());
            }
        }

        public static void ExceptionInfo(string line)
        {
            ColoredConsoleWrite(ConsoleColor.Red, "Ignore this: sending exception information to log file.");
            AddLog(line);
        }

        public void HashStatusUpdate(HashInfo info)
        {
            Debug($"(HASH SERVER)  [{info.MaskedAPIKey}] in last 1 minute  {info.Last60MinAPICalles} request/min , AVG: {info.Last60MinAPIAvgTime:0.00} ms/request , Fastest : {info.Fastest}, Slowest: {info.Slowest}");
        }

        public void LogCritical(string message, dynamic data)
        {
            Error(message);
        }

        public void LogDebug(string message)
        {
            Debug(message);
        }

        public void LogError(string message)
        {
            Error(message);
        }

        public void LogInfo(string message)
        {
            Info(message);
        }
        
    }
}
