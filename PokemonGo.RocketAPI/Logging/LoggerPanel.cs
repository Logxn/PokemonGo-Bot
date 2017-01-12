﻿/*
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


namespace PokemonGo.RocketAPI.Logging
{
    /// <summary>
    /// Description of LoggerPanel.
    /// </summary>
    public partial class LoggerPanel : UserControl
    {
        public class Message{
            public Message(ConsoleColor c, string t){
                color = c;
                text =t;
            }
            public string text;
            public ConsoleColor color;
        }
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
        public static string log = Path.Combine(path, "log.txt");
        public static Queue messages = new Queue();
        
        public LoggerPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            
            Logger.setPanel(this);
        }
        delegate void addNewLineCallback(ConsoleColor color,string text);
        public void addNewLine(ConsoleColor color, string text){
            if (this.rtbLog.InvokeRequired){
                try{
                    this.Invoke(new Action<ConsoleColor, string>(addNewLine), new object[] {color, text} );
                }catch (Exception ex1)
                {
                    AddLog(ex1.ToString());
                }
            }else{
                rtbLog.SelectionStart = rtbLog.TextLength;
                rtbLog.SelectionColor = ConsoleColorToColor(color);
                rtbLog.AppendText(text+"\n");
                if (rtbLog.Focused){
                    rtbLog.SelectionStart = rtbLog.TextLength+1;
                    rtbLog.ScrollToCaret();
                }
            }
        }
        
        Color ConsoleColorToColor( ConsoleColor ccolor)
        {
            switch (ccolor){
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
        public void Error(string line){
            messages.Enqueue(new Message(ConsoleColor.Red,line));
            AddLog(line);
        }
        public void ColoredConsoleWrite(ConsoleColor color, string line, LogLevel level = LogLevel.Info){
            messages.Enqueue(new Message(color,line));
            AddLog(line);
        }
        public void Write( string line, LogLevel level = LogLevel.Info){
            messages.Enqueue(new Message(ConsoleColor.White,line));
            AddLog(line);
        }
        
        public void AddLog(string line){
            if (!File.Exists(log))
            {
                File.Create(log);
            } 
            try
            {
                TextWriter tw = new StreamWriter(log, true); //  we need to add a new line (aka. i am the brain)
                var strtime =DateTime.Now.ToString("HH:mm:ss");
                tw.WriteLine($"[{strtime}] {line}");
                tw.Close();
            } catch (Exception)
            {
            }
        }
        void timer1_Tick(object sender, EventArgs e)
        {
            try {
                while (messages.Count > 0){
                 var msg = (Message) messages.Dequeue();
                 addNewLine(msg.color,msg.text);
                }
                
            } catch (Exception ex1) {
                AddLog(ex1.ToString());                
            }
                
        }
        
    }
}
