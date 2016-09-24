/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 1:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi.Entities.Elevation.Response;
using PokemonGo.RocketAPI.Logic.Utils;
using PokemonGo.RocketApi.PokeMap;
using POGOProtos.Map.Fort;

namespace PokemonGo.RocketAPI.Console
{
	/// <summary>
	/// Description of LocationPanel.
	/// </summary>
	public partial class LocationPanel : UserControl
	{
		public LocationPanel()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

		}
		private bool asViewOnly;
		
		public void Init(bool asViewOnly, int team = 0, int level = 0, long exp = 0)
        {
            map.Manager.Mode = AccessMode.ServerOnly;

            buttonRefreshPokemon.Visible = false;
            buttonRefreshPokemon.Enabled = false;
            buttonRefreshForts.Visible = false;            
            this.asViewOnly = asViewOnly;
            panel1.Size = new Size(700,47);            
            if (asViewOnly)
            {
            	panel1.Size = new Size(483,71);
                initViewOnly(team, level, exp);               
            }
        }
		
        public double alt;
        public bool close = true;

        private GMarkerGoogle _botMarker;
        private GMapPolygon _circle;
        private GMarkerGoogle _botStartMarker;
        private GMapOverlay routeOverlay;
        private GMapOverlay PokemonOverlay;
        private GMapRoute _botRoute = new GMapRoute("BotRoute");
        private Dictionary<string, GMarkerGoogle> _pokemonMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokemonOverlay = new GMapOverlay("Pokemon");
        private Dictionary<string, GMarkerGoogle> _pokeStopsMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokeStopsOverlay = new GMapOverlay("PokeStops");
        private Dictionary<string, GMarkerGoogle> _pokeGymsMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokeGymsOverlay = new GMapOverlay("PokeGyms");

        delegate void SetTextCallback(double cord);

        private void button1_Click(object sender, EventArgs e)
        {
            Globals.latitute = map.Position.Lat;
            Globals.longitude = map.Position.Lng;
            Globals.altitude = alt;
            close = false;
        }

        private async void buttonRefreshPokemon_Click_1(object sender, EventArgs e)
        {
            buttonRefreshPokemon.Enabled = false;
            if ((await Logic.Logic._instance.CheckAvailablePokemons(Logic.Logic._client)))
            {

                Logger.ColoredConsoleWrite(ConsoleColor.Green, $"Updated PokemonData.", LogLevel.Info);

            }
            else
            {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, $"Could not get PokemonData. Service is overloaded.", LogLevel.Warning);
            }

