/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 21/09/2016
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using GMap.NET;
using GMap.NET.MapProviders;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi.Entities.Elevation.Response;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static PokemonGo.RocketAPI.Console.GUI;
using PokemonGo.RocketAPI.Logic.Shared;
namespace PokemonGo.RocketAPI.Console
    
{
    public partial class ChangesPanel : UserControl
    {
        const ConsoleColor tryCatchColor = ConsoleColor.DarkYellow;
        private bool enableEvents = false;
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public delegate void ChangeLanguage();
        public ChangeLanguage OnChangeLanguage;

        public ChangesPanel()
        {
            InitializeComponent();
            comboLanguage.SelectedIndex = 0;
            switch (th.GetSelectedLanguage()) {
                case "default":
                    comboLanguage.SelectedIndex = 1;
                    break;
                case "de":
                    comboLanguage.SelectedIndex = 2;
                    break;
                case "es":
                    comboLanguage.SelectedIndex = 3;
                    break;
                case "ca":
                    comboLanguage.SelectedIndex = 4;
                    break;
                case "zh":
                    comboLanguage.SelectedIndex = 5;
                    break;
            } 
            th.Translate(this);
            comboBoxAttackers.DataSource = new[]{
                th.TS("Random"),
                th.TS("Best CP"),
                th.TS("Favourites"),
                th.TS("Lower than defenders CP")
            };
            comboBoxAttackers.SelectedIndex = 0;
            
        }

