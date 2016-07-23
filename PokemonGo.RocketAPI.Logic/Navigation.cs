using PokemonGo.RocketAPI.GeneratedCode;
using PokemonGo.RocketAPI.Logic.Utils;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGo.RocketAPI.Logic
{
    public  class Navigation
    {
        public static double DistanceBetween2Coordinates(double Lat1, double Lng1, double Lat2, double Lng2)
        {
            double r_earth = 6378137;
            double d_lat = (Lat2 - Lat1) * Math.PI / 180;
            double d_lon = (Lng2 - Lng1) * Math.PI / 180;
            double alpha = Math.Sin(d_lat / 2) * Math.Sin(d_lat / 2)
                + Math.Cos(Lat1 * Math.PI / 180) * Math.Cos(Lat2 * Math.PI / 180)
                * Math.Sin(d_lon / 2) * Math.Sin(d_lon / 2);
            double d = 2 * r_earth * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            return d;
        }
        private const double SpeedDownTo = 10 / 3.6;
        private readonly Client _client;

        public Navigation(Client client)
        {
            _client = client;
        }

        public async Task<PlayerUpdateResponse> HumanLikeWalking(GeoCoordinate targetLocation,
            double walkingSpeedInKilometersPerHour, Func<Task> functionExecutedWhileWalking)
        {
            var speedInMetersPerSecond = walkingSpeedInKilometersPerHour / 3.6;

            var sourceLocation = new GeoCoordinate(_client.CurrentLat, _client.CurrentLng);
            var distanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);
            Logger.ColoredConsoleWrite(ConsoleColor.DarkCyan, $"Distance to target location: {distanceToTarget:0.##} meters. Will take {distanceToTarget / speedInMetersPerSecond:0.##} seconds!");

            var nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
            var nextWaypointDistance = speedInMetersPerSecond;
            var waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

            //Initial walking
            var requestSendDateTime = DateTime.Now;
            var result =
                await
                    _client.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude, _client.getSettingHandle().DefaultAltitude);

            if (functionExecutedWhileWalking != null)
                await functionExecutedWhileWalking();
            do
            {
                var millisecondsUntilGetUpdatePlayerLocationResponse =
                    (DateTime.Now - requestSendDateTime).TotalMilliseconds;

                sourceLocation = new GeoCoordinate(_client.CurrentLat, _client.CurrentLng);
                var currentDistanceToTarget = LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation);

                if (currentDistanceToTarget < 40)
                {
                    if (speedInMetersPerSecond > SpeedDownTo)
                    {
                        Logger.ColoredConsoleWrite(ConsoleColor.DarkCyan, $"We are within 40 meters of the target. Speeding down to 10 km/h to not pass the target.");
                        speedInMetersPerSecond = SpeedDownTo;
                    }
                }

                nextWaypointDistance = Math.Min(currentDistanceToTarget,
                    millisecondsUntilGetUpdatePlayerLocationResponse / 1000 * speedInMetersPerSecond);
                nextWaypointBearing = LocationUtils.DegreeBearing(sourceLocation, targetLocation);
                waypoint = LocationUtils.CreateWaypoint(sourceLocation, nextWaypointDistance, nextWaypointBearing);

                requestSendDateTime = DateTime.Now;
                result =
                    await
                        _client.UpdatePlayerLocation(waypoint.Latitude, waypoint.Longitude,
                            _client.getSettingHandle().DefaultAltitude);
                await Task.Delay(Math.Min((int)(distanceToTarget / speedInMetersPerSecond * 1000), 3000));
            } while (LocationUtils.CalculateDistanceInMeters(sourceLocation, targetLocation) >= 30);

            return result;
        }

        public static FortData[] pathByNearestNeighbour(FortData[] pokeStops)
        {
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
    }

}