using Google.Protobuf;
using POGOProtos.Enums;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Platform;
using POGOProtos.Networking.Platform.Requests;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Hash;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random;

namespace PokemonGo.RocketAPI.Helpers
{
    public class RequestBuilder
    {
        private readonly string _authToken;
        private readonly AuthType _authType;
        private readonly double _latitude;
        private readonly double _longitude;
        private readonly double _altitude;
        private readonly AuthTicket _authTicket;
        private readonly Client _client;
        private readonly float _speed;
        private ByteString _sessionHash;
        public Signature.Types.DeviceInfo _DeviceInfo;
        private static readonly TRandom TRandomDevice = new TRandom();
        private int _token2 = TRandomDevice.Next(1, 59);
        public byte[] sessionhash_array = null;
        public bool setupdevicedone = false;

        private RandomRequestID _requestBuilderID = new RandomRequestID();
        
        public RequestBuilder(Client client, string authToken, AuthType authType, double latitude, double longitude, double altitude, AuthTicket authTicket = null)
        {
            if (!setupdevicedone) {
                _DeviceInfo = DeviceSetup.SelectedDevice.DeviceInfo;
                setupdevicedone = true;
            }

            _client = client;
            _authToken = authToken;
            _authType = authType;
            _latitude = latitude;
            _longitude = longitude;
            _altitude = altitude;
            _authTicket = authTicket;

            // Add small variance to speed.
            _speed = _speed + ((float)Math.Round(GenRandom(-1, 1), 7));

            if (_sessionHash == null) {
                _sessionHash = GenerateNewHash();
            }
        }
        
        public static ByteString GenerateNewHash()
        {
            var hashBytes = new byte[16];

            TRandomDevice.NextBytes(hashBytes);

            return ByteString.CopyFrom(hashBytes);
        }



        //private RequestEnvelope.Types.PlatformRequest GenerateSignature(IEnumerable<IMessage> requests)
        /// <summary>
        /// EB Check IMessage
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        /// Also pogolib does
        /// internal async Task<PlatformRequest> GenerateSignatureAsync(RequestEnvelope requestEnvelope)
        private RequestEnvelope.Types.PlatformRequest GenerateSignature(RequestEnvelope requestEnvelope)
        {
            var timestampSinceStart = (long)(Utils.GetTime(true) - _client.StartTime);
            var locationFixes = BuildLocationFixes(requestEnvelope, timestampSinceStart);

            if (requestEnvelope.Requests.Count > 0){
                requestEnvelope.Accuracy = locationFixes[0].Altitude;
                requestEnvelope.MsSinceLastLocationfix = (long)locationFixes[0].TimestampSnapshot;
            }
            var  at = new Signature.Types.ActivityStatus(){
                Stationary = true
            };

            
            if (_client.Platform == Platform.Ios)
                at.Tilting = (TRandomDevice.Next(1,2)==1);

            #region GenerateSignature
            var signature = new Signature {
                TimestampSinceStart = (ulong)timestampSinceStart,
                Timestamp = (ulong)Utils.GetTime(true), // true means in Ms

                SensorInfo = {
                    new Signature.Types.SensorInfo {
                        // Values are not the same used in PogoLib
                        TimestampSnapshot = (ulong)(timestampSinceStart + TRandomDevice.Next(100, 250)),
                        LinearAccelerationX = GenRandom(0.12271042913198471),
                        LinearAccelerationY = GenRandom(-0.015570580959320068),
                        LinearAccelerationZ = GenRandom(0.010850906372070313),
                        RotationRateX = GenRandom(-0.0120010357350111),
                        RotationRateY = GenRandom(-0.04214850440621376),
                        RotationRateZ = GenRandom(0.94571763277053833),
                        AttitudePitch = GenRandom(-47.149471283, 61.8397789001),
                        AttitudeYaw = GenRandom(-47.149471283, 61.8397789001),
                        AttitudeRoll = GenRandom(-47.149471283, 5),                        
                        GravityZ = GenRandom(9.8),
                        GravityX = GenRandom(0.02),
                        GravityY = GenRandom(0.3),
                        MagneticFieldX = GenRandom(-10,10),
                        MagneticFieldY = GenRandom(-10,10),
                        MagneticFieldZ = GenRandom(-40,-20),
                        MagneticFieldAccuracy = -1,
                        Status = 3
                    }
                },
                DeviceInfo = _DeviceInfo,// dInfo,
                LocationFix = { locationFixes },
                ActivityStatus = at
            };
            #endregion

            signature.SessionHash = _sessionHash;
            
            //signature.Unknown25 = Resources.Api.AndroidUnknown25;
            //if (_client.Platform == Platform.Ios)
            signature.Unknown25 = Resources.Api.IOSUnknown25;

            var serializedTicket = requestEnvelope.AuthTicket != null ? requestEnvelope.AuthTicket.ToByteArray() : requestEnvelope.AuthInfo.ToByteArray();

            /*var locationBytes = BitConverter.GetBytes(_latitude).Reverse()
                .Concat(BitConverter.GetBytes(_longitude).Reverse())
                .Concat(BitConverter.GetBytes(locationFixes[0].Altitude).Reverse()).ToArray();
            */

            var requestsBytes = requestEnvelope.Requests.Select(x => x.ToByteArray()).ToArray();

            var hashRequest = new HashRequestContent() {
                Timestamp = signature.Timestamp,
                Latitude64 = BitConverter.DoubleToInt64Bits(requestEnvelope.Latitude),
                Longitude64 = BitConverter.DoubleToInt64Bits(requestEnvelope.Longitude),
                Accuracy64 = BitConverter.DoubleToInt64Bits(requestEnvelope.Accuracy),

                AuthTicket = serializedTicket,
                SessionData = signature.SessionHash.ToByteArray(),
                Requests = new List<byte[]>(requestsBytes)                
            };

            HashResponseContent responseContent;

            responseContent = _client.Hasher.RequestHashes(hashRequest);

            signature.LocationHash1 = unchecked((int)responseContent.LocationAuthHash);
            signature.LocationHash2 = unchecked((int)responseContent.LocationHash);
            signature.RequestHash.AddRange(responseContent.RequestHashes.Select(x => (ulong)x).ToArray());

            var encryptedSignature = new RequestEnvelope.Types.PlatformRequest {
                Type = PlatformRequestType.SendEncryptedSignature,
                RequestMessage = new SendEncryptedSignatureRequest {
                    EncryptedSignature = ByteString.CopyFrom(_client.Crypter.Encrypt(signature.ToByteArray(), (uint)timestampSinceStart))
                }.ToByteString()
            };

            return encryptedSignature;
        }

