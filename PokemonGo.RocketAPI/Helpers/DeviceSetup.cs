using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static POGOProtos.Networking.Envelopes.Signature.Types;

namespace PokemonGo.RocketAPI.Helpers
{
    class DeviceSetup
    {
        // Reads last setup device (a file generated on last run)
        public static string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Device");
        public static string deviceinfo = Path.Combine(path, "DeviceInfo.txt");

        public DeviceInfo setUpDevice()
        {
            DeviceInfo _device = new DeviceInfo();

            string[] arrLine ={"lg-optimus-g",""};

            if (File.Exists(deviceinfo))
                File.ReadAllLines(deviceinfo);

            string DevicePackageName = arrLine[0].ToString();
            // Read DeviceID of File
            if (arrLine[1].ToString() != " ")
            {
                _device.DeviceId = arrLine[1].ToString();
            }
            else
            {
                _device.DeviceId = GenerateRandomDeviceId();
                // Save to file
                string[] b = { DevicePackageName, _device.DeviceId };

                File.WriteAllLines(deviceinfo, b);
            }

            // Setuprest
            _device.AndroidBoardName = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["AndroidBoardName"];
            _device.AndroidBootloader = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["AndroidBootloader"];
            _device.DeviceBrand = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceBrand"];
            _device.DeviceModel = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModel"];
            _device.DeviceModelBoot = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModelBoot"];
            _device.DeviceModelIdentifier = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["DeviceModelIdentifier"];
            _device.FirmwareBrand = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareBrand"];
            _device.FirmwareFingerprint = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareFingerprint"];
            _device.FirmwareTags = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareTags"];
            _device.FirmwareType = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["FirmwareType"];
            _device.HardwareManufacturer = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["HardwareManufacturer"];
            _device.HardwareModel = DeviceInfoHelper.DeviceInfoSets[DevicePackageName]["HardwareModel"];

            return _device;
            //setupdevicedone = true;
        }

        public static string BytesToHex(byte[] bytes)
        {
            char[] hexArray = "0123456789abcdef".ToCharArray();
            char[] hexChars = new char[bytes.Length * 2];
            for (int index = 0; index < bytes.Length; index++)
            {
                int var = bytes[index] & 0xFF;
                hexChars[index * 2] = hexArray[(int)((uint)var >> 4)];
                hexChars[index * 2 + 1] = hexArray[var & 0x0F];
            }
            return new string(hexChars).ToLower();
        }

        private static string GenerateRandomDeviceId(long numBytes = 16)
        {
            var bytes = new byte[numBytes];
            new Random().NextBytes(bytes);
            return BytesToHex(bytes);
        }

        public static DeviceInfo GetRandomIosDevice()
        {
            DeviceInfo deviceInfo = new DeviceInfo();

            // iOS device id (UDID) are 16 bytes long.
            var bytes = new byte[16];
            new Random().NextBytes(bytes);
            var deviceId = BytesToHex(bytes);

            deviceInfo.DeviceId = deviceId;
            deviceInfo.FirmwareType = DeviceInfoHelper.IosVersions[new Random().Next(DeviceInfoHelper.IosVersions.Length)];
            string[] device = DeviceInfoHelper.IosDeviceInfo[new Random().Next(DeviceInfoHelper.IosDeviceInfo.Length)];
            deviceInfo.DeviceModelBoot = device[0];
            deviceInfo.DeviceModel = device[1];

            deviceInfo.HardwareModel = device[2];
            deviceInfo.FirmwareBrand = "iPhone OS";
            deviceInfo.DeviceBrand = "Apple";
            deviceInfo.HardwareManufacturer = "Apple";

            return deviceInfo;
        }

    }
}
