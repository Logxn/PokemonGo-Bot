using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Threading.Tasks;
using POGOProtos.Map.Fort;
using POGOProtos.Networking.Responses;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Helpers;
using PokeMaster.Logic.Utils;
using POGOProtos.Enums;
using PokeMaster.Logic.Shared;
using System.Globalization;
using System.IO;

namespace PokeMaster.Logic
{
    public class Navigation
    {
        private static readonly Random RandomDevice = new Random();

        public Navigation(Client client, ISettings settings)
        {
            _client = client;
            _botSettings = settings;
        }

        private const double SpeedDownTo = 10 / 3.6;
        private readonly Client _client;
        public readonly ISettings _botSettings;

        public void SetCoordinates(double lat, double lng, double altitude)
        {
            _client.CurrentLatitude = lat;
            _client.CurrentLongitude = lng;
            _client.CurrentAltitude = altitude;
            SaveLatLngAlt(lat, lng, altitude);
        }

        public static void SaveLatLngAlt(double lat, double lng, double alt)
        {
            try{
                string latlngalt = lat.ToString(CultureInfo.InvariantCulture) + ":" + lng.ToString(CultureInfo.InvariantCulture) + ":" + alt.ToString(CultureInfo.InvariantCulture);
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\LastCoords.txt", latlngalt);
            }catch (Exception ex){
                Logger.ExceptionInfo(ex.ToString());
            }
        }

        public static double DistanceBetween2Coordinates(double Lat1, double Lng1, double Lat2, double Lng2)
        {
            const double r_earth = 6378137;
            double d_lat = (Lat2 - Lat1) * Math.PI / 180;
            double d_lon = (Lng2 - Lng1) * Math.PI / 180;
            double alpha = Math.Sin(d_lat / 2) * Math.Sin(d_lat / 2) +
                           Math.Cos(Lat1 * Math.PI / 180) * Math.Cos(Lat2 * Math.PI / 180) *
                           Math.Sin(d_lon / 2) * Math.Sin(d_lon / 2);
            double d = 2 * r_earth * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            return d;
        }

        public void HumanLikeWalking(GeoCoordinate targetLocation, double walkingSpeedInKilometersPerHour, Func<bool> functionExecutedWhileWalking, bool fromgoogle = false, bool log = true)
        {
            
            /* removing random factors*/
            /*const float randomFactor = 0.5f;
            var randomMin = (int)(walkingSpeedInKilometersPerHour * (1 - randomFactor));
            var randomMax = (int)(walkingSpeedInKilometersPerHour * (1 + randomFactor));
            walkingSpeedInKilometersPerHour = RandomDevice.Next(randomMin, randomMax);*/
            
            var speedInMetersPerSecond = walkingSpeedInKilometersPerHour / 3.6;
            Logger.Debug("speed In Meters Per Seconds to use: " + speedInMetersPerSecond);
            var sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
            var distanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);
            Logger.Debug($"Distance to target location: {distanceToTarget:0.##} meters. Will take {distanceToTarget / speedInMetersPerSecond:0.##} seconds!");
            var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
            var nextWaypointDistance = speedInMetersPerSecond;
            var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

            //Initial walking
            var requestSendDateTime = DateTime.Now;
            LocationUtils.updatePlayerLocation(_client, waypoint.Latitude, waypoint.Longitude, waypoint.Altitude);

            if (functionExecutedWhileWalking != null)
                functionExecutedWhileWalking();

            do
            {
                //update user location on map
                Task.Factory.StartNew(() => Logic.Instance.infoObservable.PushNewGeoLocations(new GeoCoordinate(waypoint.Latitude, waypoint.Longitude)));

                var millisecondsUntilGetUpdatePlayerLocationResponse =
                    (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                sourceLocation = new GeoCoordinate(_client.CurrentLatitude, _client.CurrentLongitude);
                var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);

                if (currentDistanceToTarget < 40)
                {
                    if (speedInMetersPerSecond > SpeedDownTo)
                    {
                        Logger.Warning( "We are within 40 meters of the target. Slowing down to ~10 km/h to not pass the target.");
                        speedInMetersPerSecond = SpeedDownTo;
                    }
                }

                nextWaypointDistance = Math.Min(currentDistanceToTarget, millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
                waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);
                requestSendDateTime = DateTime.Now;
                
                SetCoordinates(waypoint.Latitude, waypoint.Longitude, waypoint.Altitude);

                if (functionExecutedWhileWalking != null && !_botSettings.PauseTheWalking)
                     functionExecutedWhileWalking();// look for pokemon 

                if (GlobalVars.SnipeOpts.Enabled){
                    Logic.Instance.sniperLogic.Execute((PokemonId) GlobalVars.SnipeOpts.ID,GlobalVars.SnipeOpts.Location);
                }

                RandomHelper.RandomSleep(500, 600);
            }
            while ((LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 30 && !fromgoogle) || LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 2);
        }