        /// <summary>
        /// Generates a few random <see cref="LocationFix"/>es to act like a real GPS sensor.
        /// </summary>
        /// <param name="requestEnvelope">The <see cref="RequestEnvelope"/> these <see cref="LocationFix"/>es are used for.</param>
        /// <param name="timestampSinceStart">The milliseconds passed since starting the <see cref="Session"/> used by the current <see cref="RequestEnvelope"/>.</param>
        /// <returns></returns>
        private List<Signature.Types.LocationFix> BuildLocationFixes(RequestEnvelope requestEnvelope, long timestampSinceStart)
        {
            var locationFixes = new List<Signature.Types.LocationFix>();

            if (requestEnvelope.Requests.Count == 0 || requestEnvelope.Requests[0] == null)
                return locationFixes;

            var providerCount = TRandomDevice.Next(4, 10);

            for (var i = 0; i < providerCount; i++) {
                var timestampSnapshot = timestampSinceStart + (150 * (i + 1) + TRandomDevice.Next(250 * (i + 1) - 150 * (i + 1)));
                if (timestampSnapshot >= timestampSinceStart) {
                    if (locationFixes.Count != 0)
                        break;

                    timestampSnapshot = timestampSinceStart - TRandomDevice.Next(20, 50);

                    if (timestampSnapshot < 0)
                        timestampSnapshot = 0;
                }
                var tmpCourse = -1F;
                var mpSpeed = 0F;


                if (_client.Platform == Platform.Ios){
                    tmpCourse = (float)TRandomDevice.NextDouble(100,330);
                    mpSpeed = (float)TRandomDevice.NextDouble(0.9,2.2);
                }


                locationFixes.Insert(0, new Signature.Types.LocationFix {
                    TimestampSnapshot = (ulong)timestampSnapshot,
                    Latitude = (float)_client.CurrentLatitude,
                    Longitude = (float)_client.CurrentLongitude,
                    HorizontalAccuracy = (float)TRandomDevice.NextDouble(5.0, 25.0),
                    VerticalAccuracy = (float)TRandomDevice.NextDouble(5.0, 25.0),
                    Altitude = (float)_client.CurrentAltitude,
                    Provider = "fused",
                    ProviderStatus = 3,
                    LocationType = 1,
                    Speed = mpSpeed,
                    Course = tmpCourse,
                    // Floor = 0
                });
            }
            return locationFixes;
        }

