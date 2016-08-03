﻿using System;
using System.Windows.Forms;
using GMap.NET.MapProviders;
using System.Net;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi.Entities.Elevation.Response;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET;
using System.Threading.Tasks;
using System.Device.Location;
using System.Collections.Generic;

namespace PokemonGo.RocketAPI.Console
{
    public partial class LocationSelect : Form
    {
        private GMarkerGoogle _botMarker = new GMarkerGoogle(new PointLatLng(), GMarkerGoogleType.red);
        private GMapRoute _botRoute = new GMapRoute("BotRoute");
        public double alt;
        public bool close = true;

        public LocationSelect(bool asViewOnly)
        {
            InitializeComponent();
            map.Manager.Mode = AccessMode.ServerOnly;

            if (asViewOnly)
                initViewOnly();
        }
        
        private void initViewOnly()
        {
            //first hide all controls
            foreach (Control c in Controls)
                c.Visible = false;
            //show map
            map.Visible = true;
            map.Dock = DockStyle.Fill;
            map.ShowCenter = false;
            GMapOverlay routeOverlay = new GMapOverlay();
            routeOverlay.Routes.Add(_botRoute);
            routeOverlay.Markers.Add(_botMarker);
            map.Overlays.Add(routeOverlay);
            //show geodata controls
            label1.Visible = true;
            label2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            //don't ask at closing
            close = false;
            //add & remove live data handler after form loaded
            Globals.infoObservable.HandleNewGeoLocations += handleLiveGeoLocations;
            this.FormClosing += (object s, FormClosingEventArgs e) =>
            {
                Globals.infoObservable.HandleNewGeoLocations -= handleLiveGeoLocations;
            };
        }

        private void handleLiveGeoLocations(GeoCoordinate coords)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                textBox1.Text = coords.Latitude.ToString();
                textBox2.Text = coords.Longitude.ToString();
                PointLatLng newPosition = new PointLatLng(coords.Latitude, coords.Longitude);
                _botMarker.Position = newPosition;
                _botRoute.Points.Add(newPosition);
                map.Position = newPosition;
            }));
        }

        private void map_Load(object sender, EventArgs e)
        {
            showMap();
        }

        private void showMap()
        {
            try
            {
                map.DragButton = MouseButtons.Left;
                map.MapProvider = GMapProviders.BingMap;
                map.Position = new GMap.NET.PointLatLng(Globals.latitute, Globals.longitude);
                map.MinZoom = 0;
                map.MaxZoom = 20;
                map.Zoom = 16;

                textBox1.Text = Globals.latitute.ToString();
                textBox2.Text = Globals.longitude.ToString();
                textBox3.Text = Globals.altitude.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void map_OnMapDrag()
        {
            Task.Run(() =>
            {
                WebClient request = new WebClient();
                ElevationRequest elevationRequest = new ElevationRequest()
                {
                    Locations = new Location[] { new Location(map.Position.Lat, map.Position.Lng) },
                };
                try
                {
                    ElevationResponse elevation = GoogleMaps.Elevation.Query(elevationRequest);
                    if (elevation.Status == Status.OK)
                    {
                        foreach (Result result in elevation.Results)
                        {
                            SetText(result.Elevation);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            });
            textBox1.Text = map.Position.Lat.ToString();
            textBox2.Text = map.Position.Lng.ToString();
        }

        delegate void SetTextCallback(double cord);

        private void SetText(double cord)
        {
            if (this.textBox3.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { cord });
            }
            else
            {
                this.textBox3.Text = cord.ToString();
                this.alt = cord;
            }
        }

        private void LocationSelect_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (close)
            {
                var result = MessageBox.Show("You didn't set start location! Are you sure you want to exit this window?", "Location selector", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No || result == DialogResult.Abort)
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.latitute = map.Position.Lat;
            Globals.longitude = map.Position.Lng;
            Globals.altitude = alt;
            close = false;
            ActiveForm.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox1.Text != "-")
            {
                try
                {
                    double lat = double.Parse(textBox1.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (lat > 90.0 || lat < -90.0)
                        throw new System.ArgumentException("Value has to be between 180 and -180!");
                    map.Position = new GMap.NET.PointLatLng(lat, map.Position.Lng);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    textBox1.Text = "";
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0 && textBox2.Text != "-")
            {
                try
                {
                    double lng = double.Parse(textBox2.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (lng > 180.0 || lng < -180.0)
                        throw new System.ArgumentException("Value has to be between 90 and -90!");
                    map.Position = new GMap.NET.PointLatLng(map.Position.Lat, lng);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    textBox2.Text = "";
                }
            }
        }

        private void LocationSelect_Load(object sender, EventArgs e)
        {

        }
    }
}
