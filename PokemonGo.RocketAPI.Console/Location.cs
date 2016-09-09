using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
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
using System.Globalization;
using System.Collections.Generic;
using POGOProtos.Enums;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketApi.PokeMap;
using System.Linq;

namespace PokemonGo.RocketAPI.Console
{
    public partial class LocationSelect : Form
    {

        //private GMarkerGoogle _botMarker = new GMarkerGoogle(new PointLatLng(), GMarkerGoogleType.red_small);
        private GMarkerGoogle _botMarker = new GMarkerGoogle(new PointLatLng(),  Properties.Resources.player);

        private GMapRoute _botRoute = new GMapRoute("BotRoute");
        private GMapOverlay _pokeStopsOverlay = new GMapOverlay("PokeStops");
        private GMapOverlay _pokemonOverlay = new GMapOverlay("Pokemon");
        private Dictionary<string, GMarkerGoogle> _pokeStopsMarks = new Dictionary<string, GMarkerGoogle>();
        private Dictionary<string, GMarkerGoogle> _pokemonMarks = new Dictionary<string, GMarkerGoogle>();
        public double alt;
        public bool close = true;

        public LocationSelect(bool asViewOnly)
        {
            InitializeComponent();
            map.Manager.Mode = AccessMode.ServerOnly;

            buttonRefreshPokemon.Visible = false;
            buttonRefreshPokemon.Enabled = false;

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
            GMarkerGoogle _botStartMarker = new GMarkerGoogle(new PointLatLng(), Properties.Resources.start_point);
            _botStartMarker.Position = new PointLatLng(Globals.latitute, Globals.longitude);
            routeOverlay.Markers.Add(_botStartMarker);
            GMapPolygon circle = CreateCircle(new PointLatLng(Globals.latitute, Globals.longitude), Globals.radius, 100);
            routeOverlay.Polygons.Add(circle);
            
            map.Overlays.Add(routeOverlay);
            map.Overlays.Add(_pokeStopsOverlay);
            map.Overlays.Add(_pokemonOverlay);
            //show geodata controls
            label1.Visible = true;
            label2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            buttonRefreshPokemon.Visible = true;
            buttonRefreshPokemon.Enabled = true;
            cbShowPokeStops.Visible = true;           
            cbShowPokemon.Visible = true;
            _pokemonOverlay.IsVisibile = true;
            if(cbShowPokemon.Checked)
            {
                BackgroundWorker bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
bw.WorkerReportsProgress = true;
                bw.DoWork += 
    new DoWorkEventHandler(bw_DoWork);
bw.RunWorkerCompleted += 
    new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);


                buttonRefreshPokemon.Enabled = false;
                bw.RunWorkerAsync();
                Logic.Logic._instance.CheckAvailablePokemons(Logic.Logic._client);

                


            }
            
            //don't ask at closing
            close = false;
            //add & remove live data handler after form loaded
            Globals.infoObservable.HandleNewGeoLocations += handleLiveGeoLocations;
            Globals.infoObservable.HandleAvailablePokeStop += InfoObservable_HandlePokeStop;
            Globals.infoObservable.HandlePokeStopInfoUpdate += InfoObservable_HandlePokeStopInfoUpdate;
            Globals.infoObservable.HandleClearPokemon += infoObservable_HandleClearPokemon;
            Globals.infoObservable.HandleNewPokemonLocations += infoObservable_HandleNewPokemonLocations;
            this.FormClosing += (object s, FormClosingEventArgs e) =>
            {                
                Globals.infoObservable.HandleNewGeoLocations -= handleLiveGeoLocations;
            };
        }
 
        /// <summary>
        /// Handles the RunWorkerCompleted event of the bw control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs" /> instance containing the event data.</param>
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonRefreshPokemon.Enabled = true;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
{
            Thread.Sleep(DataCollector.MaxRetryCount * DataCollector.TimeoutBetweenRetries + 5000);
}

        void infoObservable_HandleClearPokemon()
        {
            _pokemonMarks.Clear();
            _pokemonOverlay.Markers.Clear();
        }

