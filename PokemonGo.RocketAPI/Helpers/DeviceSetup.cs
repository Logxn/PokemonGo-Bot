/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 17/02/2017
 * Time: 22:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using POGOProtos.Networking.Envelopes;

namespace PokemonGo.RocketAPI.Helpers
{
    /// <summary>
    /// Description of DeviceData.
    /// </summary>
    public class DeviceSetup
    {
        public static DeviceInfoEx SelectedDevice {
            get;
            set;
        }

        public static void SelectDevice(string deviceTradeName, string deviceID, string filename)
        {
            var instance = new DeviceSetup(filename);
            SelectedDevice = instance.FindDevice(deviceTradeName);
            if (SelectedDevice != null){
                SelectedDevice.DeviceInfo.DeviceId = deviceID;
            }
        }
        public List<DeviceInfoEx> data{get;set;}

        public DeviceSetup(string filename)
        {
            LoadFromFile(filename);
        }

        public class DeviceInfoEx
        {
            public string Tradename {get;set;}
            public string OSType {get;set;}
            public Signature.Types.DeviceInfo DeviceInfo {get;set;}

        }
        public void LoadFromFile(string filename){
            if (File.Exists(filename)) {
                var strJSON = File.ReadAllText(filename);
                data = JsonConvert.DeserializeObject<List<DeviceInfoEx>>(strJSON);
            }else
                LoadDefaultDevices();
        }

        public void SaveToFile(string filename)
        {
            var strJSON = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filename, strJSON);
        }

        public void LoadDefaultDevices()
        {
            data = new List<DeviceInfoEx>();
            DeviceInfoEx dev =  null; 
            
            // Android
            foreach (KeyValuePair<string, Dictionary<string, string>> entry in DeviceInfoHelper.DeviceInfoSets ) {
                dev = new DeviceInfoEx();
                dev.Tradename = entry.Key;
                dev.OSType = "Android";
                dev.DeviceInfo = new Signature.Types.DeviceInfo();
                dev.DeviceInfo.DeviceId = RandomDeviceId();
                dev.DeviceInfo.AndroidBoardName = entry.Value["AndroidBoardName"];
                dev.DeviceInfo.AndroidBootloader = entry.Value["AndroidBootloader"];
                dev.DeviceInfo.DeviceBrand = entry.Value["DeviceBrand"];
                dev.DeviceInfo.DeviceModel = entry.Value["DeviceModel"];
                dev.DeviceInfo.DeviceModelBoot = entry.Value["DeviceModelBoot"];
                dev.DeviceInfo.FirmwareBrand = entry.Value["FirmwareBrand"];
                dev.DeviceInfo.FirmwareFingerprint = entry.Value["FirmwareFingerprint"];
                dev.DeviceInfo.FirmwareTags = entry.Value["FirmwareTags"];
                dev.DeviceInfo.FirmwareType = entry.Value["FirmwareType"];
                dev.DeviceInfo.HardwareManufacturer = entry.Value["HardwareManufacturer"];
                dev.DeviceInfo.HardwareModel = entry.Value["HardwareModel"];
                data.Add(dev);
            }
            
            // IOS
            dev = new DeviceInfoEx();
            dev.Tradename = "iPhone 7";
            dev.OSType = "iOS";
            dev.DeviceInfo = new Signature.Types.DeviceInfo();
            dev.DeviceInfo.DeviceId = RandomDeviceId();
            dev.DeviceInfo.DeviceBrand = "Apple";
            dev.DeviceInfo.DeviceModel = "iPhone";
            dev.DeviceInfo.DeviceModelBoot = "iPhone7,2";
            dev.DeviceInfo.FirmwareBrand = "iPhone OS";
            dev.DeviceInfo.FirmwareType = "9.3.3";
            dev.DeviceInfo.HardwareModel = "N61AP";
            dev.DeviceInfo.HardwareManufacturer = "Apple";
            data.Add(dev);
        }

        public static string RandomDeviceId(int numBytes = 16)
        {
            var r = new Random();
            var str = "";
            for (var i = 0; i<numBytes;i++)
                str += r.Next(1,16).ToString("x");
            return str;
        }

        public static Signature.Types.DeviceInfo GetRandomIosDevice()
        {
            var deviceInfo = new Signature.Types.DeviceInfo();

            deviceInfo.DeviceId = RandomDeviceId();
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

        public DeviceInfoEx FindDevice( string tradename){
            foreach (var element in data) {
               if (element.Tradename == tradename)
                   return element;
            }
            return null;
        }

    }
}
