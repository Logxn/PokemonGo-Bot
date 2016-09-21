/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 21/09/2016
 * Time: 20:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Globalization;
using POGOProtos.Data;
using POGOProtos.Enums;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI.Console.PokeData;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Helpers;
using System;
using System.Threading;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using PokemonGo.RocketAPI.Logic.Utils;
using System.Collections.Generic;
using static PokemonGo.RocketAPI.Console.GUI;
using POGOProtos.Inventory.Item;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Response;
using GMap.NET;
using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
namespace PokemonGo.RocketAPI.Console
{
    /// <summary>
    /// Description of ChangesPanel.
    /// </summary>
    /// NOTES: Use .Tag property of the components to set the name of the related global variable 
    /// NOTES: Use .Tag property of the components to set the name of the related global variable 
    /// NOTES: Use .Tag property of the components to set the name of the related global variable 
    public partial class ChangesPanel : UserControl
    {
        public ChangesPanel()
        {
            InitializeComponent();
        }
        void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            var castedSender = (CheckBox) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("CheckBox_","");
            }
            try {
                typeof(Globals).GetField(globalName).SetValue(null, castedSender.Checked);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue,castedSender.Text+ " value changed");
            } catch (Exception ex) {
                Logger.AddLog("[Exception]: " + ex.ToString());
            }
        }
        void CheckBox_pauseAtEvolve1_2CheckedChanged(object sender, EventArgs e)
        {
            Globals.pauseAtEvolve = checkBox_pauseAtEvolve1_2.Checked;
            Globals.pauseAtEvolve2 = checkBox_pauseAtEvolve1_2.Checked;
            Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue,((CheckBox)sender).Text+ " value changed");
        }
        void NumRazzPercentValueChanged(object sender, EventArgs e)
        {
            Globals.razzberry_chance = ((double)((NumericUpDown)sender).Value) / 100;
            Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue,((NumericUpDown)sender).Text+ " value changed");
        }
        void NumericUpDown_DoubleValueChanged(object sender, EventArgs e)
        {
            var castedSender = (NumericUpDown) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("NumericUpDown_","");
            }
            try {
                typeof(Globals).GetField(globalName).SetValue(null,(double) castedSender.Value);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue,castedSender.Text+ " value changed");                
            } catch (Exception ex) {
                Logger.AddLog("[Exception]: " + ex.ToString());
            }
        }
        void NumericUpDown_IntValueChanged(object sender, EventArgs e)
        {
            var castedSender = (NumericUpDown) sender;
            var globalName = castedSender.Tag.ToString();
            if (globalName == "")
            {
                globalName = castedSender.Name.ToLower().Replace("NumericUpDown_","");
            }
            try {
                typeof(Globals).GetField(globalName).SetValue(null,(int) castedSender.Value);
                Logger.ColoredConsoleWrite(ConsoleColor.DarkBlue,castedSender.Text+ " value changed");
            } catch (Exception ex) {
                Logger.AddLog("[Exception]: " + ex.ToString());
            }
        }
        void ButtonSearchClick(object sender, EventArgs e)
        {
            var ret = FindLocation(textBox1.Text);
            textBoxLatitude.Text = ret[0].ToString();
            textBoxLongitude.Text = ret[1].ToString();
          
        }
        void ButtonSetLocationClick(object sender, EventArgs e)
        {
            double lat = Globals.latitute;
            double lng = Globals.longitude;
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
            if (lat != Globals.latitute && lng != Globals.longitude)
            {
                if ((!lat.Equals(Globals.latitute)) && (!lng.Equals(Globals.longitude)))
                {
                    Globals.latitute = lat;
                    Globals.longitude = lng;
                    var elevationRequest = new ElevationRequest()
                    {
                        Locations = new[] { new Location(lat, lng) },
                    };
                    if (Globals.GoogleMapsAPIKey != "")
                        elevationRequest.ApiKey = Globals.GoogleMapsAPIKey;
                    try
                    {
                        ElevationResponse elevation = GoogleMaps.Elevation.Query(elevationRequest);
                        if (elevation.Status == Status.OK)
                        {
                            foreach (Result result in elevation.Results)
                            {
                                Globals.altitude = result.Elevation;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    Globals.RelocateDefaultLocation = true;
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
            Profile updatedProfile = new Console.Profile();
            ActiveProfile.ProfileName = Globals.ProfileName;
            ActiveProfile.IsDefault = Globals.IsDefault;
            ActiveProfile.RunOrder = Globals.RunOrder;
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
        void Button8Click(object sender, EventArgs e)
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
            textBoxLatitude.Text = Globals.latitute.ToString(CultureInfo.InvariantCulture);
            textBoxLongitude.Text = Globals.longitude.ToString(CultureInfo.InvariantCulture);
        }
        public void Execute(){
            //Walk Options
            checkBox_RandomlyReduceSpeed.Checked = Globals.sleepatpokemons;
            checkBox_FarmPokestops.Checked = Globals.farmPokestops;
            checkBox_CatchPokemon.Checked = Globals.CatchPokemon;
            checkBox_BreakAtLure.Checked = Globals.BreakAtLure;
            checkBox_UseLureAtBreak.Checked = Globals.UseLureAtBreak;
            checkBox_RandomlyReduceSpeed.Checked = Globals.RandomReduceSpeed;
            checkBox_UseBreakIntervalAndLength.Checked = Globals.UseBreakFields;
            checkBox_WalkInArchimedeanSpiral.Checked = Globals.Espiral;
            numericUpDownSpeed.Value = decimal.Parse(Globals.speed.ToString());
            numericUpDownMinWalkSpeed.Value = decimal.Parse(Globals.MinWalkSpeed.ToString());
            //Other
            checkBox_useluckyegg.Checked = Globals.useLuckyEggIfNotRunning;
            checkBox_UseAnimationTimes.Checked = Globals.UseAnimationTimes;
            checkBox_evolve.Checked = Globals.evolve;
            checkBox_pauseAtEvolve1_2.Checked = Globals.pauseAtEvolve;
            checkBox_UseIncense.Checked = Globals.useincense;
            checkBox_keepPokemonsThatCanEvolve.Checked = Globals.keepPokemonsThatCanEvolve;
            checkBoxUseLuckyEggIfNotRunning.Checked = Globals.useluckyegg;
            checkBoxUseRazzBerry.Checked = Globals.userazzberry;
            numRazzPercent.Value = (int)(Globals.razzberry_chance * 100);
            checkBoxAutoIncubate.Checked = Globals.autoIncubate;
            checkBoxUseBasicIncubators.Checked = Globals.useBasicIncubators;
            //Routing
            checkBox_UseGoogleMapsRouting.Checked = Globals.UseGoogleMapsAPI;
            text_GoogleMapsAPIKey.Text = Globals.GoogleMapsAPIKey;
            numTravelSpeed.Value = (int)Globals.RelocateDefaultLocationTravelSpeed;
        }
	}
}
