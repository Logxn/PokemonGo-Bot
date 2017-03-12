using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Google.Maps;
using Google.Maps.Elevation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokeMaster.Logic.Utils;
using PokemonGo.RocketAPI;

namespace PokeMaster.Helper
{
    public class LocationHelper
    {
        public double GetElevation (double _latitude, double _longitude, double _altitude=0)
        {
            ElevationRequest elevationRequest = new ElevationRequest();
            elevationRequest.AddLocations(new LatLng(_latitude, _longitude));
            elevationRequest.Sensor = false;

            try
            {
                ElevationResponse elevation = new ElevationService().GetResponse(elevationRequest);
                if (elevation.Status == ServiceResponseStatus.Ok)
                {
                    return (double)elevation.Results[0].Elevation;
                }
                else return _altitude;
            }
            catch (Exception ex)
            {
                Logger.ColoredConsoleWrite(ConsoleColor.Red, "Error: LocationHelper.cs - GetElevation");
                Logger.ColoredConsoleWrite(ConsoleColor.Red, ex.Message);
                return _altitude;
            }
        }
    }
}
