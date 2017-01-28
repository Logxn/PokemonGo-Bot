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

        public ChangesPanel()
        {
            InitializeComponent();
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
                Logger.ColoredConsoleWrite(tryCatchColor,castedSender.Text+ " value changed");
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
            GlobalVars.pauseAtEvolve = checkBox_pauseAtEvolve1_2.Checked;
            GlobalVars.pauseAtEvolve2 = checkBox_pauseAtEvolve1_2.Checked;
            Logger.ColoredConsoleWrite(tryCatchColor,((CheckBox)sender).Text+ " value changed");
        }

        void NumRazzPercentValueChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            GlobalVars.razzberry_chance = ((double)((NumericUpDown)sender).Value) / 100;
            Logger.ColoredConsoleWrite(tryCatchColor,((NumericUpDown)sender).Text+ " value changed");
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
                    throw new System.ArgumentException("Value has to be between 90 and -90!");
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
                    throw new System.ArgumentException("Value has to be between 180 and -180!");
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
                    catch (Exception)
                    {
                        // ignored
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
            MessageBox.Show("Current Configuration Saved as - " + GlobalVars.ProfileName);
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
            LocationSelect locationSelector = new LocationSelect(false);
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
            checkBox_CollectDailyBonus.Checked = GlobalVars.CollectDailyBonus;
            checkBox_AutoTransferDoublePokemon.Checked = GlobalVars.TransferDoublePokemons;
            checkbox_Verboselogging.Checked = GlobalVars.EnableVerboseLogging;
            //Routing
            checkBox_UseGoogleMapsRouting.Checked = GlobalVars.UseGoogleMapsAPI;
            text_GoogleMapsAPIKey.Text = GlobalVars.GoogleMapsAPIKey;
            numTravelSpeed.Value = (int)GlobalVars.RelocateDefaultLocationTravelSpeed;
            enableEvents = true;
        }
        void checkbox_Verboselogging_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVars.EnableVerboseLogging = (sender as CheckBox).Checked;
            Logger.SelectedLevel = LogLevel.Error;
            if (GlobalVars.EnableVerboseLogging)
                Logger.SelectedLevel = LogLevel.Debug;
        }
        void checkBox_UseGoogleMapsRouting_CheckedChanged(object sender, EventArgs e)
        {
            GlobalVars.UseGoogleMapsAPI =checkBox_UseGoogleMapsRouting.Checked;
            if (GlobalVars.UseGoogleMapsAPI)
                GlobalVars.GoogleMapsAPIKey = text_GoogleMapsAPIKey.Text;
        }
	}
}
