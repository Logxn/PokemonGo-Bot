using System;
using System.Collections.Generic;
using System.Linq;
using POGOLib.Official.Logging;

namespace PokeMaster.Logic.Shared
{
    public class BreakSettings
    {
        private static int breaksCount = 0;
        private static int invokeCount = 0;
        private static List<BreakSettings> OrderedBreaks = new List<BreakSettings>();
        private System.Timers.Timer walkTimer = new System.Timers.Timer();
        private System.Timers.Timer idleTimer = new System.Timers.Timer();

        public int BreakSequenceId { get; set; }
        public int BreakWalkTime { get; set; }
        public int BreakIdleTime { get; set; }
        public bool BreakEnabled { get; set; }

        private void SetWalkTimer(BreakSettings _break)
        {
            //var autoEvent = new AutoResetEvent(false);
            //var stateTimer = new Timer(NextInterval, null, 0, _break.BreakWalkTime);
            walkTimer.Interval = _break.BreakWalkTime * 1000 * 60;
            walkTimer.AutoReset = false;
            walkTimer.Start();
            Logger.Info("[" + invokeCount + "] Walking Timer for sequence " + _break.BreakSequenceId + " started. We are going to walk during: " + _break.BreakWalkTime + " minutes.");
        }
        
        private void SetIdleTimer(BreakSettings _break)
        {
            idleTimer.Interval = _break.BreakIdleTime * 1000 * 60;
            idleTimer.AutoReset = false;
            idleTimer.Start();
            Logger.Warn("[" + invokeCount + "] Idle Timer for sequence " + _break.BreakSequenceId + " started. We are going to idle during: " + _break.BreakIdleTime + " minutes as soon as we finish what we are doing.");
        }

        private void IdleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("[" + invokeCount + "] ===== MUST START WALKING NOW =====");
            idleTimer.Stop();
            NextInterval();
            // Ojo con el useprevioussettings !! -> inicia desde el principio
            GlobalVars.ForceAdvancedBreakNow = false;
        }

        private void WalkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("[" + invokeCount + "] ===== MUST PAUSE WALKING NOW =====");
            walkTimer.Stop();
            SetIdleTimer(OrderedBreaks[invokeCount % OrderedBreaks.Count()]);
            GlobalVars.ForceAdvancedBreakNow = true;
        }

        void NextInterval ()
        {
            invokeCount++;
            SetWalkTimer(OrderedBreaks[invokeCount % OrderedBreaks.Count()]);
        }

        public BreakSettings GetCurrentBreak()
        {
            return OrderedBreaks[invokeCount % OrderedBreaks.Count()];
        }

        public void CheckEnabled(ISettings BotSettings)
        {
            if (BotSettings.AdvancedBreaks && breaksCount == 0)
            {
                OrderedBreaks = BotSettings.Breaks.OrderBy(x => x.BreakSequenceId).ToList();
                OrderedBreaks = OrderedBreaks.Where(x => x.BreakEnabled == true).ToList();
                breaksCount = OrderedBreaks.Count();
                walkTimer.Elapsed += WalkTimer_Elapsed;
                idleTimer.Elapsed += IdleTimer_Elapsed;
                SetWalkTimer(OrderedBreaks[invokeCount]);
            }
        }
    }


}
