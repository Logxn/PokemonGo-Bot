using PokemonGo.RocketAPI;
using System.Collections.Generic;
using System.Linq;

namespace PokeMaster.Logic.Shared
{
    public class BreakSettings
    {
        private static int breaksCount = 0;
        private static int invokeCount = 0;
        private static double MaxSpeedDefault = GlobalVars.WalkingSpeedInKilometerPerHour;
        private static double MinSpeedDefault = GlobalVars.MinWalkSpeed;

        private static List<BreakSettings> OrderedBreaks = new List<BreakSettings>();
        private System.Timers.Timer walkTimer = new System.Timers.Timer();
        private System.Timers.Timer idleTimer = new System.Timers.Timer();

        public int BreakSequenceId { get; set; }
        public int BreakWalkTime { get; set; }
        public int BreakIdleTime { get; set; }
        public bool BreakEnabled { get; set; }
        public bool BreakSettingsCatchPokemon { get; set; }
        public double BreakSettingsMaxSpeed { get; set; }
        public double BreakSettingsMinSpeed { get; set; }

        private void SetWalkTimer(BreakSettings _break)
        {
            walkTimer.Interval = _break.BreakWalkTime * 1000 * 60;
            walkTimer.AutoReset = false;
            walkTimer.Start();

            Logger.Debug("[" + invokeCount + "] Sequence " + _break.BreakSequenceId + " started." 
                + " GlobalVars Catch: " + (GlobalVars.CatchPokemon ? "Yes" : "No")
                + " GlobalVars Walk: " + GlobalVars.WalkingSpeedInKilometerPerHour + "/" + GlobalVars.MinWalkSpeed
                + " Catch: " + (_break.BreakSettingsCatchPokemon ? "Yes" : "No")
                + " Walk: "  + _break.BreakSettingsMaxSpeed + "/" + _break.BreakSettingsMinSpeed
                + " MaxSpeedDefault: " + MaxSpeedDefault + " MinSpeedDefault: " + MinSpeedDefault);

            if (_break.BreakSettingsCatchPokemon) GlobalVars.CatchPokemon = true;
            else GlobalVars.CatchPokemon = false;

            if (_break.BreakSettingsMaxSpeed > 0 && _break.BreakSettingsMinSpeed < _break.BreakSettingsMaxSpeed)
            {
                GlobalVars.WalkingSpeedInKilometerPerHour = _break.BreakSettingsMaxSpeed;
                GlobalVars.MinWalkSpeed = _break.BreakSettingsMinSpeed;
            }
            else
            {
                GlobalVars.WalkingSpeedInKilometerPerHour = MaxSpeedDefault;
                GlobalVars.MinWalkSpeed = MinSpeedDefault;
            }

            Logger.Info("[" + invokeCount + "] Walking Timer for sequence " + _break.BreakSequenceId + " started. We are going to walk during: " + _break.BreakWalkTime + " minutes." 
                + " Catch: " + (_break.BreakSettingsCatchPokemon ? "Yes" : "No") 
                + " Walk: " + GlobalVars.WalkingSpeedInKilometerPerHour +"(" + _break.BreakSettingsMaxSpeed + "/" + GlobalVars.MinWalkSpeed + "(" +_break.BreakSettingsMinSpeed +")");
        }
        
        private void SetIdleTimer(BreakSettings _break)
        {
            idleTimer.Interval = _break.BreakIdleTime * 1000 * 60;
            idleTimer.AutoReset = false;
            idleTimer.Start();
            Logger.Warning("[" + invokeCount + "] Idle Timer for sequence " + _break.BreakSequenceId + " started. We are going to idle during: " + _break.BreakIdleTime + " minutes as soon as we finish what we are doing.");
        }

        private void IdleTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            idleTimer.Stop();
            NextInterval();
            // Ojo con el useprevioussettings !! -> inicia desde el principio
            GlobalVars.ForceAdvancedBreakNow = false;
        }

        private void WalkTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
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
        /// <summary>
        /// Checks if AdvancedBreaks is enabled and loads first break
        /// </summary>
        /// <param name="BotSettings"></param>
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
                MaxSpeedDefault = GlobalVars.WalkingSpeedInKilometerPerHour;
                MinSpeedDefault = GlobalVars.MinWalkSpeed;
            }
        }
    }


}
