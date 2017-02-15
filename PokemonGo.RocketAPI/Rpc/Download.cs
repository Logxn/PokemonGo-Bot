﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokemonGo.RocketAPI.Helpers;
using POGOProtos.Enums;
using POGOProtos.Networking.Requests;
using POGOProtos.Networking.Requests.Messages;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI.Rpc
{
    public class Download : BaseRpc
    {
        public Download(Client client) : base(client)
        {
        }
        public async Task<DownloadSettingsResponse> GetSettings()
        {
            var message = new DownloadSettingsMessage
            {
                Hash = "b8fa9757195897aae92c53dbcf8a60fb3d86d745"
            };
            
            return  PostProtoPayload<Request, DownloadSettingsResponse>(RequestType.DownloadSettings, message);
        }

        public async Task<DownloadItemTemplatesResponse> GetItemTemplates()
        {
            return  PostProtoPayload<Request, DownloadItemTemplatesResponse>(RequestType.DownloadItemTemplates, new DownloadItemTemplatesMessage());
        }

        public async Task<DownloadRemoteConfigVersionResponse> GetRemoteConfigVersion(uint appVersion, string deviceManufacturer, string deviceModel, string locale, Platform platform)
        {
            return  PostProtoPayload<Request, DownloadRemoteConfigVersionResponse>(RequestType.DownloadRemoteConfigVersion, new DownloadRemoteConfigVersionMessage()
            {
                AppVersion = appVersion,
                DeviceManufacturer = deviceManufacturer,
                DeviceModel = deviceModel,
                Locale = locale,
                Platform = platform
            });
        }

        public async Task<GetAssetDigestResponse> GetAssetDigest(uint appVersion, string deviceManufacturer, string deviceModel, string locale, Platform platform)
        {
            return  PostProtoPayload<Request, GetAssetDigestResponse>(RequestType.GetAssetDigest, new GetAssetDigestMessage()
            {
                AppVersion = appVersion,
                DeviceManufacturer = deviceManufacturer,
                DeviceModel = deviceModel,
                Locale = locale,
                Platform = platform
            });
        }

        public async Task<GetDownloadUrlsResponse> GetDownloadUrls(IEnumerable<string> assetIds)
        {
            return  PostProtoPayload<Request, GetDownloadUrlsResponse>(RequestType.GetDownloadUrls, new GetDownloadUrlsMessage()
            {
                AssetId = { assetIds }
            });
        }

    }
}
