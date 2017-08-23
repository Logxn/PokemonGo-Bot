﻿/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 15/09/2016
 * Time: 1:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System.Collections;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using POGOProtos.Map.Fort;
using POGOProtos.Map.Pokemon;
using PokeMaster.Components;
using PokeMaster.Helper;
using PokeMaster.Logic.Shared;
using PokeMaster.Logic.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Location;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using PokemonGo.RocketAPI;

namespace PokeMaster
{
    /// <summary>
    /// Description of LocationPanel.
    /// </summary>
    public partial class LocationPanel : UserControl
    {
        private Helper.TranslatorHelper th = Helper.TranslatorHelper.getInstance();
        public LocationPanel()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //
            InitializeComponent();
            panel1.Size = new System.Drawing.Size(829, 92);
        }
        private bool asViewOnly;
        
        public void Init(bool asViewOnly, int team = 0, int level = 0, long exp = 0)
        {
            map.Manager.Mode = AccessMode.ServerOnly;

            buttonRefreshForts.Visible = false;            
            this.asViewOnly = asViewOnly;
            panel1.Size = new Size(700, 47);            
            if (asViewOnly) {
                panel1.Size = new Size(483, 71);
                initViewOnly(team, level, exp);
            } else {
                radiusOverlay = new GMapOverlay();
                map.Overlays.Add(radiusOverlay);
                nudRadius.Value = GlobalVars.radius;
                btnPauseWalking.Visible = false;
                if (GlobalVars.SaveForts)
                    LoadPokestopsInfo();
            }
            buttonZoomOut.Visible = true;
            buttonZoomIn.Visible = true;
            buttonSavePokestops.Visible = true;
            buttonLoadPokestops.Visible = true;
            btnPauseWalking.Text = GlobalVars.pauseAtPokeStop? th.TS("Resume Walking"): th.TS("Pause Walking");
        }
        
        public bool close = true;

        private GMarkerGoogle _botMarker;
        private GMapPolygon _circle;
        private GMarkerGoogle _botStartMarker;
        private GMapOverlay routeOverlay;
        private GMapOverlay radiusOverlay;
        private GMapOverlay PokemonOverlay;
        private GMapRoute _botRoute = new GMapRoute("BotRoute");
        private Dictionary<string, GMarkerGoogle> _pokemonMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokemonOverlay = new GMapOverlay("Pokemon");
        private Dictionary<string, GMarkerGoogle> _pokeStopsMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokeStopsOverlay = new GMapOverlay("PokeStops");
        private Dictionary<string, GMarkerGoogle> _pokeGymsMarks = new Dictionary<string, GMarkerGoogle>();
        private GMapOverlay _pokeGymsOverlay = new GMapOverlay("PokeGyms");


        delegate void SetTextCallback(decimal cord);

        private void button1_Click(object sender, EventArgs e)
        {
            GlobalVars.latitude = map.Position.Lat;
            GlobalVars.longitude = map.Position.Lng;
            GlobalVars.altitude = LocationUtils.GetAltitude(map.Position.Lat, map.Position.Lng);
            GlobalVars.radius = (int)nudRadius.Value;
            close = false;
        }

