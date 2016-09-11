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
        static private readonly Stopwatch _internalWatch = new Stopwatch();

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
                string[] b = new string[] { DevicePackageName, DeviceId };

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


        public RequestBuilder(string authToken, AuthType authType, double latitude, double longitude, double altitude,
            AuthTicket authTicket = null)
        {

            if (!setupdevicedone)
            {
                setUpDevice();
            }

            _authToken = authToken;
            _authType = authType;
            _latitude = latitude;
            _longitude = longitude;
            _altitude = altitude;
            _authTicket = authTicket;
            if (!_internalWatch.IsRunning)
                _internalWatch.Start();
        }
        private RequestEnvelope.Types.PlatformRequest GenerateSignature(IEnumerable<IMessage> requests)
        {
            var sig = new POGOProtos.Networking.Envelopes.Signature();
            sig.TimestampSinceStart = (ulong)_internalWatch.ElapsedMilliseconds;
            sig.Timestamp = (ulong)DateTime.UtcNow.ToUnixTime();
            sig.SensorInfo = new POGOProtos.Networking.Envelopes.Signature.Types.SensorInfo()
            {
                GravityZ = GenRandom(9.8),
                GravityX = GenRandom(0.02),
                GravityY = GenRandom(0.3),
                TimestampSnapshot = (ulong)_internalWatch.ElapsedMilliseconds - 230,
                LinearAccelerationX = GenRandom(0.12271042913198471),
                LinearAccelerationY = GenRandom(-0.015570580959320068),
                LinearAccelerationZ = GenRandom(0.010850906372070313),
                MagneticFieldX = GenRandom(17.950439453125),
                MagneticFieldY = GenRandom(-23.36273193359375),
                MagneticFieldZ = GenRandom(-48.8250732421875),
                RotationVectorX = GenRandom(-0.0120010357350111),
                RotationVectorY = GenRandom(-0.04214850440621376),
                RotationVectorZ = GenRandom(0.94571763277053833),
                GyroscopeRawX = GenRandom(7.62939453125e-005),
                GyroscopeRawY = GenRandom(-0.00054931640625),
                GyroscopeRawZ = GenRandom(0.0024566650390625),
                AccelerometerAxes = 3
            };
            sig.DeviceInfo = new POGOProtos.Networking.Envelopes.Signature.Types.DeviceInfo()
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
            Random r = new Random();
            int accuracy = r.Next(15, 50);
            sig.LocationFix.Add(new POGOProtos.Networking.Envelopes.Signature.Types.LocationFix()
            {
                Provider = "gps",

                //Unk4 = 120,
                TimestampSnapshot = (ulong)_internalWatch.ElapsedMilliseconds - 200,
                Latitude = (float)_latitude,
                Longitude = (float)_longitude,
                Altitude = (float)_altitude,
                HorizontalAccuracy = accuracy,        // Genauigkeit von GPS undso
                ProviderStatus = 3,
                Floor = 3,
                LocationType = 1 
            });
             
             
            //Compute 10
            var x = new System.Data.HashFunction.xxHash(32, 0x1B845238);
            var firstHash = BitConverter.ToUInt32(x.ComputeHash(_authTicket.ToByteArray()), 0);
            x = new System.Data.HashFunction.xxHash(32, firstHash);
            var locationBytes = BitConverter.GetBytes(_latitude).Reverse()
                .Concat(BitConverter.GetBytes(_longitude).Reverse())
                .Concat(BitConverter.GetBytes(_altitude).Reverse()).ToArray();
            sig.LocationHash1 = BitConverter.ToUInt32(x.ComputeHash(locationBytes), 0);
            //Compute 20
            x = new System.Data.HashFunction.xxHash(32, 0x1B845238);
            sig.LocationHash2 = BitConverter.ToUInt32(x.ComputeHash(locationBytes), 0);
            //Compute 24
            x = new System.Data.HashFunction.xxHash(64, 0x1B845238);
            var seed = BitConverter.ToUInt64(x.ComputeHash(_authTicket.ToByteArray()), 0);
            x = new System.Data.HashFunction.xxHash(64, seed);

            foreach (var req in requests)
                sig.RequestHash.Add(BitConverter.ToUInt64(x.ComputeHash(req.ToByteArray()), 0));
           
            // NEW Random Byte Array every session.
            if (sessionhash_array == null)
            {
                byte[] rByte = new byte[16];
                r.NextBytes(rByte);
                sessionhash_array = rByte;
            }
            sig.SessionHash = ByteString.CopyFrom(sessionhash_array);

            sig.Unknown25 = -8537042734809897855; // Generated via xxHash64("\"b8fa9757195897aae92c53dbcf8a60fb3d86d745\"".ToByteArray(), 0x88533787) | 0.33 Version

            var iv = new byte[32];
            new Random().NextBytes(iv);

            var platformRequest = new RequestEnvelope.Types.PlatformRequest()
            {
                Type = PlatformRequestType.SendEncryptedSignature,
                RequestMessage = new SendEncryptedSignatureRequest()
                {
                    EncryptedSignature = ByteString.CopyFrom(EncryptionHelper.Encrypt(sig.ToByteArray(), iv)),
                }.ToByteString(),
            };

            return platformRequest;
        }

        public RequestEnvelope GetRequestEnvelope(params Request[] customRequests)
        {
            var e = new RequestEnvelope
            {
                StatusCode = 2, //1

                RequestId = 1469378659230941192, //3
                Requests = { customRequests }, //4
                //Unknown6 = , //6
                Latitude = _latitude, //7
                Longitude = _longitude, //8
                Accuracy = _altitude, //9
                AuthTicket = _authTicket, //11
                MsSinceLastLocationfix = 989 //12
                
            };

            e.PlatformRequests.Add(GenerateSignature(customRequests));
            return e;
        }

        public RequestEnvelope GetInitialRequestEnvelope(params Request[] customRequests)
        {
            var e = new RequestEnvelope
            {
                StatusCode = 2, //1

                RequestId = 1469378659230941192, //3
                Requests = { customRequests }, //4

                //Unknown6 = , //6
                Latitude = _latitude, //7
                Longitude = _longitude, //8
                Accuracy = _altitude, //9
                AuthInfo = new POGOProtos.Networking.Envelopes.RequestEnvelope.Types.AuthInfo
                {
                    Provider = _authType == AuthType.Google ? "google" : "ptc",
                    Token = new POGOProtos.Networking.Envelopes.RequestEnvelope.Types.AuthInfo.Types.JWT
                    {
                        Contents = _authToken,
                        Unknown2 = 14
                    }
                }, //10
                MsSinceLastLocationfix = 989 //12
            };
            return e;
        }
        
        public RequestEnvelope GetRequestEnvelope(RequestType type, IMessage message)
        {
            return GetRequestEnvelope(new Request()
            {
                RequestType = type,
                RequestMessage = message.ToByteString()
            });

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
