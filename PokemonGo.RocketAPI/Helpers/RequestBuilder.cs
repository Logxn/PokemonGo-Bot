using Google.Protobuf;
using PokemonGo.RocketAPI.Enums;
using POGOProtos.Networking;
using POGOProtos.Networking.Envelopes;
using POGOProtos.Networking.Requests;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using PokemonGo.RocketAPI.Extensions;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Forms;
using POGOProtos.Networking.Platform;
using POGOProtos.Networking.Platform.Requests;
using Troschuetz.Random;
using POGOProtos.Enums;
using System.Text;

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
        private readonly Crypt _crypt;
        private readonly float _speed;
        private readonly ISettings _settings;
        private static long Client_4500_Unknown25 = -1553869577012279119;


        /// Device Shit
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

        public byte[] sessionhash_array = null;



        public bool setupdevicedone = false;

        public void setUpDevice()
        { 
            string[] arrLine = File.ReadAllLines(deviceinfo); 

            string DevicePackageName = arrLine[0].ToString(); 
            // Read DeviceID of File
            if (arrLine[1].ToString() != " ")
            {
                DeviceId = arrLine[1].ToString();
            } else
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


        public RequestBuilder(Client client, string authToken, AuthType authType, double latitude, double longitude, double altitude, ISettings settings,
            AuthTicket authTicket = null)
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

            if (_crypt == null)
                _crypt = new Crypt();
            
        }

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
        private RequestEnvelope.Types.PlatformRequest GenerateSignature(IEnumerable<IMessage> requests)
        {
            byte[] ticketBytes = _authTicket != null ? _authTicket.ToByteArray() : Encoding.UTF8.GetBytes(_authToken);

            if (sessionhash_array == null)
            {
                byte[] rByte = new byte[16];
                Random ra = new Random();
                ra.NextBytes(rByte);
                sessionhash_array = rByte;
            }
            // Device
            Signature.Types.DeviceInfo dInfo = new Signature.Types.DeviceInfo
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

            var sig = new Signature
            {
                SessionHash = ByteString.CopyFrom(sessionhash_array),
                Unknown25 = Client_4500_Unknown25, // Could change every Update
                Timestamp = (ulong)Utils.GetTime(true),
                TimestampSinceStart = (ulong)(Utils.GetTime(true) - _client.StartTime),
                LocationHash1 = (int)Utils.GenLocation1(ticketBytes, _latitude, _longitude, _altitude),
                LocationHash2 = (int)Utils.GenLocation2(_latitude, _longitude, _altitude),
                DeviceInfo = dInfo
            };

            sig.SensorInfo.Add(new POGOProtos.Networking.Envelopes.Signature.Types.SensorInfo
            {
                GravityZ = GenRandom(9.8),
                GravityX = GenRandom(0.02),
                GravityY = GenRandom(0.3),
                TimestampSnapshot = (ulong)(Utils.GetTime(true) - _client.StartTime - RandomDevice.Next(100, 500)),
                LinearAccelerationX = GenRandom(0.12271042913198471),
                LinearAccelerationY = GenRandom(-0.015570580959320068),
                LinearAccelerationZ = GenRandom(0.010850906372070313),
                MagneticFieldX = GenRandom(17.950439453125),
                MagneticFieldY = GenRandom(-23.36273193359375),
                MagneticFieldZ = GenRandom(-48.8250732421875),
                RotationRateX = GenRandom(-0.0120010357350111),
                RotationRateY = GenRandom(-0.04214850440621376),
                RotationRateZ = GenRandom(0.94571763277053833),
                AttitudePitch = GenRandom(-47.149471283, 61.8397789001),
                AttitudeYaw = GenRandom(-47.149471283, 61.8397789001),
                AttitudeRoll = GenRandom(-47.149471283, 5),
                MagneticFieldAccuracy = -1,
                Status = 3
            });

            Random r = new Random();
            int accuracy = r.Next(15, 50);

            Signature.Types.LocationFix locationFix = new Signature.Types.LocationFix
            {
                Provider = "network",
                TimestampSnapshot = (ulong)(Utils.GetTime(true) - _client.StartTime - RandomDevice.Next(100, 300)),
                Latitude = (float)_latitude,
                Longitude = (float)_longitude,
                Altitude = (float)_altitude,
                HorizontalAccuracy = accuracy,        // Genauigkeit von GPS undso
                ProviderStatus = 3,
                // Unnötig => Floor = 3,
                LocationType = 1
            };

            foreach (var request in requests)
                sig.RequestHash.Add(Utils.GenRequestHash(ticketBytes, request.ToByteArray()));


            var encryptedSig = new RequestEnvelope.Types.PlatformRequest
            {
                Type = PlatformRequestType.SendEncryptedSignature,
                RequestMessage = new SendEncryptedSignatureRequest
                {
                    EncryptedSignature = ByteString.CopyFrom(PCrypt.Encrypt(sig.ToByteArray(), (uint)_client.StartTime))
                }.ToByteString()
            };

            return encryptedSig;
        }

        public RequestEnvelope GetRequestEnvelope(Request[] customRequests, bool firstRequest = false)
        {

            TRandom TRandomDevice = new TRandom();

            var e = new RequestEnvelope
            {
                StatusCode = 2, //1
                RequestId = GetNextRequestId(), //3
                Requests = { customRequests }, //4
                Latitude = _latitude, //7
                Longitude = _longitude, //8
                Accuracy = _altitude, //9
                AuthTicket = _authTicket, //11
                MsSinceLastLocationfix = (long)TRandomDevice.Triangular(300,30000, 10000) //12
                
            };

            if (_authTicket != null && !firstRequest)
            {
                e.AuthTicket = _authTicket;
                e.PlatformRequests.Add(GenerateSignature(customRequests));
            } else
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

        public RequestEnvelope GetRequestEnvelope(RequestType type, IMessage message)
        {
            return GetRequestEnvelope(new Request[] { new Request
            {
                RequestType = type,
                RequestMessage = message.ToByteString()
            } });

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