        void infoObservable_HandleNewPokemonLocations(List<DataCollector.PokemonMapData> mapData)
        {
            _pokemonMarks.Clear();
            _pokemonOverlay.Markers.Clear();   

            foreach(var pokeData in mapData.Where(x => x.Id != null))
            {
                GMarkerGoogle pokemonMarker;
                Bitmap pokebitMap = Pokemons.GetPokemonMediumImage(pokeData.PokemonId);
                if(pokebitMap != null)
                {
                    var ImageSize = new System.Drawing.Size(pokebitMap.Width, pokebitMap.Height);
                    pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), pokebitMap){Offset = new System.Drawing.Point(-ImageSize.Width / 2, -ImageSize.Height / 2)};
                }
                else
                {
                    pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), GMarkerGoogleType.green_small);
                }
                 
                if (pokeData.Type == DataCollector.PokemonMapDataType.Nearby)
                {
                    pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), GMarkerGoogleType.black_small);
                }
                pokemonMarker.ToolTipText = StringUtils.getPokemonNameByLanguage(null, (PokemonId)pokeData.PokemonId) + ", " + pokeData.ExpiresAt.ToString()  +", "+  pokeData.Coordinates.Latitude.Value.ToString() + ", " + pokeData.Coordinates.Longitude.Value.ToString();
                pokemonMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                pokemonMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                _pokemonMarks.Add(pokeData.Id, pokemonMarker);
                _pokemonOverlay.Markers.Add(pokemonMarker);
                
            }

            _pokemonOverlay.IsVisibile = cbShowPokemon.Checked;
        }

        private void InfoObservable_HandlePokeStopInfoUpdate(string pokeStopId, string info)
        {
            if (_pokeStopsMarks.ContainsKey(pokeStopId)) {
                //changeType               
                
                var newMark = new GMarkerGoogle(_pokeStopsMarks[pokeStopId].Position, Properties.Resources.visited_pokestop);
                
                newMark.ToolTipText = info;
                newMark.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                
                try
                {
                    _pokeStopsOverlay.Markers[_pokeStopsOverlay.Markers.IndexOf(_pokeStopsMarks[pokeStopId])] = newMark;
                }
                catch(Exception)
                {
                    //Logger.ColoredConsoleWrite(ConsoleColor.Red, "[Debug] - Supressed error msg (Location.cs - Line 86 - Index is -1");
                    // Doing this so the bot wont crash and or restart! - Logxn
                }
                _pokeStopsMarks[pokeStopId] = newMark;
            }
        }

        private void InfoObservable_HandlePokeStop(POGOProtos.Map.Fort.FortData[] pokeStops)
        {
            _pokeStopsOverlay.Markers.Clear();
            _pokeStopsMarks.Clear();

            foreach (var pokeStop in pokeStops.Where(x => x.Id != null)) {
                GMarkerGoogle pokeStopMaker = new GMarkerGoogle(new PointLatLng(pokeStop.Latitude, pokeStop.Longitude), Properties.Resources.pokestop);
                if (pokeStop.ActiveFortModifier.Count > 0)
                {
                    pokeStopMaker = new GMarkerGoogle(new PointLatLng(pokeStop.Latitude, pokeStop.Longitude), Properties.Resources.lured_pokestop);
                }
                pokeStopMaker.ToolTipText = pokeStop.Latitude + ", " + pokeStop.Longitude;
                pokeStopMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                pokeStopMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                _pokeStopsMarks.Add(pokeStop.Id, pokeStopMaker);
                _pokeStopsOverlay.Markers.Add(pokeStopMaker);
            }
        }

        private GMapPolygon CreateCircle(PointLatLng point, double radius, int segments)
        {
            radius /= 100000;
            List<PointLatLng> gpollist = new List<PointLatLng>();
            double seg = Math.PI * 2 / segments;
            for (int i = 0; i < segments; i++)
            {
                double theta = seg * i;
                double a = point.Lat + Math.Cos(theta) * radius * 0.75;
                double b = point.Lng + Math.Sin(theta) * radius;
                gpollist.Add(new PointLatLng(a, b));
            }
            GMapPolygon circle = new GMapPolygon(gpollist, "BotZone");
            circle.Stroke = System.Drawing.Pens.Black;
            circle.Fill = System.Drawing.Brushes.Transparent;
            return circle;
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
            Globals.MapLoaded = true;
            showMap();
        }

        private void showMap()
        {
            try
            {
                map.DragButton = MouseButtons.Left;
                map.MapProvider = GMapProviders.GoogleMap;
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
                var elevationRequest = new ElevationRequest()
                {
                    Locations = new[] { new Location(map.Position.Lat, map.Position.Lng) },
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
                catch (Exception)
                {
                    // ignored
                }
            });

            textBox1.Text = map.Position.Lat.ToString(CultureInfo.InvariantCulture);
            textBox2.Text = map.Position.Lng.ToString(CultureInfo.InvariantCulture);
        }

        delegate void SetTextCallback(double cord);

        private void SetText(double cord)
        {
            if (this.textBox3.InvokeRequired)
            {
                SetTextCallback d = SetText;
                this.Invoke(d, cord);
            }
            else
            {
                this.textBox3.Text = cord.ToString(CultureInfo.InvariantCulture);
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

        private void cbShowPokeStops_CheckedChanged(object sender, EventArgs e)
        {
            _pokeStopsOverlay.IsVisibile = cbShowPokeStops.Checked;
            map.Update();
        }

        private void map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (Globals.pauseAtPokeStop)
            {                
                Globals.RouteToRepeat.Enqueue(new GeoCoordinate(item.Position.Lat, item.Position.Lng));
                item.ToolTipText = "Stop " + Globals.RouteToRepeat.Count + " Queued";
            }  
            else
            {
                MessageBox.Show("Please Pause Walking from Pokemon GUI before defining Route!");
            }
        }

        private async void buttonRefreshPokemon_Click_1(object sender, EventArgs e)
        {
            buttonRefreshPokemon.Enabled = false;
            if (await Logic.Logic._instance.CheckAvailablePokemons(Logic.Logic._client))
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Could not get PokemonData. Service is overloaded.", LogLevel.Error);
            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Could not get PokemonData. Service is overloaded.", LogLevel.Warning);
            }
            buttonRefreshPokemon.Enabled = true;
        }
        
                private void LocationSelect_Load(object sender, EventArgs e)
        {

        }

        private void cbShowPokemon_CheckedChanged(object sender, EventArgs e)
        {
            _pokemonOverlay.IsVisibile = cbShowPokemon.Checked;
            map.Update();
        }
    }
}
