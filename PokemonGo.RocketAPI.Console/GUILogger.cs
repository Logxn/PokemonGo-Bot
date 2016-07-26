using PokemonGo.RocketAPI.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console
{
    public class GUILogger : ILogger
    {
        private LogLevel maxLogLevel;
        private TextBox loggingBox;

        public void setLoggingBox(TextBox boxRef)
        {
            this.loggingBox = boxRef;
        }

        public GUILogger(LogLevel maxLogLevel)
        {
            this.maxLogLevel = maxLogLevel;
        }

        public void Write(string message, LogLevel level, Color color)
        {
            if (level > maxLogLevel)
                return;

            loggingBox.AppendText($"[{ DateTime.Now.ToString("HH:mm:ss")}] { message}" + Environment.NewLine);
            loggingBox.ForeColor = color;
        }
    }
}