        private async void buttonRefreshForts_Click(object sender, EventArgs e)
        {
            var button = ((Button)sender);
            button.Enabled = false;
            var client = Logic.Logic.objClient;
            if (client.ReadyToUse) {
                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Refreshing Forts", LogLevel.Warning);
                var mapObjects = await client.Map.GetMapObjects().ConfigureAwait(false);
                var mapCells = mapObjects.MapCells;
                var pokeStops =
                    mapCells.SelectMany(i => i.Forts)
                .Where(
                        i =>
                    i.Type == FortType.Checkpoint &&
                        i.CooldownCompleteTimestampMs < (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds)
                    .OrderBy(
                        i =>
                    LocationUtils.CalculateDistanceInMeters(GlobalVars.latitude, GlobalVars.longitude, i.Latitude, i.Longitude));
                if (pokeStops.Any()) {
                    InfoObservable_HandlePokeStop(pokeStops.ToArray());
                }
                var pokeGyms = mapCells.SelectMany(i => i.Forts)
                .Where(
                                   i =>
                    i.Type == FortType.Gym)
                    .OrderBy(
                                   i =>
                    LocationUtils.CalculateDistanceInMeters(GlobalVars.latitude, GlobalVars.longitude, i.Latitude, i.Longitude));
                if (pokeGyms.Any()) {
                    
                }                
                if (!map.Overlays.Contains(_pokeStopsOverlay)) {
                    map.Overlays.Add(_pokeStopsOverlay);
                }
                if (!map.Overlays.Contains(_pokeGymsOverlay)) {
                    map.Overlays.Add(_pokeGymsOverlay);
                }
                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Refreshing Forts Done.", LogLevel.Warning);
            }
           

            button.Enabled = true;
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
            var gpollist = new List<PointLatLng>();
            double seg = Math.PI * 2 / segments;
            for (int i = 0; i < segments; i++) {
                double theta = seg * i;
                double a = point.Lat + Math.Cos(theta) * radius * 0.75;
                double b = point.Lng + Math.Sin(theta) * radius;
                gpollist.Add(new PointLatLng(a, b));
            }
            var circle = new GMapPolygon(gpollist, "BotZone");
            circle.Stroke = Pens.Black;
            circle.Fill = Brushes.Transparent;
            return circle;
        }

        private void handleLiveGeoLocations(GeoCoordinate coords)
        {
            Invoke(new MethodInvoker(() => {
                try {
                    var newPosition = new PointLatLng(coords.Latitude, coords.Longitude);
                    _botMarker.Position = newPosition;
                    _botRoute.Points.Add(newPosition);
                    map.Position = newPosition;
                    if (!asViewOnly) {
                        textBox1.Text = coords.Latitude.ToString(CultureInfo.InvariantCulture);
                        textBox2.Text = coords.Longitude.ToString(CultureInfo.InvariantCulture);
                        tbAddress.Text = LocationUtils.FindAddress(newPosition);
                    }
                } catch (Exception e) {
                    Logger.ExceptionInfo(string.Format("Error in handleLiveGeoLocations: {0}", e));
                }
            }));
        }

        void infoObservable_HandleClearPokemon()
        {
            Invoke(new MethodInvoker(() => {
                try {
                    _pokemonMarks.Clear();
                    _pokemonOverlay.Markers.Clear();
                } catch (Exception e) {
                    Logger.ExceptionInfo(string.Format("Error in HandleClearPokemon: {0}", e));
                }
            }));
        }


        void infoObservable_HandleDeletePokemonLocation(string pokemon_Id)
        {
            Invoke(new MethodInvoker(() => {
                try {
                    if (_pokemonMarks.ContainsKey(pokemon_Id)) {
                        _pokemonOverlay.IsVisibile = false;
                        var pokemonMarker = _pokemonMarks[pokemon_Id];
                        _pokemonOverlay.Markers.Remove(pokemonMarker);
                        _pokemonMarks.Remove(pokemon_Id);
                        _pokemonOverlay.IsVisibile = true;
                    }
                } catch (Exception e) {
                    Logger.ExceptionInfo(string.Format("Error in infoObservable_HandleDeletePokemonLocation: {0}", e));
                }
            }));
        }

        void infoObservable_HandleNewPokemonLocation(MapPokemon mapPokemon)
        {
            Invoke(new MethodInvoker(() => {
                if (!_pokemonMarks.ContainsKey(mapPokemon.SpawnPointId)) {
                    GMarkerGoogle pokemonMarker;
                    Bitmap pokebitMap = PokeImgManager.GetPokemonMediumImage(mapPokemon.PokemonId);
                    if (pokebitMap != null) {
                        var ImageSize = new System.Drawing.Size(pokebitMap.Width, pokebitMap.Height);
                        pokemonMarker = new GMarkerGoogle(new PointLatLng(mapPokemon.Latitude, mapPokemon.Longitude), pokebitMap) { Offset = new System.Drawing.Point(-ImageSize.Width / 2, -ImageSize.Height / 2) };
                    } else {
                        pokemonMarker = new GMarkerGoogle(new PointLatLng(mapPokemon.Latitude, mapPokemon.Longitude), GMarkerGoogleType.green_small);
                    }
                    var expriationTime = StringUtils.TimeMStoString(mapPokemon.ExpirationTimestampMs, @"mm\:ss");
                    Logger.Debug("Expires in: " + expriationTime);
                    var address = LocationUtils.FindAddress(mapPokemon.Latitude, mapPokemon.Longitude);
                    pokemonMarker.ToolTipText = th.TS("{0}\nExpires in: {1}\n{2}\n{3},{4}", new object[] {
                        mapPokemon.PokemonId,
                        expriationTime,
                        address,
                        mapPokemon.Latitude,
                        mapPokemon.Longitude
                    });
                    pokemonMarker.ToolTip.Font = new Font("Arial", 12, GraphicsUnit.Pixel);
                    pokemonMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    _pokemonMarks.Add(mapPokemon.SpawnPointId, pokemonMarker);
                    _pokemonOverlay.Markers.Add(pokemonMarker);
                }
            }));
        }

        void infoObservable_HandleNewPokemonLocations(IEnumerable<MapPokemon> mapData)
        {
            Invoke(new MethodInvoker(() => {
                try {
                    if (mapData.Any()) {
                        _pokemonOverlay.IsVisibile = false;
                        _pokemonMarks.Clear();
                        _pokemonOverlay.Markers.Clear();
                        foreach (var pokeData in mapData)
                            infoObservable_HandleNewPokemonLocation(pokeData);
                    }
                    if (!map.Overlays.Contains(_pokemonOverlay))
                        map.Overlays.Add(_pokemonOverlay);
                    _pokemonOverlay.IsVisibile = true;
                } catch (Exception e) {
                    Logger.ExceptionInfo(string.Format("Error in HandleNewPokemonLocations: {0}", e.ToString()));
                }
            }));
        }

        private void SavePokestopsInfo()
        {
            var file = GlobalVars.FortsFile;
            if (file == "" || !System.IO.File.Exists(file))
                file = System.IO.Path.Combine(Program.path, "forts.json");
            var array = new List<MarkerInfo>();
            var infoJSON = "";
            if (System.IO.File.Exists(file)) {
                infoJSON = System.IO.File.ReadAllText(file);
                array = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MarkerInfo>>(infoJSON);
            }

            foreach (var element in _pokeStopsOverlay.Markers) {
                var mInfo = new MarkerInfo();
                mInfo.type = 0;
                mInfo.info = element.ToolTipText;
                mInfo.location = element.Position;
                if (!ContainsFort(array, mInfo))
                    array.Add(mInfo);
            }
            foreach (var element in _pokeGymsOverlay.Markers) {
                var mInfo = new MarkerInfo();
                mInfo.type = 1;
                mInfo.info = element.ToolTipText;
                mInfo.location = element.Position;
                if (!ContainsFort(array, mInfo))
                    array.Add(mInfo);
            }
            infoJSON = Newtonsoft.Json.JsonConvert.SerializeObject(array, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText(file, infoJSON);
        }

        private bool ContainsFort(List<MarkerInfo> list, MarkerInfo fort)
        {
            foreach (var element in list) {
                if (element.location == fort.location)
                    return true;
            }
            return false;
        }

        private void LoadPokestopsInfo()
        {
            var file = GlobalVars.FortsFile;
            if (file == "")
                file = System.IO.Path.Combine(Program.path, "forts.json");
            
            if (System.IO.File.Exists(file)) {
                var infoJSON = System.IO.File.ReadAllText(file);
                _pokeStopsOverlay.IsVisibile = false;
                _pokeStopsOverlay.Markers.Clear();
                _pokeGymsOverlay.IsVisibile = false;
                _pokeGymsOverlay.Markers.Clear();
                var markers = Newtonsoft.Json.JsonConvert.DeserializeObject<MarkerInfo[]>(infoJSON);
                foreach (var element in markers) {
                    GMarkerGoogle fortMaker = null;
                    if (element.type == 0)
                        fortMaker = new GMarkerGoogle(element.location, Properties.MapData.pokestop);
                    else
                        fortMaker = new GMarkerGoogle(element.location, Properties.MapData.pokegym);
                    if (element.info != null) {
                        fortMaker.ToolTipText = element.info;
                        fortMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                        fortMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                    }
                    if (element.type == 0)
                        _pokeStopsOverlay.Markers.Add(fortMaker);
                    else
                        _pokeGymsOverlay.Markers.Add(fortMaker);
                }
                if (!map.Overlays.Contains(_pokeStopsOverlay))
                    map.Overlays.Add(_pokeStopsOverlay);
                _pokeStopsOverlay.IsVisibile = true;
                if (!map.Overlays.Contains(_pokeGymsOverlay))
                    map.Overlays.Add(_pokeGymsOverlay);
                _pokeGymsOverlay.IsVisibile = true;
            }
        }

        private void InfoObservable_HandlePokeStop(FortData pokeStop, bool visited = false, string info ="")
        {
            Invoke(new MethodInvoker(() => {
                if (pokeStop.Id != null) {
                    var latLng = new PointLatLng(pokeStop.Latitude, pokeStop.Longitude);
                    var poke = (visited)? Properties.MapData.visited_pokestop:Properties.MapData.pokestop;
                    if (pokeStop.ActiveFortModifier.Count > 0) {
                        poke = (visited)? Properties.MapData.visited_lured_pokestop: Properties.MapData.lured_pokestop;
                    }
                    var pokeStopMaker = new GMarkerGoogle(latLng, poke );

                    var tooltipText = "";
                    if (_pokeStopsMarks.ContainsKey(pokeStop.Id)){
                        var markerToDel = _pokeStopsMarks[pokeStop.Id];
                        tooltipText = markerToDel.ToolTipText;
                        if (_pokeStopsOverlay.Markers.Contains(markerToDel))
                            _pokeStopsOverlay.Markers.Remove(markerToDel);
                         _pokeStopsMarks.Remove(pokeStop.Id);
                    }else{
                        if (info=="")
                            pokeStopMaker.ToolTipText = string.Format("{0}\n{1},{2}", LocationUtils.FindAddress(pokeStop.Latitude, pokeStop.Longitude), pokeStop.Latitude, pokeStop.Longitude);
                    }
                    if (info!="")
                        pokeStopMaker.ToolTipText = info;

                    if (pokeStopMaker.ToolTip==null)
                        pokeStopMaker.ToolTip = new GMapToolTip(pokeStopMaker);
                    pokeStopMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                    
                    pokeStopMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;

                    _pokeStopsMarks.Add(pokeStop.Id, pokeStopMaker);
                    _pokeStopsOverlay.Markers.Add(pokeStopMaker);
                    if (visited)
                        Task.Delay(300000)
                            .ContinueWith(t => InfoObservable_HandlePokeStop(pokeStop,false,pokeStopMaker.ToolTipText));
                        
                } else {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeStop.Id is null."));
                }
            }));
        }

        private void InfoObservable_HandlePokeStop(FortData[] pokeStops)
        {
            Invoke(new MethodInvoker(() => {
                try {
                    if (pokeStops.Length > 0) {
                        _pokeStopsOverlay.IsVisibile = false;
                        _pokeStopsOverlay.Markers.Clear();
                        _pokeStopsMarks.Clear();
                        routeOverlay.Polygons.Clear();
                        routeOverlay.Polygons.Add(_circle = CreateCircle(new PointLatLng(GlobalVars.latitude, GlobalVars.longitude), GlobalVars.radius, 100));
                        routeOverlay.Markers.Clear();
                        _botStartMarker = new GMarkerGoogle(new PointLatLng(), Properties.MapData.start_point);
                        _botStartMarker.Position = new PointLatLng(GlobalVars.latitude, GlobalVars.longitude);
                        _botStartMarker.ToolTipText = string.Format("Start Point.\n{0}\n{1},{2}", LocationUtils.FindAddress(GlobalVars.latitude, GlobalVars.longitude), GlobalVars.latitude, GlobalVars.longitude);
                        _botStartMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
                        _botStartMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
                        routeOverlay.Markers.Add(_botStartMarker);
                        routeOverlay.Markers.Add(_botMarker);
                        int prevCount = pokeStops.Length;

                        // TODO: Make this filter optionable.
                        //var filteredPokeStops = pokeStops.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.Logic.Instance.BotSettings.DefaultLatitude, Logic.Logic.Instance.BotSettings.DefaultLongitude, i.Latitude, i.Longitude) <= Logic.Logic.Instance.BotSettings.MaxWalkingRadiusInMeters).ToArray();
                        //Logger.ColoredConsoleWrite(ConsoleColor.White, string.Format("Got new Pokestop Count: {0}, unfiltered: {1}", filteredPokeStops.Length, pokeStops.Length));
                        var filteredPokeStops = pokeStops;

                        for (int i = filteredPokeStops.Length - 1; i >= 0; i--) {
                            InfoObservable_HandlePokeStop(filteredPokeStops[i]);
                        }
                        if (!map.Overlays.Contains(_pokeStopsOverlay))
                            map.Overlays.Add(_pokeStopsOverlay);
                        _pokeStopsOverlay.IsVisibile = true;
                        if (GlobalVars.SaveForts)
                            SavePokestopsInfo();
                    } else {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeStops length is 0."));
                    }
                } catch (Exception e) {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandlePokeStop: {0}", e.ToString()));
                }
            }));
        }

        private void InfoObservable_HandleUpdatePokeGym(FortData pokeGym)
        {
            Invoke(new MethodInvoker(() => {
            
            var bitmap = Properties.MapData.pokegym;
            var color = Color.Black;
            switch (pokeGym.OwnedByTeam) {
                case POGOProtos.Enums.TeamColor.Blue:
                    bitmap = Properties.MapData.pokegym_blue;
                    color = Color.Blue;
                    break;
                case POGOProtos.Enums.TeamColor.Red:
                    bitmap = Properties.MapData.pokegym_red;
                    color = Color.Red;
                    break;
                case POGOProtos.Enums.TeamColor.Yellow:
                    bitmap = Properties.MapData.pokegym_yellow;
                    color = Color.Yellow;
                    break;
            }

            var str = "";
            var pokeGymMaker = new GMarkerGoogle(new PointLatLng(pokeGym.Latitude, pokeGym.Longitude), bitmap);
            if (pokeGym.RaidInfo!=null)
                str = string.Format("Raid Info: {0}, level: {1}, starts: {2}, ends: {3}",pokeGym.RaidInfo.RaidPokemon.PokemonId,pokeGym.RaidInfo.RaidLevel,
                                         StringUtils.TimeMStoString(pokeGym.RaidInfo.RaidBattleMs, @"mm\:ss"),
                                         StringUtils.TimeMStoString(pokeGym.RaidInfo.RaidEndMs, @"mm\:ss"));
            pokeGymMaker.ToolTipText = string.Format("{0}\n{1}, {2}\n{3}\nID: {4}", LocationUtils.FindAddress(pokeGym.Latitude, pokeGym.Longitude), pokeGym.Latitude, pokeGym.Longitude, str, pokeGym.Id);
            pokeGymMaker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
            pokeGymMaker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            

            if (_pokeGymsMarks.ContainsKey(pokeGym.Id)){
                var markerToDel = _pokeGymsMarks[pokeGym.Id];
                if (_pokeGymsOverlay.Markers.Contains(markerToDel))
                    _pokeGymsOverlay.Markers.Remove(markerToDel);
                _pokeGymsMarks.Remove(pokeGym.Id);
            }
            _pokeGymsMarks.Add(pokeGym.Id, pokeGymMaker);
             _pokeGymsOverlay.Markers.Add(pokeGymMaker);

            #region Show Guard
            if (_pokeGymsMarks.ContainsKey(pokeGym.Id + "-Guard")){
                var markerToDel = _pokeGymsMarks[pokeGym.Id + "-Guard"];
                if (_pokeGymsOverlay.Markers.Contains(markerToDel))
                    _pokeGymsOverlay.Markers.Remove(markerToDel);
                 _pokeGymsMarks.Remove(pokeGym.Id +"-Guard");
            }
            GMarkerGoogle guardPokemonMarker;
            Bitmap pokebitMap = PokeImgManager.GetPokemonMediumImage(pokeGym.GuardPokemonId);
            var offsetY = 0;
            if (pokebitMap != null) {
                for (int idx = 0; idx < pokebitMap.Width; idx++) {
                    pokebitMap.SetPixel(idx, 0, color);
                    pokebitMap.SetPixel(idx, pokebitMap.Height - 1, color);
                }
                for (int idx = 0; idx < pokebitMap.Height; idx++) {
                    pokebitMap.SetPixel(0, idx, color);
                    pokebitMap.SetPixel(pokebitMap.Width - 1, idx, color);
                }
                var ImageSize = new System.Drawing.Size(pokebitMap.Width, pokebitMap.Height);
                guardPokemonMarker = new GMarkerGoogle(new PointLatLng(pokeGym.Latitude, pokeGym.Longitude), pokebitMap);
                offsetY = 5 - pokebitMap.Height / 2;
                guardPokemonMarker.Offset = new Point(-bitmap.Width / 2 - 8, offsetY - bitmap.Height);

                _pokeGymsMarks.Add(pokeGym.Id + "-Guard", guardPokemonMarker);
                _pokeGymsOverlay.Markers.Add(guardPokemonMarker);
            }
            #endregion Show Guard

            #region Show fighting
            if (_pokeGymsMarks.ContainsKey(pokeGym.Id + "-Fighting")){
                var markerToDel = _pokeGymsMarks[pokeGym.Id + "-Fighting"];
                if (_pokeGymsOverlay.Markers.Contains(markerToDel))
                    _pokeGymsOverlay.Markers.Remove(markerToDel);
                 _pokeGymsMarks.Remove(pokeGym.Id +"-Fighting");
            }
            GMarkerGoogle FightingMarker;
            if (pokeGym.IsInBattle) {
                for (int idx = 0; idx < pokebitMap.Width; idx++) {
                    pokebitMap.SetPixel(idx, 0, color);
                    pokebitMap.SetPixel(idx, pokebitMap.Height - 1, color);
                }
                for (int idx = 0; idx < pokebitMap.Height; idx++) {
                    pokebitMap.SetPixel(0, idx, color);
                    pokebitMap.SetPixel(pokebitMap.Width - 1, idx, color);
                }
                FightingMarker = new GMarkerGoogle(new PointLatLng(pokeGym.Latitude, pokeGym.Longitude),  GMarkerGoogleType.red_small );
                if (pokebitMap != null) {
                    FightingMarker.Offset = new Point(-bitmap.Width / 2 , 5 - pokebitMap.Height * 2 );
                }

                _pokeGymsMarks.Add(pokeGym.Id + "-Fighting", FightingMarker);
                _pokeGymsOverlay.Markers.Add(FightingMarker);
            }
            #endregion Show figting

            }));

        }

        private void InfoObservable_HandlePokeGym(POGOProtos.Map.Fort.FortData[] forts)
        {
            Invoke(new MethodInvoker(() => {
                try {
                    if (forts.Length > 0) {
                        _pokeGymsOverlay.IsVisibile = false;
                        _pokeGymsOverlay.Markers.Clear();
                        _pokeGymsMarks.Clear();
                        int prevCount = forts.Length;

                        var filteredForts = forts.Where(i => LocationUtils.CalculateDistanceInMeters(Logic.Logic.Instance.BotSettings.DefaultLatitude, Logic.Logic.Instance.BotSettings.DefaultLongitude, i.Latitude, i.Longitude) <= Logic.Logic.Instance.BotSettings.MaxWalkingRadiusInMeters).ToArray();
                        Logger.ColoredConsoleWrite(ConsoleColor.White, string.Format("Got new Gym Count: {0}, unfiltered: {1}", filteredForts.Length, forts.Length));

                        for (int i = filteredForts.Length - 1; i >= 0; i--) {
                            var pokeGym = filteredForts[i];
                            if (pokeGym.Id != null) {
                                InfoObservable_HandleUpdatePokeGym(pokeGym);

                            } else {
                                Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeGym.Id is null."));
                            }
                        }
                        if (!map.Overlays.Contains(_pokeGymsOverlay))
                            map.Overlays.Add(_pokeGymsOverlay);
                        _pokeGymsOverlay.IsVisibile = true;
                        if (GlobalVars.SaveForts)
                            SavePokestopsInfo();
                        
                    } else {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, string.Format("Ignore this: pokeGym length is 0."));
                    }
                } catch (Exception e) {
                    Logger.ColoredConsoleWrite(ConsoleColor.DarkRed, "Ignore this: sending exception information to log file.");
                    Logger.AddLog(string.Format("Error in HandlePokeStop: {0}", e.ToString()));
                }
            }));
        }

        private void InfoObservable_HandlePokeStopInfoUpdate(POGOProtos.Map.Fort.FortData pokeStop, string info)
        {
            InfoObservable_HandlePokeStop(pokeStop,true,info);
        }

        private void initViewOnly(int team, int level, long exp)
        {
            //first hide all controls
            foreach (Control c in Controls) {
                c.Visible = false;
            }
            cbShowPokeStops.Visible = false;
            btnPauseWalking.Visible = true;
            cbShowPokemon.Visible = true;

            Bitmap bmp = Properties.MapData.player;
            switch (team) {
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
            _botStartMarker.Position = new PointLatLng(GlobalVars.latitude, GlobalVars.longitude);
            _botStartMarker.ToolTipText = string.Format("Start Point.\n{0}\n{1},{2}", LocationUtils.FindAddress(GlobalVars.latitude, GlobalVars.longitude), GlobalVars.latitude, GlobalVars.longitude);
            _botStartMarker.ToolTip.Font = new System.Drawing.Font("Arial", 12, System.Drawing.GraphicsUnit.Pixel);
            _botStartMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            routeOverlay.Markers.Add(_botStartMarker);
            _circle = CreateCircle(new PointLatLng(GlobalVars.latitude, GlobalVars.longitude), GlobalVars.radius, 100);
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
            buttonRefreshForts.Visible = true;
            cbShowPokeStops.Visible = true;
            cbShowPokemon.Visible = true;
            _pokemonOverlay.IsVisibile = true;
            _pokeStopsOverlay.IsVisibile = true;
            _pokeGymsOverlay.IsVisibile = true;

            //don't ask at closing
            close = false;
            //add & remove live data handler after form loaded
            GlobalVars.infoObservable.HandleNewGeoLocations += handleLiveGeoLocations;
            GlobalVars.infoObservable.HandleAvailablePokeStop += InfoObservable_HandlePokeStop;
            GlobalVars.infoObservable.HandleAvailablePokeGym += InfoObservable_HandlePokeGym;
            GlobalVars.infoObservable.HandlePokeStopInfoUpdate += InfoObservable_HandlePokeStopInfoUpdate;
            GlobalVars.infoObservable.HandleClearPokemon += infoObservable_HandleClearPokemon;
            GlobalVars.infoObservable.HandleNewPokemonLocation += infoObservable_HandleNewPokemonLocation;
            GlobalVars.infoObservable.HandleNewPokemonLocations += infoObservable_HandleNewPokemonLocations;
            GlobalVars.infoObservable.HandleDeletePokemonLocation += infoObservable_HandleDeletePokemonLocation;
            GlobalVars.infoObservable.HandleUpdatePokeGym += InfoObservable_HandleUpdatePokeGym;
        }


        private void map_Load(object sender, EventArgs e)
        {
            GlobalVars.MapLoaded = true;
            showMap();
        }

        private void map_OnMapDrag()
        {
            textBox1.Text = map.Position.Lat.ToString(CultureInfo.InvariantCulture);
            textBox2.Text = map.Position.Lng.ToString(CultureInfo.InvariantCulture);
            tbAddress.Text = LocationUtils.FindAddress(map.Position);

            if (radiusOverlay != null) {
                radiusOverlay.Polygons.Clear();
                radiusOverlay.Polygons.Add(CreateCircle(new PointLatLng(map.Position.Lat, map.Position.Lng), (int)nudRadius.Value, 100));
            }
        }

        private void map_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (GlobalVars.pauseAtPokeStop) {
                    GlobalVars.RouteToRepeat.AddLast(new GeoCoordinate(item.Position.Lat, item.Position.Lng));
                    item.ToolTipText = string.Format("Stop {0} Queued", GlobalVars.RouteToRepeat.Count);
                } else {
                    GlobalVars.NextDestinationOverride.AddFirst(new GeoCoordinate(item.Position.Lat, item.Position.Lng));
                    if (!item.ToolTipText.Contains("\nNext Destination Marked")) {
                        item.ToolTipText += "\nNext Destination Marked";
                    }
                }
            if (e.Button == MouseButtons.Right){
                var gymID= getGymID(item);
                if (gymID=="")
                    return;
                
                if (MessageBox.Show(th.TS("Do you want see gym details?"), th.TS("Gym Details Confirmation"), MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                
                var details = Logic.Logic.objClient.Fort.GymGetInfo(gymID, item.Position.Lat, item.Position.Lng);
                if (details.Result == POGOProtos.Networking.Responses.GymGetInfoResponse.Types.Result.Success){
                    var str = item.ToolTipText + "\n";
                    str += details.Name + "\n";
                    str +=  details.GymStatusAndDefenders.GymDefender.Count+" Defenders: \n";
                    str += Logic.Functions.GymsLogic.strPokemons(details.GymStatusAndDefenders.GymDefender) +"\n\n";
                    str += th.TS("Do you want copy location?");
                    if (MessageBox.Show(str, th.TS("Gym Details"), MessageBoxButtons.YesNo  ) == DialogResult.Yes){
                        Clipboard.SetText(item.Position.Lat.ToString(CultureInfo.InvariantCulture)  +","+ item.Position.Lng.ToString(CultureInfo.InvariantCulture));
                    }
                }

            }
        }
        
        private string getGymID( GMapMarker item){
            foreach (var element in _pokeGymsMarks) {
                if( element.Value == item)
                    return element.Key;
            }
            return "";
        }
        


        private void showMap()
        {
            try {
                map.DragButton = MouseButtons.Left;
                map.MapProvider = GMapProviders.GoogleMap;
                map.Position = new PointLatLng(GlobalVars.latitude, GlobalVars.longitude);
                map.MinZoom = 0;
                map.MaxZoom = 20;
                map.Zoom = 16;

                textBox1.Text = GlobalVars.latitude.ToString(CultureInfo.InvariantCulture);
                textBox2.Text = GlobalVars.longitude.ToString(CultureInfo.InvariantCulture);
                textBox3.Text = GlobalVars.altitude.ToString(CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0 && textBox1.Text != "-") {
                try {
                    double lat = StrCordToDouble(textBox1.Text);
                    if (lat > 90.0 || lat < -90.0) {
                        throw new System.ArgumentException("Latitude value has to be between 90 and -90!");
                    }
                    map.Position = new GMap.NET.PointLatLng(lat, map.Position.Lng);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    textBox1.Text = "";
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0 && textBox2.Text != "-") {
                try {
                    double lng = StrCordToDouble(textBox2.Text);
                    if (lng > 180.0 || lng < -180.0) {
                        throw new System.ArgumentException("Longitude value has to be between 180 and -180!");
                    }
                    map.Position = new GMap.NET.PointLatLng(map.Position.Lat, lng);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    textBox2.Text = "";
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length > 0 && textBox3.Text != "-") {
                try {
                    double alt = StrCordToDouble(textBox3.Text);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    textBox1.Text = "";
                }
            }
        }

        void BtnGetPointsClick(object sender, EventArgs e)
        {
            var ret = LocationUtils.FindLocation(tbAddress.Text);
            textBox1.Text = ret[0].ToString();
            textBox2.Text = ret[1].ToString();
            map.Position = new PointLatLng(ret[0], ret[1]);
        }

        public void CreateBotMarker(int team, int level, long exp)
        {
            Bitmap bmp = Properties.MapData.player;
            switch (team) {
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
            routeOverlay.IsVisibile = false;
            PointLatLng pointLatLng;
            if (_botMarker != null) {
                pointLatLng = _botMarker.Position;
                routeOverlay.Markers.Remove(_botMarker);
            } else {
                pointLatLng = new PointLatLng();
            }
            _botMarker = new GMarkerGoogle(pointLatLng, bmp);
            _botMarker.ToolTipText = string.Format("Level: {0} ({1})", level, exp);
            _botMarker.ToolTip.Font = new Font("Arial", 12, GraphicsUnit.Pixel);
            _botMarker.ToolTipMode = MarkerTooltipMode.OnMouseOver;
            routeOverlay.Markers.Add(_botMarker);
            routeOverlay.IsVisibile = true;
        }

        void nudRadius_ValueChanged(object sender, EventArgs e)
        {
            radiusOverlay.Polygons.Clear();
            radiusOverlay.Polygons.Add(CreateCircle(new PointLatLng(map.Position.Lat, map.Position.Lng), (int)nudRadius.Value, 100));
        }

        void btnPauseWalking_Click(object sender, EventArgs e)
        {
            if (btnPauseWalking.Text.Equals(th.TS("Pause Walking"))) {
                GlobalVars.pauseAtPokeStop = true;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Pausing at next Pokestop. (will continue catching pokemon and farming pokestop when available)");
                if (GlobalVars.RouteToRepeat.Count > 0) {
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Cleared!");
                    GlobalVars.RouteToRepeat.Clear();
                }

                btnPauseWalking.Text = th.TS("Resume Walking");
            } else {
                GlobalVars.pauseAtPokeStop = false;
                Logger.ColoredConsoleWrite(ConsoleColor.Magenta, "Resume walking between Pokestops.");
                if (GlobalVars.RouteToRepeat.Count > 0) {
                    foreach (var geocoord in GlobalVars.RouteToRepeat) {
                        GlobalVars.NextDestinationOverride.AddLast(geocoord);
                    }
                    Logger.ColoredConsoleWrite(ConsoleColor.Yellow, "User Defined Route Captured! Beginning Route Momentarily.");
                }
                btnPauseWalking.Text = th.TS("Pause Walking");
            }
          
        }

        public static double StrCordToDouble(string str)
        {
            double ret = 0;
            double.TryParse(str.Replace(",", "."), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out ret);
            return ret;
        }
        void cbShowPokemon_CheckStateChanged(object sender, EventArgs e)
        {
            GlobalVars.ShowPokemons = (sender as CheckBox).Checked;
        }
        void buttonZoomIn_Click(object sender, EventArgs e)
        {
            map.Zoom--;
        }
        void buttonZoomOut_Click(object sender, EventArgs e)
        {
            map.Zoom++;
        }
        void buttonSavePokestops_Click(object sender, EventArgs e)
        {
            SavePokestopsInfo();
        }
        void buttonLoadPokestops_Click(object sender, EventArgs e)
        {
            LoadPokestopsInfo();
        }
    }
}
