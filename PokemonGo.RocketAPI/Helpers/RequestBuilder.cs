﻿namespace PokemonGo.RocketAPI.Helpers
{
    using System.Linq;

    using PokemonGo.RocketAPI.Enums;
    using PokemonGo.RocketAPI.GeneratedCode;

    public static class RequestBuilder
    {
        public static Request GetInitialRequest(string authToken, AuthType authType, double lat, double lng, double altitude, params Request.Types.Requests[] customRequests)
        {
            return new Request
                   {
                       Altitude = Utils.FloatAsUlong(altitude),
                       Auth = new Request.Types.AuthInfo
                              {
                                  Provider = authType == AuthType.Google ? "google" : "ptc",
                                  Token = new Request.Types.AuthInfo.Types.JWT
                                          {
                                              Contents = authToken,
                                              Unknown13 = 14
                                          }
                              },
                       Latitude = Utils.FloatAsUlong(lat),
                       Longitude = Utils.FloatAsUlong(lng),
                       RpcId = 1469378659230941192,
                       Unknown1 = 2,
                       Unknown12 = 989, // Required otherwise we receive incompatible protocol
                       Requests =
                       {
                           customRequests
                       }
                   };
        }

        public static Request GetInitialRequest(string authToken, AuthType authType, double lat, double lng, double altitude, params RequestType[] customRequestTypes)
        {
            var customRequests = customRequestTypes.ToList().Select(c => new Request.Types.Requests
                                                                         {
                                                                             Type = (int) c
                                                                         });
            return GetInitialRequest(authToken, authType, lat, lng, altitude, customRequests.ToArray());
        }

        public static Request GetRequest(Request.Types.UnknownAuth unknownAuth, double lat, double lng, double altitude, params Request.Types.Requests[] customRequests)
        {
            return new Request
                   {
                       Altitude = Utils.FloatAsUlong(altitude),
                       Unknownauth = unknownAuth,
                       Latitude = Utils.FloatAsUlong(lat),
                       Longitude = Utils.FloatAsUlong(lng),
                       RpcId = 1469378659230941192,
                       Unknown1 = 2,
                       Unknown12 = 989, // Required otherwise we receive incompatible protocol
                       Requests =
                       {
                           customRequests
                       }
                   };
        }

        public static Request GetRequest(Request.Types.UnknownAuth unknownAuth, double lat, double lng, double altitude, params RequestType[] customRequestTypes)
        {
            var customRequests = customRequestTypes.ToList().Select(c => new Request.Types.Requests
                                                                         {
                                                                             Type = (int) c
                                                                         });
            return GetRequest(unknownAuth, lat, lng, altitude, customRequests.ToArray());
        }
    }
}