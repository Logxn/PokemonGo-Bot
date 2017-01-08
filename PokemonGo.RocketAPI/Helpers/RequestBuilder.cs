using Google.Protobuf;
using POGOProtos.Enums;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Platform;
using POGOProtos.Networking.Platform.Requests;
using POGOProtos.Networking.Requests;
using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.Hash;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Diagnostics; 
using Troschuetz.Random;
using static POGOProtos.Networking.Envelopes.RequestEnvelope.Types;
using static POGOProtos.Networking.Envelopes.Signature.Types;

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
        private readonly ISettings _settings;
        private static long Client_5100_Unknown25 = -8832040574896607694; // We should move that constants somewhere else

        #region Device Shit
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        public static string deviceinfo = Path.Combine(path, "DeviceInfo.txt");
        public string DeviceId;
        public string AndroidBoardName;
        public string AndroidBootloader;
        public string DeviceBrand;
        public string DeviceModel;
        public string DeviceModelIdentifier;
        public string DeviceModelBoot;
        public string HardwareManufacturer;
        public string HardwareModel;
        public string FirmwareBrand;
        public string FirmwareTags;
        public string FirmwareType;
        public string FirmwareFingerprint;

        private int _token2 = RandomDevice.Next(1, 59);
        public bool setupdevicedone = false;

        public void setUpDevice()
        {
            string[] arrLine = File.ReadAllLines(deviceinfo);

            string DevicePackageName = arrLine[0].ToString();
            // Read DeviceID of File
            if (arrLine[1].ToString() != " ")
            {
                DeviceId = arrLine[1].ToString();
            }
            else
            {
                DeviceId = RandomString(16, "0123456789abcdef");
                // Save to file
                string[] b = { DevicePackageName, DeviceId };

                File.WriteAllLines(deviceinfo, b);
            }

            // Setuprest
            AndroidBoardName = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["AndroidBoardName"];
            AndroidBootloader = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["AndroidBootloader"];
            DeviceBrand = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceBrand"];
            DeviceModel = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModel"];
            DeviceModelBoot = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModelBoot"];
            DeviceModelIdentifier = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModelIdentifier"];
            FirmwareBrand = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareBrand"];
            FirmwareFingerprint = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareFingerprint"];
            FirmwareTags = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareTags"];
            FirmwareType = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareType"];
            HardwareManufacturer = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["HardwareManufacturer"];
            HardwareModel = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["HardwareModel"];

            setupdevicedone = true;
        }
        #endregion

        public byte[] sessionhash_array = null;

        /// <summary>
        /// This has to be reviewed when it is used
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authToken"></param>
        /// <param name="authType"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="altitude"></param>
        /// <param name="settings"></param>
        /// <param name="authTicket"></param>
        public RequestBuilder(Client client, string authToken, AuthType authType, double latitude, double longitude, double altitude, ISettings settings, AuthTicket authTicket = null)
        {
            if (!setupdevicedone)
            {
                setUpDevice();
            }

            _client = client;
            _authToken = authToken;
            _authType = authType;
            _latitude = latitude;
            _longitude = longitude;
            _altitude = altitude;
            _authTicket = authTicket;
            _settings = settings;

            // Add small variance to speed.
            _speed = _speed + ((float)Math.Round(GenRandom(-1, 1), 7));

            if (_settings.SessionHash == null)
            {
                GenerateNewHash();
            }
        }

        /// <summary>
        /// Assign the hash for this session
        /// </summary>
        private ByteString SessionHash
        {
            get { return _settings.SessionHash; }
            set { _settings.SessionHash = value; }
        }

        public void GenerateNewHash()
        {
            var hashBytes = new byte[16];

            RandomDevice.NextBytes(hashBytes);
            SessionHash = ByteString.CopyFrom(hashBytes);
        }

        public uint RequestCount { get; private set; } = 1;

        private readonly Random _random = new Random(Environment.TickCount);

        private long PositiveRandom()
        {
            long ret = _random.Next() | (_random.Next() << 32);
            // lrand48 ensures it's never < 0
            // So do the same
            if (ret < 0)
                ret = -ret;
            return ret;
        }

        private void IncrementRequestCount()
        {
            // Request counts on android jump more than 1 at a time according to logs
            // They are fully sequential on iOS though
            // So mimic that same behavior here.
            if (_client.Platform == Platform.Android)
                RequestCount += (uint)_random.Next(2, 15);
            else if (_client.Platform == Platform.Ios)
                RequestCount++;
        }

        private ulong GetNextRequestId()
        {
            if (RequestCount == 1)
            {
                IncrementRequestCount();
                if (_client.Platform == Platform.Android)
                {
                    // lrand48 is "broken" in that the first run of it will return a static value.
                    // So the first time we send a request, we need to match that initial value. 
                    // Note: On android srand(4) is called in .init_array which seeds the initial value.
                    return 0x53B77E48000000B0;
                }
                if (_client.Platform == Platform.Ios)
                {
                    // Same as lrand48, iOS uses "rand()" without a pre-seed which always gives the same first value.
                    return 0x41A700000002;
                }
            }

            // Note that the API expects a "positive" random value here. (At least on Android it does due to lrand48 implementation details)
            // So we'll just use the same for iOS since it doesn't hurt, and means less code required.
            ulong r = (((ulong)PositiveRandom() | ((RequestCount + 1) >> 31)) << 32) | (RequestCount + 1);
            IncrementRequestCount();
            return r;
        }
        //private RequestEnvelope.Types.PlatformRequest GenerateSignature(IEnumerable<IMessage> requests)
        /// <summary>
        /// EB Check IMessage
        /// </summary>
        /// <param name="requestEnvelope"></param>
        /// <returns></returns>
        /// Also pogolib does
        internal async Task<PlatformRequest> GenerateSignatureAsync(RequestEnvelope requestEnvelope)
        //private RequestEnvelope.Types.PlatformRequest GenerateSignature(RequestEnvelope requestEnvelope)
        {

            var timestampSinceStart = (long)(Utils.GetTime(true) - _client.StartTime);
            var locationFixes = BuildLocationFixes(requestEnvelope, timestampSinceStart);

            requestEnvelope.Accuracy = locationFixes[0].Altitude;
            requestEnvelope.MsSinceLastLocationfix = (long)locationFixes[0].TimestampSnapshot;

            //// ???
            //byte[] ticketBytes = _authTicket != null ? _authTicket.ToByteArray() : Encoding.UTF8.GetBytes(_authToken);

            //// ???
            //if (sessionhash_array == null)
            //{
            //    byte[] rByte = new byte[16];
            //    Random ra = new Random();
            //    ra.NextBytes(rByte);
            //    sessionhash_array = rByte;
            //}

            #region -- DeviceInfo
            DeviceInfo dInfo = new DeviceInfo
            {
                DeviceId = this.DeviceId,
                AndroidBoardName = this.AndroidBoardName, // might al
                AndroidBootloader = this.AndroidBootloader,
                DeviceBrand = this.DeviceBrand,
                DeviceModel = this.DeviceModel, // might als
                DeviceModelIdentifier = this.DeviceModelIdentifier,
                DeviceModelBoot = this.DeviceModelBoot,
                HardwareManufacturer = this.HardwareManufacturer,
                HardwareModel = this.HardwareModel,
                FirmwareBrand = this.FirmwareBrand,
                FirmwareTags = this.FirmwareTags,
                FirmwareType = this.FirmwareType,
                FirmwareFingerprint = this.FirmwareFingerprint
            };
            #endregion

            #region GenerateSignature
            var signature = new Signature
            {
                TimestampSinceStart = (ulong)timestampSinceStart,
                Timestamp = (ulong)Utils.GetTime(true), // true means in Ms
                //SessionHash = ByteString.CopyFrom(sessionhash_array),
                //Unknown25 = Client_5100_Unknown25, // Could change every Update
                SensorInfo =
                {
                    new SensorInfo
                    {
                        // Values are not the same used in PogoLib
                        TimestampSnapshot = (ulong)(timestampSinceStart + RandomDevice.Next(100, 250)),
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
/*                        MagneticFieldX = GenRandom(17.950439453125),
                        MagneticFieldY = GenRandom(-23.36273193359375),
                        MagneticFieldZ = GenRandom(-48.8250732421875),*/
                        MagneticFieldAccuracy = -1,
                        Status = 3
                    }
                },
                DeviceInfo = dInfo,
                LocationFix = { locationFixes },
                ActivityStatus = new ActivityStatus
                {
                    Stationary = true
                }
            };
            #endregion

            signature.SessionHash = SessionHash;
            signature.Unknown25 = Client_5100_Unknown25;

            var serializedTicket = requestEnvelope.AuthTicket != null ? requestEnvelope.AuthTicket.ToByteArray() : requestEnvelope.AuthInfo.ToByteArray();

            var locationBytes = BitConverter.GetBytes(_latitude).Reverse()
                .Concat(BitConverter.GetBytes(_longitude).Reverse())
                .Concat(BitConverter.GetBytes(locationFixes[0].Altitude).Reverse()).ToArray();

            var requestsBytes = requestEnvelope.Requests.Select(x => x.ToByteArray()).ToArray();

            //string envelopString = JsonConvert.SerializeObject(requestEnvelope);

            HashRequestContent hashRequest = new HashRequestContent()
            {
                Timestamp = signature.Timestamp,
                Latitude = requestEnvelope.Latitude,
                Longitude = requestEnvelope.Longitude,
                Altitude = requestEnvelope.Accuracy,
                AuthTicket = serializedTicket,
                SessionData = signature.SessionHash.ToByteArray(),
                Requests = new List<byte[]>(requestsBytes)                
            };

            //var hashRequestContent = new StringContent(JsonConvert.SerializeObject(hashRequest), Encoding.UTF8, "application/json"); <- Done by PokeHashHasher

            HashResponseContent responseContent;

            //TODO:Change .Result to await
            //responseContent = _client.Hasher.RequestHashesAsync(hashRequest).Result;
            responseContent = await _client.Hasher.RequestHashesAsync(hashRequest).ConfigureAwait(false);

            if (_client.Settings.EnableVerboseLogging) 
                Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "[SIG] Hasher Server Info [[" + hashRequest.Requests.Count + "]" + String.Join("|", requestEnvelope.Requests.Select(t => t.RequestType).ToList()) + "] "
                    +": LocAuthHash: " + responseContent.LocationAuthHash + " LocHash: " + responseContent.LocationHash, LogLevel.Debug);

            signature.LocationHash1 = unchecked((int)responseContent.LocationAuthHash);
            signature.LocationHash2 = unchecked((int)responseContent.LocationHash);

            signature.RequestHash.AddRange(responseContent.RequestHashes.Select(x => (ulong) x).ToArray());

            var encryptedSignature = new PlatformRequest
            {
                Type = PlatformRequestType.SendEncryptedSignature,
                RequestMessage = new SendEncryptedSignatureRequest
                {
                    EncryptedSignature = ByteString.CopyFrom(PCryptPokeHash.Encrypt(signature.ToByteArray(), (uint)timestampSinceStart)) // Use new PCryptPokeHash
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
        private List<LocationFix> BuildLocationFixes(RequestEnvelope requestEnvelope, long timestampSinceStart)
        {
            var locationFixes = new List<LocationFix>();
            TRandom Random = new TRandom();

            if (requestEnvelope.Requests.Count == 0 || requestEnvelope.Requests[0] == null)
                return locationFixes;

            var providerCount = Random.Next(4, 10);

            for (var i = 0; i < providerCount; i++)
            {
                var timestampSnapshot = timestampSinceStart + (150 * (i + 1) + Random.Next(250 * (i + 1) - 150 * (i + 1)));
                if (timestampSnapshot >= timestampSinceStart)
                {
                    if (locationFixes.Count != 0) break;

                    timestampSnapshot = timestampSinceStart - Random.Next(20, 50);

                    if (timestampSnapshot < 0) timestampSnapshot = 0;
                }

                locationFixes.Insert(0, new LocationFix
                {
                    TimestampSnapshot = (ulong)timestampSnapshot,
                    Latitude = LocationUtil.OffsetLatitudeLongitude(_latitude, Random.Next(100) + 10),
                    Longitude = LocationUtil.OffsetLatitudeLongitude(_longitude, Random.Next(100) + 10),
                    HorizontalAccuracy = (float)Random.NextDouble(5.0, 25.0),
                    VerticalAccuracy = (float)Random.NextDouble(5.0, 25.0),
                    Altitude = (float)Random.NextDouble(10.0, 30.0),
                    Provider = "fused",
                    ProviderStatus = 3,
                    LocationType = 1,
                    // Speed = ?,
                    Course = -1,
                    // Floor = 0
                });
            }

            return locationFixes;
        }

        internal class LocationUtil
        {
            public static float OffsetLatitudeLongitude(double lat, double ran)
            {
                const int round = 6378137;
                var dl = ran / (round * Math.Cos(Math.PI * lat / 180));

                return (float)(lat + dl * 180 / Math.PI);
            }
        }

        public async Task<RequestEnvelope> GetRequestEnvelope(Request[] customRequests, bool firstRequest = false)
        //public RequestEnvelope GetRequestEnvelope(Request[] customRequests, bool firstRequest = false)
        {
            TRandom TRandomDevice = new TRandom();
            
            var e = new RequestEnvelope
            {
                StatusCode = 2,                 //1
                RequestId = GetNextRequestId(), //3
                Requests = { customRequests },  //4
                Latitude = _latitude,           //7
                Longitude = _longitude,         //8
                Accuracy = _altitude,           //9
                AuthTicket = _authTicket,       //11
                MsSinceLastLocationfix = (long)TRandomDevice.Triangular(300, 30000, 10000) //12
            };

            if (_authTicket != null && !firstRequest)
            {
                e.AuthTicket = _authTicket;
                e.PlatformRequests.Add(await GenerateSignatureAsync(e).ConfigureAwait(false));
            }
            else
            {
                e.AuthInfo = new RequestEnvelope.Types.AuthInfo
                {
                    Provider = _authType == AuthType.Google ? "google" : "ptc",
                    Token = new RequestEnvelope.Types.AuthInfo.Types.JWT
                    {
                        Contents = _authToken,
                        Unknown2 = _token2
                    }
                };
            }
            return e;
        }

        //public void TraceMessage(string message,
        //[CallerMemberName] string memberName = "",
        //[CallerFilePath] string sourceFilePath = "",
        //[CallerLineNumber] int sourceLineNumber = 0)
        //{
        //    Logger.ColoredConsoleWrite(ConsoleColor.Cyan, "[ENV] Envelope -> Generate SIG" + memberName +sourceFilePath+sourceLineNumber, LogLevel.Debug);

        //    //Trace.WriteLine("message: " + message);
        //    //Trace.WriteLine("member name: " + memberName);
        //    //Trace.WriteLine("source file path: " + sourceFilePath);
        //    //Trace.WriteLine("source line number: " + sourceLineNumber);
        //}

        public async Task<RequestEnvelope> GetRequestEnvelope(RequestType type, IMessage message)
        //public RequestEnvelope GetRequestEnvelope(RequestType type, IMessage message)
        {
            return await GetRequestEnvelope(new Request[] { new Request
            //return GetRequestEnvelope(new Request[] { new Request
            {
                RequestType = type,
                RequestMessage = message.ToByteString()
            } }).ConfigureAwait(false);

        }
        private static readonly Random RandomDevice = new Random();

        public static double GenRandom(double num)
        {
            var randomFactor = 0.3f;
            var randomMin = (num * (1 - randomFactor));
            var randomMax = (num * (1 + randomFactor));
            var randomizedDelay = RandomDevice.NextDouble() * (randomMax - randomMin) + randomMin; ;
            return randomizedDelay; ;
        }

        public static double GenRandom(double min, double max)
        {
            Random r = new Random();
            return r.NextDouble() * (max - min) + min;
        }

        private string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789")
        {
            var outOfRange = Byte.MaxValue + 1 - (Byte.MaxValue + 1) % alphabet.Length;

            return string.Concat(
                Enumerable
                    .Repeat(0, Int32.MaxValue)
                    .Select(e => this.RandomByte())
                    .Where(randomByte => randomByte < outOfRange)
                    .Take(length)
                    .Select(randomByte => alphabet[randomByte % alphabet.Length])
            );
        }

        private byte RandomByte()
        {
            using (var randomizationProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[1];
                randomizationProvider.GetBytes(randomBytes);
                return randomBytes.Single();
            }
        }
    }
}
