using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.MapProviders;
using GoogleMapsApi;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.Elevation.Request;
using GoogleMapsApi.Entities.Elevation.Response;
using Newtonsoft.Json.Linq;
using System;
using System.Device.Location;
using System.IO;
using System.Net;
using System.Globalization;
using PokeMaster.Logic.Shared;
using PokemonGo.RocketAPI;

namespace PokeMaster.Logic.Utils
{
    public static class LocationUtils
    {
        public static double GetAltitude(double lat, double lon)
        {
            var altitude = GetRandom(11.0d, 8.6d);
            try {
                if (GlobalVars.BlockAltitude)
                    return GlobalVars.altitude;
                if (GlobalVars.GoogleMapsAPIKey != "")
                    return GetAltitudeWithKEY(lat, lon, GlobalVars.GoogleMapsAPIKey);
                var point = new GeoCoordinate(lat, lon);
                var tries = 3;
                var status = "";
                JObject json = null;
                while (tries > 0 && status.ToLower() != "ok") {
                    
                    var request = (HttpWebRequest)WebRequest.Create(
                                      string.Format("https://maps.googleapis.com/maps/api/elevation/json?locations={0},{1}"
                                      , point.Latitude.ToString(CultureInfo.InvariantCulture)
                                      , point.Longitude.ToString(CultureInfo.InvariantCulture)));
                    var response = (HttpWebResponse)request.GetResponse();
                    var sr = new StreamReader(response.GetResponseStream() ?? new MemoryStream()).ReadToEnd();
                    
                    json = JObject.Parse(sr);
                    status = (string)json.SelectToken("status");
                    if (status.ToLower() != "ok")
                        Task.Delay(1000).Wait();
                    tries--;
                }

                if (json.SelectToken("results[0].elevation") != null) {
                    altitude = (double)json.SelectToken("results[0].elevation");
                } 
            } catch (Exception e) {
                Logger.ExceptionInfo(e.ToString());
            }
            return altitude;
        }
        private static double GetRandom(double maximum, double minimum)
        {
            return new Random().NextDouble() * (maximum - minimum) + minimum;
        }

        public static double CalculateDistanceInMeters(double sourceLat, double sourceLng, double destLat, double destLng)
            // from http://stackoverflow.com/questions/6366408/calculating-distance-between-two-latitude-and-longitude-geocoordinates
        {
            var sourceLocation = new GeoCoordinate(sourceLat, sourceLng);
            var targetLocation = new GeoCoordinate(destLat, destLng);

            return sourceLocation.GetDistanceTo(targetLocation);
        }

        public static double CalculateDistanceInMeters(GeoCoordinate sourceLocation, GeoCoordinate destinationLocation)
        {
            return CalculateDistanceInMeters(sourceLocation.Latitude, sourceLocation.Longitude, destinationLocation.Latitude, destinationLocation.Longitude);
        }

        public static GeoCoordinate CreateWaypoint(GeoCoordinate sourceLocation, double distanceInMeters, double bearingDegrees)
            //from http://stackoverflow.com/a/17545955
        {
            var distanceKm = distanceInMeters / 1000.0;
            var distanceRadians = distanceKm / 6371; //6371 = Earth's radius in km

            var bearingRadians = ToRad(bearingDegrees);
            var sourceLatitudeRadians = ToRad(sourceLocation.Latitude);
            var sourceLongitudeRadians = ToRad(sourceLocation.Longitude);

            var targetLatitudeRadians = Math.Asin(Math.Sin(sourceLatitudeRadians) * Math.Cos(distanceRadians)
                                                  +
                                                  Math.Cos(sourceLatitudeRadians) * Math.Sin(distanceRadians) *
                                                  Math.Cos(bearingRadians));

            var targetLongitudeRadians = sourceLongitudeRadians + Math.Atan2(Math.Sin(bearingRadians)
                                                                             * Math.Sin(distanceRadians) *
                                                                             Math.Cos(sourceLatitudeRadians),
                                                                             Math.Cos(distanceRadians)
                                                                             - Math.Sin(sourceLatitudeRadians) * Math.Sin(targetLatitudeRadians));

            // adjust toLonRadians to be in the range -180 to +180...
            targetLongitudeRadians = (targetLongitudeRadians + 3 * Math.PI) % (2 * Math.PI) - Math.PI;

            return new GeoCoordinate(ToDegrees(targetLatitudeRadians), ToDegrees(targetLongitudeRadians), GetAltitude((ToDegrees(targetLatitudeRadians)), ToDegrees(targetLongitudeRadians)));
        }

        public static double DegreeBearing(GeoCoordinate sourceLocation, GeoCoordinate targetLocation)
            // from http://stackoverflow.com/questions/2042599/direction-between-2-latitude-longitude-points-in-c-sharp
        {
            var dLon = ToRad(targetLocation.Longitude - sourceLocation.Longitude);
            var dPhi = Math.Log(
                Math.Tan(ToRad(targetLocation.Latitude) / 2 + Math.PI / 4) /
                Math.Tan(ToRad(sourceLocation.Latitude) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : 2 * Math.PI + dLon;
            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        public static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (ToDegrees(radians) + 360) % 360;
        }

        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
        public static string FindAddress(double lat, double lng, bool fullinfo = false)
        {
            return FindAddress(new PointLatLng(lat, lng), fullinfo);
        }
        public static string FindAddress(PointLatLng pnt, bool fullinfo = false)
        {
            string ret = "";
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPlacemark(pnt, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null) {
                ret = pos.Value.Address;
                if (fullinfo)
                    ret = pos.Value.ToString();
            }
            return ret;
        }
        public static double[] FindLocation(string address)
        {
            double[] ret = { 0.0, 0.0 };
            GeoCoderStatusCode status;
            var pos = GMapProviders.GoogleMap.GetPoint(address, out status);
            if (status == GeoCoderStatusCode.G_GEO_SUCCESS && pos != null) {
                ret = new double[2];
                ret[0] = pos.Value.Lat;
                ret[1] = pos.Value.Lng;
            }
            return ret;
        }
        
        public static void updatePlayerLocation(Client client, GeoCoordinate loc, bool updateFile = true)
        {
            updatePlayerLocation(client, loc.Latitude, loc.Longitude, loc.Altitude, updateFile);
        }
        public static void updatePlayerLocation(Client client, double latitude, double longitude, double altitude, bool updateFile = true)
        {
            client.Player.SetCoordinates(latitude, longitude, altitude);
            if (updateFile) {
                string latlngalt = latitude.ToString(CultureInfo.InvariantCulture) + ":" + longitude.ToString(CultureInfo.InvariantCulture) + ":" + altitude.ToString(CultureInfo.InvariantCulture);
                File.WriteAllText(Directory.GetCurrentDirectory() + "\\Configs\\LastCoords.txt", latlngalt);
            }
        }
        public static double GetAltitudeWithKEY(double lat, double lng, string key)
        {
            var elevationRequest = new ElevationRequest {
                Locations = new[] { new Location(lat, lng) },
            };
            elevationRequest.ApiKey = key;
            try {
                ElevationResponse elevation = GoogleMaps.Elevation.Query(elevationRequest);
                if (elevation.Status == Status.OK)
                    foreach (Result result in elevation.Results)
                        return  result.Elevation;
            } catch (Exception ex1) {
                Logger.ExceptionInfo(ex1.ToString());
            }
            return  GetRandom(11.0d, 8.6d);
        }
    }
}