        internal static class LocationUtil
        {
            public static float OffsetLatitudeLongitude(double lat, double ran)
            {
                const int round = 6378137;
                var dl = ran / (round * Math.Cos(Math.PI * lat / 180));

                return (float)(lat + dl * 180 / Math.PI);
            }
        }

        public RequestEnvelope GetPlatformRequestEnvelope( RequestEnvelope.Types.PlatformRequest platfReq, bool firstRequest = false)
        {
            var req  = new Request();
            req.RequestType = RequestType.Echo;

            var _requestEnvelope = new RequestEnvelope {
                StatusCode = 2, //1
                RequestId = _requestBuilderID.Next(), //3
                Requests = { req }, //4
                Latitude = _latitude, //7
                Longitude = _longitude, //8
                Accuracy = _altitude, //9
                AuthTicket = _authTicket, //11
                MsSinceLastLocationfix = (long)TRandomDevice.Triangular(300, 30000, 10000) //12

            };

            _requestEnvelope.PlatformRequests.Add( platfReq);

            if (_authTicket != null && !firstRequest) {
                _requestEnvelope.AuthTicket = _authTicket;
            } else {
                _requestEnvelope.AuthInfo = new RequestEnvelope.Types.AuthInfo {
                    Provider = _authType == AuthType.Google ? "google" : "ptc",
                    Token = new RequestEnvelope.Types.AuthInfo.Types.JWT {
                        Contents = _authToken,
                        Unknown2 = _token2
                    }
                };
            }
            //Logger.Debug("GetPlatformRequestEnvelope");
            //Logger.Debug("_requestEnvelope"+ _requestEnvelope);

            _requestEnvelope.PlatformRequests.Add(GenerateSignature(_requestEnvelope));

            return _requestEnvelope;
        }

        public RequestEnvelope GetRequestEnvelope(Request[] customRequests, bool firstRequest = false)
        {

            var _requestEnvelope = new RequestEnvelope {
                StatusCode = 2, //1
                RequestId = _requestBuilderID.Next(), //3
                Requests = { customRequests }, //4
                Latitude = _latitude, //7
                Longitude = _longitude, //8
                Accuracy = _altitude, //9
                AuthTicket = _authTicket, //11
                MsSinceLastLocationfix = (long)TRandomDevice.Triangular(300, 30000, 10000) //12

            };

            if (_authTicket != null && !firstRequest) {
                _requestEnvelope.AuthTicket = _authTicket;
            } else {
                _requestEnvelope.AuthInfo = new RequestEnvelope.Types.AuthInfo {
                    Provider = _authType == AuthType.Google ? "google" : "ptc",
                    Token = new RequestEnvelope.Types.AuthInfo.Types.JWT {
                        Contents = _authToken,
                        Unknown2 = _token2
                    }
                };
            }

            //Logger.Debug("GetRequestEnvelope");
            //Logger.Debug("_requestEnvelope"+ _requestEnvelope);
            _requestEnvelope.PlatformRequests.Add(GenerateSignature(_requestEnvelope));

            if (customRequests.Length > 0  &&
                (customRequests[0].RequestType == RequestType.GetPlayer ||
                 customRequests[0].RequestType == RequestType.GetMapObjects)
                && ! string.IsNullOrEmpty(Resources.Api.UnknownPtr8Message)
               )
            {
                var plat8Message = new UnknownPtr8Request() {
                    Message = Resources.Api.UnknownPtr8Message
                };
    
                _requestEnvelope.PlatformRequests.Add(new RequestEnvelope.Types.PlatformRequest() {
                    Type = PlatformRequestType.UnknownPtr8,
                    RequestMessage = plat8Message.ToByteString()
                });

                /*_requestEnvelope.Requests.Add(new Request
                {
                   RequestType = RequestType.GetInbox,
                   RequestMessage = new GetInboxMessage { }.ToByteString()
                });*/
                
            }

            return _requestEnvelope;
        }

        public RequestEnvelope GetRequestEnvelope(RequestType type, IMessage message)
        {
            return  GetRequestEnvelope(new Request[] { new Request {
                    RequestType = type,
                    RequestMessage = message.ToByteString()
                }
            });

        }

        public static double GenRandom(double num)
        {
            const float randomFactor = 0.3f;
            var randomMin = (num * (1 - randomFactor));
            var randomMax = (num * (1 + randomFactor));
            var randomizedDelay = TRandomDevice.NextDouble() * (randomMax - randomMin) + randomMin;
            return randomizedDelay;
        }

        public static double GenRandom(double min, double max)
        {
            return TRandomDevice.NextDouble() * (max - min) + min;
        }
    }
}