        public static FortData[] pathByNearestNeighbour(FortData[] pokeStops)
        {
            //Normal calculation
            for (var i = 1; i < pokeStops.Length - 1; i++)
            {
                var closest = i + 1;
                var cloestDist = LocationUtils.CalculateDistanceInMeters(pokeStops[i].Latitude, pokeStops[i].Longitude, pokeStops[closest].Latitude, pokeStops[closest].Longitude);
                for (var j = closest; j < pokeStops.Length; j++)
                {
                    var initialDist = cloestDist;
                    var newDist = LocationUtils.CalculateDistanceInMeters(pokeStops[i].Latitude, pokeStops[i].Longitude, pokeStops[j].Latitude, pokeStops[j].Longitude);
                    if (initialDist > newDist)
                    {
                        cloestDist = newDist;
                        closest = j;
                    }
                }
                var tmpPok = pokeStops[closest];
                pokeStops[closest] = pokeStops[i + 1];
                pokeStops[i + 1] = tmpPok;
            }
            return pokeStops;
        }

        private List<int> calcCrossing(List<int> _chromosome1, List<int> _chromosome2)
        {
            var child = new List<int>(_chromosome1);

            if (child.Count <= 3)
            {
                return child;
            }

            int p = RandomDevice.Next(1, child.Count - 2);
            for (; p < child.Count - 1; ++p)
            {
                for (int i = 0; i < _chromosome2.Count; ++i)
                {
                    var tempIndex = child.FindIndex(x => x == _chromosome2[i]);
                    if (tempIndex > p || tempIndex < 0)
                    {
                        child[p] = _chromosome2[i];
                        break;
                    }
                }
            }

            return child;
        }

        private int calcFitness(ref FortData[] pokeStops, List<int> _chromosome, double walkingSpeedInKilometersPerHour)
        {
            if (_chromosome.Count <= 2)
            {
                return 0;
            }

            double time = 0.0;
            double length = 0.0;
            for (int i = 0; i < _chromosome.Count - 1; ++i)
            {
                double distance = DistanceBetween2Coordinates(pokeStops[_chromosome[i]].Latitude, pokeStops[_chromosome[i]].Longitude, pokeStops[_chromosome[i + 1]].Latitude, pokeStops[_chromosome[i + 1]].Longitude);
                if (distance <= 40)
                {
                    time += distance / Logic.SpeedDownTo;
                }
                else
                {
                    time += distance * 3.6 / walkingSpeedInKilometersPerHour;
                }
                length += distance;
            }

            if (time <= 380 || !(time > 0.0))
            {
                return 0;
            }

            return _botSettings.navigation_option == 1 ? Convert.ToInt32((_chromosome.Count * 10000) / time) : Convert.ToInt32(_chromosome.Count * length / time);
        }

        private double calcTime(ref FortData[] pokeStops, List<int> _chromosome, double walkingSpeedInKilometersPerHour)
        {
            double time = 0.0;
            for (int i = 0; i < _chromosome.Count - 1; ++i)
            {
                double distance = DistanceBetween2Coordinates(pokeStops[_chromosome[i]].Latitude, pokeStops[_chromosome[i]].Longitude, pokeStops[_chromosome[i + 1]].Latitude, pokeStops[_chromosome[i + 1]].Longitude);
                if (distance <= 40)
                {
                    time += distance / Logic.SpeedDownTo;
                }
                else
                {
                    time += distance * 3.6 / walkingSpeedInKilometersPerHour;
                }
            }
            return time;
        }

        private void mutate(ref List<int> _chromosome)
        {
            int i1 = RandomDevice.Next(1, _chromosome.Count - 2), i2 = RandomDevice.Next(1, _chromosome.Count - 2);
            int temp = _chromosome[i1];
            _chromosome[i1] = _chromosome[i2];
            _chromosome[i2] = temp;
        }

        private List<List<int>> selection(FortData[] pokeStops, List<List<int>> population, double walkingSpeedInKilometersPerHour)
        {
            var listSelection = new List<List<int>>();
            int sumPop = 0;
            var fittnes = new List<int>();

            for (int c = 0; c < population.Count; ++c)
            {
                var temp = calcFitness(ref pokeStops, population[c], walkingSpeedInKilometersPerHour);
                sumPop += temp;
                fittnes.Add(temp);
            }
            var fittnesSorted = new List<int>(fittnes);
            fittnesSorted.Sort();

            if (sumPop < 2)
            {
                return listSelection;
            }

            
            int selcetedChr = -1;
            do
            {
                var tempIndex = RandomDevice.Next(0, sumPop);
                int tempSumPop = 0;
                for (int c = fittnesSorted.Count - 1; c > 0; --c)
                {
                    tempSumPop += fittnesSorted[c];
                    if (tempSumPop > tempIndex)
                    {
                        var tempSelcetedChr = fittnes.FindIndex(x => x == fittnesSorted[c]);
                        if (tempSelcetedChr != selcetedChr && tempSelcetedChr >= 0)
                        {
                            selcetedChr = tempSelcetedChr;
                            listSelection.Add(population[selcetedChr]);
                            break;
                        }
                    }
                }
            }
            while (listSelection.Count < 2);

            return listSelection;
        }
    }
}