            //await Logic.Logic._instance.CheckAvailablePokemons(Logic.Logic._client);
            buttonRefreshPokemon.Enabled = true;
        }
        
        private async void buttonRefreshForts_Click(object sender, EventArgs e)
        {
        	var button = ((Button)sender);
        	button.Enabled = false;
            var client = Logic.Logic._client;
            if (client.readyToUse )
            {
            	Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Refreshing Forts", LogLevel.Warning);
                var mapObjects = await client.Map.GetMapObjects();
                var mapCells = mapObjects.Item1.MapCells;
	            var pokeStops =
	            mapCells.SelectMany(i => i.Forts)
	            .Where(
	                i =>
	                i.Type == FortType.Checkpoint &&
	                i.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
	                .OrderBy(
	                i =>
	                LocationUtils.CalculateDistanceInMeters(Globals.latitute, Globals.longitude, i.Latitude, i.Longitude));
	            if (pokeStops.Any() )
	            {
	            	InfoObservable_HandlePokeStop (pokeStops.ToArray());
	            }
	            var pokeGyms = mapCells.SelectMany(i => i.Forts)
	            .Where(
	                i =>
	                i.Type == FortType.Gym )
	                .OrderBy(
	                i =>
	                LocationUtils.CalculateDistanceInMeters(Globals.latitute, Globals.longitude, i.Latitude, i.Longitude));
	            if (pokeGyms.Any() )
	            {
	                
	            }                
	            if (!map.Overlays.Contains(_pokeStopsOverlay)){
	            	map.Overlays.Add(_pokeStopsOverlay);
	            }
	            if (!map.Overlays.Contains(_pokeGymsOverlay)){
	            	map.Overlays.Add(_pokeGymsOverlay);
	            }
            	Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Refreshing Forts Done.", LogLevel.Warning);
            }
           

            button.Enabled = true;
        }        

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(DataCollector.MaxRetryCount * DataCollector.TimeoutBetweenRetries + 10000);
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

        private void cbShowPokemon_CheckedChanged(object sender, EventArgs e)
        {
            _pokemonOverlay.IsVisibile = cbShowPokemon.Checked;
            map.Update();
        }

        private void cbShowPokeStops_CheckedChanged(object sender, EventArgs e)
        {
            _pokeStopsOverlay.IsVisibile = cbShowPokeStops.Checked;
            map.Update();
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
            Invoke(new MethodInvoker(() =>
            {
                textBox1.Text = coords.Latitude.ToString();
                textBox2.Text = coords.Longitude.ToString();
                PointLatLng newPosition = new PointLatLng(coords.Latitude, coords.Longitude);
                _botMarker.Position = newPosition;
                _botRoute.Points.Add(newPosition);
                map.Position = newPosition;
            }));
        }

        void infoObservable_HandleClearPokemon()
        {
            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    if (pokemonLock.WaitOne(5000))
                    {
                        _pokemonMarks.Clear();
                        _pokemonOverlay.Markers.Clear();
                    }
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandleClearPokemon: {0}", e.ToString()));
                }
            }));
        }


        Semaphore pokemonLock = new Semaphore(0, 1);

        void infoObservable_HandleDeletePokemonLocation( string pokemon_Id)
        {
            try {
                _pokemonOverlay.IsVisibile = false;
                var pokemonMarker = _pokemonMarks[pokemon_Id];
                _pokemonOverlay.Markers.Remove(pokemonMarker);
                _pokemonMarks.Remove(pokemon_Id);
                _pokemonOverlay.IsVisibile = true;
            } catch (Exception e) {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "[Ignore]: sending exception information to log file.");
                Logger.AddLog(string.Format("Error in infoObservable_HandleDeletePokemonLocation: {0}", e.ToString()));

            }
        }

        void infoObservable_HandleNewPokemonLocations(List<DataCollector.PokemonMapData> mapData)
        {
            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    //if (pokemonLock.WaitOne(5000))
                    //{
                        if (mapData.Count > 0)
                        {
                            //_pokemonOverlay = new GMapOverlay("Pokemon");
                            _pokemonMarks.Clear();
                            _pokemonOverlay.Markers.Clear();
                            _pokemonOverlay.IsVisibile = false;

                            int prevCount = mapData.Count;
                            //mapData.Where(x => x.Id == null && x.Coordinates.Latitude.HasValue && x.Coordinates.Longitude.HasValue).ToList().ForEach(x => x.Id = x.PokemonId.ToString() + x.Coordinates.Longitude.Value + +x.Coordinates.Latitude.Value);
                            mapData = mapData.Where(x => x.Id != null).ToList();

                            //Logger.ColoredConsoleWrite(ConsoleColor.White, string.Format("Got new Pokemon Count: {0}, unfiltered: {1}", mapData.Count, prevCount));

                            for (int i = mapData.Count - 1; i >= 0; i--)
                            {
                                var pokeData = mapData[i];
                                GMarkerGoogle pokemonMarker;
                                if (pokeData.Type == DataCollector.PokemonMapDataType.Nearby)
                                {
                                    pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), GMarkerGoogleType.black_small);
                                }
                                else
                                {
                                    Bitmap pokebitMap = PokemonsPanel.GetPokemonMediumImage(pokeData.PokemonId);
                                    if (pokebitMap != null)
                                    {
                                        var ImageSize = new System.Drawing.Size(pokebitMap.Width, pokebitMap.Height);
                                        pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), pokebitMap) { Offset = new System.Drawing.Point(-ImageSize.Width / 2, -ImageSize.Height / 2) };
                                    }
                                    else
                                    {
                                        pokemonMarker = new GMarkerGoogle(new PointLatLng(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), GMarkerGoogleType.green_small);
                                    }

                                }
                                pokemonMarker.ToolTipText = string.Format("{0}\nExpires at:{1}\n{2}\n{3},{4}", StringUtils.getPokemonNameByLanguage(null, pokeData.PokemonId), pokeData.ExpiresAt.ToString(), FindAddress(pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value), pokeData.Coordinates.Latitude.Value, pokeData.Coordinates.Longitude.Value);
                                pokemonMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                                pokemonMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                _pokemonMarks.Add(pokeData.Id, pokemonMarker);
                                _pokemonOverlay.Markers.Add(pokemonMarker);
                            }
                        }
                        if (!map.Overlays.Contains(_pokemonOverlay))
                            map.Overlays.Add(_pokemonOverlay);
                        _pokemonOverlay.IsVisibile = true;//cbShowPokemon.Checked;
                    //}
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandleNewPokemonLocations: {0}", e.ToString()));
                }
            }));
        }

        private void InfoObservable_HandlePokeStop(POGOProtos.Map.Fort.FortData[] pokeStops)
        {
            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    if (pokeStops.Length > 0)
                    {
                        _pokeStopsOverlay.IsVisibile = false;
                        _pokeStopsOverlay.Markers.Clear();
                        _pokeStopsMarks.Clear();
                        routeOverlay.Polygons.Clear();
                        routeOverlay.Polygons.Add(_circle = CreateCircle(new PointLatLng(Globals.latitute, Globals.longitude), Globals.radius, 100));
                        routeOverlay.Markers.Clear();
                        _botStartMarker = new GMarkerGoogle(new PointLatLng(), Properties.MapData.start_point);
                        _botStartMarker.Position = new PointLatLng(Globals.latitute, Globals.longitude);
                        _botStartMarker.ToolTipText = string.Format("Start Point.\n{0}\n{1},{2}", FindAddress(Globals.latitute, Globals.longitude), Globals.latitute, Globals.longitude);
                        _botStartMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                        _botStartMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        routeOverlay.Markers.Add(_botStartMarker);
                        routeOverlay.Markers.Add(_botMarker);
                        int prevCount = pokeStops.Length;
                        //<var filteredPokeStops = pokeStops;
                        var filteredPokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.Logic._instance._clientSettings.DefaultLatitude, Logic.Logic._instance._clientSettings.DefaultLongitude, i.Latitude, i.Longitude) <= Logic.Logic._instance._clientSettings.MaxWalkingRadiusInMeters).ToArray();
                        Logger.ColoredConsoleWrite(ConsoleColor.White, string.Format("Got new Pokestop Count: {0}, unfiltered: {1}", filteredPokeStops.Length, pokeStops.Length));

                        for (int i = filteredPokeStops.Length - 1; i >= 0; i--)
                        {
                            var pokeStop = filteredPokeStops[i];
                            if (pokeStop.Id != null)
                            {
                                var pokeStopMaker = new GMarkerGoogle(new PointLatLng(pokeStop.Latitude, pokeStop.Longitude), Properties.MapData.pokestop);
                                if (pokeStop.ActiveFortModifier.Count > 0)
                                {
                                    pokeStopMaker = new GMarkerGoogle(new PointLatLng(pokeStop.Latitude, pokeStop.Longitude), Properties.MapData.lured_pokestop);
                                }

                                pokeStopMaker.ToolTipText = string.Format("{0}\n{1},{2}", FindAddress(pokeStop.Latitude, pokeStop.Longitude), pokeStop.Latitude, pokeStop.Longitude);
                                pokeStopMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                                pokeStopMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                _pokeStopsMarks.Add(pokeStop.Id, pokeStopMaker);
                                _pokeStopsOverlay.Markers.Add(pokeStopMaker);
                            }
                            else
                            {
                                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeStop.Id is null."));
                            }
                        }
                        if (!map.Overlays.Contains(_pokeStopsOverlay))
                            map.Overlays.Add(_pokeStopsOverlay);
                        _pokeStopsOverlay.IsVisibile = true;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeStops length is 0."));
                    }
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandlePokeStop: {0}", e.ToString()));
                }
            }));
        }

        private int GetLevel(long value)
        {
            if (value >= 50000)
                return 10;
            if (value >= 40000)
                return 9;
            if (value >= 30000)
                return 8;
            if (value >= 20000)
                return 7;
            if (value >= 16000)
                return 6;
            if (value >= 12000)
                return 5;
            if (value >= 8000)
                return 4;
            if (value >= 4000)
                return 3;
            if (value >= 2000)
                return 2;
            return 1;
        }

        private void InfoObservable_HandlePokeGym(POGOProtos.Map.Fort.FortData[] forts)
        {
            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    if (forts.Length > 0)
                    {
                        _pokeGymsOverlay.IsVisibile = false;
                        _pokeGymsOverlay.Markers.Clear();
                        _pokeGymsMarks.Clear();
                        int prevCount = forts.Length;
                        //< var filteredForts = forts;
                        var filteredForts = forts.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.Logic._instance._clientSettings.DefaultLatitude, Logic.Logic._instance._clientSettings.DefaultLongitude, i.Latitude, i.Longitude) <= Logic.Logic._instance._clientSettings.MaxWalkingRadiusInMeters).ToArray();
                        Logger.ColoredConsoleWrite(ConsoleColor.White, string.Format("Got new Gym Count: {0}, unfiltered: {1}", filteredForts.Length, forts.Length));

                        for (int i = filteredForts.Length - 1; i >= 0; i--)
                        {
                            var pokeGym = filteredForts[i];
                            if (pokeGym.Id != null)
                            {
                                var bitmap = Properties.MapData.pokegym;
                                switch (pokeGym.OwnedByTeam)
                                {
                                    case POGOProtos.Enums.TeamColor.Blue:
                                        bitmap = Properties.MapData.pokegym_blue;
                                        break;
                                    case POGOProtos.Enums.TeamColor.Red:
                                        bitmap = Properties.MapData.pokegym_red;
                                        break;
                                    case POGOProtos.Enums.TeamColor.Yellow:
                                        bitmap = Properties.MapData.pokegym_yellow;
                                        break;
                                };

                                var str = StringUtils.getPokemonNameByLanguage(null, pokeGym.GuardPokemonId);
                                str = string.Format("Guard: {0} - CP: {1}\nLevel:{2} ({3})", str, pokeGym.GuardPokemonCp, GetLevel(pokeGym.GymPoints), pokeGym.GymPoints);
                                var pokeGymMaker = new GMarkerGoogle(new PointLatLng(pokeGym.Latitude, pokeGym.Longitude), bitmap);
                                pokeGymMaker.ToolTipText = string.Format("{0}\n{1}, {2}\n{3}", FindAddress(pokeGym.Latitude, pokeGym.Longitude), pokeGym.Latitude, pokeGym.Longitude, str);
                                pokeGymMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                                pokeGymMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                                _pokeGymsMarks.Add(pokeGym.Id, pokeGymMaker);
                                _pokeGymsOverlay.Markers.Add(pokeGymMaker);
                            }
                            else
                            {
                                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeGym.Id is null."));
                            }
                        }
                        if (!map.Overlays.Contains(_pokeGymsOverlay))
                            map.Overlays.Add(_pokeGymsOverlay);
                        _pokeGymsOverlay.IsVisibile = true;
                    }
                    else
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeGym length is 0."));
                    }
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandlePokeStop: {0}", e.ToString()));
                }
            }));
        }

        private void InfoObservable_HandlePokeStopInfoUpdate(POGOProtos.Map.Fort.FortData pokeStop, string info)
        {
            Invoke(new MethodInvoker(() =>
            {
                try
                {
                    if (_pokeStopsMarks.ContainsKey(pokeStop.Id))
                    {
                        //changeType
                        var bmp = Properties.MapData.visited_pokestop;
                        if (pokeStop.ActiveFortModifier.Count > 0)
                            bmp = Properties.MapData.visited_lured_pokestop;
                        var newMark = new GMarkerGoogle(_pokeStopsMarks[pokeStop.Id].Position, bmp);

                        newMark.ToolTipText = info;
                        newMark.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                        try
                        {
                            _pokeStopsOverlay.Markers[_pokeStopsOverlay.Markers.IndexOf(_pokeStopsMarks[pokeStop.Id])] = newMark;
                        }
                        catch (Exception e)
                        {
                            Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                            Logger.AddLog(string.Format("Error in HandlePokeStopInfoUpdate: {0}", e.ToString()));
                        }
                        _pokeStopsMarks[pokeStop.Id] = newMark;
                    }
                }
                catch (Exception e)
                {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandlePokeStopInfoUpdate: {0}", e.ToString()));
                }
            }));
        }

        private void initViewOnly(int team, int level, long exp)
        {
            //first hide all controls
            foreach (Control c in Controls)
            {
                c.Visible = false;
            }

            Bitmap bmp = Properties.MapData.player;
            switch (team)
            {
                case 1:
                    bmp = Properties.MapData.player_blue;
                    break;
                case 2:
                    bmp = Properties.MapData.player_red;
                    break;
                case 3:
                    bmp = Properties.MapData.player_yellow;
                    break;
            }

            //show map
            map.Visible = true;
            map.Dock = DockStyle.Fill;
            map.ShowCenter = false;
            routeOverlay = new GMapOverlay();
            routeOverlay.Routes.Add(_botRoute);
            PokemonOverlay = new GMapOverlay();
            CreateBotMarker(team, level, exp);
            //routeOverlay.Markers.Add(_botMarker);
            _botStartMarker = new GMarkerGoogle(new PointLatLng(), Properties.MapData.start_point);
            _botStartMarker.Position = new PointLatLng(Globals.latitute, Globals.longitude);
            _botStartMarker.ToolTipText = string.Format("Start Point.\n{0}\n{1},{2}", FindAddress(Globals.latitute, Globals.longitude), Globals.latitute, Globals.longitude);
            _botStartMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
            _botStartMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            routeOverlay.Markers.Add(_botStartMarker);
            _circle = CreateCircle(new PointLatLng(Globals.latitute, Globals.longitude), Globals.radius, 100);
            routeOverlay.Polygons.Add(_circle);
            
            map.Overlays.Add(routeOverlay);
            map.Overlays.Add(_pokeStopsOverlay);
            map.Overlays.Add(_pokemonOverlay);
            map.Overlays.Add(_pokeGymsOverlay);
            //show geodata controls
            label1.Visible = true;
            label2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            buttonRefreshPokemon.Visible = true;
            buttonRefreshPokemon.Enabled = true;
            buttonRefreshForts.Visible = true;
            cbShowPokeStops.Visible = true;
            cbShowPokemon.Visible = true;
            _pokemonOverlay.IsVisibile = true;
            _pokeStopsOverlay.IsVisibile = true;
            _pokeGymsOverlay.IsVisibile = true;
            if (cbShowPokemon.Checked)
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
                //Logic.Logic._instance.CheckAvailablePokemons(Logic.Logic._client);
            }

            //don't ask at closing
            close = false;
            //add & remove live data handler after form loaded
            Globals.infoObservable.HandleNewGeoLocations += handleLiveGeoLocations;
            Globals.infoObservable.HandleAvailablePokeStop += InfoObservable_HandlePokeStop;
            Globals.infoObservable.HandleAvailablePokeGym += InfoObservable_HandlePokeGym;
            Globals.infoObservable.HandlePokeStopInfoUpdate += InfoObservable_HandlePokeStopInfoUpdate;
            Globals.infoObservable.HandleClearPokemon += infoObservable_HandleClearPokemon;
            Globals.infoObservable.HandleNewPokemonLocations += infoObservable_HandleNewPokemonLocations;
            Globals.infoObservable.HandleDeletePokemonLocation += infoObservable_HandleDeletePokemonLocation;
        }


        private void map_Load(object sender, EventArgs e)
        {
            Globals.MapLoaded = true;
            showMap();
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

        private void map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (Globals.pauseAtPokeStop)
            {
                Globals.RouteToRepeat.AddLast(new GeoCoordinate(item.Position.Lat, item.Position.Lng));
                item.ToolTipText = string.Format("Stop {0} Queued", Globals.RouteToRepeat.Count);
            }
            else
            {
                Globals.NextDestinationOverride.AddFirst(new GeoCoordinate(item.Position.Lat, item.Position.Lng));
                if (!item.ToolTipText.Contains("\nNext Destination Marked")){
                	item.ToolTipText += "\nNext Destination Marked";
                }
            }
        }

        private void SetText(double cord)
        {
            if (textBox3.InvokeRequired)
            {
                SetTextCallback d = SetText;
                Invoke(d, cord);
            }
            else
            {
                textBox3.Text = cord.ToString(CultureInfo.InvariantCulture);
                alt = cord;
            }
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox1.Text != "-")
            {
                try
                {
                    double lat = double.Parse(textBox1.Text.Replace(',', '.'), GUI.cords, System.Globalization.NumberFormatInfo.InvariantInfo);
                    if (lat > 90.0 || lat < -90.0)
                    {
                        throw new System.ArgumentException("Value has to be between 180 and -180!");
                    }
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
                    {
                        throw new System.ArgumentException("Value has to be between 90 and -90!");
                    }
                    map.Position = new GMap.NET.PointLatLng(map.Position.Lat, lng);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    textBox2.Text = "";
                }
            }
        }
        public static string FindAddress(double lat, double lng)
        {
            string ret = "";
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPlacemark(new PointLatLng(lat, lng), out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null)
            {
                ret = pos.Value.Address;
            }
            return ret;
        }
        public static double[] FindLocation(string address)
        {
        	double[] ret = {0.0,0.0};
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPoint(address, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null)
            {
            	ret = new double[2];
            	ret[0] =pos.Value.Lat;
            	ret[1] =pos.Value.Lng;            	
            }
            return ret;
        }
        
		void BtnGetPointsClick(object sender, EventArgs e)
		{
			var ret = FindLocation(tbAddress.Text);
			textBox1.Text = ret[0].ToString();
			textBox2.Text = ret[1].ToString();
			map.Position = new PointLatLng(ret[0],ret[1]);
		}
        
		public void CreateBotMarker(int team, int level, long exp){
            Bitmap bmp = Properties.MapData.player;
            switch (team)
            {
                case 1:
                    bmp = Properties.MapData.player_blue;
                    break;
                case 2:
                    bmp = Properties.MapData.player_red;
                    break;
                case 3:
                    bmp = Properties.MapData.player_yellow;
                    break;
            }
            routeOverlay.IsVisibile =false;
            PointLatLng pointLatLng;
            if (_botMarker !=null){
        		pointLatLng = _botMarker.Position;
        		routeOverlay.Markers.Remove(_botMarker);
            }else{
        		pointLatLng = new PointLatLng();
            }
            _botMarker = new GMarkerGoogle(pointLatLng, bmp);
            _botMarker.ToolTipText = string.Format("Level: {0} ({1})", level, exp);
            _botMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
            _botMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            routeOverlay.Markers.Add(_botMarker);
			routeOverlay.IsVisibile =true;
		}        
    }
}
