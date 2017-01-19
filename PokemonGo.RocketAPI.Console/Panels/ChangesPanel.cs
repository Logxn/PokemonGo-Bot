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
                typeof(GlobalSettings).GetField(globalName).SetValue(null, castedSender.Checked);
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
            GlobalSettings.pauseAtEvolve = checkBox_pauseAtEvolve1_2.Checked;
            GlobalSettings.pauseAtEvolve2 = checkBox_pauseAtEvolve1_2.Checked;
            Logger.ColoredConsoleWrite(tryCatchColor,((CheckBox)sender).Text+ " value changed");
        }

        void NumRazzPercentValueChanged(object sender, EventArgs e)
        {
            if (! enableEvents)
                return;
            GlobalSettings.razzberry_chance = ((double)((NumericUpDown)sender).Value) / 100;
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
                typeof(GlobalSettings).GetField(globalName).SetValue(null,(double) castedSender.Value);
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
                typeof(GlobalSettings).GetField(globalName).SetValue(null,(int) castedSender.Value);
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
            double lat = GlobalSettings.latitute;
            double lng = GlobalSettings.longitude;
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
            if (lat != GlobalSettings.latitute && lng != GlobalSettings.longitude)
            {
                if ((!lat.Equals(GlobalSettings.latitute)) && (!lng.Equals(GlobalSettings.longitude)))
                {
                    GlobalSettings.latitute = lat;
                    GlobalSettings.longitude = lng;
                    var elevationRequest = new ElevationRequest()
                    {
                        Locations = new[] { new Location(lat, lng) },
                    };
                    if (GlobalSettings.GoogleMapsAPIKey != "")
                        elevationRequest.ApiKey = GlobalSettings.GoogleMapsAPIKey;
                    try
                    {
                        ElevationResponse elevation = GoogleMaps.Elevation.Query(elevationRequest);
                        if (elevation.Status == Status.OK)
                        {
                            foreach (Result result in elevation.Results)
                            {
                                GlobalSettings.altitude = result.Elevation;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    GlobalSettings.RelocateDefaultLocation = true;
                    numTravelSpeed.Value = 0;
                    textBoxLatitude.Text = "";
                    textBoxLongitude.Text = "";
                    Logger.ColoredConsoleWrite(ConsoleColor.Green, "Default Location Set will navigate there after next pokestop!");
                }
            }
          
        }

        void buttonUpdateClick(object sender, EventArgs e)
        {
            var ActiveProfile = new Profile();
            if (_clientSettings == null)
                return;
            var configString = JsonConvert.SerializeObject(_clientSettings);
            Profile updatedProfile = new Profile();
            ActiveProfile.ProfileName = GlobalSettings.ProfileName;
            ActiveProfile.IsDefault = GlobalSettings.IsDefault;
            ActiveProfile.RunOrder = GlobalSettings.RunOrder;
            ActiveProfile.SettingsJSON = configString;
            string savedProfiles = File.ReadAllText(@Program.accountProfiles);
            Collection<Profile> _profiles = JsonConvert.DeserializeObject<Collection<Profile>>(savedProfiles);
            Profile profiletoupdate = _profiles.Where(i => i.ProfileName == ActiveProfile.ProfileName).First();
            if (profiletoupdate != null)
            {
                _profiles.Remove(profiletoupdate);
                _profiles.Add(ActiveProfile);
            }
            string ProfilesString = JsonConvert.SerializeObject(_profiles);
            File.WriteAllText(@Program.accountProfiles, ProfilesString);
            MessageBox.Show("Current Configuration Saved as - " + ActiveProfile.ProfileName);
        }

        public static double[] FindLocation(string address)
        {
            double[] ret = { 0.0, 0.0 };
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPoint(address, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null)
            {
                ret = new double[2];
                ret[0] = pos.Value.Lat;
                ret[1] = pos.Value.Lng;
            }
            return ret;
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
            textBoxLatitude.Text = GlobalSettings.latitute.ToString(CultureInfo.InvariantCulture);
            textBoxLongitude.Text = GlobalSettings.longitude.ToString(CultureInfo.InvariantCulture);
        }

        public void Execute(){
            enableEvents = false;
            //Walk Options
            checkBox_RandomlyReduceSpeed.Checked = GlobalSettings.sleepatpokemons;
            checkBox_FarmPokestops.Checked = GlobalSettings.farmPokestops;
            checkBox_CatchPokemon.Checked = GlobalSettings.CatchPokemon;
            checkBox_BreakAtLure.Checked = GlobalSettings.BreakAtLure;
            checkBox_UseLureAtBreak.Checked = GlobalSettings.UseLureAtBreak;
            checkBox_RandomlyReduceSpeed.Checked = GlobalSettings.RandomReduceSpeed;
            checkBox_UseBreakIntervalAndLength.Checked = GlobalSettings.UseBreakFields;
            checkBox_WalkInArchimedeanSpiral.Checked = GlobalSettings.Espiral;
            numericUpDownSpeed.Value = decimal.Parse(GlobalSettings.speed.ToString());
            numericUpDownMinWalkSpeed.Value = decimal.Parse(GlobalSettings.MinWalkSpeed.ToString());
            //Other
            checkBox_useluckyegg.Checked = GlobalSettings.useluckyegg;
            checkBox_UseAnimationTimes.Checked = GlobalSettings.UseAnimationTimes;
            checkBox_evolve.Checked = GlobalSettings.evolve;
            checkBox_pauseAtEvolve1_2.Checked = GlobalSettings.pauseAtEvolve;
            checkBox_UseIncense.Checked = GlobalSettings.useincense;
            checkBox_keepPokemonsThatCanEvolve.Checked = GlobalSettings.keepPokemonsThatCanEvolve;
            checkBoxUseLuckyEggIfNotRunning.Checked = GlobalSettings.useLuckyEggIfNotRunning;
            checkBoxUseRazzBerry.Checked = GlobalSettings.userazzberry;
            numRazzPercent.Value = (int)(GlobalSettings.razzberry_chance * 100);
            checkBoxAutoIncubate.Checked = GlobalSettings.autoIncubate;
            checkBoxUseBasicIncubators.Checked = GlobalSettings.useBasicIncubators;
            //Routing
            checkBox_UseGoogleMapsRouting.Checked = GlobalSettings.UseGoogleMapsAPI;
            text_GoogleMapsAPIKey.Text = GlobalSettings.GoogleMapsAPIKey;
            numTravelSpeed.Value = (int)GlobalSettings.RelocateDefaultLocationTravelSpeed;
            enableEvents = true;
        }
	}
}