        void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var castedSender = (CheckBox) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("CheckBox_","");
            }
            try
            {
                typeof(GlobalVars).GetField(globalName).SetValue(null, castedSender.Checked);
                Logger.ColoredConsoleWrite(tryCatchColor,th.TS("{0} value changed to {1}",castedSender.Tag,castedSender.Checked));
            }
            catch (Exception ex)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: ChangesPanel.cs - CheckBoxes_CheckedChanged()");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
            }
        }

        void CheckBox_pauseAtEvolve1_2CheckedChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var castedSender = sender as CheckBox;
            GlobalVars.pauseAtEvolve = castedSender.Checked;
            GlobalVars.pauseAtEvolve2 = castedSender.Checked;
            Logger.ColoredConsoleWrite(tryCatchColor,th.TS("{0} value changed to {1}",castedSender.Tag,castedSender.Checked));
        }

        void NumRazzPercentValueChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var castedSender = sender as NumericUpDown;

            GlobalVars.razzberry_chance = (double) castedSender.Value / 100;
            Logger.ColoredConsoleWrite(tryCatchColor,th.TS("{0} value changed to {1}",castedSender.Tag,castedSender.Text));
        }

        void NumericUpDown_DoubleValueChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var castedSender = (NumericUpDown) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("NumericUpDown_","");
            }
            try {
                typeof(GlobalVars).GetField(globalName).SetValue(null,(double) castedSender.Value);
                Logger.ColoredConsoleWrite(tryCatchColor,castedSender.Text+ " value changed");
            } catch (Exception ex) {
                Logger.AddLog("[Exception]: " + ex.ToString());
            }
        }

        void NumericUpDown_IntValueChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var castedSender = (NumericUpDown) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("NumericUpDown_","");
            }
            try {
                typeof(GlobalVars).GetField(globalName).SetValue(null,(int) castedSender.Value);
                Logger.ColoredConsoleWrite(tryCatchColor,castedSender.Text+ " value changed");
            } catch (Exception ex) {
                Logger.AddLog("[Exception]: " + ex.ToString());
            }
        }

        /// <summary>
        /// Search for an address in google maps and returns coordenates
        /// </summary>
        void ButtonSearchClick(object sender, EventArgs e)
        {
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPoint(textBoxAddressToSearch.Text, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null)
            {
                textBoxLatitude.Text = pos.Value.Lat.ToString();
                textBoxLongitude.Text = pos.Value.Lng.ToString();
            }
        }

        void ButtonSetLocationClick(object sender, EventArgs e)
        {
            double lat = GlobalVars.latitude;
            double lng = GlobalVars.longitude;
            try
            {
                lat = double.Parse(textBoxLatitude.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (lat > 90.0 || lat < -90.0)
                {
                    throw new System.ArgumentException(th.TS("Value has to be between 90 and -90!"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxLatitude.Text = "";
            }
            try
            {
                lng = double.Parse(textBoxLongitude.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                if (lng > 180.0 || lng < -180.0)
                {
                    throw new System.ArgumentException(th.TS("Value has to be between 180 and -180!"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxLongitude.Text = "";
            }
            if (lat != GlobalVars.latitude && lng != GlobalVars.longitude)
            {
                if ((!lat.Equals(GlobalVars.latitude)) && (!lng.Equals(GlobalVars.longitude)))
                {
                    GlobalVars.latitude = lat;
                    GlobalVars.longitude = lng;
                    var elevationRequest = new ElevationRequest()
                    {
                        Locations = new[] { new Location(lat, lng) },
                    };
                    if (GlobalVars.GoogleMapsAPIKey != "")
                        elevationRequest.ApiKey = GlobalVars.GoogleMapsAPIKey;
                    try
                    {
                        ElevationResponse elevation = GoogleMaps.Elevation.Query(elevationRequest);
                        if (elevation.Status == Status.OK)
                        {
                            foreach (Result result in elevation.Results)
                            {
                                GlobalVars.altitude = result.Elevation;
                            }
                        }
                    }
                    catch (Exception ex1)
                    {
                        Logger.ExceptionInfo(ex1.ToString());
                    }
                    GlobalVars.RelocateDefaultLocation = true;
                    numTravelSpeed.Value = 0;
                    textBoxLatitude.Text = "";
                    textBoxLongitude.Text = "";
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Default Location Set will navigate there after next pokestop!");
                }
            }
            
        }

        void buttonUpdateClick(object sender, EventArgs e)
        {
            var botSettings = GlobalVars.GetSettings();
            var ConfigsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs");
            var filenameProf= Path.Combine(ConfigsPath, GlobalVars.ProfileName +".json" );
            botSettings.SaveToFile(filenameProf);
            MessageBox.Show(th.TS("Current Configuration Saved as - ") + GlobalVars.ProfileName);
        }

        void ButtonReviseClick(object sender, EventArgs e)
        {
            try
            {
                DisplayLocationSelector();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Logger.Write(ex.Message);
            }
        }

        private void DisplayLocationSelector()
        {
            var locationSelector = new LocationSelect(false);
            locationSelector.ShowDialog();
            textBoxLatitude.Text = GlobalVars.latitude.ToString(CultureInfo.InvariantCulture);
            textBoxLongitude.Text = GlobalVars.longitude.ToString(CultureInfo.InvariantCulture);
        }

        public void Execute(){
            enableEvents = false;
            //Walk Options
            checkBox_RandomlyReduceSpeed.Checked = GlobalVars.sleepatpokemons;
            checkBox_FarmPokestops.Checked = GlobalVars.FarmPokestops;
            checkBox_CatchPokemon.Checked = GlobalVars.CatchPokemon;
            checkBox_BreakAtLure.Checked = GlobalVars.BreakAtLure;
            checkBox_UseLureAtBreak.Checked = GlobalVars.UseLureAtBreak;
            checkBox_RandomlyReduceSpeed.Checked = GlobalVars.RandomReduceSpeed;
            checkBox_UseBreakIntervalAndLength.Checked = GlobalVars.UseBreakFields;
            checkBox_WalkInArchimedeanSpiral.Checked = GlobalVars.Espiral;
            numericUpDownSpeed.Value = decimal.Parse(GlobalVars.WalkingSpeedInKilometerPerHour.ToString());
            numericUpDownMinWalkSpeed.Value = decimal.Parse(GlobalVars.MinWalkSpeed.ToString());
            //Other
            checkBox_useluckyegg.Checked = GlobalVars.UseLuckyEgg;
            checkBox_UseAnimationTimes.Checked = GlobalVars.UseAnimationTimes;
            checkBox_evolve.Checked = GlobalVars.EvolvePokemonsIfEnoughCandy;
            checkBox_pauseAtEvolve1_2.Checked = GlobalVars.pauseAtEvolve;
            checkBox_UseIncense.Checked = GlobalVars.UseIncense;
            checkBox_keepPokemonsThatCanEvolve.Checked = GlobalVars.keepPokemonsThatCanEvolve;
            checkBoxUseLuckyEggIfNotRunning.Checked = GlobalVars.UseLuckyEggIfNotRunning;
            checkBoxUseRazzBerry.Checked = GlobalVars.UseRazzBerry;
            numRazzPercent.Value = (int)(GlobalVars.razzberry_chance * 100);
            checkBoxAutoIncubate.Checked = GlobalVars.AutoIncubate;
            checkBoxUseBasicIncubators.Checked = GlobalVars.UseBasicIncubators;
            checkBox_FarmGyms.Checked = GlobalVars.FarmGyms;
            checkBox_AttackGyms.Checked = GlobalVars.AttackGyms;
            checkBox_CollectDailyBonus.Checked = GlobalVars.CollectDailyBonus;
            checkBox_AutoTransferDoublePokemon.Checked = GlobalVars.TransferDoublePokemons;
            checkbox_Verboselogging.Checked = GlobalVars.EnableVerboseLogging;
            //Routing
            checkBox_UseGoogleMapsRouting.CheckedChanged -= checkBox_UseGoogleMapsRouting_CheckedChanged;
            checkBox_UseGoogleMapsRouting.Checked = GlobalVars.UseGoogleMapsAPI;
            checkBox_UseGoogleMapsRouting.CheckedChanged += checkBox_UseGoogleMapsRouting_CheckedChanged;
            
            text_GoogleMapsAPIKey.Text = GlobalVars.GoogleMapsAPIKey;
            numTravelSpeed.Value = (int)GlobalVars.RelocateDefaultLocationTravelSpeed;
            nudNumDefenders.Value = GlobalVars.NumDefenders;
            numericUpDownMaxAttacks.Value = GlobalVars.MaxAttacks;
            comboBoxAttackers.SelectedIndex = GlobalVars.GymAttackers;
            enableEvents = true;


        }
        void checkbox_Verboselogging_CheckedChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            GlobalVars.EnableVerboseLogging = (sender as CheckBox).Checked;
            Logger.SelectedLevel = LogLevel.Error;
            if (GlobalVars.EnableVerboseLogging)
                Logger.SelectedLevel = LogLevel.Debug;
        }
        void checkBox_UseGoogleMapsRouting_CheckedChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            GlobalVars.UseGoogleMapsAPI =checkBox_UseGoogleMapsRouting.Checked;
            if (GlobalVars.UseGoogleMapsAPI)
                GlobalVars.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;
        }
        void comboLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            var lang = "";
            switch (comboLanguage.SelectedIndex) {
                case 0:
                    lang = CultureInfo.CurrentCulture.Name;
                    break;
                case 1:
                    lang = "default";
                    break;
                case 2:
                    lang = "de";
                    break;
                case 3:
                    lang = "es";
                    break;
                case 4:
                    lang = "ca-ES";
                    break;
                case 5:
                    lang = "zh";
                    break;
            }

            if (lang !="")
            {
                Helper.TranslatorHelper.DownloadTranslationFile("PokemonGo.RocketAPI.Console/Lang", Program.path_translation, lang);
                th.SelectLanguage(lang);
                th.Translate(this);
                if (OnChangeLanguage != null)
                    OnChangeLanguage();
            }
        }
        void comboBoxAttackers_SelectedIndexChanged(object sender, EventArgs e)
        {
          if (! enableEvents)
                return;
            GlobalVars.GymAttackers = comboBoxAttackers.SelectedIndex;
        }

    }